namespace Enhanced.Models.BQData
{
    public class BQRefund
    {
        public string? order_tax_mode { get; set; }
        public List<Refund>? refunds { get; set; }
    }

    public class Refund
    {
        public decimal amount { get; set; }
        public string? currency_iso_code { get; set; }
        public bool excluded_from_shipment { get; set; }
        public string? order_line_id { get; set; }
        public int quantity { get; set; }
        public string? reason_code { get; set; }
        public decimal shipping_amount { get; set; }
        //public List<ShippingTaxes>? shipping_taxes { get; set; }
        //public List<Taxes>? taxes { get; set; }
    }

    public class ShippingTaxes
    {
        public decimal amount { get; set; }
        //public string? code { get; set; }
    }

    public class Taxes
    {
        public decimal amount { get; set; }
        //public string? code { get; set; }
    }

    public class BQRefundResponse : BQRefund
    {
        public string? message { get; set; }
    }
}
