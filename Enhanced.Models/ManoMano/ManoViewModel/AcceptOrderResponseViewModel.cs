using Enhanced.Models.Shared;

namespace Enhanced.Models.ManoMano.ManoViewModel
{
    public class AcceptOrderResponseViewModel
    {
        public List<AcceptResponse>? AcceptOrders { get; set; } = new();
        public List<ErrorLog>? ErrorLogs { get; set; } = new List<ErrorLog>();

    }

    public class AcceptResponse
    {
        public string? OrderID { get; set; }
        public int HttpStatusCode { get; set; }
        public string? ErrorMessage { get; set; }

        public AcceptResponse() { }

        public AcceptResponse(Content content)
        {
            OrderID = content.order_reference;
            HttpStatusCode = content.result!.http_code;
            ErrorMessage = content.result?.error?.message ?? string.Empty;
        }
    }
}
