using System.Runtime.Serialization;
using static Enhanced.Models.AmazonData.AmazonEnum;

namespace Enhanced.Models.AmazonData
{
    /// <summary>
    /// Order information.
    /// </summary>
    [DataContract]
    public partial class Order
    {
        /// <summary>
        /// Whether the order was fulfilled by Amazon (AFN) or by the seller (MFN).
        /// </summary>
        /// <value>Whether the order was fulfilled by Amazon (AFN) or by the seller (MFN).</value>
        [DataMember(Name = "FulfillmentChannel", EmitDefaultValue = false)]
        public FulfillmentChannel? FulfillmentChannel { get; set; }

        /// <summary>
        /// The current order status.
        /// </summary>
        /// <value>The current order status.</value>
        [DataMember(Name = "OrderStatus", EmitDefaultValue = false)]
        public OrderStatus? OrderStatus { get; set; }

        /// <summary>
        /// The payment method for the order. This property is limited to Cash On Delivery (COD) and Convenience Store (CVS) payment methods. Unless you need the specific COD payment information provided by the PaymentExecutionDetailItem object, we recommend using the PaymentMethodDetails property to get payment method information.
        /// </summary>
        /// <value>The payment method for the order. This property is limited to Cash On Delivery (COD) and Convenience Store (CVS) payment methods. Unless you need the specific COD payment information provided by the PaymentExecutionDetailItem object, we recommend using the PaymentMethodDetails property to get payment method information.</value>
        [DataMember(Name = "PaymentMethod", EmitDefaultValue = false)]
        public PaymentMethod? PaymentMethod { get; set; }

        /// <summary>
        /// The type of the order.
        /// </summary>
        /// <value>The type of the order.</value>
        [DataMember(Name = "OrderType", EmitDefaultValue = false)]
        public OrderType? OrderType { get; set; }        

        /// <summary>
        /// An Amazon-defined order identifier, in 3-7-7 format.
        /// </summary>
        /// <value>An Amazon-defined order identifier, in 3-7-7 format.</value>
        [DataMember(Name = "AmazonOrderId", EmitDefaultValue = false)]
        public string? AmazonOrderId { get; set; }

        /// <summary>
        /// A seller-defined order identifier.
        /// </summary>
        /// <value>A seller-defined order identifier.</value>
        [DataMember(Name = "SellerOrderId", EmitDefaultValue = false)]
        public string? SellerOrderId { get; set; }

        /// <summary>
        /// The date when the order was created.
        /// </summary>
        /// <value>The date when the order was created.</value>
        [DataMember(Name = "PurchaseDate", EmitDefaultValue = false)]
        public string? PurchaseDate { get; set; }

        /// <summary>
        /// The date when the order was last updated.  Note: LastUpdateDate is returned with an incorrect date for orders that were last updated before 2009-04-01.
        /// </summary>
        /// <value>The date when the order was last updated.  Note: LastUpdateDate is returned with an incorrect date for orders that were last updated before 2009-04-01.</value>
        [DataMember(Name = "LastUpdateDate", EmitDefaultValue = false)]
        public string? LastUpdateDate { get; set; }

        /// <summary>
        /// The sales channel of the first item in the order.
        /// </summary>
        /// <value>The sales channel of the first item in the order.</value>
        [DataMember(Name = "SalesChannel", EmitDefaultValue = false)]
        public string? SalesChannel { get; set; }

        /// <summary>
        /// The order channel of the first item in the order.
        /// </summary>
        /// <value>The order channel of the first item in the order.</value>
        [DataMember(Name = "OrderChannel", EmitDefaultValue = false)]
        public string? OrderChannel { get; set; }

        /// <summary>
        /// The shipment service level of the order.
        /// </summary>
        /// <value>The shipment service level of the order.</value>
        [DataMember(Name = "ShipServiceLevel", EmitDefaultValue = false)]
        public string? ShipServiceLevel { get; set; }

        /// <summary>
        /// The total charge for this order.
        /// </summary>
        /// <value>The total charge for this order.</value>
        [DataMember(Name = "OrderTotal", EmitDefaultValue = false)]
        public Money? OrderTotal { get; set; }

        /// <summary>
        /// The number of items shipped.
        /// </summary>
        /// <value>The number of items shipped.</value>
        [DataMember(Name = "NumberOfItemsShipped", EmitDefaultValue = false)]
        public int? NumberOfItemsShipped { get; set; }

        /// <summary>
        /// The number of items unshipped.
        /// </summary>
        /// <value>The number of items unshipped.</value>
        [DataMember(Name = "NumberOfItemsUnshipped", EmitDefaultValue = false)]
        public int? NumberOfItemsUnshipped { get; set; }

        /// <summary>
        /// Information about sub-payment methods for a Cash On Delivery (COD) order.  Note: For a COD order that is paid for using one sub-payment method, one PaymentExecutionDetailItem object is returned, with PaymentExecutionDetailItem/PaymentMethod &#x3D; COD. For a COD order that is paid for using multiple sub-payment methods, two or more PaymentExecutionDetailItem objects are returned.
        /// </summary>
        /// <value>Information about sub-payment methods for a Cash On Delivery (COD) order.  Note: For a COD order that is paid for using one sub-payment method, one PaymentExecutionDetailItem object is returned, with PaymentExecutionDetailItem/PaymentMethod &#x3D; COD. For a COD order that is paid for using multiple sub-payment methods, two or more PaymentExecutionDetailItem objects are returned.</value>
        [DataMember(Name = "PaymentExecutionDetail", EmitDefaultValue = false)]
        public PaymentExecutionDetailItem? PaymentExecutionDetail { get; set; }

        /// <summary>
        /// A list of payment methods for the order.
        /// </summary>
        /// <value>A list of payment methods for the order.</value>
        [DataMember(Name = "PaymentMethodDetails", EmitDefaultValue = false)]
        public List<string>? PaymentMethodDetails { get; set; }

        /// <summary>
        /// The identifier for the marketplace where the order was placed.
        /// </summary>
        /// <value>The identifier for the marketplace where the order was placed.</value>
        [DataMember(Name = "MarketplaceId", EmitDefaultValue = false)]
        public string? MarketplaceId { get; set; }

        /// <summary>
        /// The shipment service level category of the order.  Possible values: Expedited, FreeEconomy, NextDay, SameDay, SecondDay, Scheduled, Standard.
        /// </summary>
        /// <value>The shipment service level category of the order.  Possible values: Expedited, FreeEconomy, NextDay, SameDay, SecondDay, Scheduled, Standard.</value>
        [DataMember(Name = "ShipmentServiceLevelCategory", EmitDefaultValue = false)]
        public string? ShipmentServiceLevelCategory { get; set; }

        /// <summary>
        /// The status of the Amazon Easy Ship order. This property is included only for Amazon Easy Ship orders.  Possible values: PendingPickUp, LabelCanceled, PickedUp, OutForDelivery, Damaged, Delivered, RejectedByBuyer, Undeliverable, ReturnedToSeller, ReturningToSeller.
        /// </summary>
        /// <value>The status of the Amazon Easy Ship order. This property is included only for Amazon Easy Ship orders.  Possible values: PendingPickUp, LabelCanceled, PickedUp, OutForDelivery, Damaged, Delivered, RejectedByBuyer, Undeliverable, ReturnedToSeller, ReturningToSeller.</value>
        [DataMember(Name = "EasyShipShipmentStatus", EmitDefaultValue = false)]
        public string? EasyShipShipmentStatus { get; set; }

        /// <summary>
        /// Custom ship label for Checkout by Amazon (CBA).
        /// </summary>
        /// <value>Custom ship label for Checkout by Amazon (CBA).</value>
        [DataMember(Name = "CbaDisplayableShippingLabel", EmitDefaultValue = false)]
        public string? CbaDisplayableShippingLabel { get; set; }


        /// <summary>
        /// The start of the time period within which you have committed to ship the order. In ISO 8601 date time format. Returned only for seller-fulfilled orders.  Note: EarliestShipDate might not be returned for orders placed before February 1, 2013.
        /// </summary>
        /// <value>The start of the time period within which you have committed to ship the order. In ISO 8601 date time format. Returned only for seller-fulfilled orders.  Note: EarliestShipDate might not be returned for orders placed before February 1, 2013.</value>
        [DataMember(Name = "EarliestShipDate", EmitDefaultValue = false)]
        public string? EarliestShipDate { get; set; }

        /// <summary>
        /// The end of the time period within which you have committed to ship the order. In ISO 8601 date time format. Returned only for seller-fulfilled orders.  Note: LatestShipDate might not be returned for orders placed before February 1, 2013.
        /// </summary>
        /// <value>The end of the time period within which you have committed to ship the order. In ISO 8601 date time format. Returned only for seller-fulfilled orders.  Note: LatestShipDate might not be returned for orders placed before February 1, 2013.</value>
        [DataMember(Name = "LatestShipDate", EmitDefaultValue = false)]
        public string? LatestShipDate { get; set; }

        /// <summary>
        /// The start of the time period within which you have committed to fulfill the order. In ISO 8601 date time format. Returned only for seller-fulfilled orders.
        /// </summary>
        /// <value>The start of the time period within which you have committed to fulfill the order. In ISO 8601 date time format. Returned only for seller-fulfilled orders.</value>
        [DataMember(Name = "EarliestDeliveryDate", EmitDefaultValue = false)]
        public string? EarliestDeliveryDate { get; set; }

        /// <summary>
        /// The end of the time period within which you have committed to fulfill the order. In ISO 8601 date time format. Returned only for seller-fulfilled orders that do not have a PendingAvailability, Pending, or Canceled status.
        /// </summary>
        /// <value>The end of the time period within which you have committed to fulfill the order. In ISO 8601 date time format. Returned only for seller-fulfilled orders that do not have a PendingAvailability, Pending, or Canceled status.</value>
        [DataMember(Name = "LatestDeliveryDate", EmitDefaultValue = false)]
        public string? LatestDeliveryDate { get; set; }

        /// <summary>
        /// When true, the order is an Amazon Business order. An Amazon Business order is an order where the buyer is a Verified Business Buyer.
        /// </summary>
        /// <value>When true, the order is an Amazon Business order. An Amazon Business order is an order where the buyer is a Verified Business Buyer.</value>
        [DataMember(Name = "IsBusinessOrder", EmitDefaultValue = false)]
        public bool? IsBusinessOrder { get; set; }

        /// <summary>
        /// When true, the order is a seller-fulfilled Amazon Prime order.
        /// </summary>
        /// <value>When true, the order is a seller-fulfilled Amazon Prime order.</value>
        [DataMember(Name = "IsPrime", EmitDefaultValue = false)]
        public bool? IsPrime { get; set; }

        /// <summary>
        /// When true, the order has a Premium Shipping Service Level Agreement. For more information about Premium Shipping orders, see \&quot;Premium Shipping Options\&quot; in the Seller Central Help for your marketplace.
        /// </summary>
        /// <value>When true, the order has a Premium Shipping Service Level Agreement. For more information about Premium Shipping orders, see \&quot;Premium Shipping Options\&quot; in the Seller Central Help for your marketplace.</value>
        [DataMember(Name = "IsPremiumOrder", EmitDefaultValue = false)]
        public bool? IsPremiumOrder { get; set; }

        /// <summary>
        /// When true, the order is a GlobalExpress order.
        /// </summary>
        /// <value>When true, the order is a GlobalExpress order.</value>
        [DataMember(Name = "IsGlobalExpressEnabled", EmitDefaultValue = false)]
        public bool? IsGlobalExpressEnabled { get; set; }

        /// <summary>
        /// The order ID value for the order that is being replaced. Returned only if IsReplacementOrder &#x3D; true.
        /// </summary>
        /// <value>The order ID value for the order that is being replaced. Returned only if IsReplacementOrder &#x3D; true.</value>
        [DataMember(Name = "ReplacedOrderId", EmitDefaultValue = false)]
        public string? ReplacedOrderId { get; set; }

        /// <summary>
        /// When true, this is a replacement order.
        /// </summary>
        /// <value>When true, this is a replacement order.</value>
        [DataMember(Name = "IsReplacementOrder", EmitDefaultValue = false)]
        public bool? IsReplacementOrder { get; set; }

        /// <summary>
        /// Indicates the date by which the seller must respond to the buyer with an estimated ship date. Returned only for Sourcing on Demand orders.
        /// </summary>
        /// <value>Indicates the date by which the seller must respond to the buyer with an estimated ship date. Returned only for Sourcing on Demand orders.</value>
        [DataMember(Name = "PromiseResponseDueDate", EmitDefaultValue = false)]
        public string? PromiseResponseDueDate { get; set; }

        /// <summary>
        /// When true, the estimated ship date is set for the order. Returned only for Sourcing on Demand orders.
        /// </summary>
        /// <value>When true, the estimated ship date is set for the order. Returned only for Sourcing on Demand orders.</value>
        [DataMember(Name = "IsEstimatedShipDateSet", EmitDefaultValue = false)]
        public bool? IsEstimatedShipDateSet { get; set; }

        /// <summary>
        /// When true, the item within this order was bought and re-sold by Amazon Business EU SARL (ABEU). By buying and instantly re-selling your items, ABEU becomes the seller of record, making your inventory available for sale to customers who would not otherwise purchase from a third-party seller.
        /// </summary>
        /// <value>When true, the item within this order was bought and re-sold by Amazon Business EU SARL (ABEU). By buying and instantly re-selling your items, ABEU becomes the seller of record, making your inventory available for sale to customers who would not otherwise purchase from a third-party seller.</value>
        [DataMember(Name = "IsSoldByAB", EmitDefaultValue = false)]
        public bool? IsSoldByAB { get; set; }

        /// <summary>
        /// The recommended location for the seller to ship the items from. It is calculated at checkout. The seller may or may not choose to ship from this location.
        /// </summary>
        /// <value>The recommended location for the seller to ship the items from. It is calculated at checkout. The seller may or may not choose to ship from this location.</value>
        [DataMember(Name = "DefaultShipFromLocationAddress", EmitDefaultValue = false)]
        public Address? DefaultShipFromLocationAddress { get; set; }

        /// <summary>
        /// Contains the instructions about the fulfillment like where should it be fulfilled from.
        /// </summary>
        /// <value>Contains the instructions about the fulfillment like where should it be fulfilled from.</value>
        [DataMember(Name = "FulfillmentInstruction", EmitDefaultValue = false)]
        public FulfillmentInstruction? FulfillmentInstruction { get; set; }

        /// <summary>
        /// When true, this order is marked to be picked up from a store rather than delivered.
        /// </summary>
        /// <value>When true, this order is marked to be picked up from a store rather than delivered.</value>
        [DataMember(Name = "IsISPU", EmitDefaultValue = false)]
        public bool? IsISPU { get; set; }

        /// <summary>
        /// Tax information about the marketplace.
        /// </summary>
        /// <value>Tax information about the marketplace.</value>
        [DataMember(Name = "MarketplaceTaxInfo", EmitDefaultValue = false)]
        public MarketplaceTaxInfo? MarketplaceTaxInfo { get; set; } // TO DO

        /// <summary>
        /// The seller’s friendly name registered in the marketplace.
        /// </summary>
        /// <value>The seller’s friendly name registered in the marketplace.</value>
        [DataMember(Name = "SellerDisplayName", EmitDefaultValue = false)]
        public string? SellerDisplayName { get; set; }

        /// <summary>
        /// Gets or Sets ShippingAddress
        /// </summary>
        [DataMember(Name = "ShippingAddress", EmitDefaultValue = false)]
        public Address? ShippingAddress { get; set; }

        /// <summary>
        /// Gets or Sets BuyerInfo
        /// </summary>
        [DataMember(Name = "BuyerInfo", EmitDefaultValue = false)]
        public BuyerInfo? BuyerInfo { get; set; }

        [DataMember(Name = "OrderItemList", EmitDefaultValue = false)]
        public List<OrderItem>? OrderItemList { get; set; }
    }
}
