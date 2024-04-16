using System.Runtime.Serialization;

namespace Enhanced.Models.AmazonData
{
    /// <summary>
    /// Information about a sub-payment method used to pay for a COD order.
    /// </summary>
    [DataContract]
    public partial class PaymentExecutionDetailItem
    {
        /// <summary>
        /// Gets or Sets Payment
        /// </summary>
        [DataMember(Name = "Payment", EmitDefaultValue = false)]
        public Money? Payment { get; set; }

        /// <summary>
        /// A sub-payment method for a COD order.  Possible values:  * COD - Cash On Delivery.  * GC - Gift Card.  * PointsAccount - Amazon Points.
        /// </summary>
        /// <value>A sub-payment method for a COD order.  Possible values:  * COD - Cash On Delivery.  * GC - Gift Card.  * PointsAccount - Amazon Points.</value>
        [DataMember(Name = "PaymentMethod", EmitDefaultValue = false)]
        public string? PaymentMethod { get; set; }
    }
}
