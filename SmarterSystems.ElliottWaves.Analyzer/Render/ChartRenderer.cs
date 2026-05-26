using ScottPlot;
using SmarterSystems.ElliottWaves.Analyzer.Data;
using SmarterSystems.ElliottWaves.Analyzer.Analysis;
using SmarterSystems.ElliottWaves.Analyzer.Interfaces;

namespace SmarterSystems.ElliottWaves.Analyzer.Render
{
    /// <summary>
    /// Candlestick timeframe for chart rendering. Determines which candles are displayed
    /// and which wave degrees are shown as labels.
    /// </summary>
    public enum CandleTimeframe
    {
        /// <summary>Monthly candles — shows Cycle labels only.</summary>
        Monthly,
        /// <summary>Daily candles — shows Cycle + Primary labels.</summary>
        Daily,
        /// <summary>Hourly candles — shows Cycle + Primary + Intermediate labels.</summary>
        Hourly
    }

    /// <summary>
    /// Renders Elliott Wave analysis as a professional candlestick chart with wave labels,
    /// Fibonacci projections, and invalidation levels. Uses ScottPlot 5 for PNG output.
    ///
    /// Y-axis uses log10 scale to handle crypto's exponential price range.
    /// All prices are transformed via Math.Log10() before rendering, and tick labels
    /// display actual prices via Math.Pow(10, y).
    ///
    /// Supports three candle timeframes (Monthly, Daily, Hourly), each showing the
    /// appropriate wave degrees for that resolution.
    /// </summary>
    public static class ChartRenderer
    {
        /// <summary>Standard 1-2-5 tick sequence for clean log-scale axis spacing.</summary>
        private static readonly decimal[] TickBases = [1, 2, 5];

        // ───────────────────────── Color Scheme (TradingView Dark) ─────────────────────────

        private static readonly ScottPlot.Color BgColor = ScottPlot.Color.FromHex("#131722");
        private static readonly ScottPlot.Color DataBgColor = ScottPlot.Color.FromHex("#1E222D");
        private static readonly ScottPlot.Color GridColor = ScottPlot.Color.FromHex("#363A45");
        private static readonly ScottPlot.Color AxisColor = ScottPlot.Color.FromHex("#D1D4DC");
        private static readonly ScottPlot.Color CandleUp = ScottPlot.Color.FromHex("#26A69A");
        private static readonly ScottPlot.Color CandleDown = ScottPlot.Color.FromHex("#EF5350");
        private static readonly ScottPlot.Color CycleColor = ScottPlot.Color.FromHex("#FFD700");
        private static readonly ScottPlot.Color PrimaryColor = ScottPlot.Color.FromHex("#00BFFF");
        private static readonly ScottPlot.Color IntermediateColor = ScottPlot.Color.FromHex("#C0C0C0");
        private static readonly ScottPlot.Color InvalidationColor = ScottPlot.Color.FromHex("#EF5350");

        // ───────────────────────── Font Sizes ─────────────────────────

        private const float CycleFontSize = 16f;
        private const float PrimaryFontSize = 13f;
        private const float IntermediateFontSize = 10f;
        private const float ProjectionFontSize = 10f;
        private const float TickFontSize = 10f;
        private const float TitleFontSize = 16f;

        // ───────────────────────── Label Spacing ─────────────────────────

        private const int BaseOffsetPx = 10;   // Gap between candle wick and first label
        private const int LabelStepPx = 20;    // Vertical spacing per label tier

        // ───────────────────────── Label Data ─────────────────────────

        private record LabelInfo(
            WaveDegree Degree,
            string Text,
            bool IsHigh,
            DateTime Timestamp,
            decimal Price);

        // ───────────────────────── Public Entry Point ─────────────────────────

        /// <summary>
        /// Renders the analysis as a professional candlestick chart with Elliott Wave annotations.
        /// The timeframe determines which candles are drawn and which wave degree labels are shown:
        ///   Monthly → Cycle labels only (compact overview)
        ///   Daily   → Cycle + Primary + Intermediate labels (Intermediate shown when parent span permits)
        ///   Hourly  → Cycle + Primary + Intermediate labels (full detail)
        /// Y-axis is log10-scaled for crypto's exponential price range.
        /// </summary>
        public static byte[] RenderChart(
            string symbol,
            ElliottWavesAnalysis analysis,
            List<Ohlcv> candleData,
            CandleTimeframe timeframe,
            int? width = null,
            int? height = null)
        {
            // Aggregate hourly candles to the requested timeframe
            var displayCandles = AggregateToTimeframe(candleData, timeframe);

            // Adaptive image dimensions based on candle count
            int w = width ?? DefaultWidth;
            int h = height ?? DefaultHeight;

            var plot = new Plot();

            ApplyTheme(plot);
            AddCandlesticks(plot, displayCandles, timeframe);

            var labels = CollectLabels(analysis, timeframe);
            RenderLabels(plot, labels, displayCandles);

            // Setup y-axis BEFORE projections so the range check works correctly.
            var labelPrices = labels.Select(l => l.Price).ToList();
            SetupLogYAxis(plot, displayCandles, labelPrices);

            // Set explicit x-axis range so edge candles are not clipped by auto-fit.
            plot.Axes.Bottom.Range.Set(-2, displayCandles.Count + 2);

            // Collect in-progress waves once for both x-axis extension and projection rendering.
            var inProgressWaves = new List<ElliottWave>();
            CollectInProgressWaves(analysis.Waves, inProgressWaves);

            // Extend x-axis to accommodate projection arrows drawn beyond the last candle.
            ExtendXAxisForProjections(plot, inProgressWaves, displayCandles.Count, timeframe);

            // Render projections AFTER axis setup so range checks use correct bounds.
            RenderProjections(plot, inProgressWaves, displayCandles, timeframe);

            // Title
            var first = displayCandles.First().TimestampUtc;
            var last = displayCandles.Last().TimestampUtc;
            plot.Title($"Elliott Wave Analysis ({symbol})  |  {first:MMM yyyy} \u2013 {last:MMM yyyy}");
            plot.Axes.Title.Label.ForeColor = AxisColor;
            plot.Axes.Title.Label.FontSize = TitleFontSize;
            plot.Axes.Title.Label.Bold = true;

            return plot.GetImageBytes(w, h, ImageFormat.Png);
        }

        // ───────────────────────── Dimensions ─────────────────────────

        private const int DefaultWidth = 1920;
        private const int DefaultHeight = 1080;

        // ───────────────────────── Theme ─────────────────────────

        private static void ApplyTheme(Plot plot)
        {
            plot.FigureBackground.Color = BgColor;
            plot.DataBackground.Color = DataBgColor;
            plot.Axes.Color(AxisColor);
            plot.Grid.MajorLineColor = GridColor;
            plot.Axes.Bottom.Label.ForeColor = AxisColor;
        }

        // ───────────────────────── Log Y-Axis ─────────────────────────

        private static void SetupLogYAxis(Plot plot, List<Ohlcv> candleData, List<decimal> labelPrices)
        {
            // Use percentile-based range to exclude flash crash outliers that distort the chart.
            // Flash crashes and listing artifacts are exchange anomalies that
            // stretch the y-axis and compress meaningful price action into a narrow band.
            var sortedLows = candleData.Select(c => c.Low).Where(p => p > 0).OrderBy(p => p).ToList();
            var sortedHighs = candleData.Select(c => c.High).Where(p => p > 0).OrderBy(p => p).ToList();
            int pctIdx1 = Math.Max(0, (int)(sortedLows.Count * Thresholds.PercentileLow));       // 0.5th percentile
            int pctIdx99 = Math.Min(sortedHighs.Count - 1, (int)(sortedHighs.Count * Thresholds.PercentileHigh));  // 99.5th percentile
            decimal minPrice = sortedLows[pctIdx1];
            decimal maxPrice = sortedHighs[pctIdx99];

            // Extend range to include wave label prices so labels at wave endpoints
            // that fall outside the percentile range are still visible. Skip extreme
            // flash-crash outliers (>1 log-decade beyond the percentile bound) that
            // would compress all meaningful price action into a narrow band.
            double logMin = Log10(minPrice);
            double logMax = Log10(maxPrice);
            foreach (var price in labelPrices)
            {
                if (price <= 0) continue;
                double logP = Log10(price);
                if (logP < logMin && logMin - logP < Thresholds.YAxisEdgeLabelDecadeThreshold) // within 1 decade below
                    minPrice = price;
                if (logP > logMax && logP - logMax < Thresholds.YAxisEdgeLabelDecadeThreshold) // within 1 decade above
                    maxPrice = price;
            }

            // Y-axis range: padding must accommodate stacked wave labels at price extremes.
            // A 3-label stack (Cycle/Primary/Intermediate) at 16pt+13pt+10pt fonts with
            // 20px tier spacing needs ~70px below the lowest candle. On a typical 1080px
            // chart with axis margins, 0.55 log-units (factor of ~3.5x) ensures the
            // Cycle-degree label at the absolute low is never clipped.
            double logPadding = Thresholds.YAxisLogPadding;
            double yMin = Log10(minPrice) - logPadding;
            double yMax = Log10(maxPrice) + logPadding;

            // Force the left axis range so candlesticks render within proper bounds.
            plot.Axes.Left.Range.Set(yMin, yMax);

            var tickPositions = new List<double>();
            var tickLabels = new List<string>();

            // Generate price ticks at round numbers that span the visible y-axis range.
            // Uses 1-2-5 sequence across decades for clean spacing on log scale.
            decimal yMinPrice = (decimal)Math.Pow(10, yMin);
            decimal yMaxPrice = (decimal)Math.Pow(10, yMax);
            decimal[] bases = TickBases;
            for (decimal decade = 0.01m; decade <= 1_000_000m; decade *= 10)
            {
                foreach (var b in bases)
                {
                    decimal level = b * decade;
                    if (level >= yMinPrice && level <= yMaxPrice)
                    {
                        tickPositions.Add(Log10(level));
                        tickLabels.Add(FormatPrice(level));
                    }
                }
            }

            // Left side: no price ticks — reserved for invalidation/projection labels only
            plot.Axes.Left.SetTicks([], []);
            plot.Axes.Left.MajorTickStyle.Length = 0;
            plot.Axes.Left.MinorTickStyle.Length = 0;

            // Price scale on the right side for readability (close to last candle).
            var rightAxis = plot.Axes.AddRightAxis();
            rightAxis.Range.Set(yMin, yMax);

            // Add an invisible scatter plot locked to the right axis to force its range
            // to match the data area (ScottPlot syncs axis ranges from plottable data).
            var anchor = plot.Add.ScatterLine(new double[] { 0, 1 }, new double[] { yMin, yMax });
            anchor.Axes.YAxis = rightAxis;
            anchor.LineStyle.IsVisible = false;

            rightAxis.SetTicks(tickPositions.ToArray(), tickLabels.ToArray());
            rightAxis.TickLabelStyle.ForeColor = AxisColor;
            rightAxis.TickLabelStyle.FontSize = TickFontSize;
            rightAxis.FrameLineStyle.IsVisible = false;
            rightAxis.MajorTickStyle.Color = AxisColor;
        }

        // ───────────────────────── Candlesticks ─────────────────────────

        private static void AddCandlesticks(Plot plot, List<Ohlcv> candleData, CandleTimeframe timeframe)
        {
            var prices = new List<OHLC>(candleData.Count);
            var candleDuration = GetCandleDuration(timeframe);

            for (int i = 0; i < candleData.Count; i++)
            {
                var c = candleData[i];
                // Skip invalid candles with zero/negative prices (can't log-transform)
                if (c.Low <= 0 || c.High <= 0) continue;
                // Ensure OHLC invariants: low <= open/close <= high
                // Data anomalies (e.g., flash crash wicks) can violate these.
                decimal low = Math.Min(c.Low, Math.Min(c.Open, c.Close));
                decimal high = Math.Max(c.High, Math.Max(c.Open, c.Close));
                prices.Add(new OHLC(
                    Log10(c.Open),
                    Log10(high),
                    Log10(low),
                    Log10(c.Close),
                    c.TimestampUtc,
                    candleDuration));
            }

            var candles = plot.Add.Candlestick(prices);
            candles.Sequential = true;
            candles.RisingColor = CandleUp;
            candles.FallingColor = CandleDown;

            // X-axis ticks — adaptive format per timeframe
            int tickInterval = Math.Max(1, candleData.Count / Thresholds.TargetXAxisTickCount);
            var tickPositions = new List<double>();
            var tickLabels = new List<string>();
            string dateFormat = GetTickDateFormat(timeframe);

            for (int i = 0; i < candleData.Count; i += tickInterval)
            {
                tickPositions.Add(i);
                tickLabels.Add(candleData[i].TimestampUtc.ToString(dateFormat));
            }

            plot.Axes.Bottom.SetTicks(tickPositions.ToArray(), tickLabels.ToArray());
            plot.Axes.Bottom.TickLabelStyle.ForeColor = AxisColor;
            plot.Axes.Bottom.TickLabelStyle.FontSize = TickFontSize;
        }

        private static TimeSpan GetCandleDuration(CandleTimeframe timeframe) => timeframe switch
        {
            CandleTimeframe.Monthly => TimeSpan.FromDays(Thresholds.MonthlyCandleDays),
            CandleTimeframe.Daily => TimeSpan.FromDays(1),
            CandleTimeframe.Hourly => TimeSpan.FromHours(1),
            _ => TimeSpan.FromDays(1)
        };

        private static string GetTickDateFormat(CandleTimeframe timeframe) => timeframe switch
        {
            CandleTimeframe.Monthly => "yyyy",
            CandleTimeframe.Daily => "MMM yy",
            CandleTimeframe.Hourly => "MMM yy",
            _ => "MMM yy"
        };

        // ───────────────────────── Label Collection ─────────────────────────

        /// <summary>
        /// Collects wave labels, filtered by which degrees are visible at the given timeframe:
        ///   Monthly → Cycle only
        ///   Daily   → Cycle + Primary
        ///   Hourly  → Cycle + Primary + Intermediate
        /// </summary>
        private static List<LabelInfo> CollectLabels(ElliottWavesAnalysis analysis, CandleTimeframe timeframe)
        {
            var labels = new List<LabelInfo>();
            var visibleDegrees = GetVisibleDegrees(timeframe);

            foreach (var wave in analysis.Waves)
                CollectLabelsRecursive(wave, labels, visibleDegrees);

            return labels;
        }

        private static HashSet<WaveDegree> GetVisibleDegrees(CandleTimeframe timeframe) => timeframe switch
        {
            CandleTimeframe.Monthly => [WaveDegree.Cycle],
            CandleTimeframe.Daily => [WaveDegree.Cycle, WaveDegree.Primary, WaveDegree.Intermediate],
            CandleTimeframe.Hourly => [WaveDegree.Cycle, WaveDegree.Primary, WaveDegree.Intermediate],
            _ => [WaveDegree.Cycle, WaveDegree.Primary]
        };

        /// <summary>
        /// Minimum time span a wave must cover for its Intermediate sub-wave labels
        /// to be rendered on the Daily chart. Below this threshold, labels overlap
        /// and become unreadable.
        /// </summary>
        private static readonly TimeSpan MinSpanForIntermediateLabels = TimeSpan.FromDays(30);

        private static void CollectLabelsRecursive(ElliottWave wave, List<LabelInfo> labels,
            HashSet<WaveDegree> visibleDegrees)
        {
            var degree = wave.Degree;

            if (!wave.IsInProgress && visibleDegrees.Contains(degree))
            {
                bool isHigh = wave.EndPoint.PointType == PointType.High;
                labels.Add(new LabelInfo(
                    degree,
                    FormatLabel(degree, wave.Label),
                    isHigh,
                    wave.EndPoint.Timestamp,
                    wave.EndPoint.Price));
            }

            if (wave.SubWaves != null)
            {
                // Skip Intermediate sub-wave labels when the parent wave spans < 30 days;
                // at that density the labels overlap and are unreadable on the Daily chart.
                bool skipChildren = wave.SubWaves.Count > 0
                    && !wave.IsInProgress
                    && wave.SubWaves[0].Degree == WaveDegree.Intermediate
                    && (wave.EndPoint.Timestamp - wave.StartPoint.Timestamp) < MinSpanForIntermediateLabels;

                if (!skipChildren)
                {
                    foreach (var sub in wave.SubWaves)
                        CollectLabelsRecursive(sub, labels, visibleDegrees);
                }
            }
        }

        // ───────────────────────── Label Rendering ─────────────────────────

        /// <summary>
        /// Groups labels by PRICE + direction (not by timestamp/index), so that Cycle/Primary/Intermediate
        /// labels that share the same pivot price are stacked together even if their timestamps differ
        /// (monthly vs daily vs hourly).
        ///
        /// For X-position, finds the candle whose High or Low matches the pivot price.
        /// This ensures all degrees point to the exact same candle.
        ///
        /// Stacking order (reading top to bottom, away from candle):
        ///   HIGH pivots (above candle): (I) / 5 / v    — Cycle farthest, Intermediate closest
        ///   LOW pivots (below candle):  v / 5 / (I)    — Intermediate closest, Cycle farthest
        /// </summary>
        private static void RenderLabels(Plot plot, List<LabelInfo> labels, List<Ohlcv> candleData)
        {
            var groups = labels
                .GroupBy(l => (Price: l.Price, l.IsHigh))
                .ToList();

            foreach (var group in groups)
            {
                decimal price = group.Key.Price;
                bool isHigh = group.Key.IsHigh;

                // Find the candle whose High or Low matches this pivot price.
                // Use the finest-resolution timestamp (Intermediate > Primary > Cycle) to narrow search.
                var bestTimestamp = group
                    .OrderByDescending(l => l.Degree) // Intermediate=2, Primary=1, Cycle=0
                    .First().Timestamp;

                int xIndex = FindCandleByPrice(candleData, price, isHigh, bestTimestamp);
                if (xIndex < 0) continue;

                // Sort by degree descending so tier 0 = Intermediate (closest to candle),
                // higher tiers = larger degrees (farther from candle).
                // This produces the correct visual stacking:
                //   Highs (above candle, reading top to bottom): (I) / 5 / v
                //   Lows  (below candle, reading top to bottom): v / 5 / (I)
                var sorted = group.OrderByDescending(l => l.Degree).ToList();

                double yPrice = isHigh
                    ? Log10(candleData[xIndex].High)
                    : Log10(candleData[xIndex].Low);

                for (int tier = 0; tier < sorted.Count; tier++)
                {
                    var label = sorted[tier];
                    int offsetPx = BaseOffsetPx + tier * LabelStepPx;

                    var text = plot.Add.Text(label.Text, xIndex, yPrice);
                    text.LabelFontSize = GetFontSize(label.Degree);
                    text.LabelBold = true;
                    text.LabelFontColor = GetLabelColor(label.Degree);
                    text.LabelBackgroundColor = ScottPlot.Colors.Transparent;
                    text.LabelBorderWidth = 0;

                    if (isHigh)
                    {
                        // Labels above candle: offset upward, farthest first
                        text.LabelAlignment = Alignment.LowerCenter;
                        text.OffsetY = -offsetPx;
                    }
                    else
                    {
                        // Labels below candle: offset downward, farthest first
                        text.LabelAlignment = Alignment.UpperCenter;
                        text.OffsetY = offsetPx;
                    }
                }
            }
        }

        /// <summary>
        /// Finds the candle index whose High (for isHigh=true) or Low (for isHigh=false) matches
        /// the given price. Searches near the provided timestamp first, then widens the search.
        /// </summary>
        private static int FindCandleByPrice(List<Ohlcv> candles, decimal price, bool isHigh, DateTime nearTimestamp)
        {
            // Start near the timestamp, search outward
            int centerIdx = FindClosestTimestamp(candles, nearTimestamp);
            int searchRadius = Math.Min(candles.Count, Thresholds.ChartSearchRadius);

            for (int offset = 0; offset <= searchRadius; offset++)
            {
                // Search right
                int idx = centerIdx + offset;
                if (idx >= 0 && idx < candles.Count)
                {
                    if (isHigh && candles[idx].High == price) return idx;
                    if (!isHigh && candles[idx].Low == price) return idx;
                }

                // Search left
                if (offset > 0)
                {
                    idx = centerIdx - offset;
                    if (idx >= 0 && idx < candles.Count)
                    {
                        if (isHigh && candles[idx].High == price) return idx;
                        if (!isHigh && candles[idx].Low == price) return idx;
                    }
                }
            }

            // Fallback: closest timestamp
            return centerIdx;
        }

        // ───────────────────────── Projection Rendering ─────────────────────────

        private static void RenderProjections(Plot plot, List<ElliottWave> inProgressWaves,
            List<Ohlcv> candleData, CandleTimeframe timeframe)
        {
            var byDegree = inProgressWaves
                .Where(w => w.Projection != null)
                .GroupBy(w => w.Degree)
                .Select(g => g.Last())
                .ToList();

            // Get current y-axis range to skip projections far outside the visible area
            var yRange = plot.Axes.Left.Range;

            int lastCandleIndex = candleData.Count - 1;

            foreach (var wave in byDegree)
            {
                var proj = wave.Projection;
                if (proj == null) continue;

                var degree = wave.Degree;
                var color = GetLabelColor(degree);

                // Find the most probable target for the arrow and wave label
                var bestTarget = proj.Targets.OrderByDescending(t => t.Probability).First();

                // Draw horizontal lines with price labels for all targets (Fibonacci confluence)
                foreach (var target in proj.Targets)
                {
                    double yPos = Log10(target.Price);
                    if (yPos < yRange.Min || yPos > yRange.Max) continue;

                    var hl = plot.Add.HorizontalLine(yPos);
                    hl.LineColor = color.WithAlpha(0.5f);
                    hl.LineWidth = 1;
                    hl.LinePattern = LinePattern.Dashed;

                    string priceText = FormatPrice(target.Price);
                    var label = plot.Add.Text($"{priceText} ({target.FibonacciLevel:P1})", 0, yPos);
                    label.LabelFontSize = ProjectionFontSize;
                    label.LabelFontColor = color;
                    label.LabelBackgroundColor = DataBgColor;
                    label.LabelAlignment = Alignment.MiddleLeft;
                    label.LabelBorderWidth = 0;
                }

                // Invalidation line
                if (proj.InvalidationPoint.HasValue)
                {
                    double yPos = Log10(proj.InvalidationPoint.Value);
                    if (yPos >= yRange.Min && yPos <= yRange.Max)
                    {
                        var inv = plot.Add.HorizontalLine(yPos);
                        inv.LineColor = InvalidationColor.WithAlpha(0.7f);
                        inv.LineWidth = 1.5f;
                        inv.LinePattern = LinePattern.Dashed;

                        string priceText = FormatPrice(proj.InvalidationPoint.Value);
                        var label = plot.Add.Text($"INVALIDATION {priceText}", 0, yPos);
                        label.LabelFontSize = ProjectionFontSize;
                        label.LabelFontColor = InvalidationColor;
                        label.LabelBackgroundColor = DataBgColor;
                        label.LabelAlignment = Alignment.MiddleLeft;
                        label.LabelBorderWidth = 0;
                    }
                }

                // Dotted arrow from last candle to projected wave endpoint.
                // Clamp arrow target to visible y-axis range so the arrow is always drawn,
                // even when the projection target is far above or below the chart.
                double bestY = Log10(bestTarget.Price);
                double clampedY = Math.Clamp(bestY, yRange.Min + Thresholds.ClampMargin, yRange.Max - Thresholds.ClampMargin);

                // Calculate future x-position based on degree
                int futureCandleOffset = GetFutureCandleOffset(degree, timeframe);
                double arrowEndX = lastCandleIndex + futureCandleOffset;

                // Last candle close price as arrow start Y
                double lastCloseY = Log10(candleData[lastCandleIndex].Close);

                // Draw dotted line from last candle to projected target
                var line = plot.Add.ScatterLine(
                    new double[] { lastCandleIndex, arrowEndX },
                    new double[] { lastCloseY, clampedY });
                line.LineColor = color.WithAlpha(0.8f);
                line.LineWidth = 2;
                line.LinePattern = LinePattern.Dotted;

                // Arrowhead marker at the endpoint — direction matches price movement
                var arrowHead = plot.Add.Marker(arrowEndX, clampedY);
                arrowHead.Shape = bestY < lastCloseY
                    ? MarkerShape.FilledTriangleDown
                    : MarkerShape.FilledTriangleUp;
                arrowHead.Size = 8;
                arrowHead.Color = color.WithAlpha(0.8f);

                // Wave label above arrowhead for highs, below for lows (same as regular labels)
                bool isHighPoint = bestY >= lastCloseY;
                string waveLabel = FormatLabel(degree, wave.Label);
                var waveLabelText = plot.Add.Text(waveLabel, arrowEndX, clampedY);
                waveLabelText.LabelFontSize = GetFontSize(degree);
                waveLabelText.LabelBold = true;
                waveLabelText.LabelFontColor = color;
                waveLabelText.LabelBackgroundColor = DataBgColor;
                waveLabelText.LabelBorderWidth = 0;
                waveLabelText.LabelAlignment = isHighPoint
                    ? Alignment.LowerCenter   // above the arrowhead
                    : Alignment.UpperCenter;   // below the arrowhead
                waveLabelText.OffsetY = isHighPoint ? -Thresholds.ProjectionLabelOffsetPx : Thresholds.ProjectionLabelOffsetPx;
            }
        }

        /// <summary>
        /// Converts degree-specific future time offsets to candle count:
        ///   Cycle = 3 months, Primary = 1 month, Intermediate = 3 hours
        /// Offset separation prevents Cycle and Primary arrows from overlapping.
        /// </summary>
        private static int GetFutureCandleOffset(WaveDegree degree, CandleTimeframe timeframe)
        {
            // Target durations in hours
            double targetHours = degree switch
            {
                WaveDegree.Cycle => Thresholds.CycleFutureOffsetHours,
                WaveDegree.Primary => Thresholds.PrimaryFutureOffsetHours,
                WaveDegree.Intermediate => Thresholds.IntermediateFutureOffsetHours,
                _ => Thresholds.PrimaryFutureOffsetHours
            };

            double candleHours = timeframe switch
            {
                CandleTimeframe.Monthly => Thresholds.MonthlyCandleDays * 24,
                CandleTimeframe.Daily => 24,
                CandleTimeframe.Hourly => 1,
                _ => 24
            };

            return Math.Max(1, (int)(targetHours / candleHours));
        }

        private static void CollectInProgressWaves(List<ElliottWave> waves, List<ElliottWave> result)
        {
            if (waves == null) return;
            foreach (var wave in waves)
            {
                if (wave.IsInProgress && wave.Projection != null)
                    result.Add(wave);

                CollectInProgressWaves(wave.SubWaves, result);
            }
        }

        // ───────────────────────── Label Formatting ─────────────────────────

        /// <summary>
        /// Formats a wave label according to standard Elliott Wave notation:
        ///   Cycle:        (I) (II) (III) (IV) (V) | (A) (B) (C) etc.
        ///   Primary:      1  2  3  4  5           | A  B  C  etc.
        ///   Intermediate: i  ii  iii  iv  v       | a  b  c  etc.
        /// </summary>
        private static string FormatLabel(WaveDegree degree, WaveNumber label)
        {
            string raw = label switch
            {
                WaveNumber.One => "1",
                WaveNumber.Two => "2",
                WaveNumber.Three => "3",
                WaveNumber.Four => "4",
                WaveNumber.Five => "5",
                WaveNumber.A => "A",
                WaveNumber.B => "B",
                WaveNumber.C => "C",
                WaveNumber.D => "D",
                WaveNumber.E => "E",
                WaveNumber.W => "W",
                WaveNumber.X1 => "X",
                WaveNumber.Y => "Y",
                WaveNumber.X2 => "X",
                WaveNumber.Z => "Z",
                _ => "?"
            };

            bool isMotive = label is WaveNumber.One or WaveNumber.Two
                or WaveNumber.Three or WaveNumber.Four or WaveNumber.Five;

            return degree switch
            {
                WaveDegree.Cycle => $"({(isMotive ? ToRoman(raw, uppercase: true) : raw)})",
                WaveDegree.Primary => raw,
                WaveDegree.Intermediate => isMotive ? ToRoman(raw, uppercase: false) : raw.ToLower(),
                _ => raw
            };
        }

        private static string ToRoman(string digit, bool uppercase) => digit switch
        {
            "1" => uppercase ? "I" : "i",
            "2" => uppercase ? "II" : "ii",
            "3" => uppercase ? "III" : "iii",
            "4" => uppercase ? "IV" : "iv",
            "5" => uppercase ? "V" : "v",
            _ => uppercase ? digit : digit.ToLower()
        };

        // ───────────────────────── Candle Aggregation ─────────────────────────

        /// <summary>
        /// Aggregates hourly candle data to the requested timeframe using the shared aggregator.
        /// </summary>
        private static List<Ohlcv> AggregateToTimeframe(List<Ohlcv> hourlyCandles, CandleTimeframe timeframe)
        {
            if (timeframe == CandleTimeframe.Hourly)
                return hourlyCandles;

            if (timeframe == CandleTimeframe.Daily)
                return ElliottWavesAnalyzer.AggregateCandles(hourlyCandles,
                    x => x.TimestampUtc.Date, k => k);

            // Monthly
            return ElliottWavesAnalyzer.AggregateCandles(hourlyCandles,
                x => new { x.TimestampUtc.Year, x.TimestampUtc.Month },
                k => new DateTime(k.Year, k.Month, 1, 0, 0, 0, DateTimeKind.Utc));
        }

        // ───────────────────────── Helpers ─────────────────────────

        private static double Log10(decimal value) =>
            Math.Log10(Math.Max((double)Thresholds.OutlierPriceThreshold, (double)value));

        /// <summary>Format price for chart labels: $X.XX for prices below $10, $X for larger prices.</summary>
        private static string FormatPrice(decimal price) =>
            price < 10 ? $"${price:N2}" : $"${price:N0}";

        private static float GetFontSize(WaveDegree degree) => degree switch
        {
            WaveDegree.Cycle => CycleFontSize,
            WaveDegree.Primary => PrimaryFontSize,
            WaveDegree.Intermediate => IntermediateFontSize,
            _ => PrimaryFontSize
        };

        private static ScottPlot.Color GetLabelColor(WaveDegree degree) => degree switch
        {
            WaveDegree.Cycle => CycleColor,
            WaveDegree.Primary => PrimaryColor,
            WaveDegree.Intermediate => IntermediateColor,
            _ => AxisColor
        };

        private static int FindClosestTimestamp(List<Ohlcv> candles, DateTime target) =>
            WaveMath.FindCandleIndex(candles, target);

        private static void ExtendXAxisForProjections(Plot plot, List<ElliottWave> inProgressWaves,
            int candleCount, CandleTimeframe timeframe)
        {
            int maxOffset = 0;
            foreach (var wave in inProgressWaves.Where(w => w.Projection != null))
            {
                int offset = GetFutureCandleOffset(wave.Degree, timeframe);
                if (offset > maxOffset) maxOffset = offset;
            }
            if (maxOffset > 0)
            {
                // Extend the x-axis to fit the arrow + label below
                plot.Axes.Bottom.Range.Set(plot.Axes.Bottom.Range.Min,
                    candleCount - 1 + maxOffset + maxOffset * Thresholds.XAxisExtensionFactor);
            }
        }
    }
}
