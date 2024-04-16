using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Enhanced.Models.AmazonData
{
    public class AmazonEnum
    {
        public enum RateLimitType
        {
            UNSET,

            Order_GetOrders,
            Order_GetOrderItems,
            Order_ConfirmShipment,

            CatalogItems20220401_SearchCatalogItems,

            Feed_CreateFeed,
            Feed_CreateFeedDocument,

            Report_GetReports,
            Report_GetReportDocument,
        }

        public enum OrderStatus
        {
            /// <summary>
            /// This status is available for pre-orders only. The order has been placed, payment has not been authorized, and the release date of the item is in the future.
            /// </summary>
            PendingAvailability = 0,
            /// <summary>
            /// The order has been placed but payment has not been authorized
            /// </summary>
            Pending = 1,
            /// <summary>
            /// Payment has been authorized and the order is ready for shipment, but no items in the order have been shipped
            /// </summary>
            Unshipped = 2,
            /// <summary>
            /// One or more, but not all, items in the order have been shipped
            /// </summary>
            PartiallyShipped = 3,
            /// <summary>
            /// All items in the order have been shipped
            /// </summary>
            Shipped = 4,
            /// <summary>
            /// All items in the order have been shipped. The seller has not yet given confirmation to Amazon that the invoice has been shipped to the buyer
            /// </summary>
            InvoiceUnconfirmed = 5,
            /// <summary>
            /// The order has been canceled
            /// </summary>
            Canceled = 6,
            /// <summary>
            /// The order cannot be fulfilled. This state applies only to Multi-Channel Fulfillment orders
            /// </summary>
            Unfulfillable = 7
        }

        //[JsonConverter(typeof(StringEnumConverter))]
        /// <summary>
        /// A list that indicates how an order was fulfilled
        /// </summary>
        public enum FulfillmentChannel
        {
            /// <summary>
            /// Fulfillment by Amazon
            /// </summary>
            [EnumMember(Value = "AFN")]
            AFN = 0,
            /// <summary>
            /// Fulfilled by the seller
            /// </summary>
            [EnumMember(Value = "MFN")]
            MFN = 1
        }

        //[JsonConverter(typeof(StringEnumConverter))]
        /// <summary>
        /// A list of payment method
        /// </summary>
        public enum PaymentMethod
        {
            /// <summary>
            /// Cash on delivery
            /// </summary>
            [EnumMember(Value = "COD")]
            COD = 0,

            /// <summary>
            /// Convenience store payment
            /// </summary>
            [EnumMember(Value = "CVS")]
            CVS = 1,

            /// <summary>
            /// Any payment method other than COD or CVS
            /// </summary>
            [EnumMember(Value = "Other")]
            Other = 2
        }

        //[JsonConverter(typeof(StringEnumConverter))]
        /// <summary>
        /// A list of EasyShipShipmentStatus , Used to select Easy Ship orders with statuses that match the specified values
        /// </summary>
        public enum EasyShipShipmentStatus
        {
            PendingPickUp = 0,
            LabelCanceled = 1,
            PickedUp = 2,
            OutForDelivery = 3,
            Damaged = 4,
            Delivered = 5,
            RejectedByBuyer = 6,
            Undeliverable = 7,
            ReturnedToSeller = 8,
            ReturningToSeller = 9
        }

        /// <summary>
        /// The type of the order.
        /// </summary>
        /// <value>The type of the order.</value>
        //[JsonConverter(typeof(StringEnumConverter))]
        public enum OrderType
        {
            /// <summary>
            /// Enum StandardOrder for value: StandardOrder
            /// </summary>
            [EnumMember(Value = "StandardOrder")]
            StandardOrder = 1,

            /// <summary>
            /// Enum LongLeadTimeOrder for value: LongLeadTimeOrder
            /// </summary>
            [EnumMember(Value = "LongLeadTimeOrder")]
            LongLeadTimeOrder = 2,

            /// <summary>
            /// Enum Preorder for value: Preorder
            /// </summary>
            [EnumMember(Value = "Preorder")]
            Preorder = 3,

            /// <summary>
            /// Enum BackOrder for value: BackOrder
            /// </summary>
            [EnumMember(Value = "BackOrder")]
            BackOrder = 4,

            /// <summary>
            /// Enum SourcingOnDemandOrder for value: SourcingOnDemandOrder
            /// </summary>
            [EnumMember(Value = "SourcingOnDemandOrder")]
            SourcingOnDemandOrder = 5
        }

        /// <summary>
        /// The address type of the shipping address.
        /// </summary>
        /// <value>The address type of the shipping address.</value>
        //[JsonConverter(typeof(StringEnumConverter))]
        public enum AddressType
        {
            /// <summary>
            /// Enum Residential for value: Residential
            /// </summary>
            [EnumMember(Value = "Residential")]
            Residential = 0,

            /// <summary>
            /// Enum Commercial for value: Commercial
            /// </summary>
            [EnumMember(Value = "Commercial")]
            Commercial = 1
        }

        /// <summary>
        /// The category of deemed reseller. This applies to selling partners that are not based in the EU and is used to help them meet the VAT Deemed Reseller tax laws in the EU and UK.
        /// </summary>
        /// <value>The category of deemed reseller. This applies to selling partners that are not based in the EU and is used to help them meet the VAT Deemed Reseller tax laws in the EU and UK.</value>
        //[JsonConverter(typeof(StringEnumConverter))]
        public enum DeemedResellerCategory
        {
            /// <summary>
            /// Enum IOSS for value: IOSS
            /// </summary>
            [EnumMember(Value = "IOSS")]
            IOSS = 0,

            /// <summary>
            /// Enum UOSS for value: UOSS
            /// </summary>
            [EnumMember(Value = "UOSS")]
            UOSS = 1,

            /// <summary>
            /// Enum NO_VOEC for value: NO_VOEC
            /// </summary>
            [EnumMember(Value = "NO_VOEC")]
            NO_VOEC = 2,

            /// <summary>
            /// Enum GB_VOEC for value: GB_VOEC
            /// </summary>
            [EnumMember(Value = "GB_VOEC")]
            GB_VOEC = 3,

            /// <summary>
            /// Enum CA_MPF for value: CA_MPF
            /// </summary>
            [EnumMember(Value = "CA_MPF")]
            CA_MPF = 4,

            /// <summary>
            /// Enum AU_VOEC for value: AU_VOEC
            /// </summary>
            [EnumMember(Value = "AU_VOEC")]
            AU_VOEC = 5,

            /// <summary>
            /// Enum SG_VOEC for value: SG_VOEC
            /// </summary>
            [EnumMember(Value = "SG_VOEC")]
            SG_VOEC = 6
        }

        /// <summary>
        /// The tax collection model applied to the item.
        /// </summary>
        /// <value>The tax collection model applied to the item.</value>
        //[JsonConverter(typeof(StringEnumConverter))]
        public enum ModelEnum
        {
            /// <summary>
            /// Enum MarketplaceFacilitator for value: MarketplaceFacilitator
            /// </summary>
            [EnumMember(Value = "MarketplaceFacilitator")]
            MarketplaceFacilitator = 0,

            /// <summary>
            /// Enum LowValueGoods for value: LowValueGoods
            /// </summary>
            [EnumMember(Value = "LowValueGoods")]
            LowValueGoods = 1,
        }

        /// <summary>
        /// The party responsible for withholding the taxes and remitting them to the taxing authority.
        /// </summary>
        /// <value>The party responsible for withholding the taxes and remitting them to the taxing authority.</value>
        //[JsonConverter(typeof(StringEnumConverter))]
        public enum ResponsiblePartyEnum
        {
            /// <summary>
            /// Enum AmazonServicesInc for value: Amazon Services, Inc.
            /// </summary>
            [EnumMember(Value = "Amazon Services, Inc.")]
            AmazonServicesInc = 0,

            /// <summary>
            /// Enum AmazonCommercialServicesPtyLtd for value: Amazon Commercial Services Pty Ltd
            /// </summary>
            [EnumMember(Value = "Amazon Commercial Services Pty Ltd")]
            AmazonCommercialServicesPtyLtd = 1
        }

        public enum TokenDataType
        {
            Normal = 0,
            PII = 1,
            Grantless = 2,
        }

        public enum IncludedData
        {
            attributes,
            dimensions,
            identifiers,
            images,
            productTypes,
            relationships,
            salesRanks,
            summaries,
            vendorDetails
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public enum FeedMessageType
        {
            Inventory,
            Price,
            Product,
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public enum OperationType
        {
            Update,
            Delete,
            PartialUpdate,
        }

        public enum InventoryLookup
        {
            FulfillmentNetwork,
        }

        /// <summary>
        /// List of all FeedType https://github.com/amzn/selling-partner-api-docs/blob/main/references/feeds-api/feedtype-values.md
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enum FeedType
        {
            POST_FLAT_FILE_LISTINGS_DATA,
            POST_FLAT_FILE_INVLOADER_DATA,
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public enum ContentType
        {
            [EnumMember(Value = "text/xml; charset=UTF-8")]
            XML,

            [EnumMember(Value = "application/json; charset=UTF-8")]
            JSON,

            [EnumMember(Value = "application/pdf; charset=UTF-8")]
            PDF,

            [EnumMember(Value = "text/tab-separated-values; charset=UTF-8")]
            TXT,
        }

        /// <summary>
        /// The encryption standard required to encrypt or decrypt the document contents.
        /// </summary>
        /// <value>The encryption standard required to encrypt or decrypt the document contents.</value>
        [JsonConverter(typeof(StringEnumConverter))]
        public enum Standard
        {

            /// <summary>
            /// Enum AES for value: AES
            /// </summary>
            [EnumMember(Value = "AES")]
            AES = 0
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public enum ReportTypes
        {
            GET_V2_SETTLEMENT_REPORT_DATA_FLAT_FILE_V2,
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public enum ProcessingStatuses
        {
            /// <summary>
            /// The report was cancelled. There are two ways a report can be cancelled: an explicit cancellation request before the report starts processing, or an automatic cancellation if there is no data to return.
            /// </summary>
            CANCELLED,
            /// <summary>
            /// The report has completed processing.
            /// </summary>
            DONE,
            /// <summary>
            /// The report was aborted due to a fatal error.
            /// </summary>
            FATAL,
            /// <summary>
            /// The report is being processed.
            /// </summary>
            IN_PROGRESS,
            /// <summary>
            /// The report has not yet started processing. It may be waiting for another IN_PROGRESS report.
            /// </summary>
            IN_QUEUE
        }

        /// <summary>
        /// If present, the report document contents have been compressed with the provided algorithm.
        /// </summary>
        /// <value>If present, the report document contents have been compressed with the provided algorithm.</value>
        [JsonConverter(typeof(StringEnumConverter))]
        public enum CompressionAlgorithm
        {
            /// <summary>
            /// Enum GZIP for value: GZIP
            /// </summary>
            [EnumMember(Value = "GZIP")]
            GZIP = 0
        }

        /// <summary>
        /// Replenishment category associated with an Amazon catalog item.
        /// </summary>
        /// <value>Replenishment category associated with an Amazon catalog item.</value>
        [JsonConverter(typeof(StringEnumConverter))]
        public enum ReplenishmentCategory
        {
            /// <summary>
            /// Enum ALLOCATED for value: ALLOCATED
            /// </summary>
            [EnumMember(Value = "ALLOCATED")]
            ALLOCATED = 0,

            /// <summary>
            /// Enum BASICREPLENISHMENT for value: BASIC_REPLENISHMENT
            /// </summary>
            [EnumMember(Value = "BASIC_REPLENISHMENT")]
            BASICREPLENISHMENT = 1,

            /// <summary>
            /// Enum INSEASON for value: IN_SEASON
            /// </summary>
            [EnumMember(Value = "IN_SEASON")]
            INSEASON = 2,

            /// <summary>
            /// Enum LIMITEDREPLENISHMENT for value: LIMITED_REPLENISHMENT
            /// </summary>
            [EnumMember(Value = "LIMITED_REPLENISHMENT")]
            LIMITEDREPLENISHMENT = 3,

            /// <summary>
            /// Enum MANUFACTUREROUTOFSTOCK for value: MANUFACTURER_OUT_OF_STOCK
            /// </summary>
            [EnumMember(Value = "MANUFACTURER_OUT_OF_STOCK")]
            MANUFACTUREROUTOFSTOCK = 4,

            /// <summary>
            /// Enum NEWPRODUCT for value: NEW_PRODUCT
            /// </summary>
            [EnumMember(Value = "NEW_PRODUCT")]
            NEWPRODUCT = 5,

            /// <summary>
            /// Enum NONREPLENISHABLE for value: NON_REPLENISHABLE
            /// </summary>
            [EnumMember(Value = "NON_REPLENISHABLE")]
            NONREPLENISHABLE = 6,

            /// <summary>
            /// Enum NONSTOCKUPABLE for value: NON_STOCKUPABLE
            /// </summary>
            [EnumMember(Value = "NON_STOCKUPABLE")]
            NONSTOCKUPABLE = 7,

            /// <summary>
            /// Enum OBSOLETE for value: OBSOLETE
            /// </summary>
            [EnumMember(Value = "OBSOLETE")]
            OBSOLETE = 8,

            /// <summary>
            /// Enum PLANNEDREPLENISHMENT for value: PLANNED_REPLENISHMENT
            /// </summary>
            [EnumMember(Value = "PLANNED_REPLENISHMENT")]
            PLANNEDREPLENISHMENT = 9
        }

        public enum UpdateShipmentStatus
        {
            ReadyForPickup = 0,
            PickedUp = 1,
            RefusedPickup = 2,
        }

        public enum WeightUnit
        {
            pounds = 0,
            kilograms = 1,
        }

        public enum MeasuringUnit
        {
            inches = 0,
            centimeters = 1,
        }

        public enum AmountType
        {
            [EnumMember(Value = "FBA Inventory Reimbursement")]
            FBAInventoryReimbursement = 0,

            [EnumMember(Value = "ItemFees")]
            ItemFees = 1,

            [EnumMember(Value = "ItemPrice")]
            ItemPrice = 2,

            [EnumMember(Value = "ItemWithheldTax")]
            ItemWithheldTax = 3,

            [EnumMember(Value = "Manual Processing Fee Reimbursement")]
            ManualProcessingFeeReimbursement = 4,

            [EnumMember(Value = "Other transactions")]
            OtherTransactions = 5,

            [EnumMember(Value = "other-transaction")]
            OtherTransaction = 6,

            [EnumMember(Value = "Promotion")]
            Promotion = 7,
        }

        public enum FBAInventoryReimbursement
        {
            [EnumMember(Value = "COMPENSATED_CLAWBACK")]
            COMPENSATED_CLAWBACK,

            [EnumMember(Value = "MISSING_FROM_INBOUND")]
            MISSING_FROM_INBOUND,

            [EnumMember(Value = "REVERSAL_REIMBURSEMENT")]
            REVERSAL_REIMBURSEMENT,

            [EnumMember(Value = "WAREHOUSE_DAMAGE")]
            WAREHOUSE_DAMAGE
        }

        public enum ItemFees
        {
            [EnumMember(Value = "Commission")]
            Commission,

            [EnumMember(Value = "FBAPerUnitFulfillmentFee")]
            FBAPerUnitFulfillmentFee,

            [EnumMember(Value = "GiftwrapChargeback")]
            GiftwrapChargeback,

            [EnumMember(Value = "RefundCommission")]
            RefundCommission,

            [EnumMember(Value = "ShippingChargeback")]
            ShippingChargeback,

            [EnumMember(Value = "ShippingHB")]
            ShippingHB,

            [EnumMember(Value = "VariableClosingFee")]
            VariableClosingFee
        }

        public enum ItemPrice
        {
            [EnumMember(Value = "GiftWrap")]
            GiftWrap,

            [EnumMember(Value = "GiftWrapTax")]
            GiftWrapTax,

            [EnumMember(Value = "Goodwill")]
            Goodwill,

            [EnumMember(Value = "Principal")]
            Principal,

            [EnumMember(Value = "Shipping")]
            Shipping,

            [EnumMember(Value = "ShippingTax")]
            ShippingTax,

            [EnumMember(Value = "Tax")]
            Tax,
        }

        public enum ItemWithheldTax
        {
            [EnumMember(Value = "MarketplaceFacilitatorTax-Principal")]
            MarketplaceFacilitatorTaxPrincipal,

            [EnumMember(Value = "MarketplaceFacilitatorTax-Shipping")]
            MarketplaceFacilitatorTaxShipping,

            [EnumMember(Value = "MarketplaceFacilitatorVAT-Principal")]
            MarketplaceFacilitatorVATPrincipal,

            [EnumMember(Value = "MarketplaceFacilitatorVAT-Shipping")]
            MarketplaceFacilitatorVATShipping,
        }

        public enum ManualProcessingFeeReimbursement
        {
            [EnumMember(Value = "Manual Processing Fee Reimbursement")]
            ManualProcessingFeeReimbursement,
        }

        public enum OtherTransactions
        {
            [EnumMember(Value = "SAFE-T reimbursement")]
            SAFE_T_Reimbursement,
        }

        public enum OtherTransaction
        {
            [EnumMember(Value = "Commingling VAT")]
            ComminglingVAT,

            [EnumMember(Value = "Manual Processing Fee")]
            ManualProcessingFee,

            [EnumMember(Value = "RemovalComplete")]
            RemovalComplete,

            [EnumMember(Value = "Shipping label purchase")]
            ShippingLabelPurchase,

            [EnumMember(Value = "Shipping label purchase for return")]
            ShippingLabelPurchaseForReturn,

            [EnumMember(Value = "StorageRenewalBilling")]
            StorageRenewalBilling,

            [EnumMember(Value = "Subscription Fee")]
            SubscriptionFee,

            [EnumMember(Value = "WarehousePrep")]
            WarehousePrep,
        }

        public enum Promotion
        {
            [EnumMember(Value = "Principal")]
            Principal,

            [EnumMember(Value = "Shipping")]
            Shipping,

            [EnumMember(Value = "TaxDiscount")]
            TaxDiscount,
        }

        public enum BalancingAccountNumber
        {
            GL_79071,
            GL_79075,
            GL_51000,
            GL_40025,
        }

        public enum CodCollectionMethod
        {
            DirectPayment,
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public enum IdentifiersType
        {
            ASIN,
            EAN,
            GTIN,
            ISBN,
            JAN,
            MINSAN,
            SKU,
            UPC
        }
    }
}
