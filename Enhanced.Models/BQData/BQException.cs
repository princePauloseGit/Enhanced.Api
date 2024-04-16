using RestSharp;
using System.Net;

namespace Enhanced.Models.BQData
{
    public class BQException : Exception
    {
        public ExceptionResponse? Response { get; set; }

        public BQException(string msg, IRestResponse response = null!) : base(msg)
        {
            if (response != null)
            {
                Response = new ExceptionResponse
                {
                    Content = response.Content,
                    ResponseCode = response.StatusCode
                };
            }
        }

        public class ExceptionResponse
        {
            public string? Content { get; set; }
            public HttpStatusCode? ResponseCode { get; set; }
        }
    }
}
