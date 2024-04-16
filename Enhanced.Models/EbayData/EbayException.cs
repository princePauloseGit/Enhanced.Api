using RestSharp;
using System.Net;
using static Enhanced.Models.EbayData.EbayErrorHelper;

namespace Enhanced.Models.EbayData
{
    public class EbayException : Exception
    {
        public ExceptionResponse? Response { get; set; }
        public Errors? Errors { get; set; }

        public EbayException(string msg, IRestResponse response = null!) : base(msg)
        {
            if (response != null)
            {
                Response = new ExceptionResponse
                {
                    Content = response.Content,
                    ResponseCode = response.StatusCode
                };
                Errors = ParseResponse();
            }
        }

        private Errors ParseResponse()
        {
            try
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<Errors>(Response!.Content);
            }
            catch (Exception)
            {
                return default!;
            }
        }

    }

    public class NotFoundException : EbayException
    {
        public NotFoundException(string msg, IRestResponse response = null!) : base(msg, response)
        {

        }
    }

    public class ExceptionResponse
    {
        public string? Content { get; set; }
        public HttpStatusCode? ResponseCode { get; set; }
    }
}
