using Enhanced.Models.BraintreeData;
using Enhanced.Models.Shared;
using static Enhanced.Models.Shared.CommonEnum;

namespace Enhanced.Services.BraintreeService
{
    public class BraintreeReportManager : IBraintreeReportManager
    {
        public async Task<BraintreeSettlementReport> SearchTransaction(ParameterBraintree parameterBraintree, DownloadPaymentParameter downloadPaymentParameter)
        {
            var settlementBatches = new List<BraintreeSettlementBatch>();
            var reportDocumentIdsToUpdate = new List<ReportDocumentDetails>();
            var errorLogs = new List<ErrorLog>();
            var braintreeReportDocumentIds = new List<string>();

            var braintreeRequestService = new BraintreeRequestService(parameterBraintree);

            var (transactions, errors) = await braintreeRequestService.SearchTransaction(downloadPaymentParameter.Days).ConfigureAwait(false);

            if (errors != null && errors.Any())
            {
                errorLogs.AddRange(errors);
            }

            if (transactions == null || !transactions.Any())
            {
                return new BraintreeSettlementReport
                {
                    Settlements = settlementBatches,
                    ReportDocumentDetails = reportDocumentIdsToUpdate,
                    ErrorLogs = errorLogs
                };
            }

            var settlementGrp = transactions.GroupBy(gp => GetSettlementId(gp.SettlementBatchId)).ToList();

            foreach (var settlement in settlementGrp)
            {
                try
                {
                    if (!downloadPaymentParameter.ReportDocumentIds!.Any(x => x == GetBTSettlementId(settlement.Key)))
                    {
                        var settlementGroups = new List<SettlementGroup>();
                        var settlementDate = DateTime.Parse(settlement.Key);

                        var customerPayments = GetCustomerPayment(settlement, settlementDate);
                        var vendorPayment = GetVendorPayment(settlement, settlementDate);
                        var bankPayment = GetBankEntry(settlement, settlementDate);

                        settlementGroups.AddRange(customerPayments);

                        if (vendorPayment.Amount != 0)
                        {
                            settlementGroups.Add(vendorPayment);
                        }

                        if (bankPayment.Amount != 0)
                        {
                            settlementGroups.Add(bankPayment);
                        }

                        settlementBatches.Add(new BraintreeSettlementBatch
                        {
                            SettlementId = GetBTSettlementId(settlement.Key),
                            BraintreeSettlements = settlementGroups,
                        });

                        reportDocumentIdsToUpdate.Add(new ReportDocumentDetails
                        {
                            ReportDocumentId = GetBTSettlementId(settlement.Key),
                            CanArchived = false,
                        });

                        errorLogs.Add(new ErrorLog(Marketplace.Braintree, Sevarity.Information, "Settlement Batch Id", GetBTSettlementId(settlement.Key), Priority.Low));
                    }
                    else
                    {
                        braintreeReportDocumentIds.Add(GetBTSettlementId(settlement.Key));
                    }
                }
                catch (Exception ex)
                {
                    errorLogs.Add(new ErrorLog(Marketplace.Braintree, Sevarity.Error, "Payment Batch", GetBTSettlementId(settlement.Key), Priority.High, ex.Message, ex.StackTrace));
                    continue;
                }
            }

            if (braintreeReportDocumentIds?.Any() == true)
            {
                foreach (var reportDocumentId in downloadPaymentParameter.ReportDocumentIds!)
                {
                    if (!braintreeReportDocumentIds.Any(x => x == reportDocumentId))
                    {
                        reportDocumentIdsToUpdate.Add(new ReportDocumentDetails
                        {
                            ReportDocumentId = reportDocumentId,
                            CanArchived = true,
                        });
                    }
                }
            }            

            return new BraintreeSettlementReport
            {
                Settlements = settlementBatches,
                ReportDocumentDetails = reportDocumentIdsToUpdate,
                ErrorLogs = errorLogs
            };
        }

        private static List<SettlementGroup> GetCustomerPayment(IGrouping<string, Braintree.Transaction> settlement, DateTime postingDate)
        {
            var customerPayment = new List<SettlementGroup>();

            foreach (var transaction in settlement)
            {
                decimal? amount = 0;

                switch (transaction.Type)
                {
                    case Braintree.TransactionType.CREDIT:
                        amount = transaction.Amount;
                        break;

                    case Braintree.TransactionType.SALE:
                        amount = transaction.Amount * -1;
                        break;
                }

                var custPayment = new SettlementGroup
                {
                    PostingDate = postingDate,
                    DocumentType = string.Empty,
                    DocumentNumber = postingDate.ToString(Constant.DATETIME_FORMAT_SETTLEMENT),
                    AccountType = AccountType.Customer.ToString(),
                    ExternalDocumentNumber = transaction.OrderId,
                    Amount = amount,
                    ShortcutDimension = ShortcutDimension.Website.ToString(),
                    PaymentLine = PaymentLine.Customer.ToString(),
                };

                if (custPayment.Amount != 0)
                {
                    customerPayment.Add(custPayment);
                }
            }

            return customerPayment;
        }

        private static SettlementGroup GetVendorPayment(IGrouping<string, Braintree.Transaction> settlement, DateTime postingDate)
        {
            return new SettlementGroup
            {
                PostingDate = postingDate,
                DocumentType = string.Empty,
                DocumentNumber = postingDate.ToString(Constant.DATETIME_FORMAT_SETTLEMENT),
                AccountType = AccountType.Vendor.ToString(),
                ExternalDocumentNumber = string.Empty,
                Amount = settlement.Sum(sm => sm.ServiceFeeAmount),
                ShortcutDimension = ShortcutDimension.Website.ToString(),
                PaymentLine = PaymentLine.Vendor.ToString(),
            };
        }

        private static SettlementGroup GetBankEntry(IGrouping<string, Braintree.Transaction> settlement, DateTime postingDate)
        {
            return new SettlementGroup
            {
                PostingDate = postingDate,
                DocumentType = string.Empty,
                DocumentNumber = postingDate.ToString(Constant.DATETIME_FORMAT_SETTLEMENT),
                AccountType = AccountType.Bank.ToString(),
                ExternalDocumentNumber = string.Empty,
                Amount = settlement.Sum(sm => (sm.Type == Braintree.TransactionType.SALE ? -sm.Amount : sm.Amount) * -1) - settlement.Sum(sm => sm.ServiceFeeAmount),
                ShortcutDimension = ShortcutDimension.Website.ToString(),
                PaymentLine = PaymentLine.Vendor.ToString(),
            };
        }

        public async Task<(bool, ErrorLog)> RefundPayment(ParameterBraintree parameterBraintree, ParameterRefundBraintree parameterRefund)
        {
            var braintreeRequestService = new BraintreeRequestService(parameterBraintree);

            return await braintreeRequestService.RefundPayment(parameterRefund.TransactionId!, parameterRefund.Amount);
        }

        private static string GetSettlementId(string settlementId)
        {
            return settlementId.Split("_").FirstOrDefault()!;
        }

        private static string GetBTSettlementId(string settlementId)
        {
            return string.Concat("BT", settlementId.Replace("-", string.Empty));
        }
    }
}
