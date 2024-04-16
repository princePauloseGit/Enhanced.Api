using Enhanced.Models.PayPalData;
using static Enhanced.Models.PayPalData.PayPalSettlementReport;

namespace Enhanced.Services.PayPal
{
    public interface IPayPalReportManager
    {
        PayPalSettlementBatch GetPayPalSettlementReports(ParameterPayPalSFTP payPalSFTP, List<string> bcReportDocumentIds);
    }
}
