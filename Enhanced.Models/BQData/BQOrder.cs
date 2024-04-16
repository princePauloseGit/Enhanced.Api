namespace Enhanced.Models.BQData
{
    public partial class BQOrder
    {
        public IList<string>? order_ids { get; set; }
        public IList<string>? order_references_for_customer { get; set; }
        public IList<string>? order_references_for_seller { get; set; }
        public IList<BQEnum.OrderStatus>? order_state_codes { get; set; }
        public IList<string>? channel_codes { get; set; }
        public bool? only_null_channel { get; set; }
        public DateTime? start_date { get; set; }
        public DateTime? end_date { get; set; }
        public DateTime? start_update_date { get; set; }
        public DateTime? end_update_date { get; set; }
        public bool? paginate { get; set; }
        public bool? customer_debited { get; set; }
        public BQEnum.PayMentWorkflow? payment_workflow { get; set; }
        public bool? has_incident { get; set; }
        public IList<string>? fulfillment_center_code { get; set; }
        public BQEnum.TaxMode? order_tax_mode { get; set; }
        public Int64? shop_id { get; set; }

    }
}
