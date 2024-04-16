using System.Runtime.Serialization;

namespace Enhanced.Models.AmazonData
{
    /// <summary>
    /// The response schema for the getOrders operation.
    /// </summary>
    [DataContract]
    public partial class GetOrderResponse
    {
        /// <summary>
        /// The payload for the getOrders operation.
        /// </summary>
        /// <value>The payload for the getOrders operation.</value>
        [DataMember(Name = "payload", EmitDefaultValue = false)]
        public OrdersList? Payload { get; set; }

        /// <summary>
        /// One or more unexpected errors occurred during the getOrders operation.
        /// </summary>
        /// <value>One or more unexpected errors occurred during the getOrders operation.</value>
        [DataMember(Name = "errors", EmitDefaultValue = false)]
        public List<Error>? Errors { get; set; }
    }
}
