using System.Runtime.Serialization;
using static Enhanced.Models.AmazonData.AmazonEnum;

namespace Enhanced.Models.AmazonData
{
    /// <summary>
    /// A single order item.
    /// </summary>
    [DataContract]
    public partial class OrderItem
    {
        ///// <summary>
        ///// The category of deemed reseller. This applies to selling partners that are not based in the EU and is used to help them meet the VAT Deemed Reseller tax laws in the EU and UK.
        ///// </summary>
        ///// <value>The category of deemed reseller. This applies to selling partners that are not based in the EU and is used to help them meet the VAT Deemed Reseller tax laws in the EU and UK.</value>
        //[DataMember(Name = "DeemedResellerCategory", EmitDefaultValue = false)]
        //public DeemedResellerCategory? DeemedResellerCategory { get; set; }

        /// <summary>
        /// The Amazon Standard Identification Number (ASIN) of the item.
        /// </summary>
        /// <value>The Amazon Standard Identification Number (ASIN) of the item.</value>
        [DataMember(Name = "ASIN", EmitDefaultValue = false)]
        public string? ASIN { get; set; }

        /// <summary>
        /// The seller stock keeping unit (SKU) of the item.
        /// </summary>
        /// <value>The seller stock keeping unit (SKU) of the item.</value>
        [DataMember(Name = "SellerSKU", EmitDefaultValue = false)]
        public string? SellerSKU { get; set; }

        /// <summary>
        /// An Amazon-defined order item identifier.
        /// </summary>
        /// <value>An Amazon-defined order item identifier.</value>
        [DataMember(Name = "OrderItemId", EmitDefaultValue = false)]
        public string? OrderItemId { get; set; }

        /// <summary>
        /// The name of the item.
        /// </summary>
        /// <value>The name of the item.</value>
        [DataMember(Name = "Title", EmitDefaultValue = false)]
        public string? Title { get; set; }

        /// <summary>
        /// The number of items in the order. 
        /// </summary>
        /// <value>The number of items in the order. </value>
        [DataMember(Name = "QuantityOrdered", EmitDefaultValue = false)]
        public int? QuantityOrdered { get; set; }

        /// <summary>
        /// The number of items shipped.
        /// </summary>
        /// <value>The number of items shipped.</value>
        [DataMember(Name = "QuantityShipped", EmitDefaultValue = false)]
        public int? QuantityShipped { get; set; }

        /// <summary>
        /// Product information for the item.
        /// </summary>
        /// <value>Product information for the item.</value>
        [DataMember(Name = "ProductInfo", EmitDefaultValue = false)]
        public ProductInfoDetail? ProductInfo { get; set; }

        /// <summary>
        /// The number and value of Amazon Points granted with the purchase of an item.
        /// </summary>
        /// <value>The number and value of Amazon Points granted with the purchase of an item.</value>
        [DataMember(Name = "PointsGranted", EmitDefaultValue = false)]
        public PointsGrantedDetail? PointsGranted { get; set; }

        /// <summary>
        /// The selling price of the order item. Note that an order item is an item and a quantity. This means that the value of ItemPrice is equal to the selling price of the item multiplied by the quantity ordered. Note that ItemPrice excludes ShippingPrice and GiftWrapPrice.
        /// </summary>
        /// <value>The selling price of the order item. Note that an order item is an item and a quantity. This means that the value of ItemPrice is equal to the selling price of the item multiplied by the quantity ordered. Note that ItemPrice excludes ShippingPrice and GiftWrapPrice.</value>
        [DataMember(Name = "ItemPrice", EmitDefaultValue = false)]
        public Money? ItemPrice { get; set; }

        /// <summary>
        /// The shipping price of the item.
        /// </summary>
        /// <value>The shipping price of the item.</value>
        [DataMember(Name = "ShippingPrice", EmitDefaultValue = false)]
        public Money? ShippingPrice { get; set; }

        /// <summary>
        /// The tax on the item price.
        /// </summary>
        /// <value>The tax on the item price.</value>
        [DataMember(Name = "ItemTax", EmitDefaultValue = false)]
        public Money? ItemTax { get; set; }

        /// <summary>
        /// The tax on the shipping price.
        /// </summary>
        /// <value>The tax on the shipping price.</value>
        [DataMember(Name = "ShippingTax", EmitDefaultValue = false)]
        public Money? ShippingTax { get; set; }

        /// <summary>
        /// The discount on the shipping price.
        /// </summary>
        /// <value>The discount on the shipping price.</value>
        [DataMember(Name = "ShippingDiscount", EmitDefaultValue = false)]
        public Money? ShippingDiscount { get; set; }

        /// <summary>
        /// The tax on the discount on the shipping price.
        /// </summary>
        /// <value>The tax on the discount on the shipping price.</value>
        [DataMember(Name = "ShippingDiscountTax", EmitDefaultValue = false)]
        public Money? ShippingDiscountTax { get; set; }

        /// <summary>
        /// The total of all promotional discounts in the offer.
        /// </summary>
        /// <value>The total of all promotional discounts in the offer.</value>
        [DataMember(Name = "PromotionDiscount", EmitDefaultValue = false)]
        public Money? PromotionDiscount { get; set; }

        /// <summary>
        /// The tax on the total of all promotional discounts in the offer.
        /// </summary>
        /// <value>The tax on the total of all promotional discounts in the offer.</value>
        [DataMember(Name = "PromotionDiscountTax", EmitDefaultValue = false)]
        public Money? PromotionDiscountTax { get; set; }

        /// <summary>
        /// Gets or Sets PromotionIds
        /// </summary>
        [DataMember(Name = "PromotionIds", EmitDefaultValue = false)]
        public List<string>? PromotionIds { get; set; }

        /// <summary>
        /// The fee charged for COD service.
        /// </summary>
        /// <value>The fee charged for COD service.</value>
        [DataMember(Name = "CODFee", EmitDefaultValue = false)]
        public Money? CODFee { get; set; }

        /// <summary>
        /// The discount on the COD fee.
        /// </summary>
        /// <value>The discount on the COD fee.</value>
        [DataMember(Name = "CODFeeDiscount", EmitDefaultValue = false)]
        public Money? CODFeeDiscount { get; set; }

        /// <summary>
        /// When true, the item is a gift.
        /// </summary>
        /// <value>When true, the item is a gift.</value>
        [DataMember(Name = "IsGift", EmitDefaultValue = false)]
        public bool? IsGift { get; set; }

        /// <summary>
        /// The condition of the item as described by the seller.
        /// </summary>
        /// <value>The condition of the item as described by the seller.</value>
        [DataMember(Name = "ConditionNote", EmitDefaultValue = false)]
        public string? ConditionNote { get; set; }

        /// <summary>
        /// The condition of the item.  Possible values: New, Used, Collectible, Refurbished, Preorder, Club.
        /// </summary>
        /// <value>The condition of the item.  Possible values: New, Used, Collectible, Refurbished, Preorder, Club.</value>
        [DataMember(Name = "ConditionId", EmitDefaultValue = false)]
        public string? ConditionId { get; set; }

        /// <summary>
        /// The subcondition of the item.  Possible values: New, Mint, Very Good, Good, Acceptable, Poor, Club, OEM, Warranty, Refurbished Warranty, Refurbished, Open Box, Any, Other.
        /// </summary>
        /// <value>The subcondition of the item.  Possible values: New, Mint, Very Good, Good, Acceptable, Poor, Club, OEM, Warranty, Refurbished Warranty, Refurbished, Open Box, Any, Other.</value>
        [DataMember(Name = "ConditionSubtypeId", EmitDefaultValue = false)]
        public string? ConditionSubtypeId { get; set; }

        /// <summary>
        /// The start date of the scheduled delivery window in the time zone of the order destination. In ISO 8601 date time format.
        /// </summary>
        /// <value>The start date of the scheduled delivery window in the time zone of the order destination. In ISO 8601 date time format.</value>
        [DataMember(Name = "ScheduledDeliveryStartDate", EmitDefaultValue = false)]
        public string? ScheduledDeliveryStartDate { get; set; }

        /// <summary>
        /// The end date of the scheduled delivery window in the time zone of the order destination. In ISO 8601 date time format.
        /// </summary>
        /// <value>The end date of the scheduled delivery window in the time zone of the order destination. In ISO 8601 date time format.</value>
        [DataMember(Name = "ScheduledDeliveryEndDate", EmitDefaultValue = false)]
        public string? ScheduledDeliveryEndDate { get; set; }

        /// <summary>
        /// Indicates that the selling price is a special price that is available only for Amazon Business orders. For more information about the Amazon Business Seller Program, see the [Amazon Business website](https://www.amazon.com/b2b/info/amazon-business).   Possible values: BusinessPrice - A special price that is available only for Amazon Business orders.
        /// </summary>
        /// <value>Indicates that the selling price is a special price that is available only for Amazon Business orders. For more information about the Amazon Business Seller Program, see the [Amazon Business website](https://www.amazon.com/b2b/info/amazon-business).   Possible values: BusinessPrice - A special price that is available only for Amazon Business orders.</value>
        [DataMember(Name = "PriceDesignation", EmitDefaultValue = false)]
        public string? PriceDesignation { get; set; }

        /// <summary>
        /// Information about withheld taxes.
        /// </summary>
        /// <value>Information about withheld taxes.</value>
        [DataMember(Name = "TaxCollection", EmitDefaultValue = false)]
        public TaxCollection? TaxCollection { get; set; }

        /// <summary>
        /// When true, the product type for this item has a serial number.  Returned only for Amazon Easy Ship orders.
        /// </summary>
        /// <value>When true, the product type for this item has a serial number.  Returned only for Amazon Easy Ship orders.</value>
        [DataMember(Name = "SerialNumberRequired", EmitDefaultValue = false)]
        public bool? SerialNumberRequired { get; set; }

        /// <summary>
        /// When true, transparency codes are required.
        /// </summary>
        /// <value>When true, transparency codes are required.</value>
        [DataMember(Name = "IsTransparency", EmitDefaultValue = false)]
        public bool? IsTransparency { get; set; }

        /// <summary>
        /// The IOSS number of the seller. Sellers selling in the EU will be assigned a unique IOSS number that must be listed on all packages sent to the EU.
        /// </summary>
        /// <value>The IOSS number of the seller. Sellers selling in the EU will be assigned a unique IOSS number that must be listed on all packages sent to the EU.</value>
        [DataMember(Name = "IossNumber", EmitDefaultValue = false)]
        public string? IossNumber { get; set; }

        /// <summary>
        /// Gets or Sets BuyerInfo
        /// </summary>
        /// <value>Gets or Sets BuyerInfo</value>
        [DataMember(Name = "BuyerInfo", EmitDefaultValue = false)]
        public OrderItemBuyerInfo? BuyerInfo { get; set; }
    }

    /// <summary>
    /// Product information on the number of items.
    /// </summary>
    [DataContract]
    public partial class ProductInfoDetail
    {
        /// <summary>
        /// The total number of items that are included in the ASIN.
        /// </summary>
        /// <value>The total number of items that are included in the ASIN.</value>
        [DataMember(Name = "NumberOfItems", EmitDefaultValue = false)]
        public int? NumberOfItems { get; set; }
    }

    /// <summary>
    /// Information about withheld taxes.
    /// </summary>
    [DataContract]
    public partial class TaxCollection
    {
        /// <summary>
        /// The tax collection model applied to the item.
        /// </summary>
        /// <value>The tax collection model applied to the item.</value>
        [DataMember(Name = "Model", EmitDefaultValue = false)]
        public ModelEnum? Model { get; set; }

        /// <summary>
        /// The party responsible for withholding the taxes and remitting them to the taxing authority.
        /// </summary>
        /// <value>The party responsible for withholding the taxes and remitting them to the taxing authority.</value>
        [DataMember(Name = "ResponsibleParty", EmitDefaultValue = false)]
        public ResponsiblePartyEnum? ResponsibleParty { get; set; }
    }

    /// <summary>
    /// The number of Amazon Points offered with the purchase of an item, and their monetary value.
    /// </summary>
    [DataContract]
    public partial class PointsGrantedDetail
    {
        /// <summary>
        /// The number of Amazon Points granted with the purchase of an item.
        /// </summary>
        /// <value>The number of Amazon Points granted with the purchase of an item.</value>
        [DataMember(Name = "PointsNumber", EmitDefaultValue = false)]
        public int? PointsNumber { get; set; }

        /// <summary>
        /// The monetary value of the Amazon Points granted.
        /// </summary>
        /// <value>The monetary value of the Amazon Points granted.</value>
        [DataMember(Name = "PointsMonetaryValue", EmitDefaultValue = false)]
        public Money? PointsMonetaryValue { get; set; }
    }

    /// <summary>
    /// A single order item&#39;s buyer information.
    /// </summary>
    [DataContract]
    public partial class OrderItemBuyerInfo
    {
        /// <summary>
        /// An Amazon-defined order item identifier.
        /// </summary>
        /// <value>An Amazon-defined order item identifier.</value>
        [DataMember(Name = "OrderItemId", EmitDefaultValue = false)]
        public string? OrderItemId { get; set; }

        /// <summary>
        /// Buyer information for custom orders from the Amazon Custom program.
        /// </summary>
        /// <value>Buyer information for custom orders from the Amazon Custom program.</value>
        [DataMember(Name = "BuyerCustomizedInfo", EmitDefaultValue = false)]
        public BuyerCustomizedInfoDetail? BuyerCustomizedInfo { get; set; }

        /// <summary>
        /// The gift wrap price of the item.
        /// </summary>
        /// <value>The gift wrap price of the item.</value>
        [DataMember(Name = "GiftWrapPrice", EmitDefaultValue = false)]
        public Money? GiftWrapPrice { get; set; }

        /// <summary>
        /// The tax on the gift wrap price.
        /// </summary>
        /// <value>The tax on the gift wrap price.</value>
        [DataMember(Name = "GiftWrapTax", EmitDefaultValue = false)]
        public Money? GiftWrapTax { get; set; }

        /// <summary>
        /// A gift message provided by the buyer.
        /// </summary>
        /// <value>A gift message provided by the buyer.</value>
        [DataMember(Name = "GiftMessageText", EmitDefaultValue = false)]
        public string? GiftMessageText { get; set; }

        /// <summary>
        /// The gift wrap level specified by the buyer.
        /// </summary>
        /// <value>The gift wrap level specified by the buyer.</value>
        [DataMember(Name = "GiftWrapLevel", EmitDefaultValue = false)]
        public string? GiftWrapLevel { get; set; }
    }

    /// <summary>
    /// Buyer information for custom orders from the Amazon Custom program.
    /// </summary>
    [DataContract]
    public partial class BuyerCustomizedInfoDetail
    {
        /// <summary>
        /// The location of a zip file containing Amazon Custom data.
        /// </summary>
        /// <value>The location of a zip file containing Amazon Custom data.</value>
        [DataMember(Name = "CustomizedURL", EmitDefaultValue = false)]
        public string? CustomizedURL { get; set; }
    }
}
