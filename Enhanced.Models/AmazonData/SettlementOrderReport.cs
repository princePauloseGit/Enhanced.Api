using Enhanced.Models.Shared;
using Constant = Enhanced.Models.Shared.Constant;

namespace Enhanced.Models.AmazonData
{
    public class SettlementOrderReport
    {
        public List<SettlementOrderRow> Data { get; set; } = new List<SettlementOrderRow>();

        public SettlementOrderReport(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            var values = File.ReadAllLines(path)
                .Skip(1)
                .Select(v => SettlementOrderRow.FromCsv(v))
                .ToList();

            Data = values;
        }
    }

    public class SettlementOrderRow
    {
        public string? SettlementId { get; set; }
        public DateTime? SettlementStartDate { get; set; }
        public DateTime? SettlementEndDate { get; set; }
        public DateTime? DepositDate { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? Currency { get; set; }
        public string? TransactionType { get; set; }
        public string? OrderId { get; set; }
        public string? MerchantOrderId { get; set; }
        public string? AdjustmentId { get; set; }
        public string? ShipmentId { get; set; }
        public string? MarketplaceName { get; set; }
        public string? AmountType { get; set; }
        public string? AmountDescription { get; set; }
        public decimal? Amount { get; set; }
        public string? FulfillmentId { get; set; }
        public DateTime? PostedDate { get; set; }
        public DateTime? PostedDateTime { get; set; }
        public string? OrderItemCode { get; set; }
        public string? MerchantOrderItemId { get; set; }
        public string? MerchantAdjustmentItemId { get; set; }
        public string? SKU { get; set; }
        public int? QuantityPurchased { get; set; }
        public string? PromotionId { get; set; }

        public static SettlementOrderRow FromCsv(string csvLine)
        {
            string[] values = csvLine.Split('\t');

            var row = new SettlementOrderRow
            {
                SettlementId = values[0][^9..],
                SettlementStartDate = CommonHelper.GetDate(values[1], Constant.DATETIME_FORMAT_UTC_DOT),
                SettlementEndDate = CommonHelper.GetDate(values[2], Constant.DATETIME_FORMAT_UTC_DOT),
                DepositDate = CommonHelper.GetDate(values[3], Constant.DATETIME_FORMAT_UTC_DOT),
                TotalAmount = CommonHelper.GetDecimal(values[4]),
                Currency = values[5],
                TransactionType = values[6],
                OrderId = values[7],
                MerchantOrderId = values[8],
                AdjustmentId = values[9],
                ShipmentId = values[10],
                MarketplaceName = values[11],
                AmountType = values[12],
                AmountDescription = values[13],
                Amount = CommonHelper.GetDecimal(values[14]),
                FulfillmentId = values[15],
                PostedDate = CommonHelper.GetDate(values[16], Constant.DATE_FORMAT_DOT),
                PostedDateTime = CommonHelper.GetDate(values[17], Constant.DATETIME_FORMAT_UTC_DOT),
                OrderItemCode = values[18],
                MerchantOrderItemId = values[19],
                MerchantAdjustmentItemId = values[20],
                SKU = values[21],
                QuantityPurchased = CommonHelper.GetInt(values[22]),
                PromotionId = values.Length == 24 ? values[23] : String.Empty
            };

            return row;
        }
    }

    public class SettlementBatch
    {
        public List<SettlementRowBatch>? Settlements { get; set; } = new List<SettlementRowBatch>();
        public List<ReportDocumentDetails>? ReportDocumentDetails { get; set; } = new List<ReportDocumentDetails>();
        public List<ErrorLog>? ErrorLogs { get; set; } = new List<ErrorLog>();
    }

    public class SettlementBatchForCredit
    {
        public List<SalesReceiptLineForCredit>? Credits { get; set; } = new List<SalesReceiptLineForCredit>();
        public List<ErrorLog>? ErrorLogs { get; set; } = new List<ErrorLog>();
    }

    public class SalesReceiptLineForCredit
    {
        public string? ExternalDocumentNumber { get; set; } // Order Id
        public decimal? Amount { get; set; }
    }

    public class SettlementRowBatch
    {
        public string? SettlementId { get; set; }
        public List<SalesReceiptLine>? SalesReceiptLines { get; set; }
        public VendorPaymentEntry? VendorPaymentEntry { get; set; }
        public List<VendorGLInvoiceEntry>? VendorGLInvoiceEntries { get; set; }
        public BankEntry? BankEntry { get; set; }
    }

    public class SalesReceiptLine
    {
        public DateTime? PostingDate { get; set; } // Transaction Date - Import Date
        public DateTime? DocumentDate { get; set; } // Posting Date
        public string? DocumentNumber { get; set; }  // Settlement Id
        public string? ExternalDocumentNumber { get; set; } // Order Id
        public string? Description { get; set; }
        public decimal? Amount { get; set; }
    }

    public class VendorPaymentEntry
    {
        public DateTime? PostingDate { get; set; }
        public string? DocumentNumber { get; set; } // Settlement Id
        public decimal? Amount { get; set; }
    }

    public class VendorGLInvoiceEntry
    {
        public DateTime? PostingDate { get; set; }
        public string? DocumentNumber { get; set; } // Settlement Id
        public decimal? Amount { get; set; }
        public string? BalancingAccountNumber { get; set; }
        public string? AccountType { get; set; }

        public static VendorGLInvoiceEntry ToVendorGLInvoiceEntry(List<SettlementOrderRow> settlements, string balancingAccountNumber)
        {
            return new VendorGLInvoiceEntry
            {
                PostingDate = DateTime.UtcNow.Date,
                DocumentNumber = string.Concat(CommonHelper.GetStringFromRight(settlements.FirstOrDefault()?.SettlementId!, 9), "I"),
                Amount = settlements.Sum(sm => sm.Amount),
                BalancingAccountNumber = balancingAccountNumber.Replace("GL_", ""),
                AccountType = CommonEnum.AccountType.Vendor.ToString()
            };
        }

        public static VendorGLInvoiceEntry ToVendorGLMiscellaneousEntry(List<SettlementOrderRow> settlements, string balancingAccountNumber)
        {
            var amount = settlements.Sum(sm => sm.Amount);

            return new VendorGLInvoiceEntry
            {
                PostingDate = DateTime.UtcNow.Date,
                DocumentNumber = string.Concat(CommonHelper.GetStringFromRight(settlements.FirstOrDefault()?.SettlementId!, 9), "I"),
                Amount = amount,
                BalancingAccountNumber = balancingAccountNumber.Replace("GL_", ""),
                AccountType = amount < 0 ? CommonEnum.AccountType.Vendor.ToString() : CommonEnum.AccountType.Customer.ToString()
            };
        }
    }

    public class BankEntry
    {
        public DateTime? PostingDate { get; set; }
        public string? DocumentNumber { get; set; } // Settlement Id
        public decimal? Amount { get; set; }
    }
}
