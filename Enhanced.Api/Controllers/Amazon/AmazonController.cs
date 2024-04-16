using Enhanced.Models.AmazonData;
using Enhanced.Models.Shared;
using Enhanced.Services.AmazonServices;
using Microsoft.AspNetCore.Mvc;
using static Enhanced.Models.Shared.CommonEnum;

namespace Enhanced.Api.Controllers.Amazon
{
    [Route("api/[controller]")]
    [ApiController]
    public class AmazonController : ControllerBase
    {
        private readonly IAmazonReportManager _amazonReportManager;

        public AmazonController(IAmazonReportManager amazonReportManager)
        {
            _amazonReportManager = amazonReportManager;
        }

        [HttpPost]
        [Route("GetOrders")]
        public async Task<IActionResult> GetOrders([FromHeader] AmazonCredential amazonCredential, ParameterOrderList parameter)
        {
            try
            {
                var amazonOrderService = new AmazonOrderService(amazonCredential);
                var (orders, nextToken, errorLogs) = await amazonOrderService!.GetOrders(parameter).ConfigureAwait(false);

                if (orders != null && orders.Any())
                {
                    await GetOrderItemsByOrderId(amazonOrderService, orders, errorLogs!).ConfigureAwait(false);

                    if (string.IsNullOrEmpty(nextToken))
                    {
                        nextToken = string.Empty;
                    }
                }

                return Ok(new { orders, nextToken, errorLogs });
            }
            catch (Exception ex)
            {
                return NotFound("An error occured while getting orders : " + ex.Message);
            }
        }

        [HttpPost]
        [Route("GetOrdersByNextToken")]
        public async Task<IActionResult> GetOrdersByNextToken([FromHeader] AmazonCredential amazonCredential, RestrictedDataToken restrictedDataToken)
        {
            try
            {
                var amazonOrderService = new AmazonOrderService(amazonCredential);
                IList<string> marketplaceIds = new List<string> { amazonCredential.MarketPlaceId! };

                var (orders, nextToken, errorLogs) = await amazonOrderService!.GetOrdersByNextToken(amazonCredential.NextToken!, marketplaceIds, restrictedDataToken).ConfigureAwait(false);

                if (orders != null && orders.Any())
                {
                    await GetOrderItemsByOrderId(amazonOrderService, orders, errorLogs!).ConfigureAwait(false);

                    if (string.IsNullOrEmpty(nextToken))
                    {
                        nextToken = string.Empty;
                    }
                }

                return Ok(new { orders, nextToken, errorLogs });
            }
            catch (Exception ex)
            {
                return NotFound("An error occured while getting orders : " + ex.Message);
            }
        }

        private static async Task GetOrderItemsByOrderId(AmazonOrderService amazonOrderService, List<Order> orders, List<ErrorLog> errorLogs)
        {
            foreach (var order in orders)
            {
                var orderItems = await amazonOrderService!.GetOrderItems(order.AmazonOrderId!);

                if (orderItems != null && orderItems.Any())
                {
                    order.OrderItemList = orderItems;

                    errorLogs.Add(new ErrorLog(Marketplace.Amazon, Sevarity.Information, "Order Id", order.AmazonOrderId, Priority.Low, "Total Items: " + orderItems.Count));
                }
            }
        }

        [HttpPost]
        [Route("GetCatalogItems")]
        public async Task<IActionResult> GetCatalogItems([FromHeader] AmazonCredential amazonCredential, ParameterCatalogItemASIN parameterCatalogItemASIN)
        {
            try
            {
                var catalogItemService = new AmazonCatalogItemService(amazonCredential);
                var (catalogItems, errorLogs) = await catalogItemService!.SearchCatalogItem(parameterCatalogItemASIN, amazonCredential.MerchantId!).ConfigureAwait(false);

                return Ok(new { catalogItems, errorLogs });
            }
            catch (Exception ex)
            {
                return NotFound("An error occured while getting catalog items : " + ex.Message);
            }
        }

        [HttpPost]
        [Route("SubmitFeed/Product")]
        public async Task<IActionResult> SubmitProductFeed([FromHeader] AmazonCredential amazonCredential, ParameterBase64Data base64EncodedData)
        {
            try
            {
                var errorLogs = new List<ErrorLog>();

                var feedFilename = CommonHelper.GetFeedFileFormat(Constant.AmazonProductFileWithoutExt);
                var (sourceFile, error) = await CommonHelper.WriteToFile(base64EncodedData.Base64EncodedData!, feedFilename, Constant.AmazonProductFileWithExt).ConfigureAwait(false);

                if (error != null)
                {
                    errorLogs.Add(error);
                }

                var amazonFeedService = new AmazonFeedService(amazonCredential);
                var (result, errorLog) = await amazonFeedService.SubmitFeed(sourceFile, AmazonEnum.FeedType.POST_FLAT_FILE_LISTINGS_DATA, AmazonEnum.ContentType.TXT).ConfigureAwait(false);

                if (errorLog != null)
                {
                    errorLogs.Add(errorLog);
                }

                return Ok(new { result, errorLogs });
            }
            catch (Exception ex)
            {
                return NotFound("An error occured while submitting feedback - (Product) : " + ex.Message);
            }
        }

        [HttpPost]
        [Route("SubmitFeed/StockLevel")]
        public async Task<IActionResult> SubmitInventoryFeed([FromHeader] AmazonCredential amazonCredential, ParameterBase64Data base64EncodedData)
        {
            try
            {
                var errorLogs = new List<ErrorLog>();               

                var feedFilename = CommonHelper.GetFeedFileFormat(Constant.AmazonStockLevelFileWithoutExt);
                var (sourceFile, error) = await CommonHelper.WriteToFile(base64EncodedData.Base64EncodedData!, feedFilename, Constant.AmazonStockLevelFileWithExt).ConfigureAwait(false);

                if (error != null)
                {
                    errorLogs.Add(error);
                }

                var amazonFeedService = new AmazonFeedService(amazonCredential);
                var (result, errorLog) = await amazonFeedService.SubmitFeed(sourceFile, AmazonEnum.FeedType.POST_FLAT_FILE_INVLOADER_DATA, AmazonEnum.ContentType.TXT).ConfigureAwait(false);

                if (errorLog != null)
                {
                    errorLogs.Add(errorLog);
                }

                return Ok(new { result, errorLogs });
            }
            catch (Exception ex)
            {
                return NotFound("An error occured while submitting feedback (StockLevel) : " + ex.Message);
            }
        }

        [HttpPost]
        [Route("DownloadPayment")]
        public async Task<IActionResult> DownloadPayment([FromHeader] AmazonCredential amazonCredential, DownloadPaymentParameter amazonPaymentParameter)
        {
            try
            {
                var settlementBatch = await _amazonReportManager.GetSettlementOrder(amazonCredential, amazonPaymentParameter).ConfigureAwait(false);

                return Ok(settlementBatch);
            }
            catch (Exception ex)
            {
                return NotFound("An error occured while downloading amazon payment : " + ex.Message);
            }
        }

        [HttpPost]
        [Route("DownloadPaymentForCredits")]
        public async Task<IActionResult> DownloadPaymentForCredits([FromHeader] AmazonCredential amazonCredential, DownloadPaymentParameter amazonPaymentParameter)
        {
            try
            {
                var settlementBatchForCredits = await _amazonReportManager.GetSettlementOrderForCredits(amazonCredential, amazonPaymentParameter).ConfigureAwait(false);

                return Ok(settlementBatchForCredits);
            }
            catch (Exception ex)
            {
                return NotFound("An error occured while downloading amazon payment : " + ex.Message);
            }
        }

        [HttpPost]
        [Route("ShipmentConfirmation")]
        public async Task<IActionResult> ShipmentConfirmation([FromHeader] AmazonCredential amazonCredential, List<ParameterConfirmShipment>? parameterShipmentStatus)
        {
            try
            {
                var amazonOrderService = new AmazonOrderService(amazonCredential);
                var (shipments, errorLogs) = await amazonOrderService.ShipmentConfirmation(parameterShipmentStatus!).ConfigureAwait(false);

                return Ok(new { shipments, errorLogs });
            }
            catch (Exception ex)
            {
                return NotFound("An error occured while updating order shipment status : " + ex.Message);
            }
        }
    }
}
