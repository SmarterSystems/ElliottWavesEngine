# Architecture Design — Fibonacci-Driven Elliott Wave Detection Engine

## 1. Design Philosophy

Replace the current **pivot-first, score-later** approach with a **forward-projection model** where Fibonacci ratios are the structural driver, not decoration. The algorithm confirms early waves, projects the next wave's endpoint via Fibonacci, finds the closest matching pivot within tolerance, confirms or rejects, and proceeds.

---

## 2. Component Architecture

### 2.1 Class Diagram

```
ElliottWavesAnalyzer (orchestrator — public API preserved)
│
├── FibonacciProjector (static)
│   Pure math: project retracement/extension targets from reference wave
│
├── CycleDetector
│   Forward-projection on monthly data: W0 → W1 → W2 → W3 → W4 → W5(in-progress)
│   Pattern morphing: impulse → diagonal / impulse → A-B-C
│
├── SubWaveDetector
│   Wave-by-wave iteration through parent boundaries
│   Same Fibonacci logic, constrained by parent start/end
│
├── PivotScanner
│   Find extreme points in candle data within a date/price range
│   Find pivot nearest a Fibonacci projection within tolerance
│
├── WaveLabeler (rebuilt — simpler)
│   Create ElliottWave objects from confirmed points
│   Assign labels, pattern types, projections
│
├── WaveMath (KEPT — no changes)
│   Log, LogDistance, TargetPrice, IsUptrend, ExceedsInDirection, DetectExtension
│
└── ChartRenderer (KEPT — no changes)
    Render/ChartRenderer.cs untouched
```

### 2.2 File Layout

```
Analysis/
├── ElliottWavesAnalyzer.cs    — REWRITE (orchestrator, ~200 lines)
├── FibonacciProjector.cs      — NEW (pure Fibonacci math, ~150 lines)
├── CycleDetector.cs           — NEW (cycle-degree forward-projection, ~300 lines)
├── SubWaveDetector.cs         — NEW (lower-degree detection, ~250 lines)
├── PivotScanner.cs            — NEW (replaces PivotDetector, ~150 lines)
├── WaveLabeler.cs             — REWRITE (simplified labeling, ~200 lines)
├── WaveMath.cs                — KEEP (no changes)

DELETE:
├── PivotDetector.cs           — replaced by PivotScanner
├── WaveValidator.cs           — rules integrated into CycleDetector/SubWaveDetector
├── WaveScorer.cs              — scoring replaced by Fibonacci projection matching

Interfaces/
├── ElliottWavesAnalysis.cs    — KEEP + add FibonacciJustification property
├── PivotPoint.cs              — KEEP (no changes)
├── Fibonacci.cs               — MODIFY (add missing constants, update Thresholds)
├── IWaveLabeler.cs            — DELETE (internal interface, no longer needed)
├── IPivotDetector.cs          — DELETE (internal interface, no longer needed)

Render/
├── ChartRenderer.cs           — KEEP (no changes)
```

---

## 3. DTO Amendment (FR13)

Single addition to `ElliottWave`:

```csharp
/// <summary>
/// Fibonacci justification for this wave's endpoint detection.
/// Records which Fibonacci level was matched, the reference wave,
/// and the actual deviation from the exact level.
/// </summary>
public FibonacciJustification Justification { get; set; }
```

New class (in Interfaces/ElliottWavesAnalysis.cs):

```csharp
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
```

---

## 4. Core Algorithm: FibonacciProjector

Static class. Pure math, no state, fully testable.

```csharp
public static class FibonacciProjector
{
    // Retracement levels ordered most extreme first (for brute-force)
    public static readonly decimal[] RetracementLevels = [0.786m, 0.618m, 0.5m, 0.382m];

    // W4 retracement levels include 23.6% for shallow corrections
    public static readonly decimal[] W4RetracementLevels = [0.786m, 0.618m, 0.5m, 0.382m, 0.236m];

    // Extension levels ordered most extreme first
    public static readonly decimal[] ExtensionLevels = [4.236m, 2.618m, 1.618m, 1.0m];

    // W5 projection levels
    public static readonly decimal[] W5Levels = [1.618m, 1.0m, 0.618m];

    /// <summary>
    /// Project a retracement target price from a wave's start and end.
    /// Log-scale: target = end_log - fibLevel * (end_log - start_log)
    /// Returns (targetPrice, fibLevel) for each level.
    /// </summary>
    public static List<(decimal Price, decimal FibLevel)> ProjectRetracements(
        decimal waveStart, decimal waveEnd, decimal[] levels);

    /// <summary>
    /// Project an extension target price from a measured wave, starting from a base.
    /// Log-scale: target = base_log + fibLevel * wave_log_length
    /// </summary>
    public static List<(decimal Price, decimal FibLevel)> ProjectExtensions(
        decimal waveStart, decimal waveEnd, decimal extensionBase, decimal[] levels);

    /// <summary>
    /// Check if a price matches a target within tolerance (log-scale).
    /// Returns (matched, deviation) where deviation is abs log distance.
    /// </summary>
    public static (bool Matched, decimal Deviation) CheckMatch(
        decimal actual, decimal target, decimal tolerance = 0.02m);
}
```

All log-scale math. `ProjectRetracements` calculates where wave endpoints should land for corrections. `ProjectExtensions` calculates where impulse waves should reach. These are the building blocks used by both `CycleDetector` and `SubWaveDetector`.

---

## 5. Core Algorithm: PivotScanner

Replaces `PivotDetector`. Focused on finding specific price points rather than generic pivot detection.

```csharp
public static class PivotScanner
{
    /// <summary>
    /// Find the absolute extreme (high or low) in a candle range.
    /// Returns the candle index, timestamp, and price.
    /// </summary>
    public static PivotPoint FindAbsoluteExtreme(
        List<Ohlcv> candles, bool findHigh,
        int fromIndex = 0, int toIndex = -1);

    /// <summary>
    /// Find the absolute extreme between two dates (inclusive).
    /// Used for resolving cycle points to hourly resolution.
    /// </summary>
    public static PivotPoint FindAbsoluteExtremeBetween(
        List<Ohlcv> candles, bool findHigh,
        DateTime from, DateTime to);

    /// <summary>
    /// Scan forward from a starting point to find W1.
    /// W1 = last absolute extreme before a >50% retrace occurs.
    /// Returns W1 pivot and the retrace point, or null if W1 not confirmed.
    /// </summary>
    public static (PivotPoint W1, PivotPoint RetracePoint)? FindWave1(
        List<Ohlcv> candles, PivotPoint w0, bool isUptrend);

    /// <summary>
    /// Find the pivot nearest to a projected Fibonacci level within tolerance.
    /// Scans the candle range for the most extreme countertrend point
    /// that falls within tolerance of the target price.
    ///
    /// For retracements: find the most extreme point in the countertrend
    /// direction near the target level.
    /// For extensions: find the most extreme point in the trend direction
    /// near the target level.
    /// </summary>
    public static (PivotPoint Pivot, decimal FibLevel, decimal Deviation)?
        FindNearestFibonacciMatch(
            List<Ohlcv> candles,
            List<(decimal Price, decimal FibLevel)> targets,
            bool findHigh,
            int fromIndex, int toIndex = -1,
            decimal tolerance = 0.02m);
}
```

Key difference from `PivotDetector`: **not generic pivot scanning**. Instead, targeted searches for specific price levels projected by Fibonacci.

---

## 6. CycleDetector — Forward Projection Engine

The heart of the rebuild. Implements FR1-FR7.

```csharp
public class CycleDetector
{
    /// <summary>
    /// Detect cycle-degree waves on monthly data using forward-projection.
    /// Returns confirmed waves + in-progress wave with projections.
    /// </summary>
    public List<ElliottWave> DetectCycleWaves(List<Ohlcv> monthlyData);
```

### Algorithm Pseudocode

```
DetectCycleWaves(monthlyData):

    // FR1: Find W0
    absLow = FindAbsoluteExtreme(monthlyData, findHigh: false)
    absHigh = FindAbsoluteExtreme(monthlyData, findHigh: true)
    w0 = whichever comes first chronologically
    isUptrend = (w0 is Low)

    // FR2: Find W1
    result = FindWave1(monthlyData, w0, isUptrend)
    if result is null:
        return [single in-progress wave from w0 with no projection]
    w1 = result.W1

    // FR3: Find W2 via Fibonacci retracement brute-force
    targets = ProjectRetracements(w0.Price, w1.Price, RetracementLevels)
    match = FindNearestFibonacciMatch(candles, targets, findHigh=!isUptrend,
                                      fromIndex=w1.Index)
    if match is null:
        return [W1 confirmed, W2 in-progress with retracement projections]
    w2 = match.Pivot
    w2Justification = { FibLevel=match.FibLevel, Deviation=match.Deviation, Type="Retracement" }

    // FR4: Find W3 via Fibonacci extension brute-force
    w1Length = LogDistance(w0.Price, w1.Price)
    targets = ProjectExtensions(w0.Price, w1.Price, w2.Price, ExtensionLevels)
    match = FindNearestFibonacciMatch(candles, targets, findHigh=isUptrend,
                                      fromIndex=w2.Index)
    if match is null:
        return [W1, W2 confirmed, W3 in-progress with extension projections]
    w3 = match.Pivot

    // R2 check: W3 must not be shortest
    // (Can only fully check when W5 is known; at cycle W5 is in-progress,
    //  so just verify W3 >= W1 for now)
    if LogDistance(w2, w3) < LogDistance(w0, w1):
        // W3 is shorter than W1 — this can't be W3
        // Treat as still in-progress
        return [W1, W2 confirmed, W3 in-progress]

    w3Justification = { FibLevel=match.FibLevel, ... }

    // FR5: Find W4 via Fibonacci retracement of W3
    targets = ProjectRetracements(w2.Price, w3.Price, W4RetracementLevels)
    match = FindNearestFibonacciMatch(candles, targets, findHigh=!isUptrend,
                                      fromIndex=w3.Index)
    if match is null:
        return [W1-W3 confirmed, W4 in-progress with projections]
    w4 = match.Pivot

    // FR7: Pattern morphing checks
    if w4 violates w0 (price below w0 in uptrend):
        → Morph entire structure to A-B-C
        return LabelAsCorrection(w0, w1, w2, w3)

    if w4 overlaps w1 territory:
        → Mark as diagonal (PatternSubType = EndingDiagonal or LeadingDiagonal)
        → Continue — diagonal allows overlap

    // Alternation assessment (§5.1)
    w2Depth = retrace ratio of W2
    w4Depth = retrace ratio of W4
    alternation = assess if W2/W4 differ in character

    w4Justification = { FibLevel, Deviation, alternation note }

    // FR6: W5 always in-progress at cycle
    w5Targets = ProjectW5Targets(w0, w1, w2, w3, w4)
    return [W1-W4 confirmed, W5 in-progress with w5Targets]
```

### Pattern Morphing Detail

```
MorphToCorrection(w0, w1, w2, w3):
    // At cycle degree, just A-B-C, no sub-type classification
    A = wave from w0 to w1
    B = wave from w1 to w2
    C = wave from w2 to w3 (or in-progress if w3 not found)
    PatternType = ZigZag (generic corrective)
    No further classification at cycle degree
```

---

## 7. SubWaveDetector — Lower Degree Iteration

Implements FR9. Iterates wave-by-wave through parent boundaries.

```csharp
public class SubWaveDetector
{
    /// <summary>
    /// Detect sub-waves within a parent wave's boundaries.
    /// Uses the same Fibonacci forward-projection algorithm.
    /// </summary>
    /// <param name="candles">Daily (for Primary) or hourly (for Intermediate)</param>
    /// <param name="parentWave">The parent wave defining boundaries</param>
    /// <param name="degree">WaveDegree.Primary or WaveDegree.Intermediate</param>
    public List<ElliottWave> DetectSubWaves(
        List<Ohlcv> candles,
        ElliottWave parentWave,
        WaveDegree degree);
```

### Algorithm Pseudocode

```
DetectSubWaves(candles, parentWave, degree):

    // Slice candles to parent's exact time range
    slice = candles.Where(c => c.Timestamp >= parent.Start && c.Timestamp <= parent.End)

    // Determine if parent is motive or corrective
    isMotive = IsMotiveLabel(parentWave.Label)  // W1,W3,W5,A,C = motive; W2,W4,B = corrective

    if isMotive:
        return DetectMotiveSubWaves(slice, parentWave, degree)
    else:
        return DetectCorrectiveSubWaves(slice, parentWave, degree)


DetectMotiveSubWaves(candles, parent, degree):
    // Same forward-projection as CycleDetector but:
    // 1. W0 = parent.StartPoint (exact, not re-detected)
    // 2. W5 end = parent.EndPoint (exact, not re-detected)
    // 3. All waves MUST complete (no in-progress if parent is completed)
    // 4. Projection clustering (§7.4) for stronger targets

    w0 = parent.StartPoint
    isUptrend = (parent moves in trend direction)

    // Find W1 within parent boundaries
    result = FindWave1(candles, w0, isUptrend, maxIndex=parent.EndPoint.Index)
    ...same flow as CycleDetector...

    // Critical: W5 end MUST match parent.EndPoint
    // Use parent.EndPoint directly as W5 end
    // Verify it satisfies R2 (W3 not shortest) and R3 (no overlap unless diagonal)


DetectCorrectiveSubWaves(candles, parent, degree):
    // A-B-C detection within parent boundaries
    // A: First significant swing (same W1 detection logic)
    // B: Fibonacci retracement of A
    // C: Fibonacci extension of A from B end
    // C end MUST match parent.EndPoint
    // Classify: zigzag (5-3-5), flat (3-3-5) based on ratios
```

### Wave-by-Wave Iteration (FR9)

```
For each cycle wave [W1, W2, W3, W4]:
    primaryWaves = SubWaveDetector.DetectSubWaves(dailyData, cycleWave, Primary)
    cycleWave.SubWaves = primaryWaves

    For each primary wave:
        intermediateWaves = SubWaveDetector.DetectSubWaves(hourlyData, primaryWave, Intermediate)
        primaryWave.SubWaves = intermediateWaves
```

---

## 8. WaveLabeler — Simplified

The rebuilt labeler is much simpler. No pattern detection logic — that's in CycleDetector/SubWaveDetector. The labeler just creates `ElliottWave` objects from confirmed pivot points and assigns labels + projections.

```csharp
public static class WaveLabeler
{
    /// <summary>
    /// Create ElliottWave objects from confirmed impulse pivots.
    /// pivots = [W0, W1, W2, W3, W4, W5] (up to 6)
    /// </summary>
    public static List<ElliottWave> CreateImpulseWaves(
        List<PivotPoint> pivots,
        List<FibonacciJustification> justifications,
        WaveDegree degree,
        PatternSubType? subType = null,
        bool w5InProgress = false,
        Projection w5Projection = null);

    /// <summary>
    /// Create ElliottWave objects from confirmed corrective pivots (A-B-C).
    /// </summary>
    public static List<ElliottWave> CreateCorrectiveWaves(
        List<PivotPoint> pivots,
        List<FibonacciJustification> justifications,
        WaveDegree degree,
        PatternType patternType,
        PatternSubType? subType = null);

    /// <summary>
    /// Create a single in-progress wave with projection targets.
    /// </summary>
    public static ElliottWave CreateInProgressWave(
        PivotPoint start, WaveNumber label, WaveDegree degree,
        Projection projection);

    /// <summary>
    /// Calculate W5 projection targets per §7.2.
    /// </summary>
    public static Projection CalculateW5Projection(
        PivotPoint w0, PivotPoint w1, PivotPoint w2,
        PivotPoint w3, PivotPoint w4, bool isUptrend);
}
```

---

## 9. ElliottWavesAnalyzer — Rebuilt Orchestrator

Slim orchestrator. ~200 lines. Delegates all logic.

```csharp
public class ElliottWavesAnalyzer
{
    public ElliottWavesAnalysis Analyze(List<Ohlcv> hourlyData)
    {
        // 1. Aggregate candles (reuse existing static method)
        var dailyData = AggregateCandles(hourlyData, ...);
        var monthlyData = AggregateCandles(dailyData, ...);

        // 2. Detect cycle waves on monthly data
        var cycleDetector = new CycleDetector();
        var cycleWaves = cycleDetector.DetectCycleWaves(monthlyData);

        // 3. Resolve cycle points to hourly timestamps
        ResolveCycleTimestamps(cycleWaves, hourlyData);

        // 4. Detect sub-waves wave-by-wave
        var subDetector = new SubWaveDetector();
        foreach (var cycleWave in cycleWaves.Where(w => !w.IsInProgress))
        {
            cycleWave.SubWaves = subDetector.DetectSubWaves(
                dailyData, cycleWave, WaveDegree.Primary);

            foreach (var primaryWave in cycleWave.SubWaves.Where(w => !w.IsInProgress))
            {
                primaryWave.SubWaves = subDetector.DetectSubWaves(
                    hourlyData, primaryWave, WaveDegree.Intermediate);
            }
        }

        return new ElliottWavesAnalysis { Waves = cycleWaves };
    }

    // AggregateCandles — KEEP existing static method (unchanged)
    public static List<Ohlcv> AggregateCandles<TKey>(...) { ... }

    // ResolveCycleTimestamps — resolve monthly pivots to exact hourly candles
    private void ResolveCycleTimestamps(List<ElliottWave> waves, List<Ohlcv> hourlyData) { ... }
}
```

---

## 10. Fibonacci Constants Update

Add missing extension level to `Fibonacci` class:

```csharp
public static class Fibonacci
{
    // Existing (keep all)
    ...

    // Add:
    public const decimal E4236 = 4.236m;   // §7 key ratios: extreme extension
    public const decimal R886 = 0.886m;     // NOT USED — excluded per governing authority
}
```

Update `Thresholds`:

```csharp
/// <summary>Fibonacci projection match tolerance: 2% log-scale.</summary>
public const decimal FibonacciMatchTolerance = 0.02m;

/// <summary>W1 confirmation: retrace must exceed 50% of the move.</summary>
public const decimal W1ConfirmationRetrace = 0.50m;
```

---

## 11. Timestamp Resolution Strategy

Cycle points detected on monthly data must be mapped to exact hourly candles:

```
For each cycle wave point (start/end):
    1. Get the monthly candle's date range (1st of month → last of month)
    2. Filter hourly candles within that date range
    3. Find the exact hourly candle where High == price (for High points)
       or Low == price (for Low points)
    4. Use that hourly candle's timestamp and price as the resolved point
```

For lower degree points detected on daily data:
- Same approach: find the hourly candle within the daily candle's date range
  that matches the exact High/Low price

---

## 12. Test Architecture

### 12.1 Structural Unit Tests (synthetic data)

```
Tests/
├── FibonacciProjectorTests.cs
│   - ProjectRetracements_ReturnsCorrectLevels
│   - ProjectExtensions_ReturnsCorrectLevels
│   - CheckMatch_WithinTolerance_ReturnsTrue
│   - CheckMatch_OutsideTolerance_ReturnsFalse
│   - CheckMatch_ExactMatch_ZeroDeviation
│
├── PivotScannerTests.cs
│   - FindAbsoluteExtreme_FindsHighest
│   - FindAbsoluteExtreme_FindsLowest
│   - FindWave1_UptrendConfirmedByRetrace
│   - FindWave1_NoRetrace_ReturnsNull
│   - FindNearestFibonacciMatch_WithinTolerance
│   - FindNearestFibonacciMatch_OutsideTolerance_ReturnsNull
│   - FindNearestFibonacciMatch_MostExtremeFirst
│
├── CycleDetectorTests.cs
│   - DetectW0_AbsoluteLowFirst_Uptrend
│   - DetectW0_AbsoluteHighFirst_Downtrend
│   - DetectW1_FirstConfirmedSwing
│   - DetectW1_NoRetrace_InProgress
│   - DetectW2_BruteForce_MostExtremeFirst
│   - DetectW3_Extension_MostExtremeFirst
│   - DetectW3_NoMatch_InProgress
│   - DetectW4_WithAlternation
│   - DetectW4_OverlapsW1_MorphsDiagonal
│   - DetectW4_ViolatesW0_MorphsABC
│   - DetectW5_AlwaysInProgress_AtCycle
│   - RuleR1_W2ExceedsW1_Rejected
│   - RuleR2_W3Shortest_Rejected
│   - RuleR3_W4OverlapW1_DiagonalOrReject
│   - DirectionSymmetry_Downtrend_MirrorsUptrend
│
├── SubWaveDetectorTests.cs
│   - DetectMotiveSubWaves_WithinParentBounds
│   - DetectCorrectiveSubWaves_ABC
│   - SubWaves_CannotBeInProgress_WhenParentCompleted
│   - StartPoint_MatchesParentStart
│   - EndPoint_MatchesParentEnd
│
├── WaveLabelerTests.cs
│   - CreateImpulseWaves_CorrectLabels
│   - CreateCorrectiveWaves_CorrectLabels
│   - CreateInProgressWave_HasProjection
│   - Justification_AttachedToEachWave
```

### 12.2 Per-Asset Regression Tests (integration, real data)

```
├── RegressionTests/
│   ├── BtcRegressionTest.cs
│   ├── EthRegressionTest.cs
│   ├── SolRegressionTest.cs
│   ├── AdaRegressionTest.cs
│   ├── AvaxRegressionTest.cs
│   ├── DotRegressionTest.cs
│   ├── HbarRegressionTest.cs
│   ├── LinkRegressionTest.cs
│   ├── ManaRegressionTest.cs
│   ├── SuiRegressionTest.cs
│   └── XrpRegressionTest.cs
```

Each test:
1. Reads `C:\temp\chartdata_{symbol}.json`
2. Runs `analyzer.Analyze(data)`
3. Asserts each wave point's timestamp, price, label, degree, pattern type, and justification
4. Baseline values filled in after first successful run + manual verification

---

## 13. Data Flow Summary

```
Hourly OHLCV (input)
    │
    ├──► AggregateCandles → Daily OHLCV
    │       │
    │       └──► AggregateCandles → Monthly OHLCV
    │               │
    │               └──► CycleDetector.DetectCycleWaves(monthly)
    │                       │ Forward-projection: W0→W1→W2→W3→W4→W5(ip)
    │                       │ Pattern morphing: diagonal / A-B-C
    │                       ▼
    │                   List<ElliottWave> (cycle degree, monthly timestamps)
    │
    ├──► ResolveCycleTimestamps(cycleWaves, hourlyData)
    │       │ Monthly timestamps → exact hourly timestamps
    │       ▼
    │
    ├──► For each completed Cycle wave:
    │       │
    │       └──► SubWaveDetector.DetectSubWaves(dailyData, cycleWave, Primary)
    │               │ Same Fibonacci logic, bounded by parent
    │               ▼
    │           cycleWave.SubWaves = [Primary waves]
    │               │
    │               └──► For each completed Primary wave:
    │                       │
    │                       └──► SubWaveDetector.DetectSubWaves(hourlyData, primaryWave, Intermediate)
    │                               │
    │                               ▼
    │                           primaryWave.SubWaves = [Intermediate waves]
    │
    └──► ElliottWavesAnalysis { Waves = cycleWaves }
```

---

## 14. Implementation Order

### Phase 1: Foundation (no dependencies)
1. `FibonacciProjector` — pure math, testable in isolation
2. `PivotScanner` — candle scanning utilities
3. `FibonacciJustification` class on DTO
4. `Fibonacci` constants update
5. Unit tests for Phase 1

### Phase 2: Cycle Detection
6. `CycleDetector` — forward-projection engine
7. `WaveLabeler` (simplified rebuild)
8. Unit tests for Phase 2

### Phase 3: Orchestrator + Resolution
9. `ElliottWavesAnalyzer` (rewrite)
10. Timestamp resolution logic
11. Integration test: run against one asset (BTCUSDT), manually verify, establish baseline

### Phase 4: Lower Degrees
12. `SubWaveDetector` — wave-by-wave iteration
13. Integration test: verify sub-waves for BTCUSDT

### Phase 5: Full Regression Suite
14. Run all 11 assets
15. Manual verification of all cycle points
16. Snapshot baselines into per-asset regression tests
17. Delete old files (`PivotDetector.cs`, `WaveValidator.cs`, `WaveScorer.cs`, `IPivotDetector.cs`, `IWaveLabeler.cs`)
18. Verify visualization tests still pass

---

## 15. Key Design Decisions

| Decision | Rationale |
|----------|-----------|
| No backtracking | Greedy forward scan keeps complexity O(n) per degree. If W2 at 78.6% doesn't lead to valid W3, the algorithm reports W3 in-progress — it doesn't retry W2 at 61.8%. |
| Static classes for Projector/Scanner/Labeler | Pure functions with no state. Easier to test, no DI needed. |
| CycleDetector is a class (not static) | May need configuration in future (e.g., different asset classes). |
| SubWaveDetector reuses same Fibonacci tables | Same ratios apply at all degrees per Frost & Prechter. Only the data resolution changes. |
| Justification as separate class (not string) | Structured data enables programmatic validation in tests. |
| One regression test file per asset | Clear isolation. Failing test immediately identifies which asset broke. |
| No scoring system | The old system scored and compared patterns. The new system either finds a Fibonacci match or it doesn't. Binary, not scored. |

---

## 16. Success Criteria 

All Unit-Tests must be successful.


