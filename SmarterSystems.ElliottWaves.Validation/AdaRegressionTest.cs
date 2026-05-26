using SmarterSystems.ElliottWaves.Analyzer.Interfaces;

namespace SmarterSystems.ElliottWaves.Validation
{
    [TestClass]
    public sealed class AdaRegressionTest : RegressionTestBase
    {
        private static ElliottWavesAnalysis _analysis = null!;

        [ClassInitialize]
        public static void Setup(TestContext _)
        {
            _analysis = LoadAndAnalyze("ADAUSDT");
        }

        // ══════════════════════════════════════════════════
        // Cycle Degree: 2 waves (I completed; II in-progress)
        //
        // Cycle (I):  13.03.2020 $0.01765 → 02.09.2021 $3.101
        // Cycle (II): 02.09.2021 $3.101 → in-progress
        // ══════════════════════════════════════════════════

        [TestMethod] public void Cycle_WaveCount() => AssertWaveCount(_analysis, 2, "ADA Cycle");

        // ── Cycle (I): $0.01765 → $3.101 ──

        [TestMethod] public void CycleI_Label() => AssertLabel(_analysis.Waves[0], WaveNumber.One, "Cycle(I)");
        [TestMethod] public void CycleI_NotInProgress() => AssertInProgress(_analysis.Waves[0], false, "Cycle(I)");
        [TestMethod] public void CycleI_Start() => AssertPoint(_analysis.Waves[0].StartPoint, Utc(2020, 3, 13, 2), 0.01765m, "Cycle(I) Start");
        [TestMethod] public void CycleI_End() => AssertPoint(_analysis.Waves[0].EndPoint, Utc(2021, 9, 2, 6), 3.101m, "Cycle(I) End");

        // ── Cycle (II): $3.101 → in-progress ──

        [TestMethod] public void CycleII_Label() => AssertLabel(_analysis.Waves[1], WaveNumber.Two, "Cycle(II)");
        [TestMethod] public void CycleII_InProgress() => AssertInProgress(_analysis.Waves[1], true, "Cycle(II)");
        [TestMethod] public void CycleII_Start() => AssertPoint(_analysis.Waves[1].StartPoint, Utc(2021, 9, 2, 6), 3.101m, "Cycle(II) Start");

        // ══════════════════════════════════════════════════
        // Primary Degree — inside Cycle (I): 5 waves (impulse)
        //
        // [1]: $0.01765 → $0.15464 (26.07.2020)
        // [2]: $0.15464 → $0.07545 (23.09.2020)
        // [3]: $0.07545 → $2.47 (16.05.2021)
        // [4]: $2.47 → $1.0001 (22.06.2021)
        // [5]: $1.0001 → $3.101 (02.09.2021)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleI_Primary_WaveCount() => AssertSubWaveCount(_analysis.Waves[0], 5, "Cycle(I) Primary");

        [TestMethod] public void CycleI_P1_Label() => AssertLabel(_analysis.Waves[0].SubWaves[0], WaveNumber.One, "Cycle(I) [1]");
        [TestMethod] public void CycleI_P1_Start() => AssertPoint(_analysis.Waves[0].SubWaves[0].StartPoint, Utc(2020, 3, 13, 2), 0.01765m, "Cycle(I) [1] Start");
        [TestMethod] public void CycleI_P1_End() => AssertPoint(_analysis.Waves[0].SubWaves[0].EndPoint, Utc(2020, 7, 26, 6), 0.15464m, "Cycle(I) [1] End");

        [TestMethod] public void CycleI_P2_Label() => AssertLabel(_analysis.Waves[0].SubWaves[1], WaveNumber.Two, "Cycle(I) [2]");
        [TestMethod] public void CycleI_P2_Start() => AssertPoint(_analysis.Waves[0].SubWaves[1].StartPoint, Utc(2020, 7, 26, 6), 0.15464m, "Cycle(I) [2] Start");
        [TestMethod] public void CycleI_P2_End() => AssertPoint(_analysis.Waves[0].SubWaves[1].EndPoint, Utc(2020, 9, 23, 20), 0.07545m, "Cycle(I) [2] End");

        [TestMethod] public void CycleI_P3_Label() => AssertLabel(_analysis.Waves[0].SubWaves[2], WaveNumber.Three, "Cycle(I) [3]");
        [TestMethod] public void CycleI_P3_Start() => AssertPoint(_analysis.Waves[0].SubWaves[2].StartPoint, Utc(2020, 9, 23, 20), 0.07545m, "Cycle(I) [3] Start");
        [TestMethod] public void CycleI_P3_End() => AssertPoint(_analysis.Waves[0].SubWaves[2].EndPoint, Utc(2021, 5, 16, 7), 2.47m, "Cycle(I) [3] End");

        [TestMethod] public void CycleI_P4_Label() => AssertLabel(_analysis.Waves[0].SubWaves[3], WaveNumber.Four, "Cycle(I) [4]");
        [TestMethod] public void CycleI_P4_Start() => AssertPoint(_analysis.Waves[0].SubWaves[3].StartPoint, Utc(2021, 5, 16, 7), 2.47m, "Cycle(I) [4] Start");
        [TestMethod] public void CycleI_P4_End() => AssertPoint(_analysis.Waves[0].SubWaves[3].EndPoint, Utc(2021, 6, 22, 13), 1.0001m, "Cycle(I) [4] End");

        [TestMethod] public void CycleI_P5_Label() => AssertLabel(_analysis.Waves[0].SubWaves[4], WaveNumber.Five, "Cycle(I) [5]");
        [TestMethod] public void CycleI_P5_Start() => AssertPoint(_analysis.Waves[0].SubWaves[4].StartPoint, Utc(2021, 6, 22, 13), 1.0001m, "Cycle(I) [5] Start");
        [TestMethod] public void CycleI_P5_End() => AssertPoint(_analysis.Waves[0].SubWaves[4].EndPoint, Utc(2021, 9, 2, 6), 3.101m, "Cycle(I) [5] End");

        // ══════════════════════════════════════════════════
        // Primary Degree — inside Cycle (II): A-B in-progress
        //
        // [A]: $3.101 → $0.22 (10.06.2023)
        // [B]: $0.22 → $0.4541 (03.12.2025) — user says in-progress until target
        // ══════════════════════════════════════════════════

        [TestMethod]
        public void CycleII_HasSubWaves()
        {
            Assert.IsNotNull(_analysis.Waves[1].SubWaves, "Cycle(II) should have sub-waves");
            Assert.IsTrue(_analysis.Waves[1].SubWaves.Count >= 1, "Cycle(II) should have at least 1 primary wave");
        }

        [TestMethod] public void CycleII_A_Label() => AssertLabel(_analysis.Waves[1].SubWaves[0], WaveNumber.A, "Cycle(II) [A]");
        [TestMethod] public void CycleII_A_Start() => AssertPoint(_analysis.Waves[1].SubWaves[0].StartPoint, Utc(2021, 9, 2, 6), 3.101m, "Cycle(II) [A] Start");
        [TestMethod] public void CycleII_A_End() => AssertPoint(_analysis.Waves[1].SubWaves[0].EndPoint, Utc(2023, 6, 10, 4), 0.22m, "Cycle(II) [A] End");

        [TestMethod] public void CycleII_B_Label() => AssertLabel(_analysis.Waves[1].SubWaves[1], WaveNumber.B, "Cycle(II) [B]");
        [TestMethod] public void CycleII_B_Start() => AssertPoint(_analysis.Waves[1].SubWaves[1].StartPoint, Utc(2023, 6, 10, 4), 0.22m, "Cycle(II) [B] Start");
        [TestMethod] public void CycleII_B_End() => AssertPoint(_analysis.Waves[1].SubWaves[1].EndPoint, Utc(2024, 12, 3, 3), 1.3264m, "Cycle(II) [B] End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(I) [1]
        //
        // (i): $0.01765 → $0.02938 (13.03.2020)
        // (ii): $0.02938 → $0.02156 (16.03.2020)
        // (iii): $0.02156 → $0.09039 (04.06.2020)
        // (iv): $0.09039 → $0.06930 (15.06.2020)
        // (v): $0.06930 → $0.15464 (26.07.2020)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleI_P1_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[0].SubWaves[0], 5, "Cycle(I) [1] Intermediate");

        [TestMethod] public void CycleI_P1_i1_Label() => AssertLabel(_analysis.Waves[0].SubWaves[0].SubWaves[0], WaveNumber.One, "Cycle(I) [1] (i)");
        [TestMethod] public void CycleI_P1_i1_Start() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[0].StartPoint, Utc(2020, 3, 13, 2, 0), 0.01765m, "Cycle(I) [1] (i) Start");
        [TestMethod] public void CycleI_P1_i1_End() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[0].EndPoint, Utc(2020, 3, 13, 13, 0), 0.02938m, "Cycle(I) [1] (i) End");

        [TestMethod] public void CycleI_P1_i2_Label() => AssertLabel(_analysis.Waves[0].SubWaves[0].SubWaves[1], WaveNumber.Two, "Cycle(I) [1] (ii)");
        [TestMethod] public void CycleI_P1_i2_Start() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[1].StartPoint, Utc(2020, 3, 13, 13, 0), 0.02938m, "Cycle(I) [1] (ii) Start");
        [TestMethod] public void CycleI_P1_i2_End() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[1].EndPoint, Utc(2020, 3, 16, 10, 0), 0.02156m, "Cycle(I) [1] (ii) End");

        [TestMethod] public void CycleI_P1_i3_Label() => AssertLabel(_analysis.Waves[0].SubWaves[0].SubWaves[2], WaveNumber.Three, "Cycle(I) [1] (iii)");
        [TestMethod] public void CycleI_P1_i3_Start() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[2].StartPoint, Utc(2020, 3, 16, 10, 0), 0.02156m, "Cycle(I) [1] (iii) Start");
        [TestMethod] public void CycleI_P1_i3_End() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[2].EndPoint, Utc(2020, 6, 4, 2, 0), 0.09039m, "Cycle(I) [1] (iii) End");

        [TestMethod] public void CycleI_P1_i4_Label() => AssertLabel(_analysis.Waves[0].SubWaves[0].SubWaves[3], WaveNumber.Four, "Cycle(I) [1] (iv)");
        [TestMethod] public void CycleI_P1_i4_Start() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[3].StartPoint, Utc(2020, 6, 4, 2, 0), 0.09039m, "Cycle(I) [1] (iv) Start");
        [TestMethod] public void CycleI_P1_i4_End() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[3].EndPoint, Utc(2020, 6, 15, 6, 0), 0.0693m, "Cycle(I) [1] (iv) End");

        [TestMethod] public void CycleI_P1_i5_Label() => AssertLabel(_analysis.Waves[0].SubWaves[0].SubWaves[4], WaveNumber.Five, "Cycle(I) [1] (v)");
        [TestMethod] public void CycleI_P1_i5_Start() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[4].StartPoint, Utc(2020, 6, 15, 6, 0), 0.0693m, "Cycle(I) [1] (v) Start");
        [TestMethod] public void CycleI_P1_i5_End() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[4].EndPoint, Utc(2020, 7, 26, 6, 0), 0.15464m, "Cycle(I) [1] (v) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(I) [2]
        //
        // (a): $0.15464 → $0.10211 (27.08.2020)
        // (b): $0.10211 → $0.12812 (01.09.2020)
        // (c): $0.12812 → $0.07545 (23.09.2020)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleI_P2_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[0].SubWaves[1], 3, "Cycle(I) [2] Intermediate");

        [TestMethod] public void CycleI_P2_iA_Label() => AssertLabel(_analysis.Waves[0].SubWaves[1].SubWaves[0], WaveNumber.A, "Cycle(I) [2] (a)");
        [TestMethod] public void CycleI_P2_iA_Start() => AssertPoint(_analysis.Waves[0].SubWaves[1].SubWaves[0].StartPoint, Utc(2020, 7, 26, 6, 0), 0.15464m, "Cycle(I) [2] (a) Start");
        [TestMethod] public void CycleI_P2_iA_End() => AssertPoint(_analysis.Waves[0].SubWaves[1].SubWaves[0].EndPoint, Utc(2020, 8, 27, 17, 0), 0.10211m, "Cycle(I) [2] (a) End");

        [TestMethod] public void CycleI_P2_iB_Label() => AssertLabel(_analysis.Waves[0].SubWaves[1].SubWaves[1], WaveNumber.B, "Cycle(I) [2] (b)");
        [TestMethod] public void CycleI_P2_iB_Start() => AssertPoint(_analysis.Waves[0].SubWaves[1].SubWaves[1].StartPoint, Utc(2020, 8, 27, 17, 0), 0.10211m, "Cycle(I) [2] (b) Start");
        [TestMethod] public void CycleI_P2_iB_End() => AssertPoint(_analysis.Waves[0].SubWaves[1].SubWaves[1].EndPoint, Utc(2020, 9, 1, 22, 0), 0.12812m, "Cycle(I) [2] (b) End");

        [TestMethod] public void CycleI_P2_iC_Label() => AssertLabel(_analysis.Waves[0].SubWaves[1].SubWaves[2], WaveNumber.C, "Cycle(I) [2] (c)");
        [TestMethod] public void CycleI_P2_iC_Start() => AssertPoint(_analysis.Waves[0].SubWaves[1].SubWaves[2].StartPoint, Utc(2020, 9, 1, 22, 0), 0.12812m, "Cycle(I) [2] (c) Start");
        [TestMethod] public void CycleI_P2_iC_End() => AssertPoint(_analysis.Waves[0].SubWaves[1].SubWaves[2].EndPoint, Utc(2020, 9, 23, 20, 0), 0.07545m, "Cycle(I) [2] (c) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(I) [3]
        //
        // (i): $0.07545 → $0.18300 (24.11.2020)
        // (ii): $0.18300 → $0.11230 (26.11.2020)
        // (iii): $0.11230 → $1.5585 (14.04.2021)
        // (iv): $1.5585 → $0.92000 (23.04.2021)
        // (v): $0.92000 → $2.4700 (16.05.2021)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleI_P3_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[0].SubWaves[2], 5, "Cycle(I) [3] Intermediate");

        [TestMethod] public void CycleI_P3_i1_Label() => AssertLabel(_analysis.Waves[0].SubWaves[2].SubWaves[0], WaveNumber.One, "Cycle(I) [3] (i)");
        [TestMethod] public void CycleI_P3_i1_Start() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[0].StartPoint, Utc(2020, 9, 23, 20, 0), 0.07545m, "Cycle(I) [3] (i) Start");
        [TestMethod] public void CycleI_P3_i1_End() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[0].EndPoint, Utc(2020, 11, 24, 5, 0), 0.183m, "Cycle(I) [3] (i) End");

        [TestMethod] public void CycleI_P3_i2_Label() => AssertLabel(_analysis.Waves[0].SubWaves[2].SubWaves[1], WaveNumber.Two, "Cycle(I) [3] (ii)");
        [TestMethod] public void CycleI_P3_i2_Start() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[1].StartPoint, Utc(2020, 11, 24, 5, 0), 0.183m, "Cycle(I) [3] (ii) Start");
        [TestMethod] public void CycleI_P3_i2_End() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[1].EndPoint, Utc(2020, 11, 26, 3, 0), 0.1123m, "Cycle(I) [3] (ii) End");

        [TestMethod] public void CycleI_P3_i3_Label() => AssertLabel(_analysis.Waves[0].SubWaves[2].SubWaves[2], WaveNumber.Three, "Cycle(I) [3] (iii)");
        [TestMethod] public void CycleI_P3_i3_Start() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[2].StartPoint, Utc(2020, 11, 26, 3, 0), 0.1123m, "Cycle(I) [3] (iii) Start");
        [TestMethod] public void CycleI_P3_i3_End() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[2].EndPoint, Utc(2021, 4, 14, 8, 0), 1.55845m, "Cycle(I) [3] (iii) End");

        [TestMethod] public void CycleI_P3_i4_Label() => AssertLabel(_analysis.Waves[0].SubWaves[2].SubWaves[3], WaveNumber.Four, "Cycle(I) [3] (iv)");
        [TestMethod] public void CycleI_P3_i4_Start() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[3].StartPoint, Utc(2021, 4, 14, 8, 0), 1.55845m, "Cycle(I) [3] (iv) Start");
        [TestMethod] public void CycleI_P3_i4_End() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[3].EndPoint, Utc(2021, 4, 23, 2, 0), 0.92m, "Cycle(I) [3] (iv) End");

        [TestMethod] public void CycleI_P3_i5_Label() => AssertLabel(_analysis.Waves[0].SubWaves[2].SubWaves[4], WaveNumber.Five, "Cycle(I) [3] (v)");
        [TestMethod] public void CycleI_P3_i5_Start() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[4].StartPoint, Utc(2021, 4, 23, 2, 0), 0.92m, "Cycle(I) [3] (v) Start");
        [TestMethod] public void CycleI_P3_i5_End() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[4].EndPoint, Utc(2021, 5, 16, 7, 0), 2.47m, "Cycle(I) [3] (v) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(I) [4]
        //
        // (a): $2.4700 → $1.0100 (19.05.2021)
        // (b): $1.0100 → $1.9487 (20.05.2021)
        // (c): $1.9487 → $1.0001 (22.06.2021)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleI_P4_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[0].SubWaves[3], 3, "Cycle(I) [4] Intermediate");

        [TestMethod] public void CycleI_P4_iA_Label() => AssertLabel(_analysis.Waves[0].SubWaves[3].SubWaves[0], WaveNumber.A, "Cycle(I) [4] (a)");
        [TestMethod] public void CycleI_P4_iA_Start() => AssertPoint(_analysis.Waves[0].SubWaves[3].SubWaves[0].StartPoint, Utc(2021, 5, 16, 7, 0), 2.47m, "Cycle(I) [4] (a) Start");
        [TestMethod] public void CycleI_P4_iA_End() => AssertPoint(_analysis.Waves[0].SubWaves[3].SubWaves[0].EndPoint, Utc(2021, 5, 19, 12, 0), 1.01m, "Cycle(I) [4] (a) End");

        [TestMethod] public void CycleI_P4_iB_Label() => AssertLabel(_analysis.Waves[0].SubWaves[3].SubWaves[1], WaveNumber.B, "Cycle(I) [4] (b)");
        [TestMethod] public void CycleI_P4_iB_Start() => AssertPoint(_analysis.Waves[0].SubWaves[3].SubWaves[1].StartPoint, Utc(2021, 5, 19, 12, 0), 1.01m, "Cycle(I) [4] (b) Start");
        [TestMethod] public void CycleI_P4_iB_End() => AssertPoint(_analysis.Waves[0].SubWaves[3].SubWaves[1].EndPoint, Utc(2021, 5, 20, 13, 0), 1.9487m, "Cycle(I) [4] (b) End");

        [TestMethod] public void CycleI_P4_iC_Label() => AssertLabel(_analysis.Waves[0].SubWaves[3].SubWaves[2], WaveNumber.C, "Cycle(I) [4] (c)");
        [TestMethod] public void CycleI_P4_iC_Start() => AssertPoint(_analysis.Waves[0].SubWaves[3].SubWaves[2].StartPoint, Utc(2021, 5, 20, 13, 0), 1.9487m, "Cycle(I) [4] (c) Start");
        [TestMethod] public void CycleI_P4_iC_End() => AssertPoint(_analysis.Waves[0].SubWaves[3].SubWaves[2].EndPoint, Utc(2021, 6, 22, 13, 0), 1.0001m, "Cycle(I) [4] (c) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(I) [5]
        //
        // (i): $1.0001 → $1.4955 (04.07.2021)
        // (ii): $1.4955 → $1.0202 (20.07.2021)
        // (iii): $1.0202 → $2.9719 (23.08.2021)
        // (iv): $2.9719 → $2.4700 (26.08.2021)
        // (v): $2.4700 → $3.1010 (02.09.2021)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleI_P5_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[0].SubWaves[4], 5, "Cycle(I) [5] Intermediate");

        [TestMethod] public void CycleI_P5_i1_Label() => AssertLabel(_analysis.Waves[0].SubWaves[4].SubWaves[0], WaveNumber.One, "Cycle(I) [5] (i)");
        [TestMethod] public void CycleI_P5_i1_Start() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[0].StartPoint, Utc(2021, 6, 22, 13, 0), 1.0001m, "Cycle(I) [5] (i) Start");
        [TestMethod] public void CycleI_P5_i1_End() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[0].EndPoint, Utc(2021, 7, 4, 21, 0), 1.4955m, "Cycle(I) [5] (i) End");

        [TestMethod] public void CycleI_P5_i2_Label() => AssertLabel(_analysis.Waves[0].SubWaves[4].SubWaves[1], WaveNumber.Two, "Cycle(I) [5] (ii)");
        [TestMethod] public void CycleI_P5_i2_Start() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[1].StartPoint, Utc(2021, 7, 4, 21, 0), 1.4955m, "Cycle(I) [5] (ii) Start");
        [TestMethod] public void CycleI_P5_i2_End() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[1].EndPoint, Utc(2021, 7, 20, 10, 0), 1.0202m, "Cycle(I) [5] (ii) End");

        [TestMethod] public void CycleI_P5_i3_Label() => AssertLabel(_analysis.Waves[0].SubWaves[4].SubWaves[2], WaveNumber.Three, "Cycle(I) [5] (iii)");
        [TestMethod] public void CycleI_P5_i3_Start() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[2].StartPoint, Utc(2021, 7, 20, 10, 0), 1.0202m, "Cycle(I) [5] (iii) Start");
        [TestMethod] public void CycleI_P5_i3_End() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[2].EndPoint, Utc(2021, 8, 23, 22, 0), 2.9719m, "Cycle(I) [5] (iii) End");

        [TestMethod] public void CycleI_P5_i4_Label() => AssertLabel(_analysis.Waves[0].SubWaves[4].SubWaves[3], WaveNumber.Four, "Cycle(I) [5] (iv)");
        [TestMethod] public void CycleI_P5_i4_Start() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[3].StartPoint, Utc(2021, 8, 23, 22, 0), 2.9719m, "Cycle(I) [5] (iv) Start");
        [TestMethod] public void CycleI_P5_i4_End() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[3].EndPoint, Utc(2021, 8, 26, 19, 0), 2.47m, "Cycle(I) [5] (iv) End");

        [TestMethod] public void CycleI_P5_i5_Label() => AssertLabel(_analysis.Waves[0].SubWaves[4].SubWaves[4], WaveNumber.Five, "Cycle(I) [5] (v)");
        [TestMethod] public void CycleI_P5_i5_Start() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[4].StartPoint, Utc(2021, 8, 26, 19, 0), 2.47m, "Cycle(I) [5] (v) Start");
        [TestMethod] public void CycleI_P5_i5_End() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[4].EndPoint, Utc(2021, 9, 2, 6, 0), 3.101m, "Cycle(I) [5] (v) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(II) [A]
        //
        // (i): $3.1010 → $1.4160 (28.11.2021)
        // (ii): $1.4160 → $1.7600 (02.12.2021)
        // (iii): $1.7600 → $0.23920 (30.12.2022)
        // (iv): $0.23920 → $0.46170 (15.04.2023)
        // (v): $0.46170 → $0.22000 (10.06.2023)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleII_PA_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[1].SubWaves[0], 5, "Cycle(II) [A] Intermediate");

        [TestMethod] public void CycleII_PA_i1_Label() => AssertLabel(_analysis.Waves[1].SubWaves[0].SubWaves[0], WaveNumber.One, "Cycle(II) [A] (i)");
        [TestMethod] public void CycleII_PA_i1_Start() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[0].StartPoint, Utc(2021, 9, 2, 6, 0), 3.101m, "Cycle(II) [A] (i) Start");
        [TestMethod] public void CycleII_PA_i1_End() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[0].EndPoint, Utc(2021, 11, 28, 18, 0), 1.416m, "Cycle(II) [A] (i) End");

        [TestMethod] public void CycleII_PA_i2_Label() => AssertLabel(_analysis.Waves[1].SubWaves[0].SubWaves[1], WaveNumber.Two, "Cycle(II) [A] (ii)");
        [TestMethod] public void CycleII_PA_i2_Start() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[1].StartPoint, Utc(2021, 11, 28, 18, 0), 1.416m, "Cycle(II) [A] (ii) Start");
        [TestMethod] public void CycleII_PA_i2_End() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[1].EndPoint, Utc(2021, 12, 2, 18, 0), 1.76m, "Cycle(II) [A] (ii) End");

        [TestMethod] public void CycleII_PA_i3_Label() => AssertLabel(_analysis.Waves[1].SubWaves[0].SubWaves[2], WaveNumber.Three, "Cycle(II) [A] (iii)");
        [TestMethod] public void CycleII_PA_i3_Start() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[2].StartPoint, Utc(2021, 12, 2, 18, 0), 1.76m, "Cycle(II) [A] (iii) Start");
        [TestMethod] public void CycleII_PA_i3_End() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[2].EndPoint, Utc(2022, 12, 30, 7, 0), 0.2392m, "Cycle(II) [A] (iii) End");

        [TestMethod] public void CycleII_PA_i4_Label() => AssertLabel(_analysis.Waves[1].SubWaves[0].SubWaves[3], WaveNumber.Four, "Cycle(II) [A] (iv)");
        [TestMethod] public void CycleII_PA_i4_Start() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[3].StartPoint, Utc(2022, 12, 30, 7, 0), 0.2392m, "Cycle(II) [A] (iv) Start");
        [TestMethod] public void CycleII_PA_i4_End() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[3].EndPoint, Utc(2023, 4, 15, 13, 0), 0.4617m, "Cycle(II) [A] (iv) End");

        [TestMethod] public void CycleII_PA_i5_Label() => AssertLabel(_analysis.Waves[1].SubWaves[0].SubWaves[4], WaveNumber.Five, "Cycle(II) [A] (v)");
        [TestMethod] public void CycleII_PA_i5_Start() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[4].StartPoint, Utc(2023, 4, 15, 13, 0), 0.4617m, "Cycle(II) [A] (v) Start");
        [TestMethod] public void CycleII_PA_i5_End() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[4].EndPoint, Utc(2023, 6, 10, 4, 0), 0.22m, "Cycle(II) [A] (v) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(II) [B]
        //
        // (a): $0.22000 → $0.81040 (14.03.2024)
        // (b): $0.81040 → $0.27560 (05.08.2024)
        // (c): $0.27560 → $1.3264 (03.12.2024)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleII_PB_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[1].SubWaves[1], 3, "Cycle(II) [B] Intermediate");

        [TestMethod] public void CycleII_PB_iA_Label() => AssertLabel(_analysis.Waves[1].SubWaves[1].SubWaves[0], WaveNumber.A, "Cycle(II) [B] (a)");
        [TestMethod] public void CycleII_PB_iA_Start() => AssertPoint(_analysis.Waves[1].SubWaves[1].SubWaves[0].StartPoint, Utc(2023, 6, 10, 4, 0), 0.22m, "Cycle(II) [B] (a) Start");
        [TestMethod] public void CycleII_PB_iA_End() => AssertPoint(_analysis.Waves[1].SubWaves[1].SubWaves[0].EndPoint, Utc(2024, 3, 14, 10, 0), 0.8104m, "Cycle(II) [B] (a) End");

        [TestMethod] public void CycleII_PB_iB_Label() => AssertLabel(_analysis.Waves[1].SubWaves[1].SubWaves[1], WaveNumber.B, "Cycle(II) [B] (b)");
        [TestMethod] public void CycleII_PB_iB_Start() => AssertPoint(_analysis.Waves[1].SubWaves[1].SubWaves[1].StartPoint, Utc(2024, 3, 14, 10, 0), 0.8104m, "Cycle(II) [B] (b) Start");
        [TestMethod] public void CycleII_PB_iB_End() => AssertPoint(_analysis.Waves[1].SubWaves[1].SubWaves[1].EndPoint, Utc(2024, 8, 5, 6, 0), 0.2756m, "Cycle(II) [B] (b) End");

        [TestMethod] public void CycleII_PB_iC_Label() => AssertLabel(_analysis.Waves[1].SubWaves[1].SubWaves[2], WaveNumber.C, "Cycle(II) [B] (c)");
        [TestMethod] public void CycleII_PB_iC_Start() => AssertPoint(_analysis.Waves[1].SubWaves[1].SubWaves[2].StartPoint, Utc(2024, 8, 5, 6, 0), 0.2756m, "Cycle(II) [B] (c) Start");
        [TestMethod] public void CycleII_PB_iC_End() => AssertPoint(_analysis.Waves[1].SubWaves[1].SubWaves[2].EndPoint, Utc(2024, 12, 3, 3, 0), 1.3264m, "Cycle(II) [B] (c) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(II) [C]
        //
        // (i): $1.3264 → $0.50600 (03.02.2025)
        // (ii): $0.50600 → $1.1747 (03.03.2025)
        // (iii): $1.1747 → $1.1747 (03.03.2025) (in-progress)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleII_PC_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[1].SubWaves[2], 3, "Cycle(II) [C] Intermediate");

        [TestMethod] public void CycleII_PC_i1_Label() => AssertLabel(_analysis.Waves[1].SubWaves[2].SubWaves[0], WaveNumber.One, "Cycle(II) [C] (i)");
        [TestMethod] public void CycleII_PC_i1_Start() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[0].StartPoint, Utc(2024, 12, 3, 3, 0), 1.3264m, "Cycle(II) [C] (i) Start");
        [TestMethod] public void CycleII_PC_i1_End() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[0].EndPoint, Utc(2025, 2, 3, 2, 0), 0.506m, "Cycle(II) [C] (i) End");

        [TestMethod] public void CycleII_PC_i2_Label() => AssertLabel(_analysis.Waves[1].SubWaves[2].SubWaves[1], WaveNumber.Two, "Cycle(II) [C] (ii)");
        [TestMethod] public void CycleII_PC_i2_Start() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[1].StartPoint, Utc(2025, 2, 3, 2, 0), 0.506m, "Cycle(II) [C] (ii) Start");
        [TestMethod] public void CycleII_PC_i2_End() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[1].EndPoint, Utc(2025, 3, 3, 0, 0), 1.1747m, "Cycle(II) [C] (ii) End");

        [TestMethod] public void CycleII_PC_i3_Label() => AssertLabel(_analysis.Waves[1].SubWaves[2].SubWaves[2], WaveNumber.Three, "Cycle(II) [C] (iii)");
        [TestMethod] public void CycleII_PC_i3_InProgress() => AssertInProgress(_analysis.Waves[1].SubWaves[2].SubWaves[2], true, "Cycle(II) [C] (iii)");
        [TestMethod] public void CycleII_PC_i3_Start() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[2].StartPoint, Utc(2025, 3, 3, 0, 0), 1.1747m, "Cycle(II) [C] (iii) Start");

        // ══════════════════════════════════════════════════
        // Projections — Cycle (II): W2 retracement of Cycle I ($0.02 → $3.10)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleII_Projection_0786() => AssertProjectionTarget(_analysis.Waves[1], LogRetracement(0.01765m, 3.101m, 0.786m), 0.786m, "Cycle(II) 0.786 projection");
        [TestMethod] public void CycleII_Projection_0618() => AssertProjectionTarget(_analysis.Waves[1], LogRetracement(0.01765m, 3.101m, 0.618m), 0.618m, "Cycle(II) 0.618 projection");
        [TestMethod] public void CycleII_Projection_050() => AssertProjectionTarget(_analysis.Waves[1], LogRetracement(0.01765m, 3.101m, 0.5m), 0.5m, "Cycle(II) 0.5 projection");
        [TestMethod] public void CycleII_Projection_0382() => AssertProjectionTarget(_analysis.Waves[1], LogRetracement(0.01765m, 3.101m, 0.382m), 0.382m, "Cycle(II) 0.382 projection");

        // ══════════════════════════════════════════════════
        // Projections — Primary [C] inside Cycle (II): C extension of A ($3.10 → $0.22) from B end ($1.3264)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleII_C_Projection_2618() => AssertProjectionTarget(_analysis.Waves[1].SubWaves[2], LogExtension(3.10m, 0.22m, 1.3264m, 2.618m), 2.618m, "Cycle(II) [C] 2.618 projection");
        [TestMethod] public void CycleII_C_Projection_1618() => AssertProjectionTarget(_analysis.Waves[1].SubWaves[2], LogExtension(3.10m, 0.22m, 1.3264m, 1.618m), 1.618m, "Cycle(II) [C] 1.618 projection");
        [TestMethod] public void CycleII_C_Projection_AEqualsC() => AssertProjectionTarget(_analysis.Waves[1].SubWaves[2], LogExtension(3.10m, 0.22m, 1.3264m, 1.0m), 1.0m, "Cycle(II) [C] A=C projection");
        [TestMethod] public void CycleII_C_Projection_0618() => AssertProjectionTarget(_analysis.Waves[1].SubWaves[2], LogExtension(3.10m, 0.22m, 1.3264m, 0.618m), 0.618m, "Cycle(II) [C] 0.618 projection");
    }
}
