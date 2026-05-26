using SmarterSystems.ElliottWaves.Analyzer.Data;
using SmarterSystems.ElliottWaves.Analyzer.Interfaces;

namespace SmarterSystems.ElliottWaves.Analyzer.Analysis
{
    /// <summary>
    /// Lower-degree wave detection within parent wave boundaries.
    /// Uses absolute extremes with confirmation (same approach as CycleDetector).
    /// For completed parents: deepest-retrace method to find W3/W4.
    /// For in-progress parents: forward confirmation scanning.
    /// Falls back from motive (5-wave) to corrective (A-B-C) when needed.
    /// </summary>
    public static class SubWaveDetector
    {

        public static List<ElliottWave> DetectSubWaves(
            List<Ohlcv> candles, ElliottWave parentWave, WaveDegree degree)
        {
            int startIdx = FindCandleIndex(candles, parentWave.StartPoint.Timestamp);
            int endIdx = parentWave.IsInProgress
                ? candles.Count - 1
                : FindCandleIndex(candles, parentWave.EndPoint.Timestamp);

            if (startIdx < 0 || endIdx < 0 || endIdx - startIdx < Thresholds.MinCandlesForSubWaves)
                return [];

            bool isMotive = WaveLabeler.IsMotiveLabel(parentWave.Label);

            // Parent direction
            bool isUptrend = parentWave.IsInProgress
                ? candles[endIdx].Close > parentWave.StartPoint.Price
                : parentWave.EndPoint.Price > parentWave.StartPoint.Price;

            if (isMotive)
            {
                var result = parentWave.IsInProgress
                    ? DetectMotiveInProgress(candles, parentWave, degree, startIdx, endIdx, isUptrend)
                    : DetectMotiveCompleted(candles, parentWave, degree, startIdx, endIdx, isUptrend);
                if (result != null && result.Count > 0) return result;

                // At Primary degree, corrective fallback is allowed because it signals
                // R5 relabeling: the cycle-level wave may need to morph from motive to
                // corrective (A-B-C) when its sub-waves are corrective.
                // At Intermediate degree, motive parents must have 5-wave structure.
                if (degree == WaveDegree.Primary)
                    return DetectCorrective(candles, parentWave, degree, startIdx, endIdx, isUptrend);

                // Intermediate: retry with relaxed thresholds before giving up.
                if (!parentWave.IsInProgress)
                {
                    var relaxed = DetectMotiveCompleted(candles, parentWave, degree,
                        startIdx, endIdx, isUptrend, relaxed: true);
                    if (relaxed != null && relaxed.Count > 0) return relaxed;
                }

                return [];
            }

            // Corrective detection — only for corrective parents (2, 4, A, B, C)
            return DetectCorrective(candles, parentWave, degree, startIdx, endIdx, isUptrend);
        }

        /// <summary>
        /// §8.3: Generate alternate motive interpretations with different W3/W4 positions.
        /// Returns up to maxAlternates alternate 5-wave decompositions.
        /// Uses the relaxed multi-pass loop from DetectMotiveCompleted to find
        /// different W3/W4 candidates beyond the preferred (first-found) one.
        /// </summary>
        public static List<List<ElliottWave>> TryAlternateMotivePositions(
            List<Ohlcv> candles, ElliottWave parentWave, WaveDegree degree, int maxAlternates = 2)
        {
            if (parentWave.IsInProgress) return [];

            int startIdx = FindCandleIndex(candles, parentWave.StartPoint.Timestamp);
            int endIdx = FindCandleIndex(candles, parentWave.EndPoint.Timestamp);
            if (startIdx < 0 || endIdx < 0 || endIdx - startIdx < Thresholds.MinCandlesForSubWaves)
                return [];

            bool isUptrend = parentWave.EndPoint.Price > parentWave.StartPoint.Price;
            var w0 = MakeW0(parentWave, startIdx);
            var parentLogMove = Math.Abs(WaveMath.Log(parentWave.EndPoint.Price) - WaveMath.Log(parentWave.StartPoint.Price));

            var alternates = new List<List<ElliottWave>>();
            bool isFirst = true;

            // Use the same multi-pass W3/W4 logic as DetectMotiveCompleted
            decimal[] w3Fractions = [Thresholds.MinMotiveWaveProportion, Thresholds.W3FractionRelaxedMid];
            foreach (var fraction in w3Fractions)
            {
                int searchEnd = endIdx;
                for (int attempt = 0; attempt < 3; attempt++)
                {
                    var w3w4 = FindDeepestSignificantRetrace(candles, isUptrend, startIdx, searchEnd,
                        parentLogMove, minFraction: fraction);
                    if (w3w4 == null) break;

                    var (w3, w4) = w3w4.Value;
                    if (w3.Index <= startIdx + Thresholds.ListingSpikeMaxStartOffset) break;

                    var result = TryBuildMotiveFromW3W4(candles, w0, w3, w4, parentWave, degree,
                        startIdx, endIdx, isUptrend, parentLogMove);

                    if (result != null)
                    {
                        if (isFirst)
                        {
                            isFirst = false; // Skip the first — it's the preferred count
                        }
                        else
                        {
                            alternates.Add(result);
                            if (alternates.Count >= maxAlternates) return alternates;
                        }
                    }

                    searchEnd = w3.Index - 1;
                }
            }

            return alternates;
        }

        /// <summary>
        /// §8.3: Attempt corrective (A-B-C) detection on a motive-labeled parent wave.
        /// Used by ElliottWavesAnalyzer to generate alternate counts.
        /// Returns the corrective interpretation or empty if detection fails.
        /// </summary>
        public static List<ElliottWave> TryAlternateCorrective(
            List<Ohlcv> candles, ElliottWave parentWave, WaveDegree degree)
        {
            int startIdx = FindCandleIndex(candles, parentWave.StartPoint.Timestamp);
            int endIdx = parentWave.IsInProgress
                ? candles.Count - 1
                : FindCandleIndex(candles, parentWave.EndPoint.Timestamp);

            if (startIdx < 0 || endIdx < 0 || endIdx - startIdx < Thresholds.MinCandlesForSubWaves)
                return [];

            bool isUptrend = parentWave.IsInProgress
                ? candles[endIdx].Close > parentWave.StartPoint.Price
                : parentWave.EndPoint.Price > parentWave.StartPoint.Price;

            return DetectCorrective(candles, parentWave, degree, startIdx, endIdx, isUptrend);
        }

        // ══════════════════════════════════════════════════
        // Motive detection — completed parent (5-wave)
        // Uses deepest-retrace method to find W3/W4 pair
        // ══════════════════════════════════════════════════

        private static List<ElliottWave> DetectMotiveCompleted(
            List<Ohlcv> candles, ElliottWave parent, WaveDegree degree,
            int startIdx, int endIdx, bool isUptrend,
            bool relaxed = false)
        {
            var w0 = MakeW0(parent, startIdx);
            var parentLogMove = Math.Abs(WaveMath.Log(parent.EndPoint.Price) - WaveMath.Log(parent.StartPoint.Price));
            var parentSpan = endIdx - startIdx;

            if (!relaxed)
            {
                // Pre-check: if the deepest retrace within the parent exceeds R50 of the parent
                // log-move, the internal structure is more naturally corrective A-B-C than motive.
                var deepest = FindDeepestRetrace(candles, isUptrend, startIdx, endIdx);
                if (deepest != null && parentLogMove > 0)
                {
                    var (peak, trough) = deepest.Value;
                    var deepRetraceLog = Math.Abs(WaveMath.Log(peak.Price) - WaveMath.Log(trough.Price));
                    if (deepRetraceLog / parentLogMove > Fibonacci.R50)
                        return null;
                }
            }

            // Multi-pass W3/W4 detection: try progressively lower thresholds.
            // In relaxed mode, add a lower threshold for waves with proportionally
            // shallow retraces (common in strong wave 3 impulses).
            decimal[] w3Fractions = relaxed
                ? [Thresholds.MinMotiveWaveProportion,
                   Thresholds.W3FractionRelaxedMid,
                   Thresholds.W3FractionRelaxedLow]
                : [Thresholds.MinMotiveWaveProportion,
                   Thresholds.W3FractionRelaxedMid];

            foreach (var fraction in w3Fractions)
            {
                // In relaxed mode, try multiple W3/W4 candidates: the deepest retrace
                // first, then progressively earlier retraces if the initial one fails
                // (e.g., R3 violation when W4 goes below W1). Strict mode uses a single
                // attempt to preserve existing Primary-degree behavior (R5 relabeling).
                int maxAttempts = relaxed ? 3 : 1;
                int searchEnd = endIdx;
                for (int attempt = 0; attempt < maxAttempts; attempt++)
                {
                    var w3w4 = FindDeepestSignificantRetrace(candles, isUptrend, startIdx, searchEnd,
                        parentLogMove, minFraction: fraction);
                    if (w3w4 == null) break;

                    var (w3, w4) = w3w4.Value;

                    // Late-retrace check: if W4 is in the last 5% of the parent time span,
                    // it's likely noise near the ATH, not a structural W4.
                    // Re-run the search with a truncated range to find an earlier structural retrace.
                    if (parentSpan > Thresholds.LateRetraceMinSpan && w4.Index > startIdx + (int)(parentSpan * Thresholds.LateRetraceThreshold))
                    {
                        int truncatedEnd = startIdx + (int)(parentSpan * Thresholds.LateRetraceTruncation);
                        var earlierW3W4 = FindDeepestSignificantRetrace(candles, isUptrend,
                            startIdx, truncatedEnd, parentLogMove, minFraction: fraction);
                        if (earlierW3W4 != null)
                        {
                            w3 = earlierW3W4.Value.Item1;
                            w4 = earlierW3W4.Value.Item2;
                        }
                        else
                        {
                            break;
                        }
                    }

                    // W3 position check: W3 must be a significant fraction of parent log distance from W0.
                    // Strict mode uses R618 to prevent misidentifying an early W1 peak as W3.
                    // Relaxed mode skips this check — crash-dominated waves (flash crash at the end)
                    // have all their gradual structure in the first 15-20% of the total move, which
                    // no position threshold can accommodate without being trivially permissive.
                    if (!relaxed)
                    {
                        var w3LogFromW0 = Math.Abs(WaveMath.Log(w3.Price) - WaveMath.Log(w0.Price));
                        if (parentLogMove > 0 && w3LogFromW0 / parentLogMove < Fibonacci.R618)
                        {
                            // W3 too early — retrying with earlier range won't help, skip fraction
                            break;
                        }
                    }

                    if (w3.Index <= startIdx + Thresholds.ListingSpikeMaxStartOffset)
                    {
                        // Deepest retrace peak is at the very start (e.g. listing spike where
                        // the first candle contains the entire initial move). Re-interpret as
                        // W1/W2 instead of W3/W4: the spike is W1, the deep retrace is W2.
                        // Then search for the real W3/W4 in the remaining data after W2.
                        if (relaxed)
                        {
                            var spikeResult = TryBuildMotiveFromListingSpike(
                                candles, w0, w3, w4, parent, degree,
                                startIdx, endIdx, isUptrend, parentLogMove);
                            if (spikeResult != null) return spikeResult;
                        }
                        break;
                    }

                    var result = TryBuildMotiveFromW3W4(candles, w0, w3, w4, parent, degree,
                        startIdx, endIdx, isUptrend, parentLogMove, relaxed);
                    if (result != null) return result;

                    // TryBuildMotiveFromW3W4 failed (likely R3 violation — W4 too deep for
                    // any W1 found before W3). Retry with the range truncated before this W3
                    // to find an earlier, shallower retrace where R3 can be satisfied.
                    searchEnd = w3.Index - 1;
                }
            }

            return null;
        }

        /// <summary>
        /// Given confirmed W3/W4, find W1/W2 via forward scan and build the 5-wave structure.
        /// Includes peak refinement: after finding W1, checks for a higher peak within
        /// R618 × W1_duration candles to handle multi-peak W1 structures.
        /// </summary>
        private static List<ElliottWave> TryBuildMotiveFromW3W4(
            List<Ohlcv> candles, PivotPoint w0, PivotPoint w3, PivotPoint w4,
            ElliottWave parent, WaveDegree degree,
            int startIdx, int endIdx, bool isUptrend, decimal parentLogMove,
            bool relaxed = false)
        {
            var w0ToW3Log = Math.Abs(WaveMath.Log(w3.Price) - WaveMath.Log(w0.Price));
            PivotPoint w1 = null, w2 = null;
            int searchStart = startIdx;

            while (searchStart < w3.Index - 2)
            {
                var w0Search = new PivotPoint
                {
                    Index = searchStart,
                    Timestamp = candles[searchStart].TimestampUtc,
                    Price = w0.Price,
                    PointType = w0.PointType
                };

                // Intermediate degree uses stricter thresholds to skip noise-level swings
                // and find structurally meaningful W1 pivots (consistent with DetectMotiveInProgress).
                // In relaxed mode, use Primary-degree thresholds: the stricter Intermediate
                // minGap/minLogMove can prevent finding W1 in compressed structures
                // (e.g. token listing spikes where most of the move is in the first few days).
                var w1Result = degree == WaveDegree.Intermediate && !relaxed
                    ? PivotScanner.FindWave1(candles, w0Search, isUptrend,
                        maxIndex: w3.Index, retraceThreshold: Fibonacci.R236,
                        minGap: Thresholds.IntermediateMinGap, minLogMove: Thresholds.IntermediateMinLogMove)
                    : PivotScanner.FindWave1(candles, w0Search, isUptrend,
                        maxIndex: w3.Index, retraceThreshold: Fibonacci.R236, minGap: Thresholds.PrimaryMinGap);
                if (w1Result == null) break;

                var candidateW1 = w1Result.Value.W1;
                var w1Log = Math.Abs(WaveMath.Log(candidateW1.Price) - WaveMath.Log(w0.Price));

                // Check 1: W1 must be ≥15% of W0→W3 log move
                if (w0ToW3Log > 0 && w1Log / w0ToW3Log < Thresholds.MinMotiveWaveProportion)
                {
                    searchStart = w1Result.Value.RetracePoint.Index;
                    continue;
                }

                // Find W2: absolute countertrend extreme between W1 and W3
                var candidateW2 = FindConfirmedExtreme(candles, candidateW1, !isUptrend,
                    candidateW1.Index + 1, w3.Index, excludeEndFromSearch: true);
                if (candidateW2 == null)
                {
                    searchStart = candidateW1.Index + 1;
                    continue;
                }

                // Check 2: W2→W3/W0→W1 ratio ≤ 4.236 (Fibonacci wave proportionality)
                var w2ToW3Log = Math.Abs(WaveMath.Log(w3.Price) - WaveMath.Log(candidateW2.Price));
                if (w1Log > 0 && w2ToW3Log / w1Log > Fibonacci.E4236)
                {
                    searchStart = w1Result.Value.RetracePoint.Index;
                    continue;
                }

                // R1: W2 must not violate W0
                if (isUptrend ? candidateW2.Price <= w0.Price : candidateW2.Price >= w0.Price)
                {
                    searchStart = candidateW2.Index + 1;
                    continue;
                }

                w1 = candidateW1;
                w2 = candidateW2;
                break;
            }

            if (w1 == null || w2 == null) return null;

            // Refine W1: the forward scan may find an early confirmed swing, but the
            // true W1 peak could be later (between the forward-scan W1 and W2).
            // Use the absolute trend extreme between W0 and W2 as the refined W1.
            var absW1 = PivotScanner.FindAbsoluteExtreme(candles, findHigh: isUptrend,
                fromIndex: startIdx, toIndex: w2.Index);
            if (WaveMath.ExceedsInDirection(absW1.Price, w1.Price, isUptrend))
            {
                w1 = absW1;
                // Re-find W2 from the refined W1 position
                var newW2 = FindConfirmedExtreme(candles, w1, !isUptrend,
                    w1.Index + 1, w3.Index, excludeEndFromSearch: true);
                if (newW2 != null && (isUptrend ? newW2.Price > w0.Price : newW2.Price < w0.Price))
                    w2 = newW2;
            }

            // W3 must exceed W1 in trend direction
            if (!WaveMath.ExceedsInDirection(w3.Price, w1.Price, isUptrend))
                return null;

            // W4 depth check: if W4 retraces > R618 of the W3 move (W2→W3 in log-scale),
            // the structure is more likely corrective A-B-C than motive impulse — reject.
            // Skipped in relaxed mode (last-resort motive detection for known motive parents).
            if (!relaxed)
            {
                var w3MoveLog = Math.Abs(WaveMath.Log(w3.Price) - WaveMath.Log(w2.Price));
                var w4RetLog = Math.Abs(WaveMath.Log(w3.Price) - WaveMath.Log(w4.Price));
                if (w3MoveLog > 0 && w4RetLog / w3MoveLog > Fibonacci.R618)
                    return null;
            }

            // R3: W4 must not overlap W1 territory
            if (isUptrend ? w4.Price < w1.Price : w4.Price > w1.Price)
            {
                // §3.4/§3.5: W4/W1 overlap is allowed in diagonals — check before rejecting
                var diagW5 = new PivotPoint
                {
                    Index = endIdx,
                    Timestamp = parent.EndPoint.Timestamp,
                    Price = parent.EndPoint.Price,
                    PointType = parent.EndPoint.PointType
                };
                var diagPivots = new List<PivotPoint> { w0, w1, w2, w3, w4, diagW5 };
                var diagJust = new List<FibonacciJustification> { null, null, null, null, null };
                return TryBuildDiagonal(diagPivots, diagJust, parent, degree, isUptrend);
            }

            // R1 enforcement: W4 must be the absolute countertrend extreme in [W3+1, end-1].
            // This ensures W5's lower-degree sub-waves cannot violate R1.
            // In relaxed mode, skip this enforcement — keep the original W4 that passed R3.
            // Extended W5 structures (where a deep retrace occurs within W5's range) would
            // otherwise prevent any motive partition from being found.
            if (!relaxed)
            {
                var absW4 = PivotScanner.FindAbsoluteExtreme(candles, findHigh: !isUptrend,
                    fromIndex: w3.Index + 1, toIndex: endIdx - 1);
                if (WaveMath.ExceedsInDirection(w4.Price, absW4.Price, isUptrend))
                {
                    if (isUptrend ? absW4.Price >= w1.Price : absW4.Price <= w1.Price)
                        w4 = absW4;
                    else
                        return null; // R3 violated with absolute countertrend extreme
                }
            }

            var pivots = new List<PivotPoint> { w0, w1, w2, w3, w4 };
            return BuildCompletedImpulse(pivots, degree, parent, endIdx, isUptrend);
        }

        /// <summary>
        /// Listing-spike fallback: when the deepest retrace peak is at the very start
        /// (first candle contains the entire initial move, e.g. token listing), treat the
        /// peak as W1 and the deepest retrace as W2, then find W3/W4 in the remaining data.
        /// </summary>
        private static List<ElliottWave> TryBuildMotiveFromListingSpike(
            List<Ohlcv> candles, PivotPoint w0, PivotPoint spikePeak, PivotPoint deepTrough,
            ElliottWave parent, WaveDegree degree,
            int startIdx, int endIdx, bool isUptrend, decimal parentLogMove)
        {
            // W1 = the spike peak, W2 = the deep retrace trough
            var w1 = spikePeak;
            var w2 = deepTrough;

            // R1: W2 must not violate W0
            if (isUptrend ? w2.Price <= w0.Price : w2.Price >= w0.Price)
                return null;

            // Listing spike: the intra-candle extreme (wick) is noise — the candle's close
            // represents where the market actually settled. Use close for structural checks
            // (W3>W1 and R3) while keeping the spike index for timeline positioning.
            var spikeCandle = candles[w1.Index];
            var effectiveW1Price = spikeCandle.Close;

            // Find W3/W4 in the remaining data (W2 to parent end)
            var remainingLogMove = Math.Abs(WaveMath.Log(parent.EndPoint.Price) - WaveMath.Log(w2.Price));
            var w3w4 = FindDeepestSignificantRetrace(candles, isUptrend, w2.Index, endIdx,
                remainingLogMove, minFraction: Fibonacci.R236 * Fibonacci.R236);
            if (w3w4 == null) return null;

            var (w3, w4) = w3w4.Value;

            // W3 must exceed W1 (using effective close price, not spike wick)
            if (!WaveMath.ExceedsInDirection(w3.Price, effectiveW1Price, isUptrend))
                return null;

            // R3 relaxed for listing spikes: the spike candle's close is still an anomalous
            // level (just the first hour's close), so minor W4/W1 overlap is structurally valid.
            // This produces a leading diagonal, which is allowed in wave 1/A position (§3.5).
            // Require only that W4 doesn't violate W0 (the absolute floor).
            if (isUptrend ? w4.Price <= w0.Price : w4.Price >= w0.Price)
                return null;

            // Use the effective (close-based) price for W1 in the pivot list —
            // the spike wick is not a structural level
            var effectiveW1 = new PivotPoint
            {
                Index = w1.Index,
                Timestamp = w1.Timestamp,
                Price = effectiveW1Price,
                PointType = w1.PointType
            };

            var pivots = new List<PivotPoint> { w0, effectiveW1, w2, w3, w4 };
            return BuildCompletedImpulse(pivots, degree, parent, endIdx, isUptrend);
        }

        // ══════════════════════════════════════════════════
        // Motive detection — in-progress parent
        // Forward scan with confirmation
        // ══════════════════════════════════════════════════

        private static List<ElliottWave> DetectMotiveInProgress(
            List<Ohlcv> candles, ElliottWave parent, WaveDegree degree,
            int startIdx, int endIdx, bool isUptrend)
        {
            var w0 = MakeW0(parent, startIdx);

            // Intermediate degree: use stricter thresholds to avoid catching hourly noise
            // as wave 1. R618 retrace + minimum gap + minimum log-move ensures the first
            // pivot is a genuine swing, not a micro-wiggle on the first day.
            var w1Result = degree == WaveDegree.Intermediate
                ? PivotScanner.FindWave1(candles, w0, isUptrend,
                    maxIndex: endIdx, retraceThreshold: Fibonacci.R618,
                    minGap: Thresholds.IntermediateMinGap, minLogMove: Thresholds.IntermediateMinLogMove)
                : PivotScanner.FindWave1(candles, w0, isUptrend,
                    maxIndex: endIdx, retraceThreshold: Fibonacci.R236);
            if (w1Result == null)
                return [WaveLabeler.CreateInProgressWave(
                    parent.StartPoint, WaveNumber.One, degree, null)];

            var w1 = w1Result.Value.W1;
            var pivots = new List<PivotPoint> { w0, w1 };
            var justifications = new List<FibonacciJustification> { null };
            bool r1Enforced = false;

            // W2: absolute countertrend extreme after W1
            var w2 = FindConfirmedExtreme(candles, w1, !isUptrend,
                w1.Index + 1, endIdx);
            if (w2 == null)
            {
                var w2Proj = ProjectionBuilder.BuildRetracement(w0.Price, w1.Price,
                    FibonacciProjector.W2RetracementLevels, w0.Price);
                return WaveLabeler.CreateImpulseWaves(pivots, justifications, degree,
                    inProgressLabel: WaveNumber.Two,
                    inProgressProjection: w2Proj,
                    inProgressStart: w1);
            }

            // R1 check
            if (isUptrend ? w2.Price <= w0.Price : w2.Price >= w0.Price)
                return TryAthAsMotive(candles, w0, degree, startIdx, endIdx, isUptrend);

            // R1 enforcement: W1 must be the absolute trend extreme in [W0+1, W2].
            // If a price in this range exceeds W1, lower-degree sub-waves within W2
            // would encounter a price beyond their W0 (= W1 End), violating R1.
            var absW1Check = PivotScanner.FindAbsoluteExtreme(candles, findHigh: isUptrend,
                fromIndex: startIdx + 1, toIndex: w2.Index);
            if (WaveMath.ExceedsInDirection(absW1Check.Price, w1.Price, isUptrend))
            {
                w1 = absW1Check;
                pivots[1] = w1;
                r1Enforced = true;

                var refinedW2 = FindConfirmedExtreme(candles, w1, !isUptrend,
                    w1.Index + 1, endIdx);
                if (refinedW2 == null)
                {
                    var w2Proj = ProjectionBuilder.BuildRetracement(w0.Price, w1.Price,
                        FibonacciProjector.W2RetracementLevels, w0.Price);
                    return WaveLabeler.CreateImpulseWaves(pivots, justifications, degree,
                        inProgressLabel: WaveNumber.Two,
                        inProgressProjection: w2Proj,
                        inProgressStart: w1);
                }
                if (isUptrend ? refinedW2.Price <= w0.Price : refinedW2.Price >= w0.Price)
                    return TryAthAsMotive(candles, w0, degree, startIdx, endIdx, isUptrend);
                w2 = refinedW2;
            }

            pivots.Add(w2);
            justifications.Add(null);

            // W3: first confirmed swing beyond W1
            var w3 = FindConfirmedSwingBeyond(candles, w2, w1, isUptrend,
                w2.Index, endIdx);
            if (w3 == null)
                return BuildW3InProgress(pivots, justifications, degree, w0, w1, w2, candles[endIdx].Close);

            pivots.Add(w3);
            justifications.Add(null);

            // W4: confirmed countertrend extreme after W3
            var w4 = FindConfirmedExtreme(candles, w3, !isUptrend,
                w3.Index + 1, endIdx);
            if (w4 == null)
            {
                var w4Proj = ProjectionBuilder.BuildRetracement(w2.Price, w3.Price,
                    FibonacciProjector.W4RetracementLevels, w0.Price);
                return WaveLabeler.CreateImpulseWaves(pivots, justifications, degree,
                    inProgressLabel: WaveNumber.Four,
                    inProgressProjection: w4Proj,
                    inProgressStart: w3);
            }

            // R3: W4 must not overlap W1
            if (isUptrend ? w4.Price < w1.Price : w4.Price > w1.Price)
            {
                // At Intermediate degree, skip diagonal detection on R3 violation.
                // Early hourly structure often triggers ending-diagonal acceptance
                // (converging amplitudes) on what is really noise within a larger W1.
                // TryAthAsMotive with stricter thresholds finds the genuine first swing.
                //
                // Exception: when R1 enforcement moved W1 to a deeper level (e.g. a
                // flash-crash extreme), the W1 placement is structurally correct but
                // the extreme depth makes R3 violations inevitable. Reverting via
                // TryAthAsMotive would undo the proportionally correct W1. Instead,
                // discard the invalid W3/W4 and return W3 as in-progress.
                if (degree == WaveDegree.Intermediate)
                {
                    if (r1Enforced)
                    {
                        pivots.RemoveRange(3, pivots.Count - 3);
                        justifications.RemoveRange(2, justifications.Count - 2);
                        return BuildW3InProgress(pivots, justifications, degree, w0, w1, w2, candles[endIdx].Close);
                    }
                    return TryAthAsMotive(candles, w0, degree, startIdx, endIdx, isUptrend);
                }

                // §3.4/§3.5: Check for diagonal before falling back to ATH approach
                var diagPivots = new List<PivotPoint> { w0, w1, w2, w3, w4 };
                var diagJust = new List<FibonacciJustification> { null, null, null, null };
                var diagW5Proj = ProjectionBuilder.BuildW5(w0, w1, w4, invalidation: w3.Price);
                var diagResult = TryBuildDiagonal(diagPivots, diagJust, parent, degree,
                    isUptrend, w5InProgress: true, inProgressProjection: diagW5Proj);
                if (diagResult != null) return diagResult;
                return TryAthAsMotive(candles, w0, degree, startIdx, endIdx, isUptrend);
            }

            // R1 enforcement: W4 must be the absolute countertrend extreme in [W3+1, end].
            // Ensures W5's lower-degree sub-waves cannot violate R1.
            var absW4Check = PivotScanner.FindAbsoluteExtreme(candles, findHigh: !isUptrend,
                fromIndex: w3.Index + 1, toIndex: endIdx);
            if (absW4Check.Index < endIdx
                && WaveMath.ExceedsInDirection(w4.Price, absW4Check.Price, isUptrend))
            {
                if (isUptrend ? absW4Check.Price >= w1.Price : absW4Check.Price <= w1.Price)
                    w4 = absW4Check;
                else
                    return TryAthAsMotive(candles, w0, degree, startIdx, endIdx, isUptrend);
            }

            pivots.Add(w4);
            justifications.Add(null);

            // Proportionality check: if W1 is tiny relative to the total move (W0→ATH),
            // the 5-wave structure has a dominant W3 and negligible W1. This means the
            // entire impulse is better described as a single large W1 (ATH approach).
            // Only applies when the normal path didn't hit R1/R3 violations.
            // Uses R382 for W2 confirmation since the disproportionate W1 indicates
            // we're likely still in the first major impulse, not past W2 already.
            var totalExtreme = PivotScanner.FindAbsoluteExtreme(candles, findHigh: isUptrend,
                fromIndex: startIdx, toIndex: endIdx);
            var w0ToTotalLog = Math.Abs(WaveMath.Log(totalExtreme.Price) - WaveMath.Log(w0.Price));
            var w0ToW1Log = Math.Abs(WaveMath.Log(w1.Price) - WaveMath.Log(w0.Price));
            if (w0ToTotalLog > 0 && w0ToW1Log / w0ToTotalLog < Thresholds.MinMotiveWaveProportion)
            {
                var altW1 = FindConfirmedExtreme(candles, w0, isUptrend,
                    startIdx + 1, endIdx);
                if (altW1 != null)
                {
                    var altW2 = FindConfirmedExtreme(candles, altW1, !isUptrend,
                        altW1.Index + 1, endIdx, recoveryThreshold: Fibonacci.R382);
                    return BuildInProgressFromW1(candles, w0, altW1, altW2,
                        degree, endIdx, isUptrend);
                }
            }

            // W5 in-progress (invalidation = W3 end — W5 must exceed W3)
            var w5Proj = ProjectionBuilder.BuildW5(w0, w1, w4, invalidation: w3.Price);
            return WaveLabeler.CreateImpulseWaves(pivots, justifications, degree,
                inProgressLabel: WaveNumber.Five,
                inProgressProjection: w5Proj,
                inProgressStart: w4);
        }

        /// <summary>
        /// Fallback for in-progress motive: when 5-wave detection fails (R1/R3 violated),
        /// try treating the absolute trend extreme as W1. The entire impulse up to the ATH
        /// is W1, and the correction after it is W2. This handles cases where FindWave1
        /// finds a small early W1, leading to later R3 violations.
        /// </summary>
        private static List<ElliottWave> TryAthAsMotive(
            List<Ohlcv> candles, PivotPoint w0, WaveDegree degree,
            int startIdx, int endIdx, bool isUptrend)
        {
            // For Intermediate degree, prefer the first significant extreme (R50
            // confirmation + minimum log-move) over the absolute extreme.
            // FindConfirmedExtreme always picks the deepest point in the entire range,
            // which for in-progress parents can span years of hourly data and produce a
            // wave-1 that swallows the whole move. FindWave1 with stricter thresholds
            // finds the first pivot that is both deep enough and confirmed by a
            // meaningful countertrend bounce.
            // For Primary degree, keep the original absolute-extreme approach.
            PivotPoint altW1;
            if (degree == WaveDegree.Intermediate)
            {
                var w1Result = PivotScanner.FindWave1(candles, w0, isUptrend,
                    maxIndex: endIdx, retraceThreshold: Fibonacci.R618,
                    minGap: Thresholds.IntermediateMinGap, minLogMove: Thresholds.IntermediateMinLogMoveStrict);
                if (w1Result != null)
                {
                    altW1 = w1Result.Value.W1;
                }
                else
                {
                    altW1 = FindConfirmedExtreme(candles, w0, isUptrend,
                        startIdx + 1, endIdx);
                    if (altW1 == null) return null;
                }
            }
            else
            {
                altW1 = FindConfirmedExtreme(candles, w0, isUptrend,
                    startIdx + 1, endIdx);
                if (altW1 == null) return null;
            }

            var altW2 = FindConfirmedExtreme(candles, altW1, !isUptrend,
                altW1.Index + 1, endIdx);

            if (altW2 != null && (isUptrend ? altW2.Price > w0.Price : altW2.Price < w0.Price))
            {
                // Time proportionality: W1 and W2 should be roughly proportional.
                // When W2 is vastly longer than W1 (>3×), the initial W1 is too shallow —
                // a deeper extreme within the W0–W2 range is the structurally correct W1.
                if (degree == WaveDegree.Intermediate)
                {
                    int w1Duration = altW1.Index - w0.Index;
                    int w2Duration = altW2.Index - altW1.Index;
                    if (w1Duration > 0 && w2Duration > Thresholds.MaxW2W1DurationRatio * w1Duration)
                    {
                        var deeperW1 = PivotScanner.FindAbsoluteExtreme(candles,
                            findHigh: isUptrend,
                            fromIndex: w0.Index + 1, toIndex: altW2.Index);
                        if (WaveMath.ExceedsInDirection(deeperW1.Price, altW1.Price, isUptrend))
                        {
                            altW1 = deeperW1;
                            var deeperW2 = FindConfirmedExtreme(candles, altW1, !isUptrend,
                                altW1.Index + 1, endIdx);
                            if (deeperW2 != null)
                                altW2 = deeperW2;
                        }
                    }
                }
            }

            return BuildInProgressFromW1(candles, w0, altW1, altW2,
                degree, endIdx, isUptrend);
        }

        // ══════════════════════════════════════════════════
        // Corrective detection (A-B-C)
        // Used directly for corrective parents or as fallback from motive
        // ══════════════════════════════════════════════════

        private static List<ElliottWave> DetectCorrective(
            List<Ohlcv> candles, ElliottWave parent, WaveDegree degree,
            int startIdx, int endIdx, bool isUptrend)
        {
            var w0 = MakeW0(parent, startIdx);

            if (parent.IsInProgress)
                return DetectCorrectiveInProgress(candles, parent, degree, startIdx, endIdx, isUptrend, w0);

            return DetectCorrectiveCompleted(candles, parent, degree, startIdx, endIdx, isUptrend, w0);
        }

        private static List<ElliottWave> DetectCorrectiveCompleted(
            List<Ohlcv> candles, ElliottWave parent, WaveDegree degree,
            int startIdx, int endIdx, bool isUptrend, PivotPoint w0)
        {
            var parentLogMove = Math.Abs(WaveMath.Log(parent.EndPoint.Price) - WaveMath.Log(parent.StartPoint.Price));

            // Triangle detection: look for A-B-C-D-E with overlapping, converging amplitudes.
            // Triangles are sideways patterns where each successive wave is smaller.
            var triangleResult = DetectTriangle(candles, parent, degree, startIdx, endIdx, isUptrend, w0, parentLogMove);
            if (triangleResult != null)
                return triangleResult;

            // Strategy 1: deepest retrace identifies the A→B boundary.
            // Verify A covers at least R382 (38.2%) of the parent move.
            var deepResult = FindDeepestRetrace(candles, isUptrend, startIdx, endIdx);
            if (deepResult != null)
            {
                var (waveA, waveB) = deepResult.Value;

                // "Double-top" B refinement: scan backward from parent end to find the
                // LAST candle where the countertrend price reaches near the absolute B peak.
                // This picks the structural B→C boundary in patterns where two near-identical
                // peaks exist (e.g., two near-identical countertrend highs months apart ≈ 2% diff).
                waveB = RefineToLastNearPeak(candles, waveB, isUptrend, endIdx);

                var aDepth = Math.Abs(WaveMath.Log(waveA.Price) - WaveMath.Log(w0.Price));

                if (waveB.Index < endIdx && parentLogMove > 0
                    && aDepth / parentLogMove >= Fibonacci.R382)
                {
                    return BuildCorrectiveFromAB(w0, waveA, waveB, parent, degree, endIdx, isUptrend, candles);
                }
            }

            // Strategy 2: last significant retrace with Fibonacci-derived fraction.
            // R236 × R618 ≈ 14.6% — product of two Fibonacci levels.
            var lsResult = FindDeepestSignificantRetrace(
                candles, isUptrend, startIdx, endIdx, parentLogMove,
                minFraction: Thresholds.CorrectiveMinRetraceFraction);
            if (lsResult != null)
            {
                var (waveA, waveB) = lsResult.Value;

                // Same "double-top" refinement as Strategy 1
                waveB = RefineToLastNearPeak(candles, waveB, isUptrend, endIdx);

                if (waveB.Index < endIdx)
                    return BuildCorrectiveFromAB(w0, waveA, waveB, parent, degree, endIdx, isUptrend, candles);
            }

            // Strategy 3: forward scan fallback
            var aResult = PivotScanner.FindWave1(candles, w0, isUptrend,
                maxIndex: endIdx, retraceThreshold: Fibonacci.R236, minGap: Thresholds.PrimaryMinGap);

            if (aResult == null)
            {
                return [new ElliottWave
                {
                    Degree = degree, Label = WaveNumber.A,
                    StartPoint = parent.StartPoint, EndPoint = parent.EndPoint,
                    IsInProgress = false, PatternType = PatternType.ZigZag
                }];
            }

            var fallbackA = aResult.Value.W1;
            var fallbackB = FindConfirmedExtreme(candles, fallbackA, !isUptrend,
                fallbackA.Index + 1, endIdx, excludeEndFromSearch: true);

            if (fallbackB == null)
            {
                var fbPivots = new List<PivotPoint>
                {
                    w0,
                    new PivotPoint
                    {
                        Index = endIdx, Timestamp = parent.EndPoint.Timestamp,
                        Price = parent.EndPoint.Price, PointType = parent.EndPoint.PointType
                    }
                };
                return WaveLabeler.CreateCorrectiveWaves(fbPivots,
                    [null], degree, PatternType.ZigZag);
            }

            return BuildCorrectiveFromAB(w0, fallbackA, fallbackB, parent, degree, endIdx, isUptrend, candles);
        }

        private static List<ElliottWave> BuildCorrectiveFromAB(
            PivotPoint w0, PivotPoint waveA, PivotPoint waveB,
            ElliottWave parent, WaveDegree degree, int endIdx, bool isUptrend,
            List<Ohlcv> candles = null)
        {
            // §4.4: Try W-X-Y complex combination before defaulting to simple A-B-C.
            // If B is in the first 50% of the parent's candle range, there may be
            // a second corrective segment (Y) after the first (W=A-B-C).
            int parentSpan = endIdx - w0.Index;
            if (candles != null && parentSpan > 0 && waveB.Index - w0.Index < parentSpan * 0.50m)
            {
                var complex = TryDetectComplexCombination(
                    candles, w0, waveA, waveB, parent, degree, endIdx, isUptrend);
                if (complex != null)
                    return complex;
            }

            var pivots = new List<PivotPoint> { w0, waveA, waveB };
            var justifications = new List<FibonacciJustification> { null, null };

            var waveC = new PivotPoint
            {
                Index = endIdx,
                Timestamp = parent.EndPoint.Timestamp,
                Price = parent.EndPoint.Price,
                PointType = parent.EndPoint.PointType
            };
            pivots.Add(waveC);
            justifications.Add(null);

            // §4.2: Classify as ZigZag or Flat based on B wave behavior
            var (patternType, patternSubType) = ClassifyCorrective(w0, waveA, waveB, isUptrend, waveC);

            return WaveLabeler.CreateCorrectiveWaves(pivots, justifications,
                degree, patternType, subType: patternSubType);
        }

        /// <summary>
        /// §4.4: Detect W-X-Y double combination at sub-wave level.
        /// W = first corrective segment (A-B-C already found), X = connecting wave,
        /// Y = second corrective segment ending at parent endpoint.
        /// Validates sideways structure: Y end doesn't significantly exceed W start.
        /// </summary>
        private static List<ElliottWave> TryDetectComplexCombination(
            List<Ohlcv> candles, PivotPoint w0, PivotPoint waveA, PivotPoint waveB,
            ElliottWave parent, WaveDegree degree, int endIdx, bool isUptrend)
        {

            // Find C of W: extreme in correction direction (= isUptrend direction) between B and end
            var waveC = FindConfirmedExtreme(candles, waveB, isUptrend,
                waveB.Index + 1, endIdx, excludeEndFromSearch: true);
            if (waveC == null) return null;

            // W segment = w0 → waveA → waveB → waveC
            // Now find X wave: countertrend move from waveC
            var waveX = FindConfirmedExtreme(candles, waveC, !isUptrend,
                waveC.Index + 1, endIdx, excludeEndFromSearch: true);
            if (waveX == null) return null;

            // Y ends at parent endpoint
            var waveYEnd = new PivotPoint
            {
                Index = endIdx,
                Timestamp = parent.EndPoint.Timestamp,
                Price = parent.EndPoint.Price,
                PointType = parent.EndPoint.PointType
            };

            // Validate X is a meaningful retrace of W (at least 38.2%)
            var wLogMove = WaveMath.LogDistance(w0.Price, waveC.Price);
            var xLogMove = WaveMath.LogDistance(waveC.Price, waveX.Price);
            if (wLogMove == 0 || xLogMove / wLogMove < Fibonacci.R382)
                return null;

            // Validate Y is a substantial move (at least 50% of W amplitude)
            var yLogMove = WaveMath.LogDistance(waveX.Price, waveYEnd.Price);
            if (yLogMove < wLogMove * 0.50m)
                return null;

            // Validate X is a connecting wave (smaller than both W and Y)
            if (xLogMove >= wLogMove || xLogMove >= yLogMove)
                return null;

            // Validate sideways: Y end doesn't exceed W start significantly in trend direction
            // (W-X-Y is a sideways pattern — net progress should be limited)
            bool downtrend = waveA.Price < w0.Price;
            var logStart = WaveMath.Log(w0.Price);
            var logYEnd = WaveMath.Log(waveYEnd.Price);
            decimal netProgress = downtrend
                ? logStart - logYEnd
                : logYEnd - logStart;

            if (netProgress > wLogMove * Thresholds.DoubleComboMaxProgress)
                return null;

            // Build W-X-Y: pivots are [W_start, W_end(=C), X_end, Y_end]
            var pivots = new List<PivotPoint> { w0, waveC, waveX, waveYEnd };
            var justifications = new List<FibonacciJustification> { null, null, null };

            // Classify complex sub-type based on W and Y internal structure
            var wIsZigzag = !WaveMath.ExceedsInDirection(waveB.Price, w0.Price,
                isUptrend ? false : true); // simplified: B doesn't reach A start → zigzag-like

            return WaveLabeler.CreateCorrectiveWaves(pivots, justifications,
                degree, PatternType.Complex, subType: wIsZigzag ? PatternSubType.DoubleZigzag : null);
        }


        private static List<ElliottWave> DetectCorrectiveInProgress(
            List<Ohlcv> candles, ElliottWave parent, WaveDegree degree,
            int startIdx, int endIdx, bool isUptrend, PivotPoint w0)
        {
            // Find Wave A — absolute extreme in correction direction, confirmed by
            // substantial recovery (R236 = 23.6% of the A move). Using a higher
            // threshold than the default R118 (11.8%) prevents picking a recent deep
            // extreme with only minor bounce (e.g., 11% recovery) over an earlier
            // structurally significant A wave (e.g., 55% recovery).
            var waveA = FindConfirmedExtreme(candles, w0, isUptrend,
                startIdx + 1, endIdx, recoveryThreshold: Fibonacci.R236);

            // Fallback: find first significant retrace (R382 of move) as A-B pair.
            // This handles the case where the absolute extreme has insufficient recovery.
            if (waveA == null)
            {
                var estimatedTotalLog = Math.Abs(
                    WaveMath.Log(candles[endIdx].Close) - WaveMath.Log(parent.StartPoint.Price));
                var minMoveLog = Fibonacci.R382 * Math.Max(estimatedTotalLog, Thresholds.CorrectiveFallbackMinLogMove);
                var firstRetrace = FindFirstSignificantRetrace(
                    candles, isUptrend, startIdx, endIdx, Fibonacci.R382, minMoveLog);
                if (firstRetrace != null)
                    waveA = firstRetrace.Value.peak;
            }

            // Last resort: original 10% recovery on absolute extreme
            waveA ??= FindConfirmedExtreme(candles, w0, isUptrend,
                startIdx + 1, endIdx);

            if (waveA == null)
            {
                return [WaveLabeler.CreateInProgressWave(
                    parent.StartPoint, WaveNumber.A, degree, null, PatternType.ZigZag)];
            }

            var pivots = new List<PivotPoint> { w0, waveA };
            var justifications = new List<FibonacciJustification> { null };

            // Find Wave B — absolute countertrend extreme confirmed by recovery.
            var waveB = FindConfirmedExtreme(candles, waveA, !isUptrend,
                waveA.Index + 1, endIdx);

            if (waveB == null)
            {
                var bProj = ProjectionBuilder.BuildRetracement(w0.Price, waveA.Price,
                    FibonacciProjector.BRetracementLevels, w0.Price);
                return WaveLabeler.CreateCorrectiveWaves(pivots, justifications,
                    degree, PatternType.ZigZag,
                    inProgressLabel: WaveNumber.B,
                    inProgressProjection: bProj,
                    inProgressStart: waveA);
            }

            pivots.Add(waveB);
            justifications.Add(null);

            // §4.2: Classify based on B vs A start (C not yet known)
            var (patternType, _) = ClassifyCorrective(w0, waveA, waveB, isUptrend);

            // C in-progress with A=C projection
            bool isIntermC = degree == WaveDegree.Intermediate;
            var cLevels = isIntermC
                ? FibonacciProjector.IntermediateCExtensionLevels
                : FibonacciProjector.CExtensionLevels;
            var cProj = ProjectionBuilder.BuildExtension(w0.Price, waveA.Price, waveB.Price,
                cLevels, w0.Price, isIntermediate: isIntermC,
                currentPrice: candles[endIdx].Close);
            return WaveLabeler.CreateCorrectiveWaves(pivots, justifications,
                degree, patternType,
                inProgressLabel: WaveNumber.C,
                inProgressProjection: cProj,
                inProgressStart: waveB);
        }

        // ══════════════════════════════════════════════════
        // Triangle detection (A-B-C-D-E)
        // ══════════════════════════════════════════════════

        /// <summary>
        /// Detect triangle pattern (A-B-C-D-E) within a corrective parent.
        /// Triangles have 5 alternating waves with overlapping price ranges
        /// and converging (or expanding) amplitudes.
        /// Returns null if triangle pattern is not found.
        /// </summary>
        private static List<ElliottWave> DetectTriangle(
            List<Ohlcv> candles, ElliottWave parent, WaveDegree degree,
            int startIdx, int endIdx, bool isUptrend, PivotPoint w0,
            decimal parentLogMove)
        {
            // Need sufficient candles for 5 waves
            if (endIdx - startIdx < 10) return null;

            // Find alternating pivots: A (trend extreme), B (countertrend), C, D, E
            var pivots = new List<PivotPoint> { w0 };
            int searchFrom = startIdx + 1;
            bool findTrend = true; // A is in trend direction (countertrend of parent)

            for (int wave = 0; wave < 5 && searchFrom < endIdx; wave++)
            {
                bool findHigh = isUptrend ? !findTrend : findTrend;
                int searchTo = wave < 4 ? endIdx - 1 : endIdx; // E can be at parent end

                var extreme = FindConfirmedExtreme(candles, pivots[^1], findHigh,
                    searchFrom, searchTo,
                    recoveryThreshold: wave < 4 ? Fibonacci.R118 : 0,
                    excludeEndFromSearch: wave < 4);

                if (extreme == null)
                {
                    // For wave E, use parent endpoint if no confirmed extreme found
                    if (wave == 4)
                    {
                        extreme = new PivotPoint
                        {
                            Index = endIdx,
                            Timestamp = parent.EndPoint.Timestamp,
                            Price = parent.EndPoint.Price,
                            PointType = parent.EndPoint.PointType
                        };
                    }
                    else
                        return null;
                }

                pivots.Add(extreme);
                searchFrom = extreme.Index + 1;
                findTrend = !findTrend;
            }

            if (pivots.Count != 6) return null; // Need W0 + A + B + C + D + E

            // Verify triangle characteristics:
            // 1. Overlapping waves — A and C price ranges must overlap
            decimal aLow = Math.Min(pivots[0].Price, pivots[1].Price);
            decimal aHigh = Math.Max(pivots[0].Price, pivots[1].Price);
            decimal cLow = Math.Min(pivots[2].Price, pivots[3].Price);
            decimal cHigh = Math.Max(pivots[2].Price, pivots[3].Price);
            if (Math.Max(aLow, cLow) >= Math.Min(aHigh, cHigh))
                return null; // No overlap — not a triangle

            // 2. Converging or expanding amplitudes (at least 60% of successive pairs)
            var amps = new decimal[5];
            for (int i = 0; i < 5; i++)
                amps[i] = WaveMath.LogDistance(pivots[i].Price, pivots[i + 1].Price);

            int convergingCount = 0, expandingCount = 0;
            for (int i = 0; i < 4; i++)
            {
                if (amps[i + 1] < amps[i]) convergingCount++;
                if (amps[i + 1] > amps[i]) expandingCount++;
            }

            bool isConverging = convergingCount >= 4 * Thresholds.ConvergenceMajorityRatio;
            bool isExpanding = expandingCount >= 4 * Thresholds.ExpandingMajorityRatio;

            if (!isConverging && !isExpanding)
                return null; // Neither converging nor expanding

            // 3. Each wave should be significant (at least R118 of parent move)
            decimal minAmp = parentLogMove * Fibonacci.R118;
            foreach (var amp in amps)
            {
                if (amp < minAmp) return null;
            }

            // Classify triangle sub-type
            PatternSubType subType = ClassifyTriangleSubTypeFromPivots(pivots, isUptrend, isExpanding);

            var justifications = Enumerable.Repeat<FibonacciJustification>(null, 5).ToList();
            return WaveLabeler.CreateCorrectiveWaves(pivots, justifications,
                degree, PatternType.Triangle, subType: subType);
        }

        /// <summary>
        /// Classify triangle sub-type from pivot geometry.
        /// </summary>
        private static PatternSubType ClassifyTriangleSubTypeFromPivots(
            List<PivotPoint> pivots, bool isUptrend, bool isExpanding)
        {
            // Running triangle: B exceeds A start
            var aStart = pivots[0].Price;
            var bEnd = pivots[2].Price;
            bool aDown = pivots[1].Price < pivots[0].Price;
            if (aDown ? bEnd > aStart : bEnd < aStart)
                return PatternSubType.RunningTriangle;

            if (isExpanding)
                return PatternSubType.ExpandingTriangle;

            // Contracting — classify by trendline geometry
            var aEnd = pivots[1].Price;
            var cEnd = pivots[3].Price;
            var dEnd = pivots[4].Price;

            var totalRange = Math.Max(
                WaveMath.LogDistance(aStart, aEnd),
                Thresholds.MinTriangleRange);
            var flatThreshold = totalRange * Thresholds.TriangleFlatThreshold;

            var upperDelta = WaveMath.LogDistance(bEnd, dEnd);
            var lowerDelta = WaveMath.LogDistance(aEnd, cEnd);

            bool upperFlat = upperDelta < flatThreshold;
            bool lowerFlat = lowerDelta < flatThreshold;

            if (upperFlat && !lowerFlat)
                return PatternSubType.ContractingAscending;
            if (lowerFlat && !upperFlat)
                return PatternSubType.ContractingDescending;

            return PatternSubType.ContractingSymmetrical;
        }

        // ══════════════════════════════════════════════════
        // Pivot finding methods
        // ══════════════════════════════════════════════════

        /// <summary>
        /// Find the last retrace exceeding minFraction of the reference log-move,
        /// with a W5-significance filter: the remaining move from the trough to parent end
        /// must be at least R236 of the reference log-move. This prevents capturing
        /// noise near the parent end as W3/W4.
        /// Returns (peak, trough) where peak is the extreme before the retrace and
        /// trough is the countertrend extreme after the peak.
        /// For motive parents: peak=W3, trough=W4.
        /// For corrective parents: peak=WaveA, trough=WaveB.
        /// </summary>
        private static (PivotPoint peak, PivotPoint trough)? FindDeepestSignificantRetrace(
            List<Ohlcv> candles, bool isUptrend, int fromIndex, int toIndex,
            decimal referenceLogMove, decimal minFraction = Thresholds.MinMotiveWaveProportion)
        {
            var minRetrace = minFraction * referenceLogMove;

            decimal runningExtreme = isUptrend ? candles[fromIndex].High : candles[fromIndex].Low;
            int runningExtremeIdx = fromIndex;

            (int peakIdx, decimal peakPrice, int troughIdx, decimal troughPrice)? lastSignificant = null;
            (int peakIdx, decimal peakPrice, int troughIdx, decimal troughPrice)? prevSignificant = null;

            for (int i = fromIndex + 1; i <= toIndex; i++)
            {
                decimal trendPrice = isUptrend ? candles[i].High : candles[i].Low;
                decimal retracePrice = isUptrend ? candles[i].Low : candles[i].High;

                // Track running extreme in trend direction
                if (isUptrend ? trendPrice > runningExtreme : trendPrice < runningExtreme)
                {
                    runningExtreme = trendPrice;
                    runningExtremeIdx = i;
                }

                // Check retrace on candles after the running extreme (at least 2 candle gap)
                if (i > runningExtremeIdx + 1)
                {
                    var depth = Math.Abs(WaveMath.Log(runningExtreme) - WaveMath.Log(retracePrice));
                    if (depth >= minRetrace)
                    {
                        // Find the actual countertrend extreme from peak to here
                        var troughPoint = PivotScanner.FindAbsoluteExtreme(
                            candles, findHigh: !isUptrend,
                            fromIndex: runningExtremeIdx + 1, toIndex: i);

                        prevSignificant = lastSignificant;
                        lastSignificant = (runningExtremeIdx, runningExtreme,
                            troughPoint.Index, troughPoint.Price);
                    }
                }
            }

            if (lastSignificant == null) return null;

            var ls = lastSignificant.Value;

            // Peak = absolute trend extreme from start to peak area
            var peak = PivotScanner.FindAbsoluteExtreme(
                candles, findHigh: isUptrend,
                fromIndex: fromIndex, toIndex: ls.peakIdx);

            // Trough = absolute countertrend extreme from peak+1 to toIndex
            // (but it must not be at parent end — need room for W5/C)
            var trough = PivotScanner.FindAbsoluteExtreme(
                candles, findHigh: !isUptrend,
                fromIndex: peak.Index + 1, toIndex: toIndex);

            // Trough must not be at or after parent end
            if (trough.Index >= toIndex)
            {
                // Fall back to the previous significant retrace if available
                if (prevSignificant == null)
                    return null;

                ls = prevSignificant.Value;
                peak = PivotScanner.FindAbsoluteExtreme(
                    candles, findHigh: isUptrend,
                    fromIndex: fromIndex, toIndex: ls.peakIdx);
                trough = PivotScanner.FindAbsoluteExtreme(
                    candles, findHigh: !isUptrend,
                    fromIndex: peak.Index + 1, toIndex: toIndex);

                if (trough.Index >= toIndex)
                    return null;
            }

            return (peak, trough);
        }

        /// <summary>
        /// Find the deepest countertrend retrace within a range.
        /// Returns (peak, trough) where peak is the running extreme before the deepest retrace
        /// and trough is the countertrend extreme after the peak.
        /// Used for corrective A-B-C: the deepest retrace identifies the A→B boundary.
        /// </summary>
        private static (PivotPoint peak, PivotPoint trough)? FindDeepestRetrace(
            List<Ohlcv> candles, bool isUptrend, int fromIndex, int toIndex)
        {
            decimal runningExtreme = isUptrend ? candles[fromIndex].High : candles[fromIndex].Low;
            int runningExtremeIdx = fromIndex;

            decimal maxDepth = 0;
            int maxDepthPeakIdx = -1;

            for (int i = fromIndex + 1; i <= toIndex; i++)
            {
                decimal trendPrice = isUptrend ? candles[i].High : candles[i].Low;
                decimal retracePrice = isUptrend ? candles[i].Low : candles[i].High;

                // Track running extreme in trend direction
                if (isUptrend ? trendPrice > runningExtreme : trendPrice < runningExtreme)
                {
                    runningExtreme = trendPrice;
                    runningExtremeIdx = i;
                }

                // Check retrace depth (require at least 2 candle gap from peak)
                if (i > runningExtremeIdx + 1)
                {
                    var depth = Math.Abs(WaveMath.Log(runningExtreme) - WaveMath.Log(retracePrice));
                    if (depth > maxDepth)
                    {
                        maxDepth = depth;
                        maxDepthPeakIdx = runningExtremeIdx;
                    }
                }
            }

            if (maxDepthPeakIdx < 0) return null;

            // Peak = absolute trend extreme from start to deepest-retrace peak area
            var peak = PivotScanner.FindAbsoluteExtreme(
                candles, findHigh: isUptrend,
                fromIndex: fromIndex, toIndex: maxDepthPeakIdx);

            // Trough = absolute countertrend extreme from peak+1 to toIndex
            var trough = PivotScanner.FindAbsoluteExtreme(
                candles, findHigh: !isUptrend,
                fromIndex: peak.Index + 1, toIndex: toIndex);

            return (peak, trough);
        }

        /// <summary>
        /// Refine B endpoint for "double-top" patterns: scan backward from parent end to
        /// find the LAST candle where the countertrend price is within BEndpointProximity
        /// of the absolute B peak, AND there's a genuine valley between the two peaks
        /// (countertrend price drops below the proximity threshold between them).
        /// Returns the absolute countertrend extreme in a small window around that candle.
        /// If no qualifying later peak exists, returns the original waveB unchanged.
        /// (e.g., two near-identical countertrend peaks months apart with a deep valley between → picks the later one.)
        /// </summary>
        private static PivotPoint RefineToLastNearPeak(
            List<Ohlcv> candles, PivotPoint waveB, bool isUptrend, int endIdx)
        {
            // Minimum countertrend price to qualify as "near the absolute peak"
            decimal minCT = isUptrend
                ? waveB.Price * (decimal)Math.Exp((double)Thresholds.BEndpointProximity)   // B is LOW: threshold is upper bound
                : waveB.Price * (decimal)Math.Exp(-(double)Thresholds.BEndpointProximity); // B is HIGH: threshold is lower bound

            // Scan backward from parent end to find the LAST candle near the peak
            for (int i = endIdx - 1; i > waveB.Index; i--)
            {
                decimal ctPrice = isUptrend ? candles[i].Low : candles[i].High;
                bool nearPeak = isUptrend ? ctPrice <= minCT : ctPrice >= minCT;

                if (!nearPeak) continue;

                // Verify a genuine valley exists between the two peaks:
                // the countertrend price must retrace at least R618 (61.8%) from the
                // peak between them. This ensures deep structural separation — only
                // true "double-top/bottom" patterns qualify (e.g., peak, 60%+ valley,
                // then second near-identical peak).
                decimal valleyThreshold = isUptrend
                    ? waveB.Price * (decimal)Math.Exp((double)Fibonacci.R618)
                    : waveB.Price * (decimal)Math.Exp(-(double)Fibonacci.R618);
                bool hasValley = false;
                for (int j = waveB.Index + 1; j < i; j++)
                {
                    decimal ctJ = isUptrend ? candles[j].Low : candles[j].High;
                    if (isUptrend ? ctJ > valleyThreshold : ctJ < valleyThreshold)
                    {
                        hasValley = true;
                        break;
                    }
                }

                if (!hasValley) continue;

                // Found a genuine double-top — get precise extreme in vicinity
                int windowStart = Math.Max(waveB.Index + 1, i - Thresholds.DoublePeakSearchRadius);
                int windowEnd = Math.Min(endIdx - 1, i + Thresholds.DoublePeakSearchRadius);
                return PivotScanner.FindAbsoluteExtreme(
                    candles, findHigh: !isUptrend,
                    fromIndex: windowStart, toIndex: windowEnd);
            }

            return waveB; // No qualifying later peak found — keep absolute
        }

        /// <summary>
        /// Find the FIRST retrace where the countertrend move retraces ≥ minRetraceFraction
        /// of the move from start to the running extreme, AND the move itself exceeds minMoveLog.
        /// The minimum move requirement prevents triggering on small initial oscillations
        /// (e.g., first week after ATH: small oscillation has high retrace% but tiny absolute move).
        /// Returns (peak, trough) where peak = the running extreme at confirmation time,
        /// trough = the countertrend extreme that confirmed the retrace.
        /// </summary>
        private static (PivotPoint peak, PivotPoint trough)? FindFirstSignificantRetrace(
            List<Ohlcv> candles, bool isUptrend, int fromIndex, int toIndex,
            decimal minRetraceFraction, decimal minMoveLog = 0)
        {
            decimal runningExtreme = isUptrend ? candles[fromIndex].High : candles[fromIndex].Low;
            int runningExtremeIdx = fromIndex;
            var logStart = WaveMath.Log(isUptrend ? candles[fromIndex].Low : candles[fromIndex].High);

            for (int i = fromIndex + 1; i <= toIndex; i++)
            {
                decimal trendPrice = isUptrend ? candles[i].High : candles[i].Low;
                decimal retracePrice = isUptrend ? candles[i].Low : candles[i].High;

                // Track running extreme in trend direction
                if (isUptrend ? trendPrice > runningExtreme : trendPrice < runningExtreme)
                {
                    runningExtreme = trendPrice;
                    runningExtremeIdx = i;
                }

                // Check retrace (at least 2 candle gap from peak)
                if (i > runningExtremeIdx + 1)
                {
                    var logExtreme = WaveMath.Log(runningExtreme);
                    var move = Math.Abs(logExtreme - logStart);
                    var retrace = Math.Abs(logExtreme - WaveMath.Log(retracePrice));

                    // Move must be substantial AND retrace must be significant
                    if (move >= minMoveLog && retrace / move >= minRetraceFraction)
                    {
                        // First significant retrace found
                        var peak = PivotScanner.FindAbsoluteExtreme(
                            candles, findHigh: isUptrend,
                            fromIndex: fromIndex, toIndex: i);

                        var trough = PivotScanner.FindAbsoluteExtreme(
                            candles, findHigh: !isUptrend,
                            fromIndex: peak.Index + 1, toIndex: i);

                        return (peak, trough);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Find absolute countertrend extreme confirmed by subsequent recovery.
        /// When <paramref name="excludeEndFromSearch"/> is true (completed parents), the extreme
        /// search excludes the end candle. When false (in-progress parents), the full range is
        /// searched but the extreme must not be at the very end (no recovery possible).
        /// </summary>
        private static PivotPoint FindConfirmedExtreme(
            List<Ohlcv> candles, PivotPoint fromPoint, bool findHigh,
            int fromIndex, int toIndex,
            decimal recoveryThreshold = Fibonacci.R118,
            bool excludeEndFromSearch = false)
        {
            if (fromIndex >= toIndex - 1)
                return null;

            int searchEnd = excludeEndFromSearch ? toIndex - 1 : toIndex;
            var extreme = PivotScanner.FindAbsoluteExtreme(
                candles, findHigh, fromIndex: fromIndex, toIndex: searchEnd);

            // If extreme is at end of data → not confirmed (no subsequent candle for recovery)
            if (extreme.Index >= toIndex)
                return null;

            // Check for recovery after the extreme
            var logFrom = WaveMath.Log(fromPoint.Price);
            var logExtreme = WaveMath.Log(extreme.Price);
            var move = Math.Abs(logExtreme - logFrom);

            if (!PivotScanner.HasRecovery(candles, extreme.Index, toIndex,
                    logExtreme, move, extremeIsHigh: findHigh, recoveryThreshold))
                return null;

            return extreme;
        }

        /// <summary>
        /// Find the first confirmed swing beyond W1's price after W2.
        /// Used for W3 detection in in-progress parents.
        /// </summary>
        private static PivotPoint FindConfirmedSwingBeyond(
            List<Ohlcv> candles, PivotPoint w2, PivotPoint w1, bool isUptrend,
            int fromIndex, int toIndex)
        {
            decimal runningExtreme = w2.Price;
            int runningExtremeIdx = fromIndex;
            var logW2 = WaveMath.Log(w2.Price);

            for (int i = fromIndex + 1; i <= toIndex; i++)
            {
                decimal trendPrice = isUptrend ? candles[i].High : candles[i].Low;

                if (isUptrend ? trendPrice > runningExtreme : trendPrice < runningExtreme)
                {
                    runningExtreme = trendPrice;
                    runningExtremeIdx = i;
                }

                bool beyondW1 = isUptrend
                    ? runningExtreme > w1.Price
                    : runningExtreme < w1.Price;
                if (!beyondW1) continue;

                if (i > runningExtremeIdx)
                {
                    decimal retracePrice = isUptrend ? candles[i].Low : candles[i].High;
                    var logExtreme = WaveMath.Log(runningExtreme);
                    var move = Math.Abs(logExtreme - logW2);
                    var retrace = Math.Abs(logExtreme - WaveMath.Log(retracePrice));

                    if (move > 0 && retrace / move >= Fibonacci.R118)
                    {
                        return PivotScanner.FindAbsoluteExtreme(
                            candles, findHigh: isUptrend,
                            fromIndex: fromIndex, toIndex: i);
                    }
                }
            }

            return null;
        }

        // ══════════════════════════════════════════════════
        // Diagonal detection (§3.4, §3.5)
        // ══════════════════════════════════════════════════

        /// <summary>
        /// §3.4/§3.5: Attempt to build a diagonal when W4 overlaps W1 (R3 "violation").
        /// Ending diagonal (§3.4): parent in W5 or C position, converging amplitudes.
        /// Leading diagonal (§3.5): parent in W1 or A position, converging amplitudes.
        /// Returns null if position or convergence criteria are not met.
        /// </summary>
        private static List<ElliottWave> TryBuildDiagonal(
            List<PivotPoint> pivots, List<FibonacciJustification> justifications,
            ElliottWave parent, WaveDegree degree, bool isUptrend,
            bool w5InProgress = false, Projection inProgressProjection = null)
        {
            // Diagonals only occur in W1/A (leading) or W5/C (ending) positions
            var label = parent.Label;
            bool isEndingPosition = label is WaveNumber.Five or WaveNumber.C;
            bool isLeadingPosition = label is WaveNumber.One or WaveNumber.A;

            if (!isEndingPosition && !isLeadingPosition)
                return null;

            // Core convergence check: W3 amplitude < W1 amplitude (log scale)
            var w1Amp = WaveMath.LogDistance(pivots[1].Price, pivots[0].Price);
            var w3Amp = WaveMath.LogDistance(pivots[3].Price, pivots[2].Price);

            if (w3Amp >= w1Amp)
                return null;

            if (!w5InProgress && pivots.Count >= 6)
            {
                var w5Amp = WaveMath.LogDistance(pivots[5].Price, pivots[4].Price);

                // §3.4: Ending diagonals often end with a throw-over — W5 breaks through
                // the 1-3 trendline. When W5 is a genuine extension (exceeds W1 amplitude),
                // standard convergence (W5 < W3) and R2 don't apply to the throw-over wave.
                // W1-W3 convergence (w3Amp < w1Amp) is already confirmed above.
                bool w5ThrowOver = isEndingPosition && w5Amp > w1Amp;

                if (!w5ThrowOver)
                {
                    // Standard diagonal: full convergence W5 < W3
                    if (w5Amp >= w3Amp)
                        return null;

                }
            }

            var subType = isEndingPosition
                ? PatternSubType.EndingDiagonal
                : PatternSubType.LeadingDiagonal;

            if (w5InProgress)
            {
                return WaveLabeler.CreateImpulseWaves(pivots, justifications, degree,
                    subType: subType,
                    inProgressLabel: WaveNumber.Five,
                    inProgressProjection: inProgressProjection,
                    inProgressStart: pivots[^1]);
            }

            return WaveLabeler.CreateImpulseWaves(pivots, justifications, degree, subType: subType);
        }

        // ══════════════════════════════════════════════════
        // Corrective classification (§4.2)
        // ══════════════════════════════════════════════════

        /// <summary>
        /// §4.2: Classify corrective pattern as ZigZag or Flat based on B wave retracement.
        /// Flat = B retraces to or beyond A start (w0). Sub-types determined by C vs A end:
        ///   ExpandedFlat: C extends past A end (most common flat)
        ///   RegularFlat:  C ≈ A end (within BEndpointProximity tolerance)
        ///   RunningFlat:  C falls short of A end (rare, strong trends)
        /// When waveC is null (in-progress), returns Flat without sub-type.
        /// </summary>
        internal static (PatternType type, PatternSubType? subType) ClassifyCorrective(
            PivotPoint w0, PivotPoint waveA, PivotPoint waveB, bool waveAIsUpward,
            PivotPoint waveC = null)
        {
            // B exceeds A start in countertrend direction → Flat
            bool bExceedsAStart = waveAIsUpward
                ? waveB.Price <= w0.Price   // upward correction: B drops to/below start
                : waveB.Price >= w0.Price;  // downward correction: B rises to/above start

            if (!bExceedsAStart)
                return (PatternType.ZigZag, null);

            if (waveC == null)
                return (PatternType.Flat, null);

            // Determine flat sub-type by C vs A end
            bool cExceedsAEnd = WaveMath.ExceedsInDirection(waveC.Price, waveA.Price, waveAIsUpward);
            if (cExceedsAEnd)
                return (PatternType.Flat, PatternSubType.ExpandedFlat);

            // C ≈ A end (within 5% log-scale) → Regular; otherwise → Running
            var proximity = Math.Abs(WaveMath.Log(waveC.Price) - WaveMath.Log(waveA.Price));
            return proximity <= Thresholds.FlatRegularProximity
                ? (PatternType.Flat, PatternSubType.RegularFlat)
                : (PatternType.Flat, PatternSubType.RunningFlat);
        }

        // ══════════════════════════════════════════════════
        // Helpers
        // ══════════════════════════════════════════════════

        /// <summary>
        /// Given a confirmed W1 and optional W2, builds the in-progress impulse wave list.
        /// If W2 is confirmed, returns W1+W2 with W3 in-progress projection.
        /// If W2 is not confirmed, returns W1 with W2 in-progress projection.
        /// Consolidates duplicated logic from TryAthAsMotive and the proportionality check.
        /// </summary>
        private static List<ElliottWave> BuildInProgressFromW1(
            List<Ohlcv> candles, PivotPoint w0, PivotPoint w1, PivotPoint w2,
            WaveDegree degree, int endIdx, bool isUptrend)
        {
            var pivots = new List<PivotPoint> { w0, w1 };
            var justifications = new List<FibonacciJustification> { null };

            if (w2 != null && (isUptrend ? w2.Price > w0.Price : w2.Price < w0.Price))
            {
                pivots.Add(w2);
                justifications.Add(null);
                return BuildW3InProgress(pivots, justifications, degree, w0, w1, w2, candles[endIdx].Close);
            }

            var w2Proj = ProjectionBuilder.BuildRetracement(w0.Price, w1.Price,
                FibonacciProjector.W2RetracementLevels, w0.Price);
            return WaveLabeler.CreateImpulseWaves(pivots, justifications, degree,
                inProgressLabel: WaveNumber.Two,
                inProgressProjection: w2Proj,
                inProgressStart: w1);
        }

        /// <summary>
        /// Creates a W5 pivot from the parent endpoint and builds completed impulse waves
        /// with extension detection and truncation handling.
        /// Consolidates duplicated logic from TryBuildMotiveFromW3W4 and TryBuildMotiveFromListingSpike.
        /// </summary>
        private static List<ElliottWave> BuildCompletedImpulse(
            List<PivotPoint> pivots, WaveDegree degree, ElliottWave parent,
            int endIdx, bool isUptrend)
        {
            // W5 = parent end
            var w5 = new PivotPoint
            {
                Index = endIdx,
                Timestamp = parent.EndPoint.Timestamp,
                Price = parent.EndPoint.Price,
                PointType = parent.EndPoint.PointType
            };

            pivots.Add(w5);

            // R2: Wave 3 must not be the shortest motive wave (log scale).
            // W3 >= min(W1, W5) with 2% tolerance for borderline cases.
            var w1Len = WaveMath.LogDistance(pivots[1].Price, pivots[0].Price);
            var w3Len = WaveMath.LogDistance(pivots[3].Price, pivots[2].Price);
            var w5Len = WaveMath.LogDistance(pivots[5].Price, pivots[4].Price);
            if (w3Len < Math.Min(w1Len, w5Len) * Thresholds.W3LengthTolerance)
            {
                // §R2 relabeling: try "extended wave (3) in the making."
                // Merge W3+W4+W5 into a single extended W3 — the combined move from W2end to W5end.
                var mergedW3Len = WaveMath.LogDistance(pivots[5].Price, pivots[2].Price);
                if (mergedW3Len > w1Len)
                {
                    // Viable: the merged W3 exceeds W1, so return W1+W2 confirmed with W3 in-progress
                    var w0 = pivots[0];
                    var w1End = pivots[1];
                    var w2End = pivots[2];
                    var relabelPivots = new List<PivotPoint> { w0, w1End, w2End };
                    var relabelJustifications = new List<FibonacciJustification> { null, null };
                    pivots.RemoveAt(pivots.Count - 1); // Remove W5
                    return BuildW3InProgress(relabelPivots, relabelJustifications,
                        degree, w0, w1End, w2End, w5.Price);
                }

                pivots.RemoveAt(pivots.Count - 1); // Remove W5, let caller retry
                return null;
            }

            var justifications = new List<FibonacciJustification> { null, null, null, null, null };
            var subType = WaveMath.DetectExtension(pivots);

            // §3.3: Truncation — W5 fails to exceed W3 in trend direction
            var w3 = pivots[3];
            bool truncated = !WaveMath.ExceedsInDirection(w5.Price, w3.Price, isUptrend);
            if (truncated)
                subType = PatternSubType.Truncation;

            var waves = WaveLabeler.CreateImpulseWaves(pivots, justifications, degree,
                subType: subType);

            // Set orthodox endpoint on W5 so the orchestrator can propagate to the parent
            if (truncated && waves.Count >= 5)
                waves[4].OrthodoxEndPoint = w3;

            return waves;
        }

        /// <summary>
        /// Build a W3-in-progress wave list with extension projection.
        /// </summary>
        private static List<ElliottWave> BuildW3InProgress(
            List<PivotPoint> pivots, List<FibonacciJustification> justifications,
            WaveDegree degree, PivotPoint w0, PivotPoint w1, PivotPoint w2, decimal currentPrice)
        {
            bool isIntermediate = degree == WaveDegree.Intermediate;
            var w3Levels = isIntermediate
                ? FibonacciProjector.IntermediateW3ExtensionLevels
                : FibonacciProjector.W3ExtensionLevels;
            var w3Proj = ProjectionBuilder.BuildExtension(w0.Price, w1.Price, w2.Price,
                w3Levels, w0.Price, isW3: true, isIntermediate: isIntermediate,
                currentPrice: currentPrice);
            return WaveLabeler.CreateImpulseWaves(pivots, justifications, degree,
                inProgressLabel: WaveNumber.Three,
                inProgressProjection: w3Proj,
                inProgressStart: w2);
        }

        private static PivotPoint MakeW0(ElliottWave parent, int index)
        {
            return new PivotPoint
            {
                Index = index,
                Timestamp = parent.StartPoint.Timestamp,
                Price = parent.StartPoint.Price,
                PointType = parent.StartPoint.PointType
            };
        }

        private static int FindCandleIndex(List<Ohlcv> candles, DateTime timestamp) =>
            WaveMath.FindCandleIndex(candles, timestamp);
    }
}
