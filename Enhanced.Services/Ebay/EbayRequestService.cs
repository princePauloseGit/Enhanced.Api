using Enhanced.Models.EbayData;
using Enhanced.Models.Shared;
using Newtonsoft.Json;
using RestSharp;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Enhanced.Services.Ebay
{
    public class EbayRequestService : EbayApiUrls
    {
        protected RestClient? RequestClient { get; set; }
        protected RestRequest? Request { get; set; }
        protected ClientToken? ClientToken { get; set; }
        public AccessToken? AccessToken { get; set; }

        /// <summary>
        /// Creates request base service
        /// </summary>
        /// <param name="clientToken">Contains api clients information</param>
        /// <param name="accessToken">Contains current user's account api keys</param>
        public EbayRequestService(ClientToken clientToken, AccessToken accessToken)
        {
            ClientToken = clientToken;
            AccessToken = accessToken;
        }

        protected void CreateRequest(string url, Method method)
        {
            RequestClient = new RestClient(url);
            Request = new RestRequest(method);
        }

        protected async Task CreateAuthorizedPagedRequestAsync(string nextPage, string url, Method method)
        {
            await RefreshTokenAsync();

            if (!string.IsNullOrEmpty(nextPage))
            {
                CreateRequest(nextPage, method);
            }
            else
            {
                CreateRequest(url, method);
            }

            AddBearerToken();
        }

        protected async Task CreateAuthorizedRequestAsync(string url, Method method, object postJsonObj = null!)
        {
            await RefreshTokenAsync();
            CreateRequest(url, method);
            AddBearerToken();

            if (postJsonObj != null)
            {
                AddJsonContentType();
                AddContentLanguage();
                AddJsonBody(postJsonObj);
            }
        }

        protected async Task CreateDigitalSignatureRequestAsync(string url, Method method, EbayPaymentParameter eBayPayment, object postJsonObj = null!)
        {
            await RefreshTokenAsync();
            CreateRequest(url, method);
            AddBearerToken();

            AddSignatureKey(eBayPayment.JWE!);

            if (method == Method.POST)
            {
                var body = ComputeSha256Hash(JsonConvert.SerializeObject(postJsonObj));
                
                AddContentDigest(body);

                AddJsonBody(postJsonObj);
            }

            var digitalSign = new EbayDigitalSignature(method == Method.POST);
            var uri = RequestClient!.BuildUri(Request!);
            var signature = digitalSign.GetSignature(eBayPayment.PrivateKey!, Request!, uri);
            AddSignature(signature);

            var signatureInput = digitalSign.GetSignatureInput();
            AddSignatureInput(signatureInput);

            AddEnforceSignature();
        }

        static string ComputeSha256Hash(string content)
        {
            // Create a SHA256
            using (var sha256 = SHA256.Create())
            {
                // ComputeHash - returns byte array
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(content));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        /// <summary>
        /// Executes the request
        /// </summary>
        /// <typeparam name="T">Type to parse response to</typeparam>
        /// <returns>Returns data of T type</returns>
        protected async Task<T> ExecuteRequestAsync<T>() where T : new()
        {
            var response = await RequestClient!.ExecuteAsync<T>(Request!);
            return response.Data;
        }

        protected async Task<(HttpStatusCode, T)> ExecuteRequestStatusCodeAsync<T>() where T : new()
        {
            var response = await RequestClient!.ExecuteAsync<T>(Request!);
            return (response.StatusCode, JsonConvert.DeserializeObject<T>(response?.Content));
        }

        protected async Task<(bool, EbayErrorResponse)> ExecuteRequestNoneAsync()
        {
            var response = await RequestClient!.ExecuteAsync(Request!);
            return (CheckHttpStatusCode(response.StatusCode), JsonConvert.DeserializeObject<EbayErrorResponse>(response?.Content));
        }

        public static bool CheckHttpStatusCode(HttpStatusCode statusCode)
        {
            return statusCode == HttpStatusCode.Created || statusCode == HttpStatusCode.OK ||
                   statusCode == HttpStatusCode.NoContent || statusCode == HttpStatusCode.MultiStatus;
        }

        protected void AddJsonBody(object jsonData)
        {
            var json = JsonConvert.SerializeObject(jsonData);
            Request!.AddJsonBody(json);
        }

        private void AddJsonContentType()
        {
            Request!.AddHeader("Content-Type", "application/json");
        }

        private void AddContentLanguage()
        {
            Request!.AddHeader("Content-Language", ContentLanguage);
        }

        private void AddBearerToken()
        {
            Request!.AddHeader(Constant.HEADER_AUTHORIZATION, Constant.HEADER_PREFIX_BEARER + AccessToken!.access_token);
        }

        private void AddSignatureKey(string jwe)
        {
            Request!.AddHeader(Constant.X_EBAY_SIGNATURE_KEY, $"{jwe}");
        }

        private void AddSignature(string signSignature)
        {
            Request!.AddHeader(Constant.SIGNATURE, $"sig1=:{signSignature}:");
        }

        private void AddSignatureInput(string signatureInput)
        {
            Request!.AddHeader(Constant.SIGNATURE_INPUT, $"sig1={signatureInput}");
        }

        private void AddEnforceSignature()
        {
            Request!.AddHeader(Constant.X_EBAY_ENFORCE_SIGNATURE, "true");
        }

        private void AddContentDigest(string contentHash)
        {
            Request!.AddHeader(Constant.CONTENT_DIGEST, $"sha-256=:{contentHash}:");
        }

        protected async Task RefreshTokenAsync()
        {
            AccessToken = await OauthService.GetAccessTokenAsync(ClientToken!, AccessToken!);
        }

        public static string GetOAuthUrl(OAuthUrlInput oAuthUrlInput)
        {
            return OauthService.GetOAuthUrl(oAuthUrlInput);
        }

        public static async Task<RefreshTokenOutput> GetRefreshToken(RefreshTokenInput refreshTokenInput)
        {
            return await OauthService.GetRefreshTokenAsync(refreshTokenInput);
        }
    }
}
