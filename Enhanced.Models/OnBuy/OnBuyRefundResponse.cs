namespace Enhanced.Models.OnBuy
{
    public class OnBuyRefundResponse
    {
        public OnBuyError? error { get; set; }
        public bool success { get; set; }
        public List<Result>? results { get; set; }
    }

    public class OnBuyError
    {
        public string? message { get; set; }
        public string? errorCode { get; set; }
        public int responseCode { get; set; }
    }

    public class Result
    {
        public bool success { get; set; }
        public string? message { get; set; }
        public string? errorcode { get; set; }
        public string? order_id { get; set; }
    }
}
