using Enhanced.Models.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Enhanced.Models.EbayData
{
    /// <summary>
    /// Provides information about the client who is going to access ebay user's account
    /// Helps during auth process and refreshing the token
    /// </summary>
    public class ClientToken
    {
        /// <summary>
        /// This is base64 encoded clientId and clientSecret provided to ebay during auth.
        /// </summary>
        public string? OAuthCredentials { get; set; }
    }

    public class RefreshTokenInput
    {
        public string? OAuthSuccessUrl { get; set; }
        public CommonEnum.Environment Environment { get; set; }
        public string? OAuthCredentials { get; set; }
        public string? RuName { get; set; }
    }

    public class RefreshTokenOutput
    {
        public string? refresh_token { get; set; }
        public double refresh_token_expires_in { get; set; }
        public string? expiry_date { get; set; }
    }

    public class AccessToken
    {
        /// <summary>
        /// User Access Token
        /// </summary>
        [FromHeader]
        public string? access_token { get; set; }

        /// <summary>
        /// Access Token Expires In
        /// </summary>
        public int? expires_in { get; set; }

        /// <summary>
        /// Access Token Last Refreshed
        /// </summary>
        public DateTime? date_last_updated { get; set; }

        [FromHeader]
        public string? refresh_token { get; set; }

        /// <summary>
        /// This is base64 encoded clientId and clientSecret provided to ebay during auth.
        /// </summary>
        [FromHeader]
        public string? oauth_credentials { get; set; }

        [FromHeader]
        public CommonEnum.Environment Environment { get; set; }
    }

    public class AccessTokenViewModel
    {
        public string? access_token { get; set; }
        public int? expires_in { get; set; }
        public DateTime? date_last_updated { get; set; }
    }

    public class OAuthUrlInput
    {
        public CommonEnum.Environment Environment { get; set; }
        public string? ClientId { get; set; }
        public string? RuName { get; set; }
        public List<string>? Scopes { get; set; }
    }
}
