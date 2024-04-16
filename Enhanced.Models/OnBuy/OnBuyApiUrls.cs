namespace Enhanced.Models.OnBuy
{
    public class OnBuyApiUrls
    {
        protected static string ProductionApiBaseUrl { get; } = "https://api.onbuy.com";

        protected class OrderApiUrls
        {
            private readonly static string _resourceBaseUrl = "/v2/orders";

            public static string Refund() => $"{ProductionApiBaseUrl}{_resourceBaseUrl}/refund";
        }
    }
}
