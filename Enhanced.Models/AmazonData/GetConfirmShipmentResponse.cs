using System.Runtime.Serialization;

namespace Enhanced.Models.AmazonData
{
    /// <summary>
    /// The response schema for the getOrders operation.
    /// </summary>
    [DataContract]
    public partial class GetConfirmShipmentResponse
    {
        /// <summary>
        /// The payload for the getOrders operation.
        /// </summary>
        /// <value>The payload for the getOrders operation.</value>
        [DataMember(Name = "response", EmitDefaultValue = false)]
        public string? Response { get; set; }

        /// <summary>
        /// One or more unexpected errors occurred during the getOrders operation.
        /// </summary>
        /// <value>One or more unexpected errors occurred during the getOrders operation.</value>
        [DataMember(Name = "errors", EmitDefaultValue = false)]
        public List<Error>? Errors { get; set; }
    }

    public class ConfirmShipment
    {
        public string? OrderId { get; set; }
        public bool IsConfirmedShipment { get; set; }
    }
}
