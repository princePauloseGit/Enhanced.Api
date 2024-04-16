using Newtonsoft.Json;
using RestSharp;

namespace Enhanced.Models.AmazonData
{
    public class LWAClient
    {
        public const string AccessTokenKey = "access_token";
        public const string JsonMediaType = "application/json; charset=utf-8";

        public IRestClient RestClient { get; set; }
        public LWAAccessTokenRequestMetaBuilder LWAAccessTokenRequestMetaBuilder { get; set; }
        public LWAAuthorizationCredentials LWAAuthorizationCredentials { get; private set; }

        public LWAClient(LWAAuthorizationCredentials lwaAuthorizationCredentials)
        {

            LWAAuthorizationCredentials = lwaAuthorizationCredentials;
            LWAAccessTokenRequestMetaBuilder = new LWAAccessTokenRequestMetaBuilder();
            RestClient = new RestClient(LWAAuthorizationCredentials.Endpoint!.GetLeftPart(UriPartial.Authority));
        }

        /// <summary>
        /// Retrieves access token from LWA
        /// </summary>
        /// <param name="lwaAccessTokenRequestMeta">LWA AccessTokenRequest metadata</param>
        /// <returns>LWA Access Token</returns>
        public virtual async Task<TokenResponse> GetAccessTokenAsync()
        {
            LWAAccessTokenRequestMeta lwaAccessTokenRequestMeta = LWAAccessTokenRequestMetaBuilder.Build(LWAAuthorizationCredentials);
            var accessTokenRequest = new RestRequest(LWAAuthorizationCredentials.Endpoint!.AbsolutePath, Method.POST);

            string jsonRequestBody = JsonConvert.SerializeObject(lwaAccessTokenRequestMeta);

            accessTokenRequest.AddParameter(JsonMediaType, jsonRequestBody, ParameterType.RequestBody);

            try
            {
                var response = await RestClient.ExecuteAsync(accessTokenRequest).ConfigureAwait(false);

                if (!IsSuccessful(response))
                {
                    throw new IOException("Unsuccessful LWA token exchange", response.ErrorException);
                }

                TokenResponse tokenService = new();

                return JsonConvert.DeserializeObject<TokenResponse>(response.Content)!;
            }
            catch (System.Exception e)
            {
                throw new SystemException("Error getting LWA Access Token", e);
            }
        }

        private static bool IsSuccessful(IRestResponse response)
        {
            return (int)response.StatusCode >= 200 && (int)response.StatusCode <= 299 && response.ResponseStatus == ResponseStatus.Completed;
        }
    }

    public class LWAAccessTokenRequestMetaBuilder
    {
        public const string SellerAPIGrantType = "refresh_token";
        public const string SellerlessAPIGrantType = "client_credentials";
        private const string Delimiter = " ";

        /// <summary>
        /// Builds an instance of LWAAccessTokenRequestMeta modeling appropriate LWA token
        /// request params based on configured LWAAuthorizationCredentials
        /// </summary>
        /// <param name="lwaAuthorizationCredentials">LWA Authorization Credentials</param>
        /// <returns></returns>
        public virtual LWAAccessTokenRequestMeta Build(LWAAuthorizationCredentials lwaAuthorizationCredentials)
        {
            LWAAccessTokenRequestMeta lwaAccessTokenRequestMeta = new()
            {
                ClientId = lwaAuthorizationCredentials.ClientId,
                ClientSecret = lwaAuthorizationCredentials.ClientSecret,
                RefreshToken = lwaAuthorizationCredentials.RefreshToken
            };

            if (lwaAuthorizationCredentials.Scopes == null || lwaAuthorizationCredentials.Scopes.Count == 0)
            {
                lwaAccessTokenRequestMeta.GrantType = SellerAPIGrantType;
            }
            else
            {
                lwaAccessTokenRequestMeta.Scope = string.Join(Delimiter, lwaAuthorizationCredentials.Scopes);
                lwaAccessTokenRequestMeta.GrantType = SellerlessAPIGrantType;
            }

            return lwaAccessTokenRequestMeta;
        }
    }

    public class LWAAccessTokenRequestMeta
    {
        [JsonProperty(PropertyName = "grant_type")]
        public string? GrantType { get; set; }

        [JsonProperty(PropertyName = "refresh_token")]
        public string? RefreshToken { get; set; }

        [JsonProperty(PropertyName = "client_id")]
        public string? ClientId { get; set; }

        [JsonProperty(PropertyName = "client_secret")]
        public string? ClientSecret { get; set; }

        [JsonProperty(PropertyName = "scope")]
        public string? Scope { get; set; }
    }

    public class LWAAuthorizationCredentials
    {
        public LWAAuthorizationCredentials()
        {
            Scopes = new List<string>();
        }

        public string? ClientId { get; set; }
        public string? ClientSecret { get; set; }
        public string? RefreshToken { get; set; }
        public Uri? Endpoint { get; set; }
        public List<string>? Scopes { get; set; }
    }
}
