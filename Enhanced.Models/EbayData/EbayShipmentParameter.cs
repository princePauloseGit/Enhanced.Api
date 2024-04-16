namespace Enhanced.Models.EbayData
{
    public class EbayShipmentParameter
    {
        public List<CreateShipment>? CreateShipments { get; set; } = new();
    }

    public class CreateShipment
    {
        public string? OrderId { get; set; }
        public ShipmentData? ShipmentData { get; set; }
    }

    public class ShipmentData
    {
        public List<LineItem>? lineItems { get; set; }
        //public string? shippedDate { get; set; }
        public string? shippingCarrierCode { get; set; }
        public string? trackingNumber { get; set; }
    }

    public class LineItem
    {
        public string? lineItemId { get; set; }
        public int quantity { get; set; }
    }

    public class ShippingFilfilmentResponse : BulkResponseBase
    {
        public List<ShipmentData>? fulfillments { get; set; } = new();
        public List<Error>? errors { get; set; } = new();
    }
}
