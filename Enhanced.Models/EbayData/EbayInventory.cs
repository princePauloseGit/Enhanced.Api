namespace Enhanced.Models.EbayData
{
    public class EbayInventory
    {
        public string? condition { get; set; }
        public Availability? availability { get; set; }
        public string? conditionDescription { get; set; }
        public List<string>? groupIds { get; set; }
        public List<string>? inventoryItemGroupKeys { get; set; }
        public PackageWeightAndSize? packageWeightAndSize { get; set; }
        public Product? product { get; set; }
        public string? sku { get; set; }
        public string? locale { get; set; }
        public decimal? price { get; set; }
        public bool Variation { get; set; }
    }

    public class InventoryItems : BulkResponseBase
    {
        public List<EbayInventory>? inventoryItems { get; set; }
    }

    public class BulkInventoryItem
    {
        public List<EbayInventory>? requests { get; set; } = new();
    }

    public class InventoryItemGroup
    {
        public Dictionary<string, List<string>>? aspects { get; set; }
        public string? description { get; set; }
        public string? inventoryItemGroupKey { get; set; }
        public List<string>? imageUrls { get; set; }
        public string? subtitle { get; set; }
        public string? title { get; set; }
        public List<string>? variantSKUs { get; set; }
        public VariesBy? variesBy { get; set; }

        public void AddAspect(string name, List<string> values)
        {
            aspects ??= new Dictionary<string, List<string>>();
            aspects[name] = values;
        }

        public void AddAspect(string name, string value)
        {
            aspects ??= new Dictionary<string, List<string>>();

            if (aspects[name] == null)
            {
                aspects[name] = new List<string>();
            }

            aspects[name].Add(value);
        }

        public void AddImageUrl(string imageUrl)
        {
            imageUrls ??= new List<string>();
            imageUrls.Add(imageUrl);
        }

        public void AddVariantSKU(string variantSKU)
        {
            variantSKUs ??= new List<string>();
            variantSKUs.Add(variantSKU);
        }
    }

    public class VariesBy
    {
        public List<string>? aspectsImageVariesBy { get; set; }
        public List<Specification>? specifications { get; set; }

        public void AddAspectImageVaryBy(string aspect)
        {
            aspectsImageVariesBy ??= new List<string>();
            aspectsImageVariesBy.Add(aspect);
        }
    }

    public class Specification
    {
        public string? name { get; set; }
        public List<string>? values { get; set; }
    }

    public class Availability
    {
        public List<PickupAtLocationAvailability>? pickupAtLocationAvailability { get; set; }
        public ShipToLocationAvailability? shipToLocationAvailability { get; set; }
    }

    public class PickupAtLocationAvailability
    {
        public string? availabilityType { get; set; }
        public FulfillmentTime? fulfillmentTime { get; set; }
        public string? merchantLocationKey { get; set; }
        public int? quantity { get; set; }
    }

    public class ShipToLocationAvailability
    {
        public AllocationByFormat? allocationByFormat { get; set; }
        public List<AvailabilityDistribution>? availabilityDistributions { get; set; }
        public int? quantity { get; set; }
    }

    public class AllocationByFormat
    {
        public string? auction { get; set; }
        public string? fixedPrice { get; set; }
    }

    public class AvailabilityDistribution
    {
        public FulfillmentTime? fulfillmentTime { get; set; }
        public string? merchantLocationKey { get; set; }
        public int? quantity { get; set; }
    }

    public class FulfillmentTime
    {
        public string? unit { get; set; }
        public int? value { get; set; }
    }

    public class PackageWeightAndSize
    {
        public Dimensions? dimensions { get; set; }
        public string? packageType { get; set; }
        public Weight? weight { get; set; }
    }

    public class Weight
    {
        public string? unit { get; set; }
        public int? value { get; set; }
    }

    public class Dimensions
    {
        public int? height { get; set; }
        public int? length { get; set; }
        public int? width { get; set; }
        public string? unit { get; set; }
    }

    public class Product
    {
        public Dictionary<string, List<string>>? aspects { get; set; }
        public string? brand { get; set; }
        public string? description { get; set; }
        public List<string>? imageUrls { get; set; }
        public string? mpn { get; set; }
        public string? subTitle { get; set; }
        public string? title { get; set; }
        public List<string>? isbn { get; set; }
        public List<string>? upc { get; set; }
        public List<string>? ean { get; set; }
        public List<string>? videoIds { get; set; }
        public string? epid { get; set; }

        public void AddAspect(string name, List<string> values)
        {
            aspects ??= new Dictionary<string, List<string>>();
            aspects[name] = values;
        }
    }

    public class BulkInventoryItemResponses
    {
        public List<InventoryItemResponse>? responses { get; set; }
    }

    public class InventoryItemResponse : ErrorsWarnings
    {
        public int statusCode { get; set; }
        public string? sku { get; set; }
        public string? locale { get; set; }
    }

    public class OfferParameters
    {
        public string? SKU { get; set; }
        public string? Action { get; set; }
    }

    public class BulkInventoryParameters
    {
        public List<EbayInventory>? Listing { get; set; } = new();
        public List<EbayOffer>? Offers { get; set; } = new();
    }

    public class GroupInventoryParameters : BulkInventoryParameters
    {
        public List<InventoryItemGroup>? Groups { get; set; } = new();
    }

    public class BulkUpdateInventoryItem
    {
        public List<InventoryPriceQuantity>? requests { get; set; }
    }

    public class InventoryPriceQuantity
    {
        public List<Offer>? offers { get; set; }
        public ShipToLocationAvailability? shipToLocationAvailability { get; set; }
        public string? sku { get; set; }
    }

    public class Offer
    {
        public int? availableQuantity { get; set; }
        public string? offerId { get; set; }
        public Amount? price { get; set; }
    }

    public class ErrorsWarnings
    {
        public List<Warning>? warnings { get; set; }
        public List<Error>? errors { get; set; } = new();
    }
}
