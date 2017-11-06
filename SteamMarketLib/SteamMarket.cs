using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SteamMarketLib
{
    public static class SteamMarket
    {
        public static int MinSecondsBetweenRequests = 3;
        public static int WaitTimeAfterError = 15; // in seconds
        public static int MaxRetries = -1; // -1 is pretty much infinite

        private static DateTime m_lastApiRequest = new DateTime();

        public static string GetUrl(int appId, string encodedName, Currency currency)
        {
            return String.Concat(
                    @"https://steamcommunity.com/market/priceoverview/",
                    $"?appid={appId}",
                    $"&currency={(int)currency}",
                    $"&market_hash_name={encodedName}"
                );
        }

        public static async Task<SteamItemPriceData> GetPrice(int appId, string displayName, Currency currency = Currency.USD)
        {
            string encodedName = WebUtility.UrlEncode(displayName);
            string url = GetUrl(appId, encodedName, currency);
            string json = await GetStringFromUrl(url);
            return JsonConvert.DeserializeObject<SteamItemPriceData>(json);
        }

        private static async Task<string> GetStringFromUrl(string url)
        {
            int retries = 0;
            string str = null;

            using (HttpClient client = new HttpClient())
            {
                TimeSpan throttleTime = (m_lastApiRequest.AddSeconds(MinSecondsBetweenRequests)) - DateTime.Now;

                if (throttleTime.TotalMilliseconds > 0)
                    Thread.Sleep((int)throttleTime.TotalMilliseconds);

                while (str == null)
                {
                    try
                    {
                        str = await client.GetStringAsync(new Uri(url));
                    }
                    catch (WebException ex)
                    {
                        if (retries < (uint)MaxRetries) // uint cast to convert -1 to a large (almost infinite) number
                        {
                            await Task.Delay(WaitTimeAfterError * 1000);
                            retries++;
                        }
                        else
                        {
                            throw ex; // Give up
                        }
                    }

                    m_lastApiRequest = DateTime.Now;
                }
            }

            return str;
        }
    }
}