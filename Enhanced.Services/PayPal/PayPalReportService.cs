using Enhanced.Models.PayPalData;
using Enhanced.Models.Shared;
using System.Reflection;
using static Enhanced.Models.Shared.CommonEnum;

namespace Enhanced.Services.PayPal
{
    public class PayPalReportService : IPayPalReportService
    {
        public (List<string>, List<ErrorLog>) DownloadSFTPFiles(ParameterPayPalSFTP payPalSFTP, List<string> bcReportDocumentIds)
        {
            var paths = new List<string>();
            var errorLogs = new List<ErrorLog>();
            var port = payPalSFTP.SFTPPort ?? 0;

            var (client, errorLog) = CommonHelper.GetSftpClient(payPalSFTP.SFTPHost, port, payPalSFTP.SFTPUser, payPalSFTP.SFTPPassword, Marketplace.PayPal);

            if (errorLog != null)
            {
                errorLogs.Add(errorLog);
            }

            if (client != null)
            {
                var files = client.ListDirectory(payPalSFTP.SFTPDestinationPath);

                foreach (var file in files)
                {
                    bool hasDownloaded = bcReportDocumentIds.Any(a => file?.Name?.Contains(a.Replace("PP", string.Empty)) == true);

                    if (!file.Name.StartsWith(".") && hasDownloaded == false)
                    {
                        string rootFolderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
                        string sourcePath = Path.Combine(rootFolderPath, file.Name);

                        using (Stream fileStream = File.Create(sourcePath))
                        {
                            string errorDetails = string.Concat("File Downloaded: ", file.Name);

                            try
                            {
                                client.DownloadFile(file.FullName, fileStream);
                                
                                errorLogs.Add(new ErrorLog(Marketplace.PayPal, Sevarity.Information, errorDetails, file.Name, Priority.Low));
                            }
                            catch (Exception ex)
                            {
                                errorLogs.Add(new ErrorLog(Marketplace.PayPal, Sevarity.Error, errorDetails, file.Name, Priority.High, ex.Message, ex.StackTrace));
                                continue;
                            }
                            finally
                            {
                                paths.Add(sourcePath);
                            }
                        }
                    }
                }

                client.Disconnect();
                client.Dispose();
            }
            else
            {
                return (null!, errorLogs!);
            }

            return (paths, errorLogs);
        }
    }
}
