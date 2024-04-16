namespace Enhanced.Models.ManoMano
{
    public class AcceptOrderResponse
    {
        public List<Content>? content { get; set; }
        public Error? Error { get; set; }
        public string? message { get; set; }
    }

    public class Content
    {
        public string? order_reference { get; set; }
        public int seller_contract_id { get; set; }
        public Result? result { get; set; }
    }

    public class Error
    {
        public string? app_code { get; set; }
        public string? message { get; set; }
    }

    public class Result
    {
        public int http_code { get; set; }
        public Error? error { get; set; }
    }
}
