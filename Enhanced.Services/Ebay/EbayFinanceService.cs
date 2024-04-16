using Enhanced.Models.EbayData;
using Enhanced.Models.Shared;
using static Enhanced.Models.EbayData.EbayEnum;
using static Enhanced.Models.Shared.CommonEnum;

namespace Enhanced.Services.Ebay
{
    public class EbayFinanceService : EbayRequestService
    {
        public EbayFinanceService(ClientToken oauth, AccessToken token) : base(oauth, token) { }

        public async Task<EbaySettlementReport> GetPayouts(EbayPaymentParameter eBayPayment)
        {
            var errorLogs = new List<ErrorLog>();
            var settlementBatches = new List<EbaySettlementBatch>();
            var eBayReportDocumentIds = new List<string>();
            var reportDocumentIdsToUpdate = new List<ReportDocumentDetails>();
            var payouts = new List<Payout>();
            string nextPage = string.Empty;

            try
            {
                DateTime fromDate = DateTime.UtcNow.AddDays(-1 * eBayPayment.Days);
                DateTime toDate = DateTime.UtcNow;

                int totalDays = (int)(DateTime.UtcNow - fromDate).TotalDays;

                if (totalDays > 90)
                {
                    fromDate = DateTime.UtcNow.AddDays(-90);
                }

                string dates = string.Concat("Date From: ", fromDate.ToString(Constant.DATETIME_DDMMYYY_HHMMSS), " - Date To: ", toDate.ToString(Constant.DATETIME_DDMMYYY_HHMMSS));
                errorLogs.Add(new ErrorLog(Marketplace.EBay, Sevarity.Information, "Download Payment", "", Priority.Low, dates));

                string payoutFilter = string.Concat(
                    "?limit=200&filter=payoutStatus:{",
                    eBayPayment.PayoutStatus,
                    "}&filter=payoutDate:[",
                    fromDate.ToString(Constant.DATE_AGING_FORMAT),
                    "T00:00:01.000Z..",
                    toDate.ToString(Constant.DATE_AGING_FORMAT),
                    "T00:00:01.000Z]"
                    );

                await CreateDigitalSignatureRequestAsync(FinanceApiUrls.Payout + payoutFilter, RestSharp.Method.GET, eBayPayment);

                var payoutResponse = await ExecuteRequestAsync<EbayPayoutResponse>();
                nextPage = payoutResponse?.next!;

                if (payoutResponse?.payouts?.Any() == true)
                {
                    payouts.AddRange(payoutResponse?.payouts!);
                }

                while (!string.IsNullOrEmpty(nextPage))
                {
                    await CreateDigitalSignatureRequestAsync(nextPage, RestSharp.Method.GET, eBayPayment);

                    var nextTokenResponse = await ExecuteRequestAsync<EbayPayoutResponse>();
                    nextPage = nextTokenResponse?.next!;

                    if (nextTokenResponse?.payouts?.Any() == true)
                    {
                        payouts.AddRange(nextTokenResponse?.payouts!);
                    }
                }

                if (payouts?.Any() == true)
                {
                    foreach (var payout in payouts)
                    {
                        var settlementGroups = new List<SettlementGroup>();

                        if (eBayPayment.ReportDocumentIds?.Any(x => x == payout.payoutId) == false)
                        {
                            var payoutDate = DateTime.Parse(payout.payoutDate!);
                            errorLogs.Add(new ErrorLog(Marketplace.EBay, Sevarity.Information, "Payout Id", payout.payoutId, Priority.Low, "Total Transactions: " + payout?.transactionCount));

                            var (transactions, errors) = await GetTransactions(eBayPayment, payout?.payoutId!).ConfigureAwait(false);

                            var customerPayments = GetCustomerPayment(transactions, payoutDate);
                            var vendorPayments = GetVendorPayment(transactions, payoutDate);
                            var bankPayment = GetBankEntry(payout?.amount?.value, payoutDate);

                            if (customerPayments.Any())
                            {
                                settlementGroups.AddRange(customerPayments);
                            }

                            if (vendorPayments.Any())
                            {
                                settlementGroups.AddRange(vendorPayments);
                            }

                            if (bankPayment.Amount != 0)
                            {
                                settlementGroups.Add(bankPayment);
                            }

                            settlementBatches.Add(new EbaySettlementBatch
                            {
                                SettlementId = GetEBSettlementId(payoutDate.ToString(Constant.DATETIME_FORMAT_SETTLEMENT)),
                                Settlements = settlementGroups
                            });

                            reportDocumentIdsToUpdate.Add(new ReportDocumentDetails
                            {
                                ReportDocumentId = payout?.payoutId,
                                CanArchived = false,
                            });
                        }
                        else
                        {
                            eBayReportDocumentIds.Add(payout.payoutId!);
                        }
                    }
                }
                else
                {
                    errorLogs.Add(new ErrorLog(Marketplace.EBay, Sevarity.Warning, "Payouts", "", Priority.Low, "No records found"));
                }
            }
            catch (Exception ex)
            {
                errorLogs.Add(new ErrorLog(Marketplace.EBay, Sevarity.Error, "Payouts", "", Priority.High, ex.Message, ex.StackTrace));
            }

            if (eBayReportDocumentIds?.Any() == true)
            {
                foreach (var reportDocumentId in eBayPayment.ReportDocumentIds!)
                {
                    if (!eBayReportDocumentIds.Any(x => x == reportDocumentId))
                    {
                        reportDocumentIdsToUpdate.Add(new ReportDocumentDetails
                        {
                            ReportDocumentId = reportDocumentId,
                            CanArchived = true,
                        });
                    }
                }
            }

            var settlementReport = new EbaySettlementReport
            {
                Settlements = settlementBatches,
                ErrorLogs = errorLogs,
                ReportDocumentDetails = reportDocumentIdsToUpdate,
            };

            return settlementReport;
        }

        public async Task<(List<Transaction>, List<ErrorLog>)> GetTransactions(EbayPaymentParameter eBayPayment, string payoutId)
        {
            var errorLogs = new List<ErrorLog>();
            var transactions = new List<Transaction>();

            try
            {
                string nextPage = string.Empty;
                var transactionFilter = string.Concat("?limit=100&filter=payoutId:{", payoutId, "}");

                await CreateDigitalSignatureRequestAsync(FinanceApiUrls.Transaction + transactionFilter, RestSharp.Method.GET, eBayPayment);

                var transactionResponse = await ExecuteRequestAsync<EbayTransactionResponse>();
                nextPage = transactionResponse?.next!;

                if (transactionResponse?.transactions?.Any() == true)
                {
                    transactions.AddRange(transactionResponse?.transactions!);
                }

                while (!string.IsNullOrEmpty(nextPage))
                {
                    await CreateDigitalSignatureRequestAsync(nextPage, RestSharp.Method.GET, eBayPayment);

                    var nextTokenResponse = await ExecuteRequestAsync<EbayTransactionResponse>();
                    nextPage = nextTokenResponse?.next!;

                    if (nextTokenResponse?.transactions?.Any() == true)
                    {
                        transactions.AddRange(nextTokenResponse?.transactions!);
                    }
                }
            }
            catch (Exception ex)
            {
                errorLogs.Add(new ErrorLog(Marketplace.EBay, Sevarity.Error, "Payout Id", payoutId, Priority.High, ex.Message, ex.StackTrace));
            }

            return (transactions, errorLogs);
        }

        private static List<SettlementGroup> GetCustomerPayment(List<Transaction> transactionData, DateTime postingDate)
        {
            var customerPayments = new List<SettlementGroup>();
            var transactions = transactionData.Where(x => !string.IsNullOrEmpty(x.orderId)).ToList();

            foreach (var transaction in transactions)
            {
                decimal? amount = 0, fee = 0;

                if (transaction.bookingEntry == BookingEntry.CREDIT.ToString())
                {
                    amount = transaction?.amount?.value! * -1;
                    fee = transaction?.totalFeeAmount?.value! * -1;
                }
                else
                {
                    amount = transaction?.amount?.value!;
                    fee = transaction?.totalFeeAmount?.value!;
                }

                amount ??= 0;
                fee ??= 0;

                var custPayment = new SettlementGroup
                {
                    PostingDate = postingDate,
                    DocumentType = string.Empty,
                    DocumentNumber = postingDate.ToString(Constant.DATETIME_FORMAT_SETTLEMENT),
                    AccountType = AccountType.Customer.ToString(),
                    ExternalDocumentNumber = transaction?.orderId,
                    Amount = amount + fee,
                    ShortcutDimension = ShortcutDimension.eBay.ToString(),
                    PaymentLine = PaymentLine.Customer.ToString(),
                };

                if (custPayment.Amount != 0)
                {
                    customerPayments.Add(custPayment);
                }
            }

            return customerPayments;
        }

        private static List<SettlementGroup> GetVendorPayment(List<Transaction> transactions, DateTime postingDate)
        {
            var vendorPayments = new List<SettlementGroup>();

            var listingFeeAmount = transactions.Where(x => x.feeType != null && x.feeType.ToLower().Contains("fee"))?.Sum(sm => (sm.bookingEntry == BookingEntry.CREDIT.ToString() ? sm.amount?.value * -1 : sm.amount?.value));
            var orderFeeAmount = transactions.Where(x => !string.IsNullOrEmpty(x.orderId))?.Sum(sm => (sm.bookingEntry == BookingEntry.DEBIT.ToString() ? sm.totalFeeAmount?.value * -1 : sm.totalFeeAmount?.value));

            if (listingFeeAmount != 0)
            {
                vendorPayments.Add(new SettlementGroup
                {
                    PostingDate = postingDate,
                    DocumentType = string.Empty,
                    DocumentNumber = postingDate.ToString(Constant.DATETIME_FORMAT_SETTLEMENT),
                    AccountType = AccountType.Vendor.ToString(),
                    ExternalDocumentNumber = string.Empty,
                    Amount = listingFeeAmount * -1,
                    ShortcutDimension = ShortcutDimension.eBay.ToString(),
                    PaymentLine = PaymentLine.Invoice.ToString(),
                });
            }

            if (orderFeeAmount != 0)
            {
                vendorPayments.Add(new SettlementGroup
                {
                    PostingDate = postingDate,
                    DocumentType = string.Empty,
                    DocumentNumber = postingDate.ToString(Constant.DATETIME_FORMAT_SETTLEMENT),
                    AccountType = AccountType.Vendor.ToString(),
                    ExternalDocumentNumber = string.Empty,
                    Amount = orderFeeAmount * -1,
                    ShortcutDimension = ShortcutDimension.eBay.ToString(),
                    PaymentLine = PaymentLine.Invoice.ToString(),
                });
            }

            if (listingFeeAmount + orderFeeAmount != 0)
            {
                vendorPayments.Add(new SettlementGroup
                {
                    PostingDate = postingDate,
                    DocumentType = string.Empty,
                    DocumentNumber = postingDate.ToString(Constant.DATETIME_FORMAT_SETTLEMENT),
                    AccountType = AccountType.Vendor.ToString(),
                    ExternalDocumentNumber = string.Empty,
                    Amount = listingFeeAmount + orderFeeAmount,
                    ShortcutDimension = ShortcutDimension.eBay.ToString(),
                    PaymentLine = PaymentLine.Payment.ToString(),
                });
            }

            return vendorPayments;
        }

        private static SettlementGroup GetBankEntry(decimal? payoutAmount, DateTime postingDate)
        {
            return new SettlementGroup
            {
                PostingDate = postingDate,
                DocumentType = string.Empty,
                DocumentNumber = postingDate.ToString(Constant.DATETIME_FORMAT_SETTLEMENT),
                AccountType = AccountType.Bank.ToString(),
                ExternalDocumentNumber = string.Empty,
                Amount = payoutAmount,
                ShortcutDimension = ShortcutDimension.eBay.ToString(),
                PaymentLine = PaymentLine.Bank.ToString(),
            };
        }

        private static string GetEBSettlementId(string settlementId)
        {
            return string.Concat("EB", settlementId.Replace("-", string.Empty));
        }

        public async Task<(bool, ErrorLog)> RefundPayment(string orderId, EbayRefundRequest ebayRefundRequest)
        {
            var refundResult = new RefundResponse { refundId = string.Empty, refundStatus = string.Empty };

            try
            {
                var paymentParameter = new EbayPaymentParameter
                {
                    JWE = ebayRefundRequest.JWE,
                    PrivateKey = ebayRefundRequest.PrivateKey,
                };

                var refundParameter = new EbayRefund
                {
                    reasonForRefund = ebayRefundRequest.reasonForRefund,
                    comment = ebayRefundRequest.comment,
                    //orderLevelRefundAmount = ebayRefundRequest.orderLevelRefundAmount,
                    refundItems = ebayRefundRequest.refundItems,
                };

                await CreateDigitalSignatureRequestAsync(FulfillmentApiUrls.Refund(orderId), RestSharp.Method.POST, paymentParameter, refundParameter);

                var (statusCode, refundResponse) = await ExecuteRequestStatusCodeAsync<RefundResponse>();
                var errorMessages = refundResponse?.errors?.Select(s => s.message?.Replace("\"", "")).ToList() ?? new List<string>()!;

                if (refundResponse?.refundStatus == EbayRefundStatus.REFUNDED.ToString() || refundResponse?.refundStatus == EbayRefundStatus.PENDING.ToString())
                {
                    return (true, new ErrorLog(Marketplace.EBay, Sevarity.Information, "Order Id", orderId, Priority.Low, message: string.Join(",", errorMessages!), stackTrace: refundResponse?.refundStatus));
                }

                return (false, new ErrorLog(Marketplace.EBay, Sevarity.Error, "Order Id", orderId, Priority.High, message: string.Join(",", errorMessages!), stackTrace: "Status Code:" + statusCode));
            }
            catch (Exception ex)
            {
                return (false, new ErrorLog(Marketplace.EBay, Sevarity.Error, "Order Id", orderId, Priority.High, ex.Message, ex.StackTrace));
            }
        }
    }
}
