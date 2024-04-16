using Enhanced.Models.BQData;
using Enhanced.Models.Shared;
using static Enhanced.Models.BQData.BQEnum;
using static Enhanced.Models.BQData.BQPayment;
using static Enhanced.Models.Shared.CommonEnum;

namespace Enhanced.Services.BQ
{
    public class BQOrderManager : IBQOrderManager
    {
        private readonly IBQService _bqService;

        public BQOrderManager(IBQService bQService)
        {
            _bqService = bQService;
        }

        /// <summary>
        /// Get Download BQ Payment
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="paramPaymentBQ"></param>
        /// <returns></returns>
        public async Task<BQPaymentBatch> DownloadBQPayment(ParamBQ parameter, ParamPayment paramPaymentBQ)
        {
            var paymentGroups = new List<BQPaymentGroup>();
            var reportDocumentIdsToUpdate = new List<ReportDocumentDetails>();
            var errorLogs = new List<ErrorLog>();
            var bqReportDocumentIds = new List<string>();

            var (paymentResult, errorLog) = await _bqService.DownloadBQPayment(parameter, paramPaymentBQ);

            if (errorLog != null)
            {
                errorLogs.Add(errorLog);
            }

            if (paymentResult != null && paymentResult.Any())
            {
                errorLogs.Add(new ErrorLog(Marketplace.BQ, Sevarity.Information, "Payments", "", Priority.Low, "Total Records: " + paymentResult.Count));

                var paymentsGrp = paymentResult.GroupBy(g => g.accounting_document_number).ToList();

                foreach (var payments in paymentsGrp)
                {
                    if (!paramPaymentBQ.ReportDocumentIds!.Any(x => x == CommonHelper.GetStringFromRight(payments.Key!, 10)))
                    {
                        var paymentRows = new List<BQPaymentRow>();
                        var postingDate = payments.FirstOrDefault()?.accounting_document_creation_date;

                        var bankPayment = GetBankEntry(payments!, postingDate);
                        var customerPayments = GetCustomerPayment(payments!);
                        var vendorPaymentForGL79076 = GetVendorEntryForGL79076(payments!, postingDate);
                        var vendorPaymentForGL82010 = GetVendorEntryForGL82010(payments!, postingDate);
                        var vendorPayment = GetVendorEntry(payments!, postingDate);

                        if (bankPayment != null)
                        {
                            paymentRows.Add(bankPayment);
                        }

                        if (customerPayments != null && customerPayments.Any())
                        {
                            paymentRows.AddRange(customerPayments);
                        }

                        if (vendorPaymentForGL79076 != null && vendorPaymentForGL79076.Amount != 0 && vendorPaymentForGL79076.BalVATAmount != 0)
                        {
                            paymentRows.Add(vendorPaymentForGL79076);
                        }

                        if (vendorPaymentForGL82010 != null && vendorPaymentForGL82010.Amount != 0 && vendorPaymentForGL82010.BalVATAmount != 0)
                        {
                            paymentRows.Add(vendorPaymentForGL82010);
                        }

                        if (vendorPayment != null && vendorPayment.Amount != 0)
                        {
                            paymentRows.Add(vendorPayment);
                        }

                        paymentGroups.Add(new BQPaymentGroup
                        {
                            AccountingDocumentNumber = CommonHelper.GetStringFromRight(payments.Key!, 10),
                            BQPayments = paymentRows,
                        });

                        reportDocumentIdsToUpdate.Add(new ReportDocumentDetails
                        {
                            ReportDocumentId = CommonHelper.GetStringFromRight(payments.Key!, 10),
                            CanArchived = false,
                        });

                        errorLogs.Add(new ErrorLog(Marketplace.BQ, Sevarity.Information, "Accounting Document Number", payments.Key, Priority.Low));
                    }
                    else
                    {
                        bqReportDocumentIds.Add(CommonHelper.GetStringFromRight(payments.Key!, 10));
                    }
                }

                if (bqReportDocumentIds?.Any() == true)
                {
                    foreach (var reportDocumentId in paramPaymentBQ.ReportDocumentIds!)
                    {
                        if (!bqReportDocumentIds.Any(x => x == reportDocumentId))
                        {
                            reportDocumentIdsToUpdate.Add(new ReportDocumentDetails
                            {
                                ReportDocumentId = reportDocumentId,
                                CanArchived = true,
                            });
                        }
                    }
                }                

                return new BQPaymentBatch
                {
                    Payments = paymentGroups,
                    ReportDocumentDetails = reportDocumentIdsToUpdate,
                    ErrorLogs = errorLogs,
                };
            }
            else
            {
                errorLogs.Add(new ErrorLog(Marketplace.BQ, Sevarity.Warning, "Payments", "", Priority.Low, "No Records"));

                return new BQPaymentBatch
                {
                    ErrorLogs = errorLogs,
                };
            }
        }

        /// <summary>
        /// Get Bank Entry
        /// </summary>
        /// <param name="payments"></param>
        /// <returns></returns>
        private static BQPaymentRow GetBankEntry(IGrouping<string, PaymentList> payments, DateTime? postingDate)
        {
            var grpAccountNumber = payments.Where(x => x.type!.ToUpper() == PaymentType.PAYMENT.ToString()).ToList();

            var bankRow = new BQPaymentRow
            {
                PostingDate = postingDate,
                ExternalDocumentNumber = payments.Key,
                AccountType = AccountType.Bank.ToString(),
                AccountNo = string.Empty,
                Amount = (decimal)Math.Round(grpAccountNumber.Sum(sm => sm.amount), 3),
                BalAccountNo = string.Empty,
                BalAccountType = string.Empty,
                BalVATAmount = 0,
            };

            if (bankRow.Amount < 0)
            {
                bankRow.Amount = -bankRow.Amount;
            }

            return bankRow;
        }

        /// <summary>
        /// Get Customer Payment Entry
        /// </summary>
        /// <param name="payments"></param>
        /// <returns></returns>
        private static List<BQPaymentRow> GetCustomerPayment(IGrouping<string, PaymentList> payments)
        {
            var customerEntries = new List<BQPaymentRow>();
            var postingDate = payments.FirstOrDefault()?.accounting_document_creation_date;

            var amountGrpPayments = payments.Where(x => x.type!.ToUpper() == PaymentType.ORDER_AMOUNT.ToString()).ToList();
            var refundamtGrpPayments = payments.Where(x => x.type!.ToUpper() == PaymentType.REFUND_ORDER_AMOUNT.ToString()).ToList();

            GetCustomerPaymentByType(customerEntries, amountGrpPayments, postingDate, -1);
            GetCustomerPaymentByType(customerEntries, refundamtGrpPayments, postingDate, 1);

            return customerEntries;
        }

        /// <summary>
        /// Get Customer Payment By Entry Type
        /// </summary>
        /// <param name="customerEntries"></param>
        /// <param name="orderGrpPayments"></param>
        private static void GetCustomerPaymentByType(List<BQPaymentRow> customerEntries, List<PaymentList> orderGrpPayments, DateTime? postingDate, int multiplyFactor)
        {
            var entities = orderGrpPayments.Where(x => x.entities != null)
                            .Select(s => s.entities)
                            .Where(y => y!.order != null)
                            .GroupBy(gp => gp?.order?.id).ToList();

            foreach (var entity in entities)
            {
                var amount = orderGrpPayments.Where(x => x.entities != null && x.entities?.order?.id == entity.Key).Sum(sm => sm.amount);

                switch (multiplyFactor)
                {
                    // For payment type 'ORDER_AMOUNT' amount should be Negative
                    case -1:
                        if (amount > 0)
                        {
                            amount = -amount;
                        }
                        break;

                    // For payment type 'REFUND_ORDER_AMOUNT' amount should be Positive
                    case 1:
                        if (amount < 0)
                        {
                            amount = -amount;
                        }
                        break;
                }

                customerEntries.Add(new BQPaymentRow
                {
                    PostingDate = postingDate,
                    ExternalDocumentNumber = entity.Key,
                    AccountType = AccountType.Customer.ToString(),
                    AccountNo = string.Empty,
                    Amount = (decimal)Math.Round(amount, 3),
                    BalAccountNo = string.Empty,
                    BalAccountType = string.Empty,
                    BalVATAmount = 0,
                });
            }
        }

        /// <summary>
        /// Get Vendor Entry For GL79076
        /// </summary>
        /// <param name="payments"></param>
        /// <returns></returns>
        private static BQPaymentRow GetVendorEntryForGL79076(IGrouping<string, PaymentList> payments, DateTime? postingDate)
        {
            var commissionFee = payments.Where(x => x.type!.ToUpper() == PaymentType.COMMISSION_FEE.ToString() ||
                                                    x.type!.ToUpper() == PaymentType.REFUND_COMMISSION_FEE.ToString() ||
                                                    x.type!.ToUpper() == PaymentType.COMMISSION_VAT.ToString() ||
                                                    x.type!.ToUpper() == PaymentType.REFUND_COMMISSION_VAT.ToString())
                                         .Sum(sm => sm.amount);

            var commissionVat = payments.Where(x => x.type!.ToUpper() == PaymentType.COMMISSION_VAT.ToString() ||
                                                    x.type!.ToUpper() == PaymentType.REFUND_COMMISSION_VAT.ToString())
                                        .Sum(sm => sm.amount);
            return new BQPaymentRow
            {
                PostingDate = postingDate,
                ExternalDocumentNumber = payments.Key,
                AccountType = AccountType.Vendor.ToString(),
                AccountNo = string.Empty,
                Amount = (decimal)Math.Round(commissionFee, 3),
                BalAccountNo = "79076",
                BalAccountType = "G/L Account",
                BalVATAmount = (decimal)Math.Round(-commissionVat, 3),
            };
        }

        /// <summary>
        /// Get Vendor Entry For GL82010
        /// </summary>
        /// <param name="payments"></param>
        /// <returns></returns>
        private static BQPaymentRow GetVendorEntryForGL82010(IGrouping<string, PaymentList> payments, DateTime? postingDate)
        {
            var subscriptionFee = payments.Where(x => x.type!.ToUpper() == PaymentType.SUBSCRIPTION_FEE.ToString() ||
                                                      x.type!.ToUpper() == PaymentType.SUBSCRIPTION_VAT.ToString())
                                          .Sum(sm => sm.amount);

            var subscriptionVat = payments.Where(x => x.type!.ToUpper() == PaymentType.SUBSCRIPTION_VAT.ToString()).Sum(sm => sm.amount);

            return new BQPaymentRow
            {
                PostingDate = postingDate,
                ExternalDocumentNumber = payments.Key,
                AccountType = AccountType.Vendor.ToString(),
                AccountNo = string.Empty,
                Amount = (decimal)Math.Round(subscriptionFee, 3),
                BalAccountNo = "82010",
                BalAccountType = "G/L Account",
                BalVATAmount = (decimal)Math.Round(-subscriptionVat, 3),
            };
        }

        /// <summary>
        /// Get Vendor Entry
        /// </summary>
        /// <param name="payments"></param>
        /// <returns></returns>
        private static BQPaymentRow GetVendorEntry(IGrouping<string, PaymentList> payments, DateTime? postingDate)
        {
            var amount = payments.Where(x => x.type!.ToUpper() == PaymentType.COMMISSION_VAT.ToString() ||
                                             x.type!.ToUpper() == PaymentType.COMMISSION_FEE.ToString() ||
                                             x.type!.ToUpper() == PaymentType.REFUND_COMMISSION_VAT.ToString() ||
                                             x.type!.ToUpper() == PaymentType.REFUND_COMMISSION_FEE.ToString() ||
                                             x.type!.ToUpper() == PaymentType.SUBSCRIPTION_FEE.ToString() ||
                                             x.type!.ToUpper() == PaymentType.SUBSCRIPTION_VAT.ToString())
                                 .Sum(sm => sm.amount);

            return new BQPaymentRow
            {
                PostingDate = postingDate,
                ExternalDocumentNumber = payments.Key,
                AccountType = AccountType.Vendor.ToString(),
                AccountNo = string.Empty,
                Amount = (decimal)Math.Round(-amount, 3),
                BalAccountNo = string.Empty,
                BalAccountType = string.Empty,
                BalVATAmount = 0,
            };
        }
    }
}
