using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Globalization;

namespace SteamMarketLib
{
    [JsonObject]
    public class SteamItemPriceData
    {
        [JsonIgnore]
        public bool Success { get; set; }

        [JsonProperty("lowest_price")]
        [JsonConverter(typeof(PriceConverter))]
        public decimal LowestPrice { get; set; }

        [JsonProperty("volume")]
        [JsonConverter(typeof(VolumeConverter))]
        public uint Volume { get; set; }

        [JsonProperty("median_price")]
        [JsonConverter(typeof(PriceConverter))]
        public decimal MedianPrice { get; set; }
    }

    public class PriceConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(decimal);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            string price = token.Value<String>().Replace('-', '0'); // Replace 2.--€ with 2.00€
            return decimal.Parse(price, NumberStyles.Currency);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JToken token = JToken.FromObject(value);
            token.WriteTo(writer);
        }
    }

    public class VolumeConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(uint);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            string volume = token.Value<String>();
            return uint.Parse(volume, NumberStyles.AllowThousands, CultureInfo.InvariantCulture);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JToken token = JToken.FromObject(value);
            token.WriteTo(writer);
        }
    }
}
