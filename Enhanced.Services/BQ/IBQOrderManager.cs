using Enhanced.Models.BQData;

namespace Enhanced.Services.BQ
{
    public interface IBQOrderManager
    {
        Task<BQPaymentBatch> DownloadBQPayment(ParamBQ parameter, ParamPayment paramPaymentBQ);
    }
}
