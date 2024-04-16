using Amazon.SecurityToken;
using Amazon.SecurityToken.Model;
using Enhanced.Models.AmazonData;
using Enhanced.Models.Shared;
using RestSharp;
using static Enhanced.Models.AmazonData.AmazonEnum;

namespace Enhanced.Services.AmazonServices
{
    public static class AmazonTokenGeneration
    {
        public static async Task<TokenResponse> RefreshAccessTokenAsync(AmazonCredential credentials, TokenDataType tokenDataType = TokenDataType.Normal)
        {
            var lwaCredentials = new LWAAuthorizationCredentials
            {
                ClientId = credentials.ClientId,
                ClientSecret = credentials.ClientSecret,
                Endpoint = new Uri(Constant.AmazonToeknEndPoint),
                RefreshToken = credentials.RefreshToken,
                Scopes = null
            };

            var Client = new LWAClient(lwaCredentials);
            var accessToken = await Client.GetAccessTokenAsync();

            return accessToken;
        }

        public static async Task<IRestRequest> SignWithSTSKeysAndSecurityTokenAsync(IRestRequest restRequest, string host, AmazonCredential amazonCredential)
        {
            var dataToken = amazonCredential.GetAWSAuthenticationTokenData();

            if (dataToken == null)
            {
                AssumeRoleResponse response1 = null!;

                using (var stsClient = new AmazonSecurityTokenServiceClient(amazonCredential.AccessKey, amazonCredential.SecretKey, Amazon.RegionEndpoint.EUWest2))
                {
                    var req = new AssumeRoleRequest
                    {
                        RoleArn = amazonCredential.RoleArn,
                        DurationSeconds = 3600,
                        RoleSessionName = Guid.NewGuid().ToString()
                    };

                    response1 = await stsClient.AssumeRoleAsync(req, new CancellationToken());
                }

                //auth step 3
                var awsAuthenticationCredentials = new AWSAuthenticationCredentials
                {
                    AccessKeyId = response1.Credentials.AccessKeyId,
                    SecretKey = response1.Credentials.SecretAccessKey,
                    Region = amazonCredential.MarketPlace!.Region!.RegionName
                };

                amazonCredential.SetAWSAuthenticationTokenData(new AWSAuthenticationTokenData
                {
                    AWSAuthenticationCredential = awsAuthenticationCredentials,
                    SessionToken = response1.Credentials.SessionToken,
                    Expiration = response1.Credentials.Expiration
                });

                dataToken = amazonCredential.GetAWSAuthenticationTokenData();
            }

            lock (restRequest)
            {
                restRequest.AddOrUpdateHeader(AmazonRequestService.SecurityTokenHeaderName, dataToken.SessionToken!);
            }

            return new AWSSigV4Signer(dataToken.AWSAuthenticationCredential!).Sign(restRequest, host);
        }
    }
}
