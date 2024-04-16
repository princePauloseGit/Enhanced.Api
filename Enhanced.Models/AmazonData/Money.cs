using System.Runtime.Serialization;

namespace Enhanced.Models.AmazonData
{
    /// <summary>
    /// The monetary value of the order.
    /// </summary>
    [DataContract]
    public partial class Money
    {
        /// <summary>
        /// The three-digit currency code. In ISO 4217 format.
        /// </summary>
        /// <value>The three-digit currency code. In ISO 4217 format.</value>
        [DataMember(Name = "CurrencyCode", EmitDefaultValue = false)]
        public string? CurrencyCode { get; set; }

        /// <summary>
        /// The currency amount.
        /// </summary>
        /// <value>The currency amount.</value>
        [DataMember(Name = "Amount", EmitDefaultValue = false)]
        public string? Amount { get; set; }
    }
}
