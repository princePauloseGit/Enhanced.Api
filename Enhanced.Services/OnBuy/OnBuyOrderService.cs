using Enhanced.Models.OnBuy;
using Enhanced.Models.OnBuy.OnBuyViewModel;
using Enhanced.Models.Shared;
using System.Net;
using static Enhanced.Models.Shared.CommonEnum;

namespace Enhanced.Services.OnBuy
{
    public class OnBuyOrderService : OnBuyRequestService
    {
        public OnBuyOrderService(string authorization) : base(authorization) { }

        #region OnBuyRefund
        public virtual async Task<OnBuyRefundResponseViewModel> RefundOnBuyPayment(OnBuyRefundRequest onBuyRefundRequest)
        {
            var errorLogs = new List<ErrorLog>();
            var refundResponse = new OnBuyRefundResponseViewModel();

            try
            {
                CreateAuthorizedRequest(OrderApiUrls.Refund(), RestSharp.Method.PUT, postJsonBody: onBuyRefundRequest!);

                var (response, httpStatusCode) = await ExecuteRequestNoneAsync<OnBuyRefundResponse>();

                if (CheckHttpStatusCode(httpStatusCode))
                {
                    HandleSuccessResponse(refundResponse, response, onBuyRefundRequest.orders?.FirstOrDefault()?.order_id!, errorLogs, httpStatusCode);
                }
                else if (response.error != null)
                {
                    errorLogs.Add(new ErrorLog(Marketplace.OnBuy, Sevarity.Error, "OnBuy Refund", "", Priority.High, response.error.message, response.error.errorCode));
                    refundResponse.Orders = new OnBuyOrders
                    {
                        OrderId = onBuyRefundRequest?.orders?.FirstOrDefault()?.order_id! ?? string.Empty,
                        IsRefunded = false,
                        ErrorMessage = response?.error?.message ?? string.Empty
                    };
                }

                refundResponse.ErrorLogs = errorLogs;

                return refundResponse;
            }
            catch (Exception ex)
            {
                errorLogs.Add(new ErrorLog(Marketplace.OnBuy, Sevarity.Error, "OnBuy Refund", "", Priority.High, ex.Message, ex.StackTrace));

                return new OnBuyRefundResponseViewModel
                {
                    Orders = new OnBuyOrders
                    {
                        OrderId = onBuyRefundRequest?.orders?.FirstOrDefault()?.order_id! ?? string.Empty,
                        IsRefunded = false,
                        ErrorMessage = ex.Message
                    },
                    ErrorLogs = errorLogs,
                };
            }
        }

        private static void HandleSuccessResponse(OnBuyRefundResponseViewModel refundResponse, OnBuyRefundResponse response, string orderId, List<ErrorLog> errorLogs, HttpStatusCode httpStatusCode)
        {
            if (httpStatusCode == HttpStatusCode.NoContent)
            {
                refundResponse.Orders = new OnBuyOrders
                {
                    OrderId = orderId,
                    IsRefunded = true
                };

                if (!string.IsNullOrEmpty(orderId))
                {
                    errorLogs.Add(new ErrorLog(Marketplace.OnBuy, Sevarity.Information, "Refund Transaction Id", orderId, Priority.Low, "Refund has been processed.", ""));
                }
            }
            else
            {
                refundResponse.Orders = new OnBuyOrders();
                var result = response.results?.FirstOrDefault();

                if (result != null)
                {
                    if (result.success)
                    {
                        refundResponse.Orders.OrderId = orderId;
                        refundResponse.Orders.IsRefunded = true;

                        if (!string.IsNullOrEmpty(orderId))
                        {
                            errorLogs.Add(new ErrorLog(Marketplace.OnBuy, Sevarity.Information, "Refund Transaction Id", orderId, Priority.Low, "Refund has been processed.", ""));
                        }
                    }
                    else
                    {
                        refundResponse.Orders.OrderId = orderId;
                        refundResponse.Orders.IsRefunded = false;
                        refundResponse.Orders.ErrorMessage = result.message;

                        errorLogs.Add(new ErrorLog(Marketplace.OnBuy, Sevarity.Error, "OnBuy Refund", "", Priority.High, result.message, result.errorcode));
                    }
                }
                else
                {
                    errorLogs.Add(new ErrorLog(Marketplace.OnBuy, Sevarity.Error, "OnBuy Refund", "", Priority.High, "API indicates failure", ""));
                }
            }
        }

        #endregion
    }
}
