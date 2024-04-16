namespace Enhanced.Models.EbayData
{
    public class EbayPayoutResponse : BulkResponseBase
    {
        public List<Payout>? payouts { get; set; }
    }

    public class Payout
    {
        public Amount? amount { get; set; }
        public string? bankReference { get; set; }
        public string? lastAttemptedPayoutDate { get; set; }
        public string? payoutDate { get; set; }
        public string? payoutId { get; set; }
        public PayoutInstrument? payoutInstrument { get; set; }
        public string? payoutMemo { get; set; }
        public string? payoutStatus { get; set; }
        public string? payoutStatusDescription { get; set; }
        public Amount? totalAmount { get; set; }
        public Amount? totalFee { get; set; }
        public string? transactionCount { get; set; }
    }

    public class PayoutInstrument
    {
        public string? accountLastFourDigits { get; set; }
        public string? instrumentType { get; set; }
        public string? nickname { get; set; }
    }
}
