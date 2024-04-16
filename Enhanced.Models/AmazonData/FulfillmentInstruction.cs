using System.Runtime.Serialization;

namespace Enhanced.Models.AmazonData
{
    /// <summary>
    /// Contains the instructions about the fulfillment like where should it be fulfilled from.
    /// </summary>
    [DataContract]
    public partial class FulfillmentInstruction
    {
        /// <summary>
        /// Denotes the recommended sourceId where the order should be fulfilled from.
        /// </summary>
        /// <value>Denotes the recommended sourceId where the order should be fulfilled from.</value>
        [DataMember(Name = "FulfillmentSupplySourceId", EmitDefaultValue = false)]
        public string? FulfillmentSupplySourceId { get; set; }
    }
}
