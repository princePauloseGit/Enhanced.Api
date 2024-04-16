using System.Runtime.Serialization;
using static Enhanced.Models.AmazonData.AmazonEnum;

namespace Enhanced.Models.AmazonData
{
    /// <summary>
    /// ReportDocument
    /// </summary>
    [DataContract]
    public partial class ReportDocument
    {
        /// <summary>
        /// If present, the report document contents have been compressed with the provided algorithm.
        /// </summary>
        /// <value>If present, the report document contents have been compressed with the provided algorithm.</value>
        [DataMember(Name = "compressionAlgorithm", EmitDefaultValue = false)]
        public CompressionAlgorithm? CompressionAlgorithm { get; set; }

        /// <summary>
        /// The identifier for the report document. This identifier is unique only in combination with a seller ID.
        /// </summary>
        /// <value>The identifier for the report document. This identifier is unique only in combination with a seller ID.</value>
        [DataMember(Name = "reportDocumentId", EmitDefaultValue = false)]
        public string? ReportDocumentId { get; set; }

        /// <summary>
        /// A presigned URL for the report document. This URL expires after 5 minutes.
        /// </summary>
        /// <value>A presigned URL for the report document. This URL expires after 5 minutes.</value>
        [DataMember(Name = "url", EmitDefaultValue = false)]
        public string? Url { get; set; }

        /// <summary>
        /// Gets or Sets EncryptionDetails
        /// </summary>
        [DataMember(Name = "encryptionDetails", EmitDefaultValue = false)]
        public ReportDocumentEncryptionDetails? EncryptionDetails { get; set; }
    }

    /// <summary>
    /// Encryption details required for decryption of a report document&#39;s contents.
    /// </summary>
    [DataContract]
    public partial class ReportDocumentEncryptionDetails
    {
        /// <summary>
        /// The encryption standard required to decrypt the document contents.
        /// </summary>
        /// <value>The encryption standard required to decrypt the document contents.</value>
        [DataMember(Name = "standard", EmitDefaultValue = false)]
        public Standard Standard { get; set; }

        /// <summary>
        /// The vector to decrypt the document contents using Cipher Block Chaining (CBC).
        /// </summary>
        /// <value>The vector to decrypt the document contents using Cipher Block Chaining (CBC).</value>
        [DataMember(Name = "initializationVector", EmitDefaultValue = false)]
        public string? InitializationVector { get; set; }

        /// <summary>
        /// The encryption key used to decrypt the document contents.
        /// </summary>
        /// <value>The encryption key used to decrypt the document contents.</value>
        [DataMember(Name = "key", EmitDefaultValue = false)]
        public string? Key { get; set; }
    }
}
