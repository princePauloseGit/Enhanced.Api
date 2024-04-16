using System.Runtime.Serialization;

namespace Enhanced.Models.AmazonData
{
    /// <summary>
    /// GetCatalogItemResponse
    /// </summary>
    [DataContract]
    public partial class GetCatalogItemResponse
    {
        /// <summary>
        /// The payload for the getCatalogItem operation.
        /// </summary>
        /// <value>The payload for the getCatalogItem operation.</value>
        [DataMember(Name = "payload", EmitDefaultValue = false)]
        public List<CatalogItem>? Payload { get; set; }

        /// <summary>
        /// One or more unexpected errors occurred during the getCatalogItem operation.
        /// </summary>
        /// <value>One or more unexpected errors occurred during the getCatalogItem operation.</value>
        [DataMember(Name = "errors", EmitDefaultValue = false)]
        public List<Error>? Errors { get; set; }
    }
}
