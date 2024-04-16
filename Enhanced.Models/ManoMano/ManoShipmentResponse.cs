namespace Enhanced.Models.ManoMano
{
    public class ManoShipmentResponse
    {
        public List<ShipmentContent>? content { get; set; }
        public ShipmentError? Error { get; set; }
        public string? message { get; set; }
    }

    public class ShipmentContent
    {
        public string? order_reference { get; set; }
        public int seller_contract_id { get; set; }
        public ShipmentResult? result { get; set; }
    }

    public class ShipmentError
    {
        public string? app_code { get; set; }
        public string? message { get; set; }
    }

    public class ShipmentResult
    {
        public int http_code { get; set; }
        public ShipmentError? error { get; set; }
    }
}
