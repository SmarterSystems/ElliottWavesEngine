using System.Text.Json.Serialization;

namespace SmarterSystems.ElliottWaves.Analyzer.Data
{
    /// <summary>
    /// Open / High / Low / Close / Volume candle. Self-contained DTO with no external dependencies.
    /// Properties use JsonPropertyName so deserialization matches the camelCase format used in the
    /// bundled sample data and the fetch-test-data script output.
    /// </summary>
    public class Ohlcv
    {
        [JsonPropertyName("timestampUtc")]
        public DateTime TimestampUtc { get; set; }

        [JsonPropertyName("open")]
        public decimal Open { get; set; }

        [JsonPropertyName("high")]
        public decimal High { get; set; }

        [JsonPropertyName("low")]
        public decimal Low { get; set; }

        [JsonPropertyName("close")]
        public decimal Close { get; set; }

        [JsonPropertyName("volume")]
        public decimal Volume { get; set; }
    }
}
