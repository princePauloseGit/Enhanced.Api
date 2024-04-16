using Enhanced.Models.AmazonData;
using Enhanced.Models.BQData;
using Enhanced.Models.BQData.BQDataViewModel;
using Enhanced.Models.Shared;
using static Enhanced.Models.BQData.BQPayment;

namespace Enhanced.Services.BQ
{
    public interface IBQService
    {
        Task<BQOrderListViewModel> GetBQOrders(ParamBQ parameter, ParamBQQuery paramQueryBQ);
        Task<(List<PaymentList>, ErrorLog)> DownloadBQPayment(ParamBQ parameter, ParamPayment paramPaymentBQ);
        Task<(List<ConfirmShipment>, List<ErrorLog>)> CreateShipment(ParamBQ parameter, ParamCreateShipment createShipment);
        Task<(bool, ErrorLog)> RefundPayment(string authorization, string orderId, BQRefund refund);
    }
}
