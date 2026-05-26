using SmarterSystems.ElliottWaves.Analyzer.Data;
using SmarterSystems.ElliottWaves.Analyzer.Interfaces;

namespace SmarterSystems.ElliottWaves.Analyzer.Analysis
{
    /// <summary>
    /// Targeted pivot scanning for the forward-projection engine.
    /// Finds specific price extremes and Fibonacci-matching pivots in candle data.
    /// </summary>
    public static class PivotScanner
    {
        /// <summary>
        /// Find the absolute extreme (high or low) in a candle range.
        /// </summary>
        public static PivotPoint FindAbsoluteExtreme(
            List<Ohlcv> candles, bool findHigh,
            int fromIndex = 0, int toIndex = -1)
        {
            if (toIndex < 0) toIndex = candles.Count - 1;

            int bestIdx = fromIndex;
            decimal bestPrice = findHigh ? candles[fromIndex].High : candles[fromIndex].Low;

            for (int i = fromIndex + 1; i <= toIndex; i++)
            {
                decimal price = findHigh ? candles[i].High : candles[i].Low;
                if (findHigh ? price > bestPrice : price < bestPrice)
                {
                    bestPrice = price;
                    bestIdx = i;
                }
            }

            return CreatePivotPoint(bestIdx, candles, bestPrice, findHigh);
        }

        /// <summary>
        /// Find the absolute extreme between two timestamps (inclusive).
        /// Used for resolving coarse-timeframe points to finer resolution.
        /// </summary>
        public static PivotPoint FindAbsoluteExtremeBetween(
            List<Ohlcv> candles, bool findHigh,
            DateTime from, DateTime to)
        {
            PivotPoint best = null;

            for (int i = 0; i < candles.Count; i++)
            {
                if (candles[i].TimestampUtc < from) continue;
                if (candles[i].TimestampUtc > to) break;

                decimal price = findHigh ? candles[i].High : candles[i].Low;
                if (best == null
                    || (findHigh ? price > best.Price : price < best.Price))
                {
                    best = CreatePivotPoint(i, candles, price, findHigh);
                }
            }

            return best;
        }

        /// <summary>
        /// Scan forward from W0 to find W1 (FR2).
        /// W1 = last absolute extreme before a >50% retrace occurs.
        /// </summary>
        public static (PivotPoint W1, PivotPoint RetracePoint)? FindWave1(
            List<Ohlcv> candles, PivotPoint w0, bool isUptrend, int maxIndex = -1,
            decimal retraceThreshold = -1, int minGap = 0, decimal minLogMove = 0)
        {
            if (maxIndex < 0) maxIndex = candles.Count - 1;
            if (retraceThreshold < 0) retraceThreshold = Thresholds.W1ConfirmationRetrace;

            decimal runningExtreme = w0.Price;
            int runningExtremeIdx = w0.Index;
            var logW0 = WaveMath.Log(w0.Price);

            for (int i = w0.Index + 1; i <= maxIndex; i++)
            {
                decimal trendPrice = isUptrend ? candles[i].High : candles[i].Low;
                decimal retracePrice = isUptrend ? candles[i].Low : candles[i].High;

                // Track running extreme in trend direction
                if (isUptrend ? trendPrice > runningExtreme : trendPrice < runningExtreme)
                {
                    runningExtreme = trendPrice;
                    runningExtremeIdx = i;
                }

                // Check for retrace from the running extreme
                // The retrace must come from a candle AFTER the running extreme,
                // not the same candle (prevents intra-candle noise on coarse timeframes)
                if (runningExtremeIdx > w0.Index && i > runningExtremeIdx + minGap)
                {
                    var logExtreme = WaveMath.Log(runningExtreme);
                    var logRetrace = WaveMath.Log(retracePrice);
                    var move = Math.Abs(logExtreme - logW0);
                    var retrace = Math.Abs(logExtreme - logRetrace);

                    if (move >= minLogMove && move > 0 && retrace / move > retraceThreshold)
                    {
                        // W1 confirmed — find the actual absolute extreme between W0 and retrace point
                        var w1 = FindAbsoluteExtreme(candles, findHigh: isUptrend,
                            fromIndex: w0.Index, toIndex: i);

                        var retracePoint = CreatePivotPoint(i, candles, retracePrice, !isUptrend);

                        return (w1, retracePoint);
                    }
                }
            }

            return null; // W1 not confirmed — still in progress
        }

        /// <summary>
        /// Check whether the price recovers sufficiently after an extreme.
        /// Recovery is measured as a fraction of the move from the reference point to the extreme.
        /// The <paramref name="extremeIsHigh"/> parameter indicates whether the extreme is a high
        /// (recovery = price dropping, measured from lows) or a low (recovery = price rising, measured from highs).
        /// </summary>
        public static bool HasRecovery(
            List<Ohlcv> candles, int fromIndex, int toIndex,
            decimal logExtreme, decimal move,
            bool extremeIsHigh, decimal threshold)
        {
            for (int i = fromIndex + 1; i <= toIndex; i++)
            {
                decimal recoveryPrice = extremeIsHigh ? candles[i].Low : candles[i].High;
                var recovery = Math.Abs(WaveMath.Log(recoveryPrice) - logExtreme);
                if (move > 0 && recovery / move >= threshold)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Create a PivotPoint from candle data at the given index.
        /// </summary>
        private static PivotPoint CreatePivotPoint(int index, List<Ohlcv> candles, decimal price, bool isHigh)
        {
            return new PivotPoint
            {
                Index = index,
                Timestamp = candles[index].TimestampUtc,
                Price = price,
                PointType = isHigh ? PointType.High : PointType.Low
            };
        }
    }
}
