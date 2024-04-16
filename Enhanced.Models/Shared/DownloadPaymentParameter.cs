namespace Enhanced.Models.Shared
{
    public class DownloadPaymentParameter
    {
        public List<string>? ReportDocumentIds { get; set; } = new List<string>();
        public int Days { get; set; } = 15;
    }
}
