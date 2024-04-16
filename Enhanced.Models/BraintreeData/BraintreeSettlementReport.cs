using Enhanced.Models.AmazonData;
using Enhanced.Models.Shared;

namespace Enhanced.Models.BraintreeData
{
    public class BraintreeSettlementReport
    {
        public List<BraintreeSettlementBatch>? Settlements { get; set; } = new List<BraintreeSettlementBatch>();
        public List<ReportDocumentDetails>? ReportDocumentDetails { get; set; } = new List<ReportDocumentDetails>();
        public List<ErrorLog>? ErrorLogs { get; set; } = new List<ErrorLog>();
    }

    public class BraintreeSettlementBatch
    {
        public string? SettlementId { get; set; }
        public List<SettlementGroup>? BraintreeSettlements { get; set; }
    }
}
