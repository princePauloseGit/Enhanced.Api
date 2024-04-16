using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Enhanced.Models.AmazonData
{
    public static class ErrorResponseHelper
    {
        public static ErrorResponse ConvertToErrorResponse(this string response)
        {
            if (string.IsNullOrEmpty(response))
            {
                return null!;
            };

            try
            {
                return JsonConvert.DeserializeObject<ErrorResponse>(response)!;
            }
            catch
            {
                return null!;
            }
        }
    }

    public class ErrorResponse
    {
        [JsonProperty("errors")]
        public ErrorResponseElement[]? Errors;
    }

    public class ErrorResponseElement
    {
        [JsonProperty("message")]
        public string? Message { get; set; }
        [JsonProperty("code")]
        public string? Code { get; set; }
        [JsonProperty("details")]
        public string? Details { get; set; }
    }

    /// <summary>
    /// Error response returned when the request is unsuccessful.
    /// </summary>
    [DataContract]
    public partial class Error
    {
        /// <summary>
        /// An error code that identifies the type of error that occurred.
        /// </summary>
        /// <value>An error code that identifies the type of error that occurred.</value>
        [DataMember(Name = "code", EmitDefaultValue = false)]
        public string? Code { get; set; }

        /// <summary>
        /// A message that describes the error condition in a human-readable form.
        /// </summary>
        /// <value>A message that describes the error condition in a human-readable form.</value>
        [DataMember(Name = "message", EmitDefaultValue = false)]
        public string? Message { get; set; }

        /// <summary>
        /// Additional details that can help the caller understand or fix the issue.
        /// </summary>
        /// <value>Additional details that can help the caller understand or fix the issue.</value>
        [DataMember(Name = "details", EmitDefaultValue = false)]
        public string? Details { get; set; }
    }
}
