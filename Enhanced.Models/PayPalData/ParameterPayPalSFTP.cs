using Microsoft.AspNetCore.Mvc;

namespace Enhanced.Models.PayPalData
{
    public class ParameterPayPalSFTP
    {
        [FromHeader]
        public string? SFTPHost { get; set; }

        [FromHeader]
        public int? SFTPPort { get; set; }

        [FromHeader]
        public string? SFTPUser { get; set; }

        [FromHeader]
        public string? SFTPPassword { get; set; }

        [FromHeader]
        public string? SFTPDestinationPath { get; set; }
    }

    public class PayPalReportDocumentParam
    {
        public List<string>? ReportDocumentIds { get; set; } = new List<string>();
    }
}
