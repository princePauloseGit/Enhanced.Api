using Enhanced.Models.EbayData;
using Enhanced.Models.Shared;
using RestSharp;
using System.Text;
using System.Text.Json;
using System.Web;

namespace Enhanced.Services.Ebay
{
    public class OauthService : EbayRequestService
    {
        private static readonly string ACCESS_TOKEN_PROD = "eBayToken/access_token_prod.json";
        private static readonly string ACCESS_TOKEN_UAT = "eBayToken/access_token_uat.json";

        public OauthService(ClientToken clientToken) : base(clientToken, null!) { }

        public static async Task<AccessToken> GetAccessTokenAsync(ClientToken clientToken, AccessToken accessToken)
        {
            try
            {
                if (!string.IsNullOrEmpty(accessToken.access_token))
                {
                    bool isTokenExpired = IsTokenExpired(accessToken?.expires_in!.Value!, accessToken!.date_last_updated!.Value);

                    if (isTokenExpired)
                    {
                        return await new OauthService(clientToken).GetAccessToken(accessToken!);
                    }
                    else
                    {
                        return accessToken;
                    }
                }
                else
                {
                    var tokenPath = accessToken.Environment == CommonEnum.Environment.Production ? ACCESS_TOKEN_PROD : ACCESS_TOKEN_UAT;
                    var accessTokenFileData = await GetAccessTokenFileData(tokenPath);

                    if (accessTokenFileData != null)
                    {
                        bool isTokenExpired = IsTokenExpired(accessTokenFileData.expires_in, accessTokenFileData.date_last_updated!.Value);

                        if (isTokenExpired)
                        {
                            return await new OauthService(clientToken).GetAccessToken(accessToken!);
                        }
                        else
                        {
                            accessToken.access_token = accessTokenFileData.access_token;
                            accessToken.expires_in = accessTokenFileData.expires_in;
                            accessToken.date_last_updated = accessTokenFileData.date_last_updated;

                            return accessToken;
                        }
                    }
                    else
                    {
                        return await new OauthService(clientToken).GetAccessToken(accessToken!);
                    }
                }
            }
            catch (Exception)
            {
                return await new OauthService(clientToken).GetAccessToken(accessToken!);
            }
        }

        private static bool IsTokenExpired(int? expiresIn, DateTime dateCreated)
        {
            return DateTime.UtcNow.Subtract(dateCreated).TotalSeconds > expiresIn;
        }

        public virtual async Task<AccessToken> GetAccessToken(AccessToken token)
        {
            CreateRequest(OAuthUrls.RefreshTokenUrl, Method.POST);

            Dictionary<string, string> payloadParams = new()
            {
                { Constant.PAYLOAD_GRANT_TYPE, Constant.PAYLOAD_REFRESH_TOKEN },
                { Constant.PAYLOAD_REFRESH_TOKEN, token.refresh_token! },
            };

            var requestPayload = CommonHelper.CreateRequestPayload(payloadParams);

            Request!.AddHeader(Constant.HEADER_AUTHORIZATION, Constant.HEADER_PREFIX_BASIC + ClientToken!.OAuthCredentials);
            Request!.AddParameter(Constant.HEADER_CONTENT_TYPE, requestPayload, ParameterType.RequestBody);

            var refreshedToken = await ExecuteRequestAsync<AccessToken>();

            token.access_token = refreshedToken.access_token;
            token.expires_in = refreshedToken.expires_in;
            token.date_last_updated = DateTime.UtcNow;

            var accessTokenFileData = new AccessTokenViewModel
            {
                access_token = token.access_token,
                expires_in = token.expires_in,
                date_last_updated = token.date_last_updated,
            };

            var tokenPath = token.Environment == CommonEnum.Environment.Production ? ACCESS_TOKEN_PROD : ACCESS_TOKEN_UAT;
            await File.WriteAllTextAsync(tokenPath, JsonSerializer.Serialize(accessTokenFileData));

            return token;
        }

        public static new string GetOAuthUrl(OAuthUrlInput oAuthUrlInput)
        {
            var formattedScopes = CommonHelper.GetFormatScopes(oAuthUrlInput.Scopes!);

            //Prepare URL
            StringBuilder sb = new();
            sb.Append(OAuthUrls.AuthUrl).Append('?');

            //Prepare request payload
            var queryParams = new Dictionary<string, string>
            {
                {   Constant.PAYLOAD_CLIENT_ID, oAuthUrlInput.ClientId! },
                {   Constant.PAYLOAD_RESPONSE_TYPE, Constant.PAYLOAD_VALUE_CODE },
                {   Constant.PAYLOAD_REDIRECT_URI, oAuthUrlInput.RuName! },
                {   Constant.PAYLOAD_SCOPE, formattedScopes }
            };

            sb.Append(CommonHelper.CreateRequestPayload(queryParams));

            return sb.ToString();
        }

        public static async Task<RefreshTokenOutput> GetRefreshTokenAsync(RefreshTokenInput refreshTokenInput)
        {
            return await new OauthService(null!).GetRefreshToken(refreshTokenInput);
        }

        public new virtual async Task<RefreshTokenOutput> GetRefreshToken(RefreshTokenInput refreshTokenInput)
        {
            CreateRequest(OAuthUrls.RefreshTokenUrl, Method.POST);

            var queryParams = HttpUtility.ParseQueryString(refreshTokenInput.OAuthSuccessUrl!);
            string authorizationCode = HttpUtility.UrlEncode(queryParams.Get(Constant.PAYLOAD_VALUE_CODE))!;

            Dictionary<string, string> payloadParams = new()
            {
                { Constant.PAYLOAD_GRANT_TYPE, Constant.PAYLOAD_AUTHORIZATION_CODE },
                { Constant.PAYLOAD_VALUE_CODE, authorizationCode },
                { Constant.PAYLOAD_REDIRECT_URI, refreshTokenInput.RuName! },
            };

            var requestPayload = CommonHelper.CreateRequestPayload(payloadParams);

            Request!.AddHeader(Constant.HEADER_AUTHORIZATION, Constant.HEADER_PREFIX_BASIC + refreshTokenInput.OAuthCredentials!);
            Request!.AddParameter(Constant.HEADER_CONTENT_TYPE, requestPayload, ParameterType.RequestBody);

            var tokenResult = await ExecuteRequestAsync<RefreshTokenOutput>();

            tokenResult.expiry_date = DateTime.UtcNow.AddSeconds(tokenResult.refresh_token_expires_in).ToString("dd/MM/yyyy hh:mm tt");

            return tokenResult;
        }

        private static async Task<AccessTokenViewModel> GetAccessTokenFileData(string path)
        {
            var fileStream = GetFileStream(path);
            var result = await JsonSerializer.DeserializeAsync<AccessTokenViewModel>(fileStream);
            await DisposeObject(fileStream);
            return result!;
        }

        private static FileStream GetFileStream(string path)
        {
            return File.OpenRead(path);
        }

        private static async Task DisposeObject(FileStream fileStream)
        {
            fileStream.Close();
            await fileStream.DisposeAsync();
        }
    }
}
