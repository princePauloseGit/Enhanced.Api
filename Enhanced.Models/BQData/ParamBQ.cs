using Enhanced.Models.AmazonData;
using Microsoft.AspNetCore.Mvc;

namespace Enhanced.Models.BQData
{
    public class ParamBQ
    {
        [FromHeader]
        public string? Authorization { get; set; }
    }

    public class ParamBQQuery : ParameterBased
    {
        [FromBody]
        public DateTime start_date { get; set; }

        [FromBody]
        public bool? paginate { get; set; } = false;

        [FromBody]
        public int? max { get; set; }

        [FromBody]
        public int? offset { get; set; } = 0;
    }

    public class ParamPayment : ParameterBased
    {
        [FromBody]
        public string? payment_state { get; set; }

        [FromBody]
        public DateTime last_updated_from { get; set; }

        [FromBody]
        public bool? paginate { get; set; }

        [FromBody]
        public int? limit { get; set; } = 2000;

        [FromBody]
        public List<string>? ReportDocumentIds { get; set; } = new List<string>();
    }

    public class ParamCreateShipment
    {
        public List<Shipments>? shipments { get; set; }
    }

    public class Shipments
    {
        public string? invoice_reference { get; set; }
        public string? order_id { get; set; }
        public bool? shipped { get; set; } = true;
        public Tracking? tracking { get; set; }

        public List<ShipmentLine>? shipment_lines { get; set; }
    }

    public class Tracking
    {
        public string? carrier_code { get; set; }
        public string? carrier_name { get; set; }
        public string? tracking_number { get; set; }
        public string? tracking_url { get; set; }
    }

    public class ShipmentLine
    {
        public string? offer_sku { get; set; }
        public string? order_line_id { get; set; }
        public string? package_reference { get; set; }
        public int quantity { get; set; }
    }

    public class ShipmentResponse
    {
        public List<ShipmentError>? shipment_errors { get; set; }
        public List<ShipmentResult>? shipment_success { get; set; }
        public string? message { get; set; }
    }

    public class ShipmentError
    {
        public string? message { get; set; }
        public string? order_id { get; set; }
    }

    public class ShipmentResult
    {
        public string? id { get; set; }
        public string? order_id { get; set; }
    }
}
