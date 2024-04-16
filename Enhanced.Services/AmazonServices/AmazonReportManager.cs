using Enhanced.Models.AmazonData;
using Enhanced.Models.Shared;
using static Enhanced.Models.AmazonData.AmazonEnum;
using static Enhanced.Models.Shared.CommonEnum;

namespace Enhanced.Services.AmazonServices
{
    public class AmazonReportManager : IAmazonReportManager
    {
        /// <summary>
        /// Get Settlement Order
        /// </summary>
        /// <param name="amazonCredential"></param>
        /// <param name="days"></param>
        /// <returns></returns>
        public async Task<SettlementBatch> GetSettlementOrder(AmazonCredential amazonCredential, DownloadPaymentParameter amazonPaymentParameter)
        {
            var settlementOrderRows = new List<SettlementOrderRow>();
            var errorLogs = new List<ErrorLog>();

            DateTime fromDate = DateTime.UtcNow.AddDays(-1 * amazonPaymentParameter.Days);
            DateTime toDate = DateTime.UtcNow;

            int totalDays = (int)(DateTime.UtcNow - fromDate).TotalDays;

            if (totalDays > 90)
            {
                fromDate = DateTime.UtcNow.AddDays(-90);
            }

            var (paths, reportDocumentIdsToUpdate, errors) = await GetSettlementReportOrder(amazonCredential, amazonPaymentParameter.ReportDocumentIds!, fromDate, toDate).ConfigureAwait(false);

            if (errors != null && errors.Any())
            {
                errorLogs.AddRange(errors);
            }

            foreach (var path in paths)
            {
                var report = new SettlementOrderReport(path);
                var amazon_co_uk = report.Data.Where(x => x.MarketplaceName!.ToLower() == Constant.MarketplaceName || string.IsNullOrEmpty(x.MarketplaceName)).ToList();

                if (amazon_co_uk != null && amazon_co_uk.Any())
                {
                    settlementOrderRows.AddRange(amazon_co_uk);

                    errorLogs.Add(new ErrorLog(Marketplace.Amazon, Sevarity.Information, "Download Path", path, Priority.High, "Total Records: " + amazon_co_uk.Count));
                }

                CommonHelper.DeleteFile(path);
            }

            var result = CreateSettlementBatch(settlementOrderRows, reportDocumentIdsToUpdate);
            result.ErrorLogs?.AddRange(errorLogs);

            return result;
        }

        public async Task<SettlementBatchForCredit> GetSettlementOrderForCredits(AmazonCredential amazonCredential, DownloadPaymentParameter amazonPaymentParameter)
        {
            var result = new SettlementBatchForCredit();
            var settlementOrderRows = new List<SettlementOrderRow>();
            var errorLogs = new List<ErrorLog>();

            DateTime fromDate = DateTime.UtcNow.AddDays(-1 * amazonPaymentParameter.Days);
            DateTime toDate = DateTime.UtcNow;

            int totalDays = (int)(DateTime.UtcNow - fromDate).TotalDays;

            if (totalDays > 15)
            {
                fromDate = DateTime.UtcNow.AddDays(-15);
            }

            var (paths, reportDocumentIdsToUpdate, errors) = await GetSettlementReportOrder(amazonCredential, amazonPaymentParameter.ReportDocumentIds!, fromDate, toDate).ConfigureAwait(false);

            if (errors != null && errors.Any())
            {
                errorLogs.AddRange(errors);
            }

            if (paths != null && paths.Any())
            {
                foreach (var path in paths)
                {
                    var report = new SettlementOrderReport(path);
                    var amazon_co_uk = report.Data.Where(x => x.MarketplaceName!.ToLower() == Constant.MarketplaceName || string.IsNullOrEmpty(x.MarketplaceName)).ToList();

                    if (amazon_co_uk != null && amazon_co_uk.Any())
                    {
                        settlementOrderRows.AddRange(amazon_co_uk);

                        errorLogs.Add(new ErrorLog(Marketplace.Amazon, Sevarity.Information, "Credits - Download Path", path, Priority.High, "Total Records: " + amazon_co_uk.Count));
                    }

                    CommonHelper.DeleteFile(path);
                }

                result = CreateSettlementBatchForCredits(settlementOrderRows);

                result.ErrorLogs?.AddRange(errorLogs);
            }

            return result;
        }

        /// <summary>
        /// Create Settlement Batch
        /// </summary>
        /// <param name="settlementOrderRows"></param>
        /// <param name="reportDocumentIdsToUpdate"></param>
        private static SettlementBatch CreateSettlementBatch(List<SettlementOrderRow> settlementOrderRows, List<ReportDocumentDetails> reportDocumentIdsToUpdate)
        {
            var settlementBatch = new SettlementBatch();
            var errorLogs = new List<ErrorLog>();

            if (settlementOrderRows != null && settlementOrderRows.Any())
            {
                var settlementBatches = new List<SettlementRowBatch>();
                // Send result set by settlement
                var settlementGrp = settlementOrderRows.GroupBy(g => g.SettlementId).ToList();

                foreach (var settlementItems in settlementGrp)
                {
                    try
                    {
                        var settlementHeader = settlementItems.Where(x => x.SettlementStartDate.HasValue).FirstOrDefault();

                        if (settlementHeader?.Currency?.ToUpper() != Currency.GBP.ToString())
                        {
                            continue;
                        }

                        var settlements = settlementItems.ToList();

                        var salesReceiptLines = GetSalesReceiptLines(settlements);
                        var vendorPaymentEntry = GetVendorPaymentEntry(settlements);
                        var vendorGLEntries = GetVendorGLInvoiceEntry(settlements);
                        var bankEntry = GetBankEntry(settlementHeader!);

                        settlementBatches.Add(new SettlementRowBatch
                        {
                            SettlementId = settlementHeader?.SettlementId,
                            SalesReceiptLines = salesReceiptLines,
                            VendorPaymentEntry = vendorPaymentEntry,
                            VendorGLInvoiceEntries = vendorGLEntries,
                            BankEntry = bankEntry,
                        });

                        errorLogs.Add(new ErrorLog(Marketplace.Amazon, Sevarity.Information, "Settlement Id", settlementHeader?.SettlementId, Priority.Low));
                    }
                    catch (Exception ex)
                    {
                        errorLogs.Add(new ErrorLog(Marketplace.Amazon, Sevarity.Error, "Settlement Id", settlementItems.Key, Priority.High, ex.Message, ex.StackTrace));
                        continue;
                    }
                }

                settlementBatch.Settlements!.AddRange(settlementBatches);
            }

            if (reportDocumentIdsToUpdate != null && reportDocumentIdsToUpdate.Any())
            {
                settlementBatch.ReportDocumentDetails = reportDocumentIdsToUpdate;
            }

            if (errorLogs != null && errorLogs.Any())
            {
                settlementBatch.ErrorLogs?.AddRange(errorLogs);
            }

            return settlementBatch;
        }

        private static SettlementBatchForCredit CreateSettlementBatchForCredits(List<SettlementOrderRow> settlementOrderRows)
        {
            var settlementBatchForCredits = new SettlementBatchForCredit();
            var errorLogs = new List<ErrorLog>();

            if (settlementOrderRows != null && settlementOrderRows.Any())
            {
                var salesReceiptLineForCredits = new List<SalesReceiptLineForCredit>();
                // Send result set by settlement
                var settlementGrp = settlementOrderRows.Where(x => x.FulfillmentId != "MFN").GroupBy(g => g.SettlementId).ToList();

                foreach (var settlementItems in settlementGrp)
                {
                    try
                    {
                        var settlementHeader = settlementItems.Where(x => x.SettlementStartDate.HasValue).FirstOrDefault();

                        if (settlementHeader?.Currency?.ToUpper() != Currency.GBP.ToString())
                        {
                            continue;
                        }

                        var settlements = settlementItems.ToList();
                        var salesReceiptLines = GetSalesReceiptLines(settlements);
                        var credits = salesReceiptLines.Where(x => x.Amount > 0).ToList();

                        if (credits?.Any() == true)
                        {
                            foreach (var credit in credits)
                            {
                                salesReceiptLineForCredits.Add(new SalesReceiptLineForCredit
                                {
                                    ExternalDocumentNumber = credit.ExternalDocumentNumber,
                                    Amount = credit.Amount * -1,
                                });

                                errorLogs.Add(new ErrorLog(Marketplace.Amazon, Sevarity.Information, "Credits - Order Id", credit.ExternalDocumentNumber, Priority.Low));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        errorLogs.Add(new ErrorLog(Marketplace.Amazon, Sevarity.Error, "Credits", settlementItems.Key, Priority.High, ex.Message, ex.StackTrace));
                        continue;
                    }
                }

                settlementBatchForCredits.Credits?.AddRange(salesReceiptLineForCredits);
            }

            if (errorLogs != null && errorLogs.Any())
            {
                settlementBatchForCredits.ErrorLogs?.AddRange(errorLogs);
            }

            return settlementBatchForCredits;
        }

        /// <summary>
        /// Get Sales Receipt Lines
        /// </summary>
        /// <param name="settlementOrderRows"></param>
        /// <returns></returns>
        private static List<SalesReceiptLine> GetSalesReceiptLines(List<SettlementOrderRow> settlementOrderRows)
        {
            var salesReceiptLines = new List<SalesReceiptLine>();
            var grpOrder = settlementOrderRows.Where(x => (x.AmountType == CommonHelper.GetEnumMemberValue(AmountType.ItemPrice) ||
                                                           x.AmountType == CommonHelper.GetEnumMemberValue(AmountType.Promotion)) &&
                                                          !string.IsNullOrEmpty(x.OrderId))
                                              .GroupBy(gb => new { gb.OrderId, gb.TransactionType }).ToList();

            if (grpOrder != null && grpOrder.Any())
            {
                foreach (var settlementRows in grpOrder)
                {
                    var salesLine = new SalesReceiptLine
                    {
                        PostingDate = DateTime.UtcNow.Date,
                        DocumentDate = settlementRows.FirstOrDefault()?.PostedDate,
                        DocumentNumber = settlementRows.FirstOrDefault()?.SettlementId,
                        ExternalDocumentNumber = settlementRows.Key.OrderId,
                        Description = settlementRows.FirstOrDefault()?.SKU,
                        Amount = settlementRows.Sum(sm => sm.Amount) * -1,
                    };

                    if (salesLine.Amount != 0)
                    {
                        salesReceiptLines.Add(salesLine);
                    }
                }
            }

            return salesReceiptLines;
        }

        /// <summary>
        /// Get Vendor Payment Entry
        /// </summary>
        /// <param name="settlementOrderRows"></param>
        /// <returns></returns>
        private static VendorPaymentEntry GetVendorPaymentEntry(List<SettlementOrderRow> settlementOrderRows)
        {
            var settlements = settlementOrderRows.Where(x => x.AmountType != CommonHelper.GetEnumMemberValue(AmountType.ItemPrice) &&
                                                             x.AmountType != CommonHelper.GetEnumMemberValue(AmountType.Promotion))
                                                 .ToList();

            if (settlements != null && settlements.Any(a => a.Amount != 0))
            {
                return new VendorPaymentEntry
                {
                    PostingDate = DateTime.UtcNow.Date,
                    DocumentNumber = settlements.FirstOrDefault()?.SettlementId,
                    Amount = -settlements.Sum(sm => sm.Amount)
                };
            }

            return new VendorPaymentEntry();
        }

        /// <summary>
        /// Get Vendor GL Invoice Entry
        /// </summary>
        /// <param name="settlementOrderRows"></param>
        /// <returns></returns>
        private static List<VendorGLInvoiceEntry> GetVendorGLInvoiceEntry(List<SettlementOrderRow> settlementOrderRows)
        {
            var vendorGLInvoiceEntries = new List<VendorGLInvoiceEntry>();

            // GL 79071
            var gl79071 = settlementOrderRows.Where(x => x.AmountDescription == CommonHelper.GetEnumMemberValue(ItemFees.Commission) ||
                                                         x.AmountDescription == CommonHelper.GetEnumMemberValue(ItemFees.RefundCommission) ||
                                                         x.AmountDescription == CommonHelper.GetEnumMemberValue(ItemFees.VariableClosingFee))
                                             .ToList();

            if (gl79071 != null && gl79071.Any())
            {
                var gl79071Entry = VendorGLInvoiceEntry.ToVendorGLInvoiceEntry(gl79071, BalancingAccountNumber.GL_79071.ToString());

                if (gl79071Entry?.Amount != 0)
                {
                    vendorGLInvoiceEntries.Add(gl79071Entry!);
                }
            }

            // GL 79075
            var gl79075 = settlementOrderRows.Where(x => x.AmountType == CommonHelper.GetEnumMemberValue(AmountType.ItemFees) &&
                                                         x.AmountDescription != CommonHelper.GetEnumMemberValue(ItemFees.Commission) &&
                                                         x.AmountDescription != CommonHelper.GetEnumMemberValue(ItemFees.RefundCommission) &&
                                                         x.AmountDescription != CommonHelper.GetEnumMemberValue(ItemFees.VariableClosingFee))
                                             .ToList();

            if (gl79075 != null && gl79075.Any())
            {
                var gl79075Entry = VendorGLInvoiceEntry.ToVendorGLInvoiceEntry(gl79075, BalancingAccountNumber.GL_79075.ToString());

                if (gl79075Entry?.Amount != 0)
                {
                    vendorGLInvoiceEntries.Add(gl79075Entry!);
                }
            }

            // GL 51000
            var gl51000 = settlementOrderRows.Where(x => x.AmountDescription == CommonHelper.GetEnumMemberValue(OtherTransaction.ShippingLabelPurchase) ||
                                                         x.AmountDescription == CommonHelper.GetEnumMemberValue(OtherTransaction.ShippingLabelPurchaseForReturn))
                                             .ToList();

            if (gl51000 != null && gl51000.Any())
            {
                var gl51000Entry = VendorGLInvoiceEntry.ToVendorGLInvoiceEntry(gl51000, BalancingAccountNumber.GL_51000.ToString());

                if (gl51000Entry?.Amount != 0)
                {
                    vendorGLInvoiceEntries.Add(gl51000Entry!);
                }
            }

            // GL 40025
            var gl40025 = settlementOrderRows.Where(x => x.AmountDescription == CommonHelper.GetEnumMemberValue(OtherTransactions.SAFE_T_Reimbursement) ||
                                                         x.AmountDescription == CommonHelper.GetEnumMemberValue(OtherTransaction.ComminglingVAT))
                                             .ToList();

            if (gl40025 != null && gl40025.Any())
            {
                var gl40025Entry = VendorGLInvoiceEntry.ToVendorGLInvoiceEntry(gl40025, BalancingAccountNumber.GL_40025.ToString());

                if (gl40025Entry?.Amount != 0)
                {
                    vendorGLInvoiceEntries.Add(gl40025Entry!);
                }
            }

            // GL 40025
            var miscellaneousPaymentEntry = GetMiscellaneousPaymentEntry(settlementOrderRows);

            if (miscellaneousPaymentEntry != null && miscellaneousPaymentEntry.Any())
            {
                var gl40025Entry = VendorGLInvoiceEntry.ToVendorGLMiscellaneousEntry(miscellaneousPaymentEntry, BalancingAccountNumber.GL_40025.ToString());

                if (gl40025Entry?.Amount != 0)
                {
                    vendorGLInvoiceEntries.Add(gl40025Entry!);
                }
            }

            return vendorGLInvoiceEntries;
        }

        /// <summary>
        /// Get Bank Entry
        /// </summary>
        /// <param name="settlementHeader"></param>
        /// <returns></returns>
        private static BankEntry GetBankEntry(SettlementOrderRow settlementHeader)
        {
            if (settlementHeader != null && settlementHeader.TotalAmount != 0)
            {
                return new BankEntry
                {
                    PostingDate = DateTime.UtcNow.Date,
                    DocumentNumber = settlementHeader.SettlementId,
                    Amount = settlementHeader.TotalAmount,
                };
            }

            return new BankEntry();
        }

        /// <summary>
        /// Get Payment Entry that is not been considered in the above filter
        /// </summary>
        /// <param name="settlementOrderRows"></param>
        /// <returns></returns>
        private static List<SettlementOrderRow> GetMiscellaneousPaymentEntry(List<SettlementOrderRow> settlementOrderRows)
        {
            return settlementOrderRows.Where(x => x.AmountType != CommonHelper.GetEnumMemberValue(AmountType.ItemFees) &&
                                                  x.AmountType != CommonHelper.GetEnumMemberValue(AmountType.ItemPrice) &&
                                                  x.AmountType != CommonHelper.GetEnumMemberValue(AmountType.Promotion) &&
                                                  x.AmountDescription != CommonHelper.GetEnumMemberValue(OtherTransactions.SAFE_T_Reimbursement) &&
                                                  x.AmountDescription != CommonHelper.GetEnumMemberValue(OtherTransaction.ComminglingVAT) &&
                                                  x.AmountDescription != CommonHelper.GetEnumMemberValue(OtherTransaction.ShippingLabelPurchase) &&
                                                  x.AmountDescription != CommonHelper.GetEnumMemberValue(OtherTransaction.ShippingLabelPurchaseForReturn))
                                      .ToList();
        }

        /// <summary>
        /// Get Settlement Report Order
        /// </summary>
        /// <param name="amazonCredential"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        private static async Task<(List<string>, List<ReportDocumentDetails>, List<ErrorLog>)> GetSettlementReportOrder(AmazonCredential amazonCredential, List<string> reportDocumentIds, DateTime fromDate, DateTime toDate)
        {
            var amazonReportService = new AmazonReportService(amazonCredential);

            return await amazonReportService.DownloadExistingReportAndDownloadFile(ReportTypes.GET_V2_SETTLEMENT_REPORT_DATA_FLAT_FILE_V2, reportDocumentIds, fromDate, toDate).ConfigureAwait(false);
        }
    }
}
