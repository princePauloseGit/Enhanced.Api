using Enhanced.Models.Shared;
using static Enhanced.Models.PayPalData.PayPalEnum;

namespace Enhanced.Models.PayPalData
{
    public class PayPalSettlementReport
    {
        public DateTime? PostingDate { get; set; }
        public List<PayPalSettlementRow>? PayPalSettlementRows { get; set; } = new List<PayPalSettlementRow>();

        public PayPalSettlementReport(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            var rowHeader = File.ReadAllLines(path)?.Skip(2)?.Take(1)?.FirstOrDefault();

            if (rowHeader != null)
            {
                string[] rowValues = rowHeader!.Replace("\"", "").Split(',');

                if (rowValues.Length >= 1)
                {
                    var date = rowValues[1].Split(" ");

                    if (date.Length > 0)
                    {
                        PostingDate = CommonHelper.GetDate(date[0].Trim(), Constant.DATETIME_FORMAT_UTC_SLASH);
                    }                    
                }
            }

            var values = File.ReadAllLines(path)
                .Skip(4)
                .Select(v => PayPalSettlementRow.FromCsv(v, PostingDate))
                .ToList();

            PayPalSettlementRows = values;
        }

        public class PayPalSettlementRow
        {
            public string? TransactionId { get; set; }
            public string? InvoiceId { get; set; }
            public string? PayPalReferenceId { get; set; }
            public string? PayPalReferenceIdType { get; set; }
            public string? TransactionEventCode { get; set; }
            public DateTime? TransactionInitiationDate { get; set; }
            public DateTime? TransactionCompletionDate { get; set; }
            public string? TransactionDebitOrCredit { get; set; }
            public decimal? GrossTransactionAmount { get; set; }
            public string? GrossTransactionCurrency { get; set; }
            public string? FeeDebitOrCredit { get; set; }
            public decimal? FeeAmount { get; set; }
            public string? FeeCurrency { get; set; }
            public string? CustomField { get; set; }
            public string? ConsumerId { get; set; }
            public string? PaymentTrackingId { get; set; }
            public string? StoreId { get; set; }
            public string? BankReferenceId { get; set; }
            public decimal? CreditTransactionalFee { get; set; }
            public decimal? CreditPromotionalFee { get; set; }
            public int? CreditTerm { get; set; }
            public string? ShortcutDimension { get; set; }
            public DateTime? PostingDate { get; set; }

            public static PayPalSettlementRow FromCsv(string csvLine, DateTime? postingDate)
            {
                string[] values = csvLine.Replace("\"", "").Split(',');

                if (values[0] == Constant.PAYPALROWTYPE)
                {
                    var row = new PayPalSettlementRow
                    {
                        TransactionId = values[1],
                        InvoiceId = CommonHelper.IsNumeric(values[2]) ? values[2].PadLeft(9, '0') : values[2], // If numeric, this should be padded with leading zeros to 9 digits //https://trello.com/c/EJHepPz2/215-paypal-payment-downloads
                        PayPalReferenceId = values[3],
                        PayPalReferenceIdType = values[4],
                        TransactionEventCode = values[5],
                        TransactionInitiationDate = CommonHelper.GetDate(values[6], Constant.DATETIME_FORMAT_UTC_SLASH),
                        TransactionCompletionDate = CommonHelper.GetDate(values[7], Constant.DATETIME_FORMAT_UTC_SLASH),
                        TransactionDebitOrCredit = values[8],
                        GrossTransactionAmount = CommonHelper.GetDecimal(values[9]),
                        GrossTransactionCurrency = values[10],
                        FeeDebitOrCredit = values[11],
                        FeeAmount = CommonHelper.GetDecimal(values[12]),
                        FeeCurrency = values[13],
                        CustomField = values[14],
                        ConsumerId = values[15],
                        PaymentTrackingId = values[16],
                        StoreId = values[17],
                        BankReferenceId = values[18],
                        CreditTransactionalFee = CommonHelper.GetDecimal(values[19]),
                        CreditPromotionalFee = CommonHelper.GetDecimal(values[20]),
                        CreditTerm = CommonHelper.GetInt(values[21])
                    };

                    if (row.TransactionEventCode == EventCode.T0006.ToString() ||       // if transaction event code = T0006
                             (row.TransactionEventCode == EventCode.T1107.ToString() && // if transaction event code = T1107 and
                             CommonHelper.IsNumeric(row.InvoiceId)))                    // Invoice ID is numeric
                    {
                        row.ShortcutDimension = CommonEnum.ShortcutDimension.Website.ToString();
                    }
                    else if (row.TransactionEventCode == EventCode.T0004.ToString() ||  // if transaction event code = T0004 OR
                            (row.TransactionEventCode == EventCode.T1107.ToString() &&  // if transaction event code = T1107 and
                             row.ConsumerId.ToLower().StartsWith("ebay")))              // customer ID starts 'ebay'
                    {
                        row.ShortcutDimension = CommonEnum.ShortcutDimension.eBay.ToString();
                    }
                    else if ((!string.IsNullOrEmpty(row.PayPalReferenceId) ||           // If the field "PayPal Reference ID" is populated, or
                         row.TransactionEventCode == EventCode.T5000.ToString() ||      // transaction event code = T5000 or 
                         row.TransactionEventCode == EventCode.T5001.ToString()) &&     // transaction event code = T5001
                        !row.ConsumerId.ToLower().StartsWith("ebay"))                   // and the field "Consumer ID" does NOT start with "ebay"
                    {
                        row.ShortcutDimension = CommonEnum.ShortcutDimension.OnBuy.ToString();
                    }
                    else if (row.TransactionEventCode == EventCode.T1107.ToString() &&  // if transaction event code = T1107 and
                             !CommonHelper.IsNumeric(row.InvoiceId))                    //  invoice id is NOT numeric
                    {
                        row.ShortcutDimension = CommonEnum.ShortcutDimension.OnBuy.ToString();
                    }

                    row.PostingDate = postingDate;

                    return row;
                }

                return new PayPalSettlementRow
                {
                    TransactionId = "-1"
                };
            }
        }

        public class PayPalSettlementBatch
        {
            public List<PayPalSettlement>? PayPalSettlements { get; set; } = new List<PayPalSettlement>();
            public List<ReportDocumentDetails>? ReportDocumentDetails { get; set; } = new List<ReportDocumentDetails>();
            public List<ErrorLog>? ErrorLogs { get; set; } = new List<ErrorLog>();
        }

        public class PayPalSettlement
        {
            public string? SettlementId { get; set; }
            public List<SettlementGroup>? PayPalSettlements { get; set; }
        }
    }
}
