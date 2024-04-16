using Renci.SshNet;
using System.Globalization;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;
using static Enhanced.Models.Shared.CommonEnum;

namespace Enhanced.Models.Shared
{
    public static class CommonHelper
    {
        public static string GetEnumMemberValue<T>(this T value) where T : Enum
        {
            return typeof(T)
                .GetTypeInfo()
                .DeclaredMembers
                .SingleOrDefault(x => x.Name == value.ToString())
                ?.GetCustomAttribute<EnumMemberAttribute>(false)!
                ?.Value!;
        }

        public static string SerializeObject<T>(this T toSerialize)
        {
            var xmlSerializer = new XmlSerializer(toSerialize!.GetType());

            using (var textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, toSerialize);
                return textWriter.ToString();
            }
        }

        /// <summary>
        /// Write To File
        /// </summary>
        /// <param name="base64EncodedData"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static async Task<(string, ErrorLog)> WriteToFile(string base64EncodedData, string filename, string originalFilename)
        {
            try
            {
                byte[] binaryData = Convert.FromBase64String(base64EncodedData);
                string decodedData = GetUnZipFileText(binaryData, originalFilename); //Encoding.UTF8.GetString(data);
                string rootFolderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;

                DeletePreviousFiles(filename, rootFolderPath);

                string sourceFile = Path.Combine(rootFolderPath, filename);

                var stringBuilder = new StringBuilder();
                stringBuilder.Append(decodedData);

                await File.WriteAllTextAsync(sourceFile, stringBuilder.ToString());

                return (sourceFile, new ErrorLog(Marketplace.Amazon, Sevarity.Information, "FeedFile", sourceFile, Priority.Low));
            }
            catch (Exception ex)
            {
                return (string.Empty, new ErrorLog(Marketplace.Amazon, Sevarity.Error, "FeedFile", filename, Priority.Low, ex.Message, ex.StackTrace));
            }
        }

        private static string GetUnZipFileText(byte[] binaryData, string originalFilename)
        {
            string extractedText = string.Empty;

            using (MemoryStream zipStream = new(binaryData))
            {
                using (ZipArchive archive = new(zipStream, ZipArchiveMode.Read))
                {
                    ZipArchiveEntry entry = archive.GetEntry(originalFilename)!;

                    if (entry != null)
                    {
                        using (Stream entryStream = entry.Open())
                        {
                            using (StreamReader reader = new(entryStream, Encoding.UTF8))
                            {
                                extractedText = reader.ReadToEnd();
                            }
                        }
                    }
                }
            }

            return extractedText;
        }

        /// <summary>
        /// Delete Previous Files
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="rootFolderPath"></param>
        private static void DeletePreviousFiles(string filename, string rootFolderPath)
        {
            string fileToDelete = string.Empty;

            if (filename.StartsWith(Constant.AmazonProductFileWithoutExt))
            {
                fileToDelete = @"" + Constant.AmazonProductFileWithoutExt + "*.txt";
            }
            else if (filename.StartsWith(Constant.AmazonStockLevelFileWithoutExt))
            {
                fileToDelete = @"" + Constant.AmazonStockLevelFileWithoutExt + "*.txt";
            }
            else if (filename.StartsWith(Constant.AppeagleFileWithExt))
            {
                fileToDelete = @"" + Constant.AppeagleFileWithExt + "*.csv";
            }

            if (!string.IsNullOrEmpty(fileToDelete))
            {
                string[] fileList = Directory.GetFiles(rootFolderPath, fileToDelete);

                if (fileList != null)
                {
                    foreach (var path in fileList)
                    {
                        DeleteFile(path);
                    }
                }
            }
        }

        /// <summary>
        /// Get Feed File Format
        /// </summary>
        /// <param name="fileType"></param>
        /// <returns></returns>
        public static string GetFeedFileFormat(string fileType)
        {
            return string.Concat(
                fileType,
                "-",
                DateTime.UtcNow.ToString(Constant.AmazonFeedDateFormat),
                "_",
                DateTime.UtcNow.ToString(Constant.AmazonFeedTimeFormat),
                ".txt");
        }

        /// <summary>
        /// Get Date
        /// </summary>
        /// <param name="date"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static DateTime? GetDate(string date, string format)
        {
            if (DateTime.TryParseExact(date, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime value))
            {
                return value;
            }

            return null;
        }

        /// <summary>
        /// Get Decimal
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static decimal? GetDecimal(string data)
        {
            if (decimal.TryParse(data, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal value))
            {
                return value;
            }

            return 0.0M;
        }

        /// <summary>
        /// Get Int
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int? GetInt(string data)
        {
            if (int.TryParse(data, NumberStyles.Integer, CultureInfo.InvariantCulture, out int value))
            {
                return value;
            }

            return 0;
        }

        /// <summary>
        /// Check if string is numeric or not
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsNumeric(string input)
        {
            return int.TryParse(input, out _);
        }

        /// <summary>
        /// Decrypt String
        /// </summary>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <param name="cipherText"></param>
        /// <returns></returns>
        public static string DecryptString(byte[] key, byte[] iv, byte[] cipherText)
        {
            byte[] buffer = cipherText;

            using (var aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new(buffer))
                {
                    using (CryptoStream cryptoStream = new((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// De compress
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string Decompress(string fileName)
        {
            FileInfo fileInfo = new(fileName);

            using (FileStream originalFileStream = fileInfo.OpenRead())
            {
                string currentFileName = fileInfo.FullName;
                string newFileName = currentFileName.Remove(currentFileName.Length - fileInfo.Extension.Length);

                using (FileStream decompressedFileStream = File.Create(newFileName))
                {
                    using (var decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
                    {
                        decompressionStream.CopyTo(decompressedFileStream);
                        Console.WriteLine($"Decompressed: {fileInfo.Name}");
                    }
                    return decompressedFileStream.Name;
                }
            }
        }

        /// <summary>
        /// Delete File
        /// </summary>
        /// <param name="path"></param>
        public static void DeleteFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        /// <summary>
        /// Get SFTP Client
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static (SftpClient, ErrorLog) GetSftpClient(string? host, int port, string? user, string? password, Marketplace marketplace)
        {
            try
            {
                // Check without Port
                SftpClient client = new(host, user, password);

                IDictionary<string, HashInfo> hmacAlgorithms = client.ConnectionInfo.HmacAlgorithms;

                hmacAlgorithms["hmac-md5"] = new HashInfo(128, key => new SshNet.Security.Cryptography.HMACMD5(key));
                hmacAlgorithms["hmac-md5-96"] = new HashInfo(128, key => new SshNet.Security.Cryptography.HMACMD5(key, 96));
                hmacAlgorithms["hmac-sha1"] = new HashInfo(160, key => new SshNet.Security.Cryptography.HMACSHA1(key));
                hmacAlgorithms["hmac-sha1-96"] = new HashInfo(160, key => new SshNet.Security.Cryptography.HMACSHA1(key, 96));
                hmacAlgorithms["hmac-sha2-256"] = new HashInfo(256, key => new SshNet.Security.Cryptography.HMACSHA256(key));
                hmacAlgorithms["hmac-sha2-256-96"] = new HashInfo(256, key => new SshNet.Security.Cryptography.HMACSHA256(key, 96));
                hmacAlgorithms["hmac-sha2-512"] = new HashInfo(512, key => new SshNet.Security.Cryptography.HMACSHA512(key));
                hmacAlgorithms["hmac-sha2-512-96"] = new HashInfo(512, key => new SshNet.Security.Cryptography.HMACSHA512(key, 96));
                hmacAlgorithms["hmac-ripemd160"] = new HashInfo(160, key => new SshNet.Security.Cryptography.HMACRIPEMD160(key));
                hmacAlgorithms["hmac-ripemd160@openssh.com"] = new HashInfo(160, key => new SshNet.Security.Cryptography.HMACRIPEMD160(key));

                client.Connect();

                return client.IsConnected ? (client!, null!) : (null!, null!);
            }
            catch (Exception)
            {
                try
                {
                    // Check with Port
                    SftpClient client = new(host, port, user, password);

                    IDictionary<string, HashInfo> hmacAlgorithms = client.ConnectionInfo.HmacAlgorithms;

                    hmacAlgorithms["hmac-md5"] = new HashInfo(128, key => new SshNet.Security.Cryptography.HMACMD5(key));
                    hmacAlgorithms["hmac-md5-96"] = new HashInfo(128, key => new SshNet.Security.Cryptography.HMACMD5(key, 96));
                    hmacAlgorithms["hmac-sha1"] = new HashInfo(160, key => new SshNet.Security.Cryptography.HMACSHA1(key));
                    hmacAlgorithms["hmac-sha1-96"] = new HashInfo(160, key => new SshNet.Security.Cryptography.HMACSHA1(key, 96));
                    hmacAlgorithms["hmac-sha2-256"] = new HashInfo(256, key => new SshNet.Security.Cryptography.HMACSHA256(key));
                    hmacAlgorithms["hmac-sha2-256-96"] = new HashInfo(256, key => new SshNet.Security.Cryptography.HMACSHA256(key, 96));
                    hmacAlgorithms["hmac-sha2-512"] = new HashInfo(512, key => new SshNet.Security.Cryptography.HMACSHA512(key));
                    hmacAlgorithms["hmac-sha2-512-96"] = new HashInfo(512, key => new SshNet.Security.Cryptography.HMACSHA512(key, 96));
                    hmacAlgorithms["hmac-ripemd160"] = new HashInfo(160, key => new SshNet.Security.Cryptography.HMACRIPEMD160(key));
                    hmacAlgorithms["hmac-ripemd160@openssh.com"] = new HashInfo(160, key => new SshNet.Security.Cryptography.HMACRIPEMD160(key));

                    client.Connect();

                    if (client.IsConnected)
                    {
                        return (client!, null!);
                    }
                    else
                    {
                        var errorLog = new ErrorLog(marketplace, Sevarity.Error, "SFTP Connection", host, Priority.High, ErrorMessageConstant.SFTP_CONN, string.Concat("Is Connected: ", client.IsConnected));
                        return (null!, errorLog);
                    }
                }
                catch (Exception exp)
                {
                    var errorLog = new ErrorLog(marketplace, Sevarity.Error, "SFTP Connection", host, Priority.High, exp.Message, exp.StackTrace);
                    return (null!, errorLog);
                }
            }
        }

        /// <summary>
        /// Get String From Right by passing number of characters
        /// </summary>
        /// <param name="input"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static string GetStringFromRight(string input, int right)
        {
            return input.Substring(input.Length - right, right);
        }

        public static string CreateRequestPayload(Dictionary<string, string> payloadParams)
        {
            StringBuilder sb = new();

            foreach (var entry in payloadParams)
            {
                if (sb.Length > 0)
                {
                    sb.Append(Constant.PAYLOAD_PARAM_DELIMITER);
                }

                sb.Append(entry.Key).Append(Constant.PAYLOAD_VALUE_DELIMITER).Append(entry.Value);
            }

            return sb.ToString();
        }

        public static string GetFormatScopes(IList<string> scopes)
        {
            string scopesForRequest = string.Empty;

            if (scopes == null || scopes.Count == 0)
            {
                return scopesForRequest;
            }

            foreach (var scope in scopes)
            {
                scopesForRequest = string.IsNullOrEmpty(scopesForRequest) ? scope : scopesForRequest + "+" + scope;
            }

            return scopesForRequest.ToLower();
        }

        public static string Base64Encode(string plainText)
        {
            return string.IsNullOrEmpty(plainText)
                ? string.Empty
                : Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));
        }

        public static string Base64Decode(string base64EncodedData)
        {
            return string.IsNullOrEmpty(base64EncodedData)
                ? string.Empty
                : Encoding.UTF8.GetString(Convert.FromBase64String(base64EncodedData));
        }
    }
}
