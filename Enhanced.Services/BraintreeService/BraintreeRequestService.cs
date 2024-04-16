using Braintree;
using Enhanced.Models.BraintreeData;
using Enhanced.Models.Shared;
using static Enhanced.Models.Shared.CommonEnum;

namespace Enhanced.Services.BraintreeService
{
    public class BraintreeRequestService
    {
        private readonly IBraintreeGateway _braintreeGateway;

        public BraintreeRequestService(ParameterBraintree parameterBraintree)
        {
            _braintreeGateway = new BraintreeGateway(Braintree.Environment.PRODUCTION, parameterBraintree.MerchantId, parameterBraintree.PublicKey, parameterBraintree.PrivateKey);
        }

        public async Task<(List<Braintree.Transaction>, List<ErrorLog>)> SearchTransaction(int days)
        {
            string paymentData = string.Concat(days, " days; Payment From: ", DateTime.UtcNow.Date.AddDays(-days).ToString(Constant.DATETIME_DDMMYYY_HHMMSS), " Date To: ", DateTime.UtcNow.Date.ToString(Constant.DATETIME_DDMMYYY_HHMMSS));
            var errorLogs = new List<ErrorLog>
            {
                new ErrorLog(Marketplace.Braintree, Sevarity.Information, "Payment Transaction", "", Priority.Low, paymentData)
            };

            try
            {
                var request = new TransactionSearchRequest().Status.Is(Braintree.TransactionStatus.SETTLED).SettledAt.Between(DateTime.UtcNow.Date.AddDays(-days), DateTime.UtcNow.Date);

                var transactionCollection = await _braintreeGateway.Transaction.SearchAsync(request);
                var transactions = transactionCollection.Where(x => x.PaymentInstrumentType != PaymentInstrumentType.PAYPAL_ACCOUNT).Select(s => s).ToList();

                errorLogs.Add(new ErrorLog(Marketplace.Braintree, Sevarity.Information, "Payment Transaction", "", Priority.Low, "Total Transactions: " + transactions?.Count));

                return (transactions!, errorLogs);
            }
            catch (Exception ex)
            {
                errorLogs.Add(new ErrorLog(Marketplace.Braintree, Sevarity.Error, "Payment Transaction", "", Priority.High, ex.Message, ex.StackTrace));
            }

            return (null!, errorLogs);
        }
      

        public async Task<(bool, ErrorLog)> RefundPayment(string transactionId, decimal amount)
        {
            try
            {
                bool isSuccess = false;

                if (string.IsNullOrEmpty(transactionId))
                {
                    return (false, new ErrorLog(Marketplace.Braintree, Sevarity.Error, "Transaction Id", transactionId, Priority.High, "Transaction id is required"));
                }

                var result = await _braintreeGateway.Transaction.RefundAsync(transactionId, amount).ConfigureAwait(false);
                isSuccess = result.IsSuccess();

                var errorLog = isSuccess
                    ? new ErrorLog(Marketplace.Braintree, Sevarity.Information, "Transaction Id", transactionId, Priority.Low, stackTrace: "Refunded Amount: " + amount)
                    : new ErrorLog(Marketplace.Braintree, Sevarity.Error, "Transaction Id", transactionId, Priority.High, stackTrace: result.Message);

                return (result.IsSuccess(), errorLog);
            }
            catch (Exception ex)
            {
                return (false, new ErrorLog(Marketplace.Braintree, Sevarity.Error, "Transaction Id", transactionId, Priority.High, ex.Message, ex.StackTrace));
            }
        }
    }
}
