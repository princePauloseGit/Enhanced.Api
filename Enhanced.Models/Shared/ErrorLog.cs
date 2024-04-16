using static Enhanced.Models.Shared.CommonEnum;

namespace Enhanced.Models.Shared
{
    public class ErrorLog
    {
        public string? Source { get; set; }
        public string? Sevarity { get; set; }
        public string? RecordType { get; set; }
        public string? RecordID { get; set; }
        public string Priority { get; set; }
        public string? Message { get; set; }
        public string? StackTrace { get; set; }

        public ErrorLog(Marketplace? source, Sevarity? sevarity, string? recordType, string? recordID, Priority priority, string? message = "Success", string? stackTrace = "Success")
        {
            Source = source.ToString();
            Sevarity = sevarity.ToString();
            RecordType = recordType;
            RecordID = recordID;
            Priority = priority.ToString();
            Message = message;
            StackTrace = stackTrace;
        }
    }
}
