using SmarterSystems.ElliottWaves.Analyzer.Interfaces;

namespace SmarterSystems.ElliottWaves.Analyzer.Analysis
{
    /// <summary>
    /// Pure Fibonacci math for the forward-projection engine.
    /// All calculations use log-scale for crypto's exponential price moves.
    /// </summary>
    public static class FibonacciProjector
    {
        /// <summary>W2 retracement levels ordered most extreme first (§7.1).
        /// Per Governing Authority: 88.6% is NOT a Frost &amp; Prechter ratio and is excluded.</summary>
        public static readonly decimal[] W2RetracementLevels = [0.786m, 0.618m, 0.5m, 0.382m];

        /// <summary>W4 retracement levels include 23.6% for shallow corrections (§7.1).</summary>
        public static readonly decimal[] W4RetracementLevels = [0.786m, 0.618m, 0.5m, 0.382m, 0.236m];

        /// <summary>W3 extension levels ordered most extreme first (§7.2).</summary>
        public static readonly decimal[] W3ExtensionLevels = [4.236m, 2.618m, 1.618m, 1.0m];

        /// <summary>W5 projection levels relative to W1 (§7.2).</summary>
        public static readonly decimal[] W5Levels = [1.618m, 1.0m, 0.618m];

        /// <summary>B-wave retracement levels for corrections (§7.1).</summary>
        public static readonly decimal[] BRetracementLevels = [0.786m, 0.618m, 0.5m, 0.382m];

        /// <summary>C-wave extension levels relative to A (§7.3).</summary>
        public static readonly decimal[] CExtensionLevels = [2.618m, 1.618m, 1.0m, 0.618m];

        /// <summary>Intermediate-degree W3 extension levels — reduced because sub-waves
        /// have less room within their parent wave (§7.2).</summary>
        public static readonly decimal[] IntermediateW3ExtensionLevels = [1.618m, 1.0m, 0.618m];

        /// <summary>Intermediate-degree C-wave extension levels — reduced because sub-waves
        /// have less room within their parent wave (§7.3).</summary>
        public static readonly decimal[] IntermediateCExtensionLevels = [1.618m, 1.0m, 0.618m];

        /// <summary>
        /// Project retracement target prices from a wave's start and end (log-scale).
        /// Retracement moves against the wave direction.
        /// </summary>
        /// <param name="waveStart">Price at wave start (e.g., W0 for W1 retrace)</param>
        /// <param name="waveEnd">Price at wave end (e.g., W1 for W1 retrace)</param>
        /// <param name="levels">Fibonacci levels to project</param>
        /// <returns>List of (targetPrice, fibLevel) ordered by input level order</returns>
        public static List<(decimal Price, decimal FibLevel)> ProjectRetracements(
            decimal waveStart, decimal waveEnd, decimal[] levels)
        {
            var logStart = WaveMath.Log(waveStart);
            var logEnd = WaveMath.Log(waveEnd);
            var logMove = logEnd - logStart;

            var targets = new List<(decimal Price, decimal FibLevel)>(levels.Length);
            foreach (var level in levels)
            {
                // Retracement moves back from the end toward the start
                var logTarget = logEnd - level * logMove;
                var price = (decimal)Math.Exp((double)logTarget);
                targets.Add((price, level));
            }
            return targets;
        }

        /// <summary>
        /// Project extension target prices from a measured wave, starting from a base (log-scale).
        /// Extension moves in the same direction as the reference wave.
        /// </summary>
        /// <param name="waveStart">Start of the reference wave (e.g., W0)</param>
        /// <param name="waveEnd">End of the reference wave (e.g., W1)</param>
        /// <param name="extensionBase">Price to project from (e.g., W2 end)</param>
        /// <param name="levels">Extension multiples to project</param>
        /// <returns>List of (targetPrice, fibLevel) ordered by input level order</returns>
        public static List<(decimal Price, decimal FibLevel)> ProjectExtensions(
            decimal waveStart, decimal waveEnd, decimal extensionBase, decimal[] levels)
        {
            var logWaveLength = Math.Abs(WaveMath.Log(waveEnd) - WaveMath.Log(waveStart));
            var logBase = WaveMath.Log(extensionBase);
            bool isUptrend = waveEnd > waveStart;

            var targets = new List<(decimal Price, decimal FibLevel)>(levels.Length);
            foreach (var level in levels)
            {
                // Extension moves from base in the wave's direction
                var logTarget = isUptrend
                    ? logBase + level * logWaveLength
                    : logBase - level * logWaveLength;
                var price = (decimal)Math.Exp((double)logTarget);
                targets.Add((price, level));
            }
            return targets;
        }

    }
}
