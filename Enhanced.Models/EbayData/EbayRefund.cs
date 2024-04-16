namespace Enhanced.Models.EbayData
{
    public class EbayRefundRequest : EbayRefund
    {
        public string? JWE { get; set; }
        public string? PrivateKey { get; set; }
    }

    public class EbayRefund
    {
        public string? reasonForRefund { get; set; }
        public string? comment { get; set; }
        public List<RefundItem>? refundItems { get; set; }
        //public Amount? orderLevelRefundAmount { get; set; }
    }

    public class RefundItem
    {
        public Amount? refundAmount { get; set; }
        public string? lineItemId { get; set; }
        //public LegacyReference? legacyReference { get; set; }
    }

    public class LegacyReference
    {
        public string? legacyItemId { get; set; }
        public string? legacyTransactionId { get; set; }
    }

    public class RefundResponse
    {
        public string? refundId { get; set; }
        public string? refundStatus { get; set; }
        public List<Error>? errors { get; set; } = new();
    }
}
