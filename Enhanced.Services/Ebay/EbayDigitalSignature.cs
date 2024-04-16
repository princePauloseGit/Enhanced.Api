using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Security;
using RestSharp;
using System.Text;

namespace Enhanced.Services.Ebay
{
    public class EbayDigitalSignature
    {
        private static string[] signatureParameters = null!;

        public EbayDigitalSignature(bool hasBody = false)
        {
            if (hasBody)
            {
                signatureParameters = new string[] { "content-digest", "x-ebay-signature-key", "@method", "@path", "@authority" };
            }
            else
            {
                signatureParameters = new string[] { "x-ebay-signature-key", "@method", "@path", "@authority" };
            }
        }

        public string GetSignature(string privateKey, RestRequest request, Uri uri)
        {
            var signature = CreateSignature(request, uri);
            var signatureBase = Encoding.UTF8.GetBytes(signature);

            var key = PrivateKeyFactory.CreateKey(Convert.FromBase64String(privateKey));
            var signer = new Ed25519Signer();
            signer.Init(true, key);
            signer.BlockUpdate(signatureBase, 0, signatureBase.Length);

            return Convert.ToBase64String(signer.GenerateSignature(), Base64FormattingOptions.None);
        }

        private string CreateSignature(RestRequest request, Uri uri)
        {
            var stringBuilder = new StringBuilder();
            var requestHeaders = request.Parameters?.Where(x => x.Type == ParameterType.HttpHeader).ToList();

            if (requestHeaders != null && requestHeaders.Any())
            {
                foreach (var param in signatureParameters)
                {
                    stringBuilder.Append($"\"{param.ToLower()}\": ");

                    if (param.StartsWith("@"))
                    {
                        switch (param.ToLower())
                        {
                            case "@method":
                                stringBuilder.Append(request.Method.ToString().ToUpper());
                                break;

                            case "@path":
                                stringBuilder.Append(uri.AbsolutePath);
                                break;

                            case "@authority":
                                stringBuilder.Append(uri.Authority);
                                break;
                        }
                    }
                    else
                    {
                        var value = requestHeaders.FirstOrDefault(x => x.Name?.ToLower() == param.ToLower());

                        if (value is null)
                        {
                            throw new Exception("Header " + param + " not included in message");
                        }

                        stringBuilder.Append(value.Value);
                    }

                    stringBuilder.AppendLine();
                }
            }

            stringBuilder.Append("\"@signature-params\": ");
            stringBuilder.Append(GetSignatureInput());

            return stringBuilder.ToString().Replace("\r", string.Empty);
        }

        public string GetSignatureInput()
        {
            var stringBuilder = new StringBuilder($"(");

            foreach (var param in signatureParameters)
            {
                if (stringBuilder.ToString().EndsWith("("))
                {
                    stringBuilder.Append($"\"{param}\"");
                }
                else
                {
                    stringBuilder.Append($" \"{param}\"");
                }
            }

            stringBuilder.Append($");created={DateTimeOffset.UtcNow.ToUnixTimeSeconds()}");

            return stringBuilder.ToString().Replace("\r", string.Empty);
        }
    }
}
