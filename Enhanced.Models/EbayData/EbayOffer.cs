namespace Enhanced.Models.EbayData
{
    public class BulkOffer
    {
        public List<EbayOffer>? requests { get; set; }
    }

    public class OffersResponse : BulkResponseBase
    {
        public List<EbayOffer>? offers { get; set; }
    }

    public class PublishOffersByInventoryGroup
    {
        public string? inventoryItemGroupKey { get; set; }
        public string? marketplaceId { get; set; }
    }

    public class OfferBase
    {
        public int? availableQuantity { get; set; }
        public string? categoryId { get; set; }
        public string? listingDescription { get; set; }
        public ListingPolicy? listingPolicies { get; set; }
        public string? merchantLocationKey { get; set; }
        public PricingSummary? pricingSummary { get; set; }
        public int? quantityLimitPerBuyer { get; set; }
        public Tax? tax { get; set; }
        public List<string>? storeCategoryNames { get; set; }
        public string? listingDuration { get; set; }
        public int? lotSize { get; set; }
        public Listing? listing { get; set; }
    }

    public class ListingPolicy
    {
        public string? paymentPolicyId { get; set; }
        public string? returnPolicyId { get; set; }
        public List<ShippingCostOverride>? shippingCostOverrides { get; set; }
        public string? fulfillmentPolicyId { get; set; }
        public bool ebayPlusIfEligible { get; set; }
    }

    public class ShippingCostOverride
    {
        public Amount? surcharge { get; set; }
        public Amount? additionalShippingCost { get; set; }
        public Amount? shippingCost { get; set; }
        public int priority { get; set; }
        public string? shippingServiceType { get; set; }
    }

    public class Amount
    {
        public decimal? value { get; set; }
        public string? currency { get; set; }
    }

    public class EbayOffer : OfferBase
    {
        public string? sku { get; set; }
        public string? status { get; set; }
        public string? marketplaceId { get; set; }
        public string? format { get; set; }
        public Listing? listing { get; set; }
        public string? offerId { get; set; }
    }

    public class Listing
    {
        public string? listingId { get; set; }
        public string? listingStatus { get; set; }
        public int? soldQuantity { get; set; }
    }

    public class PricingSummary
    {
        public string? pricingVisibility { get; set; }
        public string? originallySoldForRetailPriceOn { get; set; }
        public Amount? minimumAdvertisedPrice { get; set; }
        public Amount? orignalRetailPrice { get; set; }
        public Amount? price { get; set; }
    }

    public class Tax
    {
        public bool applyTax { get; set; }
        public string? thirdPartyTaxCategory { get; set; }
        public decimal vatPercentage { get; set; }
    }

    public class OfferResponse
    {
        public int statusCode { get; set; }
        public string? sku { get; set; }
        public string? offerId { get; set; }
        public string? listingId { get; set; }
        public string? marketplaceId { get; set; }
        public string? format { get; set; }
        public List<Error>? errors { get; set; }
    }

    public class Error
    {
        public int errorId { get; set; }
        public string? domain { get; set; }
        public string? category { get; set; }
        public string? message { get; set; }
        public string? longMessage { get; set; }
        public List<Parameter>? parameters { get; set; }
    }

    public class GetOfferResponse : BulkResponseBase
    {
        public List<EbayOffer>? offers { get; set; }
    }

    public class Responses
    {
        public List<OfferResponse>? responses { get; set; }
    }

    public class BulkOfferPublish
    {
        public List<OfferPublish>? requests { get; set; }
    }

    public class OfferPublish
    {
        public string? offerId { get; set; }
    }

    public class OfferViewModel
    {
        public OfferViewModel() { }

        public OfferViewModel(EbayOffer offerResponse, OfferParameters offerInput)
        {
            Sku = offerResponse.sku;
            ListingId = offerResponse?.listing?.listingId;
            OfferId = offerResponse?.offerId;
            Action = offerInput.Action;
        }

        public string? Sku { get; set; }
        public string? ListingId { get; set; }
        public string? OfferId { get; set; }       
        public string? Action { get; set; }
    }

    public class ActiveListing
    {
        public string? Sku { get; set; }
        public string? ListingId { get; set; }
        public string? OfferId { get; set; }
    }

    public class DeleteInventoryParameter
    {
        public List<string>? SKUs { get; set; } = new();
    }

    public class UpdateStatus
    {
        public string? Sku { get; set; }
        public bool? Success { get; set; }
    }

    public class EbayErrorResponse
    {
        public List<Error>? errors { get; set; }
    }
}
