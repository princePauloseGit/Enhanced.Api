using Enhanced.Models.AmazonData;
using Enhanced.Models.BQData;
using Newtonsoft.Json;
using RestSharp;
using System.Net;
using static Enhanced.Models.BQData.BQEnum;

namespace Enhanced.Services.BQ
{
    public class BQRequestService : BQApiUrls
    {
        private readonly string AuthorizationHeaderName = "Authorization";
        private int MaxThrottledRetryCount { get; set; } = 3;
        private Dictionary<RateLimitType, RateLimits> UsagePlansTimings { get; set; } = RateLimitsTime();

        protected RestClient? RequestClient { get; set; }
        protected IRestRequest? Request { get; set; }

        private void CreateRequest(string url, Method method)
        {
            RequestClient = new RestClient(_baseUrl);
            Request = new RestRequest(url, method);
        }

        protected void CreateAuthorizedRequestAsync(string url, Method method, List<KeyValuePair<string, string>> queryParameters = null!, object postJsonObj = null!)
        {
            CreateRequest(url, method);

            if (queryParameters != null)
            {
                queryParameters.ForEach(qp => Request!.AddQueryParameter(qp.Key, qp.Value));
            }

            if (postJsonObj != null)
            {
                AddJsonContentType();
                AddJsonBody(postJsonObj);
            }
        }

        private void AddJsonContentType()
        {
            Request!.AddHeader("Content-Type", "application/json");
        }

        protected void AddJsonBody(object jsonData)
        {
            var json = JsonConvert.SerializeObject(jsonData);
            Request!.AddJsonBody(json);
        }

        public async Task<T> ExecuteRequestAsync<T>(string key, RateLimitType rateLimitType = RateLimitType.UNSET) where T : new()
        {
            var tryCount = 0;

            while (true)
            {
                try
                {
                    AuthorizationHeader(key);

                    return await ExecuteRequestTry<T>();
                }
                catch (BQTooManyRequestException ex)
                {
                    if (tryCount >= MaxThrottledRetryCount)
                    {
                        throw ex;
                    }

                    await UsagePlansTimings[rateLimitType].Delay();
                    tryCount++;
                }
            }
        }

        protected async Task<T> ExecuteRequestTry<T>() where T : new()
        {
            var response = await RequestClient!.ExecuteAsync<T>(Request!);

            if ((response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created) && !string.IsNullOrEmpty(response.Content))
            {
                response.Data = JsonConvert.DeserializeObject<T>(response.Content)!;
            }

            return response.Data;
        }

        private static Dictionary<RateLimitType, RateLimits> RateLimitsTime()
        {
            return new Dictionary<RateLimitType, RateLimits>()
            {
                { RateLimitType.BQ_GetOrders,   new RateLimits(0.0055M, 20) },
                { RateLimitType.BQ_GetPayments, new RateLimits(0.0055M, 20) },
                { RateLimitType.BQ_Shipment,    new RateLimits(0.0055M, 20) },
                { RateLimitType.BQ_Refund,      new RateLimits(0.0055M, 20) },
            };
        }

        protected void AuthorizationHeader(string Authorization)
        {
            lock (Request!)
            {
                Request.AddOrUpdateHeader(AuthorizationHeaderName, Authorization);
            }
        }

        public class BQUnauthorizedException : BQException
        {
            public BQUnauthorizedException(string msg, IRestResponse response = null!) : base(msg, response) { }
        }

        public class BQNotFoundException : BQException
        {
            public BQNotFoundException(string msg, IRestResponse response = null!) : base(msg, response) { }
        }

        public class BQTooManyRequestException : BQException
        {
            public BQTooManyRequestException(string msg, IRestResponse response = null!) : base(msg, response)
            {

            }
        }
    }
}
