using Enhanced.Models.Shared;

namespace Enhanced.Models.OnBuy.OnBuyViewModel
{
    public class OnBuyRefundResponseViewModel
    {
        public OnBuyOrders? Orders { get; set; }
        public List<ErrorLog>? ErrorLogs { get; set; } = new List<ErrorLog>();
    }

    public class OnBuyOrders
    {
        public string? OrderId { get; set; }
        public bool? IsRefunded { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
