namespace SmarterSystems.ElliottWaves.Analyzer.Interfaces
{
    public static class Fibonacci
    {
        // Retracement levels
        public const decimal R118 = 0.118m; // R236 × R50: minimum Fibonacci-derived recovery for pivot confirmation
        public const decimal R236 = 0.236m;
        public const decimal R382 = 0.382m;
        public const decimal R50 = 0.5m;
        public const decimal R618 = 0.618m;

        // Extension levels
        public const decimal E4236 = 4.236m;
    }

    /// <summary>
    /// Named constants for pattern classification and scoring thresholds.
    /// Eliminates magic numbers throughout the analyzer and labeler.
    ///
    /// Constants are organized into three categories:
    /// 1. EW-derived: directly from Frost &amp; Prechter Fibonacci ratios or rules
    /// 2. Structural: minimum data requirements for meaningful analysis
    /// 3. Empirical: tuned heuristics for scoring/classification (not F&amp;P rules)
    /// </summary>
    public static class Thresholds
    {
        // ══════════════════════════════════════════════════
        // Active constants — referenced by current analyzer code
        // ══════════════════════════════════════════════════

        /// <summary>W1 confirmation: retrace must exceed 50% of the move.</summary>
        public const decimal W1ConfirmationRetrace = 0.50m;

        /// <summary>§3.2: Extension = wave > 1.618× both other motive waves. Direct Fibonacci ratio.</summary>
        public const decimal ExtensionMultiple = 1.618m;

        /// <summary>Maximum log-scale price difference (5%) between the absolute countertrend
        /// extreme and a later confirmed extreme for preferring the later one as B endpoint.
        /// Catches "double-top" B patterns where two near-identical peaks exist but the last
        /// one is the structural B→C boundary (e.g., two peaks within 2% of each other).
        /// Must reject later peaks that are structurally different (e.g., 10% apart).</summary>
        public const decimal BEndpointProximity = 0.05m;

        /// <summary>Log-scale proximity for Regular vs Running flat classification.
        /// C ≈ A end (within 5%) → Regular Flat; beyond → Running Flat.
        /// Same numeric value as BEndpointProximity but semantically independent.</summary>
        public const decimal FlatRegularProximity = 0.05m;

        /// <summary>Minimum candles to attempt sub-wave detection. Below this, the data is too sparse.</summary>
        public const int MinCandlesForSubWaves = 5;

        /// <summary>Empirical: "right look" (§5.7) — shortest motive wave must be ≥15% of longest (log scale).
        /// Prevents degenerate impulses where one wave is negligible, which indicates the structure
        /// is actually corrective rather than a true 5-wave impulse.</summary>
        public const decimal MinMotiveWaveProportion = 0.15m;

        // ── SubWaveDetector: Pivot scanning thresholds ──

        /// <summary>Minimum candle gap for Primary-degree W1 detection.
        /// 1 daily candle — at daily resolution, a 1-candle gap suffices to distinguish
        /// genuine swings from intra-candle noise.</summary>
        public const int PrimaryMinGap = 1;

        /// <summary>Minimum candle gap for Intermediate-degree W1 detection.
        /// 48 hourly candles = 2 days — ensures the first pivot is a genuine multi-day swing,
        /// not intra-day noise at hourly resolution.</summary>
        public const int IntermediateMinGap = 48;

        /// <summary>Minimum log-scale move for Intermediate-degree W1 detection.
        /// 0.20 ≈ 22% price move — filters out micro-wiggles at hourly resolution.</summary>
        public const decimal IntermediateMinLogMove = 0.20m;

        /// <summary>Stricter minimum log-scale move for Intermediate-degree ATH-as-W1 fallback.
        /// 0.40 ≈ 49% price move — ensures only structurally significant swings qualify.</summary>
        public const decimal IntermediateMinLogMoveStrict = 0.40m;

        // ── SubWaveDetector: Late-retrace detection ──

        /// <summary>W4 is considered "late" if it falls in the last 5% of the parent time span.
        /// Late W4s are likely noise near ATH, not structural pivots.</summary>
        public const double LateRetraceThreshold = 0.95;

        /// <summary>When a late W4 is detected, truncate the search range to 90% of parent span
        /// to find an earlier structural retrace.</summary>
        public const double LateRetraceTruncation = 0.90;

        /// <summary>Minimum parent span (in candles) to apply late-retrace detection.
        /// Very short parents don't have enough data for meaningful late-retrace filtering.</summary>
        public const int LateRetraceMinSpan = 20;

        // ── SubWaveDetector: Time proportionality ──

        /// <summary>Maximum W2/W1 duration ratio before triggering deeper-W1 search.
        /// When W2 is more than 3× longer than W1, the initial W1 is likely too shallow.</summary>
        public const int MaxW2W1DurationRatio = 3;

        // ── Outlier / data quality ──

        /// <summary>Prices below this are considered data artifacts (listing glitches).
        /// Used by both CycleDetector and ElliottWavesAnalyzer for sanitization.</summary>
        public const decimal OutlierPriceThreshold = 0.001m;

        // ── CycleDetector: W3 length tolerance ──

        /// <summary>W3 length tolerance: W3 must be ≥ 98% of W1 length (2% tolerance for
        /// borderline cases where W3 is marginally shorter due to rounding).</summary>
        public const decimal W3LengthTolerance = 0.98m;

        // ── Triangle classification ──

        /// <summary>Minimum log-scale range to avoid division by zero in triangle classification.
        /// Prevents degenerate triangles with near-zero total range.</summary>
        public const decimal MinTriangleRange = 0.01m;

        /// <summary>Fraction of total range below which a trendline is considered "flat" for
        /// ascending/descending triangle classification (20% of range).</summary>
        public const decimal TriangleFlatThreshold = 0.20m;

        // ── Pattern classification ──

        /// <summary>Minimum retrace fraction for corrective wave detection.
        /// Product of R236 × R618 ≈ 14.6% — filters out shallow wiggles that
        /// don't represent meaningful A-wave swings in corrective structures.</summary>
        public const decimal CorrectiveMinRetraceFraction = 0.146m;

        /// <summary>Double combo net progress limit: net progress must be less than 1.5× W amplitude
        /// to distinguish W-X-Y from a simple A-B-C where C exceeds A significantly.</summary>
        public const decimal DoubleComboMaxProgress = 1.5m;

        /// <summary>Majority ratio for convergence/expansion detection in triangle amplitude analysis.
        /// At least 60% of successive wave pairs must converge/expand.</summary>
        public const double ConvergenceMajorityRatio = 0.6;

        /// <summary>Majority ratio for expanding triangle detection. Uses same threshold as
        /// convergence for consistency — at least 60% of successive pairs must expand.</summary>
        public const double ExpandingMajorityRatio = 0.6;

        // ── CycleDetector: W2/W3 structural confirmation ──

        /// <summary>W2 completion: minimum B-wave bounce in log-scale (0.75 ≈ 112% price move).
        /// A larger bounce threshold distinguishes completed A-B-C corrections from ongoing
        /// single-leg moves. Empirically tuned for crypto's volatile corrections.</summary>
        public const decimal W2CompletionMinBounceLog = 0.75m;

        /// <summary>W2 completion: minimum C-beyond-A extension in log-scale (0.65 ≈ 92% price move).
        /// Confirms the C wave makes meaningful progress beyond A, proving structural completion.</summary>
        public const decimal W2CompletionMinCExtensionLog = 0.65m;

        /// <summary>Default minimum bounce in log-scale for HasCompletedABC (W4 completion).
        /// 0.60 ≈ 82% price move — lower than W2 because W4 corrections are typically shallower.</summary>
        public const decimal DefaultMinBounceLog = 0.60m;

        /// <summary>Log-scale tolerance for finding an earlier peak near the absolute extreme
        /// (double-top detection). Same as BEndpointProximity — 5% log-scale proximity.</summary>
        public const decimal PeakProximityTolerance = 0.05m;

        // ── SubWaveDetector: W3 detection thresholds ──

        /// <summary>Relaxed W3 fraction: R236 × R50 ≈ 11.8%.
        /// Used as a mid-level threshold for detecting W3 impulses with moderate retraces.</summary>
        public const decimal W3FractionRelaxedMid = Fibonacci.R236 * Fibonacci.R50;

        /// <summary>Relaxed W3 fraction: R236 × R236 ≈ 5.6%.
        /// Catches large W3 moves with proportionally shallow retraces (strong impulses).</summary>
        public const decimal W3FractionRelaxedLow = Fibonacci.R236 * Fibonacci.R236;

        /// <summary>Fallback minimum log-move when estimated correction total is near zero.
        /// 0.5 log units ≈ 65% price move — prevents the R382 fraction from producing a
        /// threshold too small to filter noise.</summary>
        public const decimal CorrectiveFallbackMinLogMove = 0.5m;

        /// <summary>Search window radius (in candles) around a detected near-peak for
        /// double-top extreme refinement. 2-candle radius = 5-candle window.</summary>
        public const int DoublePeakSearchRadius = 2;

        /// <summary>Maximum candle offset from parent start to qualify as a listing spike.
        /// Peaks within the first 2 candles indicate the entire initial move occurred in
        /// a single period (listing artifact).</summary>
        public const int ListingSpikeMaxStartOffset = 2;

        // ── CycleDetector: recovery and outlier detection ──

        /// <summary>Default recovery threshold (10% log-scale) for confirming extremes.
        /// Used by FindConfirmedExtreme and HasSignificantReversal.</summary>
        public const decimal DefaultRecoveryThreshold = 0.10m;

        /// <summary>Outlier multiplier: if the next low is more than 10× the W0 candidate,
        /// the W0 is a listing artifact.</summary>
        public const decimal OutlierMultiplier = 10m;

        // ── Projection probability redistribution ──

        /// <summary>Probability assigned to the nearest Fibonacci target in cycle projection redistribution.</summary>
        public const decimal NearestTargetProbability = 0.40m;

        /// <summary>Total probability distributed among non-nearest targets.</summary>
        public const decimal RemainingTargetProbability = 0.60m;

        // ── Guideline scoring (Frost & Prechter §5-§7) ──

        /// <summary>§7.3: Expected triangle internal ratio for c/a, e/c, d/b relationships.</summary>
        public const decimal TriangleInternalRatio = Fibonacci.R618;

        /// <summary>§7.5: Fibonacci time numbers for time relationship scoring between turning points.</summary>
        public static readonly int[] FibonacciTimeNumbers = [8, 13, 21, 34, 55, 89, 144, 233];

        /// <summary>§7.4: Log-scale proximity for detecting projection clustering (5%).</summary>
        public const decimal ProjectionClusteringProximity = 0.05m;

        /// <summary>§7.4: Number of converging targets needed for maximum clustering score.</summary>
        public const int ProjectionClusteringMaxTargets = 4;

        // ── ChartRenderer ──

        /// <summary>Lower percentile bound for y-axis range (0.5th percentile). Excludes flash crash outliers.</summary>
        public const double PercentileLow = 0.005;

        /// <summary>Upper percentile bound for y-axis range (99.5th percentile). Excludes spike outliers.</summary>
        public const double PercentileHigh = 0.995;

        /// <summary>Maximum search radius (in candles) when finding a candle by price near a timestamp.</summary>
        public const int ChartSearchRadius = 800;

        /// <summary>Margin from y-axis edge when clamping projection arrows to visible range.</summary>
        public const double ClampMargin = 0.1;

        /// <summary>Cycle-degree future projection offset in hours (3 months).</summary>
        public const int CycleFutureOffsetHours = 3 * 30 * 24;

        /// <summary>Primary-degree future projection offset in hours (1 month).</summary>
        public const int PrimaryFutureOffsetHours = 1 * 30 * 24;

        /// <summary>Intermediate-degree future projection offset in hours (1 week).
        /// Provides visible separation on Daily charts (7 candles) while remaining
        /// proportional on Hourly charts (168 candles).</summary>
        public const int IntermediateFutureOffsetHours = 7 * 24;

        /// <summary>Approximate days per month for candle duration and projection calculations.</summary>
        public const int MonthlyCandleDays = 30;

        /// <summary>Y-axis log-scale padding to accommodate stacked wave labels (Cycle/Primary/Intermediate).
        /// 0.55 log units ≈ 3.5× price factor — ensures lowest label is never clipped.</summary>
        public const double YAxisLogPadding = 0.55;

        /// <summary>Maximum log-decade distance from percentile bounds for including edge labels.
        /// Labels beyond 1 decade (10× price) from the visible range are extreme outliers.</summary>
        public const double YAxisEdgeLabelDecadeThreshold = 1.0;

        /// <summary>Target number of tick labels on the X-axis. Divides candle count to determine tick interval.</summary>
        public const int TargetXAxisTickCount = 30;

        /// <summary>Pixel offset for wave labels above/below projection arrowheads.</summary>
        public const int ProjectionLabelOffsetPx = 12;

        /// <summary>X-axis extension factor beyond the furthest projection arrow.
        /// 30% extra space prevents label clipping at the chart edge.</summary>
        public const double XAxisExtensionFactor = 0.30;
    }
}
