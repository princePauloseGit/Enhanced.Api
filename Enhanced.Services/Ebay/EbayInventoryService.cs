using Enhanced.Models.EbayData;
using Enhanced.Models.Shared;
using System.Net;
using static Enhanced.Models.EbayData.EbayEnum;
using static Enhanced.Models.Shared.CommonEnum;

namespace Enhanced.Services.Ebay
{
    public class EbayInventoryService : EbayRequestService
    {
        public EbayInventoryService(ClientToken oauth, AccessToken token) : base(oauth, token) { }

        public virtual async Task<(List<OfferViewModel>, List<ErrorLog>)> GetOfferedListing(List<OfferParameters> offerParameters)
        {
            var listings = new List<OfferViewModel>();
            var errorLogs = new List<ErrorLog>();

            foreach (var offer in offerParameters)
            {
                try
                {
                    if (offer.Action!.ToUpper() == ListingAction.UPDATE.ToString())
                    {
                        await CreateAuthorizedRequestAsync(InventoryApiUrls.GetOffers(offer.SKU!), RestSharp.Method.GET);

                        var offersResponse = await ExecuteRequestAsync<GetOfferResponse>();

                        if (offersResponse?.offers != null)
                        {
                            var result = offersResponse.offers?.Select(s => new OfferViewModel(s, offer)).ToList();

                            foreach (var offerItem in result!)
                            {
                                errorLogs.Add(new ErrorLog(Marketplace.EBay, Sevarity.Information, "SKU", offer.SKU, Priority.Low, stackTrace: offerItem.Action));
                            }

                            listings.AddRange(result!);
                        }
                        else
                        {
                            errorLogs.Add(new ErrorLog(Marketplace.EBay, Sevarity.Warning, "SKU", offer.SKU, Priority.Low, stackTrace: "Not listed on eBay"));
                        }
                    }
                    else
                    {
                        listings.Add(new OfferViewModel
                        {
                            Sku = offer.SKU,
                            ListingId = string.Empty,
                            OfferId = string.Empty,
                            Action = offer.Action,
                        });
                    }
                }
                catch (Exception ex)
                {
                    errorLogs.Add(new ErrorLog(Marketplace.EBay, Sevarity.Error, "SKU", offer.SKU, Priority.High, ex.Message, ex.StackTrace));
                    continue;
                }
            }

            return (listings!, errorLogs!);
        }

        public async Task<(List<ActiveListing>, List<ErrorLog>)> BulkCreateListing(BulkInventoryParameters inventoryparams)
        {
            var errorLogs = new List<ErrorLog>();
            var listingCreatedSKUs = new List<string>();
            var createdOffers = new List<OfferPublish>();
            var activeListings = new List<ActiveListing>();

            await CreateListing(inventoryparams?.Listing!, errorLogs, listingCreatedSKUs);

            await CreateOffer(inventoryparams?.Offers!, errorLogs, listingCreatedSKUs, createdOffers);

            await PublishOffer(errorLogs, createdOffers);

            await GetOffers(listingCreatedSKUs, activeListings, errorLogs);

            return (activeListings, errorLogs);
        }

        private async Task CreateListing(List<EbayInventory> listings, List<ErrorLog> errorLogs, List<string> listingCreatedSKUs)
        {
            if (listings?.Any() == true)
            {
                DecodeBase64ProductDescription(listings);

                var bulkInventoryItems = new List<BulkInventoryItem>();

                CreateBulkListingBatchOf25Items(listings, bulkInventoryItems);

                foreach (var bulkInventoryItem in bulkInventoryItems)
                {
                    try
                    {
                        errorLogs.Add(new ErrorLog(Marketplace.EBay, Sevarity.Information, "", "", Priority.Low, stackTrace: "Total items to create listing: " + bulkInventoryItem?.requests?.Count));

                        var inventoryItemResponses = await BulkCreateOrReplaceInventoryAsync(bulkInventoryItem!);

                        if (inventoryItemResponses != null)
                        {
                            foreach (var inventoryItemResponse in inventoryItemResponses.responses!)
                            {
                                if (CheckHttpStatusCode((HttpStatusCode)inventoryItemResponse.statusCode))
                                {
                                    listingCreatedSKUs.Add(inventoryItemResponse.sku!);
                                }

                                bool isError = inventoryItemResponse?.errors?.Any() == true;

                                var errorMessages = inventoryItemResponse?.errors?.Select(s => s.message?.Replace("\"", "")).ToList() ?? new List<string>()!;
                                errorLogs.Add(new ErrorLog(Marketplace.EBay, (isError ? Sevarity.Error : Sevarity.Information), "SKU", inventoryItemResponse?.sku, (isError ? Priority.High : Priority.Low), "Status Code : " + inventoryItemResponse?.statusCode, string.Join(",", errorMessages!).Replace("\"", "")));
                            }
                        }
                        else
                        {
                            var errors = inventoryItemResponses?.responses?.SelectMany(s => s.errors!).ToList();
                            var errorMessages = errors?.Select(s => s.message?.Replace("\"", "")).ToList() ?? new List<string>()!;

                            errorLogs.Add(new ErrorLog(Marketplace.EBay, Sevarity.Error, "", "", Priority.Low, "Failed to create listing for a batch", string.Join(",", errorMessages)));
                        }
                    }
                    catch (Exception ex)
                    {
                        errorLogs.Add(new ErrorLog(Marketplace.EBay, Sevarity.Error, "", "", Priority.High, ex.Message, ex.StackTrace));
                        continue;
                    }
                }
            }
        }

        private async Task CreateOffer(List<EbayOffer> offers, List<ErrorLog> errorLogs, List<string> listingCreatedSKUs, List<OfferPublish> createdOffers)
        {
            if (listingCreatedSKUs.Any() && offers?.Any() == true)
            {
                DecodeBase64OfferDescription(offers);

                var bulkOffers = new List<BulkOffer>();
                var eligibleToOffers = offers.Where(x => listingCreatedSKUs.Contains(x.sku!)).ToList();

                CreateBulkCreateOfferBatchOf25Items(eligibleToOffers!, bulkOffers);

                foreach (var bulkOffer in bulkOffers)
                {
                    try
                    {
                        errorLogs.Add(new ErrorLog(Marketplace.EBay, Sevarity.Information, "", "", Priority.Low, stackTrace: "Total items to create offer: " + bulkOffer?.requests?.Count));

                        var createOfferResponses = await BulkCreateOfferAsync(bulkOffer!);

                        if (createOfferResponses != null)
                        {
                            foreach (var offerResponse in createOfferResponses.responses!)
                            {
                                if (CheckHttpStatusCode((HttpStatusCode)offerResponse.statusCode))
                                {
                                    createdOffers.Add(new OfferPublish
                                    {
                                        offerId = offerResponse.offerId
                                    });
                                }

                                bool isError = offerResponse?.errors?.Any() == true;
                                var errorMessages = offerResponse?.errors?.Select(s => s.message?.Replace("\"", "")).ToList() ?? new List<string>()!;

                                errorLogs.Add(new ErrorLog(Marketplace.EBay, (isError ? Sevarity.Error : Sevarity.Information), "SKU", offerResponse?.sku, (isError ? Priority.High : Priority.Low), "Status Code : " + offerResponse?.statusCode, string.Join(",", errorMessages)));
                            }
                        }
                        else
                        {
                            var errors = createOfferResponses?.responses?.SelectMany(s => s?.errors!).ToList();
                            var errorMessages = errors?.Select(s => s.message?.Replace("\"", "")).ToList() ?? new List<string>()!;

                            errorLogs.Add(new ErrorLog(Marketplace.EBay, Sevarity.Error, "", "", Priority.Low, "Failed to create offer for a batch", string.Join(",", errorMessages)));
                        }
                    }
                    catch (Exception ex)
                    {
                        errorLogs.Add(new ErrorLog(Marketplace.EBay, Sevarity.Error, "Create Offer Batch", "", Priority.High, ex.Message, ex.StackTrace));
                        continue;
                    }
                }
            }
        }

        private async Task PublishOffer(List<ErrorLog> errorLogs, List<OfferPublish> createdOffers)
        {
            if (createdOffers?.Any() == true)
            {
                var bulkOfferPublishes = new List<BulkOfferPublish>();

                CreateBulkPublishOfferBatchOf25Items(createdOffers, bulkOfferPublishes);

                foreach (var bulkOfferPublish in bulkOfferPublishes)
                {
                    try
                    {
                        errorLogs.Add(new ErrorLog(Marketplace.EBay, Sevarity.Information, "", "", Priority.Low, stackTrace: "Total items to publish: " + bulkOfferPublish?.requests?.Count));

                        var publishOfferResponses = await BulkPublishOfferAsync(bulkOfferPublish!);

                        if (publishOfferResponses != null)
                        {
                            foreach (var publishOfferResponse in publishOfferResponses.responses!)
                            {
                                bool isError = publishOfferResponse?.errors?.Any() == true;
                                var errorMessages = publishOfferResponse?.errors?.Select(s => s.message).ToList() ?? new List<string?>();

                                if (!isError)
                                {
                                    errorLogs.Add(new ErrorLog(Marketplace.EBay, Sevarity.Information, "ListingId", publishOfferResponse?.listingId, Priority.Low, "Status Code : " + publishOfferResponse?.statusCode));
                                }
                                else
                                {
                                    errorLogs.Add(new ErrorLog(Marketplace.EBay, Sevarity.Error, "OfferId_SKU", publishOfferResponse?.offerId, Priority.High, "Status Code : " + publishOfferResponse?.statusCode, string.Join(",", errorMessages!)));
                                }
                            }
                        }
                        else
                        {
                            var errors = publishOfferResponses?.responses?.SelectMany(s => s?.errors!).ToList();
                            var errorMessages = errors?.Select(s => s?.message?.Replace("\"", "")).ToList() ?? new List<string>()!;

                            errorLogs.Add(new ErrorLog(Marketplace.EBay, Sevarity.Error, "Publish Offer Batch", "", Priority.Low, "Failed to publish offer for a batch", string.Join(",", errorMessages!).Replace("\"", "")));
                        }
                    }
                    catch (Exception ex)
                    {
                        errorLogs.Add(new ErrorLog(Marketplace.EBay, Sevarity.Error, "Publish Offer Batch", "", Priority.High, ex.Message, ex.StackTrace));
                        continue;
                    }
                }
            }
        }

        private async Task GetOffers(List<string> listingCreatedSKUs, List<ActiveListing> activeListings, List<ErrorLog> errorLogs)
        {
            if (listingCreatedSKUs.Any())
            {
                foreach (var sku in listingCreatedSKUs)
                {
                    try
                    {
                        var offersResponse = await GetOffersAsync(sku);

                        if (offersResponse?.offers?.Any(a => a.status == OfferStatus.PUBLISHED.ToString()) == true)
                        {
                            var offerResult = offersResponse?.offers?.Select(s => new ActiveListing
                            {
                                Sku = s.sku,
                                ListingId = s.listing?.listingId,
                                OfferId = s.offerId,
                            }).ToList();

                            activeListings.AddRange(offerResult!);

                            errorLogs.Add(new ErrorLog(Marketplace.EBay, Sevarity.Information, "SKU", sku, Priority.Low, "Created listing for"));
                        }
                        else
                        {
                            // Delete Inventory item if no offer exists.
                            var (isDeleted, errorResponse) = await DeleteInventoryItemAsync(sku!);

                            if (isDeleted)
                            {
                                var offer = offersResponse?.offers?.FirstOrDefault();

                                var findOffer = errorLogs.FirstOrDefault(x => x.RecordType == "OfferId_SKU" && x.RecordID == offer?.offerId);

                                if (findOffer != null)
                                {
                                    findOffer.RecordType = "SKU";
                                    findOffer.RecordID = offer!.sku;
                                }

                                errorLogs.Add(new ErrorLog(Marketplace.EBay, Sevarity.Warning, "SKU", sku, Priority.High, stackTrace: "Inventory Deleted: Please check error message"));
                            }
                            else
                            {
                                var errorMessages = errorResponse?.errors?.Select(s => s.message?.Replace("\"", "")).ToList();
                                errorLogs.Add(new ErrorLog(Marketplace.EBay, Sevarity.Error, "SKU", sku, Priority.High, "Failed to delete inventory", string.Join(",", errorMessages!).Replace("\"", "")));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        errorLogs.Add(new ErrorLog(Marketplace.EBay, Sevarity.Error, "SKU", sku, Priority.High, ex.Message, ex.StackTrace));
                        continue;
                    }
                }
            }
        }

        public async Task<(List<UpdateStatus>, List<ErrorLog>)> UpdateOffers(List<EbayOffer> offers)
        {
            var errorLogs = new List<ErrorLog>();
            var updateStatuses = new List<UpdateStatus>();

            if (offers?.Any() == true)
            {
                DecodeBase64OfferDescription(offers);

                foreach (var offer in offers)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(offer.offerId))
                        {
                            errorLogs.Add(new ErrorLog(Marketplace.EBay, Sevarity.Warning, "SKU", offer.sku, Priority.Low, stackTrace: "Offer Id is required to update offer"));
                            continue;
                        }

                        var (isUpdated, errorResponse) = await UpdateOffer(offer);

                        if (isUpdated)
                        {
                            updateStatuses.Add(new UpdateStatus
                            {
                                Sku = offer.sku,
                                Success = true,
                            });

                            errorLogs.Add(new ErrorLog(Marketplace.EBay, Sevarity.Information, "SKU", offer.sku, Priority.Low));
                        }
                        else
                        {
                            var errorMessages = errorResponse?.errors?.Select(s => s.message?.Replace("\"", "")).ToList() ?? new List<string>()!;

                            errorLogs.Add(new ErrorLog(Marketplace.EBay, Sevarity.Error, "SKU", offer.sku, Priority.Low, "Failed to update offer", string.Join(",", errorMessages)));
                        }
                    }
                    catch (Exception ex)
                    {
                        errorLogs.Add(new ErrorLog(Marketplace.EBay, Sevarity.Error, "SKU", offer.sku, Priority.High, ex.Message, ex.StackTrace));
                        continue;
                    }
                }
            }

            return (updateStatuses, errorLogs);
        }

        public async Task<(List<ActiveListing>, List<ErrorLog>)> BulkCreateGroupListing(GroupInventoryParameters groupInventoryParameters)
        {
            var errorLogs = new List<ErrorLog>();
            var listingCreatedSKUs = new List<string>();
            var createdOffers = new List<OfferPublish>();
            var activeListings = new List<ActiveListing>();

            await CreateListing(groupInventoryParameters?.Listing!, errorLogs, listingCreatedSKUs);

            await CreateOffer(groupInventoryParameters?.Offers!, errorLogs, listingCreatedSKUs, createdOffers);

            if (createdOffers?.Any() == true)
            {
                foreach (var inventoryItemGroup in groupInventoryParameters?.Groups!)
                {
                    inventoryItemGroup.description = CommonHelper.Base64Decode(inventoryItemGroup.description!);
                    inventoryItemGroup.title = CommonHelper.Base64Decode(inventoryItemGroup.title!);
                    inventoryItemGroup.aspects = GetAspects(inventoryItemGroup.aspects!);

                    errorLogs.Add(new ErrorLog(Marketplace.EBay, Sevarity.Information, "SKU", inventoryItemGroup.inventoryItemGroupKey, Priority.Low));

                    var (isCreated, errorResponse) = await CreateOrReplaceInventoryItemGroupAsync(inventoryItemGroup);

                    if (isCreated)
                    {
                        var publishOffersByInventory = new PublishOffersByInventoryGroup
                        {
                            marketplaceId = UkEbayMarketPlaceId,
                            inventoryItemGroupKey = inventoryItemGroup.inventoryItemGroupKey,
                        };

                        var publishOfferGroupResponse = await PublishOfferByInventoryItemGroupAsync(publishOffersByInventory);

                        if (!string.IsNullOrEmpty(publishOfferGroupResponse?.listingId))
                        {
                            activeListings.Add(new ActiveListing
                            {
                                Sku = inventoryItemGroup.inventoryItemGroupKey,
                                ListingId = publishOfferGroupResponse.listingId,
                                OfferId = string.Empty,
                            });

                            errorLogs.Add(new ErrorLog(Marketplace.EBay, Sevarity.Information, "SKU", inventoryItemGroup.inventoryItemGroupKey, Priority.Low));
                        }
                        else
                        {
                            var errors = new List<string>();
                            var errorMessages = publishOfferGroupResponse?.errors?.Select(s => s.message?.Replace("\"", "")).ToList();

                            if (errorMessages?.Any() == true)
                            {
                                errors.AddRange(errorMessages!);
                            }

                            errorLogs.Add(new ErrorLog(Marketplace.EBay, Sevarity.Error, "SKU", inventoryItemGroup.inventoryItemGroupKey, Priority.Low, "Failed to publish group", string.Join(",", errors).Replace("\"", "")));

                            // If group is not published then delete Group
                            var (isDeleted, errorsResponse) = await DeleteInventoryItemGroupAsync(inventoryItemGroup.inventoryItemGroupKey!);

                            if (isDeleted)
                            {
                                errorLogs.Add(new ErrorLog(Marketplace.EBay, Sevarity.Warning, "SKU", inventoryItemGroup.inventoryItemGroupKey, Priority.Low));
                            }
                            else
                            {
                                var errorMsgs = errorsResponse?.errors?.Select(s => s.message?.Replace("\"", "")).ToList();
                                errorLogs.Add(new ErrorLog(Marketplace.EBay, Sevarity.Error, "SKU", inventoryItemGroup.inventoryItemGroupKey, Priority.High, "Failed to delete inventory group", string.Join(",", errorMsgs!).Replace("\"", "")));
                            }

                            // If group is not published then delete inventory items
                            var errorResponses = await DeleteInventoryItems(inventoryItemGroup.variantSKUs!);

                            if (errorResponses?.Any() == true)
                            {
                                errorLogs.AddRange(errorResponses);
                            }
                        }
                    }
                    else
                    {
                        var errors = new List<string>();
                        var errorMessages = errorResponse?.errors?.Select(s => s.message?.Replace("\"", "")).ToList();

                        errorLogs.Add(new ErrorLog(Marketplace.EBay, Sevarity.Error, "SKU", inventoryItemGroup.inventoryItemGroupKey, Priority.Low, "Failed to create group", string.Join(",", errorMessages!).Replace("\"", "")));

                        // If group is not created then delete inventory items
                        var errosResponses = await DeleteInventoryItems(inventoryItemGroup.variantSKUs!);

                        if (errosResponses?.Any() == true)
                        {
                            errorLogs.AddRange(errosResponses);
                        }
                    }
                }

                await GetOffers(listingCreatedSKUs, activeListings, errorLogs);
            }

            return (activeListings, errorLogs);
        }

        public async Task<(List<EbayBulkUpdatePriceQuantityResponse>, List<ErrorLog>)> BulkUpdatePriceQuantity(List<InventoryPriceQuantity> inventoryPriceQuantities)
        {
            var errorLogs = new List<ErrorLog>();
            var bulkUpdateInventoryItems = new List<BulkUpdateInventoryItem>();
            var bulkUpdatePriceQuantityResponses = new List<EbayBulkUpdatePriceQuantityResponse>();

            CreateBulkUpdatePriceQuantityBatchOf25Items(inventoryPriceQuantities, bulkUpdateInventoryItems);

            foreach (var bulkUpdateInventoryItem in bulkUpdateInventoryItems)
            {
                try
                {
                    errorLogs.Add(new ErrorLog(Marketplace.EBay, Sevarity.Information, "Update Price & Quantity", "", Priority.Low, "Total Items : " + bulkUpdateInventoryItem?.requests?.Count));

                    var updateResponses = await BulkUpdatePriceQuantityAsync(bulkUpdateInventoryItem!);

                    if (updateResponses?.responses?.Any() == true)
                    {
                        foreach (var offerResponse in updateResponses?.responses!)
                        {
                            if (!string.IsNullOrEmpty(offerResponse.offerId))
                            {
                                bulkUpdatePriceQuantityResponses.Add(new EbayBulkUpdatePriceQuantityResponse
                                {
                                    statusCode = offerResponse.statusCode,
                                    offerId = offerResponse.offerId,
                                    sku = offerResponse.sku,
                                });

                                bool isError = offerResponse?.errors?.Any() == true;
                                var errorMessages = offerResponse?.errors?.Select(s => s.message?.Replace("\"", "")).ToList() ?? new List<string>()!;

                                errorLogs.Add(new ErrorLog(Marketplace.EBay, (isError ? Sevarity.Error : Sevarity.Information), "SKU", offerResponse?.sku, (isError ? Priority.High : Priority.Low), string.Join(",", errorMessages!).Replace("\"", "")));
                            }
                        }
                    }
                    else
                    {
                        var errors = updateResponses?.responses?.SelectMany(s => s.errors!).ToList();
                        var errorMessages = errors?.Select(s => s.message?.Replace("\"", "")).ToList() ?? new List<string>()!;

                        errorLogs.Add(new ErrorLog(Marketplace.EBay, Sevarity.Error, "", "", Priority.Low, "Failed to bulk update Price & Quantity", string.Join(",", errors!).Replace("\"", "")));
                    }
                }
                catch (Exception ex)
                {
                    errorLogs.Add(new ErrorLog(Marketplace.EBay, Sevarity.Error, "Bulk Update Price & Quantity", "", Priority.High, ex.Message, ex.StackTrace));
                    continue;
                }
            }

            return (bulkUpdatePriceQuantityResponses, errorLogs);
        }

        private static void CreateBulkListingBatchOf25Items(List<EbayInventory> listing, List<BulkInventoryItem> bulkInventoryItems)
        {
            int total = 0;
            int batchSize = 25;

            while (total < listing?.Count)
            {
                var inventoryBatch = listing?.Skip(total).Take(batchSize).ToList();

                bulkInventoryItems.Add(new BulkInventoryItem
                {
                    requests = inventoryBatch
                });

                total += batchSize;
            }
        }

        private static void CreateBulkCreateOfferBatchOf25Items(List<EbayOffer> eligibleToOffers, List<BulkOffer> bulkOffers)
        {
            int total = 0;
            int batchSize = 25;

            while (total < eligibleToOffers?.Count)
            {
                var offers = eligibleToOffers?.Skip(total).Take(batchSize).ToList();

                bulkOffers.Add(new BulkOffer
                {
                    requests = offers
                });

                total += batchSize;
            }
        }

        private static void CreateBulkPublishOfferBatchOf25Items(List<OfferPublish> createdOffers, List<BulkOfferPublish> bulkOfferPublishes)
        {
            int total = 0;
            int batchSize = 25;

            while (total < createdOffers?.Count)
            {
                var offerPublishes = createdOffers?.Skip(total).Take(batchSize).ToList();

                bulkOfferPublishes.Add(new BulkOfferPublish
                {
                    requests = offerPublishes
                });

                total += batchSize;
            }
        }

        private static void CreateBulkUpdatePriceQuantityBatchOf25Items(List<InventoryPriceQuantity> priceQuantityInventories, List<BulkUpdateInventoryItem> bulkUpdateInventoryItems)
        {
            int total = 0;
            int batchSize = 25;

            while (total < priceQuantityInventories?.Count)
            {
                var inventoryPriceQuantities = priceQuantityInventories?.Skip(total).Take(batchSize).ToList();

                bulkUpdateInventoryItems.Add(new BulkUpdateInventoryItem
                {
                    requests = inventoryPriceQuantities
                });

                total += batchSize;
            }
        }

        public async Task<List<ErrorLog>> DeleteInventoryItems(List<string> sKUs)
        {
            var errorLogs = new List<ErrorLog>();

            foreach (var sku in sKUs)
            {
                try
                {
                    var (isDeleted, errorResponse) = await DeleteInventoryItemAsync(sku);

                    if (isDeleted)
                    {
                        errorLogs.Add(new ErrorLog(Marketplace.EBay, Sevarity.Warning, "SKU", sku, Priority.Low, "Removed from ebay"));
                    }
                    else
                    {
                        var errorMessages = errorResponse?.errors?.Select(s => s.message?.Replace("\"", "")).ToList();
                        errorLogs.Add(new ErrorLog(Marketplace.EBay, Sevarity.Error, "SKU", sku, Priority.High, "Failed to delete inventory", string.Join(",", errorMessages!).Replace("\"", "")));
                    }
                }
                catch (Exception ex)
                {
                    errorLogs.Add(new ErrorLog(Marketplace.EBay, Sevarity.Error, "SKU", sku, Priority.Low, ex.Message, ex.StackTrace));
                    continue;
                }
            }

            return errorLogs;
        }

        public async Task<(List<UpdateStatus>, List<ErrorLog>)> UpdateListingWithOffer(BulkInventoryParameters inventoryparams)
        {
            var errorLogs = new List<ErrorLog>();
            var updateStatuses = new List<UpdateStatus>();

            var inventoryErrors = await BulkUpdateInventory(inventoryparams.Listing!);

            var (offerStatuses, offerErrors) = await UpdateOffers(inventoryparams.Offers!);

            if (inventoryErrors.Any())
            {
                errorLogs.AddRange(inventoryErrors);
            }

            if (offerErrors.Any())
            {
                errorLogs.AddRange(offerErrors);
            }

            if (offerStatuses.Any())
            {
                updateStatuses.AddRange(offerStatuses);
            }

            return (updateStatuses, errorLogs);
        }

        private async Task<List<ErrorLog>> BulkUpdateInventory(List<EbayInventory> inventories)
        {
            var errorLogs = new List<ErrorLog>();

            if (inventories?.Any() == true)
            {
                var bulkInventoryItems = new List<BulkInventoryItem>();

                DecodeBase64ProductDescription(inventories);
                CreateBulkListingBatchOf25Items(inventories, bulkInventoryItems);

                foreach (var bulkInventoryItem in bulkInventoryItems)
                {
                    try
                    {
                        errorLogs.Add(new ErrorLog(Marketplace.EBay, Sevarity.Information, "", "", Priority.Low, "Total items to update inventory : " + bulkInventoryItem?.requests?.Count));

                        var inventoryItemResponses = await BulkCreateOrReplaceInventoryAsync(bulkInventoryItem!);

                        if (inventoryItemResponses != null)
                        {
                            foreach (var inventoryItemResponse in inventoryItemResponses.responses!)
                            {
                                bool isError = inventoryItemResponse?.errors?.Any() == true;
                                var errorMessages = inventoryItemResponse?.errors?.Select(s => s.message?.Replace("\"", "")).ToList() ?? new List<string>()!;

                                errorLogs.Add(new ErrorLog(Marketplace.EBay, (isError ? Sevarity.Error : Sevarity.Information), "SKU", inventoryItemResponse?.sku, (isError ? Priority.High : Priority.Low), "Status Code : " + inventoryItemResponse?.statusCode, string.Join(",", errorMessages!).Replace("\"", "")));
                            }
                        }
                        else
                        {
                            var errors = inventoryItemResponses?.responses?.SelectMany(s => s.errors!).ToList();
                            var errorMessages = errors?.Select(s => s.message?.Replace("\"", "")).ToList() ?? new List<string>()!;

                            errorLogs.Add(new ErrorLog(Marketplace.EBay, Sevarity.Error, "Update Inventory Batch", "", Priority.Low, "Failed to update listing for a batch", string.Join(",", errorMessages)));
                        }
                    }
                    catch (Exception ex)
                    {
                        errorLogs.Add(new ErrorLog(Marketplace.EBay, Sevarity.Error, "Update Inventory Batch", "", Priority.High, ex.Message, ex.StackTrace));
                        continue;
                    }
                }
            }

            return errorLogs;
        }

        private static void DecodeBase64ProductDescription(List<EbayInventory> listings)
        {
            foreach (var listing in listings)
            {
                listing.product!.description = CommonHelper.Base64Decode(listing.product?.description!);
                listing.product!.title = CommonHelper.Base64Decode(listing.product?.title!);

                if (listing?.product?.aspects != null && listing?.product?.aspects.Any() == true)
                {
                    listing.product.aspects = GetAspects(listing?.product?.aspects!);
                }

                ConvertImageUrls(listing!);
            }
        }

        private static void ConvertImageUrls(EbayInventory listing)
        {
            if (listing!.product?.imageUrls != null)
            {
                var updatedImageUrls = new List<string>();

                foreach (var imageUrl in listing!.product?.imageUrls!)
                {
                    string dateTime = DateTime.Now.ToString("yyyyMMdd-HHmmss");

                    var imageUrlWithTimestamp = string.Concat(imageUrl, "?t=", dateTime);

                    updatedImageUrls.Add(imageUrlWithTimestamp);
                }

                listing!.product.imageUrls = updatedImageUrls;
            }
        }

        private static Dictionary<string, List<string>> GetAspects(Dictionary<string, List<string>> aspectsEncoded)
        {
            var aspects = new Dictionary<string, List<string>>();

            foreach (var aspect in aspectsEncoded)
            {
                var aspectValues = new List<string>();

                foreach (var aspectValueItem in aspect.Value!)
                {
                    var aspectValue = CommonHelper.Base64Decode(aspectValueItem!);

                    aspectValues.Add(aspectValue);
                }

                aspects.Add(aspect.Key, aspectValues);
            }

            return aspects;
        }

        private static void DecodeBase64OfferDescription(List<EbayOffer> offers)
        {
            foreach (var offer in offers!)
            {
                offer.listingDescription = CommonHelper.Base64Decode(offer.listingDescription!);
            }
        }

        public async Task<BulkInventoryItemResponses> BulkCreateOrReplaceInventoryAsync(BulkInventoryItem bulkInventoryItem)
        {
            await CreateAuthorizedRequestAsync(InventoryApiUrls.BulkCreateOrReplaceInventoryItemUrl, RestSharp.Method.POST, bulkInventoryItem);
            return await ExecuteRequestAsync<BulkInventoryItemResponses>();
        }

        public async Task<Responses> BulkCreateOfferAsync(BulkOffer bulkOffer)
        {
            await CreateAuthorizedRequestAsync(InventoryApiUrls.BulkCreateOffer, RestSharp.Method.POST, bulkOffer);
            return await ExecuteRequestAsync<Responses>();
        }

        public async Task<Responses> BulkPublishOfferAsync(BulkOfferPublish bulkOfferPublish)
        {
            await CreateAuthorizedRequestAsync(InventoryApiUrls.BulkPublishOffer, RestSharp.Method.POST, bulkOfferPublish);
            return await ExecuteRequestAsync<Responses>();
        }

        public async Task<GetOfferResponse> GetOffersAsync(string sku)
        {
            await CreateAuthorizedRequestAsync(InventoryApiUrls.GetOffers(sku!), RestSharp.Method.GET);
            return await ExecuteRequestAsync<GetOfferResponse>();
        }

        public async Task<(bool, EbayErrorResponse)> DeleteInventoryItemAsync(string sku)
        {
            await CreateAuthorizedRequestAsync(InventoryApiUrls.DeleteInventoryItem(sku), RestSharp.Method.DELETE);
            return await ExecuteRequestNoneAsync();
        }

        public async Task<(bool, EbayErrorResponse)> CreateOrReplaceInventoryItemGroupAsync(InventoryItemGroup inventoryItemGroup)
        {
            await CreateAuthorizedRequestAsync(InventoryApiUrls.CreateOrReplaceInventoryItemGroupUrl(inventoryItemGroup.inventoryItemGroupKey!), RestSharp.Method.PUT, inventoryItemGroup);
            return await ExecuteRequestNoneAsync();
        }

        public async Task<EbayPublishOfferResponse> PublishOfferByInventoryItemGroupAsync(PublishOffersByInventoryGroup publishOffersByInventory)
        {
            await CreateAuthorizedRequestAsync(InventoryApiUrls.PublishByInventoryItemGroup, RestSharp.Method.POST, publishOffersByInventory);
            return await ExecuteRequestAsync<EbayPublishOfferResponse>();
        }

        public async Task<(bool, EbayErrorResponse)> DeleteInventoryItemGroupAsync(string inventoryItemGroupKey)
        {
            await CreateAuthorizedRequestAsync(InventoryApiUrls.DeleteInventoryItemGroup(inventoryItemGroupKey), RestSharp.Method.DELETE);
            return await ExecuteRequestNoneAsync();
        }

        public async Task<Responses> BulkUpdatePriceQuantityAsync(BulkUpdateInventoryItem updateInventoryItem)
        {
            await CreateAuthorizedRequestAsync(InventoryApiUrls.BulkUpdatePriceQuantity, RestSharp.Method.POST, updateInventoryItem);
            return await ExecuteRequestAsync<Responses>();
        }

        public async Task<(bool, EbayErrorResponse)> UpdateOffer(EbayOffer offer)
        {
            await CreateAuthorizedRequestAsync(InventoryApiUrls.UpdateOffer(offer.offerId!), RestSharp.Method.PUT, offer);
            return await ExecuteRequestNoneAsync();
        }

        public async Task<EbayInventory> GetInventoryItem(string sku)
        {
            await CreateAuthorizedRequestAsync(InventoryApiUrls.GetInventoryItem(sku), RestSharp.Method.GET);
            return await ExecuteRequestAsync<EbayInventory>();
        }
    }
}
