using Microsoft.AspNetCore.Mvc;

namespace Enhanced.Models.AppeagleData
{
    public class ParameterAppeagleSFTP
    {
        [FromHeader]
        public string? SFTPHost { get; set; }
        [FromHeader]
        public int? SFTPPort { get; set; }
        [FromHeader]
        public string? SFTPUser { get; set; }
        [FromHeader]
        public string? SFTPPassword { get; set; }
        [FromHeader]
        public string? SFTPDestinationPath { get; set; }
    }
}
