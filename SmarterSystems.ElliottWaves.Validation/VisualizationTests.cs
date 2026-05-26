using SmarterSystems.ElliottWaves.Analyzer.Data;
using SmarterSystems.ElliottWaves.Analyzer.Analysis;
using SmarterSystems.ElliottWaves.Analyzer.Render;
using System.Text.Json;

namespace SmarterSystems.ElliottWaves.Validation
{
    [TestClass]
    public sealed class VisualizationTests
    {
        [DataRow("ADAUSDT")]
        [DataRow("AVAXUSDT")]
        [DataRow("BTCUSDT")]
        [DataRow("DOTUSDT")]
        [DataRow("ETHUSDT")]
        [DataRow("HBARUSDT")]
        [DataRow("LINKUSDT")]
        [DataRow("MANAUSDT")]
        [DataRow("SOLUSDT")]
        [DataRow("SUIUSDT")]
        [DataRow("XRPUSDT")]
        [TestMethod]
        public void TestVisualizationHourly(string symbol)
        {
            var dataFile = TestDataPath.ChartDataFile(symbol);
            if (!File.Exists(dataFile))
            {
                Assert.Inconclusive(
                    $"Test data file not found: {dataFile}\n" +
                    "Run scripts/fetch-test-data.ps1 to download OHLCV data from Binance.");
            }

            var analyzer = new ElliottWavesAnalyzer();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var chartData = JsonSerializer.Deserialize<List<Ohlcv>>(File.ReadAllText(dataFile), options);
            Assert.IsNotNull(chartData, $"Failed to deserialize chart data from {dataFile}");
            var analysis = analyzer.Analyze(chartData);
            var hourlyData = chartData.ToList();
            var image = ChartRenderer.RenderChart(symbol, analysis, hourlyData, CandleTimeframe.Hourly);
            File.WriteAllBytes(TestDataPath.OutputFile($"{symbol}_chart.png"), image);
        }
    }
}
