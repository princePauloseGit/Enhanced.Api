namespace Enhanced.Models.Shared
{
    public class SettlementGroup
    {
        public DateTime? PostingDate { get; set; }
        public string? AccountType { get; set; }
        public string? DocumentType { get; set; }
        public string? DocumentNumber { get; set; }
        public string? ExternalDocumentNumber { get; set; }
        public decimal? Amount { get; set; }
        public string? ShortcutDimension { get; set; }
        public string? PaymentLine { get; set; }
    }

    public class ReportDocumentDetails
    {
        public string? ReportDocumentId { get; set; }
        public bool? CanArchived { get; set; }
    }
}
