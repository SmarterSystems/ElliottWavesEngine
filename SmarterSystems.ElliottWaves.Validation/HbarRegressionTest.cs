using SmarterSystems.ElliottWaves.Analyzer.Interfaces;

namespace SmarterSystems.ElliottWaves.Validation
{
    [TestClass]
    public sealed class HbarRegressionTest : RegressionTestBase
    {
        private static ElliottWavesAnalysis _analysis = null!;

        [ClassInitialize]
        public static void Setup(TestContext _)
        {
            _analysis = LoadAndAnalyze("HBARUSDT");
        }

        // ══════════════════════════════════════════════════
        // Cycle Degree: 2 waves (I completed; II in-progress)
        //
        // Cycle (I):  31.12.2019 $0.00914 → 16.09.2021 $0.576
        // Cycle (II): 16.09.2021 $0.576 → in-progress
        // ══════════════════════════════════════════════════

        [TestMethod] public void Cycle_WaveCount() => AssertWaveCount(_analysis, 2, "HBAR Cycle");

        // ── Cycle (I): $0.00914 → $0.576 ──

        [TestMethod] public void CycleI_Label() => AssertLabel(_analysis.Waves[0], WaveNumber.One, "Cycle(I)");
        [TestMethod] public void CycleI_NotInProgress() => AssertInProgress(_analysis.Waves[0], false, "Cycle(I)");
        [TestMethod] public void CycleI_Start() => AssertPoint(_analysis.Waves[0].StartPoint, Utc(2019, 12, 31, 20), 0.00914m, "Cycle(I) Start");
        [TestMethod] public void CycleI_End() => AssertPoint(_analysis.Waves[0].EndPoint, Utc(2021, 9, 16, 13), 0.576m, "Cycle(I) End");

        // ── Cycle (II): $0.576 → in-progress ──

        [TestMethod] public void CycleII_Label() => AssertLabel(_analysis.Waves[1], WaveNumber.Two, "Cycle(II)");
        [TestMethod] public void CycleII_InProgress() => AssertInProgress(_analysis.Waves[1], true, "Cycle(II)");
        [TestMethod] public void CycleII_Start() => AssertPoint(_analysis.Waves[1].StartPoint, Utc(2021, 9, 16, 13), 0.576m, "Cycle(II) Start");

        // ══════════════════════════════════════════════════
        // Primary Degree — inside Cycle (I): 5 waves (impulse)
        //
        // [1]: $0.00914 → $0.083 (12.02.2020)
        // [2]: $0.083 → $0.01761 (13.03.2020)
        // [3]: $0.01761 → $0.4555 (15.03.2021)
        // [4]: $0.4555 → $0.13821 (22.06.2021)
        // [5]: $0.13821 → $0.576 (16.09.2021)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleI_Primary_WaveCount() => AssertSubWaveCount(_analysis.Waves[0], 5, "Cycle(I) Primary");

        [TestMethod] public void CycleI_P1_Label() => AssertLabel(_analysis.Waves[0].SubWaves[0], WaveNumber.One, "Cycle(I) [1]");
        [TestMethod] public void CycleI_P1_Start() => AssertPoint(_analysis.Waves[0].SubWaves[0].StartPoint, Utc(2019, 12, 31, 20), 0.00914m, "Cycle(I) [1] Start");
        [TestMethod] public void CycleI_P1_End() => AssertPoint(_analysis.Waves[0].SubWaves[0].EndPoint, Utc(2020, 2, 12, 16), 0.083m, "Cycle(I) [1] End");

        [TestMethod] public void CycleI_P2_Label() => AssertLabel(_analysis.Waves[0].SubWaves[1], WaveNumber.Two, "Cycle(I) [2]");
        [TestMethod] public void CycleI_P2_Start() => AssertPoint(_analysis.Waves[0].SubWaves[1].StartPoint, Utc(2020, 2, 12, 16), 0.083m, "Cycle(I) [2] Start");
        [TestMethod] public void CycleI_P2_End() => AssertPoint(_analysis.Waves[0].SubWaves[1].EndPoint, Utc(2020, 3, 13, 2), 0.01761m, "Cycle(I) [2] End");

        [TestMethod] public void CycleI_P3_Label() => AssertLabel(_analysis.Waves[0].SubWaves[2], WaveNumber.Three, "Cycle(I) [3]");
        [TestMethod] public void CycleI_P3_Start() => AssertPoint(_analysis.Waves[0].SubWaves[2].StartPoint, Utc(2020, 3, 13, 2), 0.01761m, "Cycle(I) [3] Start");
        [TestMethod] public void CycleI_P3_End() => AssertPoint(_analysis.Waves[0].SubWaves[2].EndPoint, Utc(2021, 3, 15, 1), 0.4555m, "Cycle(I) [3] End");

        [TestMethod] public void CycleI_P4_Label() => AssertLabel(_analysis.Waves[0].SubWaves[3], WaveNumber.Four, "Cycle(I) [4]");
        [TestMethod] public void CycleI_P4_Start() => AssertPoint(_analysis.Waves[0].SubWaves[3].StartPoint, Utc(2021, 3, 15, 1), 0.4555m, "Cycle(I) [4] Start");
        [TestMethod] public void CycleI_P4_End() => AssertPoint(_analysis.Waves[0].SubWaves[3].EndPoint, Utc(2021, 6, 22, 12), 0.13821m, "Cycle(I) [4] End");

        [TestMethod] public void CycleI_P5_Label() => AssertLabel(_analysis.Waves[0].SubWaves[4], WaveNumber.Five, "Cycle(I) [5]");
        [TestMethod] public void CycleI_P5_Start() => AssertPoint(_analysis.Waves[0].SubWaves[4].StartPoint, Utc(2021, 6, 22, 12), 0.13821m, "Cycle(I) [5] Start");
        [TestMethod] public void CycleI_P5_End() => AssertPoint(_analysis.Waves[0].SubWaves[4].EndPoint, Utc(2021, 9, 16, 13), 0.576m, "Cycle(I) [5] End");

        // ══════════════════════════════════════════════════
        // Primary Degree — inside Cycle (II): A-B in-progress
        //
        // [A]: $0.576 → $0.0356 (31.12.2022)
        // [B]: $0.0356 → $0.40139 (17.01.2025)
        // ══════════════════════════════════════════════════

        [TestMethod]
        public void CycleII_HasSubWaves()
        {
            Assert.IsNotNull(_analysis.Waves[1].SubWaves, "Cycle(II) should have sub-waves");
            Assert.IsTrue(_analysis.Waves[1].SubWaves.Count >= 2, "Cycle(II) should have at least 2 primary waves");
        }

        [TestMethod] public void CycleII_A_Label() => AssertLabel(_analysis.Waves[1].SubWaves[0], WaveNumber.A, "Cycle(II) [A]");
        [TestMethod] public void CycleII_A_Start() => AssertPoint(_analysis.Waves[1].SubWaves[0].StartPoint, Utc(2021, 9, 16, 13), 0.576m, "Cycle(II) [A] Start");
        [TestMethod] public void CycleII_A_End() => AssertPoint(_analysis.Waves[1].SubWaves[0].EndPoint, Utc(2022, 12, 31, 20), 0.0356m, "Cycle(II) [A] End");

        [TestMethod] public void CycleII_B_Label() => AssertLabel(_analysis.Waves[1].SubWaves[1], WaveNumber.B, "Cycle(II) [B]");
        [TestMethod] public void CycleII_B_Start() => AssertPoint(_analysis.Waves[1].SubWaves[1].StartPoint, Utc(2022, 12, 31, 20), 0.0356m, "Cycle(II) [B] Start");
        [TestMethod] public void CycleII_B_End() => AssertPoint(_analysis.Waves[1].SubWaves[1].EndPoint, Utc(2025, 1, 17, 2), 0.40139m, "Cycle(II) [B] End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(I) [1]
        //
        // (i): $0.00914000 → $0.01132 (31.12.2019)
        // (ii): $0.01132 → $0.00985000 (02.01.2020)
        // (iii): $0.00985000 → $0.02381 (06.02.2020)
        // (iv): $0.02381 → $0.01790 (08.02.2020)
        // (v): $0.01790 → $0.08300 (12.02.2020)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleI_P1_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[0].SubWaves[0], 5, "Cycle(I) [1] Intermediate");

        [TestMethod] public void CycleI_P1_i1_Label() => AssertLabel(_analysis.Waves[0].SubWaves[0].SubWaves[0], WaveNumber.One, "Cycle(I) [1] (i)");
        [TestMethod] public void CycleI_P1_i1_Start() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[0].StartPoint, Utc(2019, 12, 31, 20, 0), 0.00914m, "Cycle(I) [1] (i) Start");
        [TestMethod] public void CycleI_P1_i1_End() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[0].EndPoint, Utc(2019, 12, 31, 20, 0), 0.01132m, "Cycle(I) [1] (i) End");

        [TestMethod] public void CycleI_P1_i2_Label() => AssertLabel(_analysis.Waves[0].SubWaves[0].SubWaves[1], WaveNumber.Two, "Cycle(I) [1] (ii)");
        [TestMethod] public void CycleI_P1_i2_Start() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[1].StartPoint, Utc(2019, 12, 31, 20, 0), 0.01132m, "Cycle(I) [1] (ii) Start");
        [TestMethod] public void CycleI_P1_i2_End() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[1].EndPoint, Utc(2020, 1, 2, 17, 0), 0.00985m, "Cycle(I) [1] (ii) End");

        [TestMethod] public void CycleI_P1_i3_Label() => AssertLabel(_analysis.Waves[0].SubWaves[0].SubWaves[2], WaveNumber.Three, "Cycle(I) [1] (iii)");
        [TestMethod] public void CycleI_P1_i3_Start() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[2].StartPoint, Utc(2020, 1, 2, 17, 0), 0.00985m, "Cycle(I) [1] (iii) Start");
        [TestMethod] public void CycleI_P1_i3_End() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[2].EndPoint, Utc(2020, 2, 6, 9, 0), 0.02381m, "Cycle(I) [1] (iii) End");

        [TestMethod] public void CycleI_P1_i4_Label() => AssertLabel(_analysis.Waves[0].SubWaves[0].SubWaves[3], WaveNumber.Four, "Cycle(I) [1] (iv)");
        [TestMethod] public void CycleI_P1_i4_Start() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[3].StartPoint, Utc(2020, 2, 6, 9, 0), 0.02381m, "Cycle(I) [1] (iv) Start");
        [TestMethod] public void CycleI_P1_i4_End() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[3].EndPoint, Utc(2020, 2, 8, 21, 0), 0.0179m, "Cycle(I) [1] (iv) End");

        [TestMethod] public void CycleI_P1_i5_Label() => AssertLabel(_analysis.Waves[0].SubWaves[0].SubWaves[4], WaveNumber.Five, "Cycle(I) [1] (v)");
        [TestMethod] public void CycleI_P1_i5_Start() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[4].StartPoint, Utc(2020, 2, 8, 21, 0), 0.0179m, "Cycle(I) [1] (v) Start");
        [TestMethod] public void CycleI_P1_i5_End() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[4].EndPoint, Utc(2020, 2, 12, 16, 0), 0.083m, "Cycle(I) [1] (v) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(I) [2]
        //
        // (a): $0.08300 → $0.02910 (26.02.2020)
        // (b): $0.02910 → $0.06470 (07.03.2020)
        // (c): $0.06470 → $0.01761 (13.03.2020)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleI_P2_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[0].SubWaves[1], 3, "Cycle(I) [2] Intermediate");

        [TestMethod] public void CycleI_P2_iA_Label() => AssertLabel(_analysis.Waves[0].SubWaves[1].SubWaves[0], WaveNumber.A, "Cycle(I) [2] (a)");
        [TestMethod] public void CycleI_P2_iA_Start() => AssertPoint(_analysis.Waves[0].SubWaves[1].SubWaves[0].StartPoint, Utc(2020, 2, 12, 16, 0), 0.083m, "Cycle(I) [2] (a) Start");
        [TestMethod] public void CycleI_P2_iA_End() => AssertPoint(_analysis.Waves[0].SubWaves[1].SubWaves[0].EndPoint, Utc(2020, 2, 26, 16, 0), 0.0291m, "Cycle(I) [2] (a) End");

        [TestMethod] public void CycleI_P2_iB_Label() => AssertLabel(_analysis.Waves[0].SubWaves[1].SubWaves[1], WaveNumber.B, "Cycle(I) [2] (b)");
        [TestMethod] public void CycleI_P2_iB_Start() => AssertPoint(_analysis.Waves[0].SubWaves[1].SubWaves[1].StartPoint, Utc(2020, 2, 26, 16, 0), 0.0291m, "Cycle(I) [2] (b) Start");
        [TestMethod] public void CycleI_P2_iB_End() => AssertPoint(_analysis.Waves[0].SubWaves[1].SubWaves[1].EndPoint, Utc(2020, 3, 7, 15, 0), 0.0647m, "Cycle(I) [2] (b) End");

        [TestMethod] public void CycleI_P2_iC_Label() => AssertLabel(_analysis.Waves[0].SubWaves[1].SubWaves[2], WaveNumber.C, "Cycle(I) [2] (c)");
        [TestMethod] public void CycleI_P2_iC_Start() => AssertPoint(_analysis.Waves[0].SubWaves[1].SubWaves[2].StartPoint, Utc(2020, 3, 7, 15, 0), 0.0647m, "Cycle(I) [2] (c) Start");
        [TestMethod] public void CycleI_P2_iC_End() => AssertPoint(_analysis.Waves[0].SubWaves[1].SubWaves[2].EndPoint, Utc(2020, 3, 13, 2, 0), 0.01761m, "Cycle(I) [2] (c) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(I) [3]
        //
        // (i): $0.01761 → $0.06176 (17.08.2020)
        // (ii): $0.06176 → $0.02769 (04.11.2020)
        // (iii): $0.02769 → $0.16000 (19.02.2021)
        // (iv): $0.16000 → $0.09000 (23.02.2021)
        // (v): $0.09000 → $0.45550 (15.03.2021)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleI_P3_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[0].SubWaves[2], 5, "Cycle(I) [3] Intermediate");

        [TestMethod] public void CycleI_P3_i1_Label() => AssertLabel(_analysis.Waves[0].SubWaves[2].SubWaves[0], WaveNumber.One, "Cycle(I) [3] (i)");
        [TestMethod] public void CycleI_P3_i1_Start() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[0].StartPoint, Utc(2020, 3, 13, 2, 0), 0.01761m, "Cycle(I) [3] (i) Start");
        [TestMethod] public void CycleI_P3_i1_End() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[0].EndPoint, Utc(2020, 8, 17, 21, 0), 0.06176m, "Cycle(I) [3] (i) End");

        [TestMethod] public void CycleI_P3_i2_Label() => AssertLabel(_analysis.Waves[0].SubWaves[2].SubWaves[1], WaveNumber.Two, "Cycle(I) [3] (ii)");
        [TestMethod] public void CycleI_P3_i2_Start() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[1].StartPoint, Utc(2020, 8, 17, 21, 0), 0.06176m, "Cycle(I) [3] (ii) Start");
        [TestMethod] public void CycleI_P3_i2_End() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[1].EndPoint, Utc(2020, 11, 4, 11, 0), 0.02769m, "Cycle(I) [3] (ii) End");

        [TestMethod] public void CycleI_P3_i3_Label() => AssertLabel(_analysis.Waves[0].SubWaves[2].SubWaves[2], WaveNumber.Three, "Cycle(I) [3] (iii)");
        [TestMethod] public void CycleI_P3_i3_Start() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[2].StartPoint, Utc(2020, 11, 4, 11, 0), 0.02769m, "Cycle(I) [3] (iii) Start");
        [TestMethod] public void CycleI_P3_i3_End() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[2].EndPoint, Utc(2021, 2, 19, 19, 0), 0.16m, "Cycle(I) [3] (iii) End");

        [TestMethod] public void CycleI_P3_i4_Label() => AssertLabel(_analysis.Waves[0].SubWaves[2].SubWaves[3], WaveNumber.Four, "Cycle(I) [3] (iv)");
        [TestMethod] public void CycleI_P3_i4_Start() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[3].StartPoint, Utc(2021, 2, 19, 19, 0), 0.16m, "Cycle(I) [3] (iv) Start");
        [TestMethod] public void CycleI_P3_i4_End() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[3].EndPoint, Utc(2021, 2, 23, 9, 0), 0.09m, "Cycle(I) [3] (iv) End");

        [TestMethod] public void CycleI_P3_i5_Label() => AssertLabel(_analysis.Waves[0].SubWaves[2].SubWaves[4], WaveNumber.Five, "Cycle(I) [3] (v)");
        [TestMethod] public void CycleI_P3_i5_Start() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[4].StartPoint, Utc(2021, 2, 23, 9, 0), 0.09m, "Cycle(I) [3] (v) Start");
        [TestMethod] public void CycleI_P3_i5_End() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[4].EndPoint, Utc(2021, 3, 15, 1, 0), 0.4555m, "Cycle(I) [3] (v) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(I) [4]
        //
        // (a): $0.45550 → $0.19100 (23.04.2021)
        // (b): $0.19100 → $0.40949 (13.05.2021)
        // (c): $0.40949 → $0.13821 (22.06.2021)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleI_P4_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[0].SubWaves[3], 3, "Cycle(I) [4] Intermediate");

        [TestMethod] public void CycleI_P4_iA_Label() => AssertLabel(_analysis.Waves[0].SubWaves[3].SubWaves[0], WaveNumber.A, "Cycle(I) [4] (a)");
        [TestMethod] public void CycleI_P4_iA_Start() => AssertPoint(_analysis.Waves[0].SubWaves[3].SubWaves[0].StartPoint, Utc(2021, 3, 15, 1, 0), 0.4555m, "Cycle(I) [4] (a) Start");
        [TestMethod] public void CycleI_P4_iA_End() => AssertPoint(_analysis.Waves[0].SubWaves[3].SubWaves[0].EndPoint, Utc(2021, 4, 23, 2, 0), 0.191m, "Cycle(I) [4] (a) End");

        [TestMethod] public void CycleI_P4_iB_Label() => AssertLabel(_analysis.Waves[0].SubWaves[3].SubWaves[1], WaveNumber.B, "Cycle(I) [4] (b)");
        [TestMethod] public void CycleI_P4_iB_Start() => AssertPoint(_analysis.Waves[0].SubWaves[3].SubWaves[1].StartPoint, Utc(2021, 4, 23, 2, 0), 0.191m, "Cycle(I) [4] (b) Start");
        [TestMethod] public void CycleI_P4_iB_End() => AssertPoint(_analysis.Waves[0].SubWaves[3].SubWaves[1].EndPoint, Utc(2021, 5, 13, 20, 0), 0.40949m, "Cycle(I) [4] (b) End");

        [TestMethod] public void CycleI_P4_iC_Label() => AssertLabel(_analysis.Waves[0].SubWaves[3].SubWaves[2], WaveNumber.C, "Cycle(I) [4] (c)");
        [TestMethod] public void CycleI_P4_iC_Start() => AssertPoint(_analysis.Waves[0].SubWaves[3].SubWaves[2].StartPoint, Utc(2021, 5, 13, 20, 0), 0.40949m, "Cycle(I) [4] (c) Start");
        [TestMethod] public void CycleI_P4_iC_End() => AssertPoint(_analysis.Waves[0].SubWaves[3].SubWaves[2].EndPoint, Utc(2021, 6, 22, 12, 0), 0.13821m, "Cycle(I) [4] (c) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(I) [5]
        //
        // (i): $0.13821 → $0.21360 (01.07.2021)
        // (ii): $0.21360 → $0.15500 (21.07.2021)
        // (iii): $0.15500 → $0.34477 (07.09.2021)
        // (iv): $0.34477 → $0.23100 (07.09.2021)
        // (v): $0.23100 → $0.57600 (16.09.2021)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleI_P5_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[0].SubWaves[4], 5, "Cycle(I) [5] Intermediate");

        [TestMethod] public void CycleI_P5_i1_Label() => AssertLabel(_analysis.Waves[0].SubWaves[4].SubWaves[0], WaveNumber.One, "Cycle(I) [5] (i)");
        [TestMethod] public void CycleI_P5_i1_Start() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[0].StartPoint, Utc(2021, 6, 22, 12, 0), 0.13821m, "Cycle(I) [5] (i) Start");
        [TestMethod] public void CycleI_P5_i1_End() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[0].EndPoint, Utc(2021, 7, 1, 23, 0), 0.2136m, "Cycle(I) [5] (i) End");

        [TestMethod] public void CycleI_P5_i2_Label() => AssertLabel(_analysis.Waves[0].SubWaves[4].SubWaves[1], WaveNumber.Two, "Cycle(I) [5] (ii)");
        [TestMethod] public void CycleI_P5_i2_Start() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[1].StartPoint, Utc(2021, 7, 1, 23, 0), 0.2136m, "Cycle(I) [5] (ii) Start");
        [TestMethod] public void CycleI_P5_i2_End() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[1].EndPoint, Utc(2021, 7, 21, 1, 0), 0.155m, "Cycle(I) [5] (ii) End");

        [TestMethod] public void CycleI_P5_i3_Label() => AssertLabel(_analysis.Waves[0].SubWaves[4].SubWaves[2], WaveNumber.Three, "Cycle(I) [5] (iii)");
        [TestMethod] public void CycleI_P5_i3_Start() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[2].StartPoint, Utc(2021, 7, 21, 1, 0), 0.155m, "Cycle(I) [5] (iii) Start");
        [TestMethod] public void CycleI_P5_i3_End() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[2].EndPoint, Utc(2021, 9, 7, 8, 0), 0.34477m, "Cycle(I) [5] (iii) End");

        [TestMethod] public void CycleI_P5_i4_Label() => AssertLabel(_analysis.Waves[0].SubWaves[4].SubWaves[3], WaveNumber.Four, "Cycle(I) [5] (iv)");
        [TestMethod] public void CycleI_P5_i4_Start() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[3].StartPoint, Utc(2021, 9, 7, 8, 0), 0.34477m, "Cycle(I) [5] (iv) Start");
        [TestMethod] public void CycleI_P5_i4_End() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[3].EndPoint, Utc(2021, 9, 7, 15, 0), 0.231m, "Cycle(I) [5] (iv) End");

        [TestMethod] public void CycleI_P5_i5_Label() => AssertLabel(_analysis.Waves[0].SubWaves[4].SubWaves[4], WaveNumber.Five, "Cycle(I) [5] (v)");
        [TestMethod] public void CycleI_P5_i5_Start() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[4].StartPoint, Utc(2021, 9, 7, 15, 0), 0.231m, "Cycle(I) [5] (v) Start");
        [TestMethod] public void CycleI_P5_i5_End() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[4].EndPoint, Utc(2021, 9, 16, 13, 0), 0.576m, "Cycle(I) [5] (v) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(II) [A]
        //
        // (i): $0.57600 → $0.28800 (21.09.2021)
        // (ii): $0.28800 → $0.47800 (12.11.2021)
        // (iii): $0.47800 → $0.05500 (05.07.2022)
        // (iv): $0.05500 → $0.08450 (11.08.2022)
        // (v): $0.08450 → $0.03560 (31.12.2022)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleII_PA_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[1].SubWaves[0], 5, "Cycle(II) [A] Intermediate");

        [TestMethod] public void CycleII_PA_i1_Label() => AssertLabel(_analysis.Waves[1].SubWaves[0].SubWaves[0], WaveNumber.One, "Cycle(II) [A] (i)");
        [TestMethod] public void CycleII_PA_i1_Start() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[0].StartPoint, Utc(2021, 9, 16, 13, 0), 0.576m, "Cycle(II) [A] (i) Start");
        [TestMethod] public void CycleII_PA_i1_End() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[0].EndPoint, Utc(2021, 9, 21, 21, 0), 0.288m, "Cycle(II) [A] (i) End");

        [TestMethod] public void CycleII_PA_i2_Label() => AssertLabel(_analysis.Waves[1].SubWaves[0].SubWaves[1], WaveNumber.Two, "Cycle(II) [A] (ii)");
        [TestMethod] public void CycleII_PA_i2_Start() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[1].StartPoint, Utc(2021, 9, 21, 21, 0), 0.288m, "Cycle(II) [A] (ii) Start");
        [TestMethod] public void CycleII_PA_i2_End() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[1].EndPoint, Utc(2021, 11, 12, 12, 0), 0.478m, "Cycle(II) [A] (ii) End");

        [TestMethod] public void CycleII_PA_i3_Label() => AssertLabel(_analysis.Waves[1].SubWaves[0].SubWaves[2], WaveNumber.Three, "Cycle(II) [A] (iii)");
        [TestMethod] public void CycleII_PA_i3_Start() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[2].StartPoint, Utc(2021, 11, 12, 12, 0), 0.478m, "Cycle(II) [A] (iii) Start");
        [TestMethod] public void CycleII_PA_i3_End() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[2].EndPoint, Utc(2022, 7, 5, 13, 0), 0.055m, "Cycle(II) [A] (iii) End");

        [TestMethod] public void CycleII_PA_i4_Label() => AssertLabel(_analysis.Waves[1].SubWaves[0].SubWaves[3], WaveNumber.Four, "Cycle(II) [A] (iv)");
        [TestMethod] public void CycleII_PA_i4_Start() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[3].StartPoint, Utc(2022, 7, 5, 13, 0), 0.055m, "Cycle(II) [A] (iv) Start");
        [TestMethod] public void CycleII_PA_i4_End() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[3].EndPoint, Utc(2022, 8, 11, 15, 0), 0.0845m, "Cycle(II) [A] (iv) End");

        [TestMethod] public void CycleII_PA_i5_Label() => AssertLabel(_analysis.Waves[1].SubWaves[0].SubWaves[4], WaveNumber.Five, "Cycle(II) [A] (v)");
        [TestMethod] public void CycleII_PA_i5_Start() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[4].StartPoint, Utc(2022, 8, 11, 15, 0), 0.0845m, "Cycle(II) [A] (v) Start");
        [TestMethod] public void CycleII_PA_i5_End() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[4].EndPoint, Utc(2022, 12, 31, 20, 0), 0.0356m, "Cycle(II) [A] (v) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(II) [B]
        //
        // (a): $0.03560 → $0.18170 (24.04.2024)
        // (b): $0.18170 → $0.04160 (03.11.2024)
        // (c): $0.04160 → $0.40139 (17.01.2025)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleII_PB_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[1].SubWaves[1], 3, "Cycle(II) [B] Intermediate");

        [TestMethod] public void CycleII_PB_iA_Label() => AssertLabel(_analysis.Waves[1].SubWaves[1].SubWaves[0], WaveNumber.A, "Cycle(II) [B] (a)");
        [TestMethod] public void CycleII_PB_iA_Start() => AssertPoint(_analysis.Waves[1].SubWaves[1].SubWaves[0].StartPoint, Utc(2022, 12, 31, 20, 0), 0.0356m, "Cycle(II) [B] (a) Start");
        [TestMethod] public void CycleII_PB_iA_End() => AssertPoint(_analysis.Waves[1].SubWaves[1].SubWaves[0].EndPoint, Utc(2024, 4, 24, 0, 0), 0.1817m, "Cycle(II) [B] (a) End");

        [TestMethod] public void CycleII_PB_iB_Label() => AssertLabel(_analysis.Waves[1].SubWaves[1].SubWaves[1], WaveNumber.B, "Cycle(II) [B] (b)");
        [TestMethod] public void CycleII_PB_iB_Start() => AssertPoint(_analysis.Waves[1].SubWaves[1].SubWaves[1].StartPoint, Utc(2024, 4, 24, 0, 0), 0.1817m, "Cycle(II) [B] (b) Start");
        [TestMethod] public void CycleII_PB_iB_End() => AssertPoint(_analysis.Waves[1].SubWaves[1].SubWaves[1].EndPoint, Utc(2024, 11, 3, 14, 0), 0.0416m, "Cycle(II) [B] (b) End");

        [TestMethod] public void CycleII_PB_iC_Label() => AssertLabel(_analysis.Waves[1].SubWaves[1].SubWaves[2], WaveNumber.C, "Cycle(II) [B] (c)");
        [TestMethod] public void CycleII_PB_iC_Start() => AssertPoint(_analysis.Waves[1].SubWaves[1].SubWaves[2].StartPoint, Utc(2024, 11, 3, 14, 0), 0.0416m, "Cycle(II) [B] (c) Start");
        [TestMethod] public void CycleII_PB_iC_End() => AssertPoint(_analysis.Waves[1].SubWaves[1].SubWaves[2].EndPoint, Utc(2025, 1, 17, 2, 0), 0.40139m, "Cycle(II) [B] (c) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(II) [C]
        //
        // (i): $0.40139 → $0.12488 (07.04.2025)
        // (ii): $0.12488 → $0.30500 (27.07.2025)
        // (iii): $0.30500 → $0.30500 (27.07.2025) (in-progress)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleII_PC_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[1].SubWaves[2], 3, "Cycle(II) [C] Intermediate");

        [TestMethod] public void CycleII_PC_i1_Label() => AssertLabel(_analysis.Waves[1].SubWaves[2].SubWaves[0], WaveNumber.One, "Cycle(II) [C] (i)");
        [TestMethod] public void CycleII_PC_i1_Start() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[0].StartPoint, Utc(2025, 1, 17, 2, 0), 0.40139m, "Cycle(II) [C] (i) Start");
        [TestMethod] public void CycleII_PC_i1_End() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[0].EndPoint, Utc(2025, 4, 7, 6, 0), 0.12488m, "Cycle(II) [C] (i) End");

        [TestMethod] public void CycleII_PC_i2_Label() => AssertLabel(_analysis.Waves[1].SubWaves[2].SubWaves[1], WaveNumber.Two, "Cycle(II) [C] (ii)");
        [TestMethod] public void CycleII_PC_i2_Start() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[1].StartPoint, Utc(2025, 4, 7, 6, 0), 0.12488m, "Cycle(II) [C] (ii) Start");
        [TestMethod] public void CycleII_PC_i2_End() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[1].EndPoint, Utc(2025, 7, 27, 5, 0), 0.305m, "Cycle(II) [C] (ii) End");

        [TestMethod] public void CycleII_PC_i3_Label() => AssertLabel(_analysis.Waves[1].SubWaves[2].SubWaves[2], WaveNumber.Three, "Cycle(II) [C] (iii)");
        [TestMethod] public void CycleII_PC_i3_InProgress() => AssertInProgress(_analysis.Waves[1].SubWaves[2].SubWaves[2], true, "Cycle(II) [C] (iii)");
        [TestMethod] public void CycleII_PC_i3_Start() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[2].StartPoint, Utc(2025, 7, 27, 5, 0), 0.305m, "Cycle(II) [C] (iii) Start");

        // ══════════════════════════════════════════════════
        // Projections — Cycle (II): W2 retracement of Cycle I ($0.01 → $0.58)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleII_Projection_0786() => AssertProjectionTarget(_analysis.Waves[1], LogRetracement(0.00914m, 0.576m, 0.786m), 0.786m, "Cycle(II) 0.786 projection");
        [TestMethod] public void CycleII_Projection_0618() => AssertProjectionTarget(_analysis.Waves[1], LogRetracement(0.00914m, 0.576m, 0.618m), 0.618m, "Cycle(II) 0.618 projection");
        [TestMethod] public void CycleII_Projection_050() => AssertProjectionTarget(_analysis.Waves[1], LogRetracement(0.00914m, 0.576m, 0.5m), 0.5m, "Cycle(II) 0.5 projection");
        [TestMethod] public void CycleII_Projection_0382() => AssertProjectionTarget(_analysis.Waves[1], LogRetracement(0.00914m, 0.576m, 0.382m), 0.382m, "Cycle(II) 0.382 projection");

        // ══════════════════════════════════════════════════
        // Projections — Primary [C] inside Cycle (II): C extension of A ($0.58 → $0.04) from B end ($0.40)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleII_C_Projection_2618() => AssertProjectionTarget(_analysis.Waves[1].SubWaves[2], LogExtension(0.576m, 0.0356m, 0.40139m, 2.618m), 2.618m, "Cycle(II) [C] 2.618 projection");
        [TestMethod] public void CycleII_C_Projection_1618() => AssertProjectionTarget(_analysis.Waves[1].SubWaves[2], LogExtension(0.576m, 0.0356m, 0.40139m, 1.618m), 1.618m, "Cycle(II) [C] 1.618 projection");
        [TestMethod] public void CycleII_C_Projection_AEqualsC() => AssertProjectionTarget(_analysis.Waves[1].SubWaves[2], LogExtension(0.576m, 0.0356m, 0.40139m, 1.0m), 1.0m, "Cycle(II) [C] A=C projection");
        [TestMethod] public void CycleII_C_Projection_0618() => AssertProjectionTarget(_analysis.Waves[1].SubWaves[2], LogExtension(0.576m, 0.0356m, 0.40139m, 0.618m), 0.618m, "Cycle(II) [C] 0.618 projection");
    }
}
