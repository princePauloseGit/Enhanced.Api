using Enhanced.Models.Shared;

namespace Enhanced.Models.ManoMano.ManoViewModel
{
    public class ManoShipmentResponseViewModel
    {
        public List<ShippedOrders>? ShippedOrders { get; set; } = new();
        public List<ErrorLog>? ErrorLogs { get; set; } = new List<ErrorLog>();
    }

    public class ShippedOrders
    {
        public string? OrderID { get; set; }
        public int HttpStatusCode { get; set; }
        public string? ErrorMessage { get; set; } 

        public ShippedOrders() { }

        public ShippedOrders(ShipmentContent content)
        {
            OrderID = content?.order_reference;
            HttpStatusCode = content!.result!.http_code;
            ErrorMessage = content?.result?.error?.message ?? string.Empty;
        }
    }
}
