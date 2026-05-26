# CLAUDE.md

Guidance for Claude Code and other AI coding agents working on this repository.

## What this project is

A forward-projection Elliott Wave detection engine for OHLCV time-series data. The analyzer is a pure C# class library with no external state. The validation project is an MSTest test suite covering 11 instruments.

## Build and test

```bash
dotnet restore
dotnet build SmarterSystems.ElliottWaves.sln
dotnet test SmarterSystems.ElliottWaves.Validation/SmarterSystems.ElliottWaves.Validation.csproj
```

Regression tests need OHLCV data on disk. Without data they skip with `Assert.Inconclusive`, never fail. To populate the data, run from the repo root:

```powershell
pwsh scripts/fetch-test-data.ps1
```

The script downloads hourly OHLCV for 11 instruments from the Binance public API into `./testdata/`. No API key required. Override the destination via the `ELLIOTTWAVES_TESTDATA_PATH` environment variable if needed.

## Project layout

- `SmarterSystems.ElliottWaves.Analyzer/` — the library that ships to NuGet
  - `Analysis/` — detection engine (orchestrator, cycle detector, sub-wave detector, pivot scanner, Fibonacci projector, wave labeler, wave math)
  - `Interfaces/` — public DTOs (ElliottWavesAnalysis, ElliottWave, PivotPoint, Projection, Target, enums) and Fibonacci constants
  - `Data/` — the self-contained `Ohlcv` candle DTO
  - `Render/` — `ChartRenderer` static class (depends on ScottPlot, MIT-licensed)
- `SmarterSystems.ElliottWaves.Validation/` — MSTest regression suite
  - `RegressionTestBase.cs` — base class with shared assertion helpers
  - `{Symbol}RegressionTest.cs` — one per instrument
  - `TestDataPath.cs` — resolves the test data and output directories
  - `DiagnosticTest.cs`, `PriceFinder.cs` — manual tools for inspection and test scaffolding
- `docs/` — `design.md`, `specification.md`, `elliott-wave-reference.md`

## References

- Algorithm specification: [docs/specification.md](docs/specification.md)
- Architecture design: [docs/design.md](docs/design.md)
- Elliott Wave Principle rule reference (Frost & Prechter distilled): [docs/elliott-wave-reference.md](docs/elliott-wave-reference.md)

## Conventions

- All price math runs in log scale (natural log in the analyzer, log10 in some test helpers, both yield equivalent Fibonacci ratios).
- Aggregation chain is `hourly -> daily -> monthly`. The analyzer detects Cycle-degree waves on monthly data and resolves pivot timestamps back down to hourly resolution.
- `decimal` is used throughout for price representation. Do not introduce `double` for prices.
- The public DTO contract on `ElliottWavesAnalysis`, `ElliottWave`, `Projection`, `Target`, and `PivotPoint` is frozen. New fields require a versioned discussion.

## When making changes

1. Match the existing detection style. Forward-projection, brute-force most-extreme-first, greedy commits.
2. Quality bar: zero build warnings, zero build errors.
3. Run the full validation suite when test data is present. When not, at minimum confirm `dotnet build` is warning-free.
4. Do not add new external dependencies without explicit discussion. The Analyzer's only runtime dependency is ScottPlot.
