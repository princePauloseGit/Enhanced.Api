using Enhanced.Models.Shared;

namespace Enhanced.Models.EbayData
{
    public class EbaySettlementReport
    {
        public List<EbaySettlementBatch>? Settlements { get; set; } = new List<EbaySettlementBatch>();
        public List<ReportDocumentDetails>? ReportDocumentDetails { get; set; } = new List<ReportDocumentDetails>();
        public List<ErrorLog>? ErrorLogs { get; set; } = new List<ErrorLog>();
    }

    public class EbaySettlementBatch
    {
        public string? SettlementId { get; set; }
        public List<SettlementGroup>? Settlements { get; set; }
    }
}
