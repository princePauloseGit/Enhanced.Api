using Enhanced.Models.PayPalData;
using Enhanced.Models.Shared;
using static Enhanced.Models.PayPalData.PayPalEnum;
using static Enhanced.Models.PayPalData.PayPalSettlementReport;
using static Enhanced.Models.Shared.CommonEnum;

namespace Enhanced.Services.PayPal
{
    public class PayPalReportManager : IPayPalReportManager
    {
        private readonly IPayPalReportService _payPalReportService;

        public PayPalReportManager(IPayPalReportService payPalReportService)
        {
            _payPalReportService = payPalReportService;
        }

        /// <summary>
        /// Get PayPal Settlement Reports
        /// </summary>
        /// <param name="payPalSFTP"></param>
        /// <returns></returns>
        public PayPalSettlementBatch GetPayPalSettlementReports(ParameterPayPalSFTP payPalSFTP, List<string> bcReportDocumentIds)
        {
            var reportDocumentIdsToUpdate = new List<ReportDocumentDetails>();
            var settlementBatches = new List<PayPalSettlement>();
            var errorLogs = new List<ErrorLog>();
            var payPalReportDocumentIds = new List<string>();

            var (paths, errors) = _payPalReportService.DownloadSFTPFiles(payPalSFTP, bcReportDocumentIds);

            if (errors != null && errors.Any())
            {
                errorLogs.AddRange(errors);
            }

            if (paths != null)
            {
                foreach (var path in paths)
                {
                    try
                    {
                        var report = new PayPalSettlementReport(path);

                        if (report.PayPalSettlementRows != null && report.PayPalSettlementRows.Any())
                        {
                            var payPalSettlements = GetSettlementBatch(report.PayPalSettlementRows!);
                            var settlementReportId = ((DateTime)(payPalSettlements.FirstOrDefault()?.PostingDate)!).ToString(Constant.DATETIME_FORMAT_SETTLEMENT);

                            if (!bcReportDocumentIds.Any(x => x == GetPPSettlementId(settlementReportId)))
                            {
                                string message = string.Concat("Total Rows: ", report.PayPalSettlementRows.Count, " Posting Date: ", settlementReportId);
                                errorLogs.Add(new ErrorLog(Marketplace.PayPal, Sevarity.Information, "PayPal Payment Report", GetPPSettlementId(settlementReportId), Priority.Low, message));

                                settlementBatches.Add(new PayPalSettlement
                                {
                                    SettlementId = GetPPSettlementId(settlementReportId),
                                    PayPalSettlements = payPalSettlements,
                                });

                                reportDocumentIdsToUpdate.Add(new ReportDocumentDetails
                                {
                                    ReportDocumentId = GetPPSettlementId(settlementReportId),
                                    CanArchived = false,
                                });
                            }
                            else
                            {
                                payPalReportDocumentIds.Add(GetPPSettlementId(settlementReportId));
                            }
                        }
                        else
                        {
                            errorLogs.Add(new ErrorLog(Marketplace.PayPal, Sevarity.Warning, "PayPal Payment Report", report?.PostingDate!.Value.ToString("dd/MM/yyyy"), Priority.High, "No Records", "No Records"));
                        }
                    }
                    catch (Exception ex)
                    {
                        errorLogs.Add(new ErrorLog(Marketplace.PayPal, Sevarity.Error, "PayPal Payment Report", "", Priority.High, ex.Message, ex.StackTrace));
                        continue;
                    }
                    finally
                    {
                        CommonHelper.DeleteFile(path);
                    }
                }

                if (payPalReportDocumentIds?.Any() == true)
                {
                    foreach (var reportDocumentId in bcReportDocumentIds)
                    {
                        if (!payPalReportDocumentIds.Any(x => x == reportDocumentId))
                        {
                            reportDocumentIdsToUpdate.Add(new ReportDocumentDetails
                            {
                                ReportDocumentId = reportDocumentId,
                                CanArchived = true,
                            });
                        }
                    }
                }
            }            

            var settlementBatch = new PayPalSettlementBatch
            {
                PayPalSettlements = settlementBatches,
                ReportDocumentDetails = reportDocumentIdsToUpdate,
                ErrorLogs = errorLogs,
            };

            return settlementBatch;
        }

        /// <summary>
        /// Get Settlement Batch
        /// </summary>
        /// <param name="settlements"></param>
        /// <returns></returns>
        private static List<SettlementGroup> GetSettlementBatch(List<PayPalSettlementRow> settlements)
        {
            decimal? amount = 0;
            var payPalSettlements = new List<SettlementGroup>();

            var transactionEventCodes = new List<string>
            {
                EventCode.T0004.ToString(),
                EventCode.T0006.ToString(),
                EventCode.T1107.ToString(),
                EventCode.T5000.ToString(),
                EventCode.T5001.ToString(),
            };

            var payPalCustomerPayments = GetPayPalCustomerPayment(settlements, transactionEventCodes);
            var payPalVendorPayments = GetPayPalVenodrPayment(settlements, transactionEventCodes);
            var onBuyVendorPayment = GetOnBuyVenodrPayment(settlements, EventCode.T0113.ToString());

            if (payPalCustomerPayments != null && payPalCustomerPayments.Any())
            {
                amount += payPalCustomerPayments?.Sum(sm => sm.Amount);

                payPalSettlements.AddRange(payPalCustomerPayments!);
            }

            if (payPalVendorPayments != null && payPalVendorPayments.Any())
            {
                amount += payPalVendorPayments?.Sum(sm => sm.Amount);

                payPalSettlements.AddRange(payPalVendorPayments!);
            }

            if (onBuyVendorPayment != null && onBuyVendorPayment.Amount != 0)
            {
                amount += onBuyVendorPayment?.Amount;

                payPalSettlements.Add(onBuyVendorPayment!);
            }

            var bankEntry = GetBankEntry(settlements, EventCode.T0400.ToString(), (decimal)amount!);

            if (bankEntry != null && bankEntry.Amount != 0)
            {
                payPalSettlements.Add(bankEntry);
            }

            return payPalSettlements;
        }

        /// <summary>
        /// Get PayPal Customer Payment
        /// </summary>
        /// <param name="payPalInvoiceGrp"></param>
        /// <returns></returns>
        private static List<SettlementGroup> GetPayPalCustomerPayment(List<PayPalSettlementRow> settlements, List<string> transactionEventCodes)
        {
            var payPalCustomerPayments = new List<SettlementGroup>();

            var payPalInvoiceGrp = settlements
               .Where(x => !string.IsNullOrEmpty(x.InvoiceId) && transactionEventCodes.Any(a => a == x.TransactionEventCode))
               .ToList();

            foreach (var payPalItem in payPalInvoiceGrp!)
            {
                var settlementData = new SettlementGroup
                {
                    PostingDate = payPalItem!.PostingDate,
                    DocumentType = string.Empty,
                    DocumentNumber = ((DateTime)payPalItem.PostingDate!).ToString(Constant.DATETIME_FORMAT_SETTLEMENT),
                    AccountType = AccountType.Customer.ToString(),
                    ExternalDocumentNumber = payPalItem.InvoiceId,
                    Amount = Math.Round((decimal)(payPalItem.TransactionDebitOrCredit == PaymentType.DR.ToString() ? payPalItem.GrossTransactionAmount : -payPalItem.GrossTransactionAmount)! / 100, 2),
                    ShortcutDimension = !string.IsNullOrEmpty(payPalItem.ShortcutDimension) ? payPalItem.ShortcutDimension : string.Empty,
                    PaymentLine = PaymentLine.Customer.ToString(),
                };

                if (settlementData.Amount != 0)
                {
                    payPalCustomerPayments.Add(settlementData);
                }
            }

            return payPalCustomerPayments;
        }

        /// <summary>
        /// Get PayPal Venodr Payment
        /// </summary>
        /// <param name="payPalEventCodeGrp"></param>
        /// <returns></returns>
        private static List<SettlementGroup> GetPayPalVenodrPayment(List<PayPalSettlementRow> settlements, List<string> transactionEventCodes)
        {
            var payPalVendorPayments = new List<SettlementGroup>();
            var payPalEventCodeGrp = settlements
                .Where(x => transactionEventCodes.Contains(x.TransactionEventCode!))
                .GroupBy(gp => gp.TransactionEventCode)
                .ToList();

            foreach (var payPalItem in payPalEventCodeGrp!)
            {
                var payPal = payPalItem.FirstOrDefault();

                var vendorPay = new SettlementGroup
                {
                    PostingDate = payPal!.PostingDate,
                    DocumentType = string.Empty,
                    DocumentNumber = ((DateTime)payPal.PostingDate!).ToString(Constant.DATETIME_FORMAT_SETTLEMENT),
                    AccountType = AccountType.Vendor.ToString(),
                    Amount = Math.Round((decimal)payPalItem.Sum(sm => (sm.FeeDebitOrCredit == PaymentType.DR.ToString() ? sm.FeeAmount : -sm.FeeAmount))! / 100, 2),
                    ShortcutDimension = !string.IsNullOrEmpty(payPal.ShortcutDimension) ? payPal.ShortcutDimension : string.Empty,
                    PaymentLine = PaymentLine.PayPal.ToString(),
                    ExternalDocumentNumber = string.Empty,
                };

                if (vendorPay.Amount != 0)
                {
                    payPalVendorPayments.Add(vendorPay);
                }
            }

            return payPalVendorPayments;
        }

        /// <summary>
        /// Get OnBuy Venodr Payment
        /// </summary>
        /// <param name="settlements"></param>
        /// <param name="eventCode"></param>
        /// <returns></returns>
        private static SettlementGroup GetOnBuyVenodrPayment(List<PayPalSettlementRow>? settlements, string eventCode)
        {
            var onBuySettlemets = settlements!.Where(x => x.TransactionEventCode == eventCode).ToList();

            if (onBuySettlemets != null && onBuySettlemets.Any())
            {
                var postingDate = onBuySettlemets!.FirstOrDefault()?.PostingDate;

                return new SettlementGroup
                {
                    PostingDate = postingDate,
                    DocumentType = string.Empty,
                    DocumentNumber = ((DateTime)postingDate!).ToString(Constant.DATETIME_FORMAT_SETTLEMENT),
                    AccountType = AccountType.Vendor.ToString(),
                    Amount = Math.Round((decimal)onBuySettlemets!.Sum(sm => (sm.TransactionDebitOrCredit == PaymentType.DR.ToString() ? sm.GrossTransactionAmount : -sm.GrossTransactionAmount))! / 100, 2),
                    ShortcutDimension = ShortcutDimension.OnBuy.ToString(),
                    PaymentLine = PaymentLine.OnBuy.ToString(),
                    ExternalDocumentNumber = string.Empty,
                };
            }

            return null!;
        }

        /// <summary>
        /// Get Bank Entry
        /// </summary>
        /// <param name="settlements"></param>
        /// <param name="eventCode"></param>
        /// <returns></returns>
        private static SettlementGroup GetBankEntry(List<PayPalSettlementRow>? settlements, string eventCode, decimal amount)
        {
            var payPalSettlemets = settlements!.Where(x => x.TransactionEventCode == eventCode).ToList();

            if (payPalSettlemets!= null && payPalSettlemets?.Any() == true)
            {
                var postingDate = payPalSettlemets!.FirstOrDefault()?.PostingDate;

                return new SettlementGroup
                {
                    PostingDate = postingDate,
                    DocumentType = string.Empty,
                    DocumentNumber = ((DateTime)postingDate!).ToString(Constant.DATETIME_FORMAT_SETTLEMENT),
                    AccountType = AccountType.Bank.ToString(),
                    Amount = Math.Abs(amount),
                    PaymentLine = PaymentLine.Bank.ToString(),
                    ShortcutDimension = string.Empty,
                    ExternalDocumentNumber = string.Empty
                };
            }

            return null!;
        }

        private static string GetPPSettlementId(string settlementId)
        {
            return string.Concat("PP", settlementId.Replace("-", string.Empty));
        }
    }
}
