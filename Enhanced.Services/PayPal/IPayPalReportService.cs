using Enhanced.Models.PayPalData;
using Enhanced.Models.Shared;

namespace Enhanced.Services.PayPal
{
    public interface IPayPalReportService
    {
        (List<string>, List<ErrorLog>) DownloadSFTPFiles(ParameterPayPalSFTP payPalSFTP, List<string> bcReportDocumentIds);
    }
}
