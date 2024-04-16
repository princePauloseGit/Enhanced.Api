namespace Enhanced.Models.AmazonData
{
    public class ParameterConfirmShipment : ParameterBased
    {
        public string? OrderId { get; set; }
        public ConfirmShipmentRequest? ConfirmShipmentRequest { get; set; }
    }

    public class ConfirmShipmentRequest
    {
        public string? MarketplaceId { get; set; }
        public PackageDetail? PackageDetail { get; set; }
        //public CodCollectionMethod? CODCollectionMethod { get; set; }
    }

    public class PackageDetail
    {
        /// <summary>
        /// Only positive numeric values
        /// </summary>
        public string? PackageReferenceId { get; set; }

        public string? CarrierCode { get; set; }

        //public string? CarrierName { get; set; }

        //public string? ShippingMethod { get; set; }

        public string? TrackingNumber { get; set; }

        public DateTime? ShipDate { get; set; }

        //public string? ShipFromSupplySourceId { get; set; }

        public List<OrderItems>? OrderItems { get; set; }
    }

    public class OrderItems
    {
        public string? OrderItemId { get; set; }
        public int Quantity { get; set; }
        //public List<string>? TransparencyCodes { get; set; }
    }
}
