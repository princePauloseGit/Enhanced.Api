namespace Enhanced.Models.BQData
{
    public class BQEnum
    {
        public enum OrderStatus
        {
            Staging = 0,
            Waiting_Acceptance = 1,
            Waiting_Debit = 2,
            Waiting_Debit_Payment = 3,
            Shipping = 4,
            Shipped = 5,
            To_Collect = 6,
            Received = 7,
            Closed = 8,
            Refused = 9,
            Cancelled = 10
        }

        public enum PayMentWorkflow
        {
            Pay_On_Acceptance = 0,
            Pay_On_Delivery = 1,
            Pay_On_Due_Date = 2,
            Pay_On_Shipment = 3,
            No_Customer_Payment_Confirmation = 4
        }

        public enum TaxMode
        {
            Tax_Excluded = 0,
            Tax_Included = 1
        }

        public enum RateLimitType
        {
            UNSET,

            BQ_GetOrders,
            BQ_GetPayments,
            BQ_Shipment,
            BQ_Refund,
        }

        public enum PaymentType
        {
            PAYMENT = 0,
            ORDER_AMOUNT = 1,
            REFUND_ORDER_AMOUNT = 2,
            COMMISSION_FEE = 3,
            REFUND_COMMISSION_FEE = 4,
            SUBSCRIPTION_FEE = 5,
            COMMISSION_VAT = 6,
            REFUND_COMMISSION_VAT = 7,
            SUBSCRIPTION_VAT = 8
        }
    }
}
