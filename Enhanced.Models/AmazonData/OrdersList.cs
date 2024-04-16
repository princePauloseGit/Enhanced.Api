using System.Runtime.Serialization;

namespace Enhanced.Models.AmazonData
{
    /// <summary>
    /// A list of orders along with additional information to make subsequent API calls.
    /// </summary>
    [DataContract]
    public partial class OrdersList
    {
        /// <summary>
        /// Gets or Sets Orders
        /// </summary>
        [DataMember(Name = "Orders", EmitDefaultValue = false)]
        public List<Order>? Orders { get; set; }

        /// <summary>
        /// When present and not empty, pass this string token in the next request to return the next response page.
        /// </summary>
        /// <value>When present and not empty, pass this string token in the next request to return the next response page.</value>
        [DataMember(Name = "NextToken", EmitDefaultValue = false)]
        public string? NextToken { get; set; }

        /// <summary>
        /// Gets or Sets Order Items
        /// </summary>
        [DataMember(Name = "OrderItems", EmitDefaultValue = false)]
        public List<OrderItem>? OrderItems { get; set; }

        /// <summary>
        /// A date used for selecting orders that were last updated before (or at) a specified time. An update is defined as any change in order status, including the creation of a new order. Includes updates made by Amazon and by the seller. All dates must be in ISO 8601 format.
        /// </summary>
        /// <value>A date used for selecting orders that were last updated before (or at) a specified time. An update is defined as any change in order status, including the creation of a new order. Includes updates made by Amazon and by the seller. All dates must be in ISO 8601 format.</value>
        [DataMember(Name = "LastUpdatedBefore", EmitDefaultValue = false)]
        public string? LastUpdatedBefore { get; set; }

        /// <summary>
        /// A date used for selecting orders created before (or at) a specified time. Only orders placed before the specified time are returned. The date must be in ISO 8601 format.
        /// </summary>
        /// <value>A date used for selecting orders created before (or at) a specified time. Only orders placed before the specified time are returned. The date must be in ISO 8601 format.</value>
        [DataMember(Name = "CreatedBefore", EmitDefaultValue = false)]
        public string? CreatedBefore { get; set; }
    }
}
