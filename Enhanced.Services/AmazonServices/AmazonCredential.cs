using Enhanced.Models.AmazonData;
using Enhanced.Models.Shared;
using Microsoft.AspNetCore.Mvc;
using static Enhanced.Models.AmazonData.AmazonEnum;

namespace Enhanced.Services.AmazonServices
{
    public class AmazonCredential
    {
        [FromHeader]
        public string? AccessKey { get; set; }
        [FromHeader]
        public string? SecretKey { get; set; }
        [FromHeader]
        public string? RoleArn { get; set; }
        [FromHeader]
        public string? ClientId { get; set; }
        [FromHeader]
        public string? ClientSecret { get; set; }
        [FromHeader]
        public string? RefreshToken { get; set; }
        [FromHeader]
        public string? MarketPlaceId { get; set; }
        [FromHeader]
        public string? NextToken { get; set; }
        [FromHeader]
        public string? MerchantId { get; set; }

        public MarketPlace? MarketPlace { get; set; }
        public bool IsActiveLimitRate { get; set; } = true;
        public CommonEnum.Environment Environment { get; set; } = CommonEnum.Environment.Production;
        public int MaxThrottledRetryCount { get; set; } = 5;
        public CacheTokenData? CacheTokenData { get; set; }

        internal Dictionary<AmazonEnum.RateLimitType, RateLimits> UsagePlansTimings { get; set; } = RateLimits.RateLimitsTime();

        public AmazonCredential()
        {
            CacheTokenData = new CacheTokenData();
        }

        public TokenResponse GetToken(TokenDataType tokenDataType)
        {
            return CacheTokenData!.GetToken(tokenDataType);
        }

        public void SetToken(TokenDataType tokenDataType, TokenResponse token)
        {
            CacheTokenData!.SetToken(tokenDataType, token);
        }

        public AWSAuthenticationTokenData GetAWSAuthenticationTokenData()
        {
            return CacheTokenData!.GetAWSAuthenticationTokenData();
        }

        public void SetAWSAuthenticationTokenData(AWSAuthenticationTokenData tokenData)
        {
            CacheTokenData!.SetAWSAuthenticationTokenData(tokenData);
        }
    }
}
