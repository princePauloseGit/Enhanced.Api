using Enhanced.Models.EbayData;
using Enhanced.Services.Ebay;
using Microsoft.AspNetCore.Mvc;

namespace Enhanced.Api.Controllers.Ebay
{
    [Route("api/[controller]")]
    [ApiController]
    public class EbayController : ControllerBase
    {
        [HttpPost]
        [Route("GetOAuthUrl")]
        public async Task<IActionResult> GetOAuthUrl([FromBody] OAuthUrlInput oAuthUrlInput)
        {
            try
            {
                EnvironmentManager.Environment = Models.Shared.CommonEnum.Environment.Production;

                var oAuthUrl = await Task.FromResult(EbayRequestService.GetOAuthUrl(oAuthUrlInput));

                return Ok(oAuthUrl);
            }
            catch (Exception ex)
            {
                return NotFound("An error occured while getting OAuth Url : " + ex.Message);
            }
        }

        [HttpPost]
        [Route("GetRefreshToken")]
        public async Task<IActionResult> GetRefreshToken([FromBody] RefreshTokenInput refreshTokenInput)
        {
            try
            {
                EnvironmentManager.Environment = Models.Shared.CommonEnum.Environment.Production;

                var oAuthUrl = await EbayRequestService.GetRefreshToken(refreshTokenInput);

                return Ok(oAuthUrl);
            }
            catch (Exception ex)
            {
                return NotFound("An error occured while getting Refresh Token : " + ex.Message);
            }
        }

        [HttpPost]
        [Route("GetOrders")]
        public async Task<IActionResult> GetOrders([FromHeader] AccessToken accessToken, EbayFilter ebayFilter)
        {
            try
            {
                EnvironmentManager.Environment = Models.Shared.CommonEnum.Environment.Production;
                ClientToken clientToken = new() { OAuthCredentials = accessToken.oauth_credentials };

                var ebayFulfilmentService = new EbayFulfilmentService(clientToken, accessToken);

                var (orders, errorLogs, nextToken) = await ebayFulfilmentService.GetOrders(ebayFilter).ConfigureAwait(false);
                nextToken ??= string.Empty;

                return Ok(new { nextToken, orders, errorLogs });
            }
            catch (Exception ex)
            {
                return NotFound("An error occured while getting orders : " + ex.Message);
            }
        }

        [HttpPost]
        [Route("CreateShippingFulfilment")]
        public async Task<IActionResult> CreateShippingFulfilment([FromHeader] AccessToken accessToken, [FromBody] EbayShipmentParameter ebayShipmentParameter)
        {
            try
            {
                EnvironmentManager.Environment = Models.Shared.CommonEnum.Environment.Production;
                ClientToken clientToken = new() { OAuthCredentials = accessToken.oauth_credentials };

                var ebayFulfilmentService = new EbayFulfilmentService(clientToken, accessToken);

                var (shipments, errorLogs) = await ebayFulfilmentService.CreateShippingFulfilment(ebayShipmentParameter).ConfigureAwait(false);

                return Ok(new { shipments, errorLogs });
            }
            catch (Exception ex)
            {
                return NotFound("An error occured while create shipping fulfilment : " + ex.Message);
            }
        }

        [HttpPost]
        [Route("GetOfferedListing")]
        public async Task<IActionResult> GetOfferedListing([FromHeader] AccessToken accessToken, List<OfferParameters> offerParameters)
        {
            try
            {
                EnvironmentManager.Environment = Models.Shared.CommonEnum.Environment.Production;
                ClientToken clientToken = new() { OAuthCredentials = accessToken.oauth_credentials };

                var eBayInventoryService = new EbayInventoryService(clientToken, accessToken);

                var (listing, errorLogs) = await eBayInventoryService.GetOfferedListing(offerParameters).ConfigureAwait(false);

                return Ok(new { listing, errorLogs });
            }
            catch (Exception ex)
            {
                return NotFound("An error occured while fetching offers : " + ex.Message);
            }
        }

        [HttpPost]
        [Route("BulkCreateListing")]
        public async Task<IActionResult> BulkCreateListing([FromHeader] AccessToken accessToken, BulkInventoryParameters inventoryparams)
        {
            try
            {
                EnvironmentManager.Environment = Models.Shared.CommonEnum.Environment.Production;
                ClientToken clientToken = new() { OAuthCredentials = accessToken.oauth_credentials };

                var eBayInventoryService = new EbayInventoryService(clientToken, accessToken);

                var (listing, errorLogs) = await eBayInventoryService.BulkCreateListing(inventoryparams).ConfigureAwait(false);

                return Ok(new { listing, errorLogs });
            }
            catch (Exception ex)
            {
                return NotFound("An error occured while creating listing : " + ex.Message);
            }
        }

        [HttpPost]
        [Route("CreateGroupListing")]
        public async Task<IActionResult> CreateGroupListing([FromHeader] AccessToken accessToken, GroupInventoryParameters groupInventoryParameters)
        {
            try
            {
                EnvironmentManager.Environment = Models.Shared.CommonEnum.Environment.Production;
                ClientToken clientToken = new() { OAuthCredentials = accessToken.oauth_credentials };

                var eBayInventoryService = new EbayInventoryService(clientToken, accessToken);

                var (listing, errorLogs) = await eBayInventoryService.BulkCreateGroupListing(groupInventoryParameters).ConfigureAwait(false);

                return Ok(new { listing, errorLogs });
            }
            catch (Exception ex)
            {
                return NotFound("An error occured while creating group listing : " + ex.Message);
            }
        }

        [HttpPost]
        [Route("DeleteInventoryItems")]
        public async Task<IActionResult> DeleteInventoryItems([FromHeader] AccessToken accessToken, DeleteInventoryParameter deleteInventoryItems)
        {
            try
            {
                EnvironmentManager.Environment = Models.Shared.CommonEnum.Environment.Production;
                ClientToken clientToken = new() { OAuthCredentials = accessToken.oauth_credentials };

                var eBayInventoryService = new EbayInventoryService(clientToken, accessToken);

                var errorLogs = await eBayInventoryService.DeleteInventoryItems(deleteInventoryItems?.SKUs!).ConfigureAwait(false);

                return Ok(new { errorLogs });
            }
            catch (Exception ex)
            {
                return NotFound("An error occured while delete inventory : " + ex.Message);
            }
        }

        [HttpPost]
        [Route("BulkUpdatePriceQuantity")]
        public async Task<IActionResult> BulkUpdatePriceQuantity([FromHeader] AccessToken accessToken, List<InventoryPriceQuantity> inventoryPriceQuantities)
        {
            try
            {
                EnvironmentManager.Environment = Models.Shared.CommonEnum.Environment.Production;
                ClientToken clientToken = new() { OAuthCredentials = accessToken.oauth_credentials };

                var eBayInventoryService = new EbayInventoryService(clientToken, accessToken);

                var (inventories, errorLogs) = await eBayInventoryService.BulkUpdatePriceQuantity(inventoryPriceQuantities).ConfigureAwait(false);

                return Ok(new { inventories, errorLogs });
            }
            catch (Exception ex)
            {
                return NotFound("An error occured while updating prince and quantity : " + ex.Message);
            }
        }

        [HttpPost]
        [Route("UpdateListingWithOffer")]
        public async Task<IActionResult> UpdateListingWithOffer([FromHeader] AccessToken accessToken, BulkInventoryParameters inventoryparams)
        {
            try
            {
                EnvironmentManager.Environment = Models.Shared.CommonEnum.Environment.Production;
                ClientToken clientToken = new() { OAuthCredentials = accessToken.oauth_credentials };

                var eBayInventoryService = new EbayInventoryService(clientToken, accessToken);

                var (responses, errorLogs) = await eBayInventoryService.UpdateListingWithOffer(inventoryparams).ConfigureAwait(false);

                return Ok(new { responses, errorLogs });
            }
            catch (Exception ex)
            {
                return NotFound("An error occured while updating inventory items : " + ex.Message);
            }
        }

        [HttpPost]
        [Route("DownloadPayment")]
        public async Task<IActionResult> DownloadPayment([FromHeader] AccessToken accessToken, EbayPaymentParameter eBayPayment)
        {
            try
            {
                EnvironmentManager.Environment = Models.Shared.CommonEnum.Environment.Production;
                ClientToken clientToken = new() { OAuthCredentials = accessToken.oauth_credentials };

                var eBayFinanceService = new EbayFinanceService(clientToken, accessToken);

                var response = await eBayFinanceService.GetPayouts(eBayPayment).ConfigureAwait(false);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return NotFound("An error occured while downloading payment : " + ex.Message);
            }
        }

        [HttpPost]
        [Route("RefundPayment")]
        public async Task<IActionResult> RefundPayment([FromHeader] AccessToken accessToken, [FromQuery] string orderId, [FromBody] EbayRefundRequest ebayRefundRequest)
        {
            try
            {
                EnvironmentManager.Environment = Models.Shared.CommonEnum.Environment.Production;
                ClientToken clientToken = new() { OAuthCredentials = accessToken.oauth_credentials };

                var eBayFinanceService = new EbayFinanceService(clientToken, accessToken);

                var (isRefunded, errorLog) = await eBayFinanceService.RefundPayment(orderId, ebayRefundRequest).ConfigureAwait(false);

                return Ok(new { isRefunded, errorLog });
            }
            catch (Exception ex)
            {
                return NotFound("An error occured while getting orders : " + ex.Message);
            }
        }

        [HttpGet]
        [Route("GetInventoryItem")]
        public async Task<IActionResult> GetInventoryItem([FromHeader] AccessToken accessToken, string sku)
        {
            try
            {
                EnvironmentManager.Environment = Models.Shared.CommonEnum.Environment.Production;
                ClientToken clientToken = new() { OAuthCredentials = accessToken.oauth_credentials };

                var eBayInventoryService = new EbayInventoryService(clientToken, accessToken);

                var inventory = await eBayInventoryService.GetInventoryItem(sku).ConfigureAwait(false);

                return Ok(new { inventory });
            }
            catch (Exception ex)
            {
                return NotFound("An error occured while get inventory item : " + ex.Message);
            }
        }

        [HttpGet]
        [Route("GetOffers")]
        public async Task<IActionResult> GetOffers([FromHeader] AccessToken accessToken, string sku)
        {
            try
            {
                EnvironmentManager.Environment = Models.Shared.CommonEnum.Environment.Production;
                ClientToken clientToken = new() { OAuthCredentials = accessToken.oauth_credentials };

                var eBayInventoryService = new EbayInventoryService(clientToken, accessToken);

                var offers = await eBayInventoryService.GetOffersAsync(sku).ConfigureAwait(false);

                return Ok(new { offers });
            }
            catch (Exception ex)
            {
                return NotFound("An error occured while get offers : " + ex.Message);
            }
        }
    }
}
