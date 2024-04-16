using Enhanced.Models.AmazonData;

namespace Enhanced.Models.ManoMano
{
    public class CreateShipmentRequest : ParameterBased
    {
        public List<Shipment>? shipmentOrders { get; set; }
    }

    public class Shipment
    {
        public string? carrier { get; set; }
        public string? order_reference { get; set; }
        public int seller_contract_id { get; set; }
        public string? tracking_number { get; set; }
        public string? tracking_url { get; set; }
        public List<ShipmentProduct>? products { get; set; }
    }

    public class ShipmentProduct
    {
        public string? seller_sku { get; set; }
        public int quantity { get; set; }
    }
}
