using Enhanced.Models.AmazonData;
using Enhanced.Models.Shared;
using Newtonsoft.Json;
using RestSharp;
using System.Globalization;
using System.Net;
using static Enhanced.Models.AmazonData.AmazonEnum;

namespace Enhanced.Services.AmazonServices
{
    public class AmazonRequestService : AmazonApiUrls
    {
        public static readonly string AccessTokenHeaderName = "x-amz-access-token";
        public static readonly string SecurityTokenHeaderName = "x-amz-security-token";
        private readonly string RateLimitLimitHeaderName = "x-amzn-RateLimit-Limit";
        public static readonly string ShippingBusinessIdHeaderName = "x-amzn-shipping-business-id";
        protected RestClient? RequestClient { get; set; }
        protected IRestRequest? Request { get; set; }
        protected AmazonCredential AmazonCredential { get; set; }
        protected string? AmazonSandboxUrl { get; set; }
        protected string? AmazonProductionUrl { get; set; }
        protected string? AccessToken { get; set; }
        protected IList<KeyValuePair<string, string>>? LastHeaders { get; set; }
        protected string ApiBaseUrl
        {
            get
            {
                return AmazonCredential.Environment == CommonEnum.Environment.Sandbox ? AmazonSandboxUrl! : AmazonProductionUrl!;
            }
        }

        /// <summary>
        /// Creates request base service
        /// </summary>
        /// <param name="awsCredentials">Contains api clients information</param>
        /// <param name="clientToken">Contains current user's account api keys</param>
        public AmazonRequestService(AmazonCredential amazonCredential)
        {
            if (amazonCredential == null)
            {
                return;
            }

            if (amazonCredential!.MarketPlace == null)
            {
                amazonCredential!.MarketPlace = MarketPlace.GetMarketPlaceById(amazonCredential.MarketPlaceId!);
            }

            AmazonCredential = amazonCredential;
            AmazonSandboxUrl = amazonCredential.MarketPlace!.Region!.SandboxHostUrl;
            AmazonProductionUrl = amazonCredential.MarketPlace!.Region!.HostUrl;
        }

        private void CreateRequest(string url, Method method)
        {
            RequestClient = new RestClient(ApiBaseUrl);
            Request = new RestRequest(url, method);
        }

        protected async Task CreateAuthorizedRequestAsync(string url, Method method, List<KeyValuePair<string, string>> queryParameters = null!, object postJsonObj = null!, TokenDataType tokenDataType = TokenDataType.Normal, CreateRestrictedDataTokenRequest createRestrictedDataTokenRequest = null!)
        {
            if (tokenDataType == TokenDataType.PII && createRestrictedDataTokenRequest != null)
            {
                await RefreshTokenAsync(TokenDataType.PII, createRestrictedDataTokenRequest);
            }
            else
            {
                await RefreshTokenAsync(tokenDataType);
            }

            CreateRequest(url, method);

            if (postJsonObj != null)
            {
                AddJsonBody(postJsonObj);
            }

            if (queryParameters != null)
            {
                AddQueryParameters(queryParameters);
            }
        }

        protected async Task RefreshTokenAsync(TokenDataType tokenDataType = TokenDataType.Normal, CreateRestrictedDataTokenRequest createRestrictedDataTokenRequest = null!)
        {
            var token = AmazonCredential.GetToken(tokenDataType);

            if (token == null)
            {
                if (tokenDataType == TokenDataType.PII && createRestrictedDataTokenRequest != null)
                {
                    var pii = CreateRestrictedDataToken(createRestrictedDataTokenRequest);

                    if (pii != null)
                    {
                        token = new TokenResponse
                        {
                            access_token = pii.RestrictedDataToken,
                            expires_in = pii.ExpiresIn
                        };
                    }
                    else
                    {
                        throw new ArgumentNullException(nameof(pii));
                    }
                }
                else
                {
                    token = await AmazonTokenGeneration.RefreshAccessTokenAsync(AmazonCredential, tokenDataType);
                }

                AmazonCredential.SetToken(tokenDataType, token);
            }

            AccessToken = token.access_token;
        }

        protected void AddJsonBody(object jsonData)
        {
            var json = JsonConvert.SerializeObject(jsonData);
            Request!.AddJsonBody(json);
        }

        protected void AddQueryParameters(List<KeyValuePair<string, string>> queryParameters)
        {
            if (queryParameters != null)
            {
                queryParameters.ForEach(qp => Request!.AddQueryParameter(qp.Key, qp.Value));
            }
        }

        public async Task<T> ExecuteRequestAsync<T>(RateLimitType rateLimitType = RateLimitType.UNSET) where T : new()
        {
            var tryCount = 0;

            while (true)
            {
                try
                {
                    return await ExecuteRequestTry<T>(rateLimitType);
                }
                catch (AmazonQuotaExceededException ex)
                {
                    switch (tryCount)
                    {
                        case 1:
                            Task.Delay(2000).Wait();
                            break;

                        case 2:
                            Task.Delay(3000).Wait();
                            break;

                        case 3:
                            Task.Delay(4000).Wait();
                            break;

                        case 4:
                            Task.Delay(5000).Wait();
                            break;
                    }

                    if (tryCount >= AmazonCredential.MaxThrottledRetryCount)
                    {
                        throw ex;
                    }

                    await AmazonCredential.UsagePlansTimings[rateLimitType].Delay();
                    tryCount++;
                }
            }
        }

        [Obsolete]
        protected async Task<T> ExecuteRequestTry<T>(RateLimitType rateLimitType = RateLimitType.UNSET) where T : new()
        {
            RestHeader();
            AddAccessToken();

            Request = await AmazonTokenGeneration.SignWithSTSKeysAndSecurityTokenAsync(Request!, RequestClient!.BaseUrl!.Host, AmazonCredential);

            var response = await RequestClient.ExecuteAsync<T>(Request);
            SaveLastRequestHeader(response.Headers);
            SleepForRateLimit(response.Headers, rateLimitType);
            ParseResponse(response);

            if (response.StatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(response.Content) && response.Data == null)
            {
                response.Data = JsonConvert.DeserializeObject<T>(response.Content)!;
            }

            return response.Data;
        }

        private void RestHeader()
        {
            lock (Request!)
            {
                Request.Parameters.RemoveAll(parameter => ParameterType.HttpHeader.Equals(parameter.Type) && parameter.Name == AWSSignerHelper.XAmzDateHeaderName);
                Request.Parameters.RemoveAll(parameter => ParameterType.HttpHeader.Equals(parameter.Type) && parameter.Name == AWSSignerHelper.AuthorizationHeaderName);
                Request.Parameters.RemoveAll(parameter => ParameterType.HttpHeader.Equals(parameter.Type) && parameter.Name == AccessTokenHeaderName);
                Request.Parameters.RemoveAll(parameter => ParameterType.HttpHeader.Equals(parameter.Type) && parameter.Name == SecurityTokenHeaderName);
                Request.Parameters.RemoveAll(parameter => ParameterType.HttpHeader.Equals(parameter.Type) && parameter.Name == ShippingBusinessIdHeaderName);
            }
        }

        protected void AddAccessToken()
        {
            lock (Request!)
            {
                Request.AddOrUpdateHeader(AccessTokenHeaderName, AccessToken!);
            }
        }

        [Obsolete]
        private void SaveLastRequestHeader(IList<Parameter> parameters)
        {
            LastHeaders = new List<KeyValuePair<string, string>>();

            foreach (Parameter parameter in parameters)
            {
                if (parameter != null && parameter.Name != null && parameter.Value != null)
                {
                    LastHeaders.Add(new KeyValuePair<string, string>(parameter.Name.ToString(), parameter.Value.ToString()!));
                }
            }
        }

        [Obsolete]
        private void SleepForRateLimit(IList<Parameter> headers, RateLimitType rateLimitType = RateLimitType.UNSET)
        {
            try
            {
                decimal rate = 0;
                var limitHeader = headers.Where(a => a.Name == RateLimitLimitHeaderName).FirstOrDefault();

                if (limitHeader != null)
                {
                    var rateLimitValue = limitHeader.Value!.ToString();
                    decimal.TryParse(rateLimitValue, NumberStyles.Any, CultureInfo.InvariantCulture, out rate);
                }

                if (AmazonCredential.IsActiveLimitRate)
                {
                    if (rateLimitType == RateLimitType.UNSET)
                    {
                        if (rate > 0)
                        {
                            int sleepTime = (int)(1 / rate * 1000);
                            Task.Delay(sleepTime).Wait();
                        }
                    }
                    else
                    {
                        if (rate > 0)
                        {
                            AmazonCredential.UsagePlansTimings[rateLimitType].SetRateLimit(rate);
                            int sleepTime = (int)(rate * 1000);
                            Task.Delay(sleepTime).Wait();
                        }
                        else
                        {
                            Task.Delay(2000).Wait();
                        }

                        AmazonCredential.UsagePlansTimings[rateLimitType].NextRate();
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        protected void ParseResponse(IRestResponse response)
        {
            if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Accepted || response.StatusCode == HttpStatusCode.Created)
            {
                return;
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new AmazonNotFoundException("Resource that you are looking for is not found", response);
            }
            else
            {
                var errorResponse = response.Content.ConvertToErrorResponse();

                if (errorResponse != null)
                {
                    var error = errorResponse.Errors!.FirstOrDefault();

                    switch (error!.Code)
                    {
                        case "Unauthorized":
                            throw new AmazonUnauthorizedException(error.Message!, response);

                        case "InvalidSignature":
                            throw new AmazonInvalidSignatureException(error.Message!, response);

                        case "InvalidInput":
                            throw new AmazonInvalidInputException(error.Message!, response);

                        case "QuotaExceeded":
                            throw new AmazonQuotaExceededException(error.Message!, response);
                    }
                }
            }

            throw new AmazonException("Amazon Api didn't respond with Okay, see exception for more details", response);
        }

        public CreateRestrictedDataTokenResponse CreateRestrictedDataToken(CreateRestrictedDataTokenRequest createRestrictedDataTokenRequest)
        {
            return Task.Run(() => CreateRestrictedDataTokenAsync(createRestrictedDataTokenRequest)).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public async Task<CreateRestrictedDataTokenResponse> CreateRestrictedDataTokenAsync(CreateRestrictedDataTokenRequest createRestrictedDataTokenRequest)
        {
            await CreateAuthorizedRequestAsync(TokenApiUrls.RestrictedDataToken, Method.POST, postJsonObj: createRestrictedDataTokenRequest);
            var response = await ExecuteRequestAsync<CreateRestrictedDataTokenResponse>();
            return response;
        }
    }
}
