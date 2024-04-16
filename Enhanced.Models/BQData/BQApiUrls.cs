namespace Enhanced.Models.BQData
{
    public class BQApiUrls
    {
        public readonly static string _baseUrl = "https://marketplace.kingfisher.com/api";

        public static string BQOrders
        {
            get => $"{_baseUrl}/orders";
        }

        public static string BQShipment
        {
            get => $"{_baseUrl}/shipments";
        }

        public static string BQPayment
        {
            get => $"{_baseUrl}/sellerpayment/transactions_logs";
        }

        public static string BQRefund
        {
            get => $"{_baseUrl}/orders/refund";
        }
    }
}
