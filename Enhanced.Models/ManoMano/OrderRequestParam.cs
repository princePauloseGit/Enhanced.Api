using Enhanced.Models.AmazonData;

namespace Enhanced.Models.ManoMano
{
    public class OrderRequestParam : ParameterBased
    {
        public string? seller_contract_id { get; set; }
        public string? status { get; set; }
        public DateTime? created_at_start { get; set; }
        public DateTime? created_at_end { get; set; }
        public int limit { get; set; } = 50;
        public int page { get; set; }
    }
}
