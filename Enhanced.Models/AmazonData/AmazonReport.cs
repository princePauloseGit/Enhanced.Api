using System.Runtime.Serialization;
using static Enhanced.Models.AmazonData.AmazonEnum;

namespace Enhanced.Models.AmazonData
{
    /// <summary>
    /// Report
    /// </summary>
    [DataContract]
    public partial class AmazonReport
    {
        /// <summary>
        /// The processing status of the report.
        /// </summary>
        /// <value>The processing status of the report.</value>
        [DataMember(Name = "processingStatus", EmitDefaultValue = false)]
        public ProcessingStatuses ProcessingStatus { get; set; }
        /// <summary>
        /// A list of marketplace identifiers for the report.
        /// </summary>
        /// <value>A list of marketplace identifiers for the report.</value>
        [DataMember(Name = "marketplaceIds", EmitDefaultValue = false)]
        public List<string>? MarketplaceIds { get; set; }

        /// <summary>
        /// The identifier for the report. This identifier is unique only in combination with a seller ID.
        /// </summary>
        /// <value>The identifier for the report. This identifier is unique only in combination with a seller ID.</value>
        [DataMember(Name = "reportId", EmitDefaultValue = false)]
        public string? ReportId { get; set; }

        /// <summary>
        /// The report type.
        /// </summary>
        /// <value>The report type.</value>
        [DataMember(Name = "reportType", EmitDefaultValue = false)]
        public string? ReportType { get; set; }

        /// <summary>
        /// The start of a date and time range used for selecting the data to report.
        /// </summary>
        /// <value>The start of a date and time range used for selecting the data to report.</value>
        [DataMember(Name = "dataStartTime", EmitDefaultValue = false)]
        public DateTime? DataStartTime { get; set; }

        /// <summary>
        /// The end of a date and time range used for selecting the data to report.
        /// </summary>
        /// <value>The end of a date and time range used for selecting the data to report.</value>
        [DataMember(Name = "dataEndTime", EmitDefaultValue = false)]
        public DateTime? DataEndTime { get; set; }

        /// <summary>
        /// The identifier of the report schedule that created this report (if any). This identifier is unique only in combination with a seller ID.
        /// </summary>
        /// <value>The identifier of the report schedule that created this report (if any). This identifier is unique only in combination with a seller ID.</value>
        [DataMember(Name = "reportScheduleId", EmitDefaultValue = false)]
        public string? ReportScheduleId { get; set; }

        /// <summary>
        /// The date and time when the report was created.
        /// </summary>
        /// <value>The date and time when the report was created.</value>
        [DataMember(Name = "createdTime", EmitDefaultValue = false)]
        public DateTime? CreatedTime { get; set; }

        /// <summary>
        /// The date and time when the report processing started, in ISO 8601 date time format.
        /// </summary>
        /// <value>The date and time when the report processing started, in ISO 8601 date time format.</value>
        [DataMember(Name = "processingStartTime", EmitDefaultValue = false)]
        public DateTime? ProcessingStartTime { get; set; }

        /// <summary>
        /// The date and time when the report processing completed, in ISO 8601 date time format.
        /// </summary>
        /// <value>The date and time when the report processing completed, in ISO 8601 date time format.</value>
        [DataMember(Name = "processingEndTime", EmitDefaultValue = false)]
        public DateTime? ProcessingEndTime { get; set; }

        /// <summary>
        /// The identifier for the report document. Pass this into the getReportDocument operation to get the information you will need to retrieve and decrypt the report document&#39;s contents.
        /// </summary>
        /// <value>The identifier for the report document. Pass this into the getReportDocument operation to get the information you will need to retrieve and decrypt the report document&#39;s contents.</value>
        [DataMember(Name = "reportDocumentId", EmitDefaultValue = false)]
        public string? ReportDocumentId { get; set; }
    }
}
