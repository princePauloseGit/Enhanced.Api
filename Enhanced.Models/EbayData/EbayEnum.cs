namespace Enhanced.Models.EbayData
{
    public class EbayEnum
    {
        public enum EbayResponseCodes
        {
            Invalid_Request = 20400,
            Missing_Field = 20401,
            Invalid_Input = 20402,
            System_Error = 20500,
            Service_Unavailable = 20501,
            Offer_Exits = 25002,
            AlreadyExists = 208,
            Success = 200
        }

        public enum ErrorCategories
        {
            APPLICATION = 0,
            BUSINESS = 1,
            REQUEST = 2,
        }

        public enum FulfillmentStatuses
        {
            FULFILLED = 0,
            IN_PROGRESS = 1,
            NOT_STARTED = 2,
        }

        public enum PaymenentStatuses
        {
            FAILED = 0,
            FULLY_REFUNDED = 1,
            PAID = 2,
            PARTIALLY_REFUNDED = 3,
            PENDING = 4
        }

        public enum CancelRequestStates
        {
            COMPLETED = 0,
            REJECTED = 1,
            REQUESTED = 2,
        }

        public enum CancelStates
        {
            CANCELED = 0,
            IN_PROGRESS = 1,
            NONE_REQUESTED = 2,
        }

        public enum FulfillmentInstructionsTypes
        {
            DIGITAL = 0,
            PREPARE_FOR_PICKUP = 1,
            SELLER_DEFINED = 2,
            SHIP_TO = 3,
        }

        public enum TaxTypes
        {
            GST = 0,
            PROVINCE_SALES_TAX = 1,
            REGION = 2,
            STATE_SALES_TAX = 3,
            VAT = 4,
        }

        public enum CollectionMethods
        {
            INVOICE = 0,
            NET = 1,
        }

        public enum LineItemFulfillmentStatus
        {
            FULFILLED,
            IN_PROGRESS,
            NOT_STARTED
        }

        public enum SoldFormat
        {
            AUCTION = 0,
            FIXED_PRICE = 1,
            OTHER = 2,
            SECOND_CHANCE_OFFER = 3,
        }

        public enum PaymentMethods
        {
            CREDIT_CARD = 0,
            PAYPAL = 1
        }

        public enum PaymentStatuses
        {
            FAILED = 0,
            PAID = 1,
            PENDING = 2,
        }

        public enum Conditions
        {
            NEW,
            LIKE_NEW,
            NEW_OTHER,
            NEW_WITH_DEFECTS,
            MANUFACTURER_REFURBISHED,
            SELLER_REFURBISHED,
            USED_EXCELLENT,
            USED_VERY_GOOD,
            USED_GOOD,
            USED_ACCEPTABLE,
            FOR_PARTS_OR_NOT_WORKING
        }

        public enum Locales
        {
            en_US,
            en_CA,
            fr_CA,
            en_GB,
            en_AU,
            en_IN,
            de_AT,
            fr_BE,
            fr_FR,
            de_DE,
            it_IT,
            nl_BE,
            nl_NL,
            es_ES,
            de_CH,
            fi_FI,
            zh_HK,
            hu_HU,
            en_PH,
            pl_PL,
            pt_PT,
            ru_RU,
            en_SG,
            en_IE,
            en_MY
        }

        public enum AvailabilityType
        {
            IN_STOCK,
            OUT_OF_STOCK,
            SHIP_TO_STORE
        };

        public enum Units
        {
            YEAR,
            MONTH,
            DAY,
            HOUR,
            CALENDAR_DAY,
            BUSINESS_DAY,
            MINUTE,
            SECOND,
            MILLISECOND
        }

        public enum PackageType
        {
            LETTER,
            BULKY_GOODS,
            CARAVAN,
            CARS,
            EUROPALLET,
            EXPANDABLE_TOUGH_BAGS,
            EXTRA_LARGE_PACK,
            FURNITURE,
            INDUSTRY_VEHICLES,
            LARGE_CANADA_POSTBOX,
            LARGE_CANADA_POST_BUBBLE_MAILER,
            LARGE_ENVELOPE,
            MAILING_BOX,
            MEDIUM_CANADA_POST_BOX,
            MOTORBIKES,
            MEDIUM_CANADA_POST_BUBBLE_MAILER,
            ONE_WAY_PALLET,
            PACKAGE_THICK_ENVELOPE,
            PADDED_BAGS,
            PARCEL_OR_PADDED_ENVELOPE,
            ROLL,
            SMALL_CANADA_POST_BOX,
            SMALL_CANADA_POST_BUBBLE_MAILER,
            TOUGH_BAGS,
            UPS_LETTER,
            USPS_FLAT_RATE_ENVELOPE,
            USPS_LARGE_PACK,
            VERY_LARGE_PACK,
            WINE_PAK
        }

        public enum WeightUnit
        {
            POUND,
            KILOGRAM,
            OUNCE,
            GRAM
        }

        public enum DimensionUnits
        {
            INCH,
            FEET,
            CENTIMETER,
            METER
        }

        public enum ListingStatues
        {
            ACTIVE,
            OUT_OF_STOCK,
            INACTIVE,
            ENDED,
            EBAY_ENDED,
            NOT_LISTED
        }

        public enum ListingDurations
        {
            GTC
        }

        public enum ShippingServiceTypes
        {
            DOMESTIC,
            INTERNATIONAL
        }

        public enum PricingVisibility
        {
            NONE,
            PRE_CHECKOUT,
            DURING_CHECKOUT
        }

        public enum OriginallySoldForRetailPriceOn
        {
            ON_EBAY,
            OFF_EBAY,
            ON_AND_OFF_EBAY
        }

        public enum Formats
        {
            FIXED_PRICE
        }

        public enum MarketPlaceIds
        {
            EBAY_GB
        }

        public enum DaysOfWeek
        {
            MONDAY,
            TUESDAY,
            WEDNESDAY,
            THURSDAY,
            FRIDAY,
            SATURDAY,
            SUNDAY
        }

        public enum LocationTypes
        {
            STORE,
            WHAREHOUSE
        }

        public enum MerchantLocationStatus
        {
            ENABLED,
            DISABLED
        }

        public enum ListingAction
        {
            CREATE,
            UPDATE,
            CREATE_GROUP
        }

        public enum OfferStatus
        {
            PUBLISHED,
            UNPUBLISHED
        }

        public enum BookingEntry
        {
            DEBIT,
            CREDIT
        }

        public enum EbayShippingServiceCode
        {
            UK_SellersStandardRate,
            UK_OtherCourier3Days,
            UK_OtherCourier,
            UK_RoyalMailTracked
        }

        public enum EbayRefundStatus
        {
            FAILED,
            PENDING,
            REFUNDED
        }
    }
}
