using Enhanced.Models.AppeagleData;
using Enhanced.Models.Shared;
using Renci.SshNet;
using static Enhanced.Models.Shared.CommonEnum;

namespace Enhanced.Services.Appeagle
{
    public class AppeagleService : IAppeagleService
    {
        /// <summary>
        /// Upload SFTP File
        /// </summary>
        /// <param name="appeagleSFTP"></param>
        /// <param name="base64EncodedData"></param>
        /// <returns></returns>
        public async Task<List<ErrorLog>> UploadSFTPFile(ParameterAppeagleSFTP appeagleSFTP, string base64EncodedData)
        {
            var errorLogs = new List<ErrorLog>();
            var port = appeagleSFTP.SFTPPort ?? 0;

            var (client, errorLog) = CommonHelper.GetSftpClient(appeagleSFTP.SFTPHost, port, appeagleSFTP.SFTPUser, appeagleSFTP.SFTPPassword, Marketplace.Appeagle);

            if (errorLog != null)
            {
                errorLogs.Add(errorLog);
            }

            if (client != null)
            {
                try
                {
                    var (sourceFile, error) = await CommonHelper.WriteToFile(base64EncodedData, Constant.AppeagleFileWithExt, Constant.AppeagleFileWithExt).ConfigureAwait(false);

                    if (error != null)
                    {
                        errorLogs.Add(error);
                    }

                    client.ChangeDirectory(appeagleSFTP.SFTPDestinationPath);

                    //RenameExistingFile(client);

                    using (FileStream fs = new(sourceFile, FileMode.Open))
                    {
                        client.UploadFile(fs, Path.GetFileName(sourceFile), canOverride: true);
                    }

                    client.Disconnect();
                    client.Dispose();

                    errorLogs.Add(new ErrorLog(Marketplace.Appeagle, Sevarity.Information, "File Uploaded", "", Priority.Low));
                }
                catch (Exception ex)
                {
                    errorLogs.Add(new ErrorLog(Marketplace.Appeagle, Sevarity.Error, "File Upload", "", Priority.High, ex.Message, ex.StackTrace));
                }
            }

            return errorLogs!;
        }

        /// <summary>
        /// Rename Existing File
        /// </summary>
        /// <param name="client"></param>
        private static void RenameExistingFile(SftpClient client)
        {
            string dateNow = DateTime.UtcNow.ToString(Constant.AppeagleDateCSVFormat);
            string newPath = string.Concat(Constant.AppeagleFileWithoutExt, "-", dateNow, ".csv");

            if (client.Exists(Constant.AppeagleFileWithExt))
            {
                client.RenameFile(Constant.AppeagleFileWithExt, newPath);
            }
        }
    }
}
