namespace Enhanced.Models.Shared
{
    public class Constant
    {
        #region " Amazon "       
        public readonly static string AmazonToeknEndPoint = "https://api.amazon.com/auth/o2/token";
        public const string ScopeNotificationsAPI = "sellingpartnerapi::notifications";
        public const string ScopeMigrationAPI = "sellingpartnerapi::migration";
        public readonly static string DateISO8601Format = "yyyy-MM-ddTHH:mm:ss.fffZ";
        public const string AmazonStockLevelFileWithoutExt = "StockLevel";
        public const string AmazonStockLevelFileWithExt = "StockLevel.txt";
        public const string AmazonProductFileWithoutExt = "Product";
        public const string AmazonProductFileWithExt = "Product.txt";
        public const string AmazonFeedDateFormat = "yyyy-MM-dd";
        public const string AmazonFeedTimeFormat = "hh-mm-ss";
        public const string MarketplaceName = "amazon.co.uk";

        public const string DATE_FORMAT_DOT = "dd.MM.yyyy";
        public const string DATETIME_FORMAT_UTC_DOT = "dd.MM.yyyy HH:mm:ss UTC";
        public const string DATETIME_K_FORMAT = "yyyy-MM-ddTHH:mm:ssK";
        public const string DATE_BACKSLASH_FORMAT = "M/d/yy";
        public const string DATE_MMM_FORMAT = "dd-MMM-yyyy";
        public const string DATE_AGING_FORMAT = "yyyy-MM-dd";
        public const string DATETIME_DDMMYYY_HHMMSS = "dd/MM/yyyy HH:mm:ss";
        #endregion

        #region " Appeagle "
        public const string AppeagleFileWithoutExt = "appeagle";
        public const string AppeagleFileWithExt = "appeagle.csv";
        public readonly static string AppeagleDateCSVFormat = "yyyy-MM-dd-hh-mm";
        #endregion

        #region " PayPal "
        public const string PAYPALROWTYPE = "SB";
        public const string DATETIME_FORMAT_UTC_SLASH = "yyyy/MM/dd";
        public const string DATETIME_FORMAT_SETTLEMENT = "yyyy-MM-dd";
        #endregion

        #region " eBay "
        public static readonly string PAYLOAD_PARAM_DELIMITER = "&";
        public static readonly string PAYLOAD_VALUE_DELIMITER = "=";
        public static readonly string HEADER_AUTHORIZATION = "Authorization";
        public static readonly string HEADER_PREFIX_BASIC = "Basic ";
        public static readonly string HEADER_PREFIX_BEARER = "Bearer ";
        public static readonly string HEADER_CONTENT_TYPE = "application/x-www-form-urlencoded";
        public static readonly string PAYLOAD_GRANT_TYPE = "grant_type";
        public static readonly string PAYLOAD_REFRESH_TOKEN = "refresh_token";
        public static readonly string PAYLOAD_CLIENT_ID = "client_id";
        public static readonly string PAYLOAD_RESPONSE_TYPE = "response_type";
        public static readonly string PAYLOAD_REDIRECT_URI = "redirect_uri";
        public static readonly string PAYLOAD_SCOPE = "scope";
        public static readonly string PAYLOAD_VALUE_CODE = "code";
        public static readonly string PAYLOAD_AUTHORIZATION_CODE = "authorization_code";

        public static readonly string X_EBAY_SIGNATURE_KEY = "x-ebay-signature-key";
        public static readonly string SIGNATURE = "Signature";
        public static readonly string SIGNATURE_INPUT = "Signature-Input";
        public static readonly string X_EBAY_ENFORCE_SIGNATURE = "x-ebay-enforce-signature";
        public static readonly string CONTENT_DIGEST = "Content-Digest";

        public static readonly string COUNTRY_CODE_GB = "GB";
        public static readonly string POSTAL_CODE_JE = "JE";
        public static readonly string POSTAL_CODE_GY = "GY";

        public static readonly string ORDERBATCH_MANUAL = "MANUAL";
        public static readonly string ORDERBATCH_POST = "POST";
        public static readonly string ORDERBATCH_CARRIER = "CARRIER";
        #endregion

        #region " Mano Mano "
        public static readonly string MANO_API_KEY = "x-api-key";
        #endregion
    }
}
