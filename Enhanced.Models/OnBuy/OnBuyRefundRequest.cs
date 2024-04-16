using Enhanced.Models.AmazonData;

namespace Enhanced.Models.OnBuy
{
    public class OnBuyRefundRequest : ParameterBased
    {
        public int? site_id { get; set; }
        public List<OnBuyOrder>? orders { get; set; }
    }

    public class OnBuyOrderItem
    {
        public int? onbuy_internal_reference { get; set; }
        public decimal? amount { get; set; }
    }

    public class OnBuyOrder
    {
        public string? order_id { get; set; }
        public int? order_refund_reason_id { get; set; }
        public decimal? delivery { get; set; }
        public string? seller_note { get; set; }
        public string? customer_note { get; set; }
        public List<OnBuyOrderItem>? items { get; set; }
    }
}
