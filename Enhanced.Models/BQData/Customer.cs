namespace Enhanced.Models.BQData
{
    public partial class Customer
    {
        public List<BillingAddress>? billing_address { get; set; }

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
    }
}
