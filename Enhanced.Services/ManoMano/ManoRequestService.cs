using Enhanced.Models.ManoMano;
using Enhanced.Models.Shared;
using Newtonsoft.Json;
using RestSharp;
using System.Net;

namespace Enhanced.Services.ManoMano
{
    public class ManoRequestService : ManoApiUrls
    {
        protected RestClient? RequestClient { get; set; }
        protected RestRequest? Request { get; set; }

        public string? ApiKey { get; set; }

        public ManoRequestService(string apiKey)
        {
            ApiKey = apiKey;
        }

        protected void CreateRequest(string url, Method method)
        {
            RequestClient = new RestClient(url);
            Request = new RestRequest(method);
        }

        protected async Task<T> ExecuteRequestAsync<T>() where T : new()
        {
            var response = await RequestClient!.ExecuteAsync<T>(Request!);
            return response.Data;
        }

        protected async Task<(T, HttpStatusCode)> ExecuteRequestNoneAsync<T>() where T : new()
        {
            var response = await RequestClient!.ExecuteAsync<T>(Request!);
            return (response.Data, response.StatusCode);
        }

        protected void CreateAuthorizedRequest(string url, Method method, List<KeyValuePair<string, string>> queryParameters = null!, object postJsonBody = null!)
        {
            CreateRequest(url, method);
            AddApiKey();

            if (queryParameters != null)
            {
                AddQueryParameters(queryParameters);
            }

            if (postJsonBody != null)
            {
                AddJsonContentType();
                AddJsonBody(postJsonBody);
            }
        }

        private void AddApiKey()
        {
            Request!.AddHeader(Constant.MANO_API_KEY, ApiKey!);
        }

        protected void AddQueryParameters(List<KeyValuePair<string, string>> queryParameters)
        {
            if (queryParameters != null)
            {
                queryParameters.ForEach(qp => Request!.AddQueryParameter(qp.Key, qp.Value));
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

        public static bool CheckHttpStatusCode(HttpStatusCode statusCode)
        {
            return statusCode == HttpStatusCode.Created || statusCode == HttpStatusCode.OK ||
                   statusCode == HttpStatusCode.NoContent || statusCode == HttpStatusCode.MultiStatus;
        }
    }
}
