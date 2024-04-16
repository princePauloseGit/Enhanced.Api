using static Enhanced.Models.AmazonData.AmazonEnum;

namespace Enhanced.Models.AmazonData
{
    public class ParameterCatalogItem : ParameterBased
    {
        public IList<string>? marketplaceIds { get; set; } = new List<string>();
        public IList<string> identifiers { get; set; } = new List<string>();
        public IdentifiersType? identifiersType { get; set; }
        public string? sellerId { get; set; }
        public int? pageSize { get; set; }
        public IList<IncludedData>? includedData { get; set; } = new List<IncludedData>();
    }

    public class ParameterCatalogItemASIN : ParameterBased
    {
        public List<string>? SKUs { get; set; }
    }
}
