using Enhanced.Models.AmazonData;
using Enhanced.Models.Shared;
using RestSharp;
using System.Reflection;
using static Enhanced.Models.AmazonData.AmazonEnum;
using static Enhanced.Models.Shared.CommonEnum;

namespace Enhanced.Services.AmazonServices
{
    public class AmazonReportService : AmazonRequestService
    {
        public AmazonReportService(AmazonCredential amazonCredential) : base(amazonCredential) { }

        /// <summary>
        /// Download Existing Report And Download File
        /// </summary>
        /// <param name="reportTypes"></param>
        /// <param name="createdSince"></param>
        /// <param name="createdUntil"></param>
        /// <returns></returns>
        public async Task<(List<string>, List<ReportDocumentDetails>, List<ErrorLog>)> DownloadExistingReportAndDownloadFile(ReportTypes reportTypes, List<string> bcReportDocumentIds, DateTime? createdSince = null, DateTime? createdUntil = null)
        {
            var errorLogs = new List<ErrorLog>();
            var parameters = new ParameterReportList
            {
                reportTypes = new List<ReportTypes> { reportTypes },
                marketplaceIds = new List<string> { AmazonCredential.MarketPlace!.Id! },
                processingStatuses = new List<ProcessingStatuses> { ProcessingStatuses.DONE }
            };

            if (createdSince.HasValue)
            {
                parameters.createdSince = createdSince;
            }

            if (createdUntil.HasValue)
            {
                parameters.createdUntil = createdUntil;
            }

            string dates = string.Concat("Date From: ", parameters.createdSince?.Date.ToString(Constant.DATETIME_DDMMYYY_HHMMSS), " - Date To: ", parameters.createdUntil?.Date.ToString(Constant.DATETIME_DDMMYYY_HHMMSS));

            errorLogs.Add(new ErrorLog(Marketplace.Amazon, Sevarity.Information, "Download Payment", "", Priority.Low, dates));

            var (reports, errorLog) = await GetReports(parameters).ConfigureAwait(false);

            if (errorLog != null)
            {
                errorLogs.Add(errorLog);
            }

            var reportsPath = new List<string>();
            var reportDocumentIdsToUpdate = new List<ReportDocumentDetails>();

            if (reports != null && reports.Any())
            {
                var amzReportDocumentIds = reports.Select(s => s.ReportDocumentId).ToList();

                if (amzReportDocumentIds?.Any() == true)
                {
                    foreach (var reportDocumentId in bcReportDocumentIds)
                    {
                        if (!amzReportDocumentIds.Any(x => x == reportDocumentId))
                        {
                            reportDocumentIdsToUpdate.Add(new ReportDocumentDetails
                            {
                                ReportDocumentId = reportDocumentId,
                                CanArchived = true,
                            });
                        }
                    }
                }

                foreach (var reportData in reports)
                {
                    if (!string.IsNullOrEmpty(reportData.ReportDocumentId) && !bcReportDocumentIds.Any(x => x == reportData.ReportDocumentId))
                    {
                        try
                        {
                            var filePath = await GetReportFile(reportData.ReportDocumentId).ConfigureAwait(false);
                            reportsPath.Add(filePath);

                            reportDocumentIdsToUpdate.Add(new ReportDocumentDetails
                            {
                                ReportDocumentId = reportData.ReportDocumentId,
                                CanArchived = false,
                            });

                            errorLogs.Add(new ErrorLog(Marketplace.Amazon, Sevarity.Information, "Report Document", reportData.ReportDocumentId, Priority.Low));
                        }
                        catch (Exception ex)
                        {
                            errorLogs.Add(new ErrorLog(Marketplace.Amazon, Sevarity.Error, "Report Document", reportData.ReportDocumentId, Priority.High, ex.Message, ex.StackTrace));
                            continue;
                        }
                    }
                }
            }

            return (reportsPath, reportDocumentIdsToUpdate, errorLogs);
        }

        /// <summary>
        /// Get Reports
        /// </summary>
        /// <param name="parameterReportList"></param>
        /// <returns></returns>
        /// <exception cref="AmazonInvalidInputException"></exception>
        public async Task<(List<AmazonReport>, ErrorLog)> GetReports(ParameterReportList parameterReportList)
        {
            try
            {
                if (parameterReportList.createdSince.HasValue)
                {
                    var totalDays = (parameterReportList.createdSince.Value - DateTime.UtcNow).TotalDays;

                    if (totalDays > 90)
                    {
                        throw new AmazonInvalidInputException("Amazon api not accepting createdSince more than 90 days ," +
                            "The earliest report creation date and time for reports to include in the response, in ISO 8601 date time format. The default is 90 days ago. Reports are retained for a maximum of 90 days. https://github.com/amzn/selling-partner-api-docs/blob/main/references/reports-api/reports_2021-06-30.md#parameters");
                    }
                }

                var parameters = parameterReportList.GetParameters();

                await CreateAuthorizedRequestAsync(ReportApiUrls.GetReports, Method.GET, parameters).ConfigureAwait(false);
                var response = await ExecuteRequestAsync<GetReportsResponse>(RateLimitType.Report_GetReports).ConfigureAwait(false);

                parameterReportList.nextToken = response.NextToken;
                var list = response.Reports;

                while (!string.IsNullOrEmpty(parameterReportList.nextToken))
                {
                    var nextTokenResponse = await GetReportsByNextToken(parameterReportList).ConfigureAwait(false);
                    list!.AddRange(nextTokenResponse.Reports!);
                    parameterReportList.nextToken = nextTokenResponse.NextToken;
                }

                var errorLog = new ErrorLog(Marketplace.Amazon, Sevarity.Information, "Get Report", "", Priority.Low, "Total Files: " + list?.Count);

                return (list!, errorLog);
            }
            catch (Exception ex)
            {
                var errorLog = new ErrorLog(Marketplace.Amazon, Sevarity.Error, "Get Report", "", Priority.High, ex.Message, ex.StackTrace);
                return (null!, errorLog!);
            }
        }

        /// <summary>
        /// Get Reports By Next Token
        /// </summary>
        /// <param name="parameterReportList"></param>
        /// <returns></returns>
        public async Task<GetReportsResponse> GetReportsByNextToken(ParameterReportList parameterReportList)
        {
            var parameterReportListNew = new ParameterReportList
            {
                nextToken = parameterReportList.nextToken
            };

            var parameters = parameterReportListNew.GetParameters();

            await CreateAuthorizedRequestAsync(ReportApiUrls.GetReports, Method.GET, parameters).ConfigureAwait(false);
            var response = await ExecuteRequestAsync<GetReportsResponse>(RateLimitType.Report_GetReports).ConfigureAwait(false);

            return response;
        }

        /// <summary>
        /// Get Report File
        /// </summary>
        /// <param name="reportDocumentId"></param>
        /// <returns></returns>
        public async Task<string> GetReportFile(string reportDocumentId)
        {
            var reportDocument = await GetReportDocument(reportDocumentId).ConfigureAwait(false);

            return await GetFile(reportDocument).ConfigureAwait(false);
        }

        /// <summary>
        /// Get Report Document
        /// </summary>
        /// <param name="reportDocumentId"></param>
        /// <returns></returns>
        public async Task<ReportDocument> GetReportDocument(string reportDocumentId)
        {
            await CreateAuthorizedRequestAsync(ReportApiUrls.GetReportDocument(reportDocumentId), Method.GET).ConfigureAwait(false);

            var response = await ExecuteRequestAsync<ReportDocument>(RateLimitType.Report_GetReportDocument).ConfigureAwait(false);

            if (response != null)
            {
                return response;
            }

            return null!;
        }

        /// <summary>
        /// Get File
        /// </summary>
        /// <param name="reportDocument"></param>
        /// <returns></returns>
        private static async Task<string> GetFile(ReportDocument reportDocument)
        {
            bool isEncryptedFile = reportDocument.EncryptionDetails != null;
            bool isCompressionFile = reportDocument.CompressionAlgorithm is CompressionAlgorithm.GZIP;

            var client = new System.Net.WebClient();
            string fileName = Guid.NewGuid().ToString();

            if (isCompressionFile)
            {
                client.Headers[System.Net.HttpRequestHeader.AcceptEncoding] = "gzip";
                fileName += ".gz";
            }
            else
            {
                fileName += ".txt";
            }

            string rootFolderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;

            string sourceFile = Path.Combine(rootFolderPath, fileName);

            if (isEncryptedFile)
            {
                //Later will check
                byte[] rawData = client.DownloadData(reportDocument.Url!);
                byte[] key = Convert.FromBase64String(reportDocument.EncryptionDetails!.Key!);
                byte[] iv = Convert.FromBase64String(reportDocument.EncryptionDetails.InitializationVector!);

                var reportData = CommonHelper.DecryptString(key, iv, rawData);

                File.WriteAllText(sourceFile, reportData);
            }
            else
            {
                var stream = await client.OpenReadTaskAsync(new Uri(reportDocument.Url!)).ConfigureAwait(false);

                using (Stream s = File.Create(sourceFile))
                {
                    stream?.CopyTo(s);
                }
            }

            return isCompressionFile ? CommonHelper.Decompress(sourceFile) : sourceFile;
        }
    }
}
