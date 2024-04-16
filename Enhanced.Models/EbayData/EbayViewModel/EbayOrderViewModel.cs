using Enhanced.Models.Shared;
using static Enhanced.Models.EbayData.EbayEnum;
using static Enhanced.Models.EbayData.EbayOrderList;

namespace Enhanced.Models.EbayData.EbayViewModel
{
    public class OrderList
    {
        public string? OrderId { get; set; }
        public string? OrderPaymentStatus { get; set; }
        public string? CreationDate { get; set; }
        public string? OrderFulfillmentStatus { get; set; }
        public string? OrderPaymentMethod { get; set; }
        public string? OrderBatchValue { get; set; }
        public decimal? ShippingPrice { get; set; }
        public string? ShippingAgent { get; set; }
        public string? ShippingServiceCode { get; set; }
        public bool HasAddress { get; set; }

        public List<Orderline>? OrderLines { get; set; }
        public List<FulfillmentStartInstruction>? FulfillmentStartInstructions { get; set; }

        public OrderList(EbayOrderList.EbayOrder ebayOrder)
        {
            OrderId = ebayOrder.orderId ?? string.Empty;
            OrderPaymentStatus = ebayOrder.orderPaymentStatus ?? string.Empty;
            CreationDate = ebayOrder.creationDate ?? string.Empty;
            OrderFulfillmentStatus = ebayOrder.orderFulfillmentStatus ?? string.Empty;
            OrderPaymentMethod = ebayOrder.paymentSummary!.payments!.FirstOrDefault()!.paymentMethod;
            ShippingPrice = ebayOrder.lineItems?.FirstOrDefault()?.deliveryCost?.shippingCost?.value;
            ShippingServiceCode = ebayOrder?.fulfillmentStartInstructions?.FirstOrDefault()?.shippingStep?.shippingServiceCode;

            var shippingServiceCodes = Enum.GetNames(typeof(EbayShippingServiceCode)).ToList();

            var contactAddress = ebayOrder?.fulfillmentStartInstructions?.FirstOrDefault()?.shippingStep?.shipTo?.contactAddress;

            if (contactAddress?.countryCode != Constant.COUNTRY_CODE_GB ||
                contactAddress.postalCode!.StartsWith(Constant.POSTAL_CODE_GY) ||
                contactAddress!.postalCode.StartsWith(Constant.POSTAL_CODE_JE))
            {
                OrderBatchValue = Constant.ORDERBATCH_MANUAL;
            }
            else if ((ShippingPrice >= 0.01m && ShippingPrice <= 2.00m) || shippingServiceCodes.Contains(ShippingServiceCode!))
            {
                OrderBatchValue = Constant.ORDERBATCH_POST;
            }
            else
            {
                OrderBatchValue = Constant.ORDERBATCH_CARRIER;
            }

            OrderLines = ebayOrder?.lineItems?.Select(s => new Orderline(s)).ToList();
            FulfillmentStartInstructions = ebayOrder?.fulfillmentStartInstructions?.Select(s => new FulfillmentStartInstruction(s, ebayOrder.buyer!.buyerRegistrationAddress!)).ToList();
            HasAddress = !string.IsNullOrEmpty(contactAddress?.addressLine1) || !string.IsNullOrEmpty(contactAddress?.addressLine2);
        }

        public class Orderline
        {
            public string? lineItemId { get; set; }
            public string? legacyItemId { get; set; }
            public int? quantity { get; set; }
            public string? sku { get; set; }
            public LineItemCost? LineItemCost { get; set; }

            public Orderline(EbayOrderList.LineItem lineItem)
            {
                lineItemId = lineItem.lineItemId ?? string.Empty;
                legacyItemId = lineItem.legacyItemId ?? string.Empty;
                quantity = lineItem.quantity;
                sku = lineItem.sku ?? string.Empty;
                LineItemCost = lineItem.discountedLineItemCost != null
                                ? new LineItemCost(lineItem.discountedLineItemCost!)
                                : new LineItemCost(lineItem.lineItemCost!);
            }
        }

        public class LineItemCost
        {
            public string? Currency { get; set; }
            public decimal? Value { get; set; }

            public LineItemCost(EbayOrderList.Charges amount)
            {
                Currency = amount.currency;
                Value = amount.value;
            }
        }

        public class FulfillmentStartInstruction
        {
            public string? FulfillmentInstructionsType { get; set; }
            public ShippingStep? ShippingStep { get; set; }

            public FulfillmentStartInstruction(EbayOrderList.FulfillmentStartInstruction fulfillmentStart, BuyerRegistrationAddress buyerRegistrationAddress = null!)
            {
                FulfillmentInstructionsType = fulfillmentStart.fulfillmentInstructionsType ?? string.Empty;

                ShippingStep = new ShippingStep(fulfillmentStart, buyerRegistrationAddress);
            }
        }

        public class ShippingStep
        {
            public string? shippingCarrierCode { get; set; }
            public string? shippingServiceCode { get; set; }
            public ShipTo? shipTo { get; set; }
            public BillTo? billTo { get; set; }
            public SellTo? SellTo { get; set; }
            public string? shipToReferenceId { get; set; }

            public ShippingStep(EbayOrderList.FulfillmentStartInstruction startInstruction, BuyerRegistrationAddress buyerRegistrationAddress = null!)
            {
                shippingCarrierCode = startInstruction.shippingStep!.shippingCarrierCode ?? string.Empty;
                shippingServiceCode = startInstruction.shippingStep!.shippingServiceCode ?? string.Empty;

                shipTo = new ShipTo(startInstruction, buyerRegistrationAddress);
                billTo = new BillTo(startInstruction, buyerRegistrationAddress);
                SellTo = new SellTo(startInstruction, buyerRegistrationAddress);

                shipToReferenceId = startInstruction.shippingStep!.shipToReferenceId ?? string.Empty;
            }
        }

        public class ShipTo
        {
            public string? CompanyName { get; set; }
            public ContactAddress? ContactAddress { get; set; }
            public string? Email { get; set; }
            public string? FullName { get; set; }
            public PrimaryPhone? PrimaryPhone { get; set; }

            public ShipTo(EbayOrderList.FulfillmentStartInstruction startInstruction, BuyerRegistrationAddress buyerRegistrationAddress = null!)
            {
                var ship = startInstruction?.shippingStep?.shipTo;
                string shipToReferenceId = (startInstruction!.ebaySupportedFulfillment ? startInstruction?.shippingStep!.shipToReferenceId : string.Empty) ?? string.Empty;

                CompanyName = ship?.companyName ?? string.Empty;
                Email = (startInstruction!.ebaySupportedFulfillment ? buyerRegistrationAddress?.email : ship?.email) ?? string.Empty;
                FullName = (startInstruction!.ebaySupportedFulfillment ? buyerRegistrationAddress?.fullName : ship?.fullName) ?? string.Empty;
                ContactAddress = new ContactAddress(ship?.contactAddress!, shipToReferenceId);
                PrimaryPhone = new PrimaryPhone(ship?.primaryPhone!);
            }
        }

        public class BillTo
        {
            public string? CompanyName { get; set; }
            public ContactAddress? ContactAddress { get; set; }
            public string? Email { get; set; }
            public string? FullName { get; set; }
            public PrimaryPhone? PrimaryPhone { get; set; }

            public BillTo(EbayOrderList.FulfillmentStartInstruction startInstruction, BuyerRegistrationAddress buyerRegistrationAddress = null!)
            {
                var ship = startInstruction?.shippingStep?.shipTo;
                var phoneNumber = (startInstruction!.ebaySupportedFulfillment ? buyerRegistrationAddress?.primaryPhone : ship?.primaryPhone);

                CompanyName = ship?.companyName ?? string.Empty;
                Email = (startInstruction!.ebaySupportedFulfillment ? buyerRegistrationAddress?.email : ship?.email) ?? string.Empty;
                FullName = (startInstruction!.ebaySupportedFulfillment ? buyerRegistrationAddress?.fullName : ship?.fullName) ?? string.Empty;
                PrimaryPhone = new PrimaryPhone(phoneNumber!);

                if (startInstruction!.ebaySupportedFulfillment)
                {
                    ContactAddress = new ContactAddress(buyerRegistrationAddress!);
                }
                else
                {
                    ContactAddress = new ContactAddress(ship?.contactAddress!, string.Empty);
                }
            }
        }

        public class SellTo
        {
            public string? CompanyName { get; set; }
            public ContactAddress? ContactAddress { get; set; }
            public string? Email { get; set; }
            public string? FullName { get; set; }
            public PrimaryPhone? PrimaryPhone { get; set; }

            public SellTo(EbayOrderList.FulfillmentStartInstruction startInstruction, BuyerRegistrationAddress buyerRegistrationAddress = null!)
            {
                var ship = startInstruction?.shippingStep?.shipTo;
                var phoneNumber = (startInstruction!.ebaySupportedFulfillment ? buyerRegistrationAddress?.primaryPhone : ship?.primaryPhone);

                CompanyName = ship?.companyName ?? string.Empty;
                Email = (startInstruction!.ebaySupportedFulfillment ? buyerRegistrationAddress?.email : ship?.email) ?? string.Empty;
                FullName = (startInstruction!.ebaySupportedFulfillment ? buyerRegistrationAddress?.fullName : ship?.fullName) ?? string.Empty;
                PrimaryPhone = new PrimaryPhone(phoneNumber!);

                if (startInstruction!.ebaySupportedFulfillment)
                {
                    ContactAddress = new ContactAddress(startInstruction.finalDestinationAddress!);
                }
                else
                {
                    ContactAddress = new ContactAddress(ship?.contactAddress!, string.Empty);
                }
            }
        }

        public class ContactAddress
        {
            public string? AddressLine1 { get; set; }
            public string? AddressLine2 { get; set; }
            public string? City { get; set; }
            public string? CountryCode { get; set; }
            public string? County { get; set; }
            public string? PostalCode { get; set; }
            public string? StateOrProvince { get; set; }

            // Ship To Address: Buyer Registraion Address
            public ContactAddress(EbayOrderList.ContactAddress contactAddress, string shipToReferenceId = "")
            {
                if (!string.IsNullOrEmpty(shipToReferenceId))
                {
                    AddressLine1 = string.Concat("REF# ", shipToReferenceId);
                    AddressLine2 = contactAddress.addressLine1 ?? string.Empty;
                }
                else
                {
                    AddressLine1 = contactAddress!.addressLine1 ?? string.Empty;
                    AddressLine2 = contactAddress!.addressLine2 ?? string.Empty;
                }

                City = contactAddress!.city ?? string.Empty;
                CountryCode = contactAddress!.countryCode ?? string.Empty;
                County = contactAddress!.county ?? string.Empty;
                PostalCode = contactAddress!.postalCode ?? string.Empty;
                StateOrProvince = contactAddress!.stateOrProvince ?? string.Empty;
            }

            // Bill To Address: Buyer Registraion Address
            public ContactAddress(BuyerRegistrationAddress buyerRegistrationAddress = null!)
            {
                AddressLine1 = buyerRegistrationAddress.contactAddress!.addressLine1 ?? string.Empty;
                AddressLine2 = buyerRegistrationAddress.contactAddress!.addressLine2 ?? string.Empty;
                City = buyerRegistrationAddress.contactAddress!.city ?? string.Empty;
                CountryCode = buyerRegistrationAddress.contactAddress!.countryCode ?? string.Empty;
                County = buyerRegistrationAddress.contactAddress!.county ?? string.Empty;
                PostalCode = buyerRegistrationAddress.contactAddress!.postalCode ?? string.Empty;
                StateOrProvince = buyerRegistrationAddress.contactAddress!.stateOrProvince ?? string.Empty;
            }

            // Sell To Address: Final Destination Address
            public ContactAddress(FinalDestinationAddress finalDestinationAddress = null!)
            {
                AddressLine1 = finalDestinationAddress.addressLine1 ?? string.Empty;
                AddressLine2 = finalDestinationAddress.addressLine2 ?? string.Empty;
                City = finalDestinationAddress.city ?? string.Empty;
                CountryCode = finalDestinationAddress.countryCode ?? string.Empty;
                County = finalDestinationAddress.county ?? string.Empty;
                PostalCode = finalDestinationAddress.postalCode ?? string.Empty;
                StateOrProvince = finalDestinationAddress.stateOrProvince ?? string.Empty;
            }
        }

        public class PrimaryPhone
        {
            public string? PhoneNumber { get; set; }

            public PrimaryPhone(EbayOrderList.PrimaryPhone primaryPhone)
            {
                PhoneNumber = primaryPhone?.phoneNumber ?? string.Empty;
            }
        }
    }
}
