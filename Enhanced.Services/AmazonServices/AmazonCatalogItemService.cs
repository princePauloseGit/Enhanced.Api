using Enhanced.Models.AmazonData;
using Enhanced.Models.Shared;
using static Enhanced.Models.AmazonData.AmazonEnum;
using static Enhanced.Models.Shared.CommonEnum;

namespace Enhanced.Services.AmazonServices
{
    public class AmazonCatalogItemService : AmazonRequestService
    {
        public AmazonCatalogItemService(AmazonCredential amazonCredential) : base(amazonCredential) { }

        public async Task<(List<ASINItemDimensions>, List<ErrorLog>)> SearchCatalogItem(ParameterCatalogItemASIN parameterCatalogItemASIN, string merchantId)
        {
            List<ASINItemDimensions>? dimensions = new();
            var errorLogs = new List<ErrorLog>();
            var skuBatches = new List<List<string>>();

            CreateBatchOf20Items(parameterCatalogItemASIN.SKUs!, skuBatches);

            foreach (var skuBatch in skuBatches)
            {
                ParameterCatalogItem parameterCatalogItem = new();

                if (parameterCatalogItem == null || parameterCatalogItem.marketplaceIds == null || parameterCatalogItem.marketplaceIds.Count == 0)
                {
                    parameterCatalogItem!.marketplaceIds!.Add(AmazonCredential.MarketPlace!.Id!);
                }

                foreach (var sku in skuBatch)
                {
                    parameterCatalogItem.identifiers.Add(sku);
                }

                parameterCatalogItem.identifiersType = IdentifiersType.SKU;
                parameterCatalogItem.sellerId = merchantId;
                parameterCatalogItem.pageSize = 20;
                parameterCatalogItem!.includedData!.Add(IncludedData.dimensions); // For item dimensions only
                parameterCatalogItem!.includedData!.Add(IncludedData.identifiers); // For item SKU

                var parameters = parameterCatalogItem.GetParameters();

                await CreateAuthorizedRequestAsync(CatalogApiUrls.SearchCatalogItems202204, RestSharp.Method.GET, parameters);

                var response = await ExecuteRequestAsync<ItemSearchResults>(RateLimitType.CatalogItems20220401_SearchCatalogItems);

                if (response.Items != null && response.Items.Any())
                {
                    foreach (var item in response.Items)
                    {
                        try
                        {
                            errorLogs.Add(new ErrorLog(Marketplace.Appeagle, Sevarity.Information, "Package Dimension", "", Priority.Low, "Total Items: " + response?.NumberOfResults));

                            var itemIdentifier = item.Identifiers?.SelectMany(s => s?.Identifiers!).ToList();
                            var itemSKUs = itemIdentifier?.Where(x => x?.IdentifierType == IdentifiersType.SKU.ToString()).ToList();

                            // Get only FBA item SKU ending with 'AF'
                            var itemSku = itemSKUs?.FirstOrDefault(x => x?.Identifier?[((int)x?.Identifier?.Length! - 2)..]?.ToUpper() == "AF")?.Identifier;

                            var packageDimension = item.Dimensions?.Select(x => x.Package).FirstOrDefault();

                            if (packageDimension != null)
                            {
                                if (packageDimension?.Weight?.Unit == WeightUnit.pounds.ToString())
                                {
                                    packageDimension!.Weight.Unit = WeightUnit.kilograms.ToString();
                                    packageDimension!.Weight.Value = Math.Round((decimal)packageDimension!.Weight.Value! * 0.45359237M, 3);
                                }

                                if (packageDimension?.Height?.Unit == MeasuringUnit.inches.ToString())
                                {
                                    packageDimension.Height!.Unit = MeasuringUnit.centimeters.ToString();
                                    packageDimension.Height!.Value = Math.Round((decimal)packageDimension!.Height.Value! * 2.54M, 3);
                                }

                                if (packageDimension?.Length?.Unit == MeasuringUnit.inches.ToString())
                                {
                                    packageDimension.Length!.Unit = MeasuringUnit.centimeters.ToString();
                                    packageDimension.Length!.Value = Math.Round((decimal)packageDimension!.Length.Value! * 2.54M, 3);
                                }

                                if (packageDimension?.Width?.Unit == MeasuringUnit.inches.ToString())
                                {
                                    packageDimension.Width!.Unit = MeasuringUnit.centimeters.ToString();
                                    packageDimension.Width!.Value = Math.Round((decimal)packageDimension!.Width.Value! * 2.54M, 3);
                                }

                                dimensions.Add(new ASINItemDimensions
                                {
                                    ASIN = item.Asin,
                                    SKU = itemSku,
                                    WeightInKg = new Dimension
                                    {
                                        Unit = packageDimension?.Weight?.Unit ?? string.Empty,
                                        Value = packageDimension?.Weight?.Value ?? 0
                                    },
                                    Width = new Dimension
                                    {
                                        Unit = packageDimension?.Width?.Unit ?? string.Empty,
                                        Value = packageDimension?.Width?.Value ?? 0
                                    },
                                    Height = new Dimension
                                    {
                                        Unit = packageDimension?.Height?.Unit ?? string.Empty,
                                        Value = packageDimension?.Height?.Value ?? 0
                                    },
                                    Length = new Dimension
                                    {
                                        Unit = packageDimension?.Length?.Unit ?? string.Empty,
                                        Value = packageDimension?.Length?.Value ?? 0
                                    }
                                });

                                errorLogs.Add(new ErrorLog(Marketplace.Appeagle, Sevarity.Information, "SKU", itemSku, Priority.Low, "Package Dimension", "ASIN: " + item.Asin));
                            }
                            else
                            {
                                errorLogs.Add(new ErrorLog(Marketplace.Appeagle, Sevarity.Warning, "SKU", itemSku, Priority.High, "Package Dimension not found"));
                            }
                        }
                        catch (Exception ex)
                        {
                            errorLogs.Add(new ErrorLog(Marketplace.Appeagle, Sevarity.Error, "Package Dimension", "", Priority.High, ex.Message, ex.StackTrace));
                            continue;
                        }
                    }
                }
                else
                {
                    errorLogs.Add(new ErrorLog(Marketplace.Appeagle, Sevarity.Warning, "Package Dimension", "", Priority.High, "Item response not found"));
                }
            }

            return (dimensions, errorLogs);
        }

        private static void CreateBatchOf20Items(List<string> lstSKU, List<List<string>> skuBatches)
        {
            int total = 0;
            int batchSize = 20;

            while (total < lstSKU?.Count)
            {
                var asinBatch = lstSKU?.Skip(total).Take(batchSize).ToList();

                skuBatches.Add(asinBatch!);

                total += batchSize;
            }
        }
    }
}
