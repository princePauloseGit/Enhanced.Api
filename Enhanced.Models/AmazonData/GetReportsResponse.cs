using System.Runtime.Serialization;

namespace Enhanced.Models.AmazonData
{
    /// <summary>
    /// The response for the getReports operation.
    /// </summary>
    [DataContract]
    public class GetReportsResponse
    {
        /// <summary>
        /// The payload for the getReports operation.
        /// </summary>
        /// <value>The payload for the getReports operation.</value>
        [DataMember(Name = "reports", EmitDefaultValue = false)]
        public List<AmazonReport>? Reports { get; set; }

        /// <summary>
        /// Returned when the number of results exceeds pageSize. To get the next page of results, call getReports with this token as the only parameter.
        /// </summary>
        /// <value>Returned when the number of results exceeds pageSize. To get the next page of results, call getReports with this token as the only parameter.</value>
        [DataMember(Name = "nextToken", EmitDefaultValue = false)]
        public string? NextToken { get; set; }

        /// <summary>
        /// Gets or Sets Errors
        /// </summary>
        [DataMember(Name = "errors", EmitDefaultValue = false)]
        public List<Error>? Errors { get; set; }
    }
}
