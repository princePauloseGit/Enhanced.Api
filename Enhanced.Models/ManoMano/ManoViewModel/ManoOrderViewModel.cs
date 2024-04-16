using Enhanced.Models.Shared;
using static Enhanced.Models.ManoMano.ManoOrderResponse;

namespace Enhanced.Models.ManoMano.ManoViewModel
{
    public class ManoOrderViewModel
    {
        public List<Orders>? Orders { get; set; }
        public List<ErrorLog>? ErrorLogs { get; set; } = new List<ErrorLog>();
    }

    public class Orders
    {
        public string? OrderReference { get; set; }
        public int SellerContractId { get; set; }
        public string? Status { get; set; }
        public string? CreatedAt { get; set; }
        public ManomanoAmount? TotalPrice { get; set; }
        public Addresses? Addresses { get; set; }
        public Customer? Customer { get; set; }
        public List<OrderLines>? OrderLines { get; set; }

        public Orders(ManoOrderResponse.Content manoOrderResponse)
        {
            OrderReference = manoOrderResponse.order_reference ?? string.Empty;
            SellerContractId = manoOrderResponse.seller_contract_id;
            Status = manoOrderResponse.status ?? string.Empty;
            CreatedAt = manoOrderResponse.created_at ?? string.Empty;
            TotalPrice = new ManomanoAmount(manoOrderResponse.total_price!);
            Addresses = new Addresses(manoOrderResponse.addresses!);
            Customer = new Customer(manoOrderResponse.customer!);
            OrderLines = manoOrderResponse.products?.Select(s => new OrderLines(s)).ToList();
        }
    }

    public class ManomanoAmount
    {
        public decimal? Amount { get; set; }
        public string? Currency { get; set; }

        public ManomanoAmount(ManoOrderResponse.ManomanoAmount manomanoAmount)
        {
            Amount = manomanoAmount.amount ?? 0;
            Currency = manomanoAmount.currency ?? string.Empty;
        }
    }

    public class Addresses
    {
        public Address? Billing { get; set; }
        public Address? Shipping { get; set; }

        public Addresses(ManoOrderResponse.Addresses addresses)
        {
            Billing = new Address(addresses.billing!);
            Shipping = new Address(addresses.shipping!);
        }
    }

    public class Address
    {
        public string? Address_line1 { get; set; }
        public string? Address_line2 { get; set; }
        public string? Address_line3 { get; set; }
        public string? City { get; set; }
        public string? Company { get; set; }
        public string? Country { get; set; }
        public string? Country_iso { get; set; }
        public string? Email { get; set; }
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string? Phone { get; set; }
        public string? Province { get; set; }
        public string? Zipcode { get; set; }

        public Address(ManoOrderResponse.Address address)
        {
            Address_line1 = address.address_line1 ?? string.Empty;
            Address_line2 = address.address_line2 ?? string.Empty;
            Address_line3 = address.address_line3 ?? string.Empty;
            City = address.city ?? string.Empty;
            Company = address.company ?? string.Empty;
            Country = address.country ?? string.Empty;
            Country_iso = address.country_iso ?? string.Empty;
            Email = address.email ?? string.Empty;
            Firstname = address.firstname ?? string.Empty;
            Lastname = address.lastname ?? string.Empty;
            Phone = address.phone ?? string.Empty;
            Province = address.province ?? string.Empty;
            Zipcode = address.zipcode ?? string.Empty;
        }
    }

    public class OrderLines
    {
        public ManomanoAmount? Price { get; set; }
        public int Quantity { get; set; }
        public string? SellerSku { get; set; }
        public ManomanoAmount? ShippingPrice { get; set; }

        public OrderLines(ManoOrderResponse.Product product)
        {
            Price = new ManomanoAmount(product.price!);
            Quantity = product.quantity;
            SellerSku = product.seller_sku ?? string.Empty;
            ShippingPrice = new ManomanoAmount(product.shipping_price!);
        }
    }

    public class Customer
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public Customer(ManoOrderResponse.Customer customer)
        {
            FirstName = customer.firstname ?? string.Empty;
            LastName = customer.lastname ?? string.Empty;
        }
    }

    public class Pagination
    {
        public Links? links { get; set; }
        public int page { get; set; }
        public int pages { get; set; }

        public Pagination(ManoOrderResponse.Pagination pagination)
        {
            links = new Links(pagination.links!);
            page = pagination!.page;
            pages = pagination!.pages;
        }
    }

    public class Links
    {
        public string? next { get; set; }
        public string? previous { get; set; }

        public Links(ManoOrderResponse.Links links)
        {
            next = links!.next;
            previous = links!.previous;
        }
    }
}
