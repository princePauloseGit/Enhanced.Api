namespace Enhanced.Models.EbayData
{
    public class EbayErrorHelper
    {
        public class Errors
        {
            public List<Error>? errors { get; set; }
            public List<Error>? warnings { get; set; }
            public string? resource { get; set; }
        }

        public class Error
        {
            public int errorId { get; set; }
            public string? domain { get; set; }
            public string? category { get; set; }
            public string? message { get; set; }
            public string? longMessage { get; set; }
            public List<Parameter>? parameters { get; set; }
        }

        public class Parameter
        {
            public string? name { get; set; }
            public string? value { get; set; }
        }

        public class Warning
        {
            public string? category { get; set; }
            public string? domain { get; set; }
            public string? errorId { get; set; }
            public List<string>? inputRefIds { get; set; }
            public string? longMessage { get; set; }
            public string? message { get; set; }
            public List<string>? outputRefIds { get; set; }
            public List<Parameter>? parameters { get; set; }
            public string? subdomain { get; set; }
        }
    }
}
