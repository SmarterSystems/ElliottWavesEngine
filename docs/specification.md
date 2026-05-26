# Elliott Wave Analyzer — Fibonacci-Driven Detection Rebuild

## Requirements Specification

### Governing Authority

**"Elliott Wave Principle: Key to Market Behavior" by Frost & Prechter is the single source of truth.** Every rule, guideline, ratio, pattern classification, and structural expectation in this specification must be traceable to the book. No requirement in this document may contradict or extend beyond what the book defines. If any requirement below conflicts with the Elliott Wave Principle, the book takes precedence and the requirement must be corrected.

This applies to:
- **Inviolable rules** (R1-R5): Must be enforced without exception
- **Guidelines** (alternation, channeling, depth of corrections, wave equality, volume, wave personality): Must be respected as strong tendencies — not enforced as hard constraints, but used to prefer interpretations that satisfy more guidelines
- **Pattern structures**: Internal wave counts (5-3-5-3-5 for impulse, 3-3-3-3-3 for ending diagonal, etc.) must match the book exactly
- **Fibonacci ratios**: Only ratios documented in the book (§7) shall be used for projection and validation
- **Wave degree relationships**: Fractal nesting, degree labeling, and the relationship between parent and child waves must follow the book's definitions
- **Pattern morphing**: The conditions under which a count changes (impulse → diagonal, impulse → corrective) must follow the book's rules, not custom heuristics

When implementation decisions arise that the book does not explicitly address (e.g., tolerance bands, brute-force ordering), these are engineering choices that must not violate any rule or guideline. They are documented as such below.

### Problem Statement

The current wave detection algorithm finds pivot points first, then scores them against Fibonacci levels after the fact. Fibonacci is decorative, not structural. The algorithm assumes points rather than using Fibonacci projections to discover where waves should end.

### Core Principle

**Forward-projection model**: Confirm early waves → project next wave endpoint via Fibonacci → find actual pivot nearest projection within tolerance → confirm or reject → proceed to next wave.

### Detection Flow (Cycle Degree)

```
Find W0 (absolute extreme)
  → Confirm W1 (first swing with >50% retrace)
    → Brute-force W2 from Fibonacci retracements (most extreme first)
      → Project W3 from Fibonacci extensions (most extreme first)
        → Project W4 from Fibonacci retracements of W3 (with alternation)
          → W5 is always in-progress at cycle degree (by definition)
```

---

## Functional Requirements

### FR1: Wave 0 — Starting Point

- Find the absolute highest and lowest point across the entire monthly dataset
- Whichever comes first chronologically is W0
- If absolute low comes first → uptrend impulse assumed
- If absolute high comes first → downtrend impulse assumed

### FR2: Wave 1 — First Confirmed Swing

- **Uptrend**: Scan forward from W0 (absolute low). Track running highs. When a subsequent candle retraces >50% from a running high back toward W0, the last absolute high before the retrace point is W1.
- **Downtrend**: Inverted — scan for running lows, confirm when >50% retrace upward occurs. W1 is the last absolute low before the retrace point.
- W1 is the price extreme, not the retrace point.

### FR3: Wave 2 — Fibonacci Retracement Brute-Force

- Project retracement levels of W1 (measured from W0 to W1): **78.6%, 61.8%, 50%, 38.2%**
- Try most extreme level first (78.6%)
- Per §7.1: Wave 2 typically retraces 50%, 61.8%, or 78.6% of Wave 1. 38.2% included as minimum valid depth.
- Note: 88.6% is NOT a Frost & Prechter ratio and is excluded per Governing Authority.
- Find the pivot closest to the projected level within **2% tolerance** (log-scale)
- Accept greedily — first match is taken, no backtracking
- The pivot must be the most extreme countertrend point near that level

**Justification output**: Record which Fibonacci level was matched and the actual deviation percentage.

### FR4: Wave 3 — Fibonacci Extension Brute-Force

- Project extension levels of W1 (measured from W2 end): **4.236, 2.618, 1.618, 1.0** × W1 length
- Try most extreme level first (4.236)
- Per §7.2: Wave 3 is often 1.618 × W1 (or 2.618 × W1 if strongly extended). §7 key ratios table documents 4.236 as extreme extension. 3.618 and 4.618 are NOT Frost & Prechter ratios and are excluded per Governing Authority.
- Scan forward from W2 for pivot closest to projected level within **2% tolerance**
- Accept greedily — first match is taken
- If no pivot matches any projection → **Wave 3 is in progress**; stop here, output target levels
- W3 must exceed W1 end (inviolable rule R2/R3 still apply)

**Justification output**: Record which extension level was matched and the actual deviation.

### FR5: Wave 4 — Fibonacci Retracement with Alternation

- Project retracement levels of W3 (measured from W2 end to W3 end): **78.6%, 61.8%, 50%, 38.2%, 23.6%**
- Per §7.1: Wave 4 typically retraces 23.6%, 38.2%, or 50% of Wave 3. Deeper levels included for diagonal detection.
- Try most extreme first to detect potential diagonal morphing
- **Alternation guideline (§5.1)** influences expectation:
  - If W2 was sharp (zigzag) → expect W4 sideways (flat/triangle), typically shallow (23.6%, 38.2%)
  - If W2 was sideways (flat/triangle) → expect W4 sharp (zigzag), typically deeper (61.8%, 78.6%)
  - Per §5.1: "If wave 2 is a sharp correction, expect wave 4 to be sideways, and vice versa"
  - Alternation also applies to time: sharp = fewer candles, sideways = more candles
- Still brute-force most extreme first, but alternation affects scoring/preference
- **Diagonal detection**: If W4 overlaps W1 price territory → morph to diagonal (not rejection)
- Inviolable: W4 must not violate W0 (otherwise → A-B-C morph, see FR7)

**Justification output**: Record Fibonacci level, deviation, and alternation assessment.

### FR6: Wave 5 — Always In-Progress at Cycle Degree

- By definition, W5 cannot be finished at cycle degree — if it were, a lesser degree should have been chosen
- Project W5 targets from W4 end per §7.2:
  - W5 = W1 (equality, most common for non-extended W5)
  - W5 = 0.618 × W1
  - W5 = 1.618 × W1 (extended W5)
  - W5 related to W1-W3 net distance by 0.618 or 1.0 (§7.2)
- Output projections as targets, not confirmed points
- If any inviolable rules fail during detection → the cycle may only have confirmed waves up to W1, W1-W2, W1-W2-W3, or W1-W2-W3-W4, with projection for the next wave

### FR7: Pattern Morphing

- **Start optimistic**: Always assume 1-2-3-4-5 impulse initially
- **Morph to A-B-C**: If price violates W0 (goes below W0 in uptrend, above W0 in downtrend) → entire structure reclassifies as corrective A-B-C
  - At cycle degree, do NOT attempt to classify correction sub-type (zigzag, flat, etc.)
  - Just identify A, B, C points
- **Morph to diagonal**: If W4 overlaps W1 but does NOT violate W0 → stays impulse, becomes diagonal
- No other morph triggers

### FR8: Cycle Point Resolution to Hourly Data

- After cycle waves are identified on monthly data, each confirmed cycle point must be resolved to the exact price and timestamp on the hourly OHLCV data
- Use the price and date range of the monthly candle to find the exact hourly candle that contains the extreme (high/low matching the cycle point)
- This is required because:
  - Visualization is always on hourly data
  - Lower-degree detection needs exact start/end points from the higher degree

### FR9: Lower Degree Detection (Primary, Intermediate)

- Lower-degree detection is **wave-by-wave iteration** through the higher degree's confirmed points
- For each higher-degree wave segment (e.g., cycle W0→W1, W1→W2, etc.):
  - **Start point**: Exactly the start point of the higher-degree wave (not re-detected)
  - **End point**: Exactly the end point of the higher-degree wave (not re-detected)
  - **Interior points**: Detected using the same Fibonacci brute-force logic
  - **Waves cannot be in-progress** when the higher degree wave is completed — all sub-waves must terminate within the parent's boundaries
- Primary degree: uses daily candlestick data, bounded by cycle wave start/end
- Intermediate degree: uses hourly candlestick data, bounded by primary wave start/end
- **Projection clustering** (§7.4): At lower degrees, multiple Fibonacci relationships from different reference waves may converge on the same price level. These clusters strengthen confidence in a target.
- The same forward-projection algorithm applies: W0→W1→project W2→project W3→etc., but constrained by the parent wave's exact boundaries

### FR10: Corrective Wave Detection (W2, W4, B waves)

- Project retracement levels from the prior impulse wave
- Find pivot nearest to Fibonacci levels within 2% tolerance
- Classify correction type based on which level it hits and internal structure
- Same forward-projection model applies

### FR11: Inviolable Rules (must never be violated)

- **R1**: Wave 2 never retraces more than 100% of Wave 1
- **R2**: Wave 3 is never the shortest of waves 1, 3, 5
- **R3**: Wave 4 does not enter Wave 1 price territory (exception: diagonals)
- **R4**: Corrections are never fives
- **R5**: Motive waves always subdivide into 5 waves
- All rules checked on log-scale for cycle/primary degree

### FR12: Direction Symmetry

- Downtrend detection is a perfect mirror of uptrend
- Same Fibonacci levels, same tolerance (2%), same brute-force order
- All projections inverted in direction

### FR13: Fibonacci Justification (DTO Amendment)

- Every detected wave point must include justification data:
  - Which Fibonacci level it was detected at (e.g., "W2 = 61.8% retrace of W1")
  - The actual deviation from the exact Fibonacci level (e.g., "0.8% deviation")
  - The reference wave used for projection (e.g., "retrace of W0→W1" or "extension of W1 from W2")
- This is the **only allowed amendment** to the DTOs — a justification property on `ElliottWave`
- The Tester project's console output should display justification for each detected wave point

### FR14: In-Progress Handling

- **(a) Cycle degree**: Stop at last confirmed wave. Output projection targets for next wave. Do not attempt partial detection beyond confirmed waves. If W1 cannot be confirmed (no >50% retrace found), W1 is in-progress.
- **(b) Lower degrees (Primary, Intermediate)**: Detect as many waves as possible within the parent wave's boundaries. Mark unfinished waves as in-progress with projected targets. However, if the parent wave is completed (has a confirmed end point from higher degree), all sub-waves must be confirmed — none can be in-progress.

### FR15: Wave 2 Extreme Point Selection

- Always use the most extreme countertrend point when selecting W2/W4 pivots
- Most waves are not allowed to exceed their starting point — if a less-extreme point were chosen as W2, subsequent price action could invalidate the lesser-degree count
- **Exception**: B-waves may exceed the start of the correction
- This is not a preference but a structural necessity — choosing non-extreme points creates invalid counts at lesser degrees by definition

---

## Non-Functional Requirements

### NFR1: Tolerance

- 2% deviation maximum from projected Fibonacci level (log-scale)
- Applied uniformly across all wave degrees

### NFR2: Price Scale

- All price comparisons and Fibonacci calculations on log-scale (natural log)
- Consistent with existing `WaveMath` implementation

### NFR3: Data Aggregation

- Cycle degree: monthly candlestick data (aggregated from hourly input)
- Primary degree: daily candlestick data
- Intermediate degree: hourly candlestick data
- Same as current implementation

### NFR4: DTO Contract (Set in Stone)

The following DTOs are the contract with Visualization and are frozen:
- `ElliottWavesAnalysis`, `ElliottWave`, `Projection`, `Target`, `PivotPoint`
- All enums: `WaveDegree`, `WaveNumber`, `PatternType`, `PatternSubType`, `PointType`
- **Only allowed amendment**: Fibonacci justification property on `ElliottWave` (see FR13)
- No other fields added, removed, or changed on any of these classes

Internal interfaces (`IPivotDetector`, `IWaveLabeler`) and constants (`Fibonacci`, `Thresholds`, `Scores`) are free to rewrite, replace, or delete.

Tester project continues to work with same input/output paths.

### NFR5: Performance

- Must handle 5+ years of hourly data for crypto instruments
- Greedy forward scan (no backtracking) keeps complexity linear per degree

---

## Validation & Testing

### Test Project

- **Project**: `SmarterSystems.ElliottWaves.Validation` (MSTest, .NET 10)
- **No other validation tools allowed** — no Python, no PowerShell, no external scripts
- All validation is via C# unit and integration tests
- **If unit tests + integration tests are green → solution works**

### Per-Asset Regression Tests (Integration)

One test method per asset. Each test asserts **exact timestamp and price** for every detected wave point at every degree. These are the regression safety net — any logic change that shifts a wave point will break the corresponding test.

**Assets (11 tests):**
- `ADAUSDT`, `AVAXUSDT`, `BTCUSDT`, `DOTUSDT`, `ETHUSDT`
- `HBARUSDT`, `LINKUSDT`, `MANAUSDT`, `SOLUSDT`, `SUIUSDT`, `XRPUSDT`

**Each test asserts per wave point:**
- `WaveNumber` (label: One, Two, Three, etc.)
- `WaveDegree` (Cycle, Primary, Intermediate)
- `StartPoint.Timestamp` (exact DateTime)
- `StartPoint.Price` (exact decimal)
- `EndPoint.Timestamp` (exact DateTime)
- `EndPoint.Price` (exact decimal)
- `IsInProgress` (true/false)
- `PatternType` and `PatternSubType`
- `Fibonacci justification` (which level, deviation)

**Test data**: Reads from `C:\temp\chartdata_{symbol}.json` (hourly OHLCV)

**Baseline workflow:**
1. Run analyzer against all 11 symbols after rebuild
2. Manually verify cycle points are correct (human confirms)
3. Snapshot confirmed values as expected constants in test classes
4. All future runs assert against this baseline
5. If a logic change intentionally moves a point → update the baseline deliberately

### Structural Unit Tests

Pure logic tests with synthetic price data (no files needed). Validate:
- Forward-projection algorithm mechanics (W0→W1→W2→W3→W4 detection)
- Fibonacci level brute-force ordering (most extreme first)
- 2% tolerance boundary enforcement
- Inviolable rules (R1-R5) rejection
- Pattern morphing triggers (impulse → diagonal, impulse → A-B-C)
- In-progress handling (cycle vs lower degree behavior)
- Alternation guideline application
- Log-scale calculation correctness
- Direction symmetry (uptrend/downtrend mirror)

### Existing Visualization Test

- `VisualizationTests.TestVisualization(symbol)` remains unchanged
- Validates rendering still works after rebuild

---

## Acceptance Criteria

1. All 11 per-asset regression tests pass with exact timestamp/price matches
2. All structural unit tests pass
3. Visualization tests pass (rendering unchanged)
4. If no Fibonacci-matching pivot exists for a projected wave, that wave is marked in-progress with target levels
5. If price violates W0, the structure morphs to A-B-C without crash or invalid output
6. If W4 overlaps W1, the structure morphs to diagonal
7. W5 at cycle degree is always marked as in-progress with projection targets
8. Lower-degree detection iterates wave-by-wave through parent boundaries
9. All inviolable rules (R1-R5) are enforced — no violating counts produced
10. Output DTOs match frozen contract (only justification amendment)

---

## Resolved Questions

1. **W1 confirmation threshold**: Fixed at >50% retrace. Not configurable.
2. **Wave point selection**: Always use the most extreme point. Not a scoring preference — structural necessity to avoid invalidating lesser-degree counts. B-waves are the only exception.
3. **Diagonal internal structure**: Yes, enforce 3-3-3-3-3 (ending diagonal) vs 5-3-5-3-5 (leading diagonal) at sub-wave detection.
4. **Insufficient data**: If no >50% retrace is found for W1 confirmation, W1 is still in-progress. The algorithm simply reports fewer confirmed waves.

---

## Next Steps

- `/sc:design` — Architecture design for the rebuilt detection engine
- `/sc:workflow` — Implementation plan with task breakdown
