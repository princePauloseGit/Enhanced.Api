using System.Runtime.Serialization;
using static Enhanced.Models.AmazonData.AmazonEnum;

namespace Enhanced.Models.AmazonData
{
    /// <summary>
    /// Information required to encrypt and upload a feed document&#39;s contents.
    /// </summary>
    [DataContract]
    public partial class CreateFeedDocumentResult
    {
        /// <summary>
        /// The identifier of the feed document.
        /// </summary>
        /// <value>The identifier of the feed document.</value>
        [DataMember(Name = "feedDocumentId", EmitDefaultValue = false)]
        public string? FeedDocumentId { get; set; }

        /// <summary>
        /// The presigned URL for uploading the feed contents. This URL expires after 5 minutes.
        /// </summary>
        /// <value>The presigned URL for uploading the feed contents. This URL expires after 5 minutes.</value>
        [DataMember(Name = "url", EmitDefaultValue = false)]
        public string? Url { get; set; }

        /// <summary>
        /// Gets or Sets EncryptionDetails
        /// </summary>
        [DataMember(Name = "encryptionDetails", EmitDefaultValue = false)]
        public FeedDocumentEncryptionDetails? EncryptionDetails { get; set; }
    }

    /// <summary>
    /// Encryption details for required client-side encryption and decryption of document contents.
    /// </summary>
    [DataContract]
    public partial class FeedDocumentEncryptionDetails
    {
        /// <summary>
        /// The encryption standard required to encrypt or decrypt the document contents.
        /// </summary>
        /// <value>The encryption standard required to encrypt or decrypt the document contents.</value>
        [DataMember(Name = "standard", EmitDefaultValue = false)]
        public Standard Standard { get; set; }

        /// <summary>
        /// The vector to encrypt or decrypt the document contents using Cipher Block Chaining (CBC).
        /// </summary>
        /// <value>The vector to encrypt or decrypt the document contents using Cipher Block Chaining (CBC).</value>
        [DataMember(Name = "initializationVector", EmitDefaultValue = false)]
        public string? InitializationVector { get; set; }

        /// <summary>
        /// The encryption key used to encrypt or decrypt the document contents.
        /// </summary>
        /// <value>The encryption key used to encrypt or decrypt the document contents.</value>
        [DataMember(Name = "key", EmitDefaultValue = false)]
        public string? Key { get; set; }
    }

    [DataContract]
    public partial class CreateFeedDocumentSpecification
    {
        public CreateFeedDocumentSpecification(string ContentType = default!)
        {
            if (ContentType == null)
            {
                throw new InvalidDataException("ContentType is a required property for CreateFeedDocumentSpecification and cannot be null");
            }
            else
            {
                this.ContentType = ContentType;
            }
        }

        /// <summary>
        /// The content type of the feed.
        /// </summary>
        /// <value>The content type of the feed.</value>
        [DataMember(Name = "contentType", EmitDefaultValue = false)]
        public string ContentType { get; set; }
    }

    [DataContract]
    public partial class CreateFeedSpecification
    {
        /// <summary>
        /// The feed type.
        /// </summary>
        /// <value>The feed type.</value>
        [DataMember(Name = "feedType", EmitDefaultValue = false)]
        public string? FeedType { get; set; }

        /// <summary>
        /// A list of identifiers for marketplaces that you want the feed to be applied to.
        /// </summary>
        /// <value>A list of identifiers for marketplaces that you want the feed to be applied to.</value>
        [DataMember(Name = "marketplaceIds", EmitDefaultValue = false)]
        public List<string>? MarketplaceIds { get; set; }

        /// <summary>
        /// The document identifier returned by the createFeedDocument operation. Encrypt and upload the feed document contents before calling the createFeed operation.
        /// </summary>
        /// <value>The document identifier returned by the createFeedDocument operation. Encrypt and upload the feed document contents before calling the createFeed operation.</value>
        [DataMember(Name = "inputFeedDocumentId", EmitDefaultValue = false)]
        public string? InputFeedDocumentId { get; set; }

        /// <summary>
        /// Gets or Sets FeedOptions
        /// </summary>
        [DataMember(Name = "feedOptions", EmitDefaultValue = false)]
        public Dictionary<String, string>? FeedOptions { get; set; }
    }

    /// <summary>
    /// CreateFeedResult
    /// </summary>
    [DataContract]
    public partial class CreateFeedResult
    {
        /// <summary>
        /// The identifier for the feed. This identifier is unique only in combination with a seller ID.
        /// </summary>
        /// <value>The identifier for the feed. This identifier is unique only in combination with a seller ID.</value>
        [DataMember(Name = "feedId", EmitDefaultValue = false)]
        public string FeedId { get; set; }
    }
}
