using Enhanced.Models.OnBuy;
using Enhanced.Services.OnBuy;
using Microsoft.AspNetCore.Mvc;

namespace Enhanced.Api.Controllers.OnBuy
{
    [Route("api/[controller]")]
    [ApiController]
    public class OnBuyController : ControllerBase
    {
        [HttpPost]
        [Route("RefundOnBuyPayment")]
        public async Task<IActionResult> RefundOnBuyPayment([FromHeader] OnBuyAccessParam onBuyAccessParam, OnBuyRefundRequest onBuyRefundRequest)
        {
            try
            {
                var onBuyService = new OnBuyOrderService(onBuyAccessParam.Authorization!);

                var refunds = await onBuyService.RefundOnBuyPayment(onBuyRefundRequest);

                return Ok(refunds);
            }
            catch (Exception ex)
            {
                return NotFound("An error occured while OnBuy refund payment : " + ex.Message);
            }
        }
    }
}
