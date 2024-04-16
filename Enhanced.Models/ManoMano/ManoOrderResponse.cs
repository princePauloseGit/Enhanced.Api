namespace Enhanced.Models.ManoMano
{
    public class ManoOrderResponse
    {
        public List<Content>? content { get; set; }
        public Pagination? pagination { get; set; }
        public string? message { get; set; }

        public class Content
        {
            public Addresses? addresses { get; set; }
            public string? created_at { get; set; }
            public Customer? customer { get; set; }
            public bool is_mmf { get; set; }
            public bool is_professional { get; set; }
            public ManomanoAmount? manomano_discount { get; set; }
            public string? order_reference { get; set; }
            public List<Product>? products { get; set; }
            public ManomanoAmount? products_price { get; set; }
            public ManomanoAmount? products_price_excluding_vat { get; set; }
            public ManomanoAmount? products_price_vat { get; set; }
            public int seller_contract_id { get; set; }
            public ManomanoAmount? seller_discount { get; set; }
            public ManomanoAmount? shipping_discount { get; set; }
            public ManomanoAmount? shipping_price { get; set; }
            public ManomanoAmount? shipping_price_excluding_vat { get; set; }
            public string? shipping_price_vat_rate { get; set; }
            public string? status { get; set; }
            public DateTime status_updated_at { get; set; }
            public ManomanoAmount? total_discount { get; set; }
            public ManomanoAmount? total_price { get; set; }
            public ManomanoAmount? total_price_excluding_vat { get; set; }
            public ManomanoAmount? total_price_vat { get; set; }
        }

        public class Addresses
        {
            public Billing? billing { get; set; }
            public Relay? relay { get; set; }
            public Shipping? shipping { get; set; }
        }

        public class Billing : Address
        {
            public string? recipient_code { get; set; }
        }

        public class Relay : Address
        {
            public string? id { get; set; }
            public string? name { get; set; }
        }

        public class Shipping : Address { }

        public class Address
        {
            public string? address_line1 { get; set; }
            public string? address_line2 { get; set; }
            public string? address_line3 { get; set; }
            public string? city { get; set; }
            public string? company { get; set; }
            public string? country { get; set; }
            public string? country_iso { get; set; }
            public string? email { get; set; }
            public string? firstname { get; set; }
            public string? lastname { get; set; }
            public string? phone { get; set; }
            public string? province { get; set; }
            public string? zipcode { get; set; }
        }

        public class Customer
        {
            public string? firstname { get; set; }
            public string? fiscal_number { get; set; }
            public string? lastname { get; set; }
        }

        public class ManomanoAmount
        {
            public decimal? amount { get; set; }
            public string? currency { get; set; }
        }

        public class Product
        {
            public string? carrier { get; set; }
            public ManomanoAmount? price { get; set; }
            public ManomanoAmount? price_excluding_vat { get; set; }
            public ManomanoAmount? product_price { get; set; }
            public ManomanoAmount? product_price_excluding_vat { get; set; }
            public string? product_title { get; set; }
            public int quantity { get; set; }
            public string? seller_sku { get; set; }
            public ManomanoAmount? shipping_price { get; set; }
            public ManomanoAmount? shipping_price_excluding_vat { get; set; }
            public string? shipping_vat_rate { get; set; }
            public ManomanoAmount? sum_shipping_price { get; set; }
            public string? title { get; set; }
            public ManomanoAmount? total_price { get; set; }
            public ManomanoAmount? total_price_excluding_vat { get; set; }
            public string? vat_rate { get; set; }
        }

        public class Pagination
        {
            public int items { get; set; }
            public int limit { get; set; }
            public Links? links { get; set; }
            public int page { get; set; }
            public int pages { get; set; }
        }

        public class Links
        {
            public string? first { get; set; }
            public string? @goto { get; set; }
            public string? last { get; set; }
            public string? next { get; set; }
            public string? previous { get; set; }
        }
    }

}
