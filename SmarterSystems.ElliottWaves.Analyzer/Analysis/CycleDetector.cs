using SmarterSystems.ElliottWaves.Analyzer.Data;
using SmarterSystems.ElliottWaves.Analyzer.Interfaces;

namespace SmarterSystems.ElliottWaves.Analyzer.Analysis
{
    /// <summary>
    /// Cycle-degree wave detection using absolute extremes with confirmation.
    /// Algorithm: W0 → W1(first confirmed swing, escalated if needed) → W2 → W3 → W4.
    /// Handles R1 violation morphing (impulse → A-B-C).
    /// </summary>
    public static class CycleDetector
    {

        /// <summary>
        /// Detect cycle-degree waves on monthly data.
        /// Returns 2-5 waves depending on how many confirmed pivots are found.
        /// </summary>
        public static List<ElliottWave> DetectCycleWaves(List<Ohlcv> monthlyData)
        {
            if (monthlyData.Count < 2)
                return [];

            // Step 1: Determine W0 and trend direction
            var (w0, isUptrend) = FindW0(monthlyData);

            // Step 2: Find W1 — try first confirmed swing, escalate to ATH if needed
            var w1 = FindCycleDegreeW1(monthlyData, w0, isUptrend);
            if (w1 == null)
            {
                return [WaveLabeler.CreateInProgressWave(
                    w0, WaveNumber.One, WaveDegree.Cycle, null)];
            }

            // Step 3: Find W2 — absolute countertrend extreme after W1, confirmed by 10% recovery
            var w2 = FindConfirmedExtreme(monthlyData, w1, !isUptrend);

            if (w2 == null)
                return BuildW2InProgress(w0, w1);

            // Step 4: R1 check — W2 must not retrace beyond W0
            if (isUptrend ? w2.Price <= w0.Price : w2.Price >= w0.Price)
            {
                return BuildCorrectiveFromMorph(w0, w1, w2, monthlyData, isUptrend);
            }

            // Step 5: Find W3 — absolute trend extreme after W2
            var w3Candidate = PivotScanner.FindAbsoluteExtreme(
                monthlyData, findHigh: isUptrend, fromIndex: w2.Index + 1);

            // W3 must exceed W1 in trend direction
            bool w3ExceedsW1 = WaveMath.ExceedsInDirection(
                w3Candidate.Price, w1.Price, isUptrend);

            // W3 log-length from W2 must >= W1 log-length from W0
            var w1LogLen = WaveMath.LogDistance(w0.Price, w1.Price);
            var w3LogLen = WaveMath.LogDistance(w2.Price, w3Candidate.Price);
            bool w3LengthOk = w3LogLen >= w1LogLen;

            // W3 must be confirmed by retrace (>10%)
            bool w3Confirmed = false;
            if (w3ExceedsW1 && w3LengthOk)
            {
                w3Confirmed = HasSignificantReversal(
                    monthlyData, w3Candidate, w2, isUptrend);
            }

            if (!w3ExceedsW1 || !w3LengthOk || !w3Confirmed)
            {
                // W3 not fully confirmed. Determine: is W2 completed or still in-progress?
                // W2 is completed if EITHER:
                // (a) W3 exceeds W1 price (price beyond W1 proves W2 correction ended), OR
                // (b) The W1→W2 correction has completed A-B-C structure (≥0.75 log bounce + C beyond A)
                bool w2Completed = w3ExceedsW1 ||
                    HasCompletedABC(monthlyData, w1, w2, !isUptrend,
                        minBounceLog: Thresholds.W2CompletionMinBounceLog,
                        minCBeyondALog: Thresholds.W2CompletionMinCExtensionLog);

                if (w2Completed)
                {
                    // W2 confirmed, W3 in-progress → 3 waves
                    var pivots3 = new List<PivotPoint> { w0, w1, w2 };
                    var just3 = new List<FibonacciJustification> { null, null };
                    var w3Projection = ProjectionBuilder.BuildExtension(
                        w0.Price, w1.Price, w2.Price,
                        FibonacciProjector.W3ExtensionLevels, w0.Price, isW3: true,
                        currentPrice: monthlyData[^1].Close);
                    return WaveLabeler.CreateImpulseWaves(pivots3, just3,
                        WaveDegree.Cycle,
                        inProgressLabel: WaveNumber.Three,
                        inProgressProjection: w3Projection,
                        inProgressStart: w2);
                }
                else
                    return BuildW2InProgress(w0, w1);
            }

            var w3 = w3Candidate;

            // Step 6: Find W4 — countertrend extreme after W3, confirmed by recovery + completed A-B-C
            var w4 = FindConfirmedExtreme(monthlyData, w3, !isUptrend, requireCompletedCorrection: true);

            if (w4 == null)
            {
                // W4 not confirmed → 4 waves: I, II, III completed, IV in-progress
                var pivots4 = new List<PivotPoint> { w0, w1, w2, w3 };
                var just4 = new List<FibonacciJustification> { null, null, null };
                var w4Projection = ProjectionBuilder.BuildRetracement(
                    w2.Price, w3.Price, FibonacciProjector.W4RetracementLevels, w0.Price);
                return WaveLabeler.CreateImpulseWaves(pivots4, just4,
                    WaveDegree.Cycle,
                    inProgressLabel: WaveNumber.Four,
                    inProgressProjection: w4Projection,
                    inProgressStart: w3);
            }

            // R1 check for W4
            if (isUptrend ? w4.Price <= w0.Price : w4.Price >= w0.Price)
            {
                return BuildCorrectiveFromMorph(w0, w1, w2, monthlyData, isUptrend);
            }

            // Step 7: W5 always in-progress at cycle degree
            var pivots5 = new List<PivotPoint> { w0, w1, w2, w3, w4 };
            var just5 = new List<FibonacciJustification> { null, null, null, null };
            var w5Projection = ProjectionBuilder.BuildW5(w0, w1, w4, invalidation: w3.Price);
            return WaveLabeler.CreateImpulseWaves(pivots5, just5,
                WaveDegree.Cycle,
                inProgressLabel: WaveNumber.Five,
                inProgressProjection: w5Projection,
                inProgressStart: w4);
        }

        /// <summary>
        /// Determine W0 (the starting point) and trend direction.
        /// Normal case: absolute low or high, whichever comes first → uptrend or downtrend.
        /// Special cases:
        /// - If absHigh comes before absLow, find the minimum BEFORE absHigh as W0.
        /// - Skip outlier W0 prices below OutlierPriceThreshold (listing data artifacts).
        /// </summary>
        private static (PivotPoint w0, bool isUptrend) FindW0(List<Ohlcv> monthlyData)
        {
            var absLow = PivotScanner.FindAbsoluteExtreme(monthlyData, findHigh: false);
            var absHigh = PivotScanner.FindAbsoluteExtreme(monthlyData, findHigh: true);

            PivotPoint w0;
            bool isUptrend;

            if (absLow.Index <= absHigh.Index)
            {
                // Normal uptrend: absolute low comes first
                w0 = absLow;
                isUptrend = true;

                // Skip outlier W0 (listing artifact): if W0 price is extremely low
                // and there's a much higher low nearby, use that instead
                if (w0.Price < Thresholds.OutlierPriceThreshold && w0.Index < monthlyData.Count - 2)
                {
                    var searchEnd = absHigh.Index > w0.Index + 1 ? absHigh.Index : monthlyData.Count - 1;
                    var nextLow = PivotScanner.FindAbsoluteExtreme(
                        monthlyData, findHigh: false,
                        fromIndex: w0.Index + 1, toIndex: searchEnd);
                    if (nextLow.Price > w0.Price * Thresholds.OutlierMultiplier) // Next low is >10× outlier → artifact
                        w0 = nextLow;
                }
            }
            else
            {
                // absHigh comes before absLow (high-first pattern)
                // Find the minimum BEFORE absHigh as W0 (not the absolute minimum which comes after)
                if (absHigh.Index > 0)
                {
                    w0 = PivotScanner.FindAbsoluteExtreme(
                        monthlyData, findHigh: false,
                        fromIndex: 0, toIndex: absHigh.Index);
                    isUptrend = true;
                }
                else
                {
                    // Near-unreachable: absolute high is the very first monthly candle.
                    // No data before it to find a low, so treat the high itself as W0
                    // with a downtrend assumption. Kept for correctness on edge-case data.
                    w0 = absHigh;
                    isUptrend = false;
                }
            }

            return (w0, isUptrend);
        }

        /// <summary>
        /// Find the cycle-degree W1 using escalation strategy:
        /// 1. Try the first confirmed swing (>50% retrace) — this is the "natural" W1
        /// 2. Build forward to check if W3 would be confirmed
        /// 3. If W3 IS confirmed but the ATH hasn't been exceeded since W3
        ///    (meaning the structure is still in the W1 run), escalate W1 to the ATH
        ///    so that the sub-waves live at primary degree inside Cycle I
        /// 4. If W3 is confirmed AND a new high exceeds W3 end → keep the small W1
        ///    (e.g., new high exceeds W3 end → keep the small W1)
        /// </summary>
        private static PivotPoint FindCycleDegreeW1(
            List<Ohlcv> candles, PivotPoint w0, bool isUptrend)
        {
            // Find first confirmed swing (>50% retrace)
            var firstResult = PivotScanner.FindWave1(candles, w0, isUptrend);
            if (firstResult == null)
            {
                // No >50% retrace found. This happens when the trend from W0 is a
                // long gradual move with no deep correction (e.g., only 49% max retrace).
                // Use the absolute extreme as W1 — the entire move is one big cycle wave.
                var ath = PivotScanner.FindAbsoluteExtreme(candles, findHigh: isUptrend,
                    fromIndex: w0.Index + 1);
                return ath.Index < candles.Count - 1 ? ath : null;
            }

            var firstW1 = firstResult.Value.W1;

            // Try to build forward from this W1 to see if we get a valid W3
            var testW2 = FindConfirmedExtreme(candles, firstW1, !isUptrend);
            if (testW2 == null)
                return firstW1; // Can't even find W2 → keep first W1

            // R1 check: if testW2 violates W0, the structure will morph to corrective A-B-C.
            // Escalate W1 to the ATH before the R1-violating correction so CycleA
            // captures the full impulse before the R1-violating correction.
            if (isUptrend ? testW2.Price <= w0.Price : testW2.Price >= w0.Price)
            {
                var ath = PivotScanner.FindAbsoluteExtreme(candles, findHigh: isUptrend,
                    fromIndex: w0.Index + 1, toIndex: testW2.Index);
                return WaveMath.ExceedsInDirection(ath.Price, firstW1.Price, isUptrend)
                    ? ath : firstW1;
            }

            // Find W3 candidate — use first peak near absolute to handle double-tops
            var absW3 = PivotScanner.FindAbsoluteExtreme(
                candles, findHigh: isUptrend, fromIndex: testW2.Index + 1);
            var testW3 = FindFirstPeakNearAbsolute(candles, testW2, absW3, isUptrend);
            bool w3Exceeds = WaveMath.ExceedsInDirection(testW3.Price, firstW1.Price, isUptrend);
            var w1Len = WaveMath.LogDistance(w0.Price, firstW1.Price);
            var w3Len = WaveMath.LogDistance(testW2.Price, testW3.Price);
            bool w3LenOk = w3Len >= w1Len * Thresholds.W3LengthTolerance;
            bool w3Confirmed = w3Exceeds && w3LenOk &&
                HasSignificantReversal(candles, testW3, testW2, isUptrend);

            if (!w3Confirmed)
            {
                // W3 not fully confirmed. If W3 still exceeds W1 and has significant reversal
                // but fails the length check, probe whether the multi-wave structure is viable.
                // Only escalate when testW3 is meaningfully beyond firstW1 (>11.8% of total move
                // in log-scale) AND W4 would violate R3. This prevents escalation when firstW1
                // already captures most of the move (e.g., only 2% extension beyond firstW1).
                if (w3Exceeds && !w3LenOk &&
                    HasSignificantReversal(candles, testW3, testW2, isUptrend))
                {
                    var totalMove = WaveMath.LogDistance(w0.Price, testW3.Price);
                    var extension = WaveMath.LogDistance(firstW1.Price, testW3.Price);
                    bool significantExtension = totalMove > 0 &&
                        extension / totalMove >= Fibonacci.R118;

                    if (significantExtension)
                    {
                        var probeW4 = FindConfirmedExtreme(candles, testW3, !isUptrend);
                        if (probeW4 != null)
                        {
                            bool r3Violated = isUptrend
                                ? probeW4.Price <= firstW1.Price
                                : probeW4.Price >= firstW1.Price;
                            if (r3Violated)
                                return testW3;
                        }
                        else
                        {
                            return testW3; // W4 not confirmed → escalate
                        }
                    }
                }
                return firstW1;
            }

            // W3 is confirmed. Decide: keep firstW1 (multi-wave cycle) or escalate to testW3 (single Cycle I)?
            // Use geometric mean criterion: if W4 sits above sqrt(firstW1 × testW3),
            // the correction is shallow relative to the full span → keep the multi-wave structure.
            // Otherwise the "W1→W2→W3" is really one big Cycle I with primary sub-waves inside.
            var w4Candidate = FindConfirmedExtreme(candles, testW3, !isUptrend);
            if (w4Candidate != null)
            {
                // R3 check: W4 must not overlap W1 territory
                bool r3Violated = isUptrend
                    ? w4Candidate.Price <= firstW1.Price
                    : w4Candidate.Price >= firstW1.Price;
                if (r3Violated)
                    return testW3; // R3 violated → escalate

                // Geometric mean test: W4 above sqrt(W1 × W3) → shallow correction → keep firstW1
                var geoMean = (decimal)Math.Exp(
                    ((double)WaveMath.Log(firstW1.Price) + (double)WaveMath.Log(testW3.Price)) / 2.0);
                bool w4AboveGeoMean = isUptrend
                    ? w4Candidate.Price > geoMean
                    : w4Candidate.Price < geoMean;
                if (w4AboveGeoMean)
                    return firstW1; // Shallow W4 → valid multi-wave structure
            }

            // W4 not confirmed OR W4 below geometric mean → escalate to ATH as single Cycle I
            return testW3;
        }

        /// <summary>
        /// Find the absolute countertrend extreme after a pivot point,
        /// confirmed by ≥10% recovery (log-scale) from the move.
        /// When requireCompletedCorrection is true, also requires a completed A-B-C structure
        /// (significant bounce followed by deeper extreme) to confirm cycle-degree pivots.
        /// Returns null if the extreme is at end of data or recovery is insufficient.
        /// </summary>
        private static PivotPoint FindConfirmedExtreme(
            List<Ohlcv> candles, PivotPoint fromPoint, bool findHigh,
            decimal recoveryThreshold = Thresholds.DefaultRecoveryThreshold,
            bool requireCompletedCorrection = false)
        {
            if (fromPoint.Index >= candles.Count - 2)
                return null;

            // Find absolute extreme after fromPoint
            var extreme = PivotScanner.FindAbsoluteExtreme(
                candles, findHigh, fromIndex: fromPoint.Index + 1);

            // If extreme is at end of data → not confirmed
            if (extreme.Index >= candles.Count - 1)
                return null;

            // Check for recovery: any candle after the extreme must show
            // ≥threshold reversal relative to the fromPoint→extreme move
            var logFrom = WaveMath.Log(fromPoint.Price);
            var logExtreme = WaveMath.Log(extreme.Price);
            var move = Math.Abs(logExtreme - logFrom);

            if (!PivotScanner.HasRecovery(candles, extreme.Index, candles.Count - 1,
                    logExtreme, move, extremeIsHigh: findHigh, recoveryThreshold))
                return null;

            // Structural confirmation: require completed A-B-C correction
            if (requireCompletedCorrection && !HasCompletedABC(candles, fromPoint, extreme, findHigh))
                return null;

            return extreme;
        }

        /// <summary>
        /// Check if the correction from fromPoint to extreme shows a completed A-B-C structure
        /// on the monthly data. Scans forward from fromPoint, tracking running extreme (A candidate).
        /// Requires a significant bounce (≥minBounceLog in log-scale) from the running extreme (B wave),
        /// followed by a deeper extreme (C beyond A). This distinguishes completed corrections
        /// (assets with clear A-B-C) from ongoing corrections (single leg without bounce).
        /// </summary>
        private static bool HasCompletedABC(
            List<Ohlcv> candles, PivotPoint fromPoint, PivotPoint extreme, bool findHigh,
            decimal minBounceLog = Thresholds.DefaultMinBounceLog, decimal minCBeyondALog = 0m)
        {
            decimal runningExtreme = fromPoint.Price;
            int runningExtremeIdx = fromPoint.Index;

            for (int i = fromPoint.Index + 1; i <= extreme.Index; i++)
            {
                // Track the running extreme in correction direction
                decimal trendPrice = findHigh ? candles[i].High : candles[i].Low;
                decimal bouncePrice = findHigh ? candles[i].Low : candles[i].High;

                bool newExtreme = findHigh
                    ? trendPrice > runningExtreme
                    : trendPrice < runningExtreme;

                if (newExtreme)
                {
                    runningExtreme = trendPrice;
                    runningExtremeIdx = i;
                }

                // After finding an A-end candidate, look for a significant bounce (B wave)
                if (runningExtremeIdx > fromPoint.Index && i > runningExtremeIdx)
                {
                    var bounceSize = Math.Abs(WaveMath.Log(bouncePrice) - WaveMath.Log(runningExtreme));
                    if (bounceSize >= minBounceLog)
                    {
                        // B wave found. Now check: does C go significantly beyond A?
                        var cExtension = Math.Abs(WaveMath.Log(extreme.Price) - WaveMath.Log(runningExtreme));
                        if (cExtension >= minCBeyondALog)
                            return true;
                        // Bounce qualifies but C doesn't extend enough — keep scanning
                        // for a later A-B-C pattern with deeper A
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Find the first confirmed peak within logTolerance of the absolute extreme.
        /// For assets with double-tops (two near-identical peaks separated in time),
        /// returns the earlier peak so that escalation logic uses the structurally correct W3.
        /// Falls back to the absolute extreme if no earlier peak qualifies.
        /// </summary>
        private static PivotPoint FindFirstPeakNearAbsolute(
            List<Ohlcv> candles, PivotPoint searchFrom, PivotPoint absolute, bool isUptrend,
            decimal logTolerance = Thresholds.PeakProximityTolerance)
        {
            var logAbsolute = WaveMath.Log(absolute.Price);
            for (int i = searchFrom.Index + 1; i < absolute.Index; i++)
            {
                decimal price = isUptrend ? candles[i].High : candles[i].Low;
                var logPrice = WaveMath.Log(price);
                if (Math.Abs(logPrice - logAbsolute) <= logTolerance)
                {
                    var candidate = new PivotPoint
                    {
                        Index = i,
                        Timestamp = candles[i].TimestampUtc,
                        Price = price,
                        PointType = isUptrend ? PointType.High : PointType.Low
                    };
                    // Confirm with significant reversal after this peak
                    if (HasSignificantReversal(candles, candidate, searchFrom, isUptrend))
                        return candidate;
                }
            }
            return absolute;
        }

        /// <summary>
        /// Check if a confirmed extreme has a significant reversal after it,
        /// measured as ≥threshold of the priorPoint→extreme move.
        /// Used for W3 confirmation (must retrace after the extreme).
        /// </summary>
        private static bool HasSignificantReversal(
            List<Ohlcv> candles, PivotPoint extreme, PivotPoint priorPoint,
            bool isUptrend, decimal threshold = Thresholds.DefaultRecoveryThreshold)
        {
            if (extreme.Index >= candles.Count - 1)
                return false;

            var logExtreme = WaveMath.Log(extreme.Price);
            var logPrior = WaveMath.Log(priorPoint.Price);
            var move = Math.Abs(logExtreme - logPrior);

            // For trend-direction extremes (e.g., W3 in an uptrend), the extreme is a high
            // and recovery means price dropping — so extremeIsHigh matches isUptrend.
            return PivotScanner.HasRecovery(candles, extreme.Index, candles.Count - 1,
                logExtreme, move, extremeIsHigh: isUptrend, threshold);
        }

        /// <summary>
        /// Build a W1-completed / W2-in-progress wave list with retracement projection.
        /// </summary>
        private static List<ElliottWave> BuildW2InProgress(PivotPoint w0, PivotPoint w1)
        {
            var w2Projection = ProjectionBuilder.BuildRetracement(
                w0.Price, w1.Price, FibonacciProjector.W2RetracementLevels, w0.Price);
            var pivots = new List<PivotPoint> { w0, w1 };
            var justifications = new List<FibonacciJustification> { null };
            return WaveLabeler.CreateImpulseWaves(pivots, justifications,
                WaveDegree.Cycle,
                inProgressLabel: WaveNumber.Two,
                inProgressProjection: w2Projection,
                inProgressStart: w1);
        }

        /// <summary>
        /// R1 violation: W2 violates W0 → morph to corrective A-B-C.
        /// A = W0→W1, B = W1→W2 (completed), C = W2→in-progress.
        /// </summary>
        private static List<ElliottWave> BuildCorrectiveFromMorph(
            PivotPoint w0, PivotPoint w1, PivotPoint w2,
            List<Ohlcv> monthlyData, bool isUptrend)
        {
            // A = W0→W1 (completed), B = W1→W2 (completed)
            var pivots = new List<PivotPoint> { w0, w1, w2 };
            var justifications = new List<FibonacciJustification> { null, null };

            // §4.2: Classify based on B (w2) vs A start (w0)
            var (patternType, _) = SubWaveDetector.ClassifyCorrective(w0, w1, w2, isUptrend);

            // C in-progress with A=C projection
            var cProjection = ProjectionBuilder.BuildExtension(
                w0.Price, w1.Price, w2.Price,
                FibonacciProjector.CExtensionLevels, w0.Price,
                currentPrice: monthlyData[^1].Close);

            return WaveLabeler.CreateCorrectiveWaves(pivots, justifications,
                WaveDegree.Cycle, patternType,
                inProgressLabel: WaveNumber.C,
                inProgressProjection: cProjection,
                inProgressStart: w2);
        }

        // Projection building delegated to shared ProjectionBuilder

    }
}
