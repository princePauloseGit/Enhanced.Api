using System.Runtime.Serialization;
using static Enhanced.Models.AmazonData.AmazonEnum;

namespace Enhanced.Models.AmazonData
{
    /// <summary>
    /// An item in the Amazon catalog.
    /// </summary>
    [DataContract]
    public partial class CatalogItem
    {
        /// <summary>
        /// Gets or Sets Asin
        /// </summary>
        [DataMember(Name = "asin", EmitDefaultValue = false)]
        public string? Asin { get; set; }

        /// <summary>
        /// Gets or Sets Attributes
        /// </summary>
        [DataMember(Name = "attributes", EmitDefaultValue = false)]
        public Dictionary<String, Object>? Attributes { get; set; }

        /// <summary>
        /// Gets or Sets Dimensions
        /// </summary>
        [DataMember(Name = "dimensions", EmitDefaultValue = false)]
        public List<ItemDimensions>? Dimensions { get; set; }

        /// <summary>
        /// Gets or Sets Identifiers
        /// </summary>
        [DataMember(Name = "identifiers", EmitDefaultValue = false)]
        public List<ItemIdentifiers>? Identifiers { get; set; }

        /// <summary>
        /// Gets or Sets Images
        /// </summary>
        [DataMember(Name = "images", EmitDefaultValue = false)]
        public List<ItemImages>? Images { get; set; }

        /// <summary>
        /// Gets or Sets ProductTypes
        /// </summary>
        [DataMember(Name = "productTypes", EmitDefaultValue = false)]
        public List<ItemProductTypes>? ProductTypes { get; set; }

        /// <summary>
        /// Gets or Sets Relationships
        /// </summary>
        [DataMember(Name = "relationships", EmitDefaultValue = false)]
        public List<ItemRelationships>? Relationships { get; set; }

        /// <summary>
        /// Gets or Sets SalesRanks
        /// </summary>
        [DataMember(Name = "salesRanks", EmitDefaultValue = false)]
        public List<ItemSalesRanks>? SalesRanks { get; set; }

        /// <summary>
        /// Gets or Sets Summaries
        /// </summary>
        [DataMember(Name = "summaries", EmitDefaultValue = false)]
        public List<ItemSummaries>? Summaries { get; set; }

        /// <summary>
        /// Gets or Sets VendorDetails
        /// </summary>
        [DataMember(Name = "vendorDetails", EmitDefaultValue = false)]
        public List<ItemVendorDetails>? VendorDetails { get; set; }
    }

    /// <summary>
    /// Dimensions associated with the item in the Amazon catalog for the indicated Amazon marketplace.
    /// </summary>
    [DataContract]
    public partial class ItemDimensions
    {
        /// <summary>
        /// Amazon marketplace identifier.
        /// </summary>
        /// <value>Amazon marketplace identifier.</value>
        [DataMember(Name = "marketplaceId", EmitDefaultValue = false)]
        public string? MarketplaceId { get; set; }

        /// <summary>
        /// Dimensions of an Amazon catalog item.
        /// </summary>
        /// <value>Dimensions of an Amazon catalog item.</value>
        [DataMember(Name = "item", EmitDefaultValue = false)]
        public Dimensions? Item { get; set; }

        /// <summary>
        /// Dimensions of an Amazon catalog item in its packaging.
        /// </summary>
        /// <value>Dimensions of an Amazon catalog item in its packaging.</value>
        [DataMember(Name = "package", EmitDefaultValue = false)]
        public Dimensions? Package { get; set; }
    }

    /// <summary>
    /// Dimensions of an Amazon catalog item or item in its packaging.
    /// </summary>
    [DataContract]
    public partial class Dimensions
    {
        /// <summary>
        /// Height of an item or item package.
        /// </summary>
        /// <value>Height of an item or item package.</value>
        [DataMember(Name = "height", EmitDefaultValue = false)]
        public Dimension? Height { get; set; }

        /// <summary>
        /// Length of an item or item package.
        /// </summary>
        /// <value>Length of an item or item package.</value>
        [DataMember(Name = "length", EmitDefaultValue = false)]
        public Dimension? Length { get; set; }

        /// <summary>
        /// Weight of an item or item package.
        /// </summary>
        /// <value>Weight of an item or item package.</value>
        [DataMember(Name = "weight", EmitDefaultValue = false)]
        public Dimension? Weight { get; set; }

        /// <summary>
        /// Width of an item or item package.
        /// </summary>
        /// <value>Width of an item or item package.</value>
        [DataMember(Name = "width", EmitDefaultValue = false)]
        public Dimension? Width { get; set; }
    }

    /// <summary>
    /// Individual dimension value of an Amazon catalog item or item package.
    /// </summary>
    [DataContract]
    public partial class Dimension
    {
        /// <summary>
        /// Measurement unit of the dimension value.
        /// </summary>
        /// <value>Measurement unit of the dimension value.</value>
        [DataMember(Name = "unit", EmitDefaultValue = false)]
        public string? Unit { get; set; }

        /// <summary>
        /// Numeric dimension value.
        /// </summary>
        /// <value>Numeric dimension value.</value>
        [DataMember(Name = "value", EmitDefaultValue = false)]
        public decimal? Value { get; set; }
    }

    // <summary>
    /// Identifiers associated with the item in the Amazon catalog for the indicated Amazon marketplace.
    /// </summary>
    [DataContract]
    public partial class ItemIdentifiers
    {
        /// <summary>
        /// Amazon marketplace identifier.
        /// </summary>
        /// <value>Amazon marketplace identifier.</value>
        [DataMember(Name = "marketplaceId", EmitDefaultValue = false)]
        public string? MarketplaceId { get; set; }

        /// <summary>
        /// Identifiers associated with the item in the Amazon catalog for the indicated Amazon marketplace.
        /// </summary>
        /// <value>Identifiers associated with the item in the Amazon catalog for the indicated Amazon marketplace.</value>
        [DataMember(Name = "identifiers", EmitDefaultValue = false)]
        public List<ItemIdentifier>? Identifiers { get; set; }
    }

    /// <summary>
    /// Identifier associated with the item in the Amazon catalog, such as a UPC or EAN identifier.
    /// </summary>
    [DataContract]
    public partial class ItemIdentifier
    {
        /// <summary>
        /// Type of identifier, such as UPC, EAN, or ISBN.
        /// </summary>
        /// <value>Type of identifier, such as UPC, EAN, or ISBN.</value>
        [DataMember(Name = "identifierType", EmitDefaultValue = false)]
        public string? IdentifierType { get; set; }

        /// <summary>
        /// Identifier.
        /// </summary>
        /// <value>Identifier.</value>
        [DataMember(Name = "identifier", EmitDefaultValue = false)]
        public string? Identifier { get; set; }
    }

    /// <summary>
    /// Images for an item in the Amazon catalog.
    /// </summary>
    [DataContract]
    public partial class ItemImages
    {
        /// <summary>
        /// Amazon marketplace identifier.
        /// </summary>
        /// <value>Amazon marketplace identifier.</value>
        [DataMember(Name = "marketplaceId", EmitDefaultValue = false)]
        public string? MarketplaceId { get; set; }

        /// <summary>
        /// Images for an item in the Amazon catalog for the indicated Amazon marketplace.
        /// </summary>
        /// <value>Images for an item in the Amazon catalog for the indicated Amazon marketplace.</value>
        [DataMember(Name = "images", EmitDefaultValue = false)]
        public List<ItemImage>? Images { get; set; }
    }

    /// <summary>
    /// Image for an item in the Amazon catalog.
    /// </summary>
    [DataContract]
    public partial class ItemImage
    {
        /// <summary>
        /// Variant of the image, such as &#x60;MAIN&#x60; or &#x60;PT01&#x60;.
        /// </summary>
        /// <value>Variant of the image, such as &#x60;MAIN&#x60; or &#x60;PT01&#x60;.</value>
        [DataMember(Name = "variant", EmitDefaultValue = false)]
        public string? Variant { get; set; }

        /// <summary>
        /// Link, or URL, for the image.
        /// </summary>
        /// <value>Link, or URL, for the image.</value>
        [DataMember(Name = "link", EmitDefaultValue = false)]
        public string? Link { get; set; }

        /// <summary>
        /// Height of the image in pixels.
        /// </summary>
        /// <value>Height of the image in pixels.</value>
        [DataMember(Name = "height", EmitDefaultValue = false)]
        public int? Height { get; set; }

        /// <summary>
        /// Width of the image in pixels.
        /// </summary>
        /// <value>Width of the image in pixels.</value>
        [DataMember(Name = "width", EmitDefaultValue = false)]
        public int? Width { get; set; }
    }

    /// <summary>
    /// Product types associated with the Amazon catalog item.
    /// </summary>
    [DataContract]
    public partial class ItemProductTypes
    {
        /// <summary>
        /// Amazon marketplace identifier.
        /// </summary>
        /// <value>Amazon marketplace identifier.</value>
        [DataMember(Name = "marketplaceId", EmitDefaultValue = false)]
        public string? MarketplaceId { get; set; }

        /// <summary>
        /// Name of the product type associated with the Amazon catalog item.
        /// </summary>
        /// <value>Name of the product type associated with the Amazon catalog item.</value>
        [DataMember(Name = "productType", EmitDefaultValue = false)]
        public string? ProductType { get; set; }
    }

    /// <summary>
    /// Relationships by marketplace for an Amazon catalog item (for example, variations).
    /// </summary>
    [DataContract]
    public partial class ItemRelationships
    {
        /// <summary>
        /// Amazon marketplace identifier.
        /// </summary>
        /// <value>Amazon marketplace identifier.</value>
        [DataMember(Name = "marketplaceId", EmitDefaultValue = false)]
        public string? MarketplaceId { get; set; }

        /// <summary>
        /// Relationships for the item.
        /// </summary>
        /// <value>Relationships for the item.</value>
        [DataMember(Name = "relationships", EmitDefaultValue = false)]
        public List<ItemRelationship>? Relationships { get; set; }
    }

    /// <summary>
    /// Relationship details for an Amazon catalog item.
    /// </summary>
    [DataContract]
    public partial class ItemRelationship
    {
        /// <summary>
        /// Type of relationship.
        /// </summary>
        /// <value>Type of relationship.</value>
        [DataMember(Name = "type", EmitDefaultValue = false)]
        public string? Type { get; set; }

        /// <summary>
        /// Identifiers (ASINs) of the related items that are children of this item.
        /// </summary>
        /// <value>Identifiers (ASINs) of the related items that are children of this item.</value>
        [DataMember(Name = "childAsins", EmitDefaultValue = false)]
        public List<string>? ChildAsins { get; set; }

        /// <summary>
        /// Identifiers (ASINs) of the related items that are parents of this item.
        /// </summary>
        /// <value>Identifiers (ASINs) of the related items that are parents of this item.</value>
        [DataMember(Name = "parentAsins", EmitDefaultValue = false)]
        public List<string>? ParentAsins { get; set; }

        /// <summary>
        /// For \&quot;VARIATION\&quot; relationships, variation theme indicating the combination of Amazon item catalog attributes that define the variation family.
        /// </summary>
        /// <value>For \&quot;VARIATION\&quot; relationships, variation theme indicating the combination of Amazon item catalog attributes that define the variation family.</value>
        [DataMember(Name = "variationTheme", EmitDefaultValue = false)]
        public ItemVariationTheme? VariationTheme { get; set; }
    }

    /// <summary>
    /// Variation theme indicating the combination of Amazon item catalog attributes that define the variation family.
    /// </summary>
    [DataContract]
    public partial class ItemVariationTheme
    {
        /// <summary>
        /// Names of the Amazon catalog item attributes associated with the variation theme.
        /// </summary>
        /// <value>Names of the Amazon catalog item attributes associated with the variation theme.</value>
        [DataMember(Name = "attributes", EmitDefaultValue = false)]
        public List<string>? Attributes { get; set; }

        /// <summary>
        /// Variation theme indicating the combination of Amazon item catalog attributes that define the variation family.
        /// </summary>
        /// <value>Variation theme indicating the combination of Amazon item catalog attributes that define the variation family.</value>
        [DataMember(Name = "theme", EmitDefaultValue = false)]
        public string? Theme { get; set; }
    }

    /// <summary>
    /// Sales ranks of an Amazon catalog item.
    /// </summary>
    [DataContract]
    public partial class ItemSalesRanks
    {
        /// <summary>
        /// Amazon marketplace identifier.
        /// </summary>
        /// <value>Amazon marketplace identifier.</value>
        [DataMember(Name = "marketplaceId", EmitDefaultValue = false)]
        public string? MarketplaceId { get; set; }

        /// <summary>
        /// Sales ranks of an Amazon catalog item for an Amazon marketplace by classification.
        /// </summary>
        /// <value>Sales ranks of an Amazon catalog item for an Amazon marketplace by classification.</value>
        [DataMember(Name = "classificationRanks", EmitDefaultValue = false)]
        public List<ItemClassificationSalesRank>? ClassificationRanks { get; set; }

        /// <summary>
        /// Sales ranks of an Amazon catalog item for an Amazon marketplace by website display group.
        /// </summary>
        /// <value>Sales ranks of an Amazon catalog item for an Amazon marketplace by website display group.</value>
        [DataMember(Name = "displayGroupRanks", EmitDefaultValue = false)]
        public List<ItemDisplayGroupSalesRank>? DisplayGroupRanks { get; set; }
    }

    /// <summary>
    /// Sales rank of an Amazon catalog item by classification.
    /// </summary>
    [DataContract]
    public partial class ItemClassificationSalesRank
    {
        /// <summary>
        /// Identifier of the classification associated with the sales rank.
        /// </summary>
        /// <value>Identifier of the classification associated with the sales rank.</value>
        [DataMember(Name = "classificationId", EmitDefaultValue = false)]
        public string? ClassificationId { get; set; }

        /// <summary>
        /// Title, or name, of the sales rank.
        /// </summary>
        /// <value>Title, or name, of the sales rank.</value>
        [DataMember(Name = "title", EmitDefaultValue = false)]
        public string? Title { get; set; }

        /// <summary>
        /// Corresponding Amazon retail website link, or URL, for the sales rank.
        /// </summary>
        /// <value>Corresponding Amazon retail website link, or URL, for the sales rank.</value>
        [DataMember(Name = "link", EmitDefaultValue = false)]
        public string? Link { get; set; }

        /// <summary>
        /// Sales rank value.
        /// </summary>
        /// <value>Sales rank value.</value>
        [DataMember(Name = "rank", EmitDefaultValue = false)]
        public int? Rank { get; set; }
    }

    /// <summary>
    /// Sales rank of an Amazon catalog item by website display group.
    /// </summary>
    [DataContract]
    public partial class ItemDisplayGroupSalesRank
    {
        /// <summary>
        /// Name of the website display group associated with the sales rank
        /// </summary>
        /// <value>Name of the website display group associated with the sales rank</value>
        [DataMember(Name = "websiteDisplayGroup", EmitDefaultValue = false)]
        public string? WebsiteDisplayGroup { get; set; }

        /// <summary>
        /// Title, or name, of the sales rank.
        /// </summary>
        /// <value>Title, or name, of the sales rank.</value>
        [DataMember(Name = "title", EmitDefaultValue = false)]
        public string? Title { get; set; }

        /// <summary>
        /// Corresponding Amazon retail website link, or URL, for the sales rank.
        /// </summary>
        /// <value>Corresponding Amazon retail website link, or URL, for the sales rank.</value>
        [DataMember(Name = "link", EmitDefaultValue = false)]
        public string? Link { get; set; }

        /// <summary>
        /// Sales rank value.
        /// </summary>
        /// <value>Sales rank value.</value>
        [DataMember(Name = "rank", EmitDefaultValue = false)]
        public int? Rank { get; set; }
    }

    /// <summary>
    /// Summary details of an Amazon catalog item.
    /// </summary>
    [DataContract]
    public partial class ItemSummaries
    {
        /// <summary>
        /// Classification type associated with the Amazon catalog item.
        /// </summary>
        /// <value>Classification type associated with the Amazon catalog item.</value>
        [DataMember(Name = "itemClassification", EmitDefaultValue = false)]
        public string? ItemClassification { get; set; }

        // Giving Error - commented for now
        // public ItemClassification? ItemClassification { get; set; }

        /// <summary>
        /// Amazon marketplace identifier.
        /// </summary>
        /// <value>Amazon marketplace identifier.</value>
        [DataMember(Name = "marketplaceId", EmitDefaultValue = false)]
        public string? MarketplaceId { get; set; }

        /// <summary>
        /// Name of the brand associated with an Amazon catalog item.
        /// </summary>
        /// <value>Name of the brand associated with an Amazon catalog item.</value>
        [DataMember(Name = "brand", EmitDefaultValue = false)]
        public string? Brand { get; set; }

        /// <summary>
        /// Classification (browse node) associated with an Amazon catalog item.
        /// </summary>
        /// <value>Classification (browse node) associated with an Amazon catalog item.</value>
        [DataMember(Name = "browseClassification", EmitDefaultValue = false)]
        public ItemBrowseClassification? BrowseClassification { get; set; }

        /// <summary>
        /// Name of the color associated with an Amazon catalog item.
        /// </summary>
        /// <value>Name of the color associated with an Amazon catalog item.</value>
        [DataMember(Name = "color", EmitDefaultValue = false)]
        public string? Color { get; set; }

        /// <summary>
        /// Name, or title, associated with an Amazon catalog item.
        /// </summary>
        /// <value>Name, or title, associated with an Amazon catalog item.</value>
        [DataMember(Name = "itemName", EmitDefaultValue = false)]
        public string? ItemName { get; set; }

        /// <summary>
        /// Name of the manufacturer associated with an Amazon catalog item.
        /// </summary>
        /// <value>Name of the manufacturer associated with an Amazon catalog item.</value>
        [DataMember(Name = "manufacturer", EmitDefaultValue = false)]
        public string? Manufacturer { get; set; }

        /// <summary>
        /// Model number associated with an Amazon catalog item.
        /// </summary>
        /// <value>Model number associated with an Amazon catalog item.</value>
        [DataMember(Name = "modelNumber", EmitDefaultValue = false)]
        public string? ModelNumber { get; set; }

        /// <summary>
        /// Quantity of an Amazon catalog item in one package.
        /// </summary>
        /// <value>Quantity of an Amazon catalog item in one package.</value>
        [DataMember(Name = "packageQuantity", EmitDefaultValue = false)]
        public int? PackageQuantity { get; set; }

        /// <summary>
        /// Part number associated with an Amazon catalog item.
        /// </summary>
        /// <value>Part number associated with an Amazon catalog item.</value>
        [DataMember(Name = "partNumber", EmitDefaultValue = false)]
        public string? PartNumber { get; set; }

        /// <summary>
        /// Name of the size associated with an Amazon catalog item.
        /// </summary>
        /// <value>Name of the size associated with an Amazon catalog item.</value>
        [DataMember(Name = "size", EmitDefaultValue = false)]
        public string? Size { get; set; }

        /// <summary>
        /// Name of the style associated with an Amazon catalog item.
        /// </summary>
        /// <value>Name of the style associated with an Amazon catalog item.</value>
        [DataMember(Name = "style", EmitDefaultValue = false)]
        public string? Style { get; set; }

        /// <summary>
        /// Name of the website display group associated with an Amazon catalog item.
        /// </summary>
        /// <value>Name of the website display group associated with an Amazon catalog item.</value>
        [DataMember(Name = "websiteDisplayGroup", EmitDefaultValue = false)]
        public string? WebsiteDisplayGroup { get; set; }

        /// <summary>
        /// Name of the website display group associated with an Amazon catalog item.
        /// </summary>
        /// <value>Name of the website display group associated with an Amazon catalog item.</value>
        [DataMember(Name = "websiteDisplayGroupName", EmitDefaultValue = false)]
        public string? WebsiteDisplayGroupName { get; set; }
    }

    /// <summary>
    /// Classification (browse node) associated with an Amazon catalog item.
    /// </summary>
    [DataContract]
    public partial class ItemBrowseClassification
    {
        /// <summary>
        /// Display name for the classification.
        /// </summary>
        /// <value>Display name for the classification.</value>
        [DataMember(Name = "displayName", EmitDefaultValue = false)]
        public string? DisplayName { get; set; }

        /// <summary>
        /// Identifier of the classification (browse node identifier).
        /// </summary>
        /// <value>Identifier of the classification (browse node identifier).</value>
        [DataMember(Name = "classificationId", EmitDefaultValue = false)]
        public string? ClassificationId { get; set; }
    }

    /// <summary>
    /// Vendor details associated with an Amazon catalog item. Vendor details are available to vendors only.
    /// </summary>
    [DataContract]
    public partial class ItemVendorDetails
    {
        /// <summary>
        /// Replenishment category associated with an Amazon catalog item.
        /// </summary>
        /// <value>Replenishment category associated with an Amazon catalog item.</value>
        [DataMember(Name = "replenishmentCategory", EmitDefaultValue = false)]
        public ReplenishmentCategory? ReplenishmentCategory { get; set; }

        /// <summary>
        /// Amazon marketplace identifier.
        /// </summary>
        /// <value>Amazon marketplace identifier.</value>
        [DataMember(Name = "marketplaceId", EmitDefaultValue = false)]
        public string? MarketplaceId { get; set; }

        /// <summary>
        /// Brand code associated with an Amazon catalog item.
        /// </summary>
        /// <value>Brand code associated with an Amazon catalog item.</value>
        [DataMember(Name = "brandCode", EmitDefaultValue = false)]
        public string? BrandCode { get; set; }

        /// <summary>
        /// Manufacturer code associated with an Amazon catalog item.
        /// </summary>
        /// <value>Manufacturer code associated with an Amazon catalog item.</value>
        [DataMember(Name = "manufacturerCode", EmitDefaultValue = false)]
        public string? ManufacturerCode { get; set; }

        /// <summary>
        /// Parent vendor code of the manufacturer code.
        /// </summary>
        /// <value>Parent vendor code of the manufacturer code.</value>
        [DataMember(Name = "manufacturerCodeParent", EmitDefaultValue = false)]
        public string? ManufacturerCodeParent { get; set; }

        /// <summary>
        /// Product category associated with an Amazon catalog item.
        /// </summary>
        /// <value>Product category associated with an Amazon catalog item.</value>
        [DataMember(Name = "productCategory", EmitDefaultValue = false)]
        public ItemVendorDetailsCategory? ProductCategory { get; set; }

        /// <summary>
        /// Product group associated with an Amazon catalog item.
        /// </summary>
        /// <value>Product group associated with an Amazon catalog item.</value>
        [DataMember(Name = "productGroup", EmitDefaultValue = false)]
        public string? ProductGroup { get; set; }

        /// <summary>
        /// Product subcategory associated with an Amazon catalog item.
        /// </summary>
        /// <value>Product subcategory associated with an Amazon catalog item.</value>
        [DataMember(Name = "productSubcategory", EmitDefaultValue = false)]
        public ItemVendorDetailsCategory? ProductSubcategory { get; set; }
    }

    /// <summary>
    /// Product category or subcategory associated with an Amazon catalog item.
    /// </summary>
    [DataContract]
    public partial class ItemVendorDetailsCategory
    {
        /// <summary>
        /// Display name of the product category or subcategory
        /// </summary>
        /// <value>Display name of the product category or subcategory</value>
        [DataMember(Name = "displayName", EmitDefaultValue = false)]
        public string? DisplayName { get; set; }

        /// <summary>
        /// Value (code) of the product category or subcategory.
        /// </summary>
        /// <value>Value (code) of the product category or subcategory.</value>
        [DataMember(Name = "value", EmitDefaultValue = false)]
        public string? Value { get; set; }
    }

    /// <summary>
    /// ASIN Item Dimensions
    /// </summary>
    [DataContract]
    public partial class ASINItemDimensions
    {
        /// <summary>
        /// Amazon marketplace identifier.
        /// </summary>
        /// <value>Amazon marketplace identifier.</value>
        [DataMember(Name = "asin", EmitDefaultValue = false)]
        public string? ASIN { get; set; }

        /// <summary>
        /// Amazon marketplace identifier.
        /// </summary>
        /// <value>Amazon marketplace identifier.</value>
        [DataMember(Name = "sku", EmitDefaultValue = false)]
        public string? SKU { get; set; }

        /// <summary>
        /// Height of an item or item package.
        /// </summary>
        /// <value>Height of an item or item package.</value>
        [DataMember(Name = "height", EmitDefaultValue = false)]
        public Dimension? Height { get; set; }

        /// <summary>
        /// Length of an item or item package.
        /// </summary>
        /// <value>Length of an item or item package.</value>
        [DataMember(Name = "length", EmitDefaultValue = false)]
        public Dimension? Length { get; set; }

        /// <summary>
        /// Weight of an item or item package.
        /// </summary>
        /// <value>Weight of an item or item package.</value>
        [DataMember(Name = "weightinkg", EmitDefaultValue = false)]
        public Dimension? WeightInKg { get; set; }

        /// <summary>
        /// Width of an itdecimalDimensionem or item package.
        /// </summary>
        /// <value>Width of an item or item package.</value>
        [DataMember(Name = "width", EmitDefaultValue = false)]
        public Dimension? Width { get; set; }
    }

    /// <summary>
    /// Items in the Amazon catalog and search related metadata.
    /// </summary>
    [DataContract]
    public partial class ItemSearchResults
    {
        /// <summary>
        /// For &#x60;identifiers&#x60;-based searches, the total number of Amazon catalog items found. For &#x60;keywords&#x60;-based searches, the estimated total number of Amazon catalog items matched by the search query (only results up to the page count limit will be returned per request regardless of the number found).  Note: The maximum number of items (ASINs) that can be returned and paged through is 1000.
        /// </summary>
        /// <value>For &#x60;identifiers&#x60;-based searches, the total number of Amazon catalog items found. For &#x60;keywords&#x60;-based searches, the estimated total number of Amazon catalog items matched by the search query (only results up to the page count limit will be returned per request regardless of the number found).  Note: The maximum number of items (ASINs) that can be returned and paged through is 1000.</value>
        [DataMember(Name = "numberOfResults", EmitDefaultValue = false)]
        public int? NumberOfResults { get; set; }

        /// <summary>
        /// A list of items from the Amazon catalog.
        /// </summary>
        /// <value>A list of items from the Amazon catalog.</value>
        [DataMember(Name = "items", EmitDefaultValue = false)]
        public List<CatalogItem>? Items { get; set; }
    }

    /// <summary>
    /// When a request produces a response that exceeds the &#x60;pageSize&#x60;, pagination occurs.
    /// This means the response is divided into individual pages. To retrieve the next page or the previous page, you must pass the &#x60;nextToken&#x60; value or the &#x60;previousToken&#x60; value as the &#x60;pageToken&#x60; parameter in the next request. 
    /// When you receive the last page, there will be no &#x60;nextToken&#x60; key in the pagination object.
    /// </summary>
    [DataContract]
    public partial class Pagination
    {
        /// <summary>
        /// A token that can be used to fetch the next page.
        /// </summary>
        /// <value>A token that can be used to fetch the next page.</value>
        [DataMember(Name = "nextToken", EmitDefaultValue = false)]
        public string? NextToken { get; set; }

        /// <summary>
        /// A token that can be used to fetch the previous page.
        /// </summary>
        /// <value>A token that can be used to fetch the previous page.</value>
        [DataMember(Name = "previousToken", EmitDefaultValue = false)]
        public string? PreviousToken { get; set; }
    }
}