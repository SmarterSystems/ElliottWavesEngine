namespace SmarterSystems.ElliottWaves.Analyzer.Interfaces
{
    public enum WaveDegree { Cycle, Primary, Intermediate }
    public enum WaveNumber { One, Two, Three, Four, Five, A, B, C, D, E, W, X1, Y, X2, Z }
    public enum PatternType { Impulse, Diagonal, ZigZag, Flat, Triangle, Complex }

    public enum PatternSubType
    {
        // Impulse sub-types
        Extended1,              // Wave 1 is extended (>1.618× others)
        Extended3,              // Wave 3 is extended (most common)
        Extended5,              // Wave 5 is extended
        Truncation,             // Wave 5 fails to exceed Wave 3 end

        // Diagonal sub-types
        LeadingDiagonal,        // Diagonal in W1 or A position (5-3-5-3-5, continuation)
        EndingDiagonal,         // Diagonal in W5 or C position (3-3-3-3-3, exhaustion)

        // Zigzag sub-types
        DoubleZigzag,           // W-X-Y where W and Y are both zigzags
        TripleZigzag,           // W-X-Y-X-Z where W, Y, Z are all zigzags

        // Flat sub-types
        RegularFlat,            // B ≈ 100% of A, C slightly past A end
        ExpandedFlat,           // B > 100% of A (exceeds A start), C well past A end
        RunningFlat,            // B > 100% of A, C falls short of A end (rare, strong trends)

        // Triangle sub-types
        ContractingSymmetrical, // Both trendlines converge symmetrically
        ContractingAscending,   // Upper trendline flat, lower trendline rising
        ContractingDescending,  // Lower trendline flat, upper trendline declining
        ExpandingTriangle,      // Both trendlines diverge (reverse symmetrical)
        RunningTriangle,        // B exceeds start of A (extremely common)
    }

    public class ElliottWavesAnalysis
    {
        public required List<ElliottWave> Waves { get; set; } = [];
    }

    public class ElliottWave
    {
        public required WaveDegree Degree { get; set; }
        public required WaveNumber Label { get; set; }
        public required PivotPoint StartPoint { get; set; }
        public required PivotPoint EndPoint { get; set; }
        public required bool IsInProgress { get; set; }
        public PatternType? PatternType { get; set; }
        public PatternSubType? PatternSubType { get; set; }
        public Projection Projection { get; set; }
        public List<ElliottWave> SubWaves { get; set; }

        /// <summary>
        /// Section 5.8: Orthodox end point. For truncated impulses, the orthodox end is the W3
        /// peak (not W5 end). Null when orthodox endpoint equals the actual EndPoint.
        /// Informational for JSON consumers; internal projection logic uses EndPoint directly.
        /// </summary>
        public PivotPoint OrthodoxEndPoint { get; set; }

        /// <summary>
        /// Section 8.3: Alternate wave interpretations. Each alternate is a complete sub-wave
        /// decomposition that satisfies all inviolable rules but scored lower than the preferred count.
        /// The preferred count is in SubWaves; alternates are ordered by descending score.
        /// </summary>
        public List<List<ElliottWave>> AlternateCounts { get; set; }

        /// <summary>
        /// Section 5.3: Post-extension retrace level. When the preceding motive wave contained
        /// an extended impulse, the correction typically retraces to the price territory of wave 2
        /// of that impulse. This property stores that W2 end price for projection/validation use.
        /// Null when the preceding wave had no extension or sub-wave data is unavailable.
        /// </summary>
        public decimal? PostExtensionRetraceLevel { get; set; }

        /// <summary>
        /// Section 4.2: B-wave impulse warning. True when a supposed B wave's internal structure
        /// can form a valid 5-wave impulse, suggesting the correction count may be incorrect —
        /// the "B wave" might actually be wave 1 of a new impulse sequence.
        /// </summary>
        public bool BWaveImpulseWarning { get; set; }

        /// <summary>
        /// Section 3.3: Truncation verification flag. True when a truncated W5 could not be
        /// verified to contain the required internal 5-wave (motive) structure. The truncation
        /// detection (by price) still stands, but confidence is lower without structural verification.
        /// </summary>
        public bool TruncationUnverified { get; set; }

        /// <summary>
        /// FR13: Fibonacci justification for this wave's endpoint detection.
        /// Records which Fibonacci level was matched, the reference wave,
        /// and the actual deviation from the exact level.
        /// </summary>
        public FibonacciJustification Justification { get; set; }

        /// <summary>
        /// Guideline satisfaction scores for this wave. Each property is scored 0.0-1.0
        /// (null = guideline not applicable to this wave type). Populated by post-processing
        /// after all three degrees of sub-wave detection are complete.
        /// </summary>
        public GuidelineSatisfaction Guidelines { get; set; }
    }

    /// <summary>
    /// Records how a wave endpoint was detected via Fibonacci projection.
    /// </summary>
    public class FibonacciJustification
    {
        /// <summary>The Fibonacci level matched (e.g., 0.618 for 61.8% retrace)</summary>
        public required decimal FibonacciLevel { get; set; }

        /// <summary>Deviation from exact level in log-scale (e.g., 0.008 = 0.8%)</summary>
        public required decimal Deviation { get; set; }

        /// <summary>Type of Fibonacci relationship: "Retracement" or "Extension"</summary>
        public required string Type { get; set; }

        /// <summary>Human-readable description (e.g., "61.8% retrace of W0→W1")</summary>
        public required string Description { get; set; }
    }

    /// <summary>
    /// Frost &amp; Prechter guideline satisfaction scores. Each property is 0.0-1.0 where
    /// 1.0 = perfect satisfaction of the guideline. Null = guideline not applicable.
    /// </summary>
    public class GuidelineSatisfaction
    {
        /// <summary>§5.4: Two non-extended motive waves tend toward equality in magnitude.</summary>
        public decimal? WaveEquality { get; set; }

        /// <summary>§5.1: W2 and W4 alternate between sharp (zigzag) and sideways (flat/triangle).</summary>
        public decimal? Alternation { get; set; }

        /// <summary>§5.2: W4 falls within W4-of-lesser-degree territory.</summary>
        public decimal? DepthOfCorrection { get; set; }

        /// <summary>§7.3: Triangle sub-wave ratios approximate 0.618 relationships.</summary>
        public decimal? TriangleRatios { get; set; }

        /// <summary>§7.2: W4 divides the impulse into golden ratio segments (0.382/0.618).</summary>
        public decimal? GoldenSectionDivision { get; set; }

        /// <summary>§7.5: Time spans between turning points match Fibonacci numbers.</summary>
        public decimal? TimeRelationships { get; set; }

        /// <summary>§6: Wave behavior matches expected personality (W3 strongest, W2 deep, etc.).</summary>
        public decimal? WavePersonality { get; set; }

        /// <summary>§7.4: Multiple Fibonacci projections cluster at the same price zone.</summary>
        public decimal? ProjectionClustering { get; set; }

        /// <summary>§7.1: B-wave retracement within expected 38.2%-78.6% range.</summary>
        public decimal? BWaveRetracement { get; set; }

        /// <summary>Average of all non-null guideline scores.</summary>
        public decimal? CompositeScore
        {
            get
            {
                var scores = new[] { WaveEquality, Alternation, DepthOfCorrection,
                    TriangleRatios, GoldenSectionDivision, TimeRelationships,
                    WavePersonality, ProjectionClustering, BWaveRetracement };
                var valid = scores.Where(s => s.HasValue).Select(s => s.Value).ToArray();
                return valid.Length > 0 ? Math.Round(valid.Average(), 4) : null;
            }
        }
    }

    public class Projection
    {
        public required List<Target> Targets { get; set; }
        public required decimal? InvalidationPoint { get; set; }
    }

    public class Target
    {
        public required decimal Price { get; set; }
        public required decimal FibonacciLevel { get; set; }
        public required decimal Probability { get; set; }
    }
}
