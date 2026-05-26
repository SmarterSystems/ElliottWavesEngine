using SmarterSystems.ElliottWaves.Analyzer.Interfaces;

namespace SmarterSystems.ElliottWaves.Analyzer.Analysis
{
    /// <summary>
    /// Creates ElliottWave objects from confirmed pivot points.
    /// No pattern detection logic — just labeling, projections, and DTO creation.
    /// </summary>
    public static class WaveLabeler
    {
        private static readonly WaveNumber[] MotiveLabels =
            [WaveNumber.One, WaveNumber.Two, WaveNumber.Three, WaveNumber.Four, WaveNumber.Five];

        private static readonly WaveNumber[] CorrectiveLabels =
            [WaveNumber.A, WaveNumber.B, WaveNumber.C];

        private static readonly WaveNumber[] TriangleLabels =
            [WaveNumber.A, WaveNumber.B, WaveNumber.C, WaveNumber.D, WaveNumber.E];

        private static readonly WaveNumber[] DoubleComboLabels =
            [WaveNumber.W, WaveNumber.X1, WaveNumber.Y];

        private static readonly WaveNumber[] TripleComboLabels =
            [WaveNumber.W, WaveNumber.X1, WaveNumber.Y, WaveNumber.X2, WaveNumber.Z];

        /// <summary>
        /// Create ElliottWave objects from confirmed impulse pivots.
        /// pivots = [W0, W1, W2, ...] where consecutive pairs form waves.
        /// </summary>
        public static List<ElliottWave> CreateImpulseWaves(
            List<PivotPoint> pivots,
            List<FibonacciJustification> justifications,
            WaveDegree degree,
            PatternSubType? subType = null,
            WaveNumber? inProgressLabel = null,
            Projection inProgressProjection = null,
            PivotPoint inProgressStart = null)
        {
            var waves = new List<ElliottWave>();
            var patternType = subType is PatternSubType.EndingDiagonal or PatternSubType.LeadingDiagonal
                ? PatternType.Diagonal
                : PatternType.Impulse;

            // Create confirmed waves from consecutive pivot pairs
            for (int i = 0; i < pivots.Count - 1; i++)
            {
                waves.Add(new ElliottWave
                {
                    Degree = degree,
                    Label = MotiveLabels[i],
                    StartPoint = pivots[i],
                    EndPoint = pivots[i + 1],
                    IsInProgress = false,
                    PatternType = patternType,
                    PatternSubType = subType,
                    Justification = i < justifications.Count ? justifications[i] : null
                });
            }

            // Add in-progress wave if specified
            if (inProgressLabel != null && inProgressStart != null)
            {
                waves.Add(CreateInProgressWave(inProgressStart, inProgressLabel.Value,
                    degree, inProgressProjection));
            }

            return waves;
        }

        /// <summary>
        /// Create ElliottWave objects from confirmed corrective pivots (A-B-C).
        /// </summary>
        public static List<ElliottWave> CreateCorrectiveWaves(
            List<PivotPoint> pivots,
            List<FibonacciJustification> justifications,
            WaveDegree degree,
            PatternType patternType,
            PatternSubType? subType = null,
            WaveNumber? inProgressLabel = null,
            Projection inProgressProjection = null,
            PivotPoint inProgressStart = null)
        {
            var waves = new List<ElliottWave>();
            var labels = patternType switch
            {
                PatternType.Triangle => TriangleLabels,
                PatternType.Complex when pivots.Count - 1 <= 3 => DoubleComboLabels,
                PatternType.Complex => TripleComboLabels,
                _ => CorrectiveLabels
            };

            for (int i = 0; i < pivots.Count - 1 && i < labels.Length; i++)
            {
                waves.Add(new ElliottWave
                {
                    Degree = degree,
                    Label = labels[i],
                    StartPoint = pivots[i],
                    EndPoint = pivots[i + 1],
                    IsInProgress = false,
                    PatternType = patternType,
                    PatternSubType = subType,
                    Justification = i < justifications.Count ? justifications[i] : null
                });
            }

            // Add in-progress wave if specified — inherits the corrective pattern type
            if (inProgressLabel != null && inProgressStart != null)
            {
                waves.Add(CreateInProgressWave(inProgressStart, inProgressLabel.Value,
                    degree, inProgressProjection, patternType));
            }

            return waves;
        }

        /// <summary>
        /// Create a single in-progress wave with projection targets.
        /// </summary>
        public static ElliottWave CreateInProgressWave(
            PivotPoint start, WaveNumber label, WaveDegree degree,
            Projection projection, PatternType patternType = PatternType.Impulse)
        {
            return new ElliottWave
            {
                Degree = degree,
                Label = label,
                StartPoint = start,
                EndPoint = start, // Placeholder — same as start for in-progress
                IsInProgress = true,
                PatternType = patternType,
                Projection = projection
            };
        }

        /// <summary>
        /// Determines if a wave label represents a motive (actionary) wave.
        /// §2: Waves 1, 3, 5, A, C are actionary and develop in motive mode.
        /// §2: Waves 2, 4, B are reactionary and develop in corrective mode.
        /// </summary>
        public static bool IsMotiveLabel(WaveNumber label) => label switch
        {
            WaveNumber.One or WaveNumber.Three or WaveNumber.Five
                or WaveNumber.A or WaveNumber.C => true,
            _ => false
        };
    }
}
