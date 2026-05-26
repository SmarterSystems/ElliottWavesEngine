using System.Text.Json;
using SmarterSystems.ElliottWaves.Analyzer.Data;
using SmarterSystems.ElliottWaves.Analyzer.Analysis;
using SmarterSystems.ElliottWaves.Analyzer.Interfaces;

namespace SmarterSystems.ElliottWaves.Validation
{
    /// <summary>
    /// Diagnostic utilities for wave tree inspection and test code generation.
    /// Run these manually when needed. Output goes to the testoutput directory
    /// resolved by <see cref="TestDataPath.OutputPath"/>.
    /// </summary>
    [TestClass]
    public sealed class DiagnosticTest
    {
        private static readonly (string Symbol, string ShortName)[] AllTokens =
        [
            ("BTCUSDT", "Btc"), ("ETHUSDT", "Eth"), ("SOLUSDT", "Sol"),
            ("ADAUSDT", "Ada"), ("AVAXUSDT", "Avax"), ("LINKUSDT", "Link"),
            ("MANAUSDT", "Mana"), ("HBARUSDT", "Hbar"), ("DOTUSDT", "Dot"),
            ("XRPUSDT", "Xrp"), ("SUIUSDT", "Sui")
        ];

        // ══════════════════════════════════════════════════
        // Wave tree dump — one test per token
        // ══════════════════════════════════════════════════

        [TestMethod] public void DumpAdaWaveTree() => DumpWaveTreeForToken("ADAUSDT", "Ada");
        [TestMethod] public void DumpAvaxWaveTree() => DumpWaveTreeForToken("AVAXUSDT", "Avax");
        [TestMethod] public void DumpEthWaveTree() => DumpWaveTreeForToken("ETHUSDT", "Eth");
        [TestMethod] public void DumpHbarWaveTree() => DumpWaveTreeForToken("HBARUSDT", "Hbar");
        [TestMethod] public void DumpLinkWaveTree() => DumpWaveTreeForToken("LINKUSDT", "Link");
        [TestMethod] public void DumpSuiWaveTree() => DumpWaveTreeForToken("SUIUSDT", "Sui");
        [TestMethod] public void DumpBtcWaveTree() => DumpWaveTreeForToken("BTCUSDT", "Btc");
        [TestMethod] public void DumpSolWaveTree() => DumpWaveTreeForToken("SOLUSDT", "Sol");
        [TestMethod] public void DumpManaWaveTree() => DumpWaveTreeForToken("MANAUSDT", "Mana");
        [TestMethod] public void DumpDotWaveTree() => DumpWaveTreeForToken("DOTUSDT", "Dot");
        [TestMethod] public void DumpXrpWaveTree() => DumpWaveTreeForToken("XRPUSDT", "Xrp");

        private static void DumpWaveTreeForToken(string symbol, string shortName)
        {
            var data = LoadData(symbol);
            var analysis = new ElliottWavesAnalyzer().Analyze(data);
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"=== {shortName.ToUpper()} Wave Tree ===");
            DumpWaveTree(sb, analysis.Waves, 0);
            File.WriteAllText(TestDataPath.OutputFile($"{shortName.ToLower()}_wave_tree.txt"), sb.ToString());
        }

        // ══════════════════════════════════════════════════
        // Code generator: produces intermediate-degree test methods
        // ══════════════════════════════════════════════════

        [TestMethod]
        public void GenerateIntermediateRegressionTests()
        {
            foreach (var (symbol, shortName) in AllTokens)
            {
                var data = LoadData(symbol);
                var analysis = new ElliottWavesAnalyzer().Analyze(data);
                var sb = new System.Text.StringBuilder();

                for (int ci = 0; ci < analysis.Waves.Count; ci++)
                {
                    var cycle = analysis.Waves[ci];
                    if (cycle.SubWaves == null) continue;

                    string cyclePrefix = GetCyclePrefix(cycle.Label);

                    for (int pi = 0; pi < cycle.SubWaves.Count; pi++)
                    {
                        var primary = cycle.SubWaves[pi];
                        if (primary.SubWaves == null || primary.SubWaves.Count == 0) continue;

                        string pLabel = GetPrimaryShortLabel(primary.Label);
                        string testPrefix = $"Cycle{cyclePrefix}_P{pLabel}";
                        string contextPrefix = $"Cycle({cyclePrefix}) [{pLabel}]";
                        string accessor = $"_analysis.Waves[{ci}].SubWaves[{pi}]";

                        // Comment header
                        sb.AppendLine();
                        sb.AppendLine($"        // ══════════════════════════════════════════════════");
                        sb.AppendLine($"        // Intermediate Degree — inside {contextPrefix}");
                        sb.AppendLine($"        //");
                        foreach (var interm in primary.SubWaves)
                        {
                            string iLabel = FormatIntermLabel(interm.Label);
                            string progress = interm.IsInProgress ? " (in-progress)" : "";
                            sb.AppendLine($"        // ({iLabel}): ${FormatPrice(interm.StartPoint.Price)} → ${FormatPrice(interm.EndPoint.Price)} ({interm.EndPoint.Timestamp:dd.MM.yyyy}){progress}");
                        }
                        sb.AppendLine($"        // ══════════════════════════════════════════════════");
                        sb.AppendLine();

                        // WaveCount
                        sb.AppendLine($"        [TestMethod] public void {testPrefix}_Intermediate_WaveCount() => AssertSubWaveCount({accessor}, {primary.SubWaves.Count}, \"{contextPrefix} Intermediate\");");
                        sb.AppendLine();

                        for (int ii = 0; ii < primary.SubWaves.Count; ii++)
                        {
                            var interm = primary.SubWaves[ii];
                            string iLabel = FormatIntermLabel(interm.Label);
                            string iTestSuffix = FormatIntermTestSuffix(interm.Label);
                            string testName = $"{testPrefix}_{iTestSuffix}";
                            string context = $"{contextPrefix} ({iLabel})";
                            string iAccessor = $"{accessor}.SubWaves[{ii}]";
                            var wn = interm.Label.ToString();

                            sb.AppendLine($"        [TestMethod] public void {testName}_Label() => AssertLabel({iAccessor}, WaveNumber.{wn}, \"{context}\");");

                            if (interm.IsInProgress)
                            {
                                sb.AppendLine($"        [TestMethod] public void {testName}_InProgress() => AssertInProgress({iAccessor}, true, \"{context}\");");
                                sb.AppendLine($"        [TestMethod] public void {testName}_Start() => AssertPoint({iAccessor}.StartPoint, Utc({interm.StartPoint.Timestamp.Year}, {interm.StartPoint.Timestamp.Month}, {interm.StartPoint.Timestamp.Day}, {interm.StartPoint.Timestamp.Hour}, {interm.StartPoint.Timestamp.Minute}), {FormatPriceDecimal(interm.StartPoint.Price)}, \"{context} Start\");");
                            }
                            else
                            {
                                sb.AppendLine($"        [TestMethod] public void {testName}_Start() => AssertPoint({iAccessor}.StartPoint, Utc({interm.StartPoint.Timestamp.Year}, {interm.StartPoint.Timestamp.Month}, {interm.StartPoint.Timestamp.Day}, {interm.StartPoint.Timestamp.Hour}, {interm.StartPoint.Timestamp.Minute}), {FormatPriceDecimal(interm.StartPoint.Price)}, \"{context} Start\");");
                                sb.AppendLine($"        [TestMethod] public void {testName}_End() => AssertPoint({iAccessor}.EndPoint, Utc({interm.EndPoint.Timestamp.Year}, {interm.EndPoint.Timestamp.Month}, {interm.EndPoint.Timestamp.Day}, {interm.EndPoint.Timestamp.Hour}, {interm.EndPoint.Timestamp.Minute}), {FormatPriceDecimal(interm.EndPoint.Price)}, \"{context} End\");");
                            }

                            sb.AppendLine();
                        }
                    }
                }

                File.WriteAllText(TestDataPath.OutputFile($"intermediate_tests_{shortName}.txt"), sb.ToString());
            }
        }

        // ══════════════════════════════════════════════════
        // Helpers
        // ══════════════════════════════════════════════════

        private static List<Ohlcv> LoadData(string symbol)
        {
            var path = TestDataPath.ChartDataFile(symbol);
            if (!File.Exists(path))
            {
                Assert.Inconclusive(
                    $"Test data file not found: {path}\n" +
                    "Run scripts/fetch-test-data.ps1 to download OHLCV data from Binance.");
            }
            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<List<Ohlcv>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })!;
        }

        private static void DumpWaveTree(System.Text.StringBuilder sb, List<ElliottWave> waves, int depth)
        {
            if (waves == null) return;
            foreach (var w in waves)
            {
                var indent = new string(' ', depth * 2);
                var proj = w.Projection != null
                    ? $" Proj=[{string.Join(", ", w.Projection.Targets.Select(t => $"{t.FibonacciLevel}={t.Price:F2}"))}]"
                    : "";
                sb.AppendLine($"{indent}{w.Degree} {w.Label}: ${w.StartPoint.Price:F2}@{w.StartPoint.Timestamp:yyyy-MM-dd} → ${w.EndPoint.Price:F2}@{w.EndPoint.Timestamp:yyyy-MM-dd} InProg={w.IsInProgress}{proj}");
                if (w.SubWaves != null && w.SubWaves.Count > 0)
                    DumpWaveTree(sb, w.SubWaves, depth + 1);
            }
        }

        private static string GetCyclePrefix(WaveNumber label) => label switch
        {
            WaveNumber.One => "I",
            WaveNumber.Two => "II",
            WaveNumber.Three => "III",
            WaveNumber.Four => "IV",
            WaveNumber.Five => "V",
            WaveNumber.A => "A",
            WaveNumber.B => "B",
            WaveNumber.C => "C",
            _ => label.ToString()
        };

        private static string GetPrimaryShortLabel(WaveNumber label) => label switch
        {
            WaveNumber.One => "1",
            WaveNumber.Two => "2",
            WaveNumber.Three => "3",
            WaveNumber.Four => "4",
            WaveNumber.Five => "5",
            WaveNumber.A => "A",
            WaveNumber.B => "B",
            WaveNumber.C => "C",
            _ => label.ToString()
        };

        private static string FormatIntermLabel(WaveNumber label) => label switch
        {
            WaveNumber.One => "i",
            WaveNumber.Two => "ii",
            WaveNumber.Three => "iii",
            WaveNumber.Four => "iv",
            WaveNumber.Five => "v",
            WaveNumber.A => "a",
            WaveNumber.B => "b",
            WaveNumber.C => "c",
            _ => label.ToString().ToLower()
        };

        private static string FormatIntermTestSuffix(WaveNumber label) => label switch
        {
            WaveNumber.One => "i1",
            WaveNumber.Two => "i2",
            WaveNumber.Three => "i3",
            WaveNumber.Four => "i4",
            WaveNumber.Five => "i5",
            WaveNumber.A => "iA",
            WaveNumber.B => "iB",
            WaveNumber.C => "iC",
            _ => "i" + label.ToString()
        };

        private static string FormatPrice(decimal price)
        {
            if (price >= 10000m) return price.ToString("N0");
            if (price >= 100m) return price.ToString("N2");
            if (price >= 1m) return price.ToString("N4");
            if (price >= 0.01m) return price.ToString("N5");
            return price.ToString("G");
        }

        private static string FormatPriceDecimal(decimal price)
        {
            string raw = price.ToString("G");
            if (raw.Contains('.'))
                raw = raw.TrimEnd('0').TrimEnd('.');
            return raw + "m";
        }
    }
}
