using System.Runtime.Serialization;
using static Enhanced.Models.AmazonData.AmazonEnum;

namespace Enhanced.Models.AmazonData
{
    /// <summary>
    /// The shipping address for the order.
    /// </summary>
    [DataContract]
    public partial class Address
    {
        /// <summary>
        /// The address type of the shipping address.
        /// </summary>
        /// <value>The address type of the shipping address.</value>
        [DataMember(Name = "AddressType", EmitDefaultValue = false)]
        public AddressType? AddressType { get; set; }

        /// <summary>
        /// The name.
        /// </summary>
        /// <value>The name.</value>
        [DataMember(Name = "Name", EmitDefaultValue = false)]
        public string? Name { get; set; }

        /// <summary>
        /// The street address.
        /// </summary>
        /// <value>The street address.</value>
        [DataMember(Name = "AddressLine1", EmitDefaultValue = false)]
        public string? AddressLine1 { get; set; }

        /// <summary>
        /// Additional street address information, if required.
        /// </summary>
        /// <value>Additional street address information, if required.</value>
        [DataMember(Name = "AddressLine2", EmitDefaultValue = false)]
        public string? AddressLine2 { get; set; }

        /// <summary>
        /// Additional street address information, if required.
        /// </summary>
        /// <value>Additional street address information, if required.</value>
        [DataMember(Name = "AddressLine3", EmitDefaultValue = false)]
        public string? AddressLine3 { get; set; }

        /// <summary>
        /// The city 
        /// </summary>
        /// <value>The city </value>
        [DataMember(Name = "City", EmitDefaultValue = false)]
        public string? City { get; set; }

        /// <summary>
        /// The county.
        /// </summary>
        /// <value>The county.</value>
        [DataMember(Name = "County", EmitDefaultValue = false)]
        public string County { get; set; }

        /// <summary>
        /// The district.
        /// </summary>
        /// <value>The district.</value>
        [DataMember(Name = "District", EmitDefaultValue = false)]
        public string? District { get; set; }

        /// <summary>
        /// The state or region.
        /// </summary>
        /// <value>The state or region.</value>
        [DataMember(Name = "StateOrRegion", EmitDefaultValue = false)]
        public string? StateOrRegion { get; set; }

        /// <summary>
        /// The municipality.
        /// </summary>
        /// <value>The municipality.</value>
        [DataMember(Name = "Municipality", EmitDefaultValue = false)]
        public string? Municipality { get; set; }

        /// <summary>
        /// The postal code.
        /// </summary>
        /// <value>The postal code.</value>
        [DataMember(Name = "PostalCode", EmitDefaultValue = false)]
        public string? PostalCode { get; set; }

        /// <summary>
        /// The country code. A two-character country code, in ISO 3166-1 alpha-2 format.
        /// </summary>
        /// <value>The country code. A two-character country code, in ISO 3166-1 alpha-2 format.</value>
        [DataMember(Name = "CountryCode", EmitDefaultValue = false)]
        public string? CountryCode { get; set; }

        /// <summary>
        /// The phone number. Not returned for Fulfillment by Amazon (FBA) orders.
        /// </summary>
        /// <value>The phone number. Not returned for Fulfillment by Amazon (FBA) orders.</value>
        [DataMember(Name = "Phone", EmitDefaultValue = false)]
        public string? Phone { get; set; }
    }
}
