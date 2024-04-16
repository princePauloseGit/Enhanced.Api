namespace Enhanced.Models.BQData
{
    public partial class BQOrderList
    {
        public List<OrderList>? orders { get; set; }

        public class OrderList
        {
            public string? order_id { get; set; }
            public Customer? customer { get; set; }
            public string? customer_notification_email { get; set; }
            public string? payment_type { get; set; }
            public List<Orderline>? order_lines { get; set; }
            public Decimal shipping_price { get; set; }
            public string? shipping_type_label { get; set; }
        }

        public class Orderline
        {
            public string? order_line_id { get; set; }
            public string? offer_sku { get; set; }
            public int? quantity { get; set; }
            public decimal? price_unit { get; set; }
            public decimal? commission_vat { get; set; }
        }

        public class Customer
        {
            public BillingAddress? billing_address { get; set; }
            public class BillingAddress
            {
                public string? city { get; set; }
                public string? civility { get; set; }
                public string? company { get; set; }
                public string? country { get; set; }
                public string? country_iso_code { get; set; }
                public string? firstname { get; set; }
                public string? lastname { get; set; }
                public string? state { get; set; }
                public string? street_1 { get; set; }
                public string? street_2 { get; set; }
                public string? zip_code { get; set; }
            }
            public string? civility { get; set; }
            public string? customer_id { get; set; }
            public string? firstname { get; set; }
            public string? lastname { get; set; }
            public string? locale { get; set; }
            public ShippingAddress? shipping_address { get; set; }

            public class ShippingAddress
            {
                public string? city { get; set; }
                public string? civility { get; set; }
                public string? company { get; set; }
                public string? country { get; set; }
                public string? country_iso_code { get; set; }
                public string? firstname { get; set; }
                public string? lastname { get; set; }
                public string? state { get; set; }
                public string? street_1 { get; set; }
                public string? street_2 { get; set; }
                public string? zip_code { get; set; }
                public string? phone { get; set; }
            }
        }
    }
}
