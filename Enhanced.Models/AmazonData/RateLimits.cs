using static Enhanced.Models.AmazonData.AmazonEnum;

namespace Enhanced.Models.AmazonData
{
    public class RateLimits
    {
        internal decimal Rate { get; set; }
        internal int Burst { get; set; }
        internal DateTime LastRequest { get; set; }
        internal int RequestsSent { get; set; }

        public RateLimits(decimal rate, int burst)
        {
            this.Rate = rate;
            this.Burst = burst;
            this.LastRequest = DateTime.UtcNow;
            this.RequestsSent = 0;
        }

        private int GetRatePeriodMs()
        {
            return (int)(1 / Rate * 1000 / 1);
        }

        public RateLimits NextRate()
        {
            if (RequestsSent < 0)
            {
                RequestsSent = 0;
            }

            int ratePeriodMs = GetRatePeriodMs();

            if (RequestsSent >= Burst)
            {
                var LastRequestTime = LastRequest;

                while (true)
                {
                    LastRequestTime = LastRequestTime.AddMilliseconds(ratePeriodMs);

                    if (LastRequestTime > DateTime.UtcNow)
                    {
                        break;
                    }
                    else
                    {
                        RequestsSent -= 1;
                    }

                    if (RequestsSent <= 0)
                    {
                        RequestsSent = 0;
                        break;
                    }
                }
            }

            if (RequestsSent >= Burst)
            {
                LastRequest = LastRequest.AddMilliseconds(ratePeriodMs);

                while (LastRequest >= DateTime.UtcNow)
                {
                    Task.Delay(100).Wait();
                }
            }

            if (RequestsSent + 1 <= Burst)
            {
                RequestsSent += 1;
            }

            LastRequest = DateTime.UtcNow;

            return this;
        }

        public void SetRateLimit(decimal rate)
        {
            Rate = rate;
        }

        public async Task Delay()
        {
            await Task.Delay(GetRatePeriodMs());
        }

        public static Dictionary<RateLimitType, RateLimits> RateLimitsTime()
        {
            //This has to create a new list for each connection, so that rate limits are per seller, not overall.
            return new Dictionary<RateLimitType, RateLimits>()
            {
                { RateLimitType.Order_GetOrders,                            new RateLimits(0.0167M, 20) },
                { RateLimitType.Order_GetOrderItems,                        new RateLimits(0.5M, 30) },
                { RateLimitType.Order_ConfirmShipment,                      new RateLimits(2M, 10) },

                { RateLimitType.Report_GetReports,                          new RateLimits(0.0222M, 10) },
                { RateLimitType.Report_GetReportDocument,                   new RateLimits(0.0167M, 15) },

                { RateLimitType.CatalogItems20220401_SearchCatalogItems,    new RateLimits(2.0M, 2) },

                { RateLimitType.Feed_CreateFeedDocument,                    new RateLimits(0.0083M, 15) },
                { RateLimitType.Feed_CreateFeed,                            new RateLimits(0.0083M, 15) },
            };
        }
    }
}
