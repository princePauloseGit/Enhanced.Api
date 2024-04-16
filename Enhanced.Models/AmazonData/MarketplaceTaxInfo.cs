using System.Runtime.Serialization;

namespace Enhanced.Models.AmazonData
{
    /// <summary>
    /// Tax information about the marketplace.
    /// </summary>
    [DataContract]
    public partial class MarketplaceTaxInfo
    {
        /// <summary>
        /// A list of tax classifications that apply to the order.
        /// </summary>
        /// <value>A list of tax classifications that apply to the order.</value>
        [DataMember(Name = "TaxClassifications", EmitDefaultValue = false)]
        public List<TaxClassification>? TaxClassifications { get; set; }
    }

    /// <summary>
    /// The tax classification for the order.
    /// </summary>
    [DataContract]
    public partial class TaxClassification
    {
        /// <summary>
        /// The type of tax.
        /// </summary>
        /// <value>The type of tax.</value>
        [DataMember(Name = "Name", EmitDefaultValue = false)]
        public string? Name { get; set; } = default!;

        /// <summary>
        /// The buyer&#39;s tax identifier.
        /// </summary>
        /// <value>The buyer&#39;s tax identifier.</value>
        [DataMember(Name = "Value", EmitDefaultValue = false)]
        public string Value { get; set; } = default!;
    }
}
