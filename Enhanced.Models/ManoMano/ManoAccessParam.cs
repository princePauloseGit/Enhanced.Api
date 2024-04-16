using Enhanced.Models.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Enhanced.Models.ManoMano
{
    public class ManoAccessParam
    {
        [FromHeader]
        public string? ApiKey { get; set; }

        [FromHeader]
        public CommonEnum.Environment Environment { get; set; } = CommonEnum.Environment.Production;
    }
}
