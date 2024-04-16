using Enhanced.Models.AmazonData;
using Enhanced.Models.Shared;
using System.Net;
using static Enhanced.Models.AmazonData.AmazonEnum;
using static Enhanced.Models.Shared.CommonEnum;

namespace Enhanced.Services.AmazonServices
{
    public class AmazonFeedService : AmazonRequestService
    {
        public AmazonFeedService(AmazonCredential amazonCredential) : base(amazonCredential) { }

        /// <summary>
        /// Submit Feed
        /// </summary>
        /// <param name="xmlContentOrFilePath"></param>
        /// <param name="feedType"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public async Task<(string, ErrorLog)> SubmitFeed(string xmlContentOrFilePath, FeedType feedType, ContentType contentType = ContentType.TXT)
        {
            try
            {
                var feedCreate = await CreateFeedDocument(contentType).ConfigureAwait(false);

                if (feedCreate == null)
                {
                    return (string.Empty, new ErrorLog(Marketplace.Amazon, Sevarity.Error, "Create Feed document", "", Priority.High, "Empty response"));
                }

                var response = await PostFileData(feedCreate.Url!, xmlContentOrFilePath, contentType);

                if (response == null)
                {
                    return (string.Empty, new ErrorLog(Marketplace.Amazon, Sevarity.Error, "Post File Data", "", Priority.High, "Empty response"));
                }

                var createFeedSpecification = new CreateFeedSpecification
                {
                    FeedType = feedType.ToString(),
                    InputFeedDocumentId = feedCreate.FeedDocumentId,
                    MarketplaceIds = new List<string> { AmazonCredential.MarketPlace!.Id! },
                    FeedOptions = new Dictionary<string, string>()
                };

                var feed = await CreateFeed(createFeedSpecification).ConfigureAwait(false);

                return (feed.FeedId, new ErrorLog(Marketplace.Amazon, Sevarity.Information, "Feed Id", feed.FeedId, Priority.Low, "Path:" + feedCreate?.Url));
            }
            catch (Exception ex)
            {
                return (string.Empty, new ErrorLog(Marketplace.Amazon, Sevarity.Error, "Feed Id", "", Priority.High, ex.Message, ex.StackTrace));
            }
        }

        /// <summary>
        /// Create Feed Document
        /// </summary>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public async Task<CreateFeedDocumentResult> CreateFeedDocument(ContentType contentType)
        {
            var contxt = CommonHelper.GetEnumMemberValue(contentType);
            var createFeedDocumentSpecification = new CreateFeedDocumentSpecification(contxt);

            await CreateAuthorizedRequestAsync(FeedsApiUrls.CreateFeedDocument, RestSharp.Method.POST, postJsonObj: createFeedDocumentSpecification).ConfigureAwait(false);

            var response = await ExecuteRequestAsync<CreateFeedDocumentResult>(RateLimitType.Feed_CreateFeedDocument).ConfigureAwait(false);

            if (response != null)
            {
                return response;
            }

            return null!;
        }

        /// <summary>
        /// Post File Data
        /// </summary>
        /// <param name="destinationUrl"></param>
        /// <param name="xmlContentOrFilePath"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        private static async Task<string> PostFileData(string destinationUrl, string xmlContentOrFilePath, ContentType contentType = ContentType.TXT)
        {
            var request = (HttpWebRequest)WebRequest.Create(destinationUrl);

            byte[] bytes = contentType == ContentType.XML
                ? System.Text.Encoding.ASCII.GetBytes(xmlContentOrFilePath)
                : File.ReadAllBytes(xmlContentOrFilePath);

            request.ContentType = CommonHelper.GetEnumMemberValue(contentType);
            request.ContentLength = bytes.Length;
            request.Method = "PUT";

            var requestStream = request.GetRequestStream();
            requestStream.Write(bytes, 0, bytes.Length);
            requestStream.Close();

            var response = (HttpWebResponse)await request.GetResponseAsync();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var responseStream = response.GetResponseStream();
                return new StreamReader(responseStream).ReadToEnd();
            }

            return null!;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="createFeedSpecification"></param>
        /// <returns></returns>
        public async Task<CreateFeedResult> CreateFeed(CreateFeedSpecification createFeedSpecification)
        {
            await CreateAuthorizedRequestAsync(FeedsApiUrls.CreateFeed, RestSharp.Method.POST, postJsonObj: createFeedSpecification).ConfigureAwait(false);
            var response = await ExecuteRequestAsync<CreateFeedResult>(RateLimitType.Feed_CreateFeed).ConfigureAwait(false);

            return response;
        }
    }
}
