using Enhanced.Models.Shared;

namespace Enhanced.Models.BQData.BQDataViewModel
{
    public class BQOrderListViewModel
    {
        public int? NextPage { get; set; }
        public List<OrderList>? Orders { get; set; }
        public List<ErrorLog>? ErrorLogs { get; set; } = new List<ErrorLog>();        
    }

    public class OrderList
    {
        public string? OrderId { get; set; }
        public string? CustomerNotificationEmail { get; set; }
        public string? PaymentType { get; set; }
        public Customer? Customer { get; set; }
        public List<Orderline>? OrderLines { get; set; }
        public Decimal ShippingPrice { get; set; }
        public string? ShippingTypeLabel { get; set; }

        public OrderList(BQOrderList.OrderList orderList)
        {
            OrderId = orderList.order_id ?? string.Empty;
            CustomerNotificationEmail = orderList.customer_notification_email ?? string.Empty;
            PaymentType = orderList.payment_type ?? string.Empty;
            Customer = new Customer(orderList.customer!);
            OrderLines = orderList.order_lines?.Select(s => new Orderline(s)).ToList();
            ShippingPrice = orderList.shipping_price;
            ShippingTypeLabel = orderList.shipping_type_label ?? string.Empty;
        }
    }

    public class Customer
    {
        public string? Civility { get; set; }
        public string? CustomerId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Locale { get; set; }
        public CustomerShippingAddress? ShippingAddress { get; set; }
        public CustomerBillingAddress? BillingAddress { get; set; }

        public Customer(BQOrderList.Customer customer)
        {
            CustomerId = customer?.customer_id ?? string.Empty;
            Civility = customer?.civility ?? string.Empty;
            FirstName = customer?.firstname ?? string.Empty;
            LastName = customer?.lastname ?? string.Empty;
            Locale = customer?.locale ?? string.Empty;
            BillingAddress = new CustomerBillingAddress(customer?.billing_address!);
            ShippingAddress = new CustomerShippingAddress(customer?.shipping_address!);
        }
    }

    public class CustomerShippingAddress
    {
        public string? City { get; set; }
        public string? Civility { get; set; }
        public string? Company { get; set; }
        public string? Country { get; set; }
        public string? CountryIsoCode { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? State { get; set; }
        public string? Street1 { get; set; }
        public string? Street2 { get; set; }
        public string? ZipCode { get; set; }
        public string? Phone { get; set; }

        public CustomerShippingAddress(BQOrderList.Customer.ShippingAddress shippingAddress)
        {
            City = shippingAddress?.city ?? string.Empty;
            Civility = shippingAddress?.civility ?? string.Empty;
            Company = shippingAddress?.company ?? string.Empty;
            Country = shippingAddress?.country ?? string.Empty;
            CountryIsoCode = shippingAddress?.country_iso_code ?? string.Empty;
            FirstName = shippingAddress?.firstname ?? string.Empty;
            LastName = shippingAddress?.lastname ?? string.Empty;
            State = shippingAddress?.state ?? string.Empty;
            Street1 = shippingAddress?.street_1 ?? string.Empty;
            Street2 = shippingAddress?.street_2 ?? string.Empty;
            ZipCode = shippingAddress?.zip_code ?? string.Empty;
            Phone = shippingAddress?.phone ?? string.Empty;
        }
    }

    public class CustomerBillingAddress
    {
        public string? City { get; set; }
        public string? Civility { get; set; }
        public string? Company { get; set; }
        public string? Country { get; set; }
        public string? CountryIsoCode { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? State { get; set; }
        public string? Street1 { get; set; }
        public string? Street2 { get; set; }
        public string? ZipCode { get; set; }

        public CustomerBillingAddress(BQOrderList.Customer.BillingAddress billingAddress)
        {
            City = billingAddress?.city ?? string.Empty;
            Civility = billingAddress?.civility ?? string.Empty;
            Company = billingAddress?.company ?? string.Empty;
            Country = billingAddress?.country ?? string.Empty;
            CountryIsoCode = billingAddress?.country_iso_code ?? string.Empty;
            FirstName = billingAddress?.firstname ?? string.Empty;
            LastName = billingAddress?.lastname ?? string.Empty;
            State = billingAddress?.state ?? string.Empty;
            Street1 = billingAddress?.street_1 ?? string.Empty;
            Street2 = billingAddress?.street_2 ?? string.Empty;
            ZipCode = billingAddress?.zip_code ?? string.Empty;
        }
    }

    public class Orderline
    {
        public string? OrderLineId { get; set; }
        public string? OfferSKU { get; set; }
        public int? Quantity { get; set; }
        public decimal? PriceUnit { get; set; }
        public decimal? CommissionVat { get; set; }

        public Orderline(BQOrderList.Orderline orderline)
        {
            OrderLineId = orderline?.order_line_id ?? string.Empty;
            OfferSKU = orderline?.offer_sku ?? string.Empty;
            Quantity = orderline?.quantity;
            PriceUnit = orderline?.price_unit;
            CommissionVat = orderline?.commission_vat;
        }
    }
}
