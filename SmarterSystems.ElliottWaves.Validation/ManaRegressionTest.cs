using SmarterSystems.ElliottWaves.Analyzer.Interfaces;

namespace SmarterSystems.ElliottWaves.Validation
{
    [TestClass]
    public sealed class ManaRegressionTest : RegressionTestBase
    {
        private static ElliottWavesAnalysis _analysis = null!;

        [ClassInitialize]
        public static void Setup(TestContext _)
        {
            _analysis = LoadAndAnalyze("MANAUSDT");
        }

        // ══════════════════════════════════════════════════
        // Cycle Degree: 3 waves (I, II completed; III in-progress)
        //
        // Cycle (I):   10.09.2020 $0.03 → 26.11.2021 $5.4555
        // Cycle (II):  26.11.2021 $5.4555 → 10.10.2025 $0.0465
        // Cycle (III): 10.10.2025 $0.0465 → in-progress
        // ══════════════════════════════════════════════════

        [TestMethod] public void Cycle_WaveCount() => AssertWaveCount(_analysis, 3, "MANA Cycle");

        // ── Cycle (I): $0.03 → $5.4555 ──

        [TestMethod] public void CycleI_Label() => AssertLabel(_analysis.Waves[0], WaveNumber.One, "Cycle(I)");
        [TestMethod] public void CycleI_NotInProgress() => AssertInProgress(_analysis.Waves[0], false, "Cycle(I)");
        [TestMethod] public void CycleI_Start() => AssertPoint(_analysis.Waves[0].StartPoint, Utc(2020, 9, 10, 9), 0.03m, "Cycle(I) Start");
        [TestMethod] public void CycleI_End() => AssertPoint(_analysis.Waves[0].EndPoint, Utc(2021, 11, 25, 9), 5.9m, "Cycle(I) End");

        // ── Cycle (II): $5.4555 → $0.0465 ──

        [TestMethod] public void CycleII_Label() => AssertLabel(_analysis.Waves[1], WaveNumber.Two, "Cycle(II)");
        [TestMethod] public void CycleII_NotInProgress() => AssertInProgress(_analysis.Waves[1], false, "Cycle(II)");
        [TestMethod] public void CycleII_Start() => AssertPoint(_analysis.Waves[1].StartPoint, Utc(2021, 11, 25, 9), 5.9m, "Cycle(II) Start");
        [TestMethod] public void CycleII_End() => AssertPoint(_analysis.Waves[1].EndPoint, Utc(2025, 10, 10, 21), 0.0465m, "Cycle(II) End");

        // ── Cycle (III): $0.0465 → in-progress ──

        [TestMethod] public void CycleIII_Label() => AssertLabel(_analysis.Waves[2], WaveNumber.Three, "Cycle(III)");
        [TestMethod] public void CycleIII_InProgress() => AssertInProgress(_analysis.Waves[2], true, "Cycle(III)");
        [TestMethod] public void CycleIII_Start() => AssertPoint(_analysis.Waves[2].StartPoint, Utc(2025, 10, 10, 21), 0.0465m, "Cycle(III) Start");

        // ══════════════════════════════════════════════════
        // Primary Degree — inside Cycle (I): 5 waves (impulse)
        //
        // [1]: $0.03 → $0.08276 (20.11.2020)
        // [2]: $0.08276 → $0.07 (24.12.2020)
        // [3]: $0.07 → $1.215 (14.03.2021)
        // [4]: $1.215 → $0.4141 (23.06.2021)
        // [5]: $0.4141 → $5.4555 (26.11.2021)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleI_Primary_WaveCount() => AssertSubWaveCount(_analysis.Waves[0], 5, "Cycle(I) Primary");

        [TestMethod] public void CycleI_P1_Label() => AssertLabel(_analysis.Waves[0].SubWaves[0], WaveNumber.One, "Cycle(I) [1]");
        [TestMethod] public void CycleI_P1_Start() => AssertPoint(_analysis.Waves[0].SubWaves[0].StartPoint, Utc(2020, 9, 10, 9), 0.03m, "Cycle(I) [1] Start");
        [TestMethod] public void CycleI_P1_End() => AssertPoint(_analysis.Waves[0].SubWaves[0].EndPoint, Utc(2020, 9, 29, 13), 0.0915m, "Cycle(I) [1] End");

        [TestMethod] public void CycleI_P2_Label() => AssertLabel(_analysis.Waves[0].SubWaves[1], WaveNumber.Two, "Cycle(I) [2]");
        [TestMethod] public void CycleI_P2_Start() => AssertPoint(_analysis.Waves[0].SubWaves[1].StartPoint, Utc(2020, 9, 29, 13), 0.0915m, "Cycle(I) [2] Start");
        [TestMethod] public void CycleI_P2_End() => AssertPoint(_analysis.Waves[0].SubWaves[1].EndPoint, Utc(2020, 11, 3, 6), 0.05963m, "Cycle(I) [2] End");

        [TestMethod] public void CycleI_P3_Label() => AssertLabel(_analysis.Waves[0].SubWaves[2], WaveNumber.Three, "Cycle(I) [3]");
        [TestMethod] public void CycleI_P3_Start() => AssertPoint(_analysis.Waves[0].SubWaves[2].StartPoint, Utc(2020, 11, 3, 6), 0.05963m, "Cycle(I) [3] Start");
        [TestMethod] public void CycleI_P3_End() => AssertPoint(_analysis.Waves[0].SubWaves[2].EndPoint, Utc(2021, 4, 17, 13), 1.67m, "Cycle(I) [3] End");

        [TestMethod] public void CycleI_P4_Label() => AssertLabel(_analysis.Waves[0].SubWaves[3], WaveNumber.Four, "Cycle(I) [4]");
        [TestMethod] public void CycleI_P4_Start() => AssertPoint(_analysis.Waves[0].SubWaves[3].StartPoint, Utc(2021, 4, 17, 13), 1.67m, "Cycle(I) [4] Start");
        [TestMethod] public void CycleI_P4_End() => AssertPoint(_analysis.Waves[0].SubWaves[3].EndPoint, Utc(2021, 6, 22, 13), 0.3715m, "Cycle(I) [4] End");

        [TestMethod] public void CycleI_P5_Label() => AssertLabel(_analysis.Waves[0].SubWaves[4], WaveNumber.Five, "Cycle(I) [5]");
        [TestMethod] public void CycleI_P5_Start() => AssertPoint(_analysis.Waves[0].SubWaves[4].StartPoint, Utc(2021, 6, 22, 13), 0.3715m, "Cycle(I) [5] Start");
        [TestMethod] public void CycleI_P5_End() => AssertPoint(_analysis.Waves[0].SubWaves[4].EndPoint, Utc(2021, 11, 25, 9), 5.9m, "Cycle(I) [5] End");

        // ══════════════════════════════════════════════════
        // Primary Degree — inside Cycle (II): A-B-C (corrective)
        //
        // [A]: $5.4555 → $0.284 (30.12.2022)
        // [B]: $0.284 → $0.8556 (04.12.2024)
        // [C]: $0.8556 → $0.0465 (10.10.2025)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleII_Primary_WaveCount() => AssertSubWaveCount(_analysis.Waves[1], 3, "Cycle(II) Primary");

        [TestMethod] public void CycleII_A_Label() => AssertLabel(_analysis.Waves[1].SubWaves[0], WaveNumber.A, "Cycle(II) [A]");
        [TestMethod] public void CycleII_A_Start() => AssertPoint(_analysis.Waves[1].SubWaves[0].StartPoint, Utc(2021, 11, 25, 9), 5.9m, "Cycle(II) [A] Start");
        [TestMethod] public void CycleII_A_End() => AssertPoint(_analysis.Waves[1].SubWaves[0].EndPoint, Utc(2024, 8, 5, 6), 0.2161m, "Cycle(II) [A] End");

        [TestMethod] public void CycleII_B_Label() => AssertLabel(_analysis.Waves[1].SubWaves[1], WaveNumber.B, "Cycle(II) [B]");
        [TestMethod] public void CycleII_B_Start() => AssertPoint(_analysis.Waves[1].SubWaves[1].StartPoint, Utc(2024, 8, 5, 6), 0.2161m, "Cycle(II) [B] Start");
        [TestMethod] public void CycleII_B_End() => AssertPoint(_analysis.Waves[1].SubWaves[1].EndPoint, Utc(2024, 12, 4, 19), 0.8556m, "Cycle(II) [B] End");

        [TestMethod] public void CycleII_C_Label() => AssertLabel(_analysis.Waves[1].SubWaves[2], WaveNumber.C, "Cycle(II) [C]");
        [TestMethod] public void CycleII_C_Start() => AssertPoint(_analysis.Waves[1].SubWaves[2].StartPoint, Utc(2024, 12, 4, 19), 0.8556m, "Cycle(II) [C] Start");
        [TestMethod] public void CycleII_C_End() => AssertPoint(_analysis.Waves[1].SubWaves[2].EndPoint, Utc(2025, 10, 10, 21), 0.0465m, "Cycle(II) [C] End");

        // ══════════════════════════════════════════════════
        // Primary Degree — inside Cycle (III): in-progress
        //
        // [1]: $0.0465 → $0.2885 (13.10.2025) — completed
        // ══════════════════════════════════════════════════

        [TestMethod]
        public void CycleIII_HasSubWaves()
        {
            Assert.IsNotNull(_analysis.Waves[2].SubWaves, "Cycle(III) should have sub-waves");
            Assert.IsTrue(_analysis.Waves[2].SubWaves.Count >= 1, "Cycle(III) should have at least 1 primary wave");
        }

        [TestMethod] public void CycleIII_P1_Label() => AssertLabel(_analysis.Waves[2].SubWaves[0], WaveNumber.One, "Cycle(III) [1]");
        [TestMethod] public void CycleIII_P1_Start() => AssertPoint(_analysis.Waves[2].SubWaves[0].StartPoint, Utc(2025, 10, 10, 21), 0.0465m, "Cycle(III) [1] Start");
        [TestMethod] public void CycleIII_P1_End() => AssertPoint(_analysis.Waves[2].SubWaves[0].EndPoint, Utc(2025, 10, 13, 21), 0.2885m, "Cycle(III) [1] End");

        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(I) [2]
        //
        // (a): $0.09150 → $0.06886 (07.10.2020)
        // (b): $0.06886 → $0.08460 (10.10.2020)
        // (c): $0.08460 → $0.05963 (03.11.2020)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleI_P2_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[0].SubWaves[1], 3, "Cycle(I) [2] Intermediate");

        [TestMethod] public void CycleI_P2_iA_Label() => AssertLabel(_analysis.Waves[0].SubWaves[1].SubWaves[0], WaveNumber.A, "Cycle(I) [2] (a)");
        [TestMethod] public void CycleI_P2_iA_Start() => AssertPoint(_analysis.Waves[0].SubWaves[1].SubWaves[0].StartPoint, Utc(2020, 9, 29, 13, 0), 0.0915m, "Cycle(I) [2] (a) Start");
        [TestMethod] public void CycleI_P2_iA_End() => AssertPoint(_analysis.Waves[0].SubWaves[1].SubWaves[0].EndPoint, Utc(2020, 10, 7, 1, 0), 0.06886m, "Cycle(I) [2] (a) End");

        [TestMethod] public void CycleI_P2_iB_Label() => AssertLabel(_analysis.Waves[0].SubWaves[1].SubWaves[1], WaveNumber.B, "Cycle(I) [2] (b)");
        [TestMethod] public void CycleI_P2_iB_Start() => AssertPoint(_analysis.Waves[0].SubWaves[1].SubWaves[1].StartPoint, Utc(2020, 10, 7, 1, 0), 0.06886m, "Cycle(I) [2] (b) Start");
        [TestMethod] public void CycleI_P2_iB_End() => AssertPoint(_analysis.Waves[0].SubWaves[1].SubWaves[1].EndPoint, Utc(2020, 10, 10, 1, 0), 0.0846m, "Cycle(I) [2] (b) End");

        [TestMethod] public void CycleI_P2_iC_Label() => AssertLabel(_analysis.Waves[0].SubWaves[1].SubWaves[2], WaveNumber.C, "Cycle(I) [2] (c)");
        [TestMethod] public void CycleI_P2_iC_Start() => AssertPoint(_analysis.Waves[0].SubWaves[1].SubWaves[2].StartPoint, Utc(2020, 10, 10, 1, 0), 0.0846m, "Cycle(I) [2] (c) Start");
        [TestMethod] public void CycleI_P2_iC_End() => AssertPoint(_analysis.Waves[0].SubWaves[1].SubWaves[2].EndPoint, Utc(2020, 11, 3, 6, 0), 0.05963m, "Cycle(I) [2] (c) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(I) [3]
        //
        // (i): $0.05963 → $0.16167 (08.01.2021)
        // (ii): $0.16167 → $0.08000 (11.01.2021)
        // (iii): $0.08000 → $1.2150 (14.03.2021)
        // (iv): $1.2150 → $0.77500 (17.03.2021)
        // (v): $0.77500 → $1.6700 (17.04.2021)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleI_P3_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[0].SubWaves[2], 5, "Cycle(I) [3] Intermediate");

        [TestMethod] public void CycleI_P3_i1_Label() => AssertLabel(_analysis.Waves[0].SubWaves[2].SubWaves[0], WaveNumber.One, "Cycle(I) [3] (i)");
        [TestMethod] public void CycleI_P3_i1_Start() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[0].StartPoint, Utc(2020, 11, 3, 6, 0), 0.05963m, "Cycle(I) [3] (i) Start");
        [TestMethod] public void CycleI_P3_i1_End() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[0].EndPoint, Utc(2021, 1, 8, 0, 0), 0.16167m, "Cycle(I) [3] (i) End");

        [TestMethod] public void CycleI_P3_i2_Label() => AssertLabel(_analysis.Waves[0].SubWaves[2].SubWaves[1], WaveNumber.Two, "Cycle(I) [3] (ii)");
        [TestMethod] public void CycleI_P3_i2_Start() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[1].StartPoint, Utc(2021, 1, 8, 0, 0), 0.16167m, "Cycle(I) [3] (ii) Start");
        [TestMethod] public void CycleI_P3_i2_End() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[1].EndPoint, Utc(2021, 1, 11, 1, 0), 0.08m, "Cycle(I) [3] (ii) End");

        [TestMethod] public void CycleI_P3_i3_Label() => AssertLabel(_analysis.Waves[0].SubWaves[2].SubWaves[2], WaveNumber.Three, "Cycle(I) [3] (iii)");
        [TestMethod] public void CycleI_P3_i3_Start() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[2].StartPoint, Utc(2021, 1, 11, 1, 0), 0.08m, "Cycle(I) [3] (iii) Start");
        [TestMethod] public void CycleI_P3_i3_End() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[2].EndPoint, Utc(2021, 3, 14, 15, 0), 1.215m, "Cycle(I) [3] (iii) End");

        [TestMethod] public void CycleI_P3_i4_Label() => AssertLabel(_analysis.Waves[0].SubWaves[2].SubWaves[3], WaveNumber.Four, "Cycle(I) [3] (iv)");
        [TestMethod] public void CycleI_P3_i4_Start() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[3].StartPoint, Utc(2021, 3, 14, 15, 0), 1.215m, "Cycle(I) [3] (iv) Start");
        [TestMethod] public void CycleI_P3_i4_End() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[3].EndPoint, Utc(2021, 3, 17, 9, 0), 0.775m, "Cycle(I) [3] (iv) End");

        [TestMethod] public void CycleI_P3_i5_Label() => AssertLabel(_analysis.Waves[0].SubWaves[2].SubWaves[4], WaveNumber.Five, "Cycle(I) [3] (v)");
        [TestMethod] public void CycleI_P3_i5_Start() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[4].StartPoint, Utc(2021, 3, 17, 9, 0), 0.775m, "Cycle(I) [3] (v) Start");
        [TestMethod] public void CycleI_P3_i5_End() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[4].EndPoint, Utc(2021, 4, 17, 13, 0), 1.67m, "Cycle(I) [3] (v) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(I) [4]
        //
        // (a): $1.6700 → $0.48930 (23.05.2021)
        // (b): $0.48930 → $1.0189 (27.05.2021)
        // (c): $1.0189 → $0.37150 (22.06.2021)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleI_P4_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[0].SubWaves[3], 3, "Cycle(I) [4] Intermediate");

        [TestMethod] public void CycleI_P4_iA_Label() => AssertLabel(_analysis.Waves[0].SubWaves[3].SubWaves[0], WaveNumber.A, "Cycle(I) [4] (a)");
        [TestMethod] public void CycleI_P4_iA_Start() => AssertPoint(_analysis.Waves[0].SubWaves[3].SubWaves[0].StartPoint, Utc(2021, 4, 17, 13, 0), 1.67m, "Cycle(I) [4] (a) Start");
        [TestMethod] public void CycleI_P4_iA_End() => AssertPoint(_analysis.Waves[0].SubWaves[3].SubWaves[0].EndPoint, Utc(2021, 5, 23, 16, 0), 0.4893m, "Cycle(I) [4] (a) End");

        [TestMethod] public void CycleI_P4_iB_Label() => AssertLabel(_analysis.Waves[0].SubWaves[3].SubWaves[1], WaveNumber.B, "Cycle(I) [4] (b)");
        [TestMethod] public void CycleI_P4_iB_Start() => AssertPoint(_analysis.Waves[0].SubWaves[3].SubWaves[1].StartPoint, Utc(2021, 5, 23, 16, 0), 0.4893m, "Cycle(I) [4] (b) Start");
        [TestMethod] public void CycleI_P4_iB_End() => AssertPoint(_analysis.Waves[0].SubWaves[3].SubWaves[1].EndPoint, Utc(2021, 5, 27, 0, 0), 1.0189m, "Cycle(I) [4] (b) End");

        [TestMethod] public void CycleI_P4_iC_Label() => AssertLabel(_analysis.Waves[0].SubWaves[3].SubWaves[2], WaveNumber.C, "Cycle(I) [4] (c)");
        [TestMethod] public void CycleI_P4_iC_Start() => AssertPoint(_analysis.Waves[0].SubWaves[3].SubWaves[2].StartPoint, Utc(2021, 5, 27, 0, 0), 1.0189m, "Cycle(I) [4] (c) Start");
        [TestMethod] public void CycleI_P4_iC_End() => AssertPoint(_analysis.Waves[0].SubWaves[3].SubWaves[2].EndPoint, Utc(2021, 6, 22, 13, 0), 0.3715m, "Cycle(I) [4] (c) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(I) [5]
        //
        // (i): $0.37150 → $0.84760 (08.07.2021)
        // (ii): $0.84760 → $0.49170 (20.07.2021)
        // (iii): $0.49170 → $4.9455 (30.10.2021)
        // (iv): $4.9455 → $2.2000 (10.11.2021)
        // (v): $2.2000 → $5.9000 (25.11.2021)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleI_P5_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[0].SubWaves[4], 5, "Cycle(I) [5] Intermediate");

        [TestMethod] public void CycleI_P5_i1_Label() => AssertLabel(_analysis.Waves[0].SubWaves[4].SubWaves[0], WaveNumber.One, "Cycle(I) [5] (i)");
        [TestMethod] public void CycleI_P5_i1_Start() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[0].StartPoint, Utc(2021, 6, 22, 13, 0), 0.3715m, "Cycle(I) [5] (i) Start");
        [TestMethod] public void CycleI_P5_i1_End() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[0].EndPoint, Utc(2021, 7, 8, 1, 0), 0.8476m, "Cycle(I) [5] (i) End");

        [TestMethod] public void CycleI_P5_i2_Label() => AssertLabel(_analysis.Waves[0].SubWaves[4].SubWaves[1], WaveNumber.Two, "Cycle(I) [5] (ii)");
        [TestMethod] public void CycleI_P5_i2_Start() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[1].StartPoint, Utc(2021, 7, 8, 1, 0), 0.8476m, "Cycle(I) [5] (ii) Start");
        [TestMethod] public void CycleI_P5_i2_End() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[1].EndPoint, Utc(2021, 7, 20, 9, 0), 0.4917m, "Cycle(I) [5] (ii) End");

        [TestMethod] public void CycleI_P5_i3_Label() => AssertLabel(_analysis.Waves[0].SubWaves[4].SubWaves[2], WaveNumber.Three, "Cycle(I) [5] (iii)");
        [TestMethod] public void CycleI_P5_i3_Start() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[2].StartPoint, Utc(2021, 7, 20, 9, 0), 0.4917m, "Cycle(I) [5] (iii) Start");
        [TestMethod] public void CycleI_P5_i3_End() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[2].EndPoint, Utc(2021, 10, 30, 22, 0), 4.9455m, "Cycle(I) [5] (iii) End");

        [TestMethod] public void CycleI_P5_i4_Label() => AssertLabel(_analysis.Waves[0].SubWaves[4].SubWaves[3], WaveNumber.Four, "Cycle(I) [5] (iv)");
        [TestMethod] public void CycleI_P5_i4_Start() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[3].StartPoint, Utc(2021, 10, 30, 22, 0), 4.9455m, "Cycle(I) [5] (iv) Start");
        [TestMethod] public void CycleI_P5_i4_End() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[3].EndPoint, Utc(2021, 11, 10, 21, 0), 2.2m, "Cycle(I) [5] (iv) End");

        [TestMethod] public void CycleI_P5_i5_Label() => AssertLabel(_analysis.Waves[0].SubWaves[4].SubWaves[4], WaveNumber.Five, "Cycle(I) [5] (v)");
        [TestMethod] public void CycleI_P5_i5_Start() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[4].StartPoint, Utc(2021, 11, 10, 21, 0), 2.2m, "Cycle(I) [5] (v) Start");
        [TestMethod] public void CycleI_P5_i5_End() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[4].EndPoint, Utc(2021, 11, 25, 9, 0), 5.9m, "Cycle(I) [5] (v) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(II) [A]
        //
        // (i): $5.9000 → $2.5000 (04.12.2021)
        // (ii): $2.5000 → $4.2283 (04.12.2021)
        // (iii): $4.2283 → $0.26490 (11.09.2023)
        // (iv): $0.26490 → $0.82000 (10.03.2024)
        // (v): $0.82000 → $0.21610 (05.08.2024)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleII_PA_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[1].SubWaves[0], 5, "Cycle(II) [A] Intermediate");

        [TestMethod] public void CycleII_PA_i1_Label() => AssertLabel(_analysis.Waves[1].SubWaves[0].SubWaves[0], WaveNumber.One, "Cycle(II) [A] (i)");
        [TestMethod] public void CycleII_PA_i1_Start() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[0].StartPoint, Utc(2021, 11, 25, 9, 0), 5.9m, "Cycle(II) [A] (i) Start");
        [TestMethod] public void CycleII_PA_i1_End() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[0].EndPoint, Utc(2021, 12, 4, 5, 0), 2.5m, "Cycle(II) [A] (i) End");

        [TestMethod] public void CycleII_PA_i2_Label() => AssertLabel(_analysis.Waves[1].SubWaves[0].SubWaves[1], WaveNumber.Two, "Cycle(II) [A] (ii)");
        [TestMethod] public void CycleII_PA_i2_Start() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[1].StartPoint, Utc(2021, 12, 4, 5, 0), 2.5m, "Cycle(II) [A] (ii) Start");
        [TestMethod] public void CycleII_PA_i2_End() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[1].EndPoint, Utc(2021, 12, 4, 20, 0), 4.2283m, "Cycle(II) [A] (ii) End");

        [TestMethod] public void CycleII_PA_i3_Label() => AssertLabel(_analysis.Waves[1].SubWaves[0].SubWaves[2], WaveNumber.Three, "Cycle(II) [A] (iii)");
        [TestMethod] public void CycleII_PA_i3_Start() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[2].StartPoint, Utc(2021, 12, 4, 20, 0), 4.2283m, "Cycle(II) [A] (iii) Start");
        [TestMethod] public void CycleII_PA_i3_End() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[2].EndPoint, Utc(2023, 9, 11, 19, 0), 0.2649m, "Cycle(II) [A] (iii) End");

        [TestMethod] public void CycleII_PA_i4_Label() => AssertLabel(_analysis.Waves[1].SubWaves[0].SubWaves[3], WaveNumber.Four, "Cycle(II) [A] (iv)");
        [TestMethod] public void CycleII_PA_i4_Start() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[3].StartPoint, Utc(2023, 9, 11, 19, 0), 0.2649m, "Cycle(II) [A] (iv) Start");
        [TestMethod] public void CycleII_PA_i4_End() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[3].EndPoint, Utc(2024, 3, 10, 11, 0), 0.82m, "Cycle(II) [A] (iv) End");

        [TestMethod] public void CycleII_PA_i5_Label() => AssertLabel(_analysis.Waves[1].SubWaves[0].SubWaves[4], WaveNumber.Five, "Cycle(II) [A] (v)");
        [TestMethod] public void CycleII_PA_i5_Start() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[4].StartPoint, Utc(2024, 3, 10, 11, 0), 0.82m, "Cycle(II) [A] (v) Start");
        [TestMethod] public void CycleII_PA_i5_End() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[4].EndPoint, Utc(2024, 8, 5, 6, 0), 0.2161m, "Cycle(II) [A] (v) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(II) [B]
        //
        // (a): $0.21610 → $0.78200 (25.11.2024)
        // (b): $0.78200 → $0.56170 (26.11.2024)
        // (c): $0.56170 → $0.85560 (04.12.2024)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleII_PB_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[1].SubWaves[1], 3, "Cycle(II) [B] Intermediate");

        [TestMethod] public void CycleII_PB_iA_Label() => AssertLabel(_analysis.Waves[1].SubWaves[1].SubWaves[0], WaveNumber.A, "Cycle(II) [B] (a)");
        [TestMethod] public void CycleII_PB_iA_Start() => AssertPoint(_analysis.Waves[1].SubWaves[1].SubWaves[0].StartPoint, Utc(2024, 8, 5, 6, 0), 0.2161m, "Cycle(II) [B] (a) Start");
        [TestMethod] public void CycleII_PB_iA_End() => AssertPoint(_analysis.Waves[1].SubWaves[1].SubWaves[0].EndPoint, Utc(2024, 11, 25, 3, 0), 0.782m, "Cycle(II) [B] (a) End");

        [TestMethod] public void CycleII_PB_iB_Label() => AssertLabel(_analysis.Waves[1].SubWaves[1].SubWaves[1], WaveNumber.B, "Cycle(II) [B] (b)");
        [TestMethod] public void CycleII_PB_iB_Start() => AssertPoint(_analysis.Waves[1].SubWaves[1].SubWaves[1].StartPoint, Utc(2024, 11, 25, 3, 0), 0.782m, "Cycle(II) [B] (b) Start");
        [TestMethod] public void CycleII_PB_iB_End() => AssertPoint(_analysis.Waves[1].SubWaves[1].SubWaves[1].EndPoint, Utc(2024, 11, 26, 12, 0), 0.5617m, "Cycle(II) [B] (b) End");

        [TestMethod] public void CycleII_PB_iC_Label() => AssertLabel(_analysis.Waves[1].SubWaves[1].SubWaves[2], WaveNumber.C, "Cycle(II) [B] (c)");
        [TestMethod] public void CycleII_PB_iC_Start() => AssertPoint(_analysis.Waves[1].SubWaves[1].SubWaves[2].StartPoint, Utc(2024, 11, 26, 12, 0), 0.5617m, "Cycle(II) [B] (c) Start");
        [TestMethod] public void CycleII_PB_iC_End() => AssertPoint(_analysis.Waves[1].SubWaves[1].SubWaves[2].EndPoint, Utc(2024, 12, 4, 19, 0), 0.8556m, "Cycle(II) [B] (c) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(II) [C]
        //
        // (i): $0.85560 → $0.52360 (09.12.2024)
        // (ii): $0.52360 → $0.68810 (12.12.2024)
        // (iii): $0.68810 → $0.19040 (07.04.2025)
        // (iv): $0.19040 → $0.40450 (14.05.2025)
        // (v): $0.40450 → $0.04650 (10.10.2025)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleII_PC_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[1].SubWaves[2], 5, "Cycle(II) [C] Intermediate");

        [TestMethod] public void CycleII_PC_i1_Label() => AssertLabel(_analysis.Waves[1].SubWaves[2].SubWaves[0], WaveNumber.One, "Cycle(II) [C] (i)");
        [TestMethod] public void CycleII_PC_i1_Start() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[0].StartPoint, Utc(2024, 12, 4, 19, 0), 0.8556m, "Cycle(II) [C] (i) Start");
        [TestMethod] public void CycleII_PC_i1_End() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[0].EndPoint, Utc(2024, 12, 9, 21, 0), 0.5236m, "Cycle(II) [C] (i) End");

        [TestMethod] public void CycleII_PC_i2_Label() => AssertLabel(_analysis.Waves[1].SubWaves[2].SubWaves[1], WaveNumber.Two, "Cycle(II) [C] (ii)");
        [TestMethod] public void CycleII_PC_i2_Start() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[1].StartPoint, Utc(2024, 12, 9, 21, 0), 0.5236m, "Cycle(II) [C] (ii) Start");
        [TestMethod] public void CycleII_PC_i2_End() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[1].EndPoint, Utc(2024, 12, 12, 5, 0), 0.6881m, "Cycle(II) [C] (ii) End");

        [TestMethod] public void CycleII_PC_i3_Label() => AssertLabel(_analysis.Waves[1].SubWaves[2].SubWaves[2], WaveNumber.Three, "Cycle(II) [C] (iii)");
        [TestMethod] public void CycleII_PC_i3_Start() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[2].StartPoint, Utc(2024, 12, 12, 5, 0), 0.6881m, "Cycle(II) [C] (iii) Start");
        [TestMethod] public void CycleII_PC_i3_End() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[2].EndPoint, Utc(2025, 4, 7, 6, 0), 0.1904m, "Cycle(II) [C] (iii) End");

        [TestMethod] public void CycleII_PC_i4_Label() => AssertLabel(_analysis.Waves[1].SubWaves[2].SubWaves[3], WaveNumber.Four, "Cycle(II) [C] (iv)");
        [TestMethod] public void CycleII_PC_i4_Start() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[3].StartPoint, Utc(2025, 4, 7, 6, 0), 0.1904m, "Cycle(II) [C] (iv) Start");
        [TestMethod] public void CycleII_PC_i4_End() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[3].EndPoint, Utc(2025, 5, 14, 1, 0), 0.4045m, "Cycle(II) [C] (iv) End");

        [TestMethod] public void CycleII_PC_i5_Label() => AssertLabel(_analysis.Waves[1].SubWaves[2].SubWaves[4], WaveNumber.Five, "Cycle(II) [C] (v)");
        [TestMethod] public void CycleII_PC_i5_Start() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[4].StartPoint, Utc(2025, 5, 14, 1, 0), 0.4045m, "Cycle(II) [C] (v) Start");
        [TestMethod] public void CycleII_PC_i5_End() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[4].EndPoint, Utc(2025, 10, 10, 21, 0), 0.0465m, "Cycle(II) [C] (v) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(III) [1]
        //
        // (i): $0.04650 → $0.21280 (10.10.2025)
        // (ii): $0.21280 → $0.15000 (10.10.2025)
        // (iii): $0.15000 → $0.27730 (13.10.2025)
        // (iv): $0.27730 → $0.26440 (13.10.2025)
        // (v): $0.26440 → $0.28850 (13.10.2025)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleIII_P1_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[2].SubWaves[0], 5, "Cycle(III) [1] Intermediate");

        [TestMethod] public void CycleIII_P1_i1_Label() => AssertLabel(_analysis.Waves[2].SubWaves[0].SubWaves[0], WaveNumber.One, "Cycle(III) [1] (i)");
        [TestMethod] public void CycleIII_P1_i1_Start() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[0].StartPoint, Utc(2025, 10, 10, 21, 0), 0.0465m, "Cycle(III) [1] (i) Start");
        [TestMethod] public void CycleIII_P1_i1_End() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[0].EndPoint, Utc(2025, 10, 10, 21, 0), 0.2128m, "Cycle(III) [1] (i) End");

        [TestMethod] public void CycleIII_P1_i2_Label() => AssertLabel(_analysis.Waves[2].SubWaves[0].SubWaves[1], WaveNumber.Two, "Cycle(III) [1] (ii)");
        [TestMethod] public void CycleIII_P1_i2_Start() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[1].StartPoint, Utc(2025, 10, 10, 21, 0), 0.2128m, "Cycle(III) [1] (ii) Start");
        [TestMethod] public void CycleIII_P1_i2_End() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[1].EndPoint, Utc(2025, 10, 10, 22, 0), 0.15m, "Cycle(III) [1] (ii) End");

        [TestMethod] public void CycleIII_P1_i3_Label() => AssertLabel(_analysis.Waves[2].SubWaves[0].SubWaves[2], WaveNumber.Three, "Cycle(III) [1] (iii)");
        [TestMethod] public void CycleIII_P1_i3_Start() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[2].StartPoint, Utc(2025, 10, 10, 22, 0), 0.15m, "Cycle(III) [1] (iii) Start");
        [TestMethod] public void CycleIII_P1_i3_End() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[2].EndPoint, Utc(2025, 10, 13, 10, 0), 0.2773m, "Cycle(III) [1] (iii) End");

        [TestMethod] public void CycleIII_P1_i4_Label() => AssertLabel(_analysis.Waves[2].SubWaves[0].SubWaves[3], WaveNumber.Four, "Cycle(III) [1] (iv)");
        [TestMethod] public void CycleIII_P1_i4_Start() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[3].StartPoint, Utc(2025, 10, 13, 10, 0), 0.2773m, "Cycle(III) [1] (iv) Start");
        [TestMethod] public void CycleIII_P1_i4_End() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[3].EndPoint, Utc(2025, 10, 13, 11, 0), 0.2644m, "Cycle(III) [1] (iv) End");

        [TestMethod] public void CycleIII_P1_i5_Label() => AssertLabel(_analysis.Waves[2].SubWaves[0].SubWaves[4], WaveNumber.Five, "Cycle(III) [1] (v)");
        [TestMethod] public void CycleIII_P1_i5_Start() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[4].StartPoint, Utc(2025, 10, 13, 11, 0), 0.2644m, "Cycle(III) [1] (v) Start");
        [TestMethod] public void CycleIII_P1_i5_End() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[4].EndPoint, Utc(2025, 10, 13, 21, 0), 0.2885m, "Cycle(III) [1] (v) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(III) [2]
        //
        // (a): $0.28850 → $0.11450 (24.12.2025)
        // (b): $0.11450 → $0.18860 (23.01.2026)
        // (c): $0.18860 → $0.08140 (06.02.2026)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleIII_P2_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[2].SubWaves[1], 3, "Cycle(III) [2] Intermediate");

        [TestMethod] public void CycleIII_P2_iA_Label() => AssertLabel(_analysis.Waves[2].SubWaves[1].SubWaves[0], WaveNumber.A, "Cycle(III) [2] (a)");
        [TestMethod] public void CycleIII_P2_iA_Start() => AssertPoint(_analysis.Waves[2].SubWaves[1].SubWaves[0].StartPoint, Utc(2025, 10, 13, 21, 0), 0.2885m, "Cycle(III) [2] (a) Start");
        [TestMethod] public void CycleIII_P2_iA_End() => AssertPoint(_analysis.Waves[2].SubWaves[1].SubWaves[0].EndPoint, Utc(2025, 12, 24, 3, 0), 0.1145m, "Cycle(III) [2] (a) End");

        [TestMethod] public void CycleIII_P2_iB_Label() => AssertLabel(_analysis.Waves[2].SubWaves[1].SubWaves[1], WaveNumber.B, "Cycle(III) [2] (b)");
        [TestMethod] public void CycleIII_P2_iB_Start() => AssertPoint(_analysis.Waves[2].SubWaves[1].SubWaves[1].StartPoint, Utc(2025, 12, 24, 3, 0), 0.1145m, "Cycle(III) [2] (b) Start");
        [TestMethod] public void CycleIII_P2_iB_End() => AssertPoint(_analysis.Waves[2].SubWaves[1].SubWaves[1].EndPoint, Utc(2026, 1, 23, 4, 0), 0.1886m, "Cycle(III) [2] (b) End");

        [TestMethod] public void CycleIII_P2_iC_Label() => AssertLabel(_analysis.Waves[2].SubWaves[1].SubWaves[2], WaveNumber.C, "Cycle(III) [2] (c)");
        [TestMethod] public void CycleIII_P2_iC_Start() => AssertPoint(_analysis.Waves[2].SubWaves[1].SubWaves[2].StartPoint, Utc(2026, 1, 23, 4, 0), 0.1886m, "Cycle(III) [2] (c) Start");
        [TestMethod] public void CycleIII_P2_iC_End() => AssertPoint(_analysis.Waves[2].SubWaves[1].SubWaves[2].EndPoint, Utc(2026, 2, 6, 0, 0), 0.0814m, "Cycle(III) [2] (c) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(III) [3]
        //
        // (i): $0.08140 → $0.10910 (14.02.2026)
        // (ii): $0.10910 → $0.10910 (14.02.2026) (in-progress)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleIII_P3_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[2].SubWaves[2], 2, "Cycle(III) [3] Intermediate");

        [TestMethod] public void CycleIII_P3_i1_Label() => AssertLabel(_analysis.Waves[2].SubWaves[2].SubWaves[0], WaveNumber.One, "Cycle(III) [3] (i)");
        [TestMethod] public void CycleIII_P3_i1_Start() => AssertPoint(_analysis.Waves[2].SubWaves[2].SubWaves[0].StartPoint, Utc(2026, 2, 6, 0, 0), 0.0814m, "Cycle(III) [3] (i) Start");
        [TestMethod] public void CycleIII_P3_i1_End() => AssertPoint(_analysis.Waves[2].SubWaves[2].SubWaves[0].EndPoint, Utc(2026, 2, 14, 19, 0), 0.1091m, "Cycle(III) [3] (i) End");

        [TestMethod] public void CycleIII_P3_i2_Label() => AssertLabel(_analysis.Waves[2].SubWaves[2].SubWaves[1], WaveNumber.Two, "Cycle(III) [3] (ii)");
        [TestMethod] public void CycleIII_P3_i2_InProgress() => AssertInProgress(_analysis.Waves[2].SubWaves[2].SubWaves[1], true, "Cycle(III) [3] (ii)");
        [TestMethod] public void CycleIII_P3_i2_Start() => AssertPoint(_analysis.Waves[2].SubWaves[2].SubWaves[1].StartPoint, Utc(2026, 2, 14, 19, 0), 0.1091m, "Cycle(III) [3] (ii) Start");

        // ══════════════════════════════════════════════════
        // Projections — Cycle (III): W3 extension of Cycle I ($0.03 → $5.90) from Cycle II end ($0.05)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleIII_Projection_4236() => AssertProjectionTarget(_analysis.Waves[2], LogExtension(0.03m, 5.9m, 0.0465m, 4.236m), 4.236m, "Cycle(III) 4.236 projection");
        [TestMethod] public void CycleIII_Projection_2618() => AssertProjectionTarget(_analysis.Waves[2], LogExtension(0.03m, 5.9m, 0.0465m, 2.618m), 2.618m, "Cycle(III) 2.618 projection");
        [TestMethod] public void CycleIII_Projection_1618() => AssertProjectionTarget(_analysis.Waves[2], LogExtension(0.03m, 5.9m, 0.0465m, 1.618m), 1.618m, "Cycle(III) 1.618 projection");
        [TestMethod] public void CycleIII_Projection_100() => AssertProjectionTarget(_analysis.Waves[2], LogExtension(0.03m, 5.9m, 0.0465m, 1.0m), 1.0m, "Cycle(III) 1.0 projection");

        // ══════════════════════════════════════════════════
        // Projections — Primary [3] inside Cycle (III): W3 extension of P1 ($0.05 → $0.29) from P2 end ($0.08)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleIII_P3_Projection_4236() { var (s, e, b) = GetCycleIII_P1P2(); AssertProjectionTarget(_analysis.Waves[2].SubWaves[2], LogExtension(s, e, b, 4.236m), 4.236m, "Cycle(III) [3] 4.236 projection"); }
        [TestMethod] public void CycleIII_P3_Projection_2618() { var (s, e, b) = GetCycleIII_P1P2(); AssertProjectionTarget(_analysis.Waves[2].SubWaves[2], LogExtension(s, e, b, 2.618m), 2.618m, "Cycle(III) [3] 2.618 projection"); }
        [TestMethod] public void CycleIII_P3_Projection_1618() { var (s, e, b) = GetCycleIII_P1P2(); AssertProjectionTarget(_analysis.Waves[2].SubWaves[2], LogExtension(s, e, b, 1.618m), 1.618m, "Cycle(III) [3] 1.618 projection"); }
        [TestMethod] public void CycleIII_P3_Projection_100() { var (s, e, b) = GetCycleIII_P1P2(); AssertProjectionTarget(_analysis.Waves[2].SubWaves[2], LogExtension(s, e, b, 1.0m), 1.0m, "Cycle(III) [3] 1.0 projection"); }

        private (decimal p1Start, decimal p1End, decimal p2End) GetCycleIII_P1P2()
        {
            Assert.IsTrue(_analysis.Waves[2].SubWaves.Count >= 3, "Cycle(III) should have [1],[2],[3]");
            return (_analysis.Waves[2].SubWaves[0].StartPoint.Price,
                    _analysis.Waves[2].SubWaves[0].EndPoint.Price,
                    _analysis.Waves[2].SubWaves[1].EndPoint.Price);
        }
    }
}
