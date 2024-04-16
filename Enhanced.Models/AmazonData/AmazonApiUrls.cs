namespace Enhanced.Models.AmazonData
{
    public class AmazonApiUrls
    {        
        public class OrdersApiUrls
        {
            private readonly static string _resourceBaseUrl = "/orders/v0";
            public static string Orders
            {
                get => $"{_resourceBaseUrl}/orders";
            }

            public static string OrderItems(string orderId) => $"{_resourceBaseUrl}/orders/{orderId}/orderItems";

            public static string ShipmentConfirmation(string orderId) => $"{_resourceBaseUrl}/orders/{orderId}/shipmentConfirmation";
        }

        protected class CatalogApiUrls
        {
            private readonly static string _202204resourceBaseUrl = "/catalog/2022-04-01";

            public static string GetCatalogItem202204(string asin) => $"{_202204resourceBaseUrl}/items/{asin}";

            public static string SearchCatalogItems202204 => $"{_202204resourceBaseUrl}/items";
        }

        protected class FeedsApiUrls
        {
            private readonly static string _resourceBaseUrl = "/feeds/2021-06-30";

            public static string CreateFeed
            {
                get => $"{_resourceBaseUrl}/feeds";
            }

            public static string CreateFeedDocument
            {
                get => $"{_resourceBaseUrl}/documents";
            }
        }

        protected class ReportApiUrls
        {
            private readonly static string _resourceBaseUrl = "/reports/2021-06-30";
            public static string GetReports
            {
                get => $"{_resourceBaseUrl}/reports";
            }
            public static string GetReportDocument(string reportDocumentId) => $"{_resourceBaseUrl}/documents/{reportDocumentId}";
        }

        protected class TokenApiUrls
        {
            private readonly static string _resourceBaseUrl = "/tokens/2021-03-01";

            public static string RestrictedDataToken
            {
                get => $"{_resourceBaseUrl}/restrictedDataToken";
            }
        }
    }
}
