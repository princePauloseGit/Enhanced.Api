using Enhanced.Models.AppeagleData;
using Enhanced.Models.Shared;

namespace Enhanced.Services.Appeagle
{
    public interface IAppeagleService
    {
        Task<List<ErrorLog>> UploadSFTPFile(ParameterAppeagleSFTP appeagleSFTP, string base64EncodedData);
    }
}
