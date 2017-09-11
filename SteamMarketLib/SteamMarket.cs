using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SteamMarketLib
{
    public static class SteamMarket
    {
        // Settings
        static int secondsBetweenRequests = 3; // in seconds
        static int waitTimeAfterError = 15; // in seconds
        static int maxRetries = -1; // -1 is pretty much infinite

        static DateTime lastApiRequest = new DateTime();

        public static string GetUrl(int appID, string encodedName)
        {
            Currency? currency = null;

            switch (RegionInfo.CurrentRegion.ISOCurrencySymbol)
            {
                case "USD":
                    currency = Currency.USD;
                    break;
                case "GBP":
                    currency = Currency.GBP;
                    break;
                case "EUR":
                    currency = Currency.EUR;
                    break;
            }

            if (!currency.HasValue)
                throw new Exception("Incompatible culture!");

            return String.Concat(
                @"https://steamcommunity.com/market/priceoverview/",
                $"?appid={appID}",
                $"&currency={(int)currency.Value}",
                $"&market_hash_name={encodedName}"
                );
        }

        public async static Task<SteamItemPriceData> GetPrice(int appID, string displayName)
        {
            string encodedName = WebUtility.UrlEncode(displayName);
            string url = GetUrl(appID, encodedName);
            string json = await GetStringFromUrl(url);
            return JsonConvert.DeserializeObject<SteamItemPriceData>(json);
        }

        static async Task<string> GetStringFromUrl(string url)
        {
            int retries = 0;
            string str = null;

            using (HttpClient client = new HttpClient())
            {
                TimeSpan throttleTime = (lastApiRequest.AddSeconds(secondsBetweenRequests)) - DateTime.Now;

                if (throttleTime.TotalSeconds > 0)
                    Thread.Sleep((int)throttleTime.TotalMilliseconds);

                while (str == null)
                {
                    try
                    {
                        str = await client.GetStringAsync(new Uri(url));
                    }
                    catch (WebException ex)
                    {
                        if (retries < (uint)maxRetries)
                        {
                            await Task.Delay(waitTimeAfterError * 1000);
                            retries++;
                        }
                        else
                        {
                            throw ex;
                        }
                    }

                    lastApiRequest = DateTime.Now;
                }
            }

            return str;
        }
    }
}
