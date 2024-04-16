using Enhanced.Models.BraintreeData;
using Enhanced.Models.Shared;

namespace Enhanced.Services.BraintreeService
{
    public interface IBraintreeReportManager
    {
        Task<(bool, ErrorLog)> RefundPayment(ParameterBraintree parameterBraintree, ParameterRefundBraintree parameterRefund);
        Task<BraintreeSettlementReport> SearchTransaction(ParameterBraintree parameterBraintree, DownloadPaymentParameter downloadPaymentParameter);
    }
}
