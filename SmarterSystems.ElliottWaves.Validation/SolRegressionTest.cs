using SmarterSystems.ElliottWaves.Analyzer.Interfaces;

namespace SmarterSystems.ElliottWaves.Validation
{
    [TestClass]
    public sealed class SolRegressionTest : RegressionTestBase
    {
        private static ElliottWavesAnalysis _analysis = null!;

        [ClassInitialize]
        public static void Setup(TestContext _)
        {
            _analysis = LoadAndAnalyze("SOLUSDT");
        }

        // ══════════════════════════════════════════════════
        // Cycle Degree: 3 waves (I, II completed; III in-progress)
        //
        // Cycle (I):   23.12.2020 $1.0301 → 06.11.2021 $259.90
        // Cycle (II):  06.11.2021 $259.90 → 29.12.2022 $8.00
        // Cycle (III): 29.12.2022 $8.00 → in-progress
        // ══════════════════════════════════════════════════

        [TestMethod] public void Cycle_WaveCount() => AssertWaveCount(_analysis, 3, "SOL Cycle");

        // ── Cycle (I): $1.0301 → $259.90 ──

        [TestMethod] public void CycleI_Label() => AssertLabel(_analysis.Waves[0], WaveNumber.One, "Cycle(I)");
        [TestMethod] public void CycleI_NotInProgress() => AssertInProgress(_analysis.Waves[0], false, "Cycle(I)");
        [TestMethod] public void CycleI_Start() => AssertPoint(_analysis.Waves[0].StartPoint, Utc(2020, 12, 23, 22), 1.03010000m, "Cycle(I) Start");
        [TestMethod] public void CycleI_End() => AssertPoint(_analysis.Waves[0].EndPoint, Utc(2021, 11, 6, 21), 259.90m, "Cycle(I) End");

        // ── Cycle (II): $259.90 → $8.00 ──

        [TestMethod] public void CycleII_Label() => AssertLabel(_analysis.Waves[1], WaveNumber.Two, "Cycle(II)");
        [TestMethod] public void CycleII_NotInProgress() => AssertInProgress(_analysis.Waves[1], false, "Cycle(II)");
        [TestMethod] public void CycleII_Start() => AssertPoint(_analysis.Waves[1].StartPoint, Utc(2021, 11, 6, 21), 259.90m, "Cycle(II) Start");
        [TestMethod] public void CycleII_End() => AssertPoint(_analysis.Waves[1].EndPoint, Utc(2022, 12, 29, 20), 8.00m, "Cycle(II) End");

        // ── Cycle (III): $8.00 → in-progress ──

        [TestMethod] public void CycleIII_Label() => AssertLabel(_analysis.Waves[2], WaveNumber.Three, "Cycle(III)");
        [TestMethod] public void CycleIII_InProgress() => AssertInProgress(_analysis.Waves[2], true, "Cycle(III)");
        [TestMethod] public void CycleIII_Start() => AssertPoint(_analysis.Waves[2].StartPoint, Utc(2022, 12, 29, 20), 8.00m, "Cycle(III) Start");

        // ══════════════════════════════════════════════════
        // Primary Degree — inside Cycle (I): 5 waves (impulse)
        //
        // [1]: $1.0301 → $4.17 (18.01.2021)
        // [2]: $4.17 → $2.9717 (21.01.2021)
        // [3]: $2.9717 → $47.70 (21.05.2021)
        // [4]: $47.70 → $19.117 (23.05.2021)
        // [5]: $19.117 → $259.90 (06.11.2021)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleI_Primary_WaveCount() => AssertSubWaveCount(_analysis.Waves[0], 5, "Cycle(I) Primary");

        [TestMethod] public void CycleI_P1_Label() => AssertLabel(_analysis.Waves[0].SubWaves[0], WaveNumber.One, "Cycle(I) [1]");
        [TestMethod] public void CycleI_P1_Start() => AssertPoint(_analysis.Waves[0].SubWaves[0].StartPoint, Utc(2020, 12, 23, 22), 1.03010000m, "Cycle(I) [1] Start");
        [TestMethod] public void CycleI_P1_End() => AssertPoint(_analysis.Waves[0].SubWaves[0].EndPoint, Utc(2021, 1, 18, 8), 4.17m, "Cycle(I) [1] End");

        [TestMethod] public void CycleI_P2_Label() => AssertLabel(_analysis.Waves[0].SubWaves[1], WaveNumber.Two, "Cycle(I) [2]");
        [TestMethod] public void CycleI_P2_Start() => AssertPoint(_analysis.Waves[0].SubWaves[1].StartPoint, Utc(2021, 1, 18, 8), 4.17m, "Cycle(I) [2] Start");
        [TestMethod] public void CycleI_P2_End() => AssertPoint(_analysis.Waves[0].SubWaves[1].EndPoint, Utc(2021, 1, 22, 1), 2.5295m, "Cycle(I) [2] End");

        [TestMethod] public void CycleI_P3_Label() => AssertLabel(_analysis.Waves[0].SubWaves[2], WaveNumber.Three, "Cycle(I) [3]");
        [TestMethod] public void CycleI_P3_Start() => AssertPoint(_analysis.Waves[0].SubWaves[2].StartPoint, Utc(2021, 1, 22, 1), 2.5295m, "Cycle(I) [3] Start");
        [TestMethod] public void CycleI_P3_End() => AssertPoint(_analysis.Waves[0].SubWaves[2].EndPoint, Utc(2021, 5, 18, 16), 58.38m, "Cycle(I) [3] End");

        [TestMethod] public void CycleI_P4_Label() => AssertLabel(_analysis.Waves[0].SubWaves[3], WaveNumber.Four, "Cycle(I) [4]");
        [TestMethod] public void CycleI_P4_Start() => AssertPoint(_analysis.Waves[0].SubWaves[3].StartPoint, Utc(2021, 5, 18, 16), 58.38m, "Cycle(I) [4] Start");
        [TestMethod] public void CycleI_P4_End() => AssertPoint(_analysis.Waves[0].SubWaves[3].EndPoint, Utc(2021, 5, 23, 16), 19.117m, "Cycle(I) [4] End");

        [TestMethod] public void CycleI_P5_Label() => AssertLabel(_analysis.Waves[0].SubWaves[4], WaveNumber.Five, "Cycle(I) [5]");
        [TestMethod] public void CycleI_P5_Start() => AssertPoint(_analysis.Waves[0].SubWaves[4].StartPoint, Utc(2021, 5, 23, 16), 19.117m, "Cycle(I) [5] Start");
        [TestMethod] public void CycleI_P5_End() => AssertPoint(_analysis.Waves[0].SubWaves[4].EndPoint, Utc(2021, 11, 6, 21), 259.90m, "Cycle(I) [5] End");

        // ══════════════════════════════════════════════════
        // Primary Degree — inside Cycle (II): A-B-C (corrective)
        //
        // [A]: $259.90 → $25.86 (14.06.2022)
        // [B]: $25.86 → $48.38 (13.08.2022)
        // [C]: $48.38 → $8.00 (29.12.2022)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleII_Primary_WaveCount() => AssertSubWaveCount(_analysis.Waves[1], 3, "Cycle(II) Primary");

        [TestMethod] public void CycleII_A_Label() => AssertLabel(_analysis.Waves[1].SubWaves[0], WaveNumber.A, "Cycle(II) [A]");
        [TestMethod] public void CycleII_A_Start() => AssertPoint(_analysis.Waves[1].SubWaves[0].StartPoint, Utc(2021, 11, 6, 21), 259.90m, "Cycle(II) [A] Start");
        [TestMethod] public void CycleII_A_End() => AssertPoint(_analysis.Waves[1].SubWaves[0].EndPoint, Utc(2022, 6, 14, 2), 25.86m, "Cycle(II) [A] End");

        [TestMethod] public void CycleII_B_Label() => AssertLabel(_analysis.Waves[1].SubWaves[1], WaveNumber.B, "Cycle(II) [B]");
        [TestMethod] public void CycleII_B_Start() => AssertPoint(_analysis.Waves[1].SubWaves[1].StartPoint, Utc(2022, 6, 14, 2), 25.86m, "Cycle(II) [B] Start");
        [TestMethod] public void CycleII_B_End() => AssertPoint(_analysis.Waves[1].SubWaves[1].EndPoint, Utc(2022, 8, 13, 6), 48.38m, "Cycle(II) [B] End");

        [TestMethod] public void CycleII_C_Label() => AssertLabel(_analysis.Waves[1].SubWaves[2], WaveNumber.C, "Cycle(II) [C]");
        [TestMethod] public void CycleII_C_Start() => AssertPoint(_analysis.Waves[1].SubWaves[2].StartPoint, Utc(2022, 8, 13, 6), 48.38m, "Cycle(II) [C] Start");
        [TestMethod] public void CycleII_C_End() => AssertPoint(_analysis.Waves[1].SubWaves[2].EndPoint, Utc(2022, 12, 29, 20), 8.00m, "Cycle(II) [C] End");

        // ══════════════════════════════════════════════════
        // Primary Degree — inside Cycle (III): in-progress
        //
        // [1]: $8.00 → $295.83 (19.01.2025)
        // [2]: $295.83 → $67.50 (06.02.2026)
        // [3]: in-progress
        // ══════════════════════════════════════════════════

        [TestMethod]
        public void CycleIII_HasSubWaves()
        {
            Assert.IsNotNull(_analysis.Waves[2].SubWaves, "Cycle(III) should have sub-waves");
            Assert.IsTrue(_analysis.Waves[2].SubWaves.Count >= 1, "Cycle(III) should have at least 1 primary wave");
        }

        [TestMethod] public void CycleIII_P1_Label() => AssertLabel(_analysis.Waves[2].SubWaves[0], WaveNumber.One, "Cycle(III) [1]");
        [TestMethod] public void CycleIII_P1_Start() => AssertPoint(_analysis.Waves[2].SubWaves[0].StartPoint, Utc(2022, 12, 29, 20), 8.00m, "Cycle(III) [1] Start");
        [TestMethod] public void CycleIII_P1_End() => AssertPoint(_analysis.Waves[2].SubWaves[0].EndPoint, Utc(2025, 1, 19, 11), 295.83m, "Cycle(III) [1] End");

        [TestMethod] public void CycleIII_P2_Label() => AssertLabel(_analysis.Waves[2].SubWaves[1], WaveNumber.Two, "Cycle(III) [2]");
        [TestMethod] public void CycleIII_P2_Start() => AssertPoint(_analysis.Waves[2].SubWaves[1].StartPoint, Utc(2025, 1, 19, 11), 295.83m, "Cycle(III) [2] Start");
        [TestMethod] public void CycleIII_P2_End() => AssertPoint(_analysis.Waves[2].SubWaves[1].EndPoint, Utc(2026, 2, 6, 0), 67.50m, "Cycle(III) [2] End");

        [TestMethod] public void CycleIII_P3_Label() => AssertLabel(_analysis.Waves[2].SubWaves[2], WaveNumber.Three, "Cycle(III) [3]");
        [TestMethod] public void CycleIII_P3_InProgress() => AssertInProgress(_analysis.Waves[2].SubWaves[2], true, "Cycle(III) [3]");

        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(I) [1]
        //
        // (i): $1.0301 → $1.4599 (25.12.2020)
        // (ii): $1.4599 → $1.2616 (27.12.2020)
        // (iii): $1.2616 → $3.8800 (12.01.2021)
        // (iv): $3.8800 → $2.9618 (15.01.2021)
        // (v): $2.9618 → $4.1700 (18.01.2021)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleI_P1_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[0].SubWaves[0], 5, "Cycle(I) [1] Intermediate");

        [TestMethod] public void CycleI_P1_i1_Label() => AssertLabel(_analysis.Waves[0].SubWaves[0].SubWaves[0], WaveNumber.One, "Cycle(I) [1] (i)");
        [TestMethod] public void CycleI_P1_i1_Start() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[0].StartPoint, Utc(2020, 12, 23, 22, 0), 1.0301m, "Cycle(I) [1] (i) Start");
        [TestMethod] public void CycleI_P1_i1_End() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[0].EndPoint, Utc(2020, 12, 25, 3, 0), 1.4599m, "Cycle(I) [1] (i) End");

        [TestMethod] public void CycleI_P1_i2_Label() => AssertLabel(_analysis.Waves[0].SubWaves[0].SubWaves[1], WaveNumber.Two, "Cycle(I) [1] (ii)");
        [TestMethod] public void CycleI_P1_i2_Start() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[1].StartPoint, Utc(2020, 12, 25, 3, 0), 1.4599m, "Cycle(I) [1] (ii) Start");
        [TestMethod] public void CycleI_P1_i2_End() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[1].EndPoint, Utc(2020, 12, 27, 12, 0), 1.2616m, "Cycle(I) [1] (ii) End");

        [TestMethod] public void CycleI_P1_i3_Label() => AssertLabel(_analysis.Waves[0].SubWaves[0].SubWaves[2], WaveNumber.Three, "Cycle(I) [1] (iii)");
        [TestMethod] public void CycleI_P1_i3_Start() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[2].StartPoint, Utc(2020, 12, 27, 12, 0), 1.2616m, "Cycle(I) [1] (iii) Start");
        [TestMethod] public void CycleI_P1_i3_End() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[2].EndPoint, Utc(2021, 1, 12, 18, 0), 3.88m, "Cycle(I) [1] (iii) End");

        [TestMethod] public void CycleI_P1_i4_Label() => AssertLabel(_analysis.Waves[0].SubWaves[0].SubWaves[3], WaveNumber.Four, "Cycle(I) [1] (iv)");
        [TestMethod] public void CycleI_P1_i4_Start() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[3].StartPoint, Utc(2021, 1, 12, 18, 0), 3.88m, "Cycle(I) [1] (iv) Start");
        [TestMethod] public void CycleI_P1_i4_End() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[3].EndPoint, Utc(2021, 1, 15, 16, 0), 2.9618m, "Cycle(I) [1] (iv) End");

        [TestMethod] public void CycleI_P1_i5_Label() => AssertLabel(_analysis.Waves[0].SubWaves[0].SubWaves[4], WaveNumber.Five, "Cycle(I) [1] (v)");
        [TestMethod] public void CycleI_P1_i5_Start() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[4].StartPoint, Utc(2021, 1, 15, 16, 0), 2.9618m, "Cycle(I) [1] (v) Start");
        [TestMethod] public void CycleI_P1_i5_End() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[4].EndPoint, Utc(2021, 1, 18, 8, 0), 4.17m, "Cycle(I) [1] (v) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(I) [2]
        //
        // (a): $4.1700 → $3.3400 (20.01.2021)
        // (b): $3.3400 → $3.7413 (20.01.2021)
        // (c): $3.7413 → $2.5295 (22.01.2021)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleI_P2_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[0].SubWaves[1], 3, "Cycle(I) [2] Intermediate");

        [TestMethod] public void CycleI_P2_iA_Label() => AssertLabel(_analysis.Waves[0].SubWaves[1].SubWaves[0], WaveNumber.A, "Cycle(I) [2] (a)");
        [TestMethod] public void CycleI_P2_iA_Start() => AssertPoint(_analysis.Waves[0].SubWaves[1].SubWaves[0].StartPoint, Utc(2021, 1, 18, 8, 0), 4.17m, "Cycle(I) [2] (a) Start");
        [TestMethod] public void CycleI_P2_iA_End() => AssertPoint(_analysis.Waves[0].SubWaves[1].SubWaves[0].EndPoint, Utc(2021, 1, 20, 13, 0), 3.34m, "Cycle(I) [2] (a) End");

        [TestMethod] public void CycleI_P2_iB_Label() => AssertLabel(_analysis.Waves[0].SubWaves[1].SubWaves[1], WaveNumber.B, "Cycle(I) [2] (b)");
        [TestMethod] public void CycleI_P2_iB_Start() => AssertPoint(_analysis.Waves[0].SubWaves[1].SubWaves[1].StartPoint, Utc(2021, 1, 20, 13, 0), 3.34m, "Cycle(I) [2] (b) Start");
        [TestMethod] public void CycleI_P2_iB_End() => AssertPoint(_analysis.Waves[0].SubWaves[1].SubWaves[1].EndPoint, Utc(2021, 1, 20, 23, 0), 3.7413m, "Cycle(I) [2] (b) End");

        [TestMethod] public void CycleI_P2_iC_Label() => AssertLabel(_analysis.Waves[0].SubWaves[1].SubWaves[2], WaveNumber.C, "Cycle(I) [2] (c)");
        [TestMethod] public void CycleI_P2_iC_Start() => AssertPoint(_analysis.Waves[0].SubWaves[1].SubWaves[2].StartPoint, Utc(2021, 1, 20, 23, 0), 3.7413m, "Cycle(I) [2] (c) Start");
        [TestMethod] public void CycleI_P2_iC_End() => AssertPoint(_analysis.Waves[0].SubWaves[1].SubWaves[2].EndPoint, Utc(2021, 1, 22, 1, 0), 2.5295m, "Cycle(I) [2] (c) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(I) [3]
        //
        // (i): $2.5295 → $4.0970 (26.01.2021)
        // (ii): $4.0970 → $3.5114 (27.01.2021)
        // (iii): $3.5114 → $18.2052 (24.02.2021)
        // (iv): $18.2052 → $11.4170 (28.02.2021)
        // (v): $11.4170 → $58.3800 (18.05.2021)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleI_P3_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[0].SubWaves[2], 5, "Cycle(I) [3] Intermediate");

        [TestMethod] public void CycleI_P3_i1_Label() => AssertLabel(_analysis.Waves[0].SubWaves[2].SubWaves[0], WaveNumber.One, "Cycle(I) [3] (i)");
        [TestMethod] public void CycleI_P3_i1_Start() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[0].StartPoint, Utc(2021, 1, 22, 1, 0), 2.5295m, "Cycle(I) [3] (i) Start");
        [TestMethod] public void CycleI_P3_i1_End() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[0].EndPoint, Utc(2021, 1, 26, 20, 0), 4.097m, "Cycle(I) [3] (i) End");

        [TestMethod] public void CycleI_P3_i2_Label() => AssertLabel(_analysis.Waves[0].SubWaves[2].SubWaves[1], WaveNumber.Two, "Cycle(I) [3] (ii)");
        [TestMethod] public void CycleI_P3_i2_Start() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[1].StartPoint, Utc(2021, 1, 26, 20, 0), 4.097m, "Cycle(I) [3] (ii) Start");
        [TestMethod] public void CycleI_P3_i2_End() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[1].EndPoint, Utc(2021, 1, 27, 14, 0), 3.5114m, "Cycle(I) [3] (ii) End");

        [TestMethod] public void CycleI_P3_i3_Label() => AssertLabel(_analysis.Waves[0].SubWaves[2].SubWaves[2], WaveNumber.Three, "Cycle(I) [3] (iii)");
        [TestMethod] public void CycleI_P3_i3_Start() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[2].StartPoint, Utc(2021, 1, 27, 14, 0), 3.5114m, "Cycle(I) [3] (iii) Start");
        [TestMethod] public void CycleI_P3_i3_End() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[2].EndPoint, Utc(2021, 2, 24, 18, 0), 18.2052m, "Cycle(I) [3] (iii) End");

        [TestMethod] public void CycleI_P3_i4_Label() => AssertLabel(_analysis.Waves[0].SubWaves[2].SubWaves[3], WaveNumber.Four, "Cycle(I) [3] (iv)");
        [TestMethod] public void CycleI_P3_i4_Start() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[3].StartPoint, Utc(2021, 2, 24, 18, 0), 18.2052m, "Cycle(I) [3] (iv) Start");
        [TestMethod] public void CycleI_P3_i4_End() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[3].EndPoint, Utc(2021, 2, 28, 15, 0), 11.417m, "Cycle(I) [3] (iv) End");

        [TestMethod] public void CycleI_P3_i5_Label() => AssertLabel(_analysis.Waves[0].SubWaves[2].SubWaves[4], WaveNumber.Five, "Cycle(I) [3] (v)");
        [TestMethod] public void CycleI_P3_i5_Start() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[4].StartPoint, Utc(2021, 2, 28, 15, 0), 11.417m, "Cycle(I) [3] (v) Start");
        [TestMethod] public void CycleI_P3_i5_End() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[4].EndPoint, Utc(2021, 5, 18, 16, 0), 58.38m, "Cycle(I) [3] (v) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(I) [4]
        //
        // (a): $58.3800 → $26.5000 (19.05.2021)
        // (b): $26.5000 → $53.0000 (20.05.2021)
        // (c): $53.0000 → $19.1170 (23.05.2021)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleI_P4_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[0].SubWaves[3], 3, "Cycle(I) [4] Intermediate");

        [TestMethod] public void CycleI_P4_iA_Label() => AssertLabel(_analysis.Waves[0].SubWaves[3].SubWaves[0], WaveNumber.A, "Cycle(I) [4] (a)");
        [TestMethod] public void CycleI_P4_iA_Start() => AssertPoint(_analysis.Waves[0].SubWaves[3].SubWaves[0].StartPoint, Utc(2021, 5, 18, 16, 0), 58.38m, "Cycle(I) [4] (a) Start");
        [TestMethod] public void CycleI_P4_iA_End() => AssertPoint(_analysis.Waves[0].SubWaves[3].SubWaves[0].EndPoint, Utc(2021, 5, 19, 12, 0), 26.5m, "Cycle(I) [4] (a) End");

        [TestMethod] public void CycleI_P4_iB_Label() => AssertLabel(_analysis.Waves[0].SubWaves[3].SubWaves[1], WaveNumber.B, "Cycle(I) [4] (b)");
        [TestMethod] public void CycleI_P4_iB_Start() => AssertPoint(_analysis.Waves[0].SubWaves[3].SubWaves[1].StartPoint, Utc(2021, 5, 19, 12, 0), 26.5m, "Cycle(I) [4] (b) Start");
        [TestMethod] public void CycleI_P4_iB_End() => AssertPoint(_analysis.Waves[0].SubWaves[3].SubWaves[1].EndPoint, Utc(2021, 5, 20, 17, 0), 53m, "Cycle(I) [4] (b) End");

        [TestMethod] public void CycleI_P4_iC_Label() => AssertLabel(_analysis.Waves[0].SubWaves[3].SubWaves[2], WaveNumber.C, "Cycle(I) [4] (c)");
        [TestMethod] public void CycleI_P4_iC_Start() => AssertPoint(_analysis.Waves[0].SubWaves[3].SubWaves[2].StartPoint, Utc(2021, 5, 20, 17, 0), 53m, "Cycle(I) [4] (c) Start");
        [TestMethod] public void CycleI_P4_iC_End() => AssertPoint(_analysis.Waves[0].SubWaves[3].SubWaves[2].EndPoint, Utc(2021, 5, 23, 16, 0), 19.117m, "Cycle(I) [4] (c) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(I) [5]
        //
        // (i): $19.1170 → $44.2000 (07.06.2021)
        // (ii): $44.2000 → $20.1870 (22.06.2021)
        // (iii): $20.1870 → $216.00 (09.09.2021)
        // (iv): $216.00 → $116.00 (21.09.2021)
        // (v): $116.00 → $259.90 (06.11.2021)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleI_P5_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[0].SubWaves[4], 5, "Cycle(I) [5] Intermediate");

        [TestMethod] public void CycleI_P5_i1_Label() => AssertLabel(_analysis.Waves[0].SubWaves[4].SubWaves[0], WaveNumber.One, "Cycle(I) [5] (i)");
        [TestMethod] public void CycleI_P5_i1_Start() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[0].StartPoint, Utc(2021, 5, 23, 16, 0), 19.117m, "Cycle(I) [5] (i) Start");
        [TestMethod] public void CycleI_P5_i1_End() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[0].EndPoint, Utc(2021, 6, 7, 8, 0), 44.2m, "Cycle(I) [5] (i) End");

        [TestMethod] public void CycleI_P5_i2_Label() => AssertLabel(_analysis.Waves[0].SubWaves[4].SubWaves[1], WaveNumber.Two, "Cycle(I) [5] (ii)");
        [TestMethod] public void CycleI_P5_i2_Start() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[1].StartPoint, Utc(2021, 6, 7, 8, 0), 44.2m, "Cycle(I) [5] (ii) Start");
        [TestMethod] public void CycleI_P5_i2_End() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[1].EndPoint, Utc(2021, 6, 22, 13, 0), 20.187m, "Cycle(I) [5] (ii) End");

        [TestMethod] public void CycleI_P5_i3_Label() => AssertLabel(_analysis.Waves[0].SubWaves[4].SubWaves[2], WaveNumber.Three, "Cycle(I) [5] (iii)");
        [TestMethod] public void CycleI_P5_i3_Start() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[2].StartPoint, Utc(2021, 6, 22, 13, 0), 20.187m, "Cycle(I) [5] (iii) Start");
        [TestMethod] public void CycleI_P5_i3_End() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[2].EndPoint, Utc(2021, 9, 9, 2, 0), 216m, "Cycle(I) [5] (iii) End");

        [TestMethod] public void CycleI_P5_i4_Label() => AssertLabel(_analysis.Waves[0].SubWaves[4].SubWaves[3], WaveNumber.Four, "Cycle(I) [5] (iv)");
        [TestMethod] public void CycleI_P5_i4_Start() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[3].StartPoint, Utc(2021, 9, 9, 2, 0), 216m, "Cycle(I) [5] (iv) Start");
        [TestMethod] public void CycleI_P5_i4_End() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[3].EndPoint, Utc(2021, 9, 21, 21, 0), 116m, "Cycle(I) [5] (iv) End");

        [TestMethod] public void CycleI_P5_i5_Label() => AssertLabel(_analysis.Waves[0].SubWaves[4].SubWaves[4], WaveNumber.Five, "Cycle(I) [5] (v)");
        [TestMethod] public void CycleI_P5_i5_Start() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[4].StartPoint, Utc(2021, 9, 21, 21, 0), 116m, "Cycle(I) [5] (v) Start");
        [TestMethod] public void CycleI_P5_i5_End() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[4].EndPoint, Utc(2021, 11, 6, 21, 0), 259.9m, "Cycle(I) [5] (v) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(II) [A]
        //
        // (i): $259.90 → $169.00 (04.12.2021)
        // (ii): $169.00 → $210.52 (04.12.2021)
        // (iii): $210.52 → $37.3700 (12.05.2022)
        // (iv): $37.3700 → $59.3100 (15.05.2022)
        // (v): $59.3100 → $25.8600 (14.06.2022)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleII_PA_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[1].SubWaves[0], 5, "Cycle(II) [A] Intermediate");

        [TestMethod] public void CycleII_PA_i1_Label() => AssertLabel(_analysis.Waves[1].SubWaves[0].SubWaves[0], WaveNumber.One, "Cycle(II) [A] (i)");
        [TestMethod] public void CycleII_PA_i1_Start() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[0].StartPoint, Utc(2021, 11, 6, 21, 0), 259.9m, "Cycle(II) [A] (i) Start");
        [TestMethod] public void CycleII_PA_i1_End() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[0].EndPoint, Utc(2021, 12, 4, 5, 0), 169m, "Cycle(II) [A] (i) End");

        [TestMethod] public void CycleII_PA_i2_Label() => AssertLabel(_analysis.Waves[1].SubWaves[0].SubWaves[1], WaveNumber.Two, "Cycle(II) [A] (ii)");
        [TestMethod] public void CycleII_PA_i2_Start() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[1].StartPoint, Utc(2021, 12, 4, 5, 0), 169m, "Cycle(II) [A] (ii) Start");
        [TestMethod] public void CycleII_PA_i2_End() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[1].EndPoint, Utc(2021, 12, 4, 20, 0), 210.52m, "Cycle(II) [A] (ii) End");

        [TestMethod] public void CycleII_PA_i3_Label() => AssertLabel(_analysis.Waves[1].SubWaves[0].SubWaves[2], WaveNumber.Three, "Cycle(II) [A] (iii)");
        [TestMethod] public void CycleII_PA_i3_Start() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[2].StartPoint, Utc(2021, 12, 4, 20, 0), 210.52m, "Cycle(II) [A] (iii) Start");
        [TestMethod] public void CycleII_PA_i3_End() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[2].EndPoint, Utc(2022, 5, 12, 7, 0), 37.37m, "Cycle(II) [A] (iii) End");

        [TestMethod] public void CycleII_PA_i4_Label() => AssertLabel(_analysis.Waves[1].SubWaves[0].SubWaves[3], WaveNumber.Four, "Cycle(II) [A] (iv)");
        [TestMethod] public void CycleII_PA_i4_Start() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[3].StartPoint, Utc(2022, 5, 12, 7, 0), 37.37m, "Cycle(II) [A] (iv) Start");
        [TestMethod] public void CycleII_PA_i4_End() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[3].EndPoint, Utc(2022, 5, 15, 23, 0), 59.31m, "Cycle(II) [A] (iv) End");

        [TestMethod] public void CycleII_PA_i5_Label() => AssertLabel(_analysis.Waves[1].SubWaves[0].SubWaves[4], WaveNumber.Five, "Cycle(II) [A] (v)");
        [TestMethod] public void CycleII_PA_i5_Start() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[4].StartPoint, Utc(2022, 5, 15, 23, 0), 59.31m, "Cycle(II) [A] (v) Start");
        [TestMethod] public void CycleII_PA_i5_End() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[4].EndPoint, Utc(2022, 6, 14, 2, 0), 25.86m, "Cycle(II) [A] (v) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(II) [B]
        //
        // (a): $25.8600 → $42.9200 (24.06.2022)
        // (b): $42.9200 → $30.9200 (30.06.2022)
        // (c): $30.9200 → $48.3800 (13.08.2022)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleII_PB_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[1].SubWaves[1], 3, "Cycle(II) [B] Intermediate");

        [TestMethod] public void CycleII_PB_iA_Label() => AssertLabel(_analysis.Waves[1].SubWaves[1].SubWaves[0], WaveNumber.A, "Cycle(II) [B] (a)");
        [TestMethod] public void CycleII_PB_iA_Start() => AssertPoint(_analysis.Waves[1].SubWaves[1].SubWaves[0].StartPoint, Utc(2022, 6, 14, 2, 0), 25.86m, "Cycle(II) [B] (a) Start");
        [TestMethod] public void CycleII_PB_iA_End() => AssertPoint(_analysis.Waves[1].SubWaves[1].SubWaves[0].EndPoint, Utc(2022, 6, 24, 23, 0), 42.92m, "Cycle(II) [B] (a) End");

        [TestMethod] public void CycleII_PB_iB_Label() => AssertLabel(_analysis.Waves[1].SubWaves[1].SubWaves[1], WaveNumber.B, "Cycle(II) [B] (b)");
        [TestMethod] public void CycleII_PB_iB_Start() => AssertPoint(_analysis.Waves[1].SubWaves[1].SubWaves[1].StartPoint, Utc(2022, 6, 24, 23, 0), 42.92m, "Cycle(II) [B] (b) Start");
        [TestMethod] public void CycleII_PB_iB_End() => AssertPoint(_analysis.Waves[1].SubWaves[1].SubWaves[1].EndPoint, Utc(2022, 6, 30, 13, 0), 30.92m, "Cycle(II) [B] (b) End");

        [TestMethod] public void CycleII_PB_iC_Label() => AssertLabel(_analysis.Waves[1].SubWaves[1].SubWaves[2], WaveNumber.C, "Cycle(II) [B] (c)");
        [TestMethod] public void CycleII_PB_iC_Start() => AssertPoint(_analysis.Waves[1].SubWaves[1].SubWaves[2].StartPoint, Utc(2022, 6, 30, 13, 0), 30.92m, "Cycle(II) [B] (c) Start");
        [TestMethod] public void CycleII_PB_iC_End() => AssertPoint(_analysis.Waves[1].SubWaves[1].SubWaves[2].EndPoint, Utc(2022, 8, 13, 6, 0), 48.38m, "Cycle(II) [B] (c) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(II) [C]
        //
        // (i): $48.3800 → $30.0000 (29.08.2022)
        // (ii): $30.0000 → $39.0100 (13.09.2022)
        // (iii): $39.0100 → $10.9400 (22.11.2022)
        // (iv): $10.9400 → $14.9800 (26.11.2022)
        // (v): $14.9800 → $8.0000 (29.12.2022)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleII_PC_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[1].SubWaves[2], 5, "Cycle(II) [C] Intermediate");

        [TestMethod] public void CycleII_PC_i1_Label() => AssertLabel(_analysis.Waves[1].SubWaves[2].SubWaves[0], WaveNumber.One, "Cycle(II) [C] (i)");
        [TestMethod] public void CycleII_PC_i1_Start() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[0].StartPoint, Utc(2022, 8, 13, 6, 0), 48.38m, "Cycle(II) [C] (i) Start");
        [TestMethod] public void CycleII_PC_i1_End() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[0].EndPoint, Utc(2022, 8, 29, 0, 0), 30m, "Cycle(II) [C] (i) End");

        [TestMethod] public void CycleII_PC_i2_Label() => AssertLabel(_analysis.Waves[1].SubWaves[2].SubWaves[1], WaveNumber.Two, "Cycle(II) [C] (ii)");
        [TestMethod] public void CycleII_PC_i2_Start() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[1].StartPoint, Utc(2022, 8, 29, 0, 0), 30m, "Cycle(II) [C] (ii) Start");
        [TestMethod] public void CycleII_PC_i2_End() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[1].EndPoint, Utc(2022, 9, 13, 6, 0), 39.01m, "Cycle(II) [C] (ii) End");

        [TestMethod] public void CycleII_PC_i3_Label() => AssertLabel(_analysis.Waves[1].SubWaves[2].SubWaves[2], WaveNumber.Three, "Cycle(II) [C] (iii)");
        [TestMethod] public void CycleII_PC_i3_Start() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[2].StartPoint, Utc(2022, 9, 13, 6, 0), 39.01m, "Cycle(II) [C] (iii) Start");
        [TestMethod] public void CycleII_PC_i3_End() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[2].EndPoint, Utc(2022, 11, 22, 12, 0), 10.94m, "Cycle(II) [C] (iii) End");

        [TestMethod] public void CycleII_PC_i4_Label() => AssertLabel(_analysis.Waves[1].SubWaves[2].SubWaves[3], WaveNumber.Four, "Cycle(II) [C] (iv)");
        [TestMethod] public void CycleII_PC_i4_Start() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[3].StartPoint, Utc(2022, 11, 22, 12, 0), 10.94m, "Cycle(II) [C] (iv) Start");
        [TestMethod] public void CycleII_PC_i4_End() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[3].EndPoint, Utc(2022, 11, 26, 10, 0), 14.98m, "Cycle(II) [C] (iv) End");

        [TestMethod] public void CycleII_PC_i5_Label() => AssertLabel(_analysis.Waves[1].SubWaves[2].SubWaves[4], WaveNumber.Five, "Cycle(II) [C] (v)");
        [TestMethod] public void CycleII_PC_i5_Start() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[4].StartPoint, Utc(2022, 11, 26, 10, 0), 14.98m, "Cycle(II) [C] (v) Start");
        [TestMethod] public void CycleII_PC_i5_End() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[4].EndPoint, Utc(2022, 12, 29, 20, 0), 8m, "Cycle(II) [C] (v) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(III) [1]
        //
        // (i): $8.0000 → $27.1200 (20.02.2023)
        // (ii): $27.1200 → $12.8000 (10.06.2023)
        // (iii): $12.8000 → $210.18 (18.03.2024)
        // (iv): $210.18 → $110.00 (05.08.2024)
        // (v): $110.00 → $295.83 (19.01.2025)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleIII_P1_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[2].SubWaves[0], 5, "Cycle(III) [1] Intermediate");

        [TestMethod] public void CycleIII_P1_i1_Label() => AssertLabel(_analysis.Waves[2].SubWaves[0].SubWaves[0], WaveNumber.One, "Cycle(III) [1] (i)");
        [TestMethod] public void CycleIII_P1_i1_Start() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[0].StartPoint, Utc(2022, 12, 29, 20, 0), 8m, "Cycle(III) [1] (i) Start");
        [TestMethod] public void CycleIII_P1_i1_End() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[0].EndPoint, Utc(2023, 2, 20, 7, 0), 27.12m, "Cycle(III) [1] (i) End");

        [TestMethod] public void CycleIII_P1_i2_Label() => AssertLabel(_analysis.Waves[2].SubWaves[0].SubWaves[1], WaveNumber.Two, "Cycle(III) [1] (ii)");
        [TestMethod] public void CycleIII_P1_i2_Start() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[1].StartPoint, Utc(2023, 2, 20, 7, 0), 27.12m, "Cycle(III) [1] (ii) Start");
        [TestMethod] public void CycleIII_P1_i2_End() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[1].EndPoint, Utc(2023, 6, 10, 4, 0), 12.8m, "Cycle(III) [1] (ii) End");

        [TestMethod] public void CycleIII_P1_i3_Label() => AssertLabel(_analysis.Waves[2].SubWaves[0].SubWaves[2], WaveNumber.Three, "Cycle(III) [1] (iii)");
        [TestMethod] public void CycleIII_P1_i3_Start() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[2].StartPoint, Utc(2023, 6, 10, 4, 0), 12.8m, "Cycle(III) [1] (iii) Start");
        [TestMethod] public void CycleIII_P1_i3_End() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[2].EndPoint, Utc(2024, 3, 18, 10, 0), 210.18m, "Cycle(III) [1] (iii) End");

        [TestMethod] public void CycleIII_P1_i4_Label() => AssertLabel(_analysis.Waves[2].SubWaves[0].SubWaves[3], WaveNumber.Four, "Cycle(III) [1] (iv)");
        [TestMethod] public void CycleIII_P1_i4_Start() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[3].StartPoint, Utc(2024, 3, 18, 10, 0), 210.18m, "Cycle(III) [1] (iv) Start");
        [TestMethod] public void CycleIII_P1_i4_End() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[3].EndPoint, Utc(2024, 8, 5, 6, 0), 110m, "Cycle(III) [1] (iv) End");

        [TestMethod] public void CycleIII_P1_i5_Label() => AssertLabel(_analysis.Waves[2].SubWaves[0].SubWaves[4], WaveNumber.Five, "Cycle(III) [1] (v)");
        [TestMethod] public void CycleIII_P1_i5_Start() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[4].StartPoint, Utc(2024, 8, 5, 6, 0), 110m, "Cycle(III) [1] (v) Start");
        [TestMethod] public void CycleIII_P1_i5_End() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[4].EndPoint, Utc(2025, 1, 19, 11, 0), 295.83m, "Cycle(III) [1] (v) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(III) [2]
        //
        // (a): $295.83 → $95.2600 (07.04.2025)
        // (b): $95.2600 → $253.51 (18.09.2025)
        // (c): $253.51 → $67.5000 (06.02.2026)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleIII_P2_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[2].SubWaves[1], 3, "Cycle(III) [2] Intermediate");

        [TestMethod] public void CycleIII_P2_iA_Label() => AssertLabel(_analysis.Waves[2].SubWaves[1].SubWaves[0], WaveNumber.A, "Cycle(III) [2] (a)");
        [TestMethod] public void CycleIII_P2_iA_Start() => AssertPoint(_analysis.Waves[2].SubWaves[1].SubWaves[0].StartPoint, Utc(2025, 1, 19, 11, 0), 295.83m, "Cycle(III) [2] (a) Start");
        [TestMethod] public void CycleIII_P2_iA_End() => AssertPoint(_analysis.Waves[2].SubWaves[1].SubWaves[0].EndPoint, Utc(2025, 4, 7, 6, 0), 95.26m, "Cycle(III) [2] (a) End");

        [TestMethod] public void CycleIII_P2_iB_Label() => AssertLabel(_analysis.Waves[2].SubWaves[1].SubWaves[1], WaveNumber.B, "Cycle(III) [2] (b)");
        [TestMethod] public void CycleIII_P2_iB_Start() => AssertPoint(_analysis.Waves[2].SubWaves[1].SubWaves[1].StartPoint, Utc(2025, 4, 7, 6, 0), 95.26m, "Cycle(III) [2] (b) Start");
        [TestMethod] public void CycleIII_P2_iB_End() => AssertPoint(_analysis.Waves[2].SubWaves[1].SubWaves[1].EndPoint, Utc(2025, 9, 18, 17, 0), 253.51m, "Cycle(III) [2] (b) End");

        [TestMethod] public void CycleIII_P2_iC_Label() => AssertLabel(_analysis.Waves[2].SubWaves[1].SubWaves[2], WaveNumber.C, "Cycle(III) [2] (c)");
        [TestMethod] public void CycleIII_P2_iC_Start() => AssertPoint(_analysis.Waves[2].SubWaves[1].SubWaves[2].StartPoint, Utc(2025, 9, 18, 17, 0), 253.51m, "Cycle(III) [2] (c) Start");
        [TestMethod] public void CycleIII_P2_iC_End() => AssertPoint(_analysis.Waves[2].SubWaves[1].SubWaves[2].EndPoint, Utc(2026, 2, 6, 0, 0), 67.5m, "Cycle(III) [2] (c) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(III) [3]
        //
        // (i): $67.5000 → $97.6800 (16.03.2026)
        // (ii): $97.6800 → $87.0000 (19.03.2026)
        // (iii): $87.0000 → $87.0000 (19.03.2026) (in-progress)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleIII_P3_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[2].SubWaves[2], 3, "Cycle(III) [3] Intermediate");

        [TestMethod] public void CycleIII_P3_i1_Label() => AssertLabel(_analysis.Waves[2].SubWaves[2].SubWaves[0], WaveNumber.One, "Cycle(III) [3] (i)");
        [TestMethod] public void CycleIII_P3_i1_Start() => AssertPoint(_analysis.Waves[2].SubWaves[2].SubWaves[0].StartPoint, Utc(2026, 2, 6, 0, 0), 67.5m, "Cycle(III) [3] (i) Start");
        [TestMethod] public void CycleIII_P3_i1_End() => AssertPoint(_analysis.Waves[2].SubWaves[2].SubWaves[0].EndPoint, Utc(2026, 3, 16, 22, 0), 97.68m, "Cycle(III) [3] (i) End");

        [TestMethod] public void CycleIII_P3_i2_Label() => AssertLabel(_analysis.Waves[2].SubWaves[2].SubWaves[1], WaveNumber.Two, "Cycle(III) [3] (ii)");
        [TestMethod] public void CycleIII_P3_i2_Start() => AssertPoint(_analysis.Waves[2].SubWaves[2].SubWaves[1].StartPoint, Utc(2026, 3, 16, 22, 0), 97.68m, "Cycle(III) [3] (ii) Start");
        [TestMethod] public void CycleIII_P3_i2_End() => AssertPoint(_analysis.Waves[2].SubWaves[2].SubWaves[1].EndPoint, Utc(2026, 3, 19, 16, 0), 87m, "Cycle(III) [3] (ii) End");

        [TestMethod] public void CycleIII_P3_i3_Label() => AssertLabel(_analysis.Waves[2].SubWaves[2].SubWaves[2], WaveNumber.Three, "Cycle(III) [3] (iii)");
        [TestMethod] public void CycleIII_P3_i3_InProgress() => AssertInProgress(_analysis.Waves[2].SubWaves[2].SubWaves[2], true, "Cycle(III) [3] (iii)");
        [TestMethod] public void CycleIII_P3_i3_Start() => AssertPoint(_analysis.Waves[2].SubWaves[2].SubWaves[2].StartPoint, Utc(2026, 3, 19, 16, 0), 87m, "Cycle(III) [3] (iii) Start");

        // ══════════════════════════════════════════════════
        // Projections — Cycle (III): W3 extension of Cycle I ($1.03 → $259.90) from Cycle II end ($8.00)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleIII_Projection_4236() => AssertProjectionTarget(_analysis.Waves[2], LogExtension(1.03010000m, 259.90m, 8.00m, 4.236m), 4.236m, "Cycle(III) 4.236 projection");
        [TestMethod] public void CycleIII_Projection_2618() => AssertProjectionTarget(_analysis.Waves[2], LogExtension(1.03010000m, 259.90m, 8.00m, 2.618m), 2.618m, "Cycle(III) 2.618 projection");
        [TestMethod] public void CycleIII_Projection_1618() => AssertProjectionTarget(_analysis.Waves[2], LogExtension(1.03010000m, 259.90m, 8.00m, 1.618m), 1.618m, "Cycle(III) 1.618 projection");
        [TestMethod] public void CycleIII_Projection_100() => AssertProjectionTarget(_analysis.Waves[2], LogExtension(1.03010000m, 259.90m, 8.00m, 1.0m), 1.0m, "Cycle(III) 1.0 projection");

        // ══════════════════════════════════════════════════
        // Projections — Primary [3] inside Cycle (III): W3 extension of P1 ($8.00 → $295.83) from P2 end ($67.50)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleIII_P3_Projection_4236() => AssertProjectionTarget(_analysis.Waves[2].SubWaves[2], LogExtension(8.00m, 295.83m, 67.50m, 4.236m), 4.236m, "Cycle(III) [3] 4.236 projection");
        [TestMethod] public void CycleIII_P3_Projection_2618() => AssertProjectionTarget(_analysis.Waves[2].SubWaves[2], LogExtension(8.00m, 295.83m, 67.50m, 2.618m), 2.618m, "Cycle(III) [3] 2.618 projection");
        [TestMethod] public void CycleIII_P3_Projection_1618() => AssertProjectionTarget(_analysis.Waves[2].SubWaves[2], LogExtension(8.00m, 295.83m, 67.50m, 1.618m), 1.618m, "Cycle(III) [3] 1.618 projection");
        [TestMethod] public void CycleIII_P3_Projection_100() => AssertProjectionTarget(_analysis.Waves[2].SubWaves[2], LogExtension(8.00m, 295.83m, 67.50m, 1.0m), 1.0m, "Cycle(III) [3] 1.0 projection");
    }
}
