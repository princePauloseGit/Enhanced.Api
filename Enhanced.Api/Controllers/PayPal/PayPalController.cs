using Enhanced.Models.PayPalData;
using Enhanced.Services.PayPal;
using Microsoft.AspNetCore.Mvc;

namespace Enhanced.Api.Controllers.PayPal
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayPalController : ControllerBase
    {
        private readonly IPayPalReportManager _payPalReportManager;

        public PayPalController(IPayPalReportManager payPalReportManager)
        {
            _payPalReportManager = payPalReportManager;
        }

        [HttpPost]
        [Route("DownloadPayment")]
        public IActionResult DownloadPayment([FromHeader] ParameterPayPalSFTP payPalSFTP, PayPalReportDocumentParam reportDocumentParam)
        {
            try
            {
                var settlementBatch = _payPalReportManager.GetPayPalSettlementReports(payPalSFTP, reportDocumentParam.ReportDocumentIds!);

                return Ok(settlementBatch);
            }
            catch (Exception ex)
            {
                return NotFound("An error occured while downloading paypal payment : " + ex.Message);
            }
        }
    }
}
