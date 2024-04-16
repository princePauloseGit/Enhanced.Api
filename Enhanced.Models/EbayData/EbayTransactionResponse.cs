namespace Enhanced.Models.EbayData
{
    public class EbayTransactionResponse : BulkResponseBase
    {
        public List<Transaction>? transactions { get; set; }
    }

    public class Transaction
    {
        public Amount? amount { get; set; }
        public string? bookingEntry { get; set; }
        public Buyer? buyer { get; set; }
        public EBayCollectedTaxAmount? eBayCollectedTaxAmount { get; set; }
        public FeeJurisdiction? feeJurisdiction { get; set; }
        public string? feeType { get; set; }
        public string? orderId { get; set; }
        public List<OrderLineItem>? orderLineItems { get; set; }
        public string? paymentsEntity { get; set; }
        public string? payoutId { get; set; }
        public List<Reference>? references { get; set; }
        public string? salesRecordReference { get; set; }
        public Amount? totalFeeAmount { get; set; }
        public Amount? totalFeeBasisAmount { get; set; }
        public string? transactionDate { get; set; }
        public string? transactionId { get; set; }
        public string? transactionMemo { get; set; }
        public string? transactionStatus { get; set; }
        public string? transactionType { get; set; }
    }

    public class Buyer
    {
        public string? username { get; set; }
    }

    public class EBayCollectedTaxAmount
    {
        public string? currency { get; set; }
        public string? value { get; set; }
    }

    public class FeeBasisAmount
    {
        public string? currency { get; set; }
        public string? value { get; set; }
    }

    public class FeeJurisdiction
    {
        public string? regionName { get; set; }
        public string? regionType { get; set; }
    }

    public class MarketplaceFee
    {
        public Amount? amount { get; set; }
        public FeeJurisdiction? feeJurisdiction { get; set; }
        public string? feeMemo { get; set; }
        public string? feeType { get; set; }
    }

    public class OrderLineItem
    {
        public FeeBasisAmount? feeBasisAmount { get; set; }
        public string? lineItemId { get; set; }
        public List<MarketplaceFee>? marketplaceFees { get; set; }
    }

    public class Reference
    {
        public string? referenceId { get; set; }
        public string? referenceType { get; set; }
    }
}
