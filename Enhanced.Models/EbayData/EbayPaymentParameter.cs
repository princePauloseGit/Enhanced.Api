using Enhanced.Models.Shared;

namespace Enhanced.Models.EbayData
{
    public class EbayPaymentParameter : DownloadPaymentParameter
    {
        public string? JWE { get; set; }

        public string? PrivateKey { get; set; }

        public string? PayoutStatus { get; set; }
    }
}
