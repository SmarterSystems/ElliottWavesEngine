using SmarterSystems.ElliottWaves.Analyzer.Interfaces;

namespace SmarterSystems.ElliottWaves.Analyzer.Analysis
{
    /// <summary>
    /// Shared Fibonacci projection building for all wave detectors.
    /// Encapsulates retracement/extension target creation and probability assignment.
    /// </summary>
    public static class ProjectionBuilder
    {
        public static Projection BuildRetracement(
            decimal waveStart, decimal waveEnd, decimal[] levels, decimal invalidation)
        {
            var targets = FibonacciProjector.ProjectRetracements(waveStart, waveEnd, levels);
            var targetList = targets.Select(t => new Target
            {
                Price = t.Price,
                FibonacciLevel = t.FibLevel,
                Probability = GetRetracementProbability(t.FibLevel)
            }).ToList();
            NormalizeProbabilities(targetList);
            return new Projection { Targets = targetList, InvalidationPoint = invalidation };
        }

        /// <summary>
        /// Higher extension levels used when the price has already exceeded all standard targets.
        /// Ensures at least one projection target lies beyond the current price.
        /// </summary>
        private static readonly decimal[] EscalationLevels = [2.618m, 4.236m, 6.854m, 11.09m];

        public static Projection BuildExtension(
            decimal waveStart, decimal waveEnd, decimal extensionBase,
            decimal[] levels, decimal invalidation, bool isW3 = false,
            bool isIntermediate = false, decimal? currentPrice = null)
        {
            var targets = FibonacciProjector.ProjectExtensions(
                waveStart, waveEnd, extensionBase, levels);

            // Remove any projection targets the price has already exceeded —
            // they would point backward (e.g., upward in a downtrend).
            // If all targets are exceeded, escalate to higher extension levels
            // until at least one target lies beyond the current price.
            if (currentPrice.HasValue && targets.Count > 0)
            {
                bool isUptrend = waveEnd > waveStart;

                bool allExceeded = targets.All(t => isUptrend
                    ? currentPrice.Value >= t.Price
                    : currentPrice.Value <= t.Price);

                if (allExceeded)
                {
                    targets.Clear();
                    foreach (var level in EscalationLevels)
                    {
                        if (levels.Contains(level)) continue;
                        var escalated = FibonacciProjector.ProjectExtensions(
                            waveStart, waveEnd, extensionBase, [level]);
                        targets.AddRange(escalated);

                        bool beyondCurrent = isUptrend
                            ? escalated[0].Price > currentPrice.Value
                            : escalated[0].Price < currentPrice.Value;
                        if (beyondCurrent) break;
                    }
                }

                // Final filter: remove all exceeded targets (standard and escalated)
                targets.RemoveAll(t => isUptrend
                    ? currentPrice.Value >= t.Price
                    : currentPrice.Value <= t.Price);
            }

            var targetList = targets.Select(t => new Target
            {
                Price = t.Price,
                FibonacciLevel = t.FibLevel,
                Probability = GetExtensionProbability(t.FibLevel, isW3, isIntermediate)
            }).ToList();
            NormalizeProbabilities(targetList);
            return new Projection { Targets = targetList, InvalidationPoint = invalidation };
        }

        public static Projection BuildW5(
            PivotPoint w0, PivotPoint w1, PivotPoint w4, decimal? invalidation)
        {
            var targets = FibonacciProjector.ProjectExtensions(
                w0.Price, w1.Price, w4.Price, FibonacciProjector.W5Levels);
            var targetList = targets.Select(t => new Target
            {
                Price = t.Price,
                FibonacciLevel = t.FibLevel,
                Probability = GetW5Probability(t.FibLevel)
            }).ToList();
            NormalizeProbabilities(targetList);
            return new Projection { Targets = targetList, InvalidationPoint = invalidation };
        }

        /// <summary>
        /// Normalize target probabilities to sum to 1.0. Probability functions assign weights
        /// per Fibonacci level, but the actual levels present vary (W2 has 4 levels, W4 has 5,
        /// extension targets may be filtered/escalated at runtime). Normalizing after target
        /// creation ensures probabilities always sum to 1.0 regardless of which levels remain.
        /// </summary>
        private static void NormalizeProbabilities(List<Target> targets)
        {
            if (targets.Count == 0) return;
            var sum = targets.Sum(t => t.Probability);
            if (sum <= 0) return;
            foreach (var t in targets)
                t.Probability = Math.Round(t.Probability / sum, 4);
        }

        private static decimal GetW5Probability(decimal level) => level switch
        {
            1.0m => 0.35m,    // W5 = W1 (equality) — most common
            0.618m => 0.25m,  // W5 = 0.618 × W1
            _ => 0.15m
        };

        private static decimal GetRetracementProbability(decimal level) => level switch
        {
            0.786m => 0.10m,
            0.618m => 0.25m,
            0.5m => 0.35m,
            0.382m => 0.20m,
            0.236m => 0.10m,
            _ => 0.10m
        };

        private static decimal GetExtensionProbability(decimal level, bool isW3, bool isIntermediate) =>
            (isW3, isIntermediate) switch
            {
                (true, true) => GetIntermediateW3ExtensionProbability(level),
                (true, false) => GetW3ExtensionProbability(level),
                (false, true) => GetIntermediateCExtensionProbability(level),
                (false, false) => GetCExtensionProbability(level),
            };

        /// <summary>
        /// W3 extension probability: 1.618 is most common for impulse wave 3.
        /// </summary>
        private static decimal GetW3ExtensionProbability(decimal level) => level switch
        {
            4.236m => 0.05m,
            2.618m => 0.15m,
            1.618m => 0.40m,
            1.0m => 0.30m,
            0.618m => 0.10m,
            _ => 0.10m
        };

        /// <summary>
        /// C-wave extension probability: 1.0 (A=C) is most likely for corrective C waves.
        /// </summary>
        private static decimal GetCExtensionProbability(decimal level) => level switch
        {
            4.236m => 0.05m,
            2.618m => 0.15m,
            1.618m => 0.30m,
            1.0m => 0.40m,
            0.618m => 0.10m,
            _ => 0.10m
        };

        /// <summary>
        /// Intermediate W3 extension: 1.0 (sub-wave equals wave i) is most common
        /// because lower-degree waves have less room within their parent wave.
        /// </summary>
        private static decimal GetIntermediateW3ExtensionProbability(decimal level) => level switch
        {
            1.618m => 0.35m,
            1.0m => 0.40m,
            0.618m => 0.25m,
            _ => 0.10m
        };

        /// <summary>
        /// Intermediate C-wave extension: 1.0 (a=c) is most common at lower degree.
        /// </summary>
        private static decimal GetIntermediateCExtensionProbability(decimal level) => level switch
        {
            1.618m => 0.25m,
            1.0m => 0.45m,
            0.618m => 0.30m,
            _ => 0.10m
        };
    }
}
