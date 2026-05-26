using SmarterSystems.ElliottWaves.Analyzer.Interfaces;

namespace SmarterSystems.ElliottWaves.Validation
{
    [TestClass]
    public sealed class EthRegressionTest : RegressionTestBase
    {
        private static ElliottWavesAnalysis _analysis = null!;

        [ClassInitialize]
        public static void Setup(TestContext _)
        {
            _analysis = LoadAndAnalyze("ETHUSDT");
        }

        // ══════════════════════════════════════════════════
        // Cycle Degree: 3 waves (I, II completed; III in-progress)
        //
        // Cycle (I):   15.12.2018 $81.79 → 10.11.2021 $4,868.00
        // Cycle (II):  10.11.2021 $4,868.00 → 18.06.2022 $881.56
        // Cycle (III): 18.06.2022 $881.56 → in-progress
        // ══════════════════════════════════════════════════

        [TestMethod] public void Cycle_WaveCount() => AssertWaveCount(_analysis, 3, "ETH Cycle");

        // ── Cycle (I): $81.79 → $4,868.00 ──

        [TestMethod] public void CycleI_Label() => AssertLabel(_analysis.Waves[0], WaveNumber.One, "Cycle(I)");
        [TestMethod] public void CycleI_NotInProgress() => AssertInProgress(_analysis.Waves[0], false, "Cycle(I)");
        [TestMethod] public void CycleI_Start() => AssertPoint(_analysis.Waves[0].StartPoint, Utc(2018, 12, 15, 15), 81.79m, "Cycle(I) Start");
        [TestMethod] public void CycleI_End() => AssertPoint(_analysis.Waves[0].EndPoint, Utc(2021, 11, 10, 14), 4868.00m, "Cycle(I) End");

        // ── Cycle (II): $4,868.00 → $881.56 ──

        [TestMethod] public void CycleII_Label() => AssertLabel(_analysis.Waves[1], WaveNumber.Two, "Cycle(II)");
        [TestMethod] public void CycleII_NotInProgress() => AssertInProgress(_analysis.Waves[1], false, "Cycle(II)");
        [TestMethod] public void CycleII_Start() => AssertPoint(_analysis.Waves[1].StartPoint, Utc(2021, 11, 10, 14), 4868.00m, "Cycle(II) Start");
        [TestMethod] public void CycleII_End() => AssertPoint(_analysis.Waves[1].EndPoint, Utc(2022, 6, 18, 20), 881.56m, "Cycle(II) End");

        // ── Cycle (III): $881.56 → in-progress ──

        [TestMethod] public void CycleIII_Label() => AssertLabel(_analysis.Waves[2], WaveNumber.Three, "Cycle(III)");
        [TestMethod] public void CycleIII_InProgress() => AssertInProgress(_analysis.Waves[2], true, "Cycle(III)");
        [TestMethod] public void CycleIII_Start() => AssertPoint(_analysis.Waves[2].StartPoint, Utc(2022, 6, 18, 20), 881.56m, "Cycle(III) Start");

        // ══════════════════════════════════════════════════
        // Primary Degree — inside Cycle (I): 5 waves (impulse)
        //
        // [1]: $81.79 → $366.80 (26.06.2019)
        // [2]: $366.80 → $86.00 (13.03.2020)
        // [3]: $86.00 → $4,372.72 (12.05.2021)
        // [4]: $4,372.72 → $1,706.00 (20.07.2021)
        // [5]: $1,706.00 → $4,868.00 (10.11.2021)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleI_Primary_WaveCount() => AssertSubWaveCount(_analysis.Waves[0], 5, "Cycle(I) Primary");

        [TestMethod] public void CycleI_P1_Label() => AssertLabel(_analysis.Waves[0].SubWaves[0], WaveNumber.One, "Cycle(I) [1]");
        [TestMethod] public void CycleI_P1_Start() => AssertPoint(_analysis.Waves[0].SubWaves[0].StartPoint, Utc(2018, 12, 15, 15), 81.79m, "Cycle(I) [1] Start");
        [TestMethod] public void CycleI_P1_End() => AssertPoint(_analysis.Waves[0].SubWaves[0].EndPoint, Utc(2019, 6, 26, 20), 366.80m, "Cycle(I) [1] End");

        [TestMethod] public void CycleI_P2_Label() => AssertLabel(_analysis.Waves[0].SubWaves[1], WaveNumber.Two, "Cycle(I) [2]");
        [TestMethod] public void CycleI_P2_Start() => AssertPoint(_analysis.Waves[0].SubWaves[1].StartPoint, Utc(2019, 6, 26, 20), 366.80m, "Cycle(I) [2] Start");
        [TestMethod] public void CycleI_P2_End() => AssertPoint(_analysis.Waves[0].SubWaves[1].EndPoint, Utc(2020, 3, 13, 2), 86.00m, "Cycle(I) [2] End");

        [TestMethod] public void CycleI_P3_Label() => AssertLabel(_analysis.Waves[0].SubWaves[2], WaveNumber.Three, "Cycle(I) [3]");
        [TestMethod] public void CycleI_P3_Start() => AssertPoint(_analysis.Waves[0].SubWaves[2].StartPoint, Utc(2020, 3, 13, 2), 86.00m, "Cycle(I) [3] Start");
        [TestMethod] public void CycleI_P3_End() => AssertPoint(_analysis.Waves[0].SubWaves[2].EndPoint, Utc(2021, 5, 12, 14), 4372.72m, "Cycle(I) [3] End");

        [TestMethod] public void CycleI_P4_Label() => AssertLabel(_analysis.Waves[0].SubWaves[3], WaveNumber.Four, "Cycle(I) [4]");
        [TestMethod] public void CycleI_P4_Start() => AssertPoint(_analysis.Waves[0].SubWaves[3].StartPoint, Utc(2021, 5, 12, 14), 4372.72m, "Cycle(I) [4] Start");
        [TestMethod] public void CycleI_P4_End() => AssertPoint(_analysis.Waves[0].SubWaves[3].EndPoint, Utc(2021, 6, 22, 13), 1700.48m, "Cycle(I) [4] End");

        [TestMethod] public void CycleI_P5_Label() => AssertLabel(_analysis.Waves[0].SubWaves[4], WaveNumber.Five, "Cycle(I) [5]");
        [TestMethod] public void CycleI_P5_Start() => AssertPoint(_analysis.Waves[0].SubWaves[4].StartPoint, Utc(2021, 6, 22, 13), 1700.48m, "Cycle(I) [5] Start");
        [TestMethod] public void CycleI_P5_End() => AssertPoint(_analysis.Waves[0].SubWaves[4].EndPoint, Utc(2021, 11, 10, 14), 4868.00m, "Cycle(I) [5] End");

        // ══════════════════════════════════════════════════
        // Primary Degree — inside Cycle (II): A-B-C (corrective)
        //
        // [A]: $4,868.00 → $2,159.00 (24.01.2022)
        // [B]: $2,159.00 → $3,580.34 (03.04.2022)
        // [C]: $3,580.34 → $881.56 (18.06.2022)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleII_Primary_WaveCount() => AssertSubWaveCount(_analysis.Waves[1], 3, "Cycle(II) Primary");

        [TestMethod] public void CycleII_A_Label() => AssertLabel(_analysis.Waves[1].SubWaves[0], WaveNumber.A, "Cycle(II) [A]");
        [TestMethod] public void CycleII_A_Start() => AssertPoint(_analysis.Waves[1].SubWaves[0].StartPoint, Utc(2021, 11, 10, 14), 4868.00m, "Cycle(II) [A] Start");
        [TestMethod] public void CycleII_A_End() => AssertPoint(_analysis.Waves[1].SubWaves[0].EndPoint, Utc(2022, 1, 24, 15), 2159.00m, "Cycle(II) [A] End");

        [TestMethod] public void CycleII_B_Label() => AssertLabel(_analysis.Waves[1].SubWaves[1], WaveNumber.B, "Cycle(II) [B]");
        [TestMethod] public void CycleII_B_Start() => AssertPoint(_analysis.Waves[1].SubWaves[1].StartPoint, Utc(2022, 1, 24, 15), 2159.00m, "Cycle(II) [B] Start");
        [TestMethod] public void CycleII_B_End() => AssertPoint(_analysis.Waves[1].SubWaves[1].EndPoint, Utc(2022, 4, 3, 22), 3580.34m, "Cycle(II) [B] End");

        [TestMethod] public void CycleII_C_Label() => AssertLabel(_analysis.Waves[1].SubWaves[2], WaveNumber.C, "Cycle(II) [C]");
        [TestMethod] public void CycleII_C_Start() => AssertPoint(_analysis.Waves[1].SubWaves[2].StartPoint, Utc(2022, 4, 3, 22), 3580.34m, "Cycle(II) [C] Start");
        [TestMethod] public void CycleII_C_End() => AssertPoint(_analysis.Waves[1].SubWaves[2].EndPoint, Utc(2022, 6, 18, 20), 881.56m, "Cycle(II) [C] End");

        // ══════════════════════════════════════════════════
        // Primary Degree — inside Cycle (III): in-progress
        //
        // [1]: $881.56 → $4,797.97 (25.08.2025)
        // [2]: $4,797.97 → $1,747.80 (06.02.2026)
        // [3]: $1,747.80 → in-progress
        // ══════════════════════════════════════════════════

        [TestMethod]
        public void CycleIII_HasSubWaves()
        {
            Assert.IsNotNull(_analysis.Waves[2].SubWaves, "Cycle(III) should have sub-waves");
            Assert.IsTrue(_analysis.Waves[2].SubWaves.Count >= 2, "Cycle(III) should have at least 2 primary waves");
        }

        [TestMethod] public void CycleIII_P1_Label() => AssertLabel(_analysis.Waves[2].SubWaves[0], WaveNumber.One, "Cycle(III) [1]");
        [TestMethod] public void CycleIII_P1_Start() => AssertPoint(_analysis.Waves[2].SubWaves[0].StartPoint, Utc(2022, 6, 18, 20), 881.56m, "Cycle(III) [1] Start");
        [TestMethod] public void CycleIII_P1_End() => AssertPoint(_analysis.Waves[2].SubWaves[0].EndPoint, Utc(2025, 8, 24, 19), 4956.78m, "Cycle(III) [1] End");

        [TestMethod] public void CycleIII_P2_Label() => AssertLabel(_analysis.Waves[2].SubWaves[1], WaveNumber.Two, "Cycle(III) [2]");
        [TestMethod] public void CycleIII_P2_Start() => AssertPoint(_analysis.Waves[2].SubWaves[1].StartPoint, Utc(2025, 8, 24, 19), 4956.78m, "Cycle(III) [2] Start");
        [TestMethod] public void CycleIII_P2_End() => AssertPoint(_analysis.Waves[2].SubWaves[1].EndPoint, Utc(2026, 2, 6, 0), 1747.80m, "Cycle(III) [2] End");

        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(I) [1]
        //
        // (i): $81.7900 → $159.26 (05.01.2019)
        // (ii): $159.26 → $100.91 (28.01.2019)
        // (iii): $100.91 → $288.62 (30.05.2019)
        // (iv): $288.62 → $226.56 (09.06.2019)
        // (v): $226.56 → $366.80 (26.06.2019)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleI_P1_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[0].SubWaves[0], 5, "Cycle(I) [1] Intermediate");

        [TestMethod] public void CycleI_P1_i1_Label() => AssertLabel(_analysis.Waves[0].SubWaves[0].SubWaves[0], WaveNumber.One, "Cycle(I) [1] (i)");
        [TestMethod] public void CycleI_P1_i1_Start() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[0].StartPoint, Utc(2018, 12, 15, 15, 0), 81.79m, "Cycle(I) [1] (i) Start");
        [TestMethod] public void CycleI_P1_i1_End() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[0].EndPoint, Utc(2019, 1, 5, 2, 0), 159.26m, "Cycle(I) [1] (i) End");

        [TestMethod] public void CycleI_P1_i2_Label() => AssertLabel(_analysis.Waves[0].SubWaves[0].SubWaves[1], WaveNumber.Two, "Cycle(I) [1] (ii)");
        [TestMethod] public void CycleI_P1_i2_Start() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[1].StartPoint, Utc(2019, 1, 5, 2, 0), 159.26m, "Cycle(I) [1] (ii) Start");
        [TestMethod] public void CycleI_P1_i2_End() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[1].EndPoint, Utc(2019, 1, 28, 15, 0), 100.91m, "Cycle(I) [1] (ii) End");

        [TestMethod] public void CycleI_P1_i3_Label() => AssertLabel(_analysis.Waves[0].SubWaves[0].SubWaves[2], WaveNumber.Three, "Cycle(I) [1] (iii)");
        [TestMethod] public void CycleI_P1_i3_Start() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[2].StartPoint, Utc(2019, 1, 28, 15, 0), 100.91m, "Cycle(I) [1] (iii) Start");
        [TestMethod] public void CycleI_P1_i3_End() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[2].EndPoint, Utc(2019, 5, 30, 16, 0), 288.62m, "Cycle(I) [1] (iii) End");

        [TestMethod] public void CycleI_P1_i4_Label() => AssertLabel(_analysis.Waves[0].SubWaves[0].SubWaves[3], WaveNumber.Four, "Cycle(I) [1] (iv)");
        [TestMethod] public void CycleI_P1_i4_Start() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[3].StartPoint, Utc(2019, 5, 30, 16, 0), 288.62m, "Cycle(I) [1] (iv) Start");
        [TestMethod] public void CycleI_P1_i4_End() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[3].EndPoint, Utc(2019, 6, 9, 22, 0), 226.56m, "Cycle(I) [1] (iv) End");

        [TestMethod] public void CycleI_P1_i5_Label() => AssertLabel(_analysis.Waves[0].SubWaves[0].SubWaves[4], WaveNumber.Five, "Cycle(I) [1] (v)");
        [TestMethod] public void CycleI_P1_i5_Start() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[4].StartPoint, Utc(2019, 6, 9, 22, 0), 226.56m, "Cycle(I) [1] (v) Start");
        [TestMethod] public void CycleI_P1_i5_End() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[4].EndPoint, Utc(2019, 6, 26, 20, 0), 366.8m, "Cycle(I) [1] (v) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(I) [2]
        //
        // (a): $366.80 → $116.26 (18.12.2019)
        // (b): $116.26 → $288.41 (15.02.2020)
        // (c): $288.41 → $86.0000 (13.03.2020)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleI_P2_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[0].SubWaves[1], 3, "Cycle(I) [2] Intermediate");

        [TestMethod] public void CycleI_P2_iA_Label() => AssertLabel(_analysis.Waves[0].SubWaves[1].SubWaves[0], WaveNumber.A, "Cycle(I) [2] (a)");
        [TestMethod] public void CycleI_P2_iA_Start() => AssertPoint(_analysis.Waves[0].SubWaves[1].SubWaves[0].StartPoint, Utc(2019, 6, 26, 20, 0), 366.8m, "Cycle(I) [2] (a) Start");
        [TestMethod] public void CycleI_P2_iA_End() => AssertPoint(_analysis.Waves[0].SubWaves[1].SubWaves[0].EndPoint, Utc(2019, 12, 18, 13, 0), 116.26m, "Cycle(I) [2] (a) End");

        [TestMethod] public void CycleI_P2_iB_Label() => AssertLabel(_analysis.Waves[0].SubWaves[1].SubWaves[1], WaveNumber.B, "Cycle(I) [2] (b)");
        [TestMethod] public void CycleI_P2_iB_Start() => AssertPoint(_analysis.Waves[0].SubWaves[1].SubWaves[1].StartPoint, Utc(2019, 12, 18, 13, 0), 116.26m, "Cycle(I) [2] (b) Start");
        [TestMethod] public void CycleI_P2_iB_End() => AssertPoint(_analysis.Waves[0].SubWaves[1].SubWaves[1].EndPoint, Utc(2020, 2, 15, 0, 0), 288.41m, "Cycle(I) [2] (b) End");

        [TestMethod] public void CycleI_P2_iC_Label() => AssertLabel(_analysis.Waves[0].SubWaves[1].SubWaves[2], WaveNumber.C, "Cycle(I) [2] (c)");
        [TestMethod] public void CycleI_P2_iC_Start() => AssertPoint(_analysis.Waves[0].SubWaves[1].SubWaves[2].StartPoint, Utc(2020, 2, 15, 0, 0), 288.41m, "Cycle(I) [2] (c) Start");
        [TestMethod] public void CycleI_P2_iC_End() => AssertPoint(_analysis.Waves[0].SubWaves[1].SubWaves[2].EndPoint, Utc(2020, 3, 13, 2, 0), 86m, "Cycle(I) [2] (c) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(I) [3]
        //
        // (i): $86.0000 → $176.22 (07.04.2020)
        // (ii): $176.22 → $148.33 (16.04.2020)
        // (iii): $148.33 → $2'042.34 (20.02.2021)
        // (iv): $2'042.34 → $1'293.18 (28.02.2021)
        // (v): $1'293.18 → $4'372.72 (12.05.2021)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleI_P3_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[0].SubWaves[2], 5, "Cycle(I) [3] Intermediate");

        [TestMethod] public void CycleI_P3_i1_Label() => AssertLabel(_analysis.Waves[0].SubWaves[2].SubWaves[0], WaveNumber.One, "Cycle(I) [3] (i)");
        [TestMethod] public void CycleI_P3_i1_Start() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[0].StartPoint, Utc(2020, 3, 13, 2, 0), 86m, "Cycle(I) [3] (i) Start");
        [TestMethod] public void CycleI_P3_i1_End() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[0].EndPoint, Utc(2020, 4, 7, 0, 0), 176.22m, "Cycle(I) [3] (i) End");

        [TestMethod] public void CycleI_P3_i2_Label() => AssertLabel(_analysis.Waves[0].SubWaves[2].SubWaves[1], WaveNumber.Two, "Cycle(I) [3] (ii)");
        [TestMethod] public void CycleI_P3_i2_Start() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[1].StartPoint, Utc(2020, 4, 7, 0, 0), 176.22m, "Cycle(I) [3] (ii) Start");
        [TestMethod] public void CycleI_P3_i2_End() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[1].EndPoint, Utc(2020, 4, 16, 0, 0), 148.33m, "Cycle(I) [3] (ii) End");

        [TestMethod] public void CycleI_P3_i3_Label() => AssertLabel(_analysis.Waves[0].SubWaves[2].SubWaves[2], WaveNumber.Three, "Cycle(I) [3] (iii)");
        [TestMethod] public void CycleI_P3_i3_Start() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[2].StartPoint, Utc(2020, 4, 16, 0, 0), 148.33m, "Cycle(I) [3] (iii) Start");
        [TestMethod] public void CycleI_P3_i3_End() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[2].EndPoint, Utc(2021, 2, 20, 4, 0), 2042.34m, "Cycle(I) [3] (iii) End");

        [TestMethod] public void CycleI_P3_i4_Label() => AssertLabel(_analysis.Waves[0].SubWaves[2].SubWaves[3], WaveNumber.Four, "Cycle(I) [3] (iv)");
        [TestMethod] public void CycleI_P3_i4_Start() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[3].StartPoint, Utc(2021, 2, 20, 4, 0), 2042.34m, "Cycle(I) [3] (iv) Start");
        [TestMethod] public void CycleI_P3_i4_End() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[3].EndPoint, Utc(2021, 2, 28, 15, 0), 1293.18m, "Cycle(I) [3] (iv) End");

        [TestMethod] public void CycleI_P3_i5_Label() => AssertLabel(_analysis.Waves[0].SubWaves[2].SubWaves[4], WaveNumber.Five, "Cycle(I) [3] (v)");
        [TestMethod] public void CycleI_P3_i5_Start() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[4].StartPoint, Utc(2021, 2, 28, 15, 0), 1293.18m, "Cycle(I) [3] (v) Start");
        [TestMethod] public void CycleI_P3_i5_End() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[4].EndPoint, Utc(2021, 5, 12, 14, 0), 4372.72m, "Cycle(I) [3] (v) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(I) [4]
        //
        // (a): $4'372.72 → $1'728.74 (23.05.2021)
        // (b): $1'728.74 → $2'910.00 (26.05.2021)
        // (c): $2'910.00 → $1'700.48 (22.06.2021)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleI_P4_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[0].SubWaves[3], 3, "Cycle(I) [4] Intermediate");

        [TestMethod] public void CycleI_P4_iA_Label() => AssertLabel(_analysis.Waves[0].SubWaves[3].SubWaves[0], WaveNumber.A, "Cycle(I) [4] (a)");
        [TestMethod] public void CycleI_P4_iA_Start() => AssertPoint(_analysis.Waves[0].SubWaves[3].SubWaves[0].StartPoint, Utc(2021, 5, 12, 14, 0), 4372.72m, "Cycle(I) [4] (a) Start");
        [TestMethod] public void CycleI_P4_iA_End() => AssertPoint(_analysis.Waves[0].SubWaves[3].SubWaves[0].EndPoint, Utc(2021, 5, 23, 16, 0), 1728.74m, "Cycle(I) [4] (a) End");

        [TestMethod] public void CycleI_P4_iB_Label() => AssertLabel(_analysis.Waves[0].SubWaves[3].SubWaves[1], WaveNumber.B, "Cycle(I) [4] (b)");
        [TestMethod] public void CycleI_P4_iB_Start() => AssertPoint(_analysis.Waves[0].SubWaves[3].SubWaves[1].StartPoint, Utc(2021, 5, 23, 16, 0), 1728.74m, "Cycle(I) [4] (b) Start");
        [TestMethod] public void CycleI_P4_iB_End() => AssertPoint(_analysis.Waves[0].SubWaves[3].SubWaves[1].EndPoint, Utc(2021, 5, 26, 8, 0), 2910m, "Cycle(I) [4] (b) End");

        [TestMethod] public void CycleI_P4_iC_Label() => AssertLabel(_analysis.Waves[0].SubWaves[3].SubWaves[2], WaveNumber.C, "Cycle(I) [4] (c)");
        [TestMethod] public void CycleI_P4_iC_Start() => AssertPoint(_analysis.Waves[0].SubWaves[3].SubWaves[2].StartPoint, Utc(2021, 5, 26, 8, 0), 2910m, "Cycle(I) [4] (c) Start");
        [TestMethod] public void CycleI_P4_iC_End() => AssertPoint(_analysis.Waves[0].SubWaves[3].SubWaves[2].EndPoint, Utc(2021, 6, 22, 13, 0), 1700.48m, "Cycle(I) [4] (c) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(I) [5]
        //
        // (i): $1'700.48 → $2'409.00 (07.07.2021)
        // (ii): $2'409.00 → $1'706.00 (20.07.2021)
        // (iii): $1'706.00 → $4'027.88 (03.09.2021)
        // (iv): $4'027.88 → $2'652.00 (21.09.2021)
        // (v): $2'652.00 → $4'868.00 (10.11.2021)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleI_P5_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[0].SubWaves[4], 5, "Cycle(I) [5] Intermediate");

        [TestMethod] public void CycleI_P5_i1_Label() => AssertLabel(_analysis.Waves[0].SubWaves[4].SubWaves[0], WaveNumber.One, "Cycle(I) [5] (i)");
        [TestMethod] public void CycleI_P5_i1_Start() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[0].StartPoint, Utc(2021, 6, 22, 13, 0), 1700.48m, "Cycle(I) [5] (i) Start");
        [TestMethod] public void CycleI_P5_i1_End() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[0].EndPoint, Utc(2021, 7, 7, 7, 0), 2409m, "Cycle(I) [5] (i) End");

        [TestMethod] public void CycleI_P5_i2_Label() => AssertLabel(_analysis.Waves[0].SubWaves[4].SubWaves[1], WaveNumber.Two, "Cycle(I) [5] (ii)");
        [TestMethod] public void CycleI_P5_i2_Start() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[1].StartPoint, Utc(2021, 7, 7, 7, 0), 2409m, "Cycle(I) [5] (ii) Start");
        [TestMethod] public void CycleI_P5_i2_End() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[1].EndPoint, Utc(2021, 7, 20, 3, 0), 1706m, "Cycle(I) [5] (ii) End");

        [TestMethod] public void CycleI_P5_i3_Label() => AssertLabel(_analysis.Waves[0].SubWaves[4].SubWaves[2], WaveNumber.Three, "Cycle(I) [5] (iii)");
        [TestMethod] public void CycleI_P5_i3_Start() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[2].StartPoint, Utc(2021, 7, 20, 3, 0), 1706m, "Cycle(I) [5] (iii) Start");
        [TestMethod] public void CycleI_P5_i3_End() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[2].EndPoint, Utc(2021, 9, 3, 14, 0), 4027.88m, "Cycle(I) [5] (iii) End");

        [TestMethod] public void CycleI_P5_i4_Label() => AssertLabel(_analysis.Waves[0].SubWaves[4].SubWaves[3], WaveNumber.Four, "Cycle(I) [5] (iv)");
        [TestMethod] public void CycleI_P5_i4_Start() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[3].StartPoint, Utc(2021, 9, 3, 14, 0), 4027.88m, "Cycle(I) [5] (iv) Start");
        [TestMethod] public void CycleI_P5_i4_End() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[3].EndPoint, Utc(2021, 9, 21, 21, 0), 2652m, "Cycle(I) [5] (iv) End");

        [TestMethod] public void CycleI_P5_i5_Label() => AssertLabel(_analysis.Waves[0].SubWaves[4].SubWaves[4], WaveNumber.Five, "Cycle(I) [5] (v)");
        [TestMethod] public void CycleI_P5_i5_Start() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[4].StartPoint, Utc(2021, 9, 21, 21, 0), 2652m, "Cycle(I) [5] (v) Start");
        [TestMethod] public void CycleI_P5_i5_End() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[4].EndPoint, Utc(2021, 11, 10, 14, 0), 4868m, "Cycle(I) [5] (v) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(II) [A]
        //
        // (i): $4'868.00 → $3'913.00 (26.11.2021)
        // (ii): $3'913.00 → $4'778.75 (01.12.2021)
        // (iii): $4'778.75 → $2'928.83 (10.01.2022)
        // (iv): $2'928.83 → $3'411.43 (12.01.2022)
        // (v): $3'411.43 → $2'159.00 (24.01.2022)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleII_PA_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[1].SubWaves[0], 5, "Cycle(II) [A] Intermediate");

        [TestMethod] public void CycleII_PA_i1_Label() => AssertLabel(_analysis.Waves[1].SubWaves[0].SubWaves[0], WaveNumber.One, "Cycle(II) [A] (i)");
        [TestMethod] public void CycleII_PA_i1_Start() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[0].StartPoint, Utc(2021, 11, 10, 14, 0), 4868m, "Cycle(II) [A] (i) Start");
        [TestMethod] public void CycleII_PA_i1_End() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[0].EndPoint, Utc(2021, 11, 26, 12, 0), 3913m, "Cycle(II) [A] (i) End");

        [TestMethod] public void CycleII_PA_i2_Label() => AssertLabel(_analysis.Waves[1].SubWaves[0].SubWaves[1], WaveNumber.Two, "Cycle(II) [A] (ii)");
        [TestMethod] public void CycleII_PA_i2_Start() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[1].StartPoint, Utc(2021, 11, 26, 12, 0), 3913m, "Cycle(II) [A] (ii) Start");
        [TestMethod] public void CycleII_PA_i2_End() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[1].EndPoint, Utc(2021, 12, 1, 7, 0), 4778.75m, "Cycle(II) [A] (ii) End");

        [TestMethod] public void CycleII_PA_i3_Label() => AssertLabel(_analysis.Waves[1].SubWaves[0].SubWaves[2], WaveNumber.Three, "Cycle(II) [A] (iii)");
        [TestMethod] public void CycleII_PA_i3_Start() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[2].StartPoint, Utc(2021, 12, 1, 7, 0), 4778.75m, "Cycle(II) [A] (iii) Start");
        [TestMethod] public void CycleII_PA_i3_End() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[2].EndPoint, Utc(2022, 1, 10, 14, 0), 2928.83m, "Cycle(II) [A] (iii) End");

        [TestMethod] public void CycleII_PA_i4_Label() => AssertLabel(_analysis.Waves[1].SubWaves[0].SubWaves[3], WaveNumber.Four, "Cycle(II) [A] (iv)");
        [TestMethod] public void CycleII_PA_i4_Start() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[3].StartPoint, Utc(2022, 1, 10, 14, 0), 2928.83m, "Cycle(II) [A] (iv) Start");
        [TestMethod] public void CycleII_PA_i4_End() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[3].EndPoint, Utc(2022, 1, 12, 21, 0), 3411.43m, "Cycle(II) [A] (iv) End");

        [TestMethod] public void CycleII_PA_i5_Label() => AssertLabel(_analysis.Waves[1].SubWaves[0].SubWaves[4], WaveNumber.Five, "Cycle(II) [A] (v)");
        [TestMethod] public void CycleII_PA_i5_Start() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[4].StartPoint, Utc(2022, 1, 12, 21, 0), 3411.43m, "Cycle(II) [A] (v) Start");
        [TestMethod] public void CycleII_PA_i5_End() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[4].EndPoint, Utc(2022, 1, 24, 15, 0), 2159m, "Cycle(II) [A] (v) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(II) [B]
        //
        // (a): $2'159.00 → $3'283.66 (10.02.2022)
        // (b): $3'283.66 → $2'300.00 (24.02.2022)
        // (c): $2'300.00 → $3'580.34 (03.04.2022)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleII_PB_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[1].SubWaves[1], 3, "Cycle(II) [B] Intermediate");

        [TestMethod] public void CycleII_PB_iA_Label() => AssertLabel(_analysis.Waves[1].SubWaves[1].SubWaves[0], WaveNumber.A, "Cycle(II) [B] (a)");
        [TestMethod] public void CycleII_PB_iA_Start() => AssertPoint(_analysis.Waves[1].SubWaves[1].SubWaves[0].StartPoint, Utc(2022, 1, 24, 15, 0), 2159m, "Cycle(II) [B] (a) Start");
        [TestMethod] public void CycleII_PB_iA_End() => AssertPoint(_analysis.Waves[1].SubWaves[1].SubWaves[0].EndPoint, Utc(2022, 2, 10, 11, 0), 3283.66m, "Cycle(II) [B] (a) End");

        [TestMethod] public void CycleII_PB_iB_Label() => AssertLabel(_analysis.Waves[1].SubWaves[1].SubWaves[1], WaveNumber.B, "Cycle(II) [B] (b)");
        [TestMethod] public void CycleII_PB_iB_Start() => AssertPoint(_analysis.Waves[1].SubWaves[1].SubWaves[1].StartPoint, Utc(2022, 2, 10, 11, 0), 3283.66m, "Cycle(II) [B] (b) Start");
        [TestMethod] public void CycleII_PB_iB_End() => AssertPoint(_analysis.Waves[1].SubWaves[1].SubWaves[1].EndPoint, Utc(2022, 2, 24, 5, 0), 2300m, "Cycle(II) [B] (b) End");

        [TestMethod] public void CycleII_PB_iC_Label() => AssertLabel(_analysis.Waves[1].SubWaves[1].SubWaves[2], WaveNumber.C, "Cycle(II) [B] (c)");
        [TestMethod] public void CycleII_PB_iC_Start() => AssertPoint(_analysis.Waves[1].SubWaves[1].SubWaves[2].StartPoint, Utc(2022, 2, 24, 5, 0), 2300m, "Cycle(II) [B] (c) Start");
        [TestMethod] public void CycleII_PB_iC_End() => AssertPoint(_analysis.Waves[1].SubWaves[1].SubWaves[2].EndPoint, Utc(2022, 4, 3, 22, 0), 3580.34m, "Cycle(II) [B] (c) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(II) [C]
        //
        // (i): $3'580.34 → $2'883.22 (18.04.2022)
        // (ii): $2'883.22 → $3'180.00 (21.04.2022)
        // (iii): $3'180.00 → $1'703.00 (27.05.2022)
        // (iv): $1'703.00 → $2'016.45 (31.05.2022)
        // (v): $2'016.45 → $881.56 (18.06.2022)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleII_PC_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[1].SubWaves[2], 5, "Cycle(II) [C] Intermediate");

        [TestMethod] public void CycleII_PC_i1_Label() => AssertLabel(_analysis.Waves[1].SubWaves[2].SubWaves[0], WaveNumber.One, "Cycle(II) [C] (i)");
        [TestMethod] public void CycleII_PC_i1_Start() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[0].StartPoint, Utc(2022, 4, 3, 22, 0), 3580.34m, "Cycle(II) [C] (i) Start");
        [TestMethod] public void CycleII_PC_i1_End() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[0].EndPoint, Utc(2022, 4, 18, 7, 0), 2883.22m, "Cycle(II) [C] (i) End");

        [TestMethod] public void CycleII_PC_i2_Label() => AssertLabel(_analysis.Waves[1].SubWaves[2].SubWaves[1], WaveNumber.Two, "Cycle(II) [C] (ii)");
        [TestMethod] public void CycleII_PC_i2_Start() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[1].StartPoint, Utc(2022, 4, 18, 7, 0), 2883.22m, "Cycle(II) [C] (ii) Start");
        [TestMethod] public void CycleII_PC_i2_End() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[1].EndPoint, Utc(2022, 4, 21, 12, 0), 3180m, "Cycle(II) [C] (ii) End");

        [TestMethod] public void CycleII_PC_i3_Label() => AssertLabel(_analysis.Waves[1].SubWaves[2].SubWaves[2], WaveNumber.Three, "Cycle(II) [C] (iii)");
        [TestMethod] public void CycleII_PC_i3_Start() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[2].StartPoint, Utc(2022, 4, 21, 12, 0), 3180m, "Cycle(II) [C] (iii) Start");
        [TestMethod] public void CycleII_PC_i3_End() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[2].EndPoint, Utc(2022, 5, 27, 23, 0), 1703m, "Cycle(II) [C] (iii) End");

        [TestMethod] public void CycleII_PC_i4_Label() => AssertLabel(_analysis.Waves[1].SubWaves[2].SubWaves[3], WaveNumber.Four, "Cycle(II) [C] (iv)");
        [TestMethod] public void CycleII_PC_i4_Start() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[3].StartPoint, Utc(2022, 5, 27, 23, 0), 1703m, "Cycle(II) [C] (iv) Start");
        [TestMethod] public void CycleII_PC_i4_End() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[3].EndPoint, Utc(2022, 5, 31, 0, 0), 2016.45m, "Cycle(II) [C] (iv) End");

        [TestMethod] public void CycleII_PC_i5_Label() => AssertLabel(_analysis.Waves[1].SubWaves[2].SubWaves[4], WaveNumber.Five, "Cycle(II) [C] (v)");
        [TestMethod] public void CycleII_PC_i5_Start() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[4].StartPoint, Utc(2022, 5, 31, 0, 0), 2016.45m, "Cycle(II) [C] (v) Start");
        [TestMethod] public void CycleII_PC_i5_End() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[4].EndPoint, Utc(2022, 6, 18, 20, 0), 881.56m, "Cycle(II) [C] (v) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(III) [1]
        //
        // (i): $881.56 → $1'280.00 (26.06.2022)
        // (ii): $1'280.00 → $998.00 (30.06.2022)
        // (iii): $998.00 → $4'107.80 (16.12.2024)
        // (iv): $4'107.80 → $1'385.05 (09.04.2025)
        // (v): $1'385.05 → $4'956.78 (24.08.2025)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleIII_P1_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[2].SubWaves[0], 5, "Cycle(III) [1] Intermediate");

        [TestMethod] public void CycleIII_P1_i1_Label() => AssertLabel(_analysis.Waves[2].SubWaves[0].SubWaves[0], WaveNumber.One, "Cycle(III) [1] (i)");
        [TestMethod] public void CycleIII_P1_i1_Start() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[0].StartPoint, Utc(2022, 6, 18, 20, 0), 881.56m, "Cycle(III) [1] (i) Start");
        [TestMethod] public void CycleIII_P1_i1_End() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[0].EndPoint, Utc(2022, 6, 26, 12, 0), 1280m, "Cycle(III) [1] (i) End");

        [TestMethod] public void CycleIII_P1_i2_Label() => AssertLabel(_analysis.Waves[2].SubWaves[0].SubWaves[1], WaveNumber.Two, "Cycle(III) [1] (ii)");
        [TestMethod] public void CycleIII_P1_i2_Start() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[1].StartPoint, Utc(2022, 6, 26, 12, 0), 1280m, "Cycle(III) [1] (ii) Start");
        [TestMethod] public void CycleIII_P1_i2_End() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[1].EndPoint, Utc(2022, 6, 30, 13, 0), 998m, "Cycle(III) [1] (ii) End");

        [TestMethod] public void CycleIII_P1_i3_Label() => AssertLabel(_analysis.Waves[2].SubWaves[0].SubWaves[2], WaveNumber.Three, "Cycle(III) [1] (iii)");
        [TestMethod] public void CycleIII_P1_i3_Start() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[2].StartPoint, Utc(2022, 6, 30, 13, 0), 998m, "Cycle(III) [1] (iii) Start");
        [TestMethod] public void CycleIII_P1_i3_End() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[2].EndPoint, Utc(2024, 12, 16, 18, 0), 4107.8m, "Cycle(III) [1] (iii) End");

        [TestMethod] public void CycleIII_P1_i4_Label() => AssertLabel(_analysis.Waves[2].SubWaves[0].SubWaves[3], WaveNumber.Four, "Cycle(III) [1] (iv)");
        [TestMethod] public void CycleIII_P1_i4_Start() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[3].StartPoint, Utc(2024, 12, 16, 18, 0), 4107.8m, "Cycle(III) [1] (iv) Start");
        [TestMethod] public void CycleIII_P1_i4_End() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[3].EndPoint, Utc(2025, 4, 9, 1, 0), 1385.05m, "Cycle(III) [1] (iv) End");

        [TestMethod] public void CycleIII_P1_i5_Label() => AssertLabel(_analysis.Waves[2].SubWaves[0].SubWaves[4], WaveNumber.Five, "Cycle(III) [1] (v)");
        [TestMethod] public void CycleIII_P1_i5_Start() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[4].StartPoint, Utc(2025, 4, 9, 1, 0), 1385.05m, "Cycle(III) [1] (v) Start");
        [TestMethod] public void CycleIII_P1_i5_End() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[4].EndPoint, Utc(2025, 8, 24, 19, 0), 4956.78m, "Cycle(III) [1] (v) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(III) [2]
        //
        // (a): $4'956.78 → $2'623.57 (21.11.2025)
        // (b): $2'623.57 → $3'447.44 (10.12.2025)
        // (c): $3'447.44 → $1'747.80 (06.02.2026)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleIII_P2_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[2].SubWaves[1], 3, "Cycle(III) [2] Intermediate");

        [TestMethod] public void CycleIII_P2_iA_Label() => AssertLabel(_analysis.Waves[2].SubWaves[1].SubWaves[0], WaveNumber.A, "Cycle(III) [2] (a)");
        [TestMethod] public void CycleIII_P2_iA_Start() => AssertPoint(_analysis.Waves[2].SubWaves[1].SubWaves[0].StartPoint, Utc(2025, 8, 24, 19, 0), 4956.78m, "Cycle(III) [2] (a) Start");
        [TestMethod] public void CycleIII_P2_iA_End() => AssertPoint(_analysis.Waves[2].SubWaves[1].SubWaves[0].EndPoint, Utc(2025, 11, 21, 12, 0), 2623.57m, "Cycle(III) [2] (a) End");

        [TestMethod] public void CycleIII_P2_iB_Label() => AssertLabel(_analysis.Waves[2].SubWaves[1].SubWaves[1], WaveNumber.B, "Cycle(III) [2] (b)");
        [TestMethod] public void CycleIII_P2_iB_Start() => AssertPoint(_analysis.Waves[2].SubWaves[1].SubWaves[1].StartPoint, Utc(2025, 11, 21, 12, 0), 2623.57m, "Cycle(III) [2] (b) Start");
        [TestMethod] public void CycleIII_P2_iB_End() => AssertPoint(_analysis.Waves[2].SubWaves[1].SubWaves[1].EndPoint, Utc(2025, 12, 10, 20, 0), 3447.44m, "Cycle(III) [2] (b) End");

        [TestMethod] public void CycleIII_P2_iC_Label() => AssertLabel(_analysis.Waves[2].SubWaves[1].SubWaves[2], WaveNumber.C, "Cycle(III) [2] (c)");
        [TestMethod] public void CycleIII_P2_iC_Start() => AssertPoint(_analysis.Waves[2].SubWaves[1].SubWaves[2].StartPoint, Utc(2025, 12, 10, 20, 0), 3447.44m, "Cycle(III) [2] (c) Start");
        [TestMethod] public void CycleIII_P2_iC_End() => AssertPoint(_analysis.Waves[2].SubWaves[1].SubWaves[2].EndPoint, Utc(2026, 2, 6, 0, 0), 1747.8m, "Cycle(III) [2] (c) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(III) [3]
        //
        // (i): $1'747.80 → $2'386.02 (16.03.2026)
        // (ii): $2'386.02 → $2'099.38 (19.03.2026)
        // (iii): $2'099.38 → $2'099.38 (19.03.2026) (in-progress)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleIII_P3_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[2].SubWaves[2], 3, "Cycle(III) [3] Intermediate");

        [TestMethod] public void CycleIII_P3_i1_Label() => AssertLabel(_analysis.Waves[2].SubWaves[2].SubWaves[0], WaveNumber.One, "Cycle(III) [3] (i)");
        [TestMethod] public void CycleIII_P3_i1_Start() => AssertPoint(_analysis.Waves[2].SubWaves[2].SubWaves[0].StartPoint, Utc(2026, 2, 6, 0, 0), 1747.8m, "Cycle(III) [3] (i) Start");
        [TestMethod] public void CycleIII_P3_i1_End() => AssertPoint(_analysis.Waves[2].SubWaves[2].SubWaves[0].EndPoint, Utc(2026, 3, 16, 22, 0), 2386.02m, "Cycle(III) [3] (i) End");

        [TestMethod] public void CycleIII_P3_i2_Label() => AssertLabel(_analysis.Waves[2].SubWaves[2].SubWaves[1], WaveNumber.Two, "Cycle(III) [3] (ii)");
        [TestMethod] public void CycleIII_P3_i2_Start() => AssertPoint(_analysis.Waves[2].SubWaves[2].SubWaves[1].StartPoint, Utc(2026, 3, 16, 22, 0), 2386.02m, "Cycle(III) [3] (ii) Start");
        [TestMethod] public void CycleIII_P3_i2_End() => AssertPoint(_analysis.Waves[2].SubWaves[2].SubWaves[1].EndPoint, Utc(2026, 3, 19, 15, 0), 2099.38m, "Cycle(III) [3] (ii) End");

        [TestMethod] public void CycleIII_P3_i3_Label() => AssertLabel(_analysis.Waves[2].SubWaves[2].SubWaves[2], WaveNumber.Three, "Cycle(III) [3] (iii)");
        [TestMethod] public void CycleIII_P3_i3_InProgress() => AssertInProgress(_analysis.Waves[2].SubWaves[2].SubWaves[2], true, "Cycle(III) [3] (iii)");
        [TestMethod] public void CycleIII_P3_i3_Start() => AssertPoint(_analysis.Waves[2].SubWaves[2].SubWaves[2].StartPoint, Utc(2026, 3, 19, 15, 0), 2099.38m, "Cycle(III) [3] (iii) Start");

        // ══════════════════════════════════════════════════
        // Projections — Cycle (III): W3 extension of Cycle I ($81.79 → $4,868.00) from Cycle II end ($881.56)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleIII_Projection_4236() => AssertProjectionTarget(_analysis.Waves[2], LogExtension(81.79m, 4868.00m, 881.56m, 4.236m), 4.236m, "Cycle(III) 4.236 projection");
        [TestMethod] public void CycleIII_Projection_2618() => AssertProjectionTarget(_analysis.Waves[2], LogExtension(81.79m, 4868.00m, 881.56m, 2.618m), 2.618m, "Cycle(III) 2.618 projection");
        [TestMethod] public void CycleIII_Projection_1618() => AssertProjectionTarget(_analysis.Waves[2], LogExtension(81.79m, 4868.00m, 881.56m, 1.618m), 1.618m, "Cycle(III) 1.618 projection");
        [TestMethod] public void CycleIII_Projection_100() => AssertProjectionTarget(_analysis.Waves[2], LogExtension(81.79m, 4868.00m, 881.56m, 1.0m), 1.0m, "Cycle(III) 1.0 projection");

        // ══════════════════════════════════════════════════
        // Projections — Primary [3] inside Cycle (III)
        // W3 extension of P1 ($881.56 → $4,956.78) from P2 end ($1,747.80)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleIII_P3_Projection_4236() => AssertProjectionTarget(_analysis.Waves[2].SubWaves[2], LogExtension(881.56m, 4956.78m, 1747.80m, 4.236m), 4.236m, "Cycle(III) [3] 4.236 projection");
        [TestMethod] public void CycleIII_P3_Projection_2618() => AssertProjectionTarget(_analysis.Waves[2].SubWaves[2], LogExtension(881.56m, 4956.78m, 1747.80m, 2.618m), 2.618m, "Cycle(III) [3] 2.618 projection");
        [TestMethod] public void CycleIII_P3_Projection_1618() => AssertProjectionTarget(_analysis.Waves[2].SubWaves[2], LogExtension(881.56m, 4956.78m, 1747.80m, 1.618m), 1.618m, "Cycle(III) [3] 1.618 projection");
        [TestMethod] public void CycleIII_P3_Projection_100() => AssertProjectionTarget(_analysis.Waves[2].SubWaves[2], LogExtension(881.56m, 4956.78m, 1747.80m, 1.0m), 1.0m, "Cycle(III) [3] 1.0 projection");
    }
}
