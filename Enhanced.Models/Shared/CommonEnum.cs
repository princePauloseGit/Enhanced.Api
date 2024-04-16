namespace Enhanced.Models.Shared
{
    public class CommonEnum
    {
        public enum Environment
        {
            Sandbox = 0,
            Production = 1
        }

        public enum ShortcutDimension
        {
            OnBuy,
            eBay,
            Website,
        }

        public enum AccountType
        {
            Customer,
            Vendor,
            Bank,
        }

        public enum PaymentLine
        {
            Customer,
            PayPal,
            OnBuy,
            Bank,
            Vendor,
            Invoice,
            Payment
        }

        public enum DocumentType
        {
            Payment,
            Refund,
        }

        public enum Priority
        {
            Low = 0,
            High = 1,
        }

        public enum Sevarity
        {
            Information,
            Warning,
            Error
        }

        public enum Marketplace
        { 
            Amazon,
            BQ,
            Appeagle,
            PayPal,
            Braintree,
            EBay,
            ManoMano,
            OnBuy,
        }

        public enum Currency
        {
            GBP
        }
    }
}
