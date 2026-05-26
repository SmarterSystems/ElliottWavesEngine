using SmarterSystems.ElliottWaves.Analyzer.Data;
using SmarterSystems.ElliottWaves.Analyzer.Interfaces;

namespace SmarterSystems.ElliottWaves.Analyzer.Analysis
{
    /// <summary>
    /// Shared math utilities for Elliott Wave analysis.
    /// All price comparisons use log scale for crypto's exponential price moves.
    /// </summary>
    public static class WaveMath
    {
        /// <summary>
        /// Returns the natural logarithm of a decimal value.
        /// </summary>
        public static decimal Log(decimal value) =>
            value <= 0 ? throw new ArgumentOutOfRangeException(nameof(value), value, "Log requires a positive price.") :
            (decimal)Math.Log((double)value);

        /// <summary>
        /// Returns the absolute log-scale distance between two prices.
        /// Replaces the repeated pattern: Math.Abs(WaveMath.Log(a) - WaveMath.Log(b))
        /// </summary>
        public static decimal LogDistance(decimal priceA, decimal priceB) =>
            Math.Abs(Log(priceA) - Log(priceB));

        /// <summary>
        /// Returns true if priceA exceeds priceB in the trend direction.
        /// Uptrend: A > B. Downtrend: A &lt; B.
        /// </summary>
        public static bool ExceedsInDirection(decimal priceA, decimal priceB, bool isUptrend) =>
            isUptrend ? priceA > priceB : priceA < priceB;

        /// <summary>
        /// Auto-detects extension from 6 impulse pivots [start, W1end, W2end, W3end, W4end, W5end].
        /// A motive wave is extended if it exceeds ExtensionMultiple × both other motive waves (log scale).
        /// </summary>
        public static PatternSubType? DetectExtension(List<PivotPoint> pivots)
        {
            var w1Len = LogDistance(pivots[1].Price, pivots[0].Price);
            var w3Len = LogDistance(pivots[3].Price, pivots[2].Price);
            var w5Len = LogDistance(pivots[5].Price, pivots[4].Price);

            var ext = Thresholds.ExtensionMultiple;
            if (w1Len >= w3Len * ext && w1Len >= w5Len * ext) return PatternSubType.Extended1;
            if (w3Len >= w1Len * ext && w3Len >= w5Len * ext) return PatternSubType.Extended3;
            if (w5Len >= w1Len * ext && w5Len >= w3Len * ext) return PatternSubType.Extended5;
            return null;
        }

        /// <summary>
        /// Binary search for the candle index closest to a given timestamp.
        /// </summary>
        public static int FindCandleIndex(List<Ohlcv> candles, DateTime timestamp)
        {
            int lo = 0, hi = candles.Count - 1;
            while (lo <= hi)
            {
                int mid = (lo + hi) / 2;
                if (candles[mid].TimestampUtc == timestamp) return mid;
                if (candles[mid].TimestampUtc < timestamp) lo = mid + 1;
                else hi = mid - 1;
            }
            if (lo >= candles.Count) return candles.Count - 1;
            if (lo == 0) return 0;
            return Math.Abs((candles[lo].TimestampUtc - timestamp).Ticks)
                <= Math.Abs((candles[lo - 1].TimestampUtc - timestamp).Ticks) ? lo : lo - 1;
        }
    }
}
