using Enhanced.Models.AmazonData;
using Enhanced.Models.Shared;

namespace Enhanced.Models.BQData
{
    public class BQPaymentBatch
    {
        public List<BQPaymentGroup>? Payments { get; set; } = new List<BQPaymentGroup>();
        public List<ReportDocumentDetails>? ReportDocumentDetails { get; set; } = new List<ReportDocumentDetails>();
        public List<ErrorLog>? ErrorLogs { get; set; } = new List<ErrorLog>();
    }

    public class BQPaymentGroup
    {
        public string? AccountingDocumentNumber { get; set; }
        public List<BQPaymentRow>? BQPayments { get; set; }
    }

    public class BQPaymentRow
    {
        public DateTime? PostingDate { get; set; }
        public string? ExternalDocumentNumber { get; set; }
        public string? AccountType { get; set; }
        public string? AccountNo { get; set; }
        public decimal? Amount { get; set; }
        public string? BalAccountNo { get; set; }
        public string? BalAccountType { get; set; }
        public decimal? BalVATAmount { get; set; }
    }
}
