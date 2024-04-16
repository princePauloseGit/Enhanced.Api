using Enhanced.Models.AmazonData;
using Enhanced.Models.BQData;
using Enhanced.Models.BQData.BQDataViewModel;
using Enhanced.Models.Shared;
using Newtonsoft.Json;
using static Enhanced.Models.BQData.BQEnum;
using static Enhanced.Models.BQData.BQPayment;
using static Enhanced.Models.Shared.CommonEnum;

namespace Enhanced.Services.BQ
{
    public class BQService : BQRequestService, IBQService
    {
        private readonly string page_token = "page_token";

        /// <summary>
        /// Get B & Q Orders
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="paramQueryBQ"></param>
        /// <returns></returns>
        public async Task<BQOrderListViewModel> GetBQOrders(ParamBQ parameter, ParamBQQuery paramQueryBQ)
        {
            var errorLogs = new List<ErrorLog>();

            try
            {
                var emptyAddressOrders = new List<OrderList>();
                var queryParameters = paramQueryBQ.GetParameters();
                errorLogs.Add(new ErrorLog(Marketplace.BQ, Sevarity.Information, "B&Q Parameters", "", Priority.Low, stackTrace: JsonConvert.SerializeObject(queryParameters)));

                CreateAuthorizedRequestAsync(BQOrders, RestSharp.Method.GET, queryParameters);

                var bqOrders = await ExecuteRequestAsync<BQOrderList>(parameter.Authorization!, RateLimitType.BQ_GetOrders);

                var orders = bqOrders.orders?.Select(s => new OrderList(s)).ToList()!;

                if (orders.Any(a => string.IsNullOrEmpty(a.Customer?.BillingAddress?.Street1) || string.IsNullOrEmpty(a.Customer?.ShippingAddress?.Street1)))
                {
                    var noAddresses = orders.Where(a => string.IsNullOrEmpty(a.Customer?.BillingAddress?.Street1) || string.IsNullOrEmpty(a.Customer?.ShippingAddress?.Street1));

                    foreach (var noAddress in noAddresses)
                    {
                        errorLogs.Add(new ErrorLog(Marketplace.BQ, Sevarity.Warning, "B&Q Orders", noAddress.OrderId, Priority.High, stackTrace: "The address is empty - will be looking in next iteration"));
                        emptyAddressOrders.Add(noAddress);
                    }
                }

                foreach (var order in emptyAddressOrders)
                {
                    orders.Remove(order);
                }

                errorLogs.Add(new ErrorLog(Marketplace.BQ, Sevarity.Information, "B&Q Orders", "", Priority.Low, "Total Orders: " + orders?.Count));

                foreach (var order in orders!)
                {
                    errorLogs.Add(new ErrorLog(Marketplace.BQ, Sevarity.Information, "Order Id", order.OrderId, Priority.Low, "Total Items: " + order?.OrderLines?.Count));
                }

                return new BQOrderListViewModel
                {
                    NextPage = orders.Count == paramQueryBQ?.max ? paramQueryBQ?.max + paramQueryBQ?.offset : 0,
                    Orders = orders,
                    ErrorLogs = errorLogs,
                };
            }
            catch (Exception ex)
            {
                errorLogs.Add(new ErrorLog(Marketplace.BQ, Sevarity.Error, "B&Q Orders", "", Priority.High, ex.Message, ex.StackTrace));

                return new BQOrderListViewModel
                {
                    Orders = new List<OrderList>(),
                    ErrorLogs = errorLogs,
                };
            }
        }

        /// <summary>
        /// Update Shipment Status
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="createShipment"></param>
        /// <returns></returns>
        public async Task<(List<ConfirmShipment>, List<ErrorLog>)> CreateShipment(ParamBQ parameter, ParamCreateShipment createShipment)
        {
            var errorLogs = new List<ErrorLog>();
            var confirmShipments = new List<ConfirmShipment>();
            var shipmentBatches = new List<List<Shipments>>();
            var recordsToShip = new List<Shipments>();
            var recordsWithErrors = new List<Shipments>();

            if (createShipment?.shipments?.Any() == true)
            {
                foreach (var shipItem in createShipment?.shipments!)
                {
                    if (string.IsNullOrEmpty(shipItem?.order_id) || shipItem?.shipment_lines?.Any(an => string.IsNullOrEmpty(an?.order_line_id)) == true)
                    {
                        confirmShipments.Add(new ConfirmShipment
                        {
                            OrderId = shipItem!.order_id!,
                            IsConfirmedShipment = true
                        });

                        errorLogs.Add(new ErrorLog(Marketplace.BQ, Sevarity.Warning, "Invoice Id", shipItem?.invoice_reference, Priority.Low, stackTrace: "Shipment does not have Order Id/Order Line Id, marked as shipped considering a manual order"));
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(shipItem?.tracking?.tracking_number))
                        {
                            shipItem!.tracking!.tracking_number = "N/A";
                        }

                        recordsToShip.Add(shipItem!);
                    }
                }

                CreateBatchOf1000Items(recordsToShip!, shipmentBatches);

                foreach (var shipments in shipmentBatches!)
                {
                    try
                    {
                        var requestShipment = new ParamCreateShipment
                        {
                            shipments = shipments,
                        };

                        errorLogs.Add(new ErrorLog(Marketplace.BQ, Sevarity.Information, "B&Q Shipment", "", Priority.Low, stackTrace: "Total Shipment Orders: " + shipments?.Count));

                        CreateAuthorizedRequestAsync(BQShipment, RestSharp.Method.POST, postJsonObj: requestShipment!);

                        var response = await ExecuteRequestAsync<ShipmentResponse>(parameter.Authorization!, RateLimitType.BQ_Shipment);

                        if (response?.shipment_success != null && response?.shipment_success?.Any() == true)
                        {
                            foreach (var shipment in response?.shipment_success!)
                            {
                                confirmShipments.Add(new ConfirmShipment
                                {
                                    OrderId = shipment.order_id!,
                                    IsConfirmedShipment = true
                                });

                                errorLogs.Add(new ErrorLog(Marketplace.BQ, Sevarity.Information, "Order Id", shipment.order_id, Priority.Low));
                            }
                        }

                        if (response?.shipment_errors != null && response?.shipment_errors?.Any() == true)
                        {
                            foreach (var shipment in response?.shipment_errors!)
                            {
                                if (shipment?.message?.StartsWith("The order status must be 'SHIPPING'") == true)
                                {
                                    confirmShipments.Add(new ConfirmShipment
                                    {
                                        OrderId = shipment.order_id!,
                                        IsConfirmedShipment = true
                                    });
                                }
                                else
                                {
                                    errorLogs.Add(new ErrorLog(Marketplace.BQ, Sevarity.Error, "Order Id", shipment?.order_id, Priority.High, stackTrace: shipment?.message));
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(response?.message))
                        {
                            errorLogs.Add(new ErrorLog(Marketplace.BQ, Sevarity.Error, "Create Shipment", "", Priority.High, stackTrace: response?.message));
                        }
                    }
                    catch (Exception ex)
                    {
                        errorLogs.Add(new ErrorLog(Marketplace.BQ, Sevarity.Error, "Create Shipment", "", Priority.High, ex.Message, ex.StackTrace));
                        continue;
                    }
                }
            }
            else
            {
                errorLogs.Add(new ErrorLog(Marketplace.BQ, Sevarity.Warning, "Create Shipment", "", Priority.Low, stackTrace: "No shipments sent to API"));

                return new(confirmShipments, errorLogs);
            }

            return (confirmShipments, errorLogs);
        }

        private static void CreateBatchOf1000Items(List<Shipments> lstShipments, List<List<Shipments>> shipmentBatches)
        {
            int total = 0;
            int batchSize = 1000;

            while (total < lstShipments?.Count)
            {
                var asinBatch = lstShipments?.Skip(total).Take(batchSize).ToList();

                shipmentBatches.Add(asinBatch!);

                total += batchSize;
            }
        }

        /// <summary>
        /// Download B&Q Payment
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="paramPaymentBQ"></param>
        /// <returns></returns>
        public async Task<(List<PaymentList>, ErrorLog)> DownloadBQPayment(ParamBQ parameter, ParamPayment paramPaymentBQ)
        {
            List<PaymentList> payments = new();

            try
            {
                var queryParameters = paramPaymentBQ.GetParameters();
                var errorLog = new ErrorLog(Marketplace.BQ, Sevarity.Information, "B&Q Parameters", "", Priority.Low, stackTrace: JsonConvert.SerializeObject(queryParameters));

                CreateAuthorizedRequestAsync(BQPayment, RestSharp.Method.GET, queryParameters);

                var bqPayment = await ExecuteRequestAsync<BQPayment>(parameter.Authorization!, RateLimitType.BQ_GetPayments);
                string token = bqPayment?.next_page_token!;

                if (bqPayment!.data != null)
                {
                    payments.AddRange(bqPayment.data);
                }

                while (!string.IsNullOrEmpty(token))
                {
                    var (nextPayments, nextToken) = await GetPaymentByNextToken(parameter.Authorization!, token!);

                    payments.AddRange(nextPayments);

                    token = nextToken;
                }

                return (payments, errorLog);
            }
            catch (Exception ex)
            {
                var errorLog = new ErrorLog(Marketplace.BQ, Sevarity.Error, "B&Q Payment", "", Priority.Low, ex.Message, ex.StackTrace);

                return (payments, errorLog);
            }
        }

        public async Task<(List<PaymentList>, string)> GetPaymentByNextToken(string authorization, string nextToken)
        {
            var queryParameters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(page_token, nextToken)
            };

            CreateAuthorizedRequestAsync(BQPayment, RestSharp.Method.GET, queryParameters);

            var bqPayment = await ExecuteRequestAsync<BQPayment>(authorization!, RateLimitType.BQ_GetPayments);

            return (bqPayment?.data!, bqPayment?.next_page_token!);
        }

        public async Task<(bool, ErrorLog)> RefundPayment(string authorization, string orderId, BQRefund refund)
        {
            try
            {
                CreateAuthorizedRequestAsync(BQRefund, RestSharp.Method.PUT, postJsonObj: refund);

                var bqRefundResponse = await ExecuteRequestAsync<BQRefundResponse>(authorization, RateLimitType.BQ_Refund);

                if (bqRefundResponse?.refunds?.Any() == true)
                {
                    return (true, new ErrorLog(Marketplace.BQ, Sevarity.Information, "Order Id", orderId, Priority.Low));
                }

                return (false, new ErrorLog(Marketplace.BQ, Sevarity.Error, "Order Id", orderId, Priority.High, "Failed to refund payment", stackTrace: bqRefundResponse?.message));
            }
            catch (Exception ex)
            {
                return (false, new ErrorLog(Marketplace.BQ, Sevarity.Error, "Order Id", orderId, Priority.High, ex.Message, ex.StackTrace));
            }
        }
    }
}
