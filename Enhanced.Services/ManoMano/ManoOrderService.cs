using Enhanced.Models.EbayData;
using Enhanced.Models.ManoMano;
using Enhanced.Models.ManoMano.ManoViewModel;
using Enhanced.Models.Shared;
using static Enhanced.Models.Shared.CommonEnum;

namespace Enhanced.Services.ManoMano
{
    public class ManoOrderService : ManoRequestService
    {
        public ManoOrderService(string apiKey) : base(apiKey) { }

        #region GetOrders

        public virtual async Task<ManoOrderViewModel> GetOrders(OrderRequestParam orderRequestParam)
        {
            var errorLogs = new List<ErrorLog>();
            var allOrders = new List<Orders>(); // Store all orders in this list

            try
            {
                var page = 1;
                var totalPages = 1; // Default to 1, will be updated from the response later

                do
                {
                    // Build the API request with the current page
                    orderRequestParam.page = page;
                    var queryParameters = orderRequestParam.GetParameters();

                    CreateAuthorizedRequest(OrderApiUrls.GetOrders(), RestSharp.Method.GET, queryParameters);

                    var response = await ExecuteRequestAsync<ManoOrderResponse>();

                    if (response.message != null)
                    {
                        errorLogs.Add(new ErrorLog(Marketplace.ManoMano, Sevarity.Error, "ManoMano Orders", "", Priority.High, "Please check API key or Seller Contract ID", response.message));

                        return new ManoOrderViewModel
                        {
                            Orders = allOrders,
                            ErrorLogs = errorLogs,
                        };
                    }

                    // Get orders from the current response and add them to the allOrders list
                    var orders = response.content?.Select(s => new Orders(s)).ToList();

                    if (orders != null && orders?.Any() == true)
                    {
                        allOrders.AddRange(orders);
                    }

                    totalPages = response.pagination?.pages ?? 1;

                    // Update the page number for the next iteration
                    page++;

                } while (page <= totalPages); // Continue loop until all pages are fetched

                errorLogs.Add(new ErrorLog(Marketplace.ManoMano, Sevarity.Information, "ManoMano Orders", "", Priority.Low, "Total Orders: " + allOrders.Count));

                foreach (var order in allOrders)
                {
                    errorLogs.Add(new ErrorLog(Marketplace.ManoMano, Sevarity.Information, "Order Id", order.OrderReference, Priority.Low, "Total Items: " + order.OrderLines?.Count));
                }

                return new ManoOrderViewModel
                {
                    Orders = allOrders,
                    ErrorLogs = errorLogs,
                };
            }
            catch (Exception ex)
            {
                errorLogs.Add(new ErrorLog(Marketplace.ManoMano, Sevarity.Error, "ManoMano Orders", "", Priority.High, ex.Message, ex.StackTrace));

                return new ManoOrderViewModel
                {
                    Orders = new List<Orders>(),
                    ErrorLogs = errorLogs,
                };
            }
        }

        #endregion

        #region Accept Orders
        public virtual async Task<AcceptOrderResponseViewModel> AcceptOrders(AcceptOrderRequest acceptOrders)
        {
            var acceptOrderRequests = new List<AcceptOrderRequest>();
            var acceptOrderResponse = new AcceptOrderResponseViewModel();

            CreateAcceptOrderBatchOf50Items(acceptOrders.acceptOrders!, acceptOrderRequests);

            foreach (var acceptOrderRequest in acceptOrderRequests)
            {
                var errorLogs = new List<ErrorLog>();
                var orders = new List<AcceptResponse>();

                try
                {
                    CreateAuthorizedRequest(OrderApiUrls.AcceptOrders(), RestSharp.Method.POST, postJsonBody: acceptOrderRequest.acceptOrders!);

                    var (response, httpStatusCode) = await ExecuteRequestNoneAsync<AcceptOrderResponse>();

                    if (CheckHttpStatusCode(httpStatusCode))
                    {
                        if (acceptOrderRequest.acceptOrders != null)
                        {
                            foreach (var order in acceptOrderRequest.acceptOrders!)
                            {
                                orders.Add(new AcceptResponse
                                {
                                    OrderID = order.order_reference,
                                    HttpStatusCode = (int)httpStatusCode,
                                    ErrorMessage = string.Empty
                                });

                                errorLogs.Add(new ErrorLog(Marketplace.ManoMano, Sevarity.Information, "ManoMano Accept Orders", order.order_reference, Priority.Low, "Request succeeded with no content"));
                            }
                        }
                    }
                    else if (response.content != null && response.content?.Any() == true)
                    {
                        orders = response.content?.Select(s => new AcceptResponse(s)).ToList()!;

                        foreach (var order in orders)
                        {
                            errorLogs.Add(new ErrorLog(Marketplace.ManoMano, Sevarity.Information, "Order Id", order.OrderID, Priority.Low, order.ErrorMessage));
                        }
                    }
                    else if (response.Error != null)
                    {
                        errorLogs.Add(new ErrorLog(Marketplace.ManoMano, Sevarity.Error, "ManoMano Accept Orders", "", Priority.High, response.Error.message, response.Error.app_code));
                    }
                    else if (response.message != null)
                    {
                        errorLogs.Add(new ErrorLog(Marketplace.ManoMano, Sevarity.Error, "ManoMano Accept Orders", "", Priority.High, "", response.message));
                    }

                    if (orders?.Any() == true)
                    {
                        acceptOrderResponse.AcceptOrders?.AddRange(orders);
                    }

                    acceptOrderResponse.ErrorLogs?.AddRange(errorLogs);
                }
                catch (Exception ex)
                {
                    errorLogs.Add(new ErrorLog(Marketplace.ManoMano, Sevarity.Error, "ManoMano Accept Orders", "", Priority.High, ex.Message, ex.StackTrace));
                    acceptOrderResponse.ErrorLogs?.AddRange(errorLogs);

                    continue;
                }
            }

            return acceptOrderResponse;
        }

        private static void CreateAcceptOrderBatchOf50Items(List<AcceptOrders> acceptOrders, List<AcceptOrderRequest> acceptOrderRequests)
        {
            int total = 0;
            int batchSize = 50;

            while (total < acceptOrders?.Count)
            {
                var acceptOrdersBatch = acceptOrders?.Skip(total).Take(batchSize).ToList();

                acceptOrderRequests?.Add(new AcceptOrderRequest
                {
                    acceptOrders = acceptOrdersBatch,
                });

                total += batchSize;
            }
        }
        #endregion

        #region CreateShipment

        public virtual async Task<ManoShipmentResponseViewModel> CreateShipment(CreateShipmentRequest shipmentRequest)
        {
            var createShipmentRequests = new List<CreateShipmentRequest>();
            var shipmentResponse = new ManoShipmentResponseViewModel();

            CreateShipmentOrderBatchOf50Items(shipmentRequest.shipmentOrders!, createShipmentRequests);

            foreach (var createShipmentRequest in createShipmentRequests)
            {
                var errorLogs = new List<ErrorLog>();
                var orders = new List<ShippedOrders>();

                try
                {
                    CreateAuthorizedRequest(OrderApiUrls.CreateShipment(), RestSharp.Method.POST, postJsonBody: createShipmentRequest.shipmentOrders!);

                    var (response, httpStatusCode) = await ExecuteRequestNoneAsync<ManoShipmentResponse>();

                    if (CheckHttpStatusCode(httpStatusCode))
                    {
                        if (createShipmentRequest.shipmentOrders != null)
                        {
                            foreach (var order in createShipmentRequest.shipmentOrders)
                            {
                                orders.Add(new ShippedOrders
                                {
                                    OrderID = order.order_reference,
                                    HttpStatusCode = (int)httpStatusCode,
                                    ErrorMessage = string.Empty
                                });

                                errorLogs.Add(new ErrorLog(Marketplace.ManoMano, Sevarity.Information, "ManoMano Create Shipment", order.order_reference, Priority.Low));
                            }
                        }
                    }
                    else if (response.content != null && response.content.Any())
                    {
                        orders = response.content?.Select(s => new ShippedOrders(s)).ToList()!;

                        if (orders != null && orders?.Any() == true)
                        {
                            foreach (var order in orders)
                            {
                                errorLogs.Add(new ErrorLog(Marketplace.ManoMano, Sevarity.Information, "Order Id", order.OrderID, Priority.Low, response.Error!.message));
                            }
                        }
                    }
                    else if (response.Error != null)
                    {
                        errorLogs.Add(new ErrorLog(Marketplace.ManoMano, Sevarity.Error, "ManoMano Create Shipment", "", Priority.High, response.Error.message, "Failed to create shipment, please check error log for more details"));
                    }
                    else if (response.message != null)
                    {
                        errorLogs.Add(new ErrorLog(Marketplace.ManoMano, Sevarity.Error, "ManoMano Create Shipment", "", Priority.High, "", response.message));
                    }

                    if (orders?.Any() == true)
                    {
                        shipmentResponse.ShippedOrders?.AddRange(orders);
                    }

                    shipmentResponse.ErrorLogs?.AddRange(errorLogs);
                }
                catch (Exception ex)
                {
                    errorLogs.Add(new ErrorLog(Marketplace.ManoMano, Sevarity.Error, "ManoMano Create Shipment", "", Priority.High, ex.Message, ex.StackTrace));
                    shipmentResponse.ErrorLogs?.AddRange(errorLogs);

                    continue;
                }
            }

            return shipmentResponse;
        }

        private static void CreateShipmentOrderBatchOf50Items(List<Shipment> shipments, List<CreateShipmentRequest> createShipmentRequests)
        {
            int total = 0;
            int batchSize = 50;

            while (total < shipments?.Count)
            {
                var shipmentOrdersBatch = shipments?.Skip(total).Take(batchSize).ToList();

                createShipmentRequests?.Add(new CreateShipmentRequest
                {
                    shipmentOrders = shipmentOrdersBatch,
                });

                total += batchSize;
            }
        }
        #endregion
    }
}
