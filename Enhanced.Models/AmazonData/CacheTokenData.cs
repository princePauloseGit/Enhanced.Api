using static Enhanced.Models.AmazonData.AmazonEnum;

namespace Enhanced.Models.AmazonData
{
    public class CacheTokenData
    {
        protected TokenResponse? NormalAccessToken { get; set; }
        protected TokenResponse? PIIAccessToken { get; set; }
        protected TokenResponse? GrantlessAccessToken { get; set; }
        protected AWSAuthenticationTokenData? AWSAuthenticationTokenData { get; set; }

        public AWSAuthenticationTokenData GetAWSAuthenticationTokenData()
        {

            return (AWSAuthenticationTokenData != null && AWSAuthenticationTokenData.Expiration.AddSeconds(-60) > DateTime.UtcNow)
                ? AWSAuthenticationTokenData
                : null!;
        }

        public void SetAWSAuthenticationTokenData(AWSAuthenticationTokenData tokenData)
        {
            AWSAuthenticationTokenData = tokenData;
        }

        public TokenResponse GetToken(TokenDataType tokenDataType)
        {
            TokenResponse token = null!;

            switch (tokenDataType)
            {
                case TokenDataType.Normal:
                    token = NormalAccessToken!;
                    break;

                case TokenDataType.Grantless:
                    token = GrantlessAccessToken!;
                    break;
            }

            return (token == null) ? null! : ((!IsTokenExpired(token.expires_in, token.date_Created)) ? token : null!);
        }

        public void SetToken(TokenDataType tokenDataType, TokenResponse token)
        {
            switch (tokenDataType)
            {
                case TokenDataType.Normal:
                    NormalAccessToken = token;
                    break;

                case TokenDataType.Grantless:
                    GrantlessAccessToken = token;
                    break;
            }
        }

        public static bool IsTokenExpired(int? expiresIn, DateTime? dateCreated) => dateCreated != null && DateTime.UtcNow.Subtract((DateTime)dateCreated).TotalSeconds > (expiresIn - 60);
    }

    public class AWSAuthenticationTokenData
    {
        public AWSAuthenticationCredentials? AWSAuthenticationCredential { get; set; }
        public string? SessionToken { get; set; }
        public DateTime Expiration { get; set; }
    }

    public class AWSAuthenticationCredentials
    {
        public string? AccessKeyId { get; set; }
        public string? SecretKey { get; set; }
        public string? Region { get; set; }
    }
}
