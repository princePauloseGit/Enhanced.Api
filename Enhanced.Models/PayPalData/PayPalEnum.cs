using System.Runtime.Serialization;

namespace Enhanced.Models.PayPalData
{
    public class PayPalEnum
    {
        public enum EventCode
        {
            T0004,
            T0006,
            T5000,
            T5001,
            T1107,
            T0113,
            T0400,
        }

        public enum PaymentType
        {
            [EnumMember(Value = "Payment")]
            DR, // Debit

            [EnumMember(Value = "Refund")]
            CR, // Credit
        }
    }
}
