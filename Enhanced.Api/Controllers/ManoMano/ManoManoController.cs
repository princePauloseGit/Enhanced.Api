using Enhanced.Models.EbayData;
using Enhanced.Models.ManoMano;
using Enhanced.Services.ManoMano;
using Microsoft.AspNetCore.Mvc;

namespace Enhanced.Api.Controllers.ManoMano
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManoManoController : ControllerBase
    {
        [HttpPost]
        [Route("GetOrders")]
        public async Task<IActionResult> GetOrders([FromHeader] ManoAccessParam accessParam, OrderRequestParam orderRequestParam)
        {
            try
            {
                EnvironmentManager.Environment = accessParam.Environment;
                var manoOrderService = new ManoOrderService(accessParam.ApiKey!);

                var orders = await manoOrderService.GetOrders(orderRequestParam);

                return Ok(orders);
            }
            catch (Exception ex)
            {
                return NotFound("An error occured while getting orders : " + ex.Message);
            }
        }

        [HttpPost]
        [Route("AcceptOrders")]
        public async Task<IActionResult> AcceptOrders([FromHeader] ManoAccessParam accessParam, AcceptOrderRequest acceptOrders)
        {
            try
            {
                EnvironmentManager.Environment = accessParam.Environment;
                var manoOrderService = new ManoOrderService(accessParam.ApiKey!);

                var orders = await manoOrderService.AcceptOrders(acceptOrders);

                return Ok(orders);
            }
            catch (Exception ex)
            {
                return NotFound("An error occured while getting orders : " + ex.Message);
            }
        }

        [HttpPost]
        [Route("CreateShipment")]
        public async Task<IActionResult> CreateShipment([FromHeader] ManoAccessParam accessParam, CreateShipmentRequest createShipmentRequest)
        {
            try
            {
                EnvironmentManager.Environment = accessParam.Environment;
                var manoOrderService = new ManoOrderService(accessParam.ApiKey!);

                var orders = await manoOrderService.CreateShipment(createShipmentRequest);

                return Ok(orders);
            }
            catch (Exception ex)
            {
                return NotFound("An error occured while getting orders : " + ex.Message);
            }
        }
    }
}
