using Enhanced.Models.AmazonData;
using Enhanced.Models.EbayData;
using Enhanced.Models.EbayData.EbayViewModel;
using Enhanced.Models.Shared;
using static Enhanced.Models.EbayData.EbayOrderList;
using static Enhanced.Models.Shared.CommonEnum;

namespace Enhanced.Services.Ebay
{
    public class EbayFulfilmentService : EbayRequestService
    {
        public EbayFulfilmentService(ClientToken oauth, AccessToken token) : base(oauth, token) { }

        public async Task<(List<OrderList>, List<ErrorLog>, string)> GetOrders(EbayFilter ebayFilter)
        {
            var errorLogs = new List<ErrorLog>();
            var orders = new List<OrderList>();
            string nextPage = string.Empty;

            try
            {
                DateTime fromDate = DateTime.UtcNow.Date.AddDays(-1 * (int)ebayFilter.Days!);
                int totalDays = (int)(DateTime.UtcNow.Date - fromDate).TotalDays;

                if (totalDays > 30)
                {
                    fromDate = DateTime.UtcNow.AddDays(-30);
                }

                string dates = string.Concat("Date From: ", fromDate.ToString(Constant.DATETIME_DDMMYYY_HHMMSS));

                errorLogs.Add(new ErrorLog(Marketplace.EBay, Sevarity.Information, "Order Downloads", "", Priority.Low, dates));

                string orderFilter = string.Concat(
                    "?filter=creationdate:%5B",
                    fromDate.ToString(Constant.DATE_AGING_FORMAT),
                    "T00:00:01.000Z..%5D",
                    ",orderfulfillmentstatus:%7BNOT_STARTED%7CIN_PROGRESS%7D",
                    "&limit=",
                    ebayFilter.Limit
                    );

                if (!string.IsNullOrEmpty(ebayFilter.NextPage))
                {
                    nextPage = CommonHelper.Base64Decode(ebayFilter.NextPage);
                }

                if (ebayFilter.ManualTest == true && !string.IsNullOrEmpty(nextPage))
                {
                    await CreateAuthorizedPagedRequestAsync(nextPage, string.Empty, RestSharp.Method.GET);
                }
                else
                {
                    await CreateAuthorizedPagedRequestAsync(nextPage, FulfillmentApiUrls.Order + orderFilter, RestSharp.Method.GET);
                }

                var response = await ExecuteRequestAsync<OrdersResponse>();
                nextPage = response?.next!;

                AddOrders(errorLogs, orders, response!);

                if (ebayFilter.ManualTest == true)
                {
                    nextPage = CommonHelper.Base64Encode(nextPage);
                    return (orders, errorLogs, nextPage);
                }

                while (!string.IsNullOrEmpty(nextPage))
                {
                    await CreateAuthorizedPagedRequestAsync(nextPage, string.Empty, RestSharp.Method.GET);

                    var nextTokenResponse = await ExecuteRequestAsync<OrdersResponse>();
                    nextPage = nextTokenResponse?.next!;

                    AddOrders(errorLogs, orders, response!);
                }
            }
            catch (Exception ex)
            {
                errorLogs.Add(new ErrorLog(Marketplace.EBay, Sevarity.Error, "Orders", "", Priority.Low, ex.Message, ex.StackTrace));
            }

            return (orders, errorLogs, string.Empty);
        }

        private static void AddOrders(List<ErrorLog> errorLogs, List<OrderList> orders, OrdersResponse response)
        {
            if (response?.orders?.Any() == true)
            {
                var emptyAddressOrders = new List<OrderList>();

                var orderResponse = response.orders?.Select(s => new OrderList(s)).ToList()!;

                foreach (var noAddress in orderResponse.Where(x => !x.HasAddress))
                {
                    errorLogs.Add(new ErrorLog(Marketplace.BQ, Sevarity.Warning, "eBay Orders", noAddress.OrderId, Priority.High, stackTrace: "The address is empty - will be looking in next iteration"));
                    emptyAddressOrders.Add(noAddress);
                }

                foreach (var order in emptyAddressOrders)
                {
                    orderResponse.Remove(order);
                }

                errorLogs.Add(new ErrorLog(Marketplace.EBay, Sevarity.Information, "Orders", "", Priority.Low, "Total Orders: " + orderResponse?.Count));

                foreach (var order in orderResponse!)
                {
                    errorLogs.Add(new ErrorLog(Marketplace.EBay, Sevarity.Information, "Order Id", order.OrderId, Priority.Low, "Total Items: " + order?.OrderLines?.Count));
                }

                orders.AddRange(orderResponse);
            }
            else
            {
                errorLogs.Add(new ErrorLog(Marketplace.EBay, Sevarity.Warning, "Orders", "", Priority.Low, "No orders found"));
            }
        }

        public async Task<(List<ConfirmShipment>, List<ErrorLog>)> CreateShippingFulfilment(EbayShipmentParameter ebayShipmentParameter)
        {
            var errorLogs = new List<ErrorLog>();
            var confirmShipments = new List<ConfirmShipment>();

            foreach (var shipment in ebayShipmentParameter.CreateShipments!)
            {
                try
                {
                    await CreateAuthorizedRequestAsync(FulfillmentApiUrls.CreateShippingApiUrl(shipment.OrderId!), RestSharp.Method.GET);

                    var shippingFulfilment = await ExecuteRequestAsync<ShippingFilfilmentResponse>();

                    if (shippingFulfilment?.errors?.Any() == true)
                    {
                        confirmShipments.Add(new ConfirmShipment
                        {
                            OrderId = shipment.OrderId!,
                            IsConfirmedShipment = false,
                        });

                        var errorMessages = shippingFulfilment?.errors?.Select(s => s.message?.Replace("\"", "")).ToList();
                        errorLogs.Add(new ErrorLog(Marketplace.EBay, Sevarity.Error, "Order Id", shipment.OrderId!, Priority.High, "Failed to confirm shipment", string.Join(",", errorMessages!).Replace("\"", "")));

                        return (confirmShipments, errorLogs);
                    }

                    if (shippingFulfilment?.fulfillments?.Any() == false)
                    {
                        await CreateAuthorizedRequestAsync(FulfillmentApiUrls.CreateShippingApiUrl(shipment.OrderId!), RestSharp.Method.POST, shipment.ShipmentData!);

                        var (isSuccess, errorResponse) = await ExecuteRequestNoneAsync();

                        confirmShipments.Add(new ConfirmShipment
                        {
                            OrderId = shipment.OrderId!,
                            IsConfirmedShipment = isSuccess,
                        });

                        if (isSuccess)
                        {
                            errorLogs.Add(new ErrorLog(Marketplace.EBay, Sevarity.Information, "Order Id", shipment.OrderId!, Priority.Low, "The fulfillment has been marked as shipped"));
                        }
                        else
                        {
                            var errorMessages = errorResponse?.errors?.Select(s => s.message?.Replace("\"", "")).ToList();
                            errorLogs.Add(new ErrorLog(Marketplace.EBay, Sevarity.Error, "Order Id", shipment.OrderId!, Priority.High, "Failed to confirm shipment", string.Join(",", errorMessages!).Replace("\"", "")));
                        }
                    }
                    else
                    {
                        confirmShipments.Add(new ConfirmShipment
                        {
                            OrderId = shipment.OrderId!,
                            IsConfirmedShipment = true,
                        });

                        errorLogs.Add(new ErrorLog(Marketplace.EBay, Sevarity.Warning, "Order Id", shipment.OrderId!, Priority.Low, "The fulfillment has been already marked as shipped"));
                    }
                }
                catch (Exception ex)
                {
                    confirmShipments.Add(new ConfirmShipment
                    {
                        OrderId = shipment.OrderId!,
                        IsConfirmedShipment = false,
                    });

                    errorLogs.Add(new ErrorLog(Marketplace.EBay, Sevarity.Error, "Order Id", shipment.OrderId!, Priority.High, ex.Message, ex.StackTrace));
                    continue;
                }
            }

            return (confirmShipments, errorLogs);
        }
    }
}
