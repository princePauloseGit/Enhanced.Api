using Enhanced.Models.BraintreeData;
using Enhanced.Models.Shared;
using Enhanced.Services.BraintreeService;
using Microsoft.AspNetCore.Mvc;

namespace Enhanced.Api.Controllers.Braintree
{
    [Route("api/[controller]")]
    [ApiController]
    public class BraintreeController : ControllerBase
    {
        private readonly IBraintreeReportManager _braintreeReportManager;

        public BraintreeController(IBraintreeReportManager braintreeReportManager)
        {
            _braintreeReportManager = braintreeReportManager;
        }

        [HttpPost]
        [Route("DownloadPayment")]
        public async Task<IActionResult> DownloadPayment([FromHeader] ParameterBraintree parameterBraintree, DownloadPaymentParameter downloadPaymentParameter)
        {
            try
            {
                var transactions = await _braintreeReportManager.SearchTransaction(parameterBraintree, downloadPaymentParameter);

                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return NotFound("An error occured while downloading braintree payment : " + ex.Message);
            }
        }

        [HttpPost]
        [Route("RefundPayment")]
        public async Task<IActionResult> RefundPayment([FromHeader] ParameterBraintree parameterBraintree, ParameterRefundBraintree parameterRefund)
        {
            try
            {
                var (isRefunded, errorLog) = await _braintreeReportManager.RefundPayment(parameterBraintree, parameterRefund);

                return Ok(new { isRefunded, errorLog });
            }
            catch (Exception ex)
            {
                return NotFound("An error occured while braintree refund payment : " + ex.Message);
            }
        }
    }
}
