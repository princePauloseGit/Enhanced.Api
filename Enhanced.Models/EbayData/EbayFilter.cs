namespace Enhanced.Models.EbayData
{
    public class EbayFilter
    {
        public int? Limit { get; set; } = 200;
        public int? Days { get; set; } = 10;
        public string? NextPage { get; set; } = string.Empty;
        public bool? ManualTest { get; set; } = false;
    }
}
