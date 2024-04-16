using System.Runtime.Serialization;

namespace Enhanced.Models.AmazonData
{
    /// <summary>
    /// Buyer information
    /// </summary>
    [DataContract]
    public partial class BuyerInfo
    {
        /// <summary>
        /// The anonymized email address of the buyer.
        /// </summary>
        /// <value>The anonymized email address of the buyer.</value>
        [DataMember(Name = "BuyerEmail", EmitDefaultValue = false)]
        public string? BuyerEmail { get; set; }

        /// <summary>
        /// The name of the buyer.
        /// </summary>
        /// <value>The name of the buyer.</value>
        [DataMember(Name = "BuyerName", EmitDefaultValue = false)]
        public string? BuyerName { get; set; }

        /// <summary>
        /// The county of the buyer.
        /// </summary>
        /// <value>The county of the buyer.</value>
        [DataMember(Name = "BuyerCounty", EmitDefaultValue = false)]
        public string? BuyerCounty { get; set; }

        /// <summary>
        /// Tax information about the buyer.
        /// </summary>
        /// <value>Tax information about the buyer.</value>
        [DataMember(Name = "BuyerTaxInfo", EmitDefaultValue = false)]
        public BuyerTaxInfo? BuyerTaxInfo { get; set; }

        /// <summary>
        /// The purchase order (PO) number entered by the buyer at checkout. Returned only for orders where the buyer entered a PO number at checkout.
        /// </summary>
        /// <value>The purchase order (PO) number entered by the buyer at checkout. Returned only for orders where the buyer entered a PO number at checkout.</value>
        [DataMember(Name = "PurchaseOrderNumber", EmitDefaultValue = false)]
        public string? PurchaseOrderNumber { get; set; }
    }

    /// <summary>
    /// Tax information about the buyer.
    /// </summary>
    [DataContract]
    public partial class BuyerTaxInfo
    {
        /// <summary>
        /// The legal name of the company.
        /// </summary>
        /// <value>The legal name of the company.</value>
        [DataMember(Name = "CompanyLegalName", EmitDefaultValue = false)]
        public string? CompanyLegalName { get; set; }

        /// <summary>
        /// The country or region imposing the tax.
        /// </summary>
        /// <value>The country or region imposing the tax.</value>
        [DataMember(Name = "TaxingRegion", EmitDefaultValue = false)]
        public string? TaxingRegion { get; set; }

        /// <summary>
        /// A list of tax classifications that apply to the order.
        /// </summary>
        /// <value>A list of tax classifications that apply to the order.</value>
        [DataMember(Name = "TaxClassifications", EmitDefaultValue = false)]
        public List<TaxClassification>? TaxClassifications { get; set; }
    }
}
