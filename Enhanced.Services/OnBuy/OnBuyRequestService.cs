using Enhanced.Models.OnBuy;
using Enhanced.Models.Shared;
using Newtonsoft.Json;
using RestSharp;
using System.Net;

namespace Enhanced.Services.OnBuy
{
    public class OnBuyRequestService : OnBuyApiUrls
    {
        protected RestClient? RequestClient { get; set; }
        protected RestRequest? Request { get; set; }
        public string? AuthorizationKey { get; set; }

        public OnBuyRequestService(string apiKey)
        {
            AuthorizationKey = apiKey;
        }

        protected void CreateAuthorizedRequest(string url, Method method, object postJsonBody = null!)
        {
            CreateRequest(url, method);
            AddAuthorizationKey();

            if (postJsonBody != null)
            {
                AddJsonContentType();
                AddJsonBody(postJsonBody);
            }
        }

        protected async Task<(T, HttpStatusCode)> ExecuteRequestNoneAsync<T>() where T : new()
        {
            var response = await RequestClient!.ExecuteAsync<T>(Request!);
            return (response.Data, response.StatusCode);
        }

        protected void CreateRequest(string url, Method method)
        {
            RequestClient = new RestClient(url);
            Request = new RestRequest(method);
        }

        private void AddAuthorizationKey()
        {
            Request!.AddHeader(Constant.HEADER_AUTHORIZATION, AuthorizationKey!);
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

        public static bool CheckHttpStatusCode(HttpStatusCode statusCode)
        {
            return statusCode == HttpStatusCode.Created || statusCode == HttpStatusCode.OK ||
                   statusCode == HttpStatusCode.NoContent || statusCode == HttpStatusCode.MultiStatus;
        }
    }
}
