using Enhanced.Models.Shared;

namespace Enhanced.Models.EbayData
{
    public class EbayApiUrls
    {
        protected static string EbaySandboxBaseUrlZ { get; } = "https://apiz.sandbox.ebay.com";
        protected static string EbayProductionUrlZ { get; } = "https://apiz.ebay.com";
        protected static string EbaySandboxBaseUrl { get; } = "https://api.sandbox.ebay.com";
        protected static string EbayProductionUrl { get; } = "https://api.ebay.com";
        protected static string SignInSandboxUrl { get; } = "https://auth.sandbox.ebay.com";
        protected static string SignInProductionUrl { get; } = "https://signin.ebay.com";
        protected static string UkEbayMarketPlaceId { get; } = "EBAY_GB";
        protected static string ContentLanguage { get; } = "en-GB";

        protected static string ApiBaseUrl
        {
            get
            {
                return EnvironmentManager.Environment == CommonEnum.Environment.Sandbox ? EbaySandboxBaseUrl : EbayProductionUrl;
            }
        }

        protected static string ApiBaseUrlZ
        {
            get
            {
                return EnvironmentManager.Environment == CommonEnum.Environment.Sandbox ? EbaySandboxBaseUrlZ : EbayProductionUrlZ;
            }
        }

        protected static string SingInBaseUrl
        {
            get
            {
                return EnvironmentManager.Environment == CommonEnum.Environment.Sandbox ? SignInSandboxUrl : SignInProductionUrl;
            }
        }

        protected class OAuthUrls
        {
            public static string AuthUrl
            {
                get => $"{SingInBaseUrl}/oauth2/authorize";
            }

            public static string RefreshTokenUrl
            {
                get => $"{ApiBaseUrl}/identity/v1/oauth2/token";
            }
        }

        protected class InventoryApiUrls
        {
            private readonly static string _resourceBaseUrl = "/sell/inventory/v1";

            public static string BulkCreateOrReplaceInventoryItemUrl
            {
                get => $"{ApiBaseUrl}{_resourceBaseUrl}/bulk_create_or_replace_inventory_item";
            }

            public static string CreateOrReplaceInventoryItemGroupUrl(string inventoryItemGroupKey) => $"{ApiBaseUrl}{_resourceBaseUrl}/inventory_item_group/{inventoryItemGroupKey}";

            public static string GetOffers(string sku) => $"{ApiBaseUrl}{_resourceBaseUrl}/offer?sku={sku}";

            public static string BulkCreateOffer
            {
                get => $"{ApiBaseUrl}{_resourceBaseUrl}/bulk_create_offer";
            }

            public static string BulkUpdatePriceQuantity
            {
                get => $"{ApiBaseUrl}{_resourceBaseUrl}/bulk_update_price_quantity";
            }

            public static string BulkPublishOffer
            {
                get => $"{ApiBaseUrl}{_resourceBaseUrl}/bulk_publish_offer";
            }

            public static string PublishByInventoryItemGroup
            {
                get => $"{ApiBaseUrl}{_resourceBaseUrl}/offer/publish_by_inventory_item_group";
            }

            public static string UpdateOffer(string offerId) => $"{ApiBaseUrl}{_resourceBaseUrl}/offer/{offerId}";

            public static string DeleteInventoryItem(string sku) => $"{ApiBaseUrl}{_resourceBaseUrl}/inventory_item/{sku}";

            public static string GetInventoryItem(string sku) => $"{ApiBaseUrl}{_resourceBaseUrl}/inventory_item/{sku}";

            public static string DeleteInventoryItemGroup(string inventoryItemGroupKey) => $"{ApiBaseUrl}{_resourceBaseUrl}/inventory_item_group/{inventoryItemGroupKey}";
        }

        protected class FulfillmentApiUrls
        {
            private readonly static string _resourceBaseUrl = "/sell/fulfillment/v1";

            public static string Order
            {
                get => $"{ApiBaseUrl}{_resourceBaseUrl}/order";
            }

            public static string CreateShippingApiUrl(string orderId) => $"{ApiBaseUrl}{_resourceBaseUrl}/order/{orderId}/shipping_fulfillment";

            public static string Refund(string orderId) => $"{ApiBaseUrl}{_resourceBaseUrl}/order/{orderId}/issue_refund";
        }

        protected class FinanceApiUrls
        {
            private readonly static string _resourceBaseUrl = "/sell/finances/v1";

            public static string Transaction
            {
                get => $"{ApiBaseUrlZ}{_resourceBaseUrl}/transaction";
            }

            public static string Payout
            {
                get => $"{ApiBaseUrlZ}{_resourceBaseUrl}/payout";
            }
        }
    }

    public static class EnvironmentManager
    {
        public static CommonEnum.Environment Environment { get; set; } = CommonEnum.Environment.Sandbox;
    }
}
