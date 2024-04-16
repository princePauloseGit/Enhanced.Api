namespace Enhanced.Models.EbayData
{
    public class BulkResponseBase
    {
        public string? href { get; set; }
        public string? limit { get; set; }
        public string? next { get; set; }
        public string? offset { get; set; }
        public string? prev { get; set; }
        public string? total { get; set; }
        public List<Warning>? warnings { get; set; }
    }
}
