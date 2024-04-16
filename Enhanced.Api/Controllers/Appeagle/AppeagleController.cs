using Enhanced.Models.AppeagleData;
using Enhanced.Models.Shared;
using Enhanced.Services.Appeagle;
using Microsoft.AspNetCore.Mvc;

namespace Enhanced.Api.Controllers.Appeagle
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppeagleController : ControllerBase
    {
        private readonly IAppeagleService _appeagleService;

        public AppeagleController(IAppeagleService appeagleService)
        {
            _appeagleService = appeagleService;
        }

        [HttpPost]
        [Route("UploadFile")]
        public async Task<IActionResult> UploadFile([FromHeader] ParameterAppeagleSFTP appeagleSFTP, ParameterBase64Data base64EncodedData)
        {
            try
            {
                var errorLogs = await _appeagleService.UploadSFTPFile(appeagleSFTP, base64EncodedData.Base64EncodedData!);

                return Ok(errorLogs);
            }
            catch (Exception ex)
            {
                return NotFound("An error occured while uploading file : " + ex.Message);
            }
        }
    }
}
