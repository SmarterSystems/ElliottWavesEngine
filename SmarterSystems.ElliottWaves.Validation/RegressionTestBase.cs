using System.Text.Json;
using SmarterSystems.ElliottWaves.Analyzer.Data;
using SmarterSystems.ElliottWaves.Analyzer.Analysis;
using SmarterSystems.ElliottWaves.Analyzer.Interfaces;

namespace SmarterSystems.ElliottWaves.Validation
{
    /// <summary>
    /// Base class for per-asset regression tests.
    /// Loads chart data, runs analysis, and provides assertion helpers
    /// including log-scale Fibonacci projection calculations.
    /// </summary>
    public abstract class RegressionTestBase
    {
        protected static ElliottWavesAnalysis LoadAndAnalyze(string symbol)
        {
            var path = TestDataPath.ChartDataFile(symbol);
            if (!File.Exists(path))
            {
                Assert.Inconclusive(
                    $"Test data file not found: {path}\n" +
                    "Run scripts/fetch-test-data.ps1 to download OHLCV data from Binance, " +
                    "or set ELLIOTTWAVES_TESTDATA_PATH to a directory containing the chartdata files.");
            }
            var json = File.ReadAllText(path);
            var data = JsonSerializer.Deserialize<List<Ohlcv>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return new ElliottWavesAnalyzer().Analyze(data!);
        }

        protected static void AssertPoint(PivotPoint actual, DateTime expectedTime, decimal expectedPrice, string context)
        {
            Assert.IsNotNull(actual, $"{context}: PivotPoint is null");
            Assert.AreEqual(expectedTime, actual.Timestamp,
                $"{context}: Timestamp mismatch. Expected {expectedTime:yyyy-MM-ddTHH:mm:ssZ}, got {actual.Timestamp:yyyy-MM-ddTHH:mm:ssZ}");
            Assert.AreEqual(expectedPrice, actual.Price,
                $"{context}: Price mismatch. Expected {expectedPrice}, got {actual.Price}");
        }

        protected static void AssertWaveCount(ElliottWavesAnalysis analysis, int expected, string context)
        {
            Assert.IsNotNull(analysis?.Waves, $"{context}: Waves is null");
            Assert.AreEqual(expected, analysis.Waves.Count,
                $"{context}: Expected {expected} cycle waves, got {analysis.Waves.Count}");
        }

        protected static void AssertSubWaveCount(ElliottWave wave, int expected, string context)
        {
            Assert.IsNotNull(wave.SubWaves, $"{context}: SubWaves is null");
            Assert.AreEqual(expected, wave.SubWaves.Count,
                $"{context}: Expected {expected} sub-waves, got {wave.SubWaves.Count}");
        }

        protected static void AssertLabel(ElliottWave wave, WaveNumber expectedLabel, string context)
        {
            Assert.AreEqual(expectedLabel, wave.Label,
                $"{context}: Label mismatch. Expected {expectedLabel}, got {wave.Label}");
        }

        protected static void AssertInProgress(ElliottWave wave, bool expected, string context)
        {
            Assert.AreEqual(expected, wave.IsInProgress,
                $"{context}: InProgress mismatch. Expected {expected}, got {wave.IsInProgress}");
        }

        protected static DateTime Utc(int y, int m, int d, int h = 0, int min = 0)
            => new DateTime(y, m, d, h, min, 0, DateTimeKind.Utc);

        // ══════════════════════════════════════════════════
        // Log-scale Fibonacci projection helpers
        // ══════════════════════════════════════════════════

        /// <summary>
        /// Calculates a Fibonacci retracement level using log scale.
        /// Given a move from startPrice to endPrice, returns the price at the given retracement level.
        /// E.g., 0.618 retracement of an up move: price = 10^(log10(end) - level * (log10(end) - log10(start)))
        /// </summary>
        protected static decimal LogRetracement(decimal startPrice, decimal endPrice, decimal level)
        {
            var logStart = Math.Log10((double)startPrice);
            var logEnd = Math.Log10((double)endPrice);
            var logResult = logEnd - (double)level * (logEnd - logStart);
            return (decimal)Math.Pow(10, logResult);
        }

        /// <summary>
        /// Calculates a Fibonacci extension level using log scale.
        /// Given waves: W1 from w1Start to w1End, and W2 ending at w2End,
        /// returns the price at the extension level measured from w2End.
        /// E.g., 1.618 extension: price = 10^(log10(w2End) + level * (log10(w1End) - log10(w1Start)))
        /// For motive extensions, the direction of W1 is projected from W2's end.
        /// </summary>
        protected static decimal LogExtension(decimal w1Start, decimal w1End, decimal w2End, decimal level)
        {
            var logW1Start = Math.Log10((double)w1Start);
            var logW1End = Math.Log10((double)w1End);
            var logW2End = Math.Log10((double)w2End);
            var logW1Range = logW1End - logW1Start;
            var logResult = logW2End + (double)level * logW1Range;
            return (decimal)Math.Pow(10, logResult);
        }

        /// <summary>
        /// Calculates A=C projection in log scale for corrective patterns.
        /// Given wave A from aStart to aEnd and wave B ending at bEnd,
        /// projects wave C equal to wave A (in log scale) from bEnd.
        /// For a bearish correction (A down, B up): C goes down from bEnd by log-distance of A.
        /// For a bullish correction (A up, B down): C goes up from bEnd by log-distance of A.
        /// </summary>
        protected static decimal LogAEqualsC(decimal aStart, decimal aEnd, decimal bEnd)
        {
            var logAStart = Math.Log10((double)aStart);
            var logAEnd = Math.Log10((double)aEnd);
            var logBEnd = Math.Log10((double)bEnd);
            var logARange = logAEnd - logAStart; // negative for bearish A, positive for bullish A
            var logResult = logBEnd + logARange;
            return (decimal)Math.Pow(10, logResult);
        }

        /// <summary>
        /// Asserts that an expected projection target exists in the wave's projection targets,
        /// using a tolerance of 1% for price comparison.
        /// </summary>
        protected static void AssertProjectionTarget(ElliottWave wave, decimal expectedPrice, decimal fibLevel, string context)
        {
            Assert.IsNotNull(wave.Projection, $"{context}: Projection is null");
            Assert.IsNotNull(wave.Projection.Targets, $"{context}: Projection.Targets is null");

            var tolerance = 0.01m; // 1% tolerance
            var match = wave.Projection.Targets.FirstOrDefault(t =>
                Math.Abs(t.FibonacciLevel - fibLevel) < 0.001m &&
                Math.Abs((t.Price - expectedPrice) / expectedPrice) < tolerance);

            Assert.IsNotNull(match,
                $"{context}: No projection target found for Fib {fibLevel} near ${expectedPrice:F2}. " +
                $"Available targets: {string.Join(", ", wave.Projection.Targets.Select(t => $"Fib {t.FibonacciLevel}=${t.Price:F2}"))}");
        }

        /// <summary>
        /// Rounds a projection price to a reasonable number of decimal places
        /// based on the magnitude of the price.
        /// </summary>
        protected static decimal RoundProjection(decimal price)
        {
            if (price >= 10000m) return Math.Round(price, 0);
            if (price >= 1000m) return Math.Round(price, 1);
            if (price >= 100m) return Math.Round(price, 2);
            if (price >= 10m) return Math.Round(price, 2);
            if (price >= 1m) return Math.Round(price, 3);
            if (price >= 0.1m) return Math.Round(price, 4);
            return Math.Round(price, 5);
        }
    }
}
