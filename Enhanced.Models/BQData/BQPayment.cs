using Enhanced.Models.EbayData;
using static Enhanced.Models.EbayData.EbayOrderList;

namespace Enhanced.Models.BQData
{
    public partial class BQPayment
    {
        public List<PaymentList>? data { get; set; }
        public string? previous_page_token { get; set; }
        public string? next_page_token { get; set; }
        public class PaymentList
        {
            public DateTime accounting_document_creation_date { get; set; }
            public string? accounting_document_number { get; set; }
            public double amount { get; set; }
            public double amount_credited { get; set; }
            public double amount_debited { get; set; }
            public double balance { get; set; }
            public string? currency_iso_code { get; set; }
            public DateTime date_created { get; set; }
            public Entities? entities { get; set; }
            public string? id { get; set; }
            public DateTime last_updated { get; set; }
            public string? payment_state { get; set; }
            public string? type { get; set; }
        }
        public class Entities
        {
            public Debit? debit { get; set; }
            public string? domain { get; set; }
            public ManualAccountingDocument? manual_accounting_document { get; set; }
            public Order? order { get; set; }
            public OrderLine? order_line { get; set; }
            public OrderTax? order_tax { get; set; }
            public Refund? refund { get; set; }
            public ShopTax? shop_tax { get; set; }
            public TransactionInfo? transaction_info { get; set; }
        }
        public class Debit
        {
            public string? id { get; set; }
        }
        public class ManualAccountingDocument
        {
            public string? emission_date { get; set; }
            public string? number { get; set; }
        }
        public class Order
        {
            public string? commercial_id { get; set; }
            public string? creation_date { get; set; }
            public string? id { get; set; }
            public PaymentInfo? payment_info { get; set; }
            public References? references { get; set; }
        }
        public class PaymentInfo
        {
            public string? imprint_number { get; set; }
            public string? payment_type { get; set; }
        }
        public class References
        {
            public string? order_reference_for_customer { get; set; }
            public string? order_reference_for_seller { get; set; }
        }
        public class OrderLine
        {
            public string? id { get; set; }
        }
        public class ShopTax
        {
            public string? code { get; set; }
            public string? rate { get; set; }
        }
        public class TransactionInfo
        {
            public DateTime date { get; set; }
            public string? number { get; set; }
        }
    }
}
