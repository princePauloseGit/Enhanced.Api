using Enhanced.Models.AmazonData;
using Enhanced.Models.Shared;
using Newtonsoft.Json;
using RestSharp;
using static Enhanced.Models.Shared.CommonEnum;

namespace Enhanced.Services.AmazonServices
{
    public class AmazonOrderService : AmazonRequestService
    {
        public AmazonOrderService(AmazonCredential amazonCredential) : base(amazonCredential) { }

        public async Task<(List<Order>, string?, List<ErrorLog>?)> GetOrders(ParameterOrderList parameter)
        {
            var errorLogs = new List<ErrorLog>();
            string nextToken = string.Empty;

            try
            {
                var queryParameters = parameter.GetParameters();
                errorLogs.Add(new ErrorLog(Marketplace.Amazon, Sevarity.Information, "Amazon Parameters", "", Priority.Low, stackTrace: JsonConvert.SerializeObject(queryParameters)));
                CreateRestrictedDataTokenRequest? restrictedDataTokenRequest = GetRestrictedToken(parameter.IsNeedRestrictedDataToken);

                await CreateAuthorizedRequestAsync(OrdersApiUrls.Orders, Method.GET, queryParameters, tokenDataType: AmazonEnum.TokenDataType.PII, createRestrictedDataTokenRequest: restrictedDataTokenRequest!);

                var response = await ExecuteRequestAsync<GetOrderResponse>(AmazonEnum.RateLimitType.Order_GetOrders);
                nextToken = response?.Payload?.NextToken!;

                errorLogs.Add(new ErrorLog(Marketplace.Amazon, Sevarity.Information, "Orders", "", Priority.Low, "Total Orders: " + response?.Payload?.Orders?.Count));

                return (response?.Payload!.Orders!, nextToken, errorLogs);
            }
            catch (Exception ex)
            {
                errorLogs.Add(new ErrorLog(Marketplace.Amazon, Sevarity.Error, "Orders", "", Priority.High, ex.Message, ex.StackTrace));

                return (new List<Order>(), nextToken, errorLogs);
            }
        }

        private static CreateRestrictedDataTokenRequest? GetRestrictedToken(bool isNeedRestrictedDataToken)
        {
            return isNeedRestrictedDataToken ? new CreateRestrictedDataTokenRequest
            {
                restrictedResources = new List<RestrictedResource>
                {
                    new RestrictedResource
                    {
                        method = Method.GET.ToString(),
                        path = OrdersApiUrls.Orders,
                        dataElements = new List<string> { "buyerInfo", "shippingAddress" }
                    }
                }
            }
            : null;
        }

        public async Task<(List<Order>, string?, List<ErrorLog>?)> GetOrdersByNextToken(string nextToken, IList<string> marketplaceIds, RestrictedDataToken restrictedDataToken)
        {
            var errorLogs = new List<ErrorLog>();

            try
            {
                var queryParameters = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("NextToken", nextToken),
                    new KeyValuePair<string, string>("MarketplaceIds", string.Join(",", marketplaceIds))
                };

                CreateRestrictedDataTokenRequest? restrictedDataTokenRequest = GetRestrictedToken(restrictedDataToken.IsNeedRestrictedDataToken);

                await CreateAuthorizedRequestAsync(OrdersApiUrls.Orders, Method.GET, queryParameters, tokenDataType: AmazonEnum.TokenDataType.PII, createRestrictedDataTokenRequest: restrictedDataTokenRequest!);
                var response = await ExecuteRequestAsync<GetOrderResponse>(AmazonEnum.RateLimitType.Order_GetOrders);

                errorLogs.Add(new ErrorLog(Marketplace.Amazon, Sevarity.Information, "Orders", "", Priority.Low, "Total Orders: " + response?.Payload?.Orders?.Count));

                return (response!.Payload?.Orders!, response!.Payload?.NextToken, errorLogs);
            }
            catch (Exception ex)
            {
                errorLogs.Add(new ErrorLog(Marketplace.Amazon, Sevarity.Error, "Orders", "", Priority.High, ex.Message, ex.StackTrace));

                return (new List<Order>(), nextToken, errorLogs);
            }
        }

        public async Task<List<OrderItem>> GetOrderItems(string orderId)
        {
            var orderItemList = new List<OrderItem>();
            await CreateAuthorizedRequestAsync(OrdersApiUrls.OrderItems(orderId), Method.GET);

            var response = await ExecuteRequestAsync<GetOrderResponse>(AmazonEnum.RateLimitType.Order_GetOrderItems);

            var nextToken = response.Payload!.NextToken;
            orderItemList.AddRange(response.Payload.OrderItems!);

            while (!string.IsNullOrEmpty(nextToken))
            {
                var (orderItems, nextTkn) = await GetOrderItemsNextToken(orderId, nextToken);
                orderItemList.AddRange(orderItems);
                nextToken = nextTkn;
            }

            return orderItemList;
        }

        public async Task<(List<OrderItem>, string)> GetOrderItemsNextToken(string orderId, string nextToken)
        {
            var queryParameters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("NextToken", nextToken)
            };

            await CreateAuthorizedRequestAsync(OrdersApiUrls.OrderItems(orderId), RestSharp.Method.GET, queryParameters);
            var response = await ExecuteRequestAsync<GetOrderResponse>(AmazonEnum.RateLimitType.Order_GetOrderItems);
            return (response.Payload!.OrderItems!, response.Payload!.NextToken!);
        }

        /// <summary>
        /// Update Shipment Status
        /// </summary>
        /// <param name="parameterShipmentStatus"></param>
        /// <returns></returns>
        public async Task<(List<ConfirmShipment>, List<ErrorLog>)> ShipmentConfirmation(List<ParameterConfirmShipment> parameterShipmentStatus)
        {
            var errorLogs = new List<ErrorLog>();
            var confirmShipments = new List<ConfirmShipment>();

            foreach (var parameterShipment in parameterShipmentStatus)
            {
                try
                {
                    if (string.IsNullOrEmpty(parameterShipment.ConfirmShipmentRequest!.MarketplaceId))
                    {
                        parameterShipment.ConfirmShipmentRequest!.MarketplaceId = AmazonCredential.MarketPlaceId;
                    }

                    await CreateAuthorizedRequestAsync(OrdersApiUrls.ShipmentConfirmation(parameterShipment.OrderId!), Method.POST, postJsonObj: parameterShipment);
                    var response = await ExecuteRequestAsync<GetConfirmShipmentResponse>(AmazonEnum.RateLimitType.Order_ConfirmShipment);

                    var errorMessages = response.Errors?.Select(s => s.Message?.Replace("\"", "")).ToList();

                    errorLogs.Add(new ErrorLog(Marketplace.Amazon, Sevarity.Information, "Confirm Shipment", parameterShipment.OrderId!, Priority.Low, "Error Messages", string.Join(",", errorMessages!).Replace("\"", "")));

                    confirmShipments.Add(new ConfirmShipment
                    {
                        OrderId = parameterShipment.OrderId!,
                        IsConfirmedShipment = true,
                    });
                }
                catch (Exception ex)
                {
                    confirmShipments.Add(new ConfirmShipment
                    {
                        OrderId = parameterShipment.OrderId!,
                        IsConfirmedShipment = false,
                    });

                    errorLogs.Add(new ErrorLog(Marketplace.Amazon, Sevarity.Error, "Confirm Shipment", parameterShipment.OrderId!, Priority.High, ex.Message, ex.StackTrace));
                }
            }

            return (confirmShipments, errorLogs);
        }
    }
}
