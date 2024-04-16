using Enhanced.Models.AmazonData;
using Enhanced.Models.Shared;

namespace Enhanced.Services.AmazonServices
{
    public interface IAmazonReportManager
    {
        Task<SettlementBatch> GetSettlementOrder(AmazonCredential amazonCredential, DownloadPaymentParameter amazonPaymentParameter);
        Task<SettlementBatchForCredit> GetSettlementOrderForCredits(AmazonCredential amazonCredential, DownloadPaymentParameter amazonPaymentParameter);
    }
}
