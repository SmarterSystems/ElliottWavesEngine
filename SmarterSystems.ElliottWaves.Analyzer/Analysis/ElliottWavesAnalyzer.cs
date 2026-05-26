using SmarterSystems.ElliottWaves.Analyzer.Data;
using SmarterSystems.ElliottWaves.Analyzer.Interfaces;

namespace SmarterSystems.ElliottWaves.Analyzer.Analysis
{
    /// <summary>
    /// Orchestrator for multi-degree Elliott Wave analysis.
    /// Delegates to CycleDetector (monthly), SubWaveDetector (daily).
    /// Public API preserved: Analyze(List&lt;Ohlcv&gt;) → ElliottWavesAnalysis.
    /// </summary>
    public class ElliottWavesAnalyzer
    {
        private enum CorrectiveType { Simple, Triangle, DoubleCombination, TripleCombination }


        public ElliottWavesAnalysis Analyze(List<Ohlcv> hourlyData)
        {
            // 0. Sanitize outlier prices before aggregation so daily/monthly data is clean
            SanitizeOutlierPrices(hourlyData);

            // 1. Aggregate candles: hourly → daily → monthly
            var dailyData = AggregateCandles(hourlyData, x => x.TimestampUtc.Date, k => k);
            var monthlyData = AggregateCandles(dailyData,
                x => new { x.TimestampUtc.Year, x.TimestampUtc.Month },
                k => new DateTime(k.Year, k.Month, 1));

            // 2. Detect cycle-degree waves on monthly data
            var cycleWaves = CycleDetector.DetectCycleWaves(monthlyData);

            if (cycleWaves.Count == 0)
                return new ElliottWavesAnalysis { Waves = [] };

            // 3. Resolve cycle timestamps: monthly → hourly resolution
            ResolveCycleTimestamps(cycleWaves, hourlyData);

            // 4. Detect primary sub-waves on daily data, then intermediate on hourly
            foreach (var cycleWave in cycleWaves)
            {
                cycleWave.SubWaves = SubWaveDetector.DetectSubWaves(
                    dailyData, cycleWave, WaveDegree.Primary);

                // Resolve sub-wave timestamps: daily → hourly
                if (cycleWave.SubWaves != null && cycleWave.SubWaves.Count > 0)
                {
                    ResolveSubWaveTimestamps(cycleWave.SubWaves, cycleWave, hourlyData);

                    // Detect intermediate sub-waves on hourly data within each primary wave
                    foreach (var primaryWave in cycleWave.SubWaves)
                    {
                        primaryWave.SubWaves = SubWaveDetector.DetectSubWaves(
                            hourlyData, primaryWave, WaveDegree.Intermediate);
                    }
                }
            }

            // 4a. Validate diagonal internal structure (§3.4/§3.5)
            ValidateDiagonalStructure(cycleWaves);

            // 4b. Validate corrective internal structure (§4.1/§4.2)
            ValidateCorrectiveStructure(cycleWaves);

            // 4c. §3.3: Verify truncated W5 has internal motive structure
            ValidateTruncations(cycleWaves);

            // 4d. §5.8: Populate orthodox endpoints for irregular/running patterns
            PopulateOrthodoxEndpoints(cycleWaves);

            // 5. §5.3: Post-extension behavior — set retrace level on corrections after extended impulses
            PopulatePostExtensionRetraceLevel(cycleWaves);

            // 6. R5 relabeling: motive waves with corrective sub-waves → corrective
            // Applied at Cycle degree only. Primary-level R5 is not applied because
            // SubWaveDetector's corrective fallback at Intermediate degree indicates
            // "couldn't confirm 5-wave motive" — not "confirmed corrective structure."
            ApplyR5Relabeling(cycleWaves);

            // 6a. §8.3: Collect alternate counts (corrective fallback for motive waves)
            CollectAlternateCounts(cycleWaves, dailyData, monthlyData);

            // 7. Frost & Prechter guideline satisfaction scoring
            PopulateGuidelineScores(cycleWaves);

            // 7a. Rank preferred vs alternate counts by guideline score
            RankCountsByGuidelineScore(cycleWaves);

            // 8. Align Cycle retracement projections to the Primary sub-wave target.
            //    The Cycle wave's endpoint equals the terminal sub-wave's endpoint,
            //    so the Cycle projection should target the nearest fib retracement
            //    to the Primary C (or W5) projection's best target.
            AlignCycleProjectionToSubWaves(cycleWaves);

            return new ElliottWavesAnalysis { Waves = cycleWaves };
        }

        /// <summary>
        /// Resolve monthly-detected cycle points to exact hourly timestamps.
        /// </summary>
        private static void ResolveCycleTimestamps(List<ElliottWave> waves, List<Ohlcv> hourlyData)
        {
            foreach (var wave in waves)
            {
                ResolvePointToHourly(wave.StartPoint, hourlyData);

                if (!wave.IsInProgress)
                    ResolvePointToHourly(wave.EndPoint, hourlyData);
            }

            EnsureBoundaryConsistency(waves);
        }

        /// <summary>
        /// Resolve a pivot point to the exact hourly candle within a time window.
        /// Monthly resolution (searchDays=0): searches within the point's calendar month.
        /// Daily resolution (searchDays>0): searches within the next searchDays days from the point's date.
        /// </summary>
        private static void ResolvePointToHourly(PivotPoint point, List<Ohlcv> hourlyData, int searchDays = 0)
        {
            DateTime rangeStart, rangeEnd;
            if (searchDays > 0)
            {
                rangeStart = point.Timestamp.Date;
                rangeEnd = rangeStart.AddDays(searchDays);
            }
            else
            {
                rangeStart = new DateTime(point.Timestamp.Year, point.Timestamp.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                rangeEnd = rangeStart.AddMonths(1);
            }

            bool findHigh = point.PointType == PointType.High;
            var resolved = PivotScanner.FindAbsoluteExtremeBetween(
                hourlyData, findHigh, rangeStart, rangeEnd);

            if (resolved != null)
            {
                point.Timestamp = resolved.Timestamp;
                point.Price = resolved.Price;
                point.Index = resolved.Index;
            }
        }

        /// <summary>
        /// Resolve daily-detected sub-wave points to exact hourly timestamps.
        /// Ensures boundary consistency with parent wave.
        /// </summary>
        private static void ResolveSubWaveTimestamps(
            List<ElliottWave> subWaves, ElliottWave parent, List<Ohlcv> hourlyData)
        {
            // Resolve each point from daily → hourly (1-day window)
            foreach (var wave in subWaves)
            {
                ResolvePointToHourly(wave.StartPoint, hourlyData, searchDays: 1);
                if (!wave.IsInProgress)
                    ResolvePointToHourly(wave.EndPoint, hourlyData, searchDays: 1);
            }

            // Force boundary consistency:
            // First sub-wave starts at parent start
            subWaves[0].StartPoint.Timestamp = parent.StartPoint.Timestamp;
            subWaves[0].StartPoint.Price = parent.StartPoint.Price;

            EnsureBoundaryConsistency(subWaves);

            // Last completed sub-wave ends at parent end (if parent is completed)
            if (!parent.IsInProgress)
            {
                var last = subWaves[^1];
                if (!last.IsInProgress)
                {
                    last.EndPoint.Timestamp = parent.EndPoint.Timestamp;
                    last.EndPoint.Price = parent.EndPoint.Price;
                }
            }
        }

        /// <summary>
        /// Enforce temporal consistency: each wave's start point matches the previous wave's end point.
        /// </summary>
        private static void EnsureBoundaryConsistency(List<ElliottWave> waves)
        {
            for (int i = 1; i < waves.Count; i++)
            {
                waves[i].StartPoint.Timestamp = waves[i - 1].EndPoint.Timestamp;
                waves[i].StartPoint.Price = waves[i - 1].EndPoint.Price;
            }
        }

        /// <summary>
        /// R5 two-pass relabeling: if the first cycle wave has a motive label
        /// but its sub-waves are all corrective (A-B-C), the entire structure
        /// is corrective. Classify the corrective pattern type and apply
        /// appropriate labels (ABC, ABCDE, WXY, or WXYXZ).
        /// </summary>
        private static void ApplyR5Relabeling(List<ElliottWave> cycleWaves)
        {
            if (cycleWaves.Count == 0) return;

            var firstWave = cycleWaves[0];

            // Only applies when cycle waves are motive-labeled
            if (!IsMotiveNumber(firstWave.Label)) return;

            // Check if first wave's sub-waves are all corrective labels
            if (firstWave.SubWaves == null || firstWave.SubWaves.Count == 0) return;
            if (!firstWave.SubWaves.All(sw => IsCorrectiveNumber(sw.Label))) return;

            // Classify the corrective pattern and build the label map
            var type = ClassifyCorrectivePattern(cycleWaves);
            var labelMap = BuildLabelMap(type);

            // Determine PatternType and SubType for the corrective structure
            PatternType? corrPatternType = type switch
            {
                CorrectiveType.Simple => PatternType.ZigZag,
                CorrectiveType.Triangle => PatternType.Triangle,
                CorrectiveType.DoubleCombination or CorrectiveType.TripleCombination => PatternType.Complex,
                _ => PatternType.ZigZag
            };
            PatternSubType? corrSubType = type switch
            {
                CorrectiveType.Triangle => ClassifyTriangleSubType(cycleWaves),
                CorrectiveType.DoubleCombination => ClassifyComplexSubType(cycleWaves, isDouble: true),
                CorrectiveType.TripleCombination => ClassifyComplexSubType(cycleWaves, isDouble: false),
                _ => null
            };

            // R5 triggered: relabel cycle waves and set pattern classification
            foreach (var wave in cycleWaves)
            {
                if (labelMap.TryGetValue(wave.Label, out var newLabel))
                    wave.Label = newLabel;
                wave.PatternType = corrPatternType;
                wave.PatternSubType = corrSubType;
            }

            // Relabel last cycle wave's sub-waves from motive to corrective
            var lastWave = cycleWaves[^1];
            if (lastWave.SubWaves != null)
            {
                foreach (var sw in lastWave.SubWaves)
                {
                    if (labelMap.TryGetValue(sw.Label, out var newSubLabel))
                        sw.Label = newSubLabel;
                }
            }
        }

        /// <summary>
        /// Classify the corrective pattern type based on wave count and geometry.
        /// </summary>
        private static CorrectiveType ClassifyCorrectivePattern(List<ElliottWave> cycleWaves)
        {
            int count = cycleWaves.Count;

            if (count <= 3)
            {
                if (count == 3 && IsDoubleCombination(cycleWaves))
                    return CorrectiveType.DoubleCombination;
                return CorrectiveType.Simple;
            }

            if (count == 5)
            {
                if (IsTriangle(cycleWaves))
                    return CorrectiveType.Triangle;
                return CorrectiveType.TripleCombination;
            }

            // 4+ waves: in-progress — check overlap to distinguish triangle vs complex
            if (HasOverlappingWaves(cycleWaves))
                return CorrectiveType.Triangle;

            return CorrectiveType.TripleCombination;
        }

        /// <summary>
        /// Build motive→corrective label map for the classified pattern type.
        /// </summary>
        private static Dictionary<WaveNumber, WaveNumber> BuildLabelMap(CorrectiveType type) => type switch
        {
            CorrectiveType.Simple => new Dictionary<WaveNumber, WaveNumber>
            {
                { WaveNumber.One, WaveNumber.A },
                { WaveNumber.Two, WaveNumber.B },
                { WaveNumber.Three, WaveNumber.C }
            },
            CorrectiveType.Triangle => new Dictionary<WaveNumber, WaveNumber>
            {
                { WaveNumber.One, WaveNumber.A },
                { WaveNumber.Two, WaveNumber.B },
                { WaveNumber.Three, WaveNumber.C },
                { WaveNumber.Four, WaveNumber.D },
                { WaveNumber.Five, WaveNumber.E }
            },
            CorrectiveType.DoubleCombination => new Dictionary<WaveNumber, WaveNumber>
            {
                { WaveNumber.One, WaveNumber.W },
                { WaveNumber.Two, WaveNumber.X1 },
                { WaveNumber.Three, WaveNumber.Y }
            },
            CorrectiveType.TripleCombination => new Dictionary<WaveNumber, WaveNumber>
            {
                { WaveNumber.One, WaveNumber.W },
                { WaveNumber.Two, WaveNumber.X1 },
                { WaveNumber.Three, WaveNumber.Y },
                { WaveNumber.Four, WaveNumber.X2 },
                { WaveNumber.Five, WaveNumber.Z }
            },
            _ => new Dictionary<WaveNumber, WaveNumber>
            {
                { WaveNumber.One, WaveNumber.A },
                { WaveNumber.Two, WaveNumber.B },
                { WaveNumber.Three, WaveNumber.C }
            }
        };

        /// <summary>
        /// Detect double combination (W-X-Y): all three waves have corrective sub-waves
        /// and the structure is sideways — Y does not exceed W significantly in the trend direction.
        /// </summary>
        private static bool IsDoubleCombination(List<ElliottWave> waves)
        {
            if (waves.Count < 3) return false;

            // All waves must have corrective internal structure
            foreach (var w in waves)
            {
                if (w.SubWaves == null || w.SubWaves.Count == 0) return false;
                if (!w.SubWaves.All(sw => IsCorrectiveNumber(sw.Label))) return false;
            }

            // Sideways check: Y's end doesn't exceed W's end significantly in trend direction.
            // In a double combo, the net progress is limited compared to W's amplitude.
            var w0 = waves[0]; // W
            var y = waves[2];  // Y

            decimal wAmplitude = WaveMath.LogDistance(w0.StartPoint.Price, w0.EndPoint.Price);
            if (wAmplitude == 0) return false;

            // Net progress in log scale: how far Y's end is from W's start in the trend direction
            bool downtrend = w0.EndPoint.Price < w0.StartPoint.Price;
            var logWStart = WaveMath.Log(w0.StartPoint.Price);
            var logYEnd = WaveMath.Log(y.EndPoint.Price);
            decimal netProgress = downtrend
                ? logWStart - logYEnd
                : logYEnd - logWStart;

            // In a double combo, net progress is typically < 1.5× the amplitude of W
            // (vs. a simple ABC where C often exceeds A significantly)
            return netProgress < wAmplitude * Thresholds.DoubleComboMaxProgress;
        }

        /// <summary>
        /// Detect triangle (A-B-C-D-E): waves overlap and amplitudes converge or diverge.
        /// </summary>
        private static bool IsTriangle(List<ElliottWave> waves)
        {
            if (waves.Count < 5) return false;
            return HasOverlappingWaves(waves) && HasConvergingAmplitudes(waves);
        }

        /// <summary>
        /// Check if wave C's price range overlaps wave A's price range.
        /// This is the defining characteristic of triangles and combinations.
        /// </summary>
        private static bool HasOverlappingWaves(List<ElliottWave> waves)
        {
            if (waves.Count < 3) return false;

            var a = waves[0];
            var c = waves[2];

            decimal aLow = Math.Min(a.StartPoint.Price, a.EndPoint.Price);
            decimal aHigh = Math.Max(a.StartPoint.Price, a.EndPoint.Price);
            decimal cLow = Math.Min(c.StartPoint.Price, c.EndPoint.Price);
            decimal cHigh = Math.Max(c.StartPoint.Price, c.EndPoint.Price);

            // Overlap: the ranges intersect
            return Math.Max(aLow, cLow) < Math.Min(aHigh, cHigh);
        }

        /// <summary>
        /// Classify triangle sub-type from wave geometry.
        /// Uses trendline slopes through alternating wave endpoints.
        /// </summary>
        private static PatternSubType ClassifyTriangleSubType(List<ElliottWave> waves)
        {
            if (waves.Count < 4)
                return PatternSubType.ContractingSymmetrical;

            // Running triangle: B exceeds A start in countertrend direction
            var aStart = waves[0].StartPoint.Price;
            var bEnd = waves[1].EndPoint.Price;
            bool aDown = waves[0].EndPoint.Price < waves[0].StartPoint.Price;
            if (aDown ? bEnd > aStart : bEnd < aStart)
                return PatternSubType.RunningTriangle;

            // Expanding: majority of successive amplitudes increase
            int expandCount = 0;
            for (int i = 1; i < waves.Count; i++)
            {
                if (WaveAmplitude(waves[i]) > WaveAmplitude(waves[i - 1]))
                    expandCount++;
            }
            if (expandCount >= (waves.Count - 1) * Thresholds.ExpandingMajorityRatio)
                return PatternSubType.ExpandingTriangle;

            // Contracting — classify by trendline geometry
            // Odd endpoints (A, C, E): trend-direction extremes
            // Even endpoints (B, D): countertrend extremes
            var aEnd = waves[0].EndPoint.Price;
            var cEnd = waves[2].EndPoint.Price;
            var dEnd = waves[3].EndPoint.Price;

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

        /// <summary>
        /// Check if wave amplitudes are converging (each wave smaller than its predecessor).
        /// Characteristic of contracting triangles.
        /// </summary>
        private static bool HasConvergingAmplitudes(List<ElliottWave> waves)
        {
            int convergingCount = 0;
            for (int i = 1; i < waves.Count; i++)
            {
                if (WaveAmplitude(waves[i]) < WaveAmplitude(waves[i - 1]))
                    convergingCount++;
            }
            // At least 60% of waves should be converging for a triangle
            return convergingCount >= (waves.Count - 1) * Thresholds.ConvergenceMajorityRatio;
        }

        /// <summary>
        /// Log-scale amplitude of a wave. Uses log scale to match the rest of the codebase,
        /// ensuring consistent geometry evaluation for crypto's exponential price ranges.
        /// </summary>
        private static decimal WaveAmplitude(ElliottWave w)
            => WaveMath.LogDistance(w.StartPoint.Price, w.EndPoint.Price);

        /// <summary>
        /// §4.3/§4.4: Classify complex corrective sub-type.
        /// Double zigzag (W-X-Y): both W and Y have zigzag internal structure.
        /// Triple zigzag (W-X-Y-X-Z): W, Y, and Z all have zigzag internal structure.
        /// Returns null if the actionable waves are not all zigzags (generic combination).
        /// </summary>
        private static PatternSubType? ClassifyComplexSubType(List<ElliottWave> waves, bool isDouble)
        {
            // Actionable waves (W, Y, Z) are at even indices (0, 2, 4)
            // X waves (connecting waves) are at odd indices (1, 3)
            var actionableIndices = isDouble
                ? new[] { 0, 2 }        // W, Y
                : new[] { 0, 2, 4 };    // W, Y, Z

            bool allZigzag = true;
            foreach (int idx in actionableIndices)
            {
                if (idx >= waves.Count) { allZigzag = false; break; }
                var w = waves[idx];
                // A wave is a zigzag if its sub-waves form A-B-C with A exceeding the start
                // and the overall pattern moves sharply in one direction.
                // Check if sub-waves exist and the pattern type is ZigZag, or if the
                // sub-wave labels are corrective (A, B, C) — indicating zigzag structure.
                if (w.PatternType == PatternType.ZigZag) continue;
                if (w.SubWaves != null && w.SubWaves.Count == 3
                    && w.SubWaves[0].Label == WaveNumber.A
                    && w.SubWaves[1].Label == WaveNumber.B
                    && w.SubWaves[2].Label == WaveNumber.C) continue;
                allZigzag = false;
                break;
            }

            if (allZigzag)
                return isDouble ? PatternSubType.DoubleZigzag : PatternSubType.TripleZigzag;

            // §4.4 Complex corrective rules validation:

            // Rule 1: "Triangle only as final component" — if a triangle appears,
            // it must be at the final actionable position (Y for double, Z for triple)
            int finalActionableIdx = isDouble ? 2 : 4;
            foreach (int idx in actionableIndices)
            {
                if (idx >= waves.Count) continue;
                if (waves[idx].PatternType == PatternType.Triangle && idx != finalActionableIdx)
                    return null; // Triangle in non-final position invalidates combination
            }

            // Rule 2: "Never more than one triangle per combination"
            int triangleCount = 0;
            foreach (int idx in actionableIndices)
            {
                if (idx >= waves.Count) continue;
                if (waves[idx].PatternType == PatternType.Triangle)
                    triangleCount++;
            }
            if (triangleCount > 1)
                return null;

            return null; // Generic combination — no specific sub-type
        }

        /// <summary>
        /// §5.3: When a completed motive wave contains an extended sub-wave, the following
        /// correction typically retraces to the W2 territory of that extended sub-wave.
        /// Sets PostExtensionRetraceLevel on the corrective wave that follows.
        /// </summary>
        private static void PopulatePostExtensionRetraceLevel(List<ElliottWave> waves)
        {
            PopulatePostExtensionForWaveList(waves);
            foreach (var wave in waves)
            {
                if (wave.SubWaves != null && wave.SubWaves.Count > 0)
                    PopulatePostExtensionRetraceLevel(wave.SubWaves);
            }
        }

        private static void PopulatePostExtensionForWaveList(List<ElliottWave> waves)
        {
            for (int i = 0; i < waves.Count - 1; i++)
            {
                var motive = waves[i];
                var correction = waves[i + 1];

                if (motive.IsInProgress || motive.SubWaves == null || motive.SubWaves.Count < 5)
                    continue;

                // Check for extension in the motive wave's sub-waves
                var subType = motive.PatternSubType;
                if (subType is not (PatternSubType.Extended1 or PatternSubType.Extended3 or PatternSubType.Extended5))
                    continue;

                // §5.3: Drill into the specific extended sub-wave to find its W2.
                // Extended1 → W1 (index 0), Extended3 → W3 (index 2), Extended5 → W5 (index 4)
                var extendedIdx = subType switch
                {
                    PatternSubType.Extended1 => 0,
                    PatternSubType.Extended3 => 2,
                    PatternSubType.Extended5 => 4,
                    _ => -1
                };

                decimal? retracePrice = null;
                if (extendedIdx >= 0 && extendedIdx < motive.SubWaves.Count)
                {
                    var extWave = motive.SubWaves[extendedIdx];
                    if (extWave.SubWaves != null && extWave.SubWaves.Count >= 2
                        && !extWave.SubWaves[1].IsInProgress)
                    {
                        // W2 of the extended sub-wave
                        retracePrice = extWave.SubWaves[1].EndPoint.Price;
                    }
                }

                // Fallback: parent W2 when extended sub-wave has no decomposition
                retracePrice ??= motive.SubWaves.Count >= 2 && !motive.SubWaves[1].IsInProgress
                    ? motive.SubWaves[1].EndPoint.Price
                    : null;

                if (retracePrice != null)
                    correction.PostExtensionRetraceLevel = retracePrice.Value;
            }
        }

        private static bool IsMotiveNumber(WaveNumber label) => label is
            WaveNumber.One or WaveNumber.Two or WaveNumber.Three or WaveNumber.Four or WaveNumber.Five;

        private static bool IsCorrectiveNumber(WaveNumber label) => label is
            WaveNumber.A or WaveNumber.B or WaveNumber.C or
            WaveNumber.D or WaveNumber.E or
            WaveNumber.W or WaveNumber.X1 or WaveNumber.Y or WaveNumber.X2 or WaveNumber.Z;

        /// <summary>
        /// Align the Cycle wave's retracement projection to match the Primary sub-wave target.
        /// The terminal sub-wave (C or 5) ends at the same price as the parent Cycle wave,
        /// so the Cycle projection arrow should point to the nearest fib retracement level
        /// that matches the sub-wave's best target (A=C for C-waves, or the W5 projection).
        /// This ensures Cycle and Primary arrows are consistent and avoids contradictions.
        /// </summary>
        private static void AlignCycleProjectionToSubWaves(List<ElliottWave> cycleWaves)
        {
            foreach (var cycleWave in cycleWaves)
            {
                if (!cycleWave.IsInProgress || cycleWave.Projection == null)
                    continue;
                if (cycleWave.SubWaves == null || cycleWave.SubWaves.Count == 0)
                    continue;

                // Find the in-progress terminal sub-wave (C or 5) with a projection
                var terminalSubWave = cycleWave.SubWaves
                    .LastOrDefault(sw => sw.IsInProgress && sw.Projection != null);
                if (terminalSubWave == null)
                    continue;

                // Get the sub-wave's best target (highest probability = A=C for C-waves)
                var subBestTarget = terminalSubWave.Projection.Targets
                    .OrderByDescending(t => t.Probability)
                    .FirstOrDefault();
                if (subBestTarget == null)
                    continue;

                // Find the nearest Cycle retracement level to this sub-wave target price
                decimal subTargetLog = WaveMath.Log(subBestTarget.Price);

                Target nearestCycleTarget = null;
                decimal nearestDistance = decimal.MaxValue;
                foreach (var target in cycleWave.Projection.Targets)
                {
                    decimal dist = Math.Abs(WaveMath.Log(target.Price) - subTargetLog);
                    if (dist < nearestDistance)
                    {
                        nearestDistance = dist;
                        nearestCycleTarget = target;
                    }
                }

                if (nearestCycleTarget == null)
                    continue;

                // Redistribute probabilities: nearest level gets highest, others proportional
                if (cycleWave.Projection.Targets.Count == 1)
                {
                    nearestCycleTarget.Probability = 1.0m;
                }
                else
                {
                    int others = cycleWave.Projection.Targets.Count - 1;
                    foreach (var target in cycleWave.Projection.Targets)
                    {
                        target.Probability = target == nearestCycleTarget
                            ? Thresholds.NearestTargetProbability
                            : Thresholds.RemainingTargetProbability / others;
                    }
                }
            }
        }

        // ══════════════════════════════════════════════════
        // Step 4a: Diagonal internal structure validation (§3.4/§3.5)
        // ══════════════════════════════════════════════════

        /// <summary>
        /// Validate diagonal internal structure after all sub-wave degrees are detected.
        /// Ending diagonals (§3.4) require 3-3-3-3-3 internal structure.
        /// Leading diagonals (§3.5) require 5-3-5-3-5 internal structure.
        /// On failure: demote to plain Impulse (nullify PatternSubType).
        /// </summary>
        private static void ValidateDiagonalStructure(List<ElliottWave> waves)
        {
            foreach (var wave in waves)
            {
                if (wave.SubWaves != null)
                {
                    foreach (var sub in wave.SubWaves)
                    {
                        ValidateDiagonalSingle(sub);
                        if (sub.SubWaves != null)
                        {
                            foreach (var intm in sub.SubWaves)
                                ValidateDiagonalSingle(intm);
                        }
                    }
                }
            }
        }

        private static void ValidateDiagonalSingle(ElliottWave wave)
        {
            if (wave.PatternSubType is not (PatternSubType.EndingDiagonal or PatternSubType.LeadingDiagonal))
                return;
            if (wave.SubWaves == null || wave.SubWaves.Count < 3)
            {
                // Diagonal without sufficient sub-waves cannot be validated — demote
                if (wave.PatternSubType != null)
                    DemoteDiagonal(wave);
                return;
            }

            bool isEnding = wave.PatternSubType == PatternSubType.EndingDiagonal;

            foreach (var sub in wave.SubWaves)
            {
                if (sub.IsInProgress)
                    continue; // Can't validate in-progress waves

                bool isActionary = sub.Label is WaveNumber.One or WaveNumber.Three or WaveNumber.Five;

                if (sub.SubWaves == null || sub.SubWaves.Count == 0)
                {
                    // Completed sub-wave without internal structure — can't confirm diagonal
                    if (!sub.IsInProgress)
                    {
                        DemoteDiagonal(wave);
                        return;
                    }
                    continue;
                }

                bool hasMotiveStructure = sub.SubWaves.Count == 5
                    && sub.SubWaves.Any(sw => sw.Label == WaveNumber.One);
                bool hasCorrectiveStructure = sub.SubWaves.Count <= 3
                    && sub.SubWaves.Any(sw => sw.Label == WaveNumber.A);

                if (isEnding)
                {
                    // All sub-waves should be corrective (3-wave)
                    if (isActionary && hasMotiveStructure && !hasCorrectiveStructure)
                    {
                        DemoteDiagonal(wave);
                        return;
                    }
                }
                else // Leading diagonal
                {
                    if (isActionary)
                    {
                        // Actionary waves (1,3,5) should be motive (5-wave)
                        if (hasCorrectiveStructure && !hasMotiveStructure)
                        {
                            DemoteDiagonal(wave);
                            return;
                        }
                    }
                    else
                    {
                        // Reactionary waves (2,4) should be corrective (3-wave)
                        if (hasMotiveStructure && !hasCorrectiveStructure)
                        {
                            DemoteDiagonal(wave);
                            return;
                        }
                    }
                }
            }
        }

        private static void DemoteDiagonal(ElliottWave wave)
        {
            wave.PatternSubType = null;
            wave.PatternType = PatternType.Impulse;
            foreach (var sub in wave.SubWaves ?? [])
            {
                sub.PatternSubType = null;
                sub.PatternType = PatternType.Impulse;
            }
        }

        // ══════════════════════════════════════════════════
        // Step 4b: Corrective internal structure validation (§4.1/§4.2)
        // ══════════════════════════════════════════════════

        /// <summary>
        /// Validate that corrective sub-waves match their expected internal structure.
        /// ZigZag: A=5-wave (motive), B=3-wave (corrective), C=5-wave (motive)
        /// Flat: A=3-wave (corrective), B=3-wave (corrective), C=5-wave (motive)
        /// On mismatch: set BWaveImpulseWarning if B has motive structure.
        /// This is informational — we don't demote the pattern since detection may
        /// not have resolved sub-structure at all degrees.
        /// </summary>
        private static void ValidateCorrectiveStructure(List<ElliottWave> waves)
        {
            foreach (var wave in waves)
            {
                ValidateCorrectiveSingle(wave);
                if (wave.SubWaves != null)
                    ValidateCorrectiveStructure(wave.SubWaves);
            }
        }

        private static void ValidateCorrectiveSingle(ElliottWave wave)
        {
            if (wave.IsInProgress) return;
            if (wave.SubWaves == null || wave.SubWaves.Count < 2) return;
            if (wave.PatternType is not (PatternType.ZigZag or PatternType.Flat)) return;

            bool isZigZag = wave.PatternType == PatternType.ZigZag;

            foreach (var sub in wave.SubWaves)
            {
                if (sub.IsInProgress || sub.SubWaves == null || sub.SubWaves.Count == 0)
                    continue;

                bool hasMotiveInternal = sub.SubWaves.Count >= 5
                    && sub.SubWaves.Any(sw => sw.Label == WaveNumber.One);
                bool hasCorrectiveInternal = sub.SubWaves.Any(sw => sw.Label == WaveNumber.A);

                if (sub.Label == WaveNumber.A)
                {
                    if (isZigZag)
                    {
                        // ZigZag A should be motive (5-wave)
                        // If corrective, the pattern might actually be a flat
                        if (hasCorrectiveInternal && !hasMotiveInternal)
                            wave.PatternType = PatternType.Flat;
                    }
                    // Flat A should be corrective (3-wave) — already the default
                }
                else if (sub.Label == WaveNumber.B)
                {
                    // B should always be corrective (3-wave)
                    // If it has motive structure, warn — might be misidentified
                    if (hasMotiveInternal && !hasCorrectiveInternal)
                        wave.BWaveImpulseWarning = true;
                }
                else if (sub.Label == WaveNumber.C)
                {
                    // C should always be motive (5-wave) for both zigzag and flat
                    // No reclassification — C being corrective is unusual but possible
                }
            }
        }

        // ══════════════════════════════════════════════════
        // Step 4c: Truncation W5 internal verification (§3.3)
        // ══════════════════════════════════════════════════

        /// <summary>
        /// §3.3: Walk the wave tree and verify that truncated W5 waves contain internal motive
        /// structure. Sets TruncationUnverified=true on impulse parents where W5 lacks motive sub-waves.
        /// </summary>
        private static void ValidateTruncations(List<ElliottWave> waves)
        {
            foreach (var wave in waves)
            {
                if (wave.PatternSubType == PatternSubType.Truncation
                    && wave.SubWaves != null && wave.SubWaves.Count >= 5)
                {
                    var w5 = wave.SubWaves[4]; // W5 is the truncated wave
                    bool verified = w5.SubWaves != null
                        && w5.SubWaves.Count >= 3
                        && w5.SubWaves.Any(sw => WaveLabeler.IsMotiveLabel(sw.Label));

                    if (!verified)
                        wave.TruncationUnverified = true;
                }

                if (wave.SubWaves != null)
                    ValidateTruncations(wave.SubWaves);
            }
        }

        // ══════════════════════════════════════════════════
        // Step 4d: Orthodox endpoint propagation (§5.8)
        // ══════════════════════════════════════════════════

        /// <summary>
        /// §5.8: Set orthodox endpoints for irregular/running patterns where the
        /// actual extreme differs from the "orthodox" wave endpoint.
        /// Truncation is already handled in SubWaveDetector.BuildCompletedImpulse.
        /// </summary>
        private static void PopulateOrthodoxEndpoints(List<ElliottWave> waves)
        {
            foreach (var wave in waves)
            {
                PopulateOrthodoxSingle(wave);
                if (wave.SubWaves != null)
                {
                    PopulateOrthodoxEndpoints(wave.SubWaves);
                }
            }
        }

        private static void PopulateOrthodoxSingle(ElliottWave wave)
        {
            if (wave.SubWaves == null || wave.SubWaves.Count < 2)
                return;

            // For expanded/running flats and running triangles, B exceeds the A start.
            // The orthodox endpoint is the B-wave extreme.
            if (wave.PatternSubType is PatternSubType.ExpandedFlat
                or PatternSubType.RunningFlat
                or PatternSubType.RunningTriangle)
            {
                wave.OrthodoxEndPoint = wave.SubWaves[1].EndPoint;
            }
        }

        // ══════════════════════════════════════════════════
        // Step 6a: Alternate count collection (§8.3)
        // ══════════════════════════════════════════════════

        /// <summary>
        /// §8.3: Collect alternate counts at both Cycle and Primary degrees.
        /// - Cycle: corrective + alternate motive positions on monthly data
        /// - Primary: corrective + alternate motive positions on daily data
        /// - Corrective patterns: zigzag↔flat alternate interpretations
        /// </summary>
        private static void CollectAlternateCounts(
            List<ElliottWave> cycleWaves, List<Ohlcv> dailyData, List<Ohlcv> monthlyData)
        {
            // ── Cycle-degree alternates ──
            foreach (var cycleWave in cycleWaves)
            {
                if (WaveLabeler.IsMotiveLabel(cycleWave.Label)
                    && cycleWave.SubWaves != null && cycleWave.SubWaves.Count >= 3
                    && !cycleWave.SubWaves.Any(sw => sw.Label == WaveNumber.A))
                {
                    cycleWave.AlternateCounts ??= [];

                    var corrAlt = SubWaveDetector.TryAlternateCorrective(
                        monthlyData, cycleWave, WaveDegree.Cycle);
                    if (corrAlt != null && corrAlt.Count > 0)
                        cycleWave.AlternateCounts.Add(corrAlt);

                    if (!cycleWave.IsInProgress)
                    {
                        var motiveAlts = SubWaveDetector.TryAlternateMotivePositions(
                            monthlyData, cycleWave, WaveDegree.Cycle);
                        foreach (var alt in motiveAlts)
                            cycleWave.AlternateCounts.Add(alt);
                    }

                    if (cycleWave.AlternateCounts.Count == 0)
                        cycleWave.AlternateCounts = null;
                }
            }

            // ── Primary-degree alternates ──
            foreach (var cycleWave in cycleWaves)
            {
                if (cycleWave.SubWaves == null) continue;

                foreach (var primaryWave in cycleWave.SubWaves)
                {
                    // Motive → corrective alternates + alternate motive positions
                    if (WaveLabeler.IsMotiveLabel(primaryWave.Label)
                        && primaryWave.SubWaves != null && primaryWave.SubWaves.Count >= 3
                        && !primaryWave.SubWaves.Any(sw => sw.Label == WaveNumber.A))
                    {
                        primaryWave.AlternateCounts ??= [];

                        var corrAlt = SubWaveDetector.TryAlternateCorrective(
                            dailyData, primaryWave, WaveDegree.Primary);
                        if (corrAlt != null && corrAlt.Count > 0)
                            primaryWave.AlternateCounts.Add(corrAlt);

                        if (!primaryWave.IsInProgress)
                        {
                            var motiveAlts = SubWaveDetector.TryAlternateMotivePositions(
                                dailyData, primaryWave, WaveDegree.Primary);
                            foreach (var alt in motiveAlts)
                                primaryWave.AlternateCounts.Add(alt);
                        }

                        if (primaryWave.AlternateCounts.Count == 0)
                            primaryWave.AlternateCounts = null;
                    }

                    // Corrective pattern alternates: zigzag↔flat
                    if (!WaveLabeler.IsMotiveLabel(primaryWave.Label)
                        && primaryWave.SubWaves != null && primaryWave.SubWaves.Count >= 2
                        && !primaryWave.IsInProgress)
                    {
                        CollectCorrectivePatternAlternates(primaryWave, dailyData);
                    }
                }
            }
        }

        /// <summary>
        /// For corrective-labeled waves, try the opposite pattern interpretation as an alternate.
        /// ZigZag → try Flat, Flat → try ZigZag, Triangle → try simple corrective.
        /// </summary>
        private static void CollectCorrectivePatternAlternates(
            ElliottWave wave, List<Ohlcv> dailyData)
        {
            if (wave.SubWaves == null || wave.SubWaves.Count < 2) return;

            // Re-run corrective detection — if it produces a different pattern type, store as alternate
            var alt = SubWaveDetector.TryAlternateCorrective(
                dailyData, wave, WaveDegree.Primary);

            if (alt == null || alt.Count == 0) return;

            // Only store if the alternate has a different pattern type than the preferred
            var altPatternType = alt[0].PatternType;
            var prefPatternType = wave.SubWaves[0].PatternType;
            if (altPatternType != prefPatternType)
            {
                wave.AlternateCounts ??= [];
                wave.AlternateCounts.Add(alt);
            }
        }

        // ══════════════════════════════════════════════════
        // Step 7: Guideline satisfaction scoring (§5-§7)
        // ══════════════════════════════════════════════════

        /// <summary>
        /// Recursively walk the wave tree and populate GuidelineSatisfaction scores.
        /// </summary>
        private static void PopulateGuidelineScores(List<ElliottWave> waves)
        {
            // Collect all projection targets for clustering analysis
            var allTargets = new List<(ElliottWave Wave, Target Target)>();
            CollectProjectionTargets(waves, allTargets);

            PopulateGuidelineScoresRecursive(waves, allTargets);
        }

        private static void PopulateGuidelineScoresRecursive(
            List<ElliottWave> waves, List<(ElliottWave Wave, Target Target)> allTargets)
        {
            foreach (var wave in waves)
            {
                var guidelines = new GuidelineSatisfaction();
                bool hasAnyScore = false;

                // Impulse-specific guidelines
                if (IsCompletedImpulse(wave))
                {
                    guidelines.WaveEquality = ScoreWaveEquality(wave);
                    guidelines.Alternation = ScoreAlternation(wave);
                    guidelines.WavePersonality = ScoreWavePersonality(wave);
                    guidelines.GoldenSectionDivision = ScoreGoldenSectionDivision(wave);
                    guidelines.DepthOfCorrection = ScoreDepthOfCorrection(wave);
                    hasAnyScore = true;
                }

                // Corrective-specific guidelines
                if (IsCompletedCorrective(wave))
                {
                    guidelines.BWaveRetracement = ScoreBWaveRetracement(wave);
                    guidelines.WavePersonality = ScoreCorrectivePersonality(wave);
                    hasAnyScore = true;
                }

                // Triangle-specific guidelines
                if (wave.PatternType == PatternType.Triangle
                    && wave.SubWaves != null && wave.SubWaves.Count >= 5)
                {
                    guidelines.TriangleRatios = ScoreTriangleRatios(wave);
                    hasAnyScore = true;
                }

                // Time relationships (any wave with sub-waves)
                if (wave.SubWaves != null && wave.SubWaves.Count >= 2 && !wave.IsInProgress)
                {
                    guidelines.TimeRelationships = ScoreTimeRelationships(wave);
                    hasAnyScore = true;
                }

                // Projection clustering (in-progress waves with projections)
                if (wave.IsInProgress && wave.Projection?.Targets != null)
                {
                    guidelines.ProjectionClustering = ScoreProjectionClustering(wave, allTargets);
                    hasAnyScore = true;
                }

                if (hasAnyScore)
                    wave.Guidelines = guidelines;

                // Recurse into sub-waves
                if (wave.SubWaves != null)
                    PopulateGuidelineScoresRecursive(wave.SubWaves, allTargets);
            }
        }

        /// <summary>
        /// §8.3: Rank preferred vs alternate counts by composite guideline score.
        /// If an alternate scores higher than the preferred count, swap them.
        /// Also score alternates so they can be ranked.
        /// </summary>
        private static void RankCountsByGuidelineScore(List<ElliottWave> waves)
        {
            foreach (var wave in waves)
            {
                if (wave.AlternateCounts != null && wave.AlternateCounts.Count > 0
                    && wave.SubWaves != null && wave.SubWaves.Count > 0)
                {
                    // Score each alternate's sub-waves
                    var allTargets = new List<(ElliottWave Wave, Target Target)>();
                    foreach (var alt in wave.AlternateCounts)
                    {
                        var altTargets = new List<(ElliottWave, Target)>();
                        CollectProjectionTargets(alt, altTargets);
                        PopulateGuidelineScoresRecursive(alt, altTargets);
                    }

                    // Get preferred count's composite score
                    decimal preferredScore = GetAverageComposite(wave.SubWaves);

                    // Find best alternate
                    int bestAltIdx = -1;
                    decimal bestAltScore = preferredScore;
                    for (int i = 0; i < wave.AlternateCounts.Count; i++)
                    {
                        decimal altScore = GetAverageComposite(wave.AlternateCounts[i]);
                        if (altScore > bestAltScore)
                        {
                            bestAltScore = altScore;
                            bestAltIdx = i;
                        }
                    }

                    // Swap if an alternate scores higher
                    if (bestAltIdx >= 0)
                    {
                        var oldPreferred = wave.SubWaves;
                        wave.SubWaves = wave.AlternateCounts[bestAltIdx];
                        wave.AlternateCounts[bestAltIdx] = oldPreferred;
                    }

                    // Sort remaining alternates by descending score
                    wave.AlternateCounts.Sort((a, b) =>
                        GetAverageComposite(b).CompareTo(GetAverageComposite(a)));
                }

                // Recurse
                if (wave.SubWaves != null)
                    RankCountsByGuidelineScore(wave.SubWaves);
            }
        }

        /// <summary>
        /// Get average composite guideline score across all waves in a list.
        /// </summary>
        private static decimal GetAverageComposite(List<ElliottWave> waves)
        {
            var scores = waves
                .Where(w => w.Guidelines?.CompositeScore != null)
                .Select(w => w.Guidelines.CompositeScore.Value)
                .ToArray();
            return scores.Length > 0 ? scores.Average() : 0;
        }

        private static bool IsCompletedImpulse(ElliottWave wave) =>
            !wave.IsInProgress
            && wave.PatternType is PatternType.Impulse or PatternType.Diagonal
            && wave.SubWaves != null && wave.SubWaves.Count >= 5
            && wave.SubWaves[0].Label == WaveNumber.One;

        private static bool IsCompletedCorrective(ElliottWave wave) =>
            !wave.IsInProgress
            && wave.PatternType is PatternType.ZigZag or PatternType.Flat
            && wave.SubWaves != null && wave.SubWaves.Count >= 2
            && wave.SubWaves[0].Label == WaveNumber.A;

        // ── §5.4: Wave Equality ──

        /// <summary>
        /// Two non-extended motive waves tend toward equality in magnitude.
        /// Score = ratio of smaller to larger (1.0 = perfectly equal).
        /// </summary>
        private static decimal? ScoreWaveEquality(ElliottWave wave)
        {
            if (wave.SubWaves.Count < 5) return null;

            var w1Len = WaveMath.LogDistance(wave.SubWaves[0].StartPoint.Price, wave.SubWaves[0].EndPoint.Price);
            var w3Len = WaveMath.LogDistance(wave.SubWaves[2].StartPoint.Price, wave.SubWaves[2].EndPoint.Price);
            var w5Len = WaveMath.LogDistance(wave.SubWaves[4].StartPoint.Price, wave.SubWaves[4].EndPoint.Price);

            decimal a, b;
            switch (wave.PatternSubType)
            {
                case PatternSubType.Extended1: a = w3Len; b = w5Len; break;
                case PatternSubType.Extended3: a = w1Len; b = w5Len; break;
                case PatternSubType.Extended5: a = w1Len; b = w3Len; break;
                default:
                    // No extension — find the two most equal waves
                    var lens = new[] { w1Len, w3Len, w5Len };
                    Array.Sort(lens);
                    // Best equality is between the two closest values
                    var r01 = lens[1] > 0 ? lens[0] / lens[1] : 0;
                    var r12 = lens[2] > 0 ? lens[1] / lens[2] : 0;
                    return Math.Max(r01, r12);
            }

            if (a == 0 || b == 0) return 0;
            return Math.Min(a, b) / Math.Max(a, b);
        }

        // ── §5.1: Alternation ──

        /// <summary>
        /// W2 and W4 should alternate in pattern type, depth, duration, and complexity.
        /// Scores four dimensions of alternation:
        /// 1. Pattern type: sharp (zigzag) vs sideways (flat/triangle)
        /// 2. Depth: different retrace depths (one deep, one shallow)
        /// 3. Duration: different time spans (one short, one prolonged)
        /// 4. Complexity: different sub-wave count (simple vs complex)
        /// </summary>
        private static decimal? ScoreAlternation(ElliottWave wave)
        {
            if (wave.SubWaves.Count < 4) return null;

            var w2 = wave.SubWaves[1]; // W2
            var w4 = wave.SubWaves[3]; // W4

            decimal score = 0;
            int checks = 0;

            // 1. Pattern type alternation
            if (w2.PatternType != null && w4.PatternType != null)
            {
                bool w2Sharp = w2.PatternType == PatternType.ZigZag;
                bool w4Sharp = w4.PatternType == PatternType.ZigZag;
                score += w2Sharp != w4Sharp ? 1.0m : 0.0m;
                checks++;
            }

            // 2. Depth alternation: compare retrace depths relative to preceding motive wave
            var w1Len = WaveMath.LogDistance(wave.SubWaves[0].StartPoint.Price, wave.SubWaves[0].EndPoint.Price);
            var w3Len = wave.SubWaves.Count >= 3
                ? WaveMath.LogDistance(wave.SubWaves[2].StartPoint.Price, wave.SubWaves[2].EndPoint.Price) : 0;
            var w2Depth = WaveMath.LogDistance(w2.StartPoint.Price, w2.EndPoint.Price);
            var w4Depth = WaveMath.LogDistance(w4.StartPoint.Price, w4.EndPoint.Price);

            if (w1Len > 0 && w3Len > 0)
            {
                var w2Pct = w2Depth / w1Len;
                var w4Pct = w4Depth / w3Len;
                // Alternation: one deep (>50%), one shallow (<50%), or significant difference
                var depthDiff = Math.Abs(w2Pct - w4Pct);
                score += Math.Min(1.0m, depthDiff / Fibonacci.R382);
                checks++;
            }

            // 3. Duration alternation: different time spans
            if (!w2.IsInProgress && !w4.IsInProgress)
            {
                var w2Hours = (w2.EndPoint.Timestamp - w2.StartPoint.Timestamp).TotalHours;
                var w4Hours = (w4.EndPoint.Timestamp - w4.StartPoint.Timestamp).TotalHours;
                if (w2Hours > 0 && w4Hours > 0)
                {
                    var ratio = Math.Min(w2Hours, w4Hours) / Math.Max(w2Hours, w4Hours);
                    // Perfect alternation: large duration difference (ratio near 0)
                    // Equal duration: ratio near 1.0 (poor alternation)
                    score += 1.0m - (decimal)ratio;
                    checks++;
                }
            }

            // 4. Complexity alternation: different sub-wave counts
            int w2Complexity = w2.SubWaves?.Count ?? 0;
            int w4Complexity = w4.SubWaves?.Count ?? 0;
            if (w2Complexity > 0 && w4Complexity > 0)
            {
                score += w2Complexity != w4Complexity ? 1.0m : 0.0m;
                checks++;
            }

            return checks > 0 ? Math.Round(score / checks, 4) : null;
        }

        // ── §6: Wave Personality ──

        /// <summary>
        /// Score wave behavior against expected personality traits:
        /// W3 strongest, W2 deep retrace, W4 shallower than W2, W5 less dynamic than W3.
        /// </summary>
        private static decimal? ScoreWavePersonality(ElliottWave wave)
        {
            if (wave.SubWaves.Count < 5) return null;

            var w1Len = WaveMath.LogDistance(wave.SubWaves[0].StartPoint.Price, wave.SubWaves[0].EndPoint.Price);
            var w2Ret = WaveMath.LogDistance(wave.SubWaves[1].StartPoint.Price, wave.SubWaves[1].EndPoint.Price);
            var w3Len = WaveMath.LogDistance(wave.SubWaves[2].StartPoint.Price, wave.SubWaves[2].EndPoint.Price);
            var w4Ret = WaveMath.LogDistance(wave.SubWaves[3].StartPoint.Price, wave.SubWaves[3].EndPoint.Price);
            var w5Len = WaveMath.LogDistance(wave.SubWaves[4].StartPoint.Price, wave.SubWaves[4].EndPoint.Price);

            decimal score = 0;
            int checks = 0;

            // W3 is strongest
            if (w3Len >= w1Len && w3Len >= w5Len) score += 1.0m;
            checks++;

            // W2 deep retrace (ideally 61.8%+ of W1)
            if (w1Len > 0)
            {
                var w2Pct = w2Ret / w1Len;
                score += Math.Min(1.0m, w2Pct / Fibonacci.R618);
                checks++;
            }

            // W4 shallower than W2
            if (w4Ret < w2Ret) score += 1.0m;
            checks++;

            // W5 less dynamic than W3
            if (w5Len < w3Len) score += 1.0m;
            checks++;

            return checks > 0 ? Math.Round(score / checks, 4) : null;
        }

        // ── §6: Corrective Wave Personality ──

        /// <summary>
        /// Score corrective wave behavior against expected personality traits (§6):
        /// A: sharp, impulsive — typically the strongest leg
        /// B: retracement of A — typically weaker
        /// C: impulsive, often ≈ A in length — completes the correction
        /// </summary>
        private static decimal? ScoreCorrectivePersonality(ElliottWave wave)
        {
            if (wave.SubWaves == null || wave.SubWaves.Count < 3) return null;

            var aLen = WaveMath.LogDistance(wave.SubWaves[0].StartPoint.Price, wave.SubWaves[0].EndPoint.Price);
            var bLen = WaveMath.LogDistance(wave.SubWaves[1].StartPoint.Price, wave.SubWaves[1].EndPoint.Price);
            var cLen = WaveMath.LogDistance(wave.SubWaves[2].StartPoint.Price, wave.SubWaves[2].EndPoint.Price);

            if (aLen == 0) return null;

            decimal score = 0;
            int checks = 0;

            // B wave should be shorter than A (retracement, not a new impulse)
            if (bLen < aLen) score += 1.0m;
            checks++;

            // C ≈ A relationship (wave equality tendency)
            if (aLen > 0 && cLen > 0)
            {
                var ratio = Math.Min(aLen, cLen) / Math.Max(aLen, cLen);
                score += ratio; // Closer to 1.0 = more equal = better
                checks++;
            }

            // A should be impulsive — check if A has motive sub-structure
            if (wave.SubWaves[0].SubWaves != null && wave.SubWaves[0].SubWaves.Count > 0)
            {
                bool aIsMotive = wave.SubWaves[0].SubWaves.Count >= 5
                    && wave.SubWaves[0].SubWaves.Any(sw => sw.Label == WaveNumber.One);
                score += aIsMotive ? 1.0m : 0.5m;
                checks++;
            }

            return checks > 0 ? Math.Round(score / checks, 4) : null;
        }

        // ── §7.1: B-Wave Retracement ──

        /// <summary>
        /// B-wave retracement scoring, context-aware per §4.2:
        /// - ZigZag B: ideal range 38.2%-78.6% of A
        /// - Flat B: ideal range 90%-145% of A (covers regular ≈100%, expanded 123.6%-138.2%)
        /// </summary>
        private static decimal? ScoreBWaveRetracement(ElliottWave wave)
        {
            if (wave.SubWaves.Count < 2) return null;

            var aMove = WaveMath.LogDistance(wave.SubWaves[0].StartPoint.Price, wave.SubWaves[0].EndPoint.Price);
            var bRetrace = WaveMath.LogDistance(wave.SubWaves[1].StartPoint.Price, wave.SubWaves[1].EndPoint.Price);

            if (aMove == 0) return null;
            var ratio = bRetrace / aMove;

            bool isFlat = wave.PatternType is PatternType.Flat;

            if (isFlat)
            {
                // Flat B: ideal at 90%-145% (regular ≈100%, expanded 123.6%-138.2%)
                if (ratio >= 0.90m && ratio <= 1.45m)
                    return 1.0m;
                if (ratio < 0.90m)
                    return Math.Max(0, 1.0m - (0.90m - ratio) / 0.90m);
                return Math.Max(0, 1.0m - (ratio - 1.45m) / 0.55m);
            }

            // ZigZag B (and other patterns): ideal at 38.2%-78.6%
            if (ratio >= Fibonacci.R382 && ratio <= 0.786m)
                return 1.0m;
            if (ratio < Fibonacci.R382)
                return Math.Max(0, 1.0m - (Fibonacci.R382 - ratio) / Fibonacci.R382);
            return Math.Max(0, 1.0m - (ratio - 0.786m) / (1.0m - 0.786m));
        }

        // ── §5.2: Depth of Correction ──

        /// <summary>
        /// W4 tends to fall within the W4-of-lesser-degree territory.
        /// Compare current W4 range with W4 inside W3's sub-waves.
        /// </summary>
        private static decimal? ScoreDepthOfCorrection(ElliottWave wave)
        {
            if (wave.SubWaves.Count < 4) return null;

            var w3 = wave.SubWaves[2]; // W3
            var w4 = wave.SubWaves[3]; // W4

            // Need W3's sub-waves to find W4-of-lesser-degree
            if (w3.SubWaves == null || w3.SubWaves.Count < 4) return null;

            var lesserW4 = w3.SubWaves[3]; // W4 inside W3

            // W4 price range
            var w4Low = Math.Min(WaveMath.Log(w4.StartPoint.Price), WaveMath.Log(w4.EndPoint.Price));
            var w4High = Math.Max(WaveMath.Log(w4.StartPoint.Price), WaveMath.Log(w4.EndPoint.Price));

            // Lesser-degree W4 price range
            var lw4Low = Math.Min(WaveMath.Log(lesserW4.StartPoint.Price), WaveMath.Log(lesserW4.EndPoint.Price));
            var lw4High = Math.Max(WaveMath.Log(lesserW4.StartPoint.Price), WaveMath.Log(lesserW4.EndPoint.Price));

            // Compute overlap
            var overlapLow = Math.Max(w4Low, lw4Low);
            var overlapHigh = Math.Min(w4High, lw4High);

            if (overlapHigh <= overlapLow) return 0.0m; // No overlap

            var overlapRange = overlapHigh - overlapLow;
            var lesserRange = lw4High - lw4Low;

            return lesserRange > 0 ? Math.Min(1.0m, overlapRange / lesserRange) : 0.0m;
        }

        // ── §7.3: Triangle Internal Ratios ──

        /// <summary>
        /// Triangle sub-wave ratios: c≈0.618a, e≈0.618c, d≈0.618b.
        /// </summary>
        private static decimal? ScoreTriangleRatios(ElliottWave wave)
        {
            if (wave.SubWaves.Count < 5) return null;

            var amps = wave.SubWaves.Select(WaveAmplitude).ToArray();

            decimal totalDeviation = 0;
            int ratioCount = 0;

            // c/a ≈ 0.618
            if (amps[0] > 0) { totalDeviation += RatioDeviation(amps[2] / amps[0]); ratioCount++; }
            // e/c ≈ 0.618
            if (amps[2] > 0) { totalDeviation += RatioDeviation(amps[4] / amps[2]); ratioCount++; }
            // d/b ≈ 0.618
            if (amps[1] > 0) { totalDeviation += RatioDeviation(amps[3] / amps[1]); ratioCount++; }

            if (ratioCount == 0) return null;
            return Math.Max(0, 1.0m - totalDeviation / ratioCount);
        }

        private static decimal RatioDeviation(decimal actual) =>
            Math.Abs(actual - Thresholds.TriangleInternalRatio) / Thresholds.TriangleInternalRatio;

        // ── §7.2: Golden Section Division ──

        /// <summary>
        /// W4 divides the impulse into golden ratio segments (0.382/0.618).
        /// </summary>
        private static decimal? ScoreGoldenSectionDivision(ElliottWave wave)
        {
            if (wave.SubWaves.Count < 5) return null;

            var totalRange = WaveMath.LogDistance(
                wave.SubWaves[0].StartPoint.Price, wave.SubWaves[4].EndPoint.Price);
            if (totalRange == 0) return null;

            // W0 to W3 end (the segment before W4)
            var segmentToW3 = WaveMath.LogDistance(
                wave.SubWaves[0].StartPoint.Price, wave.SubWaves[2].EndPoint.Price);

            var ratio = segmentToW3 / totalRange;

            // Ideal: ratio is either 0.618 or 0.382
            var dev618 = Math.Abs(ratio - Fibonacci.R618);
            var dev382 = Math.Abs(ratio - Fibonacci.R382);
            var minDev = Math.Min(dev618, dev382);

            return Math.Max(0, 1.0m - minDev / Fibonacci.R382);
        }

        // ── §7.5: Time Relationships ──

        /// <summary>
        /// Time spans between turning points compared to Fibonacci numbers.
        /// </summary>
        private static decimal? ScoreTimeRelationships(ElliottWave wave)
        {
            if (wave.SubWaves == null || wave.SubWaves.Count < 2) return null;

            decimal totalProximity = 0;
            int spanCount = 0;

            foreach (var sub in wave.SubWaves)
            {
                if (sub.IsInProgress) continue;

                // Time span in hours
                var hours = (sub.EndPoint.Timestamp - sub.StartPoint.Timestamp).TotalHours;
                if (hours <= 0) continue;

                // Convert to appropriate unit based on degree
                double periods = wave.Degree switch
                {
                    WaveDegree.Cycle => hours / (Thresholds.MonthlyCandleDays * 24.0),
                    WaveDegree.Primary => hours / 24.0,
                    _ => hours
                };

                if (periods <= 0) continue;

                // Find nearest Fibonacci time number
                double nearestDist = double.MaxValue;
                foreach (var fib in Thresholds.FibonacciTimeNumbers)
                {
                    var dist = Math.Abs(periods - fib) / fib;
                    if (dist < nearestDist) nearestDist = dist;
                }

                totalProximity += (decimal)Math.Min(1.0, nearestDist);
                spanCount++;
            }

            if (spanCount == 0) return null;
            return Math.Max(0, 1.0m - totalProximity / spanCount);
        }

        // ── §7.4: Projection Clustering ──

        private static void CollectProjectionTargets(
            List<ElliottWave> waves, List<(ElliottWave Wave, Target Target)> targets)
        {
            foreach (var wave in waves)
            {
                if (wave.IsInProgress && wave.Projection?.Targets != null)
                {
                    foreach (var t in wave.Projection.Targets)
                        targets.Add((wave, t));
                }
                if (wave.SubWaves != null)
                    CollectProjectionTargets(wave.SubWaves, targets);
            }
        }

        /// <summary>
        /// §7.4: Score how many projection targets from different waves converge
        /// on the same price zone as this wave's targets.
        /// </summary>
        private static decimal? ScoreProjectionClustering(
            ElliottWave wave, List<(ElliottWave Wave, Target Target)> allTargets)
        {
            if (wave.Projection?.Targets == null || wave.Projection.Targets.Count == 0)
                return null;

            int maxCluster = 0;
            foreach (var myTarget in wave.Projection.Targets)
            {
                var myLog = WaveMath.Log(myTarget.Price);
                int clusterCount = 0;
                foreach (var (otherWave, otherTarget) in allTargets)
                {
                    if (ReferenceEquals(otherWave, wave)) continue;
                    var otherLog = WaveMath.Log(otherTarget.Price);
                    if (Math.Abs(myLog - otherLog) <= Thresholds.ProjectionClusteringProximity)
                        clusterCount++;
                }
                maxCluster = Math.Max(maxCluster, clusterCount);
            }

            if (maxCluster == 0) return 0.0m;
            return Math.Min(1.0m, (decimal)maxCluster / (Thresholds.ProjectionClusteringMaxTargets - 1));
        }

        // ══════════════════════════════════════════════════
        // Enhanced §4.4: Complex corrective rules
        // (integrated into existing ClassifyComplexSubType)
        // ══════════════════════════════════════════════════

        /// <summary>
        /// Replace outlier Low prices (listing data artifacts) in-place.
        /// Uses the prior candle's Low as replacement so aggregations stay clean.
        /// </summary>
        private static void SanitizeOutlierPrices(List<Ohlcv> candles)
        {
            for (int i = 0; i < candles.Count; i++)
            {
                if (candles[i].Low < Thresholds.OutlierPriceThreshold && i > 0
                    && candles[i - 1].Low >= Thresholds.OutlierPriceThreshold)
                {
                    candles[i] = new Ohlcv
                    {
                        TimestampUtc = candles[i].TimestampUtc,
                        Open = candles[i].Open,
                        High = candles[i].High,
                        Low = candles[i - 1].Low,
                        Close = candles[i].Close,
                        Volume = candles[i].Volume
                    };
                }
            }
        }

        /// <summary>
        /// Aggregate candles from one timeframe to another.
        /// </summary>
        internal static List<Ohlcv> AggregateCandles<TKey>(List<Ohlcv> candles,
            Func<Ohlcv, TKey> keySelector, Func<TKey, DateTime> timestampSelector)
        {
            return candles.GroupBy(keySelector).Select(g => new Ohlcv
            {
                TimestampUtc = timestampSelector(g.Key),
                Open = g.First().Open,
                High = g.Max(x => x.High),
                Low = g.Min(x => x.Low),
                Close = g.Last().Close,
                Volume = g.Sum(x => x.Volume)
            }).OrderBy(c => c.TimestampUtc).ToList();
        }
    }
}
