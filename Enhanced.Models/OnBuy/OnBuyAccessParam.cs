using Microsoft.AspNetCore.Mvc;

namespace Enhanced.Models.OnBuy
{
    public class OnBuyAccessParam
    {
        [FromHeader]
        public string? Authorization { get; set; }
    }
}
