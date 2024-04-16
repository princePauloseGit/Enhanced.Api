namespace Enhanced.Models.AmazonData
{
    public class CreateRestrictedDataTokenRequest
    {
        public IList<RestrictedResource>? restrictedResources { get; set; }
    }

    public class RestrictedResource
    {
        public string? method { get; set; }
        public string? path { get; set; }
        public IList<string>? dataElements { get; set; }
    }

    public class CreateRestrictedDataTokenResponse
    {
        public string? RestrictedDataToken { get; set; }
        public int ExpiresIn { get; set; }
    }
}
