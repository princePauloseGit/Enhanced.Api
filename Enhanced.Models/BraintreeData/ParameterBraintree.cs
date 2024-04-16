using Microsoft.AspNetCore.Mvc;

namespace Enhanced.Models.BraintreeData
{
    public class ParameterBraintree
    {
        [FromHeader]
        public string? MerchantId { get; set; }

        [FromHeader]
        public string? PrivateKey { get; set; }

        [FromHeader]
        public string? PublicKey { get; set; }
    }

    public class ParameterRefundBraintree
    {
        public string? TransactionId { get; set; }
        public decimal Amount { get; set; }
    }
}
