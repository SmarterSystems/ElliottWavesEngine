using SmarterSystems.ElliottWaves.Analyzer.Interfaces;

namespace SmarterSystems.ElliottWaves.Validation
{
    [TestClass]
    public sealed class LinkRegressionTest : RegressionTestBase
    {
        private static ElliottWavesAnalysis _analysis = null!;

        [ClassInitialize]
        public static void Setup(TestContext _)
        {
            _analysis = LoadAndAnalyze("LINKUSDT");
        }

        // ══════════════════════════════════════════════════
        // Cycle Degree: 3 waves (I, II completed; III in-progress)
        //
        // Cycle (I):   29.01.2019 $0.4061 → 10.05.2021 $53.00
        // Cycle (II):  10.05.2021 $53.00 → 10.06.2023 $4.761
        // Cycle (III): 10.06.2023 $4.761 → in-progress
        // ══════════════════════════════════════════════════

        [TestMethod] public void Cycle_WaveCount() => AssertWaveCount(_analysis, 3, "LINK Cycle");

        // ── Cycle (I): $0.4061 → $53.00 ──

        [TestMethod] public void CycleI_Label() => AssertLabel(_analysis.Waves[0], WaveNumber.One, "Cycle(I)");
        [TestMethod] public void CycleI_NotInProgress() => AssertInProgress(_analysis.Waves[0], false, "Cycle(I)");
        [TestMethod] public void CycleI_Start() => AssertPoint(_analysis.Waves[0].StartPoint, Utc(2019, 1, 28, 15), 0.3504m, "Cycle(I) Start");
        [TestMethod] public void CycleI_End() => AssertPoint(_analysis.Waves[0].EndPoint, Utc(2021, 5, 10, 0), 53.00m, "Cycle(I) End");

        // ── Cycle (II): $53.00 → $4.761 ──

        [TestMethod] public void CycleII_Label() => AssertLabel(_analysis.Waves[1], WaveNumber.Two, "Cycle(II)");
        [TestMethod] public void CycleII_NotInProgress() => AssertInProgress(_analysis.Waves[1], false, "Cycle(II)");
        [TestMethod] public void CycleII_Start() => AssertPoint(_analysis.Waves[1].StartPoint, Utc(2021, 5, 10, 0), 53.00m, "Cycle(II) Start");
        [TestMethod] public void CycleII_End() => AssertPoint(_analysis.Waves[1].EndPoint, Utc(2023, 6, 10, 4), 4.761m, "Cycle(II) End");

        // ── Cycle (III): $4.761 → in-progress ──

        [TestMethod] public void CycleIII_Label() => AssertLabel(_analysis.Waves[2], WaveNumber.Three, "Cycle(III)");
        [TestMethod] public void CycleIII_InProgress() => AssertInProgress(_analysis.Waves[2], true, "Cycle(III)");
        [TestMethod] public void CycleIII_Start() => AssertPoint(_analysis.Waves[2].StartPoint, Utc(2023, 6, 10, 4), 4.761m, "Cycle(III) Start");

        // ══════════════════════════════════════════════════
        // Primary Degree — inside Cycle (I): 5 waves (impulse)
        //
        // [1]: $0.4061 → $4.5826 (29.06.2019)
        // [2]: $4.5826 → $1.36 (13.03.2020)
        // [3]: $1.36 → $14.0551 (10.08.2020)
        // [4]: $14.0551 → $7.2869 (23.09.2020)
        // [5]: $7.2869 → $53.00 (10.05.2021)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleI_Primary_WaveCount() => AssertSubWaveCount(_analysis.Waves[0], 5, "Cycle(I) Primary");

        [TestMethod] public void CycleI_P1_Label() => AssertLabel(_analysis.Waves[0].SubWaves[0], WaveNumber.One, "Cycle(I) [1]");
        [TestMethod] public void CycleI_P1_Start() => AssertPoint(_analysis.Waves[0].SubWaves[0].StartPoint, Utc(2019, 1, 28, 15), 0.3504m, "Cycle(I) [1] Start");
        [TestMethod] public void CycleI_P1_End() => AssertPoint(_analysis.Waves[0].SubWaves[0].EndPoint, Utc(2019, 5, 22, 17), 1.474m, "Cycle(I) [1] End");

        [TestMethod] public void CycleI_P2_Label() => AssertLabel(_analysis.Waves[0].SubWaves[1], WaveNumber.Two, "Cycle(I) [2]");
        [TestMethod] public void CycleI_P2_Start() => AssertPoint(_analysis.Waves[0].SubWaves[1].StartPoint, Utc(2019, 5, 22, 17), 1.474m, "Cycle(I) [2] Start");
        [TestMethod] public void CycleI_P2_End() => AssertPoint(_analysis.Waves[0].SubWaves[1].EndPoint, Utc(2019, 6, 4, 0), 0.8368m, "Cycle(I) [2] End");

        [TestMethod] public void CycleI_P3_Label() => AssertLabel(_analysis.Waves[0].SubWaves[2], WaveNumber.Three, "Cycle(I) [3]");
        [TestMethod] public void CycleI_P3_Start() => AssertPoint(_analysis.Waves[0].SubWaves[2].StartPoint, Utc(2019, 6, 4, 0), 0.8368m, "Cycle(I) [3] Start");
        [TestMethod] public void CycleI_P3_End() => AssertPoint(_analysis.Waves[0].SubWaves[2].EndPoint, Utc(2020, 8, 16, 7), 20.1111m, "Cycle(I) [3] End");

        [TestMethod] public void CycleI_P4_Label() => AssertLabel(_analysis.Waves[0].SubWaves[3], WaveNumber.Four, "Cycle(I) [4]");
        [TestMethod] public void CycleI_P4_Start() => AssertPoint(_analysis.Waves[0].SubWaves[3].StartPoint, Utc(2020, 8, 16, 7), 20.1111m, "Cycle(I) [4] Start");
        [TestMethod] public void CycleI_P4_End() => AssertPoint(_analysis.Waves[0].SubWaves[3].EndPoint, Utc(2020, 9, 23, 20), 7.2869m, "Cycle(I) [4] End");

        [TestMethod] public void CycleI_P5_Label() => AssertLabel(_analysis.Waves[0].SubWaves[4], WaveNumber.Five, "Cycle(I) [5]");
        [TestMethod] public void CycleI_P5_Start() => AssertPoint(_analysis.Waves[0].SubWaves[4].StartPoint, Utc(2020, 9, 23, 20), 7.2869m, "Cycle(I) [5] Start");
        [TestMethod] public void CycleI_P5_End() => AssertPoint(_analysis.Waves[0].SubWaves[4].EndPoint, Utc(2021, 5, 10, 0), 53.00m, "Cycle(I) [5] End");

        // ══════════════════════════════════════════════════
        // Primary Degree — inside Cycle (II): A-B-C (corrective)
        //
        // [A]: $53.00 → $13.384 (20.07.2021)
        // [B]: $13.384 → $38.31 (10.11.2021)
        // [C]: $38.31 → $4.761 (10.06.2023)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleII_Primary_WaveCount() => AssertSubWaveCount(_analysis.Waves[1], 3, "Cycle(II) Primary");

        [TestMethod] public void CycleII_A_Label() => AssertLabel(_analysis.Waves[1].SubWaves[0], WaveNumber.A, "Cycle(II) [A]");
        [TestMethod] public void CycleII_A_Start() => AssertPoint(_analysis.Waves[1].SubWaves[0].StartPoint, Utc(2021, 5, 10, 0), 53.00m, "Cycle(II) [A] Start");
        [TestMethod] public void CycleII_A_End() => AssertPoint(_analysis.Waves[1].SubWaves[0].EndPoint, Utc(2021, 7, 20, 3), 13.384m, "Cycle(II) [A] End");

        [TestMethod] public void CycleII_B_Label() => AssertLabel(_analysis.Waves[1].SubWaves[1], WaveNumber.B, "Cycle(II) [B]");
        [TestMethod] public void CycleII_B_Start() => AssertPoint(_analysis.Waves[1].SubWaves[1].StartPoint, Utc(2021, 7, 20, 3), 13.384m, "Cycle(II) [B] Start");
        [TestMethod] public void CycleII_B_End() => AssertPoint(_analysis.Waves[1].SubWaves[1].EndPoint, Utc(2021, 11, 10, 15), 38.31m, "Cycle(II) [B] End");

        [TestMethod] public void CycleII_C_Label() => AssertLabel(_analysis.Waves[1].SubWaves[2], WaveNumber.C, "Cycle(II) [C]");
        [TestMethod] public void CycleII_C_Start() => AssertPoint(_analysis.Waves[1].SubWaves[2].StartPoint, Utc(2021, 11, 10, 15), 38.31m, "Cycle(II) [C] Start");
        [TestMethod] public void CycleII_C_End() => AssertPoint(_analysis.Waves[1].SubWaves[2].EndPoint, Utc(2023, 6, 10, 4), 4.761m, "Cycle(II) [C] End");

        // ══════════════════════════════════════════════════
        // Primary Degree — inside Cycle (III): in-progress
        //
        // [1]: $4.761 → $30.94 (13.12.2024)
        // [2]: $30.94 → $7.15 (06.02.2026)
        // [3]: in-progress
        // ══════════════════════════════════════════════════

        [TestMethod]
        public void CycleIII_HasSubWaves()
        {
            Assert.IsNotNull(_analysis.Waves[2].SubWaves, "Cycle(III) should have sub-waves");
            Assert.IsTrue(_analysis.Waves[2].SubWaves.Count >= 2, "Cycle(III) should have at least 2 primary waves");
        }

        [TestMethod] public void CycleIII_P1_Label() => AssertLabel(_analysis.Waves[2].SubWaves[0], WaveNumber.One, "Cycle(III) [1]");
        [TestMethod] public void CycleIII_P1_Start() => AssertPoint(_analysis.Waves[2].SubWaves[0].StartPoint, Utc(2023, 6, 10, 4), 4.761m, "Cycle(III) [1] Start");
        [TestMethod] public void CycleIII_P1_End() => AssertPoint(_analysis.Waves[2].SubWaves[0].EndPoint, Utc(2024, 12, 13, 2), 30.94m, "Cycle(III) [1] End");

        [TestMethod] public void CycleIII_P2_Label() => AssertLabel(_analysis.Waves[2].SubWaves[1], WaveNumber.Two, "Cycle(III) [2]");
        [TestMethod] public void CycleIII_P2_Start() => AssertPoint(_analysis.Waves[2].SubWaves[1].StartPoint, Utc(2024, 12, 13, 2), 30.94m, "Cycle(III) [2] Start");
        [TestMethod] public void CycleIII_P2_End() => AssertPoint(_analysis.Waves[2].SubWaves[1].EndPoint, Utc(2026, 2, 6, 0), 7.15m, "Cycle(III) [2] End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(I) [1]
        //
        // (i): $0.35040 → $0.46380 (29.01.2019)
        // (ii): $0.46380 → $0.38000 (01.02.2019)
        // (iii): $0.38000 → $0.95650 (14.05.2019)
        // (iv): $0.95650 → $0.75510 (16.05.2019)
        // (v): $0.75510 → $1.4740 (22.05.2019)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleI_P1_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[0].SubWaves[0], 5, "Cycle(I) [1] Intermediate");

        [TestMethod] public void CycleI_P1_i1_Label() => AssertLabel(_analysis.Waves[0].SubWaves[0].SubWaves[0], WaveNumber.One, "Cycle(I) [1] (i)");
        [TestMethod] public void CycleI_P1_i1_Start() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[0].StartPoint, Utc(2019, 1, 28, 15, 0), 0.3504m, "Cycle(I) [1] (i) Start");
        [TestMethod] public void CycleI_P1_i1_End() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[0].EndPoint, Utc(2019, 1, 29, 19, 0), 0.4638m, "Cycle(I) [1] (i) End");

        [TestMethod] public void CycleI_P1_i2_Label() => AssertLabel(_analysis.Waves[0].SubWaves[0].SubWaves[1], WaveNumber.Two, "Cycle(I) [1] (ii)");
        [TestMethod] public void CycleI_P1_i2_Start() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[1].StartPoint, Utc(2019, 1, 29, 19, 0), 0.4638m, "Cycle(I) [1] (ii) Start");
        [TestMethod] public void CycleI_P1_i2_End() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[1].EndPoint, Utc(2019, 2, 1, 1, 0), 0.38m, "Cycle(I) [1] (ii) End");

        [TestMethod] public void CycleI_P1_i3_Label() => AssertLabel(_analysis.Waves[0].SubWaves[0].SubWaves[2], WaveNumber.Three, "Cycle(I) [1] (iii)");
        [TestMethod] public void CycleI_P1_i3_Start() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[2].StartPoint, Utc(2019, 2, 1, 1, 0), 0.38m, "Cycle(I) [1] (iii) Start");
        [TestMethod] public void CycleI_P1_i3_End() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[2].EndPoint, Utc(2019, 5, 14, 15, 0), 0.9565m, "Cycle(I) [1] (iii) End");

        [TestMethod] public void CycleI_P1_i4_Label() => AssertLabel(_analysis.Waves[0].SubWaves[0].SubWaves[3], WaveNumber.Four, "Cycle(I) [1] (iv)");
        [TestMethod] public void CycleI_P1_i4_Start() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[3].StartPoint, Utc(2019, 5, 14, 15, 0), 0.9565m, "Cycle(I) [1] (iv) Start");
        [TestMethod] public void CycleI_P1_i4_End() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[3].EndPoint, Utc(2019, 5, 16, 10, 0), 0.7551m, "Cycle(I) [1] (iv) End");

        [TestMethod] public void CycleI_P1_i5_Label() => AssertLabel(_analysis.Waves[0].SubWaves[0].SubWaves[4], WaveNumber.Five, "Cycle(I) [1] (v)");
        [TestMethod] public void CycleI_P1_i5_Start() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[4].StartPoint, Utc(2019, 5, 16, 10, 0), 0.7551m, "Cycle(I) [1] (v) Start");
        [TestMethod] public void CycleI_P1_i5_End() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[4].EndPoint, Utc(2019, 5, 22, 17, 0), 1.474m, "Cycle(I) [1] (v) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(I) [2]
        //
        // (a): $1.4740 → $1.0340 (26.05.2019)
        // (b): $1.0340 → $1.3500 (28.05.2019)
        // (c): $1.3500 → $0.83680 (04.06.2019)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleI_P2_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[0].SubWaves[1], 3, "Cycle(I) [2] Intermediate");

        [TestMethod] public void CycleI_P2_iA_Label() => AssertLabel(_analysis.Waves[0].SubWaves[1].SubWaves[0], WaveNumber.A, "Cycle(I) [2] (a)");
        [TestMethod] public void CycleI_P2_iA_Start() => AssertPoint(_analysis.Waves[0].SubWaves[1].SubWaves[0].StartPoint, Utc(2019, 5, 22, 17, 0), 1.474m, "Cycle(I) [2] (a) Start");
        [TestMethod] public void CycleI_P2_iA_End() => AssertPoint(_analysis.Waves[0].SubWaves[1].SubWaves[0].EndPoint, Utc(2019, 5, 26, 20, 0), 1.034m, "Cycle(I) [2] (a) End");

        [TestMethod] public void CycleI_P2_iB_Label() => AssertLabel(_analysis.Waves[0].SubWaves[1].SubWaves[1], WaveNumber.B, "Cycle(I) [2] (b)");
        [TestMethod] public void CycleI_P2_iB_Start() => AssertPoint(_analysis.Waves[0].SubWaves[1].SubWaves[1].StartPoint, Utc(2019, 5, 26, 20, 0), 1.034m, "Cycle(I) [2] (b) Start");
        [TestMethod] public void CycleI_P2_iB_End() => AssertPoint(_analysis.Waves[0].SubWaves[1].SubWaves[1].EndPoint, Utc(2019, 5, 28, 20, 0), 1.35m, "Cycle(I) [2] (b) End");

        [TestMethod] public void CycleI_P2_iC_Label() => AssertLabel(_analysis.Waves[0].SubWaves[1].SubWaves[2], WaveNumber.C, "Cycle(I) [2] (c)");
        [TestMethod] public void CycleI_P2_iC_Start() => AssertPoint(_analysis.Waves[0].SubWaves[1].SubWaves[2].StartPoint, Utc(2019, 5, 28, 20, 0), 1.35m, "Cycle(I) [2] (c) Start");
        [TestMethod] public void CycleI_P2_iC_End() => AssertPoint(_analysis.Waves[0].SubWaves[1].SubWaves[2].EndPoint, Utc(2019, 6, 4, 0, 0), 0.8368m, "Cycle(I) [2] (c) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(I) [3]
        //
        // (i): $0.83680 → $1.2700 (06.06.2019)
        // (ii): $1.2700 → $1.0085 (09.06.2019)
        // (iii): $1.0085 → $4.9762 (04.03.2020)
        // (iv): $4.9762 → $1.3600 (13.03.2020)
        // (v): $1.3600 → $20.1111 (16.08.2020)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleI_P3_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[0].SubWaves[2], 5, "Cycle(I) [3] Intermediate");

        [TestMethod] public void CycleI_P3_i1_Label() => AssertLabel(_analysis.Waves[0].SubWaves[2].SubWaves[0], WaveNumber.One, "Cycle(I) [3] (i)");
        [TestMethod] public void CycleI_P3_i1_Start() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[0].StartPoint, Utc(2019, 6, 4, 0, 0), 0.8368m, "Cycle(I) [3] (i) Start");
        [TestMethod] public void CycleI_P3_i1_End() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[0].EndPoint, Utc(2019, 6, 6, 14, 0), 1.27m, "Cycle(I) [3] (i) End");

        [TestMethod] public void CycleI_P3_i2_Label() => AssertLabel(_analysis.Waves[0].SubWaves[2].SubWaves[1], WaveNumber.Two, "Cycle(I) [3] (ii)");
        [TestMethod] public void CycleI_P3_i2_Start() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[1].StartPoint, Utc(2019, 6, 6, 14, 0), 1.27m, "Cycle(I) [3] (ii) Start");
        [TestMethod] public void CycleI_P3_i2_End() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[1].EndPoint, Utc(2019, 6, 9, 16, 0), 1.0085m, "Cycle(I) [3] (ii) End");

        [TestMethod] public void CycleI_P3_i3_Label() => AssertLabel(_analysis.Waves[0].SubWaves[2].SubWaves[2], WaveNumber.Three, "Cycle(I) [3] (iii)");
        [TestMethod] public void CycleI_P3_i3_Start() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[2].StartPoint, Utc(2019, 6, 9, 16, 0), 1.0085m, "Cycle(I) [3] (iii) Start");
        [TestMethod] public void CycleI_P3_i3_End() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[2].EndPoint, Utc(2020, 3, 4, 14, 0), 4.9762m, "Cycle(I) [3] (iii) End");

        [TestMethod] public void CycleI_P3_i4_Label() => AssertLabel(_analysis.Waves[0].SubWaves[2].SubWaves[3], WaveNumber.Four, "Cycle(I) [3] (iv)");
        [TestMethod] public void CycleI_P3_i4_Start() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[3].StartPoint, Utc(2020, 3, 4, 14, 0), 4.9762m, "Cycle(I) [3] (iv) Start");
        [TestMethod] public void CycleI_P3_i4_End() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[3].EndPoint, Utc(2020, 3, 13, 2, 0), 1.36m, "Cycle(I) [3] (iv) End");

        [TestMethod] public void CycleI_P3_i5_Label() => AssertLabel(_analysis.Waves[0].SubWaves[2].SubWaves[4], WaveNumber.Five, "Cycle(I) [3] (v)");
        [TestMethod] public void CycleI_P3_i5_Start() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[4].StartPoint, Utc(2020, 3, 13, 2, 0), 1.36m, "Cycle(I) [3] (v) Start");
        [TestMethod] public void CycleI_P3_i5_End() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[4].EndPoint, Utc(2020, 8, 16, 7, 0), 20.1111m, "Cycle(I) [3] (v) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(I) [4]
        //
        // (a): $20.1111 → $9.1000 (05.09.2020)
        // (b): $9.1000 → $13.2800 (10.09.2020)
        // (c): $13.2800 → $7.2869 (23.09.2020)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleI_P4_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[0].SubWaves[3], 3, "Cycle(I) [4] Intermediate");

        [TestMethod] public void CycleI_P4_iA_Label() => AssertLabel(_analysis.Waves[0].SubWaves[3].SubWaves[0], WaveNumber.A, "Cycle(I) [4] (a)");
        [TestMethod] public void CycleI_P4_iA_Start() => AssertPoint(_analysis.Waves[0].SubWaves[3].SubWaves[0].StartPoint, Utc(2020, 8, 16, 7, 0), 20.1111m, "Cycle(I) [4] (a) Start");
        [TestMethod] public void CycleI_P4_iA_End() => AssertPoint(_analysis.Waves[0].SubWaves[3].SubWaves[0].EndPoint, Utc(2020, 9, 5, 18, 0), 9.1m, "Cycle(I) [4] (a) End");

        [TestMethod] public void CycleI_P4_iB_Label() => AssertLabel(_analysis.Waves[0].SubWaves[3].SubWaves[1], WaveNumber.B, "Cycle(I) [4] (b)");
        [TestMethod] public void CycleI_P4_iB_Start() => AssertPoint(_analysis.Waves[0].SubWaves[3].SubWaves[1].StartPoint, Utc(2020, 9, 5, 18, 0), 9.1m, "Cycle(I) [4] (b) Start");
        [TestMethod] public void CycleI_P4_iB_End() => AssertPoint(_analysis.Waves[0].SubWaves[3].SubWaves[1].EndPoint, Utc(2020, 9, 10, 2, 0), 13.28m, "Cycle(I) [4] (b) End");

        [TestMethod] public void CycleI_P4_iC_Label() => AssertLabel(_analysis.Waves[0].SubWaves[3].SubWaves[2], WaveNumber.C, "Cycle(I) [4] (c)");
        [TestMethod] public void CycleI_P4_iC_Start() => AssertPoint(_analysis.Waves[0].SubWaves[3].SubWaves[2].StartPoint, Utc(2020, 9, 10, 2, 0), 13.28m, "Cycle(I) [4] (c) Start");
        [TestMethod] public void CycleI_P4_iC_End() => AssertPoint(_analysis.Waves[0].SubWaves[3].SubWaves[2].EndPoint, Utc(2020, 9, 23, 20, 0), 7.2869m, "Cycle(I) [4] (c) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(I) [5]
        //
        // (i): $7.2869 → $16.3900 (24.11.2020)
        // (ii): $16.3900 → $8.0500 (23.12.2020)
        // (iii): $8.0500 → $44.3300 (15.04.2021)
        // (iv): $44.3300 → $28.8000 (18.04.2021)
        // (v): $28.8000 → $53.0000 (10.05.2021)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleI_P5_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[0].SubWaves[4], 5, "Cycle(I) [5] Intermediate");

        [TestMethod] public void CycleI_P5_i1_Label() => AssertLabel(_analysis.Waves[0].SubWaves[4].SubWaves[0], WaveNumber.One, "Cycle(I) [5] (i)");
        [TestMethod] public void CycleI_P5_i1_Start() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[0].StartPoint, Utc(2020, 9, 23, 20, 0), 7.2869m, "Cycle(I) [5] (i) Start");
        [TestMethod] public void CycleI_P5_i1_End() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[0].EndPoint, Utc(2020, 11, 24, 5, 0), 16.39m, "Cycle(I) [5] (i) End");

        [TestMethod] public void CycleI_P5_i2_Label() => AssertLabel(_analysis.Waves[0].SubWaves[4].SubWaves[1], WaveNumber.Two, "Cycle(I) [5] (ii)");
        [TestMethod] public void CycleI_P5_i2_Start() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[1].StartPoint, Utc(2020, 11, 24, 5, 0), 16.39m, "Cycle(I) [5] (ii) Start");
        [TestMethod] public void CycleI_P5_i2_End() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[1].EndPoint, Utc(2020, 12, 23, 22, 0), 8.05m, "Cycle(I) [5] (ii) End");

        [TestMethod] public void CycleI_P5_i3_Label() => AssertLabel(_analysis.Waves[0].SubWaves[4].SubWaves[2], WaveNumber.Three, "Cycle(I) [5] (iii)");
        [TestMethod] public void CycleI_P5_i3_Start() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[2].StartPoint, Utc(2020, 12, 23, 22, 0), 8.05m, "Cycle(I) [5] (iii) Start");
        [TestMethod] public void CycleI_P5_i3_End() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[2].EndPoint, Utc(2021, 4, 15, 15, 0), 44.33m, "Cycle(I) [5] (iii) End");

        [TestMethod] public void CycleI_P5_i4_Label() => AssertLabel(_analysis.Waves[0].SubWaves[4].SubWaves[3], WaveNumber.Four, "Cycle(I) [5] (iv)");
        [TestMethod] public void CycleI_P5_i4_Start() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[3].StartPoint, Utc(2021, 4, 15, 15, 0), 44.33m, "Cycle(I) [5] (iv) Start");
        [TestMethod] public void CycleI_P5_i4_End() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[3].EndPoint, Utc(2021, 4, 18, 3, 0), 28.8m, "Cycle(I) [5] (iv) End");

        [TestMethod] public void CycleI_P5_i5_Label() => AssertLabel(_analysis.Waves[0].SubWaves[4].SubWaves[4], WaveNumber.Five, "Cycle(I) [5] (v)");
        [TestMethod] public void CycleI_P5_i5_Start() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[4].StartPoint, Utc(2021, 4, 18, 3, 0), 28.8m, "Cycle(I) [5] (v) Start");
        [TestMethod] public void CycleI_P5_i5_End() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[4].EndPoint, Utc(2021, 5, 10, 0, 0), 53m, "Cycle(I) [5] (v) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(II) [A]
        //
        // (i): $53.0000 → $39.2270 (13.05.2021)
        // (ii): $39.2270 → $47.8750 (14.05.2021)
        // (iii): $47.8750 → $15.0000 (23.05.2021)
        // (iv): $15.0000 → $35.3340 (27.05.2021)
        // (v): $35.3340 → $13.3840 (20.07.2021)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleII_PA_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[1].SubWaves[0], 5, "Cycle(II) [A] Intermediate");

        [TestMethod] public void CycleII_PA_i1_Label() => AssertLabel(_analysis.Waves[1].SubWaves[0].SubWaves[0], WaveNumber.One, "Cycle(II) [A] (i)");
        [TestMethod] public void CycleII_PA_i1_Start() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[0].StartPoint, Utc(2021, 5, 10, 0, 0), 53m, "Cycle(II) [A] (i) Start");
        [TestMethod] public void CycleII_PA_i1_End() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[0].EndPoint, Utc(2021, 5, 13, 0, 0), 39.227m, "Cycle(II) [A] (i) End");

        [TestMethod] public void CycleII_PA_i2_Label() => AssertLabel(_analysis.Waves[1].SubWaves[0].SubWaves[1], WaveNumber.Two, "Cycle(II) [A] (ii)");
        [TestMethod] public void CycleII_PA_i2_Start() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[1].StartPoint, Utc(2021, 5, 13, 0, 0), 39.227m, "Cycle(II) [A] (ii) Start");
        [TestMethod] public void CycleII_PA_i2_End() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[1].EndPoint, Utc(2021, 5, 14, 16, 0), 47.875m, "Cycle(II) [A] (ii) End");

        [TestMethod] public void CycleII_PA_i3_Label() => AssertLabel(_analysis.Waves[1].SubWaves[0].SubWaves[2], WaveNumber.Three, "Cycle(II) [A] (iii)");
        [TestMethod] public void CycleII_PA_i3_Start() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[2].StartPoint, Utc(2021, 5, 14, 16, 0), 47.875m, "Cycle(II) [A] (iii) Start");
        [TestMethod] public void CycleII_PA_i3_End() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[2].EndPoint, Utc(2021, 5, 23, 16, 0), 15m, "Cycle(II) [A] (iii) End");

        [TestMethod] public void CycleII_PA_i4_Label() => AssertLabel(_analysis.Waves[1].SubWaves[0].SubWaves[3], WaveNumber.Four, "Cycle(II) [A] (iv)");
        [TestMethod] public void CycleII_PA_i4_Start() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[3].StartPoint, Utc(2021, 5, 23, 16, 0), 15m, "Cycle(II) [A] (iv) Start");
        [TestMethod] public void CycleII_PA_i4_End() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[3].EndPoint, Utc(2021, 5, 27, 13, 0), 35.334m, "Cycle(II) [A] (iv) End");

        [TestMethod] public void CycleII_PA_i5_Label() => AssertLabel(_analysis.Waves[1].SubWaves[0].SubWaves[4], WaveNumber.Five, "Cycle(II) [A] (v)");
        [TestMethod] public void CycleII_PA_i5_Start() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[4].StartPoint, Utc(2021, 5, 27, 13, 0), 35.334m, "Cycle(II) [A] (v) Start");
        [TestMethod] public void CycleII_PA_i5_End() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[4].EndPoint, Utc(2021, 7, 20, 3, 0), 13.384m, "Cycle(II) [A] (v) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(II) [B]
        //
        // (a): $13.3840 → $36.3300 (06.09.2021)
        // (b): $36.3300 → $20.8200 (21.09.2021)
        // (c): $20.8200 → $38.3100 (10.11.2021)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleII_PB_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[1].SubWaves[1], 3, "Cycle(II) [B] Intermediate");

        [TestMethod] public void CycleII_PB_iA_Label() => AssertLabel(_analysis.Waves[1].SubWaves[1].SubWaves[0], WaveNumber.A, "Cycle(II) [B] (a)");
        [TestMethod] public void CycleII_PB_iA_Start() => AssertPoint(_analysis.Waves[1].SubWaves[1].SubWaves[0].StartPoint, Utc(2021, 7, 20, 3, 0), 13.384m, "Cycle(II) [B] (a) Start");
        [TestMethod] public void CycleII_PB_iA_End() => AssertPoint(_analysis.Waves[1].SubWaves[1].SubWaves[0].EndPoint, Utc(2021, 9, 6, 14, 0), 36.33m, "Cycle(II) [B] (a) End");

        [TestMethod] public void CycleII_PB_iB_Label() => AssertLabel(_analysis.Waves[1].SubWaves[1].SubWaves[1], WaveNumber.B, "Cycle(II) [B] (b)");
        [TestMethod] public void CycleII_PB_iB_Start() => AssertPoint(_analysis.Waves[1].SubWaves[1].SubWaves[1].StartPoint, Utc(2021, 9, 6, 14, 0), 36.33m, "Cycle(II) [B] (b) Start");
        [TestMethod] public void CycleII_PB_iB_End() => AssertPoint(_analysis.Waves[1].SubWaves[1].SubWaves[1].EndPoint, Utc(2021, 9, 21, 21, 0), 20.82m, "Cycle(II) [B] (b) End");

        [TestMethod] public void CycleII_PB_iC_Label() => AssertLabel(_analysis.Waves[1].SubWaves[1].SubWaves[2], WaveNumber.C, "Cycle(II) [B] (c)");
        [TestMethod] public void CycleII_PB_iC_Start() => AssertPoint(_analysis.Waves[1].SubWaves[1].SubWaves[2].StartPoint, Utc(2021, 9, 21, 21, 0), 20.82m, "Cycle(II) [B] (c) Start");
        [TestMethod] public void CycleII_PB_iC_End() => AssertPoint(_analysis.Waves[1].SubWaves[1].SubWaves[2].EndPoint, Utc(2021, 11, 10, 15, 0), 38.31m, "Cycle(II) [B] (c) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(II) [C]
        //
        // (i): $38.3100 → $15.3200 (04.12.2021)
        // (ii): $15.3200 → $28.7100 (11.01.2022)
        // (iii): $28.7100 → $5.3000 (13.06.2022)
        // (iv): $5.3000 → $9.5000 (12.08.2022)
        // (v): $9.5000 → $4.7610 (10.06.2023)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleII_PC_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[1].SubWaves[2], 5, "Cycle(II) [C] Intermediate");

        [TestMethod] public void CycleII_PC_i1_Label() => AssertLabel(_analysis.Waves[1].SubWaves[2].SubWaves[0], WaveNumber.One, "Cycle(II) [C] (i)");
        [TestMethod] public void CycleII_PC_i1_Start() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[0].StartPoint, Utc(2021, 11, 10, 15, 0), 38.31m, "Cycle(II) [C] (i) Start");
        [TestMethod] public void CycleII_PC_i1_End() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[0].EndPoint, Utc(2021, 12, 4, 5, 0), 15.32m, "Cycle(II) [C] (i) End");

        [TestMethod] public void CycleII_PC_i2_Label() => AssertLabel(_analysis.Waves[1].SubWaves[2].SubWaves[1], WaveNumber.Two, "Cycle(II) [C] (ii)");
        [TestMethod] public void CycleII_PC_i2_Start() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[1].StartPoint, Utc(2021, 12, 4, 5, 0), 15.32m, "Cycle(II) [C] (ii) Start");
        [TestMethod] public void CycleII_PC_i2_End() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[1].EndPoint, Utc(2022, 1, 11, 3, 0), 28.71m, "Cycle(II) [C] (ii) End");

        [TestMethod] public void CycleII_PC_i3_Label() => AssertLabel(_analysis.Waves[1].SubWaves[2].SubWaves[2], WaveNumber.Three, "Cycle(II) [C] (iii)");
        [TestMethod] public void CycleII_PC_i3_Start() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[2].StartPoint, Utc(2022, 1, 11, 3, 0), 28.71m, "Cycle(II) [C] (iii) Start");
        [TestMethod] public void CycleII_PC_i3_End() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[2].EndPoint, Utc(2022, 6, 13, 9, 0), 5.3m, "Cycle(II) [C] (iii) End");

        [TestMethod] public void CycleII_PC_i4_Label() => AssertLabel(_analysis.Waves[1].SubWaves[2].SubWaves[3], WaveNumber.Four, "Cycle(II) [C] (iv)");
        [TestMethod] public void CycleII_PC_i4_Start() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[3].StartPoint, Utc(2022, 6, 13, 9, 0), 5.3m, "Cycle(II) [C] (iv) Start");
        [TestMethod] public void CycleII_PC_i4_End() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[3].EndPoint, Utc(2022, 8, 12, 13, 0), 9.5m, "Cycle(II) [C] (iv) End");

        [TestMethod] public void CycleII_PC_i5_Label() => AssertLabel(_analysis.Waves[1].SubWaves[2].SubWaves[4], WaveNumber.Five, "Cycle(II) [C] (v)");
        [TestMethod] public void CycleII_PC_i5_Start() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[4].StartPoint, Utc(2022, 8, 12, 13, 0), 9.5m, "Cycle(II) [C] (v) Start");
        [TestMethod] public void CycleII_PC_i5_End() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[4].EndPoint, Utc(2023, 6, 10, 4, 0), 4.761m, "Cycle(II) [C] (v) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(III) [1]
        //
        // (i): $4.7610 → $8.4640 (20.07.2023)
        // (ii): $8.4640 → $5.6000 (17.08.2023)
        // (iii): $5.6000 → $17.6720 (28.12.2023)
        // (iv): $17.6720 → $12.2000 (03.01.2024)
        // (v): $12.2000 → $30.9400 (13.12.2024)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleIII_P1_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[2].SubWaves[0], 5, "Cycle(III) [1] Intermediate");

        [TestMethod] public void CycleIII_P1_i1_Label() => AssertLabel(_analysis.Waves[2].SubWaves[0].SubWaves[0], WaveNumber.One, "Cycle(III) [1] (i)");
        [TestMethod] public void CycleIII_P1_i1_Start() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[0].StartPoint, Utc(2023, 6, 10, 4, 0), 4.761m, "Cycle(III) [1] (i) Start");
        [TestMethod] public void CycleIII_P1_i1_End() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[0].EndPoint, Utc(2023, 7, 20, 22, 0), 8.464m, "Cycle(III) [1] (i) End");

        [TestMethod] public void CycleIII_P1_i2_Label() => AssertLabel(_analysis.Waves[2].SubWaves[0].SubWaves[1], WaveNumber.Two, "Cycle(III) [1] (ii)");
        [TestMethod] public void CycleIII_P1_i2_Start() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[1].StartPoint, Utc(2023, 7, 20, 22, 0), 8.464m, "Cycle(III) [1] (ii) Start");
        [TestMethod] public void CycleIII_P1_i2_End() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[1].EndPoint, Utc(2023, 8, 17, 21, 0), 5.6m, "Cycle(III) [1] (ii) End");

        [TestMethod] public void CycleIII_P1_i3_Label() => AssertLabel(_analysis.Waves[2].SubWaves[0].SubWaves[2], WaveNumber.Three, "Cycle(III) [1] (iii)");
        [TestMethod] public void CycleIII_P1_i3_Start() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[2].StartPoint, Utc(2023, 8, 17, 21, 0), 5.6m, "Cycle(III) [1] (iii) Start");
        [TestMethod] public void CycleIII_P1_i3_End() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[2].EndPoint, Utc(2023, 12, 28, 1, 0), 17.672m, "Cycle(III) [1] (iii) End");

        [TestMethod] public void CycleIII_P1_i4_Label() => AssertLabel(_analysis.Waves[2].SubWaves[0].SubWaves[3], WaveNumber.Four, "Cycle(III) [1] (iv)");
        [TestMethod] public void CycleIII_P1_i4_Start() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[3].StartPoint, Utc(2023, 12, 28, 1, 0), 17.672m, "Cycle(III) [1] (iv) Start");
        [TestMethod] public void CycleIII_P1_i4_End() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[3].EndPoint, Utc(2024, 1, 3, 12, 0), 12.2m, "Cycle(III) [1] (iv) End");

        [TestMethod] public void CycleIII_P1_i5_Label() => AssertLabel(_analysis.Waves[2].SubWaves[0].SubWaves[4], WaveNumber.Five, "Cycle(III) [1] (v)");
        [TestMethod] public void CycleIII_P1_i5_Start() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[4].StartPoint, Utc(2024, 1, 3, 12, 0), 12.2m, "Cycle(III) [1] (v) Start");
        [TestMethod] public void CycleIII_P1_i5_End() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[4].EndPoint, Utc(2024, 12, 13, 2, 0), 30.94m, "Cycle(III) [1] (v) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(III) [2]
        //
        // (a): $30.9400 → $10.1000 (07.04.2025)
        // (b): $10.1000 → $27.8700 (22.08.2025)
        // (c): $27.8700 → $7.1500 (06.02.2026)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleIII_P2_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[2].SubWaves[1], 3, "Cycle(III) [2] Intermediate");

        [TestMethod] public void CycleIII_P2_iA_Label() => AssertLabel(_analysis.Waves[2].SubWaves[1].SubWaves[0], WaveNumber.A, "Cycle(III) [2] (a)");
        [TestMethod] public void CycleIII_P2_iA_Start() => AssertPoint(_analysis.Waves[2].SubWaves[1].SubWaves[0].StartPoint, Utc(2024, 12, 13, 2, 0), 30.94m, "Cycle(III) [2] (a) Start");
        [TestMethod] public void CycleIII_P2_iA_End() => AssertPoint(_analysis.Waves[2].SubWaves[1].SubWaves[0].EndPoint, Utc(2025, 4, 7, 6, 0), 10.1m, "Cycle(III) [2] (a) End");

        [TestMethod] public void CycleIII_P2_iB_Label() => AssertLabel(_analysis.Waves[2].SubWaves[1].SubWaves[1], WaveNumber.B, "Cycle(III) [2] (b)");
        [TestMethod] public void CycleIII_P2_iB_Start() => AssertPoint(_analysis.Waves[2].SubWaves[1].SubWaves[1].StartPoint, Utc(2025, 4, 7, 6, 0), 10.1m, "Cycle(III) [2] (b) Start");
        [TestMethod] public void CycleIII_P2_iB_End() => AssertPoint(_analysis.Waves[2].SubWaves[1].SubWaves[1].EndPoint, Utc(2025, 8, 22, 16, 0), 27.87m, "Cycle(III) [2] (b) End");

        [TestMethod] public void CycleIII_P2_iC_Label() => AssertLabel(_analysis.Waves[2].SubWaves[1].SubWaves[2], WaveNumber.C, "Cycle(III) [2] (c)");
        [TestMethod] public void CycleIII_P2_iC_Start() => AssertPoint(_analysis.Waves[2].SubWaves[1].SubWaves[2].StartPoint, Utc(2025, 8, 22, 16, 0), 27.87m, "Cycle(III) [2] (c) Start");
        [TestMethod] public void CycleIII_P2_iC_End() => AssertPoint(_analysis.Waves[2].SubWaves[1].SubWaves[2].EndPoint, Utc(2026, 2, 6, 0, 0), 7.15m, "Cycle(III) [2] (c) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(III) [3]
        //
        // (i): $7.1500 → $7.1500 (06.02.2026) (in-progress)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleIII_P3_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[2].SubWaves[2], 1, "Cycle(III) [3] Intermediate");

        [TestMethod] public void CycleIII_P3_i1_Label() => AssertLabel(_analysis.Waves[2].SubWaves[2].SubWaves[0], WaveNumber.One, "Cycle(III) [3] (i)");
        [TestMethod] public void CycleIII_P3_i1_InProgress() => AssertInProgress(_analysis.Waves[2].SubWaves[2].SubWaves[0], true, "Cycle(III) [3] (i)");
        [TestMethod] public void CycleIII_P3_i1_Start() => AssertPoint(_analysis.Waves[2].SubWaves[2].SubWaves[0].StartPoint, Utc(2026, 2, 6, 0, 0), 7.15m, "Cycle(III) [3] (i) Start");

        // ══════════════════════════════════════════════════
        // Projections — Cycle (III): W3 extension of Cycle I ($0.35 → $53.00) from Cycle II end ($4.76)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleIII_Projection_4236() => AssertProjectionTarget(_analysis.Waves[2], LogExtension(0.3504m, 53.00m, 4.761m, 4.236m), 4.236m, "Cycle(III) 4.236 projection");
        [TestMethod] public void CycleIII_Projection_2618() => AssertProjectionTarget(_analysis.Waves[2], LogExtension(0.3504m, 53.00m, 4.761m, 2.618m), 2.618m, "Cycle(III) 2.618 projection");
        [TestMethod] public void CycleIII_Projection_1618() => AssertProjectionTarget(_analysis.Waves[2], LogExtension(0.3504m, 53.00m, 4.761m, 1.618m), 1.618m, "Cycle(III) 1.618 projection");
        [TestMethod] public void CycleIII_Projection_100() => AssertProjectionTarget(_analysis.Waves[2], LogExtension(0.3504m, 53.00m, 4.761m, 1.0m), 1.0m, "Cycle(III) 1.0 projection");

        // ══════════════════════════════════════════════════
        // Projections — Primary [3] inside Cycle (III): W3 extension of P1 ($4.76 → $30.94) from P2 end ($7.15)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleIII_P3_Projection_4236() => AssertProjectionTarget(_analysis.Waves[2].SubWaves[2], LogExtension(4.761m, 30.94m, 7.15m, 4.236m), 4.236m, "Cycle(III) [3] 4.236 projection");
        [TestMethod] public void CycleIII_P3_Projection_2618() => AssertProjectionTarget(_analysis.Waves[2].SubWaves[2], LogExtension(4.761m, 30.94m, 7.15m, 2.618m), 2.618m, "Cycle(III) [3] 2.618 projection");
        [TestMethod] public void CycleIII_P3_Projection_1618() => AssertProjectionTarget(_analysis.Waves[2].SubWaves[2], LogExtension(4.761m, 30.94m, 7.15m, 1.618m), 1.618m, "Cycle(III) [3] 1.618 projection");
        [TestMethod] public void CycleIII_P3_Projection_100() => AssertProjectionTarget(_analysis.Waves[2].SubWaves[2], LogExtension(4.761m, 30.94m, 7.15m, 1.0m), 1.0m, "Cycle(III) [3] 1.0 projection");
    }
}
