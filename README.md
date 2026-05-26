# SmarterSystems.ElliottWaves

[![CI](https://github.com/SmarterSystems/ElliottWavesEngine/actions/workflows/ci.yml/badge.svg)](https://github.com/SmarterSystems/ElliottWavesEngine/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/SmarterSystems.ElliottWavesEngine.svg)](https://www.nuget.org/packages/SmarterSystems.ElliottWavesEngine/)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

A forward-projection Elliott Wave detection engine for OHLCV time-series data, written in C# / .NET 10.

The library detects Cycle-, Primary-, and Intermediate-degree waves on candlestick data by projecting Fibonacci retracement and extension levels forward from confirmed pivots and accepting the most extreme actual price that lands within tolerance. Pattern morphing (impulse, diagonal, A-B-C correction) and Frost-and-Prechter inviolable rules (R1 through R5) are enforced structurally rather than as post-hoc scores.

It includes a `ChartRenderer` that produces TradingView-styled PNG charts annotated with wave labels, Fibonacci projection targets, and invalidation levels.

The library has been battle-tested in production on the SmarterSystems CryptoWealth trading platform across 11 instruments (BTC, ETH, SOL, AVAX, ADA, LINK, MANA, HBAR, DOT, XRP, SUI).

## Install

```bash
dotnet add package SmarterSystems.ElliottWavesEngine
```

## Quick start

```csharp
using SmarterSystems.ElliottWaves.Analyzer.Analysis;
using SmarterSystems.ElliottWaves.Analyzer.Data;
using SmarterSystems.ElliottWaves.Analyzer.Render;

// Load hourly OHLCV candles however you prefer (JSON, exchange API, database).
List<Ohlcv> hourlyData = LoadCandles();

// Run the analyzer.
var analyzer = new ElliottWavesAnalyzer();
var analysis = analyzer.Analyze(hourlyData);

// Walk the detected wave structure.
foreach (var cycleWave in analysis.Waves)
{
    Console.WriteLine($"{cycleWave.Degree} {cycleWave.Label}: " +
        $"${cycleWave.StartPoint.Price} @ {cycleWave.StartPoint.Timestamp:yyyy-MM-dd} -> " +
        $"${cycleWave.EndPoint.Price} @ {cycleWave.EndPoint.Timestamp:yyyy-MM-dd} " +
        $"(in-progress: {cycleWave.IsInProgress})");

    if (cycleWave.Projection != null)
    {
        foreach (var target in cycleWave.Projection.Targets)
            Console.WriteLine($"   target: ${target.Price} (Fib {target.FibonacciLevel:P1}, p={target.Probability:P0})");
    }

    foreach (var primary in cycleWave.SubWaves ?? new())
        Console.WriteLine($"   {primary.Degree} {primary.Label}: {primary.PatternType}");
}

// Render to PNG.
byte[] png = ChartRenderer.RenderChart(
    symbol: "BTCUSDT",
    analysis: analysis,
    candleData: hourlyData,
    timeframe: CandleTimeframe.Daily);
File.WriteAllBytes("btc-analysis.png", png);
```

## How it works

The detection engine is a forward-projection model rather than a pivot-scoring model. The algorithm:

1. **Wave 0** is the absolute extreme of the input data, chronologically determining trend direction.
2. **Wave 1** is confirmed when price retraces more than fifty percent of the move from Wave 0 to the running extreme.
3. **Wave 2** is the most extreme countertrend pivot that lands within tolerance of one of the Fibonacci retracement levels `[0.786, 0.618, 0.5, 0.382]` projected from Wave 1, tried in most-extreme-first order.
4. **Wave 3** is the most extreme trend-direction pivot within tolerance of the Fibonacci extension levels `[4.236, 2.618, 1.618, 1.0]` of Wave 1 projected from Wave 2, with `0.382` retracement confirmation.
5. **Wave 4** is the retracement of Wave 3 using levels `[0.786, 0.618, 0.5, 0.382, 0.236]`, with the alternation guideline influencing preference.
6. **Wave 5** at Cycle degree is always reported as in-progress with projection targets.

Rule violations trigger morphing rather than rejection:

- Wave 2 retracing beyond Wave 0 morphs the structure to A-B-C.
- Wave 4 overlapping Wave 1 marks the structure as a diagonal.

Below Cycle degree the same algorithm runs wave-by-wave within parent boundaries. Sub-waves cannot be in-progress when the parent is completed.

All Fibonacci math runs in natural-log space to handle the exponential price range of crypto markets cleanly. Tolerances are specified in log-scale, defaulting to two percent.

For the full specification, see [docs/specification.md](docs/specification.md). For the architecture rationale and pseudocode, see [docs/design.md](docs/design.md). For the Frost-and-Prechter rule reference distilled from the source material, see [docs/elliott-wave-reference.md](docs/elliott-wave-reference.md).

## Repository layout

```
SmarterSystems.ElliottWaves.Analyzer/    Class library shipped to NuGet
  Analysis/                              Detection engine
  Interfaces/                            Public DTOs and Fibonacci constants
  Data/                                  Ohlcv DTO
  Render/                                ScottPlot-based chart renderer
SmarterSystems.ElliottWaves.Validation/  MSTest regression suite (one per instrument)
docs/                                    Specification, design, Elliott Wave reference
scripts/                                 fetch-test-data.ps1
```

## Building and running tests

The library requires the .NET 10 SDK.

```bash
dotnet restore
dotnet build SmarterSystems.ElliottWaves.sln
dotnet test SmarterSystems.ElliottWaves.Validation/SmarterSystems.ElliottWaves.Validation.csproj
```

## Test data

The regression tests assert exact wave timestamps and prices against snapshot baselines for each of the 11 instruments. The OHLCV input data is not committed to the repository. Fetch it with the included script:

```powershell
pwsh ./scripts/fetch-test-data.ps1
```

The script downloads roughly seven years of hourly candles per instrument from the Binance public API into `./testdata/`. No API key is required. Total download size is approximately one hundred megabytes.

If a test data file is missing when a test runs, the test reports `Inconclusive` rather than failing. This keeps `dotnet test` runnable on a fresh clone without internet access.

Override the data location via the `ELLIOTTWAVES_TESTDATA_PATH` environment variable if needed.

## Versioning

Semantic versioning. The public DTO contract on `ElliottWavesAnalysis`, `ElliottWave`, `Projection`, `Target`, and `PivotPoint` is treated as a stability contract from `1.0.0` onward. Detection-engine internal changes that move wave-detection results trigger a minor version bump, with the regression snapshots updated in the same commit.

## Contributing

Issues and pull requests welcome. Please open an issue before submitting larger changes so the design fit can be discussed first. A pull request must keep `dotnet build` warning-free and must not regress any passing regression test for which test data was downloaded.

## License

MIT. See [LICENSE](LICENSE).

Copyright 2026 Benjamin Lutz / Smarter Systems.

## Acknowledgements

- The Elliott Wave Principle as documented in Frost and Prechter, *Elliott Wave Principle: Key to Market Behavior*. This implementation is a structural interpretation of the published rules and guidelines; the book remains the governing authority.
- [ScottPlot](https://scottplot.net/) for the rendering backend.
- [Binance](https://binance.com/) public klines API for the regression test data.
