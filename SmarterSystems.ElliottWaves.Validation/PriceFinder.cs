using System.Text.Json;
using SmarterSystems.ElliottWaves.Analyzer.Data;

namespace SmarterSystems.ElliottWaves.Validation
{
    [TestClass]
    public sealed class PriceFinder
    {
        /// <summary>
        /// Finds the exact hourly candle high/low for all specified dates across all tokens.
        /// Writes results to <c>price_lookup.txt</c> in the testoutput directory
        /// resolved by <see cref="TestDataPath.OutputPath"/>.
        /// </summary>
        [TestMethod]
        public void FindAllPrices()
        {
            var sb = new System.Text.StringBuilder();

            // BTC dates: Cycle 1 start, Primary dates, Cycle 2 dates, Cycle 3 dates, Cycle 4
            FindPrices(sb, "BTCUSDT", new[]
            {
                // Cycle 1: 15.09.2017-17.12.2017
                ("C1 Start", new DateTime(2017, 9, 15), false),
                ("C1 P1 End", new DateTime(2017, 9, 18), true),
                ("C1 P2 End", new DateTime(2017, 9, 22), false),
                ("C1 P3 End", new DateTime(2017, 12, 8), true),
                ("C1 P4 End", new DateTime(2017, 12, 10), false),
                ("C1 End/P5 End", new DateTime(2017, 12, 17), true),
                // Cycle 2: 17.12.2017-15.12.2018
                ("C2 PA End", new DateTime(2018, 2, 6), false),
                ("C2 PB End", new DateTime(2018, 2, 20), true),
                ("C2 End/PC End", new DateTime(2018, 12, 15), false),
                // Cycle 3: 15.12.2018-06.10.2025
                ("C3 P1 End", new DateTime(2019, 6, 26), true),
                ("C3 P2 End", new DateTime(2020, 3, 13), false),
                ("C3 P3 End", new DateTime(2021, 11, 10), true),
                ("C3 P4 End", new DateTime(2022, 11, 21), false),
                ("C3 End/P5 End", new DateTime(2025, 10, 6), true),
                // Cycle 4
                ("C4 PA End", new DateTime(2026, 2, 6), false),
            });

            // ETH dates
            FindPrices(sb, "ETHUSDT", new[]
            {
                // Cycle 1: 15.12.2018-10.11.2021
                ("C1 Start", new DateTime(2018, 12, 15), false),
                ("C1 P1 End", new DateTime(2019, 6, 26), true),
                ("C1 P2 End", new DateTime(2020, 3, 13), false),
                ("C1 P3 End", new DateTime(2021, 5, 12), true),
                ("C1 P4 End", new DateTime(2021, 7, 20), false),
                ("C1 End/P5 End", new DateTime(2021, 11, 10), true),
                // Cycle 2: 10.11.2021-18.06.2022
                ("C2 PA End", new DateTime(2022, 1, 24), false),
                ("C2 PB End", new DateTime(2022, 4, 3), true),
                ("C2 End/PC End", new DateTime(2022, 6, 18), false),
                // Cycle 3: 18.06.2022-Now
                ("C3 P1 End", new DateTime(2025, 8, 25), true),
                ("C3 P2 End", new DateTime(2026, 2, 6), false),
            });

            // SOL dates
            FindPrices(sb, "SOLUSDT", new[]
            {
                // Cycle 1: 23.12.2020-07.11.2021
                ("C1 Start", new DateTime(2020, 12, 23), false),
                ("C1 P1 End", new DateTime(2021, 1, 18), true),
                ("C1 P2 End", new DateTime(2021, 1, 21), false),
                ("C1 P3 End", new DateTime(2021, 5, 21), true),
                ("C1 P4 End", new DateTime(2021, 5, 23), false),
                ("C1 End/P5 End", new DateTime(2021, 11, 6), true),
                // Cycle 2: 06.11.2021-29.12.2022
                ("C2 PA End", new DateTime(2022, 6, 14), false),
                ("C2 PB End", new DateTime(2022, 8, 13), true),
                ("C2 End/PC End", new DateTime(2022, 12, 29), false),
                // Cycle 3: 29.12.2022-Now
                ("C3 P1 End", new DateTime(2025, 1, 19), true),
            });

            // ADA dates
            FindPrices(sb, "ADAUSDT", new[]
            {
                // Cycle 1: 13.03.2020-02.09.2021
                ("C1 Start", new DateTime(2020, 3, 13), false),
                ("C1 P1 End", new DateTime(2020, 7, 26), true),
                ("C1 P2 End", new DateTime(2020, 9, 23), false),
                ("C1 P3 End", new DateTime(2021, 5, 16), true),
                ("C1 P4 End", new DateTime(2021, 6, 22), false),
                ("C1 End/P5 End", new DateTime(2021, 9, 2), true),
                // Cycle 2: 02.09.2021-Now
                ("C2 PA End", new DateTime(2023, 6, 10), false),
                ("C2 PB End", new DateTime(2025, 12, 3), true),
            });

            // AVAX dates
            FindPrices(sb, "AVAXUSDT", new[]
            {
                // Cycle 1: 22.09.2020-21.11.2021
                ("C1 Start", new DateTime(2020, 9, 22), false),
                ("C1 P1 End", new DateTime(2021, 2, 22), true),  // user wrote 22.02.2020 but means 2021
                ("C1 P2 End", new DateTime(2020, 12, 23), false),
                ("C1 P3 End", new DateTime(2021, 2, 10), true),
                ("C1 P4 End", new DateTime(2021, 6, 22), false),
                ("C1 End/P5 End", new DateTime(2021, 11, 21), true),
                // Cycle 2: 21.11.2021-Now
                ("C2 PA End", new DateTime(2023, 9, 25), false),
                ("C2 PB End", new DateTime(2024, 2, 18), true), // user wrote 18.02.2024
            });

            // DOT dates
            FindPrices(sb, "DOTUSDT", new[]
            {
                // Cycle A: 18.08.2020-04.11.2021
                ("CA Start", new DateTime(2020, 8, 18), false),
                ("CA P1 End", new DateTime(2020, 8, 27), true),
                ("CA P2 End", new DateTime(2020, 9, 5), false),
                ("CA P3 End", new DateTime(2021, 5, 15), true),
                ("CA P4 End", new DateTime(2021, 7, 21), false),
                ("CA End/P5 End", new DateTime(2021, 11, 4), true),
                // Cycle B: 04.11.2021-10.10.2025
                ("CB PA End", new DateTime(2023, 10, 19), false),
                ("CB PB End", new DateTime(2024, 12, 4), true),
                ("CB End/PC End", new DateTime(2025, 10, 10), false),
                // Cycle C: 10.10.2025-Now
                ("CC P1 End", new DateTime(2025, 11, 8), true),
                ("CC P2 End", new DateTime(2026, 2, 6), false),
            });

            // HBAR dates
            FindPrices(sb, "HBARUSDT", new[]
            {
                // Cycle 1: 31.12.2019-16.09.2021
                ("C1 Start", new DateTime(2019, 12, 31), false),
                ("C1 P1 End", new DateTime(2020, 2, 12), true),
                ("C1 P2 End", new DateTime(2020, 3, 13), false),
                ("C1 P3 End", new DateTime(2021, 3, 15), true),
                ("C1 P4 End", new DateTime(2021, 6, 22), false),
                ("C1 End/P5 End", new DateTime(2021, 9, 16), true),
                // Cycle 2: 16.09.2021-Now
                ("C2 PA End", new DateTime(2022, 12, 31), false),
                ("C2 PB End", new DateTime(2025, 1, 17), true),
            });

            // LINK dates
            FindPrices(sb, "LINKUSDT", new[]
            {
                // Cycle 1: 29.01.2019-10.05.2021
                ("C1 Start", new DateTime(2019, 1, 29), false),
                ("C1 P1 End", new DateTime(2019, 6, 29), true),
                ("C1 P2 End", new DateTime(2020, 3, 13), false),
                ("C1 P3 End", new DateTime(2020, 8, 10), true),
                ("C1 P4 End", new DateTime(2020, 9, 23), false),
                ("C1 End/P5 End", new DateTime(2021, 5, 10), true),
                // Cycle 2: 10.05.2021-10.06.2023
                ("C2 PA End", new DateTime(2021, 7, 20), false),
                ("C2 PB End", new DateTime(2021, 11, 10), true),
                ("C2 End/PC End", new DateTime(2023, 6, 10), false),
                // Cycle 3: 10.06.2023-Now
                ("C3 P1 End", new DateTime(2024, 12, 13), true),
                ("C3 P2 End", new DateTime(2026, 2, 6), false),
            });

            // MANA dates
            FindPrices(sb, "MANAUSDT", new[]
            {
                // Cycle 1: 10.09.2020-26.11.2021
                ("C1 Start", new DateTime(2020, 9, 10), false),
                ("C1 P1 End", new DateTime(2020, 11, 20), true),
                ("C1 P2 End", new DateTime(2020, 12, 24), false),
                ("C1 P3 End", new DateTime(2021, 3, 14), true),
                ("C1 P4 End", new DateTime(2021, 6, 23), false),
                ("C1 End/P5 End", new DateTime(2021, 11, 26), true), // user wrote 26.11.21 but probably 25th
                // Cycle 2: 26.11.2021-10.10.2025
                ("C2 PA End", new DateTime(2022, 12, 30), false),
                ("C2 PB End", new DateTime(2024, 12, 4), true),
                ("C2 End/PC End", new DateTime(2025, 10, 10), false),
                // Cycle 3: 10.10.2025-Now
                ("C3 P1 End", new DateTime(2025, 10, 13), true),
            });

            // SUI dates
            FindPrices(sb, "SUIUSDT", new[]
            {
                // Cycle A: 01.05.2023-06.01.2025
                ("CA Start (Low)", new DateTime(2023, 5, 3), false), // earliest data
                ("CA PA End", new DateTime(2023, 5, 3), true), // same as start? need high of that day
                ("CA PB End", new DateTime(2024, 8, 5), false),
                ("CA End/PC End", new DateTime(2025, 1, 6), true),
                // Cycle B: 06.01.2025-10.10.2025
                ("CB PA End", new DateTime(2025, 4, 7), false),
                ("CB PB End", new DateTime(2025, 7, 28), true),
                ("CB End/PC End", new DateTime(2025, 10, 10), false),
                // Cycle C: 10.10.2025-Now
                ("CC PA End", new DateTime(2025, 10, 13), true),
                ("CC PB End", new DateTime(2026, 2, 6), false),
            });

            // XRP dates
            FindPrices(sb, "XRPUSDT", new[]
            {
                // Cycle A: 13.03.2020-18.07.2025
                ("CA Start", new DateTime(2020, 3, 13), false),
                ("CA PA End", new DateTime(2021, 4, 14), true),
                ("CA PB End", new DateTime(2022, 6, 17), false), // user wrote 17.06.2022
                ("CA End/PC End", new DateTime(2025, 7, 18), true),
                // Cycle B: 18.07.2025-Now
                ("CB PA End", new DateTime(2026, 2, 6), false),
            });

            File.WriteAllText(TestDataPath.OutputFile("price_lookup.txt"), sb.ToString());
        }

        private static void FindPrices(System.Text.StringBuilder sb, string symbol,
            (string label, DateTime date, bool isHigh)[] lookups)
        {
            var path = TestDataPath.ChartDataFile(symbol);
            if (!File.Exists(path))
            {
                sb.AppendLine($"\n=== {symbol} === SKIPPED: data file not found at {path}");
                return;
            }
            var json = File.ReadAllText(path);
            var candles = JsonSerializer.Deserialize<List<Ohlcv>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })!;

            sb.AppendLine($"\n=== {symbol} ===");
            sb.AppendLine($"  Data range: {candles.First().TimestampUtc:yyyy-MM-dd HH:mm} to {candles.Last().TimestampUtc:yyyy-MM-dd HH:mm}");
            sb.AppendLine($"  Total candles: {candles.Count}");

            foreach (var (label, date, isHigh) in lookups)
            {
                // Find all candles on the given date
                var dayCandles = candles.Where(c => c.TimestampUtc.Date == date.Date).ToList();
                if (dayCandles.Count == 0)
                {
                    // Try nearby dates (±1 day)
                    dayCandles = candles.Where(c =>
                        c.TimestampUtc.Date >= date.Date.AddDays(-1) &&
                        c.TimestampUtc.Date <= date.Date.AddDays(1)).ToList();
                    if (dayCandles.Count == 0)
                    {
                        sb.AppendLine($"  {label}: NO DATA for {date:yyyy-MM-dd} (±1 day)");
                        continue;
                    }
                    sb.Append($"  {label}: (±1 day) ");
                }
                else
                {
                    sb.Append($"  {label}: ");
                }

                if (isHigh)
                {
                    var extreme = dayCandles.OrderByDescending(c => c.High).First();
                    sb.AppendLine($"HIGH ${extreme.High} at {extreme.TimestampUtc:yyyy-MM-dd HH:mm}");
                }
                else
                {
                    var extreme = dayCandles.OrderBy(c => c.Low).First();
                    sb.AppendLine($"LOW ${extreme.Low} at {extreme.TimestampUtc:yyyy-MM-dd HH:mm}");
                }
            }
        }
    }
}
