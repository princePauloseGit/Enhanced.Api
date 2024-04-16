using Enhanced.Models.EbayData;
using Enhanced.Models.Shared;

namespace Enhanced.Models.ManoMano
{
    public class ManoApiUrls
    {
        protected static string ProductionApiBaseUrl { get; } = "https://partnersapi.manomano.com";
        protected static string SandboxApiBaseUrl { get; } = "https://partnersapi.sandbox.manomano.com";

        protected class OrderApiUrls
        {
            private readonly static string _resourceBaseUrl = "/orders/v1";

            protected static string ApiBaseUrl
            {
                get
                {
                    return EnvironmentManager.Environment == CommonEnum.Environment.Sandbox ? SandboxApiBaseUrl : ProductionApiBaseUrl;
                }
            }

            public static string GetOrders() => $"{ApiBaseUrl}{_resourceBaseUrl}/orders";

            public static string AcceptOrders() => $"{ApiBaseUrl}{_resourceBaseUrl}/accept-orders";

            public static string CreateShipment() => $"{ApiBaseUrl}{_resourceBaseUrl}/shippings";
        }
    }
}
