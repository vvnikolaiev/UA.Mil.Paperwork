using Mil.Paperwork.Infrastructure.Enums;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Mil.Paperwork.Domain.Helpers
{
    public class PreciousMetalRate
    {
        public MetalType Type { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal Rate { get; set; }
    }

    public static class PreciousMetalsExchangeApiHelper
    {
        private const string ApiUrl = "https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?date={0}&json";
        private const decimal GramsInOz = 31.1035m;

        private static readonly Dictionary<string, MetalType> MetalCodes = new()
        {
            { MetalType.XAU.ToString(), MetalType.XAU }, // Gold
            { MetalType.XAG.ToString(), MetalType.XAG }, // Silver
            { MetalType.XPT.ToString(), MetalType.XPT }, // Platinum
            { MetalType.XPD.ToString(), MetalType.XPD }, // Palladium
        };

        public static async Task<Dictionary<MetalType, PreciousMetalRate>> GetMetalRatesAsync(DateTime date)
        {
            var dateStr = date.ToString("yyyyMMdd");
            var url = string.Format(ApiUrl, dateStr);

            using var client = new HttpClient();
            var json = await client.GetStringAsync(url);

            var allRates = JsonSerializer.Deserialize<List<ExchangeRateDto>>(json);

            var result = new Dictionary<MetalType, PreciousMetalRate>();
            foreach (var rate in allRates)
            {
                if (rate != null && MetalCodes.ContainsKey(rate.CC))
                {
                    var type = MetalCodes[rate.CC];
                    result.Add(type, new PreciousMetalRate
                    {
                        Type = type,
                        Code = rate.CC,
                        Name = rate.Txt,
                        Rate = Math.Round(rate.Rate / GramsInOz, 2)
                    });
                }
            }
            return result;
        }

        private class ExchangeRateDto
        {
            [JsonPropertyName("r030")]
            public int R030 { get; set; }
            [JsonPropertyName("txt")]
            public string Txt { get; set; }
            [JsonPropertyName("rate")]
            public decimal Rate { get; set; }
            [JsonPropertyName("cc")]
            public string CC { get; set; }
            [JsonPropertyName("exchangedate")]
            public string ExchangeDate { get; set; }
        }
    }
}