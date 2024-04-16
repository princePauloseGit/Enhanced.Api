using Enhanced.Models.AmazonData;

namespace Enhanced.Models.ManoMano
{
    public class AcceptOrderRequest : ParameterBased
    {
        public List<AcceptOrders>? acceptOrders { get; set; } = new();
    }

    public class AcceptOrders
    {
        public string? order_reference { get; set; }
        public int seller_contract_id { get; set; }
    }
}
