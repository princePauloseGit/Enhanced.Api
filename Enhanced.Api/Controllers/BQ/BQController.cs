using Enhanced.Models.BQData;
using Enhanced.Services.BQ;
using Microsoft.AspNetCore.Mvc;

namespace Enhanced.Api.Controllers.BQ
{
    [Route("api/[controller]")]
    [ApiController]
    public class BQController : ControllerBase
    {
        private readonly IBQOrderManager _bqOrderManager;
        private readonly IBQService _bqService;

        public BQController(IBQOrderManager bQOrderManager, IBQService bqService)
        {
            _bqOrderManager = bQOrderManager;
            _bqService = bqService;
        }

        [HttpPost]
        [Route("GetBQOrders")]
        public async Task<IActionResult> GetBQOrders([FromHeader] ParamBQ paramBQAPI, [FromBody] ParamBQQuery paramQueryBQ)
        {
            try
            {
                return Ok(await _bqService.GetBQOrders(paramBQAPI, paramQueryBQ));
            }
            catch (Exception ex)
            {
                return NotFound("An error occured while getting orders : " + ex.Message);
            }
        }

        [HttpPost]
        [Route("CreateShipment")]
        public async Task<IActionResult> CreateShipment([FromHeader] ParamBQ paramBQAPI, ParamCreateShipment createShipment)
        {
            try
            {
                var (shipments, errorLogs) = await _bqService.CreateShipment(paramBQAPI, createShipment);

                return Ok(new { shipments, errorLogs });
            }
            catch (Exception ex)
            {
                return NotFound("An error occured while getting orders : " + ex.Message);
            }
        }

        [HttpPost]
        [Route("DownloadBQPayment")]
        public async Task<IActionResult> DownloadBQPayment([FromHeader] ParamBQ paramBQ, [FromBody] ParamPayment paramPaymentBQ)
        {
            try
            {
                var payment = await _bqOrderManager.DownloadBQPayment(paramBQ, paramPaymentBQ);
                return Ok(payment);
            }
            catch (Exception ex)
            {
                return NotFound("An error occured while downloading B&Q payment : " + ex.Message);
            }
        }

        [HttpPost]
        [Route("RefundPayment")]
        public async Task<IActionResult> RefundPayment([FromHeader] ParamBQ paramBQAPI, [FromQuery] string orderId, BQRefund refund)
        {
            try
            {
                var (isRefunded, errorLog) = await _bqService.RefundPayment(paramBQAPI.Authorization!, orderId, refund);

                return Ok(new { isRefunded, errorLog });
            }
            catch (Exception ex)
            {
                return NotFound("An error occured while refund payment : " + ex.Message);
            }
        }
    }
}
