using SmarterSystems.ElliottWaves.Analyzer.Interfaces;

namespace SmarterSystems.ElliottWaves.Validation
{
    [TestClass]
    public sealed class BtcRegressionTest : RegressionTestBase
    {
        private static ElliottWavesAnalysis _analysis = null!;

        [ClassInitialize]
        public static void Setup(TestContext _)
        {
            _analysis = LoadAndAnalyze("BTCUSDT");
        }

        // ══════════════════════════════════════════════════
        // Cycle Degree: 4 waves (I, II, III completed; IV in-progress)
        //
        // Cycle (I):  15.09.2017 $2,817 → 17.12.2017 $19,798.68
        // Cycle (II): 17.12.2017 $19,798.68 → 15.12.2018 $3,156.26
        // Cycle (III): 15.12.2018 $3,156.26 → 06.10.2025 $126,199.63
        // Cycle (IV): 06.10.2025 $126,199.63 → in-progress
        // ══════════════════════════════════════════════════

        [TestMethod] public void Cycle_WaveCount() => AssertWaveCount(_analysis, 4, "BTC Cycle");

        // ── Cycle (I): $2,817 → $19,798.68 ──

        [TestMethod] public void CycleI_Label() => AssertLabel(_analysis.Waves[0], WaveNumber.One, "Cycle(I)");
        [TestMethod] public void CycleI_NotInProgress() => AssertInProgress(_analysis.Waves[0], false, "Cycle(I)");
        [TestMethod] public void CycleI_Start() => AssertPoint(_analysis.Waves[0].StartPoint, Utc(2017, 9, 15, 11), 2817.00m, "Cycle(I) Start");
        [TestMethod] public void CycleI_End() => AssertPoint(_analysis.Waves[0].EndPoint, Utc(2017, 12, 17, 12), 19798.68m, "Cycle(I) End");

        // ── Cycle (II): $19,798.68 → $3,156.26 ──

        [TestMethod] public void CycleII_Label() => AssertLabel(_analysis.Waves[1], WaveNumber.Two, "Cycle(II)");
        [TestMethod] public void CycleII_NotInProgress() => AssertInProgress(_analysis.Waves[1], false, "Cycle(II)");
        [TestMethod] public void CycleII_Start() => AssertPoint(_analysis.Waves[1].StartPoint, Utc(2017, 12, 17, 12), 19798.68m, "Cycle(II) Start");
        [TestMethod] public void CycleII_End() => AssertPoint(_analysis.Waves[1].EndPoint, Utc(2018, 12, 15, 15), 3156.26m, "Cycle(II) End");

        // ── Cycle (III): $3,156.26 → $126,199.63 ──

        [TestMethod] public void CycleIII_Label() => AssertLabel(_analysis.Waves[2], WaveNumber.Three, "Cycle(III)");
        [TestMethod] public void CycleIII_NotInProgress() => AssertInProgress(_analysis.Waves[2], false, "Cycle(III)");
        [TestMethod] public void CycleIII_Start() => AssertPoint(_analysis.Waves[2].StartPoint, Utc(2018, 12, 15, 15), 3156.26m, "Cycle(III) Start");
        [TestMethod] public void CycleIII_End() => AssertPoint(_analysis.Waves[2].EndPoint, Utc(2025, 10, 6, 18), 126199.63m, "Cycle(III) End");

        // ── Cycle (IV): $126,199.63 → in-progress ──

        [TestMethod] public void CycleIV_Label() => AssertLabel(_analysis.Waves[3], WaveNumber.Four, "Cycle(IV)");
        [TestMethod] public void CycleIV_InProgress() => AssertInProgress(_analysis.Waves[3], true, "Cycle(IV)");
        [TestMethod] public void CycleIV_Start() => AssertPoint(_analysis.Waves[3].StartPoint, Utc(2025, 10, 6, 18), 126199.63m, "Cycle(IV) Start");

        // ══════════════════════════════════════════════════
        // Primary Degree — inside Cycle (I): 5 waves (impulse)
        //
        // [1]: $2,817 → $4,123.20 (18.09.2017)
        // [2]: $4,123.20 → $3,505.55 (22.09.2017)
        // [3]: $3,505.55 → $17,204.99 (08.12.2017)
        // [4]: $17,204.99 → $12,368.00 (10.12.2017)
        // [5]: $12,368.00 → $19,798.68 (17.12.2017)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleI_Primary_WaveCount() => AssertSubWaveCount(_analysis.Waves[0], 5, "Cycle(I) Primary");

        [TestMethod] public void CycleI_P1_Label() => AssertLabel(_analysis.Waves[0].SubWaves[0], WaveNumber.One, "Cycle(I) [1]");
        [TestMethod] public void CycleI_P1_Start() => AssertPoint(_analysis.Waves[0].SubWaves[0].StartPoint, Utc(2017, 9, 15, 11), 2817.00m, "Cycle(I) [1] Start");
        [TestMethod] public void CycleI_P1_End() => AssertPoint(_analysis.Waves[0].SubWaves[0].EndPoint, Utc(2017, 9, 18, 13), 4123.20m, "Cycle(I) [1] End");

        [TestMethod] public void CycleI_P2_Label() => AssertLabel(_analysis.Waves[0].SubWaves[1], WaveNumber.Two, "Cycle(I) [2]");
        [TestMethod] public void CycleI_P2_Start() => AssertPoint(_analysis.Waves[0].SubWaves[1].StartPoint, Utc(2017, 9, 18, 13), 4123.20m, "Cycle(I) [2] Start");
        [TestMethod] public void CycleI_P2_End() => AssertPoint(_analysis.Waves[0].SubWaves[1].EndPoint, Utc(2017, 9, 22, 15), 3505.55m, "Cycle(I) [2] End");

        [TestMethod] public void CycleI_P3_Label() => AssertLabel(_analysis.Waves[0].SubWaves[2], WaveNumber.Three, "Cycle(I) [3]");
        [TestMethod] public void CycleI_P3_Start() => AssertPoint(_analysis.Waves[0].SubWaves[2].StartPoint, Utc(2017, 9, 22, 15), 3505.55m, "Cycle(I) [3] Start");
        [TestMethod] public void CycleI_P3_End() => AssertPoint(_analysis.Waves[0].SubWaves[2].EndPoint, Utc(2017, 12, 8, 1), 17204.99m, "Cycle(I) [3] End");

        [TestMethod] public void CycleI_P4_Label() => AssertLabel(_analysis.Waves[0].SubWaves[3], WaveNumber.Four, "Cycle(I) [4]");
        [TestMethod] public void CycleI_P4_Start() => AssertPoint(_analysis.Waves[0].SubWaves[3].StartPoint, Utc(2017, 12, 8, 1), 17204.99m, "Cycle(I) [4] Start");
        [TestMethod] public void CycleI_P4_End() => AssertPoint(_analysis.Waves[0].SubWaves[3].EndPoint, Utc(2017, 12, 10, 4), 12368.00m, "Cycle(I) [4] End");

        [TestMethod] public void CycleI_P5_Label() => AssertLabel(_analysis.Waves[0].SubWaves[4], WaveNumber.Five, "Cycle(I) [5]");
        [TestMethod] public void CycleI_P5_Start() => AssertPoint(_analysis.Waves[0].SubWaves[4].StartPoint, Utc(2017, 12, 10, 4), 12368.00m, "Cycle(I) [5] Start");
        [TestMethod] public void CycleI_P5_End() => AssertPoint(_analysis.Waves[0].SubWaves[4].EndPoint, Utc(2017, 12, 17, 12), 19798.68m, "Cycle(I) [5] End");

        // ══════════════════════════════════════════════════
        // Primary Degree — inside Cycle (II): A-B-C (corrective)
        //
        // [A]: $19,798.68 → $6,000.01 (06.02.2018)
        // [B]: $6,000.01 → $11,786.01 (20.02.2018)
        // [C]: $11,786.01 → $3,156.26 (15.12.2018)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleII_Primary_WaveCount() => AssertSubWaveCount(_analysis.Waves[1], 3, "Cycle(II) Primary");

        [TestMethod] public void CycleII_A_Label() => AssertLabel(_analysis.Waves[1].SubWaves[0], WaveNumber.A, "Cycle(II) [A]");
        [TestMethod] public void CycleII_A_Start() => AssertPoint(_analysis.Waves[1].SubWaves[0].StartPoint, Utc(2017, 12, 17, 12), 19798.68m, "Cycle(II) [A] Start");
        [TestMethod] public void CycleII_A_End() => AssertPoint(_analysis.Waves[1].SubWaves[0].EndPoint, Utc(2018, 2, 6, 7), 6000.01m, "Cycle(II) [A] End");

        [TestMethod] public void CycleII_B_Label() => AssertLabel(_analysis.Waves[1].SubWaves[1], WaveNumber.B, "Cycle(II) [B]");
        [TestMethod] public void CycleII_B_Start() => AssertPoint(_analysis.Waves[1].SubWaves[1].StartPoint, Utc(2018, 2, 6, 7), 6000.01m, "Cycle(II) [B] Start");
        [TestMethod] public void CycleII_B_End() => AssertPoint(_analysis.Waves[1].SubWaves[1].EndPoint, Utc(2018, 2, 20, 22), 11786.01m, "Cycle(II) [B] End");

        [TestMethod] public void CycleII_C_Label() => AssertLabel(_analysis.Waves[1].SubWaves[2], WaveNumber.C, "Cycle(II) [C]");
        [TestMethod] public void CycleII_C_Start() => AssertPoint(_analysis.Waves[1].SubWaves[2].StartPoint, Utc(2018, 2, 20, 22), 11786.01m, "Cycle(II) [C] Start");
        [TestMethod] public void CycleII_C_End() => AssertPoint(_analysis.Waves[1].SubWaves[2].EndPoint, Utc(2018, 12, 15, 15), 3156.26m, "Cycle(II) [C] End");

        // ══════════════════════════════════════════════════
        // Primary Degree — inside Cycle (III): 5 waves (impulse)
        //
        // [1]: $3,156.26 → $13,970.00 (26.06.2019)
        // [2]: $13,970.00 → $3,782.13 (13.03.2020)
        // [3]: $3,782.13 → $69,000.00 (10.11.2021)
        // [4]: $69,000.00 → $15,476.00 (21.11.2022)
        // [5]: $15,476.00 → $126,199.63 (06.10.2025)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleIII_Primary_WaveCount() => AssertSubWaveCount(_analysis.Waves[2], 5, "Cycle(III) Primary");

        [TestMethod] public void CycleIII_P1_Label() => AssertLabel(_analysis.Waves[2].SubWaves[0], WaveNumber.One, "Cycle(III) [1]");
        [TestMethod] public void CycleIII_P1_Start() => AssertPoint(_analysis.Waves[2].SubWaves[0].StartPoint, Utc(2018, 12, 15, 15), 3156.26m, "Cycle(III) [1] Start");
        [TestMethod] public void CycleIII_P1_End() => AssertPoint(_analysis.Waves[2].SubWaves[0].EndPoint, Utc(2019, 6, 26, 20), 13970.00m, "Cycle(III) [1] End");

        [TestMethod] public void CycleIII_P2_Label() => AssertLabel(_analysis.Waves[2].SubWaves[1], WaveNumber.Two, "Cycle(III) [2]");
        [TestMethod] public void CycleIII_P2_Start() => AssertPoint(_analysis.Waves[2].SubWaves[1].StartPoint, Utc(2019, 6, 26, 20), 13970.00m, "Cycle(III) [2] Start");
        [TestMethod] public void CycleIII_P2_End() => AssertPoint(_analysis.Waves[2].SubWaves[1].EndPoint, Utc(2020, 3, 13, 2), 3782.13m, "Cycle(III) [2] End");

        [TestMethod] public void CycleIII_P3_Label() => AssertLabel(_analysis.Waves[2].SubWaves[2], WaveNumber.Three, "Cycle(III) [3]");
        [TestMethod] public void CycleIII_P3_Start() => AssertPoint(_analysis.Waves[2].SubWaves[2].StartPoint, Utc(2020, 3, 13, 2), 3782.13m, "Cycle(III) [3] Start");
        [TestMethod] public void CycleIII_P3_End() => AssertPoint(_analysis.Waves[2].SubWaves[2].EndPoint, Utc(2021, 11, 10, 14), 69000.00m, "Cycle(III) [3] End");

        [TestMethod] public void CycleIII_P4_Label() => AssertLabel(_analysis.Waves[2].SubWaves[3], WaveNumber.Four, "Cycle(III) [4]");
        [TestMethod] public void CycleIII_P4_Start() => AssertPoint(_analysis.Waves[2].SubWaves[3].StartPoint, Utc(2021, 11, 10, 14), 69000.00m, "Cycle(III) [4] Start");
        [TestMethod] public void CycleIII_P4_End() => AssertPoint(_analysis.Waves[2].SubWaves[3].EndPoint, Utc(2022, 11, 21, 21), 15476.00m, "Cycle(III) [4] End");

        [TestMethod] public void CycleIII_P5_Label() => AssertLabel(_analysis.Waves[2].SubWaves[4], WaveNumber.Five, "Cycle(III) [5]");
        [TestMethod] public void CycleIII_P5_Start() => AssertPoint(_analysis.Waves[2].SubWaves[4].StartPoint, Utc(2022, 11, 21, 21), 15476.00m, "Cycle(III) [5] Start");
        [TestMethod] public void CycleIII_P5_End() => AssertPoint(_analysis.Waves[2].SubWaves[4].EndPoint, Utc(2025, 10, 6, 18), 126199.63m, "Cycle(III) [5] End");

        // ══════════════════════════════════════════════════
        // Primary Degree — inside Cycle (IV): A-B-C (corrective, in-progress)
        //
        // [A]: $126,199.63 → $60,000.00 (06.02.2026) — completed
        // [B]: $60,000.00 → $76,000.00 — completed
        // [C]: $76,000.00 → in-progress
        // ══════════════════════════════════════════════════

        [TestMethod]
        public void CycleIV_HasSubWaves()
        {
            Assert.IsNotNull(_analysis.Waves[3].SubWaves, "Cycle(IV) should have sub-waves");
            Assert.IsTrue(_analysis.Waves[3].SubWaves.Count >= 2, "Cycle(IV) should have at least 2 primary waves");
        }

        [TestMethod] public void CycleIV_A_Label() => AssertLabel(_analysis.Waves[3].SubWaves[0], WaveNumber.A, "Cycle(IV) [A]");
        [TestMethod] public void CycleIV_A_Start() => AssertPoint(_analysis.Waves[3].SubWaves[0].StartPoint, Utc(2025, 10, 6, 18), 126199.63m, "Cycle(IV) [A] Start");
        [TestMethod] public void CycleIV_A_End() => AssertPoint(_analysis.Waves[3].SubWaves[0].EndPoint, Utc(2026, 2, 6, 0), 60000.00m, "Cycle(IV) [A] End");

        [TestMethod] public void CycleIV_B_Label() => AssertLabel(_analysis.Waves[3].SubWaves[1], WaveNumber.B, "Cycle(IV) [B]");
        [TestMethod] public void CycleIV_B_NotInProgress() => AssertInProgress(_analysis.Waves[3].SubWaves[1], false, "Cycle(IV) [B]");

        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(I) [2]
        //
        // (a): $4'123.20 → $3'820.00 (20.09.2017)
        // (b): $3'820.00 → $4'046.08 (20.09.2017)
        // (c): $4'046.08 → $3'505.55 (22.09.2017)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleI_P2_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[0].SubWaves[1], 3, "Cycle(I) [2] Intermediate");

        [TestMethod] public void CycleI_P2_iA_Label() => AssertLabel(_analysis.Waves[0].SubWaves[1].SubWaves[0], WaveNumber.A, "Cycle(I) [2] (a)");
        [TestMethod] public void CycleI_P2_iA_Start() => AssertPoint(_analysis.Waves[0].SubWaves[1].SubWaves[0].StartPoint, Utc(2017, 9, 18, 13, 0), 4123.2m, "Cycle(I) [2] (a) Start");
        [TestMethod] public void CycleI_P2_iA_End() => AssertPoint(_analysis.Waves[0].SubWaves[1].SubWaves[0].EndPoint, Utc(2017, 9, 20, 5, 0), 3820m, "Cycle(I) [2] (a) End");

        [TestMethod] public void CycleI_P2_iB_Label() => AssertLabel(_analysis.Waves[0].SubWaves[1].SubWaves[1], WaveNumber.B, "Cycle(I) [2] (b)");
        [TestMethod] public void CycleI_P2_iB_Start() => AssertPoint(_analysis.Waves[0].SubWaves[1].SubWaves[1].StartPoint, Utc(2017, 9, 20, 5, 0), 3820m, "Cycle(I) [2] (b) Start");
        [TestMethod] public void CycleI_P2_iB_End() => AssertPoint(_analysis.Waves[0].SubWaves[1].SubWaves[1].EndPoint, Utc(2017, 9, 20, 14, 0), 4046.08m, "Cycle(I) [2] (b) End");

        [TestMethod] public void CycleI_P2_iC_Label() => AssertLabel(_analysis.Waves[0].SubWaves[1].SubWaves[2], WaveNumber.C, "Cycle(I) [2] (c)");
        [TestMethod] public void CycleI_P2_iC_Start() => AssertPoint(_analysis.Waves[0].SubWaves[1].SubWaves[2].StartPoint, Utc(2017, 9, 20, 14, 0), 4046.08m, "Cycle(I) [2] (c) Start");
        [TestMethod] public void CycleI_P2_iC_End() => AssertPoint(_analysis.Waves[0].SubWaves[1].SubWaves[2].EndPoint, Utc(2017, 9, 22, 15, 0), 3505.55m, "Cycle(I) [2] (c) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(I) [3]
        //
        // (i): $3'505.55 → $4'561.63 (02.10.2017)
        // (ii): $4'561.63 → $4'110.00 (05.10.2017)
        // (iii): $4'110.00 → $11'300 (29.11.2017)
        // (iv): $11'300 → $8'520.00 (29.11.2017)
        // (v): $8'520.00 → $17'205 (08.12.2017)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleI_P3_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[0].SubWaves[2], 5, "Cycle(I) [3] Intermediate");

        [TestMethod] public void CycleI_P3_i1_Label() => AssertLabel(_analysis.Waves[0].SubWaves[2].SubWaves[0], WaveNumber.One, "Cycle(I) [3] (i)");
        [TestMethod] public void CycleI_P3_i1_Start() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[0].StartPoint, Utc(2017, 9, 22, 15, 0), 3505.55m, "Cycle(I) [3] (i) Start");
        [TestMethod] public void CycleI_P3_i1_End() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[0].EndPoint, Utc(2017, 10, 2, 14, 0), 4561.63m, "Cycle(I) [3] (i) End");

        [TestMethod] public void CycleI_P3_i2_Label() => AssertLabel(_analysis.Waves[0].SubWaves[2].SubWaves[1], WaveNumber.Two, "Cycle(I) [3] (ii)");
        [TestMethod] public void CycleI_P3_i2_Start() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[1].StartPoint, Utc(2017, 10, 2, 14, 0), 4561.63m, "Cycle(I) [3] (ii) Start");
        [TestMethod] public void CycleI_P3_i2_End() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[1].EndPoint, Utc(2017, 10, 5, 6, 0), 4110m, "Cycle(I) [3] (ii) End");

        [TestMethod] public void CycleI_P3_i3_Label() => AssertLabel(_analysis.Waves[0].SubWaves[2].SubWaves[2], WaveNumber.Three, "Cycle(I) [3] (iii)");
        [TestMethod] public void CycleI_P3_i3_Start() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[2].StartPoint, Utc(2017, 10, 5, 6, 0), 4110m, "Cycle(I) [3] (iii) Start");
        [TestMethod] public void CycleI_P3_i3_End() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[2].EndPoint, Utc(2017, 11, 29, 14, 0), 11300.03m, "Cycle(I) [3] (iii) End");

        [TestMethod] public void CycleI_P3_i4_Label() => AssertLabel(_analysis.Waves[0].SubWaves[2].SubWaves[3], WaveNumber.Four, "Cycle(I) [3] (iv)");
        [TestMethod] public void CycleI_P3_i4_Start() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[3].StartPoint, Utc(2017, 11, 29, 14, 0), 11300.03m, "Cycle(I) [3] (iv) Start");
        [TestMethod] public void CycleI_P3_i4_End() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[3].EndPoint, Utc(2017, 11, 29, 19, 0), 8520m, "Cycle(I) [3] (iv) End");

        [TestMethod] public void CycleI_P3_i5_Label() => AssertLabel(_analysis.Waves[0].SubWaves[2].SubWaves[4], WaveNumber.Five, "Cycle(I) [3] (v)");
        [TestMethod] public void CycleI_P3_i5_Start() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[4].StartPoint, Utc(2017, 11, 29, 19, 0), 8520m, "Cycle(I) [3] (v) Start");
        [TestMethod] public void CycleI_P3_i5_End() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[4].EndPoint, Utc(2017, 12, 8, 1, 0), 17204.99m, "Cycle(I) [3] (v) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(I) [4]
        //
        // (a): $17'205 → $12'535 (09.12.2017)
        // (b): $12'535 → $14'813 (09.12.2017)
        // (c): $14'813 → $12'368 (10.12.2017)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleI_P4_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[0].SubWaves[3], 3, "Cycle(I) [4] Intermediate");

        [TestMethod] public void CycleI_P4_iA_Label() => AssertLabel(_analysis.Waves[0].SubWaves[3].SubWaves[0], WaveNumber.A, "Cycle(I) [4] (a)");
        [TestMethod] public void CycleI_P4_iA_Start() => AssertPoint(_analysis.Waves[0].SubWaves[3].SubWaves[0].StartPoint, Utc(2017, 12, 8, 1, 0), 17204.99m, "Cycle(I) [4] (a) Start");
        [TestMethod] public void CycleI_P4_iA_End() => AssertPoint(_analysis.Waves[0].SubWaves[3].SubWaves[0].EndPoint, Utc(2017, 12, 9, 17, 0), 12535m, "Cycle(I) [4] (a) End");

        [TestMethod] public void CycleI_P4_iB_Label() => AssertLabel(_analysis.Waves[0].SubWaves[3].SubWaves[1], WaveNumber.B, "Cycle(I) [4] (b)");
        [TestMethod] public void CycleI_P4_iB_Start() => AssertPoint(_analysis.Waves[0].SubWaves[3].SubWaves[1].StartPoint, Utc(2017, 12, 9, 17, 0), 12535m, "Cycle(I) [4] (b) Start");
        [TestMethod] public void CycleI_P4_iB_End() => AssertPoint(_analysis.Waves[0].SubWaves[3].SubWaves[1].EndPoint, Utc(2017, 12, 9, 22, 0), 14813m, "Cycle(I) [4] (b) End");

        [TestMethod] public void CycleI_P4_iC_Label() => AssertLabel(_analysis.Waves[0].SubWaves[3].SubWaves[2], WaveNumber.C, "Cycle(I) [4] (c)");
        [TestMethod] public void CycleI_P4_iC_Start() => AssertPoint(_analysis.Waves[0].SubWaves[3].SubWaves[2].StartPoint, Utc(2017, 12, 9, 22, 0), 14813m, "Cycle(I) [4] (c) Start");
        [TestMethod] public void CycleI_P4_iC_End() => AssertPoint(_analysis.Waves[0].SubWaves[3].SubWaves[2].EndPoint, Utc(2017, 12, 10, 4, 0), 12368m, "Cycle(I) [4] (c) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(I) [5]
        //
        // (i): $12'368 → $13'882 (10.12.2017)
        // (ii): $13'882 → $12'900 (10.12.2017)
        // (iii): $12'900 → $17'470 (11.12.2017)
        // (iv): $17'470 → $14'667 (13.12.2017)
        // (v): $14'667 → $19'799 (17.12.2017)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleI_P5_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[0].SubWaves[4], 5, "Cycle(I) [5] Intermediate");

        [TestMethod] public void CycleI_P5_i1_Label() => AssertLabel(_analysis.Waves[0].SubWaves[4].SubWaves[0], WaveNumber.One, "Cycle(I) [5] (i)");
        [TestMethod] public void CycleI_P5_i1_Start() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[0].StartPoint, Utc(2017, 12, 10, 4, 0), 12368m, "Cycle(I) [5] (i) Start");
        [TestMethod] public void CycleI_P5_i1_End() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[0].EndPoint, Utc(2017, 12, 10, 6, 0), 13882.4m, "Cycle(I) [5] (i) End");

        [TestMethod] public void CycleI_P5_i2_Label() => AssertLabel(_analysis.Waves[0].SubWaves[4].SubWaves[1], WaveNumber.Two, "Cycle(I) [5] (ii)");
        [TestMethod] public void CycleI_P5_i2_Start() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[1].StartPoint, Utc(2017, 12, 10, 6, 0), 13882.4m, "Cycle(I) [5] (ii) Start");
        [TestMethod] public void CycleI_P5_i2_End() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[1].EndPoint, Utc(2017, 12, 10, 8, 0), 12900.13m, "Cycle(I) [5] (ii) End");

        [TestMethod] public void CycleI_P5_i3_Label() => AssertLabel(_analysis.Waves[0].SubWaves[4].SubWaves[2], WaveNumber.Three, "Cycle(I) [5] (iii)");
        [TestMethod] public void CycleI_P5_i3_Start() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[2].StartPoint, Utc(2017, 12, 10, 8, 0), 12900.13m, "Cycle(I) [5] (iii) Start");
        [TestMethod] public void CycleI_P5_i3_End() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[2].EndPoint, Utc(2017, 12, 11, 20, 0), 17470m, "Cycle(I) [5] (iii) End");

        [TestMethod] public void CycleI_P5_i4_Label() => AssertLabel(_analysis.Waves[0].SubWaves[4].SubWaves[3], WaveNumber.Four, "Cycle(I) [5] (iv)");
        [TestMethod] public void CycleI_P5_i4_Start() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[3].StartPoint, Utc(2017, 12, 11, 20, 0), 17470m, "Cycle(I) [5] (iv) Start");
        [TestMethod] public void CycleI_P5_i4_End() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[3].EndPoint, Utc(2017, 12, 13, 3, 0), 14666.56m, "Cycle(I) [5] (iv) End");

        [TestMethod] public void CycleI_P5_i5_Label() => AssertLabel(_analysis.Waves[0].SubWaves[4].SubWaves[4], WaveNumber.Five, "Cycle(I) [5] (v)");
        [TestMethod] public void CycleI_P5_i5_Start() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[4].StartPoint, Utc(2017, 12, 13, 3, 0), 14666.56m, "Cycle(I) [5] (v) Start");
        [TestMethod] public void CycleI_P5_i5_End() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[4].EndPoint, Utc(2017, 12, 17, 12, 0), 19798.68m, "Cycle(I) [5] (v) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(II) [A]
        //
        // (i): $19'799 → $16'300 (19.12.2017)
        // (ii): $16'300 → $18'185 (19.12.2017)
        // (iii): $18'185 → $9'035.00 (16.01.2018)
        // (iv): $9'035.00 → $13'099 (20.01.2018)
        // (v): $13'099 → $6'000.01 (06.02.2018)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleII_PA_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[1].SubWaves[0], 5, "Cycle(II) [A] Intermediate");

        [TestMethod] public void CycleII_PA_i1_Label() => AssertLabel(_analysis.Waves[1].SubWaves[0].SubWaves[0], WaveNumber.One, "Cycle(II) [A] (i)");
        [TestMethod] public void CycleII_PA_i1_Start() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[0].StartPoint, Utc(2017, 12, 17, 12, 0), 19798.68m, "Cycle(II) [A] (i) Start");
        [TestMethod] public void CycleII_PA_i1_End() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[0].EndPoint, Utc(2017, 12, 19, 10, 0), 16300m, "Cycle(II) [A] (i) End");

        [TestMethod] public void CycleII_PA_i2_Label() => AssertLabel(_analysis.Waves[1].SubWaves[0].SubWaves[1], WaveNumber.Two, "Cycle(II) [A] (ii)");
        [TestMethod] public void CycleII_PA_i2_Start() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[1].StartPoint, Utc(2017, 12, 19, 10, 0), 16300m, "Cycle(II) [A] (ii) Start");
        [TestMethod] public void CycleII_PA_i2_End() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[1].EndPoint, Utc(2017, 12, 19, 18, 0), 18185.18m, "Cycle(II) [A] (ii) End");

        [TestMethod] public void CycleII_PA_i3_Label() => AssertLabel(_analysis.Waves[1].SubWaves[0].SubWaves[2], WaveNumber.Three, "Cycle(II) [A] (iii)");
        [TestMethod] public void CycleII_PA_i3_Start() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[2].StartPoint, Utc(2017, 12, 19, 18, 0), 18185.18m, "Cycle(II) [A] (iii) Start");
        [TestMethod] public void CycleII_PA_i3_End() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[2].EndPoint, Utc(2018, 1, 16, 22, 0), 9035m, "Cycle(II) [A] (iii) End");

        [TestMethod] public void CycleII_PA_i4_Label() => AssertLabel(_analysis.Waves[1].SubWaves[0].SubWaves[3], WaveNumber.Four, "Cycle(II) [A] (iv)");
        [TestMethod] public void CycleII_PA_i4_Start() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[3].StartPoint, Utc(2018, 1, 16, 22, 0), 9035m, "Cycle(II) [A] (iv) Start");
        [TestMethod] public void CycleII_PA_i4_End() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[3].EndPoint, Utc(2018, 1, 20, 18, 0), 13099m, "Cycle(II) [A] (iv) End");

        [TestMethod] public void CycleII_PA_i5_Label() => AssertLabel(_analysis.Waves[1].SubWaves[0].SubWaves[4], WaveNumber.Five, "Cycle(II) [A] (v)");
        [TestMethod] public void CycleII_PA_i5_Start() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[4].StartPoint, Utc(2018, 1, 20, 18, 0), 13099m, "Cycle(II) [A] (v) Start");
        [TestMethod] public void CycleII_PA_i5_End() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[4].EndPoint, Utc(2018, 2, 6, 7, 0), 6000.01m, "Cycle(II) [A] (v) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(II) [B]
        //
        // (a): $6'000.01 → $9'065.78 (10.02.2018)
        // (b): $9'065.78 → $7'726.53 (11.02.2018)
        // (c): $7'726.53 → $11'786 (20.02.2018)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleII_PB_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[1].SubWaves[1], 3, "Cycle(II) [B] Intermediate");

        [TestMethod] public void CycleII_PB_iA_Label() => AssertLabel(_analysis.Waves[1].SubWaves[1].SubWaves[0], WaveNumber.A, "Cycle(II) [B] (a)");
        [TestMethod] public void CycleII_PB_iA_Start() => AssertPoint(_analysis.Waves[1].SubWaves[1].SubWaves[0].StartPoint, Utc(2018, 2, 6, 7, 0), 6000.01m, "Cycle(II) [B] (a) Start");
        [TestMethod] public void CycleII_PB_iA_End() => AssertPoint(_analysis.Waves[1].SubWaves[1].SubWaves[0].EndPoint, Utc(2018, 2, 10, 7, 0), 9065.78m, "Cycle(II) [B] (a) End");

        [TestMethod] public void CycleII_PB_iB_Label() => AssertLabel(_analysis.Waves[1].SubWaves[1].SubWaves[1], WaveNumber.B, "Cycle(II) [B] (b)");
        [TestMethod] public void CycleII_PB_iB_Start() => AssertPoint(_analysis.Waves[1].SubWaves[1].SubWaves[1].StartPoint, Utc(2018, 2, 10, 7, 0), 9065.78m, "Cycle(II) [B] (b) Start");
        [TestMethod] public void CycleII_PB_iB_End() => AssertPoint(_analysis.Waves[1].SubWaves[1].SubWaves[1].EndPoint, Utc(2018, 2, 11, 8, 0), 7726.53m, "Cycle(II) [B] (b) End");

        [TestMethod] public void CycleII_PB_iC_Label() => AssertLabel(_analysis.Waves[1].SubWaves[1].SubWaves[2], WaveNumber.C, "Cycle(II) [B] (c)");
        [TestMethod] public void CycleII_PB_iC_Start() => AssertPoint(_analysis.Waves[1].SubWaves[1].SubWaves[2].StartPoint, Utc(2018, 2, 11, 8, 0), 7726.53m, "Cycle(II) [B] (c) Start");
        [TestMethod] public void CycleII_PB_iC_End() => AssertPoint(_analysis.Waves[1].SubWaves[1].SubWaves[2].EndPoint, Utc(2018, 2, 20, 22, 0), 11786.01m, "Cycle(II) [B] (c) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(II) [C]
        //
        // (i): $11'786 → $6'430.00 (01.04.2018)
        // (ii): $6'430.00 → $10'020 (05.05.2018)
        // (iii): $10'020 → $3'652.66 (25.11.2018)
        // (iv): $3'652.66 → $4'450.38 (29.11.2018)
        // (v): $4'450.38 → $3'156.26 (15.12.2018)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleII_PC_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[1].SubWaves[2], 5, "Cycle(II) [C] Intermediate");

        [TestMethod] public void CycleII_PC_i1_Label() => AssertLabel(_analysis.Waves[1].SubWaves[2].SubWaves[0], WaveNumber.One, "Cycle(II) [C] (i)");
        [TestMethod] public void CycleII_PC_i1_Start() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[0].StartPoint, Utc(2018, 2, 20, 22, 0), 11786.01m, "Cycle(II) [C] (i) Start");
        [TestMethod] public void CycleII_PC_i1_End() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[0].EndPoint, Utc(2018, 4, 1, 14, 0), 6430m, "Cycle(II) [C] (i) End");

        [TestMethod] public void CycleII_PC_i2_Label() => AssertLabel(_analysis.Waves[1].SubWaves[2].SubWaves[1], WaveNumber.Two, "Cycle(II) [C] (ii)");
        [TestMethod] public void CycleII_PC_i2_Start() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[1].StartPoint, Utc(2018, 4, 1, 14, 0), 6430m, "Cycle(II) [C] (ii) Start");
        [TestMethod] public void CycleII_PC_i2_End() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[1].EndPoint, Utc(2018, 5, 5, 12, 0), 10020m, "Cycle(II) [C] (ii) End");

        [TestMethod] public void CycleII_PC_i3_Label() => AssertLabel(_analysis.Waves[1].SubWaves[2].SubWaves[2], WaveNumber.Three, "Cycle(II) [C] (iii)");
        [TestMethod] public void CycleII_PC_i3_Start() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[2].StartPoint, Utc(2018, 5, 5, 12, 0), 10020m, "Cycle(II) [C] (iii) Start");
        [TestMethod] public void CycleII_PC_i3_End() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[2].EndPoint, Utc(2018, 11, 25, 10, 0), 3652.66m, "Cycle(II) [C] (iii) End");

        [TestMethod] public void CycleII_PC_i4_Label() => AssertLabel(_analysis.Waves[1].SubWaves[2].SubWaves[3], WaveNumber.Four, "Cycle(II) [C] (iv)");
        [TestMethod] public void CycleII_PC_i4_Start() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[3].StartPoint, Utc(2018, 11, 25, 10, 0), 3652.66m, "Cycle(II) [C] (iv) Start");
        [TestMethod] public void CycleII_PC_i4_End() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[3].EndPoint, Utc(2018, 11, 29, 11, 0), 4450.38m, "Cycle(II) [C] (iv) End");

        [TestMethod] public void CycleII_PC_i5_Label() => AssertLabel(_analysis.Waves[1].SubWaves[2].SubWaves[4], WaveNumber.Five, "Cycle(II) [C] (v)");
        [TestMethod] public void CycleII_PC_i5_Start() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[4].StartPoint, Utc(2018, 11, 29, 11, 0), 4450.38m, "Cycle(II) [C] (v) Start");
        [TestMethod] public void CycleII_PC_i5_End() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[4].EndPoint, Utc(2018, 12, 15, 15, 0), 3156.26m, "Cycle(II) [C] (v) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(III) [1]
        //
        // (i): $3'156.26 → $4'198.00 (24.12.2018)
        // (ii): $4'198.00 → $3'349.92 (29.01.2019)
        // (iii): $3'349.92 → $9'074.26 (30.05.2019)
        // (iv): $9'074.26 → $7'444.58 (06.06.2019)
        // (v): $7'444.58 → $13'970 (26.06.2019)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleIII_P1_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[2].SubWaves[0], 5, "Cycle(III) [1] Intermediate");

        [TestMethod] public void CycleIII_P1_i1_Label() => AssertLabel(_analysis.Waves[2].SubWaves[0].SubWaves[0], WaveNumber.One, "Cycle(III) [1] (i)");
        [TestMethod] public void CycleIII_P1_i1_Start() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[0].StartPoint, Utc(2018, 12, 15, 15, 0), 3156.26m, "Cycle(III) [1] (i) Start");
        [TestMethod] public void CycleIII_P1_i1_End() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[0].EndPoint, Utc(2018, 12, 24, 8, 0), 4198m, "Cycle(III) [1] (i) End");

        [TestMethod] public void CycleIII_P1_i2_Label() => AssertLabel(_analysis.Waves[2].SubWaves[0].SubWaves[1], WaveNumber.Two, "Cycle(III) [1] (ii)");
        [TestMethod] public void CycleIII_P1_i2_Start() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[1].StartPoint, Utc(2018, 12, 24, 8, 0), 4198m, "Cycle(III) [1] (ii) Start");
        [TestMethod] public void CycleIII_P1_i2_End() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[1].EndPoint, Utc(2019, 1, 29, 7, 0), 3349.92m, "Cycle(III) [1] (ii) End");

        [TestMethod] public void CycleIII_P1_i3_Label() => AssertLabel(_analysis.Waves[2].SubWaves[0].SubWaves[2], WaveNumber.Three, "Cycle(III) [1] (iii)");
        [TestMethod] public void CycleIII_P1_i3_Start() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[2].StartPoint, Utc(2019, 1, 29, 7, 0), 3349.92m, "Cycle(III) [1] (iii) Start");
        [TestMethod] public void CycleIII_P1_i3_End() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[2].EndPoint, Utc(2019, 5, 30, 16, 0), 9074.26m, "Cycle(III) [1] (iii) End");

        [TestMethod] public void CycleIII_P1_i4_Label() => AssertLabel(_analysis.Waves[2].SubWaves[0].SubWaves[3], WaveNumber.Four, "Cycle(III) [1] (iv)");
        [TestMethod] public void CycleIII_P1_i4_Start() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[3].StartPoint, Utc(2019, 5, 30, 16, 0), 9074.26m, "Cycle(III) [1] (iv) Start");
        [TestMethod] public void CycleIII_P1_i4_End() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[3].EndPoint, Utc(2019, 6, 6, 18, 0), 7444.58m, "Cycle(III) [1] (iv) End");

        [TestMethod] public void CycleIII_P1_i5_Label() => AssertLabel(_analysis.Waves[2].SubWaves[0].SubWaves[4], WaveNumber.Five, "Cycle(III) [1] (v)");
        [TestMethod] public void CycleIII_P1_i5_Start() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[4].StartPoint, Utc(2019, 6, 6, 18, 0), 7444.58m, "Cycle(III) [1] (v) Start");
        [TestMethod] public void CycleIII_P1_i5_End() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[4].EndPoint, Utc(2019, 6, 26, 20, 0), 13970m, "Cycle(III) [1] (v) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(III) [2]
        //
        // (a): $13'970 → $6'435.00 (18.12.2019)
        // (b): $6'435.00 → $10'500 (13.02.2020)
        // (c): $10'500 → $3'782.13 (13.03.2020)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleIII_P2_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[2].SubWaves[1], 3, "Cycle(III) [2] Intermediate");

        [TestMethod] public void CycleIII_P2_iA_Label() => AssertLabel(_analysis.Waves[2].SubWaves[1].SubWaves[0], WaveNumber.A, "Cycle(III) [2] (a)");
        [TestMethod] public void CycleIII_P2_iA_Start() => AssertPoint(_analysis.Waves[2].SubWaves[1].SubWaves[0].StartPoint, Utc(2019, 6, 26, 20, 0), 13970m, "Cycle(III) [2] (a) Start");
        [TestMethod] public void CycleIII_P2_iA_End() => AssertPoint(_analysis.Waves[2].SubWaves[1].SubWaves[0].EndPoint, Utc(2019, 12, 18, 13, 0), 6435m, "Cycle(III) [2] (a) End");

        [TestMethod] public void CycleIII_P2_iB_Label() => AssertLabel(_analysis.Waves[2].SubWaves[1].SubWaves[1], WaveNumber.B, "Cycle(III) [2] (b)");
        [TestMethod] public void CycleIII_P2_iB_Start() => AssertPoint(_analysis.Waves[2].SubWaves[1].SubWaves[1].StartPoint, Utc(2019, 12, 18, 13, 0), 6435m, "Cycle(III) [2] (b) Start");
        [TestMethod] public void CycleIII_P2_iB_End() => AssertPoint(_analysis.Waves[2].SubWaves[1].SubWaves[1].EndPoint, Utc(2020, 2, 13, 13, 0), 10500m, "Cycle(III) [2] (b) End");

        [TestMethod] public void CycleIII_P2_iC_Label() => AssertLabel(_analysis.Waves[2].SubWaves[1].SubWaves[2], WaveNumber.C, "Cycle(III) [2] (c)");
        [TestMethod] public void CycleIII_P2_iC_Start() => AssertPoint(_analysis.Waves[2].SubWaves[1].SubWaves[2].StartPoint, Utc(2020, 2, 13, 13, 0), 10500m, "Cycle(III) [2] (c) Start");
        [TestMethod] public void CycleIII_P2_iC_End() => AssertPoint(_analysis.Waves[2].SubWaves[1].SubWaves[2].EndPoint, Utc(2020, 3, 13, 2, 0), 3782.13m, "Cycle(III) [2] (c) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(III) [3]
        //
        // (i): $3'782.13 → $6'900.00 (20.03.2020)
        // (ii): $6'900.00 → $5'670.00 (20.03.2020)
        // (iii): $5'670.00 → $64'854 (14.04.2021)
        // (iv): $64'854 → $28'805 (22.06.2021)
        // (v): $28'805 → $69'000 (10.11.2021)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleIII_P3_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[2].SubWaves[2], 5, "Cycle(III) [3] Intermediate");

        [TestMethod] public void CycleIII_P3_i1_Label() => AssertLabel(_analysis.Waves[2].SubWaves[2].SubWaves[0], WaveNumber.One, "Cycle(III) [3] (i)");
        [TestMethod] public void CycleIII_P3_i1_Start() => AssertPoint(_analysis.Waves[2].SubWaves[2].SubWaves[0].StartPoint, Utc(2020, 3, 13, 2, 0), 3782.13m, "Cycle(III) [3] (i) Start");
        [TestMethod] public void CycleIII_P3_i1_End() => AssertPoint(_analysis.Waves[2].SubWaves[2].SubWaves[0].EndPoint, Utc(2020, 3, 20, 10, 0), 6900m, "Cycle(III) [3] (i) End");

        [TestMethod] public void CycleIII_P3_i2_Label() => AssertLabel(_analysis.Waves[2].SubWaves[2].SubWaves[1], WaveNumber.Two, "Cycle(III) [3] (ii)");
        [TestMethod] public void CycleIII_P3_i2_Start() => AssertPoint(_analysis.Waves[2].SubWaves[2].SubWaves[1].StartPoint, Utc(2020, 3, 20, 10, 0), 6900m, "Cycle(III) [3] (ii) Start");
        [TestMethod] public void CycleIII_P3_i2_End() => AssertPoint(_analysis.Waves[2].SubWaves[2].SubWaves[1].EndPoint, Utc(2020, 3, 20, 20, 0), 5670m, "Cycle(III) [3] (ii) End");

        [TestMethod] public void CycleIII_P3_i3_Label() => AssertLabel(_analysis.Waves[2].SubWaves[2].SubWaves[2], WaveNumber.Three, "Cycle(III) [3] (iii)");
        [TestMethod] public void CycleIII_P3_i3_Start() => AssertPoint(_analysis.Waves[2].SubWaves[2].SubWaves[2].StartPoint, Utc(2020, 3, 20, 20, 0), 5670m, "Cycle(III) [3] (iii) Start");
        [TestMethod] public void CycleIII_P3_i3_End() => AssertPoint(_analysis.Waves[2].SubWaves[2].SubWaves[2].EndPoint, Utc(2021, 4, 14, 6, 0), 64854m, "Cycle(III) [3] (iii) End");

        [TestMethod] public void CycleIII_P3_i4_Label() => AssertLabel(_analysis.Waves[2].SubWaves[2].SubWaves[3], WaveNumber.Four, "Cycle(III) [3] (iv)");
        [TestMethod] public void CycleIII_P3_i4_Start() => AssertPoint(_analysis.Waves[2].SubWaves[2].SubWaves[3].StartPoint, Utc(2021, 4, 14, 6, 0), 64854m, "Cycle(III) [3] (iv) Start");
        [TestMethod] public void CycleIII_P3_i4_End() => AssertPoint(_analysis.Waves[2].SubWaves[2].SubWaves[3].EndPoint, Utc(2021, 6, 22, 13, 0), 28805m, "Cycle(III) [3] (iv) End");

        [TestMethod] public void CycleIII_P3_i5_Label() => AssertLabel(_analysis.Waves[2].SubWaves[2].SubWaves[4], WaveNumber.Five, "Cycle(III) [3] (v)");
        [TestMethod] public void CycleIII_P3_i5_Start() => AssertPoint(_analysis.Waves[2].SubWaves[2].SubWaves[4].StartPoint, Utc(2021, 6, 22, 13, 0), 28805m, "Cycle(III) [3] (v) Start");
        [TestMethod] public void CycleIII_P3_i5_End() => AssertPoint(_analysis.Waves[2].SubWaves[2].SubWaves[4].EndPoint, Utc(2021, 11, 10, 14, 0), 69000m, "Cycle(III) [3] (v) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(III) [4]
        //
        // (a): $69'000 → $32'917 (24.01.2022)
        // (b): $32'917 → $48'190 (28.03.2022)
        // (c): $48'190 → $15'476 (21.11.2022)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleIII_P4_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[2].SubWaves[3], 3, "Cycle(III) [4] Intermediate");

        [TestMethod] public void CycleIII_P4_iA_Label() => AssertLabel(_analysis.Waves[2].SubWaves[3].SubWaves[0], WaveNumber.A, "Cycle(III) [4] (a)");
        [TestMethod] public void CycleIII_P4_iA_Start() => AssertPoint(_analysis.Waves[2].SubWaves[3].SubWaves[0].StartPoint, Utc(2021, 11, 10, 14, 0), 69000m, "Cycle(III) [4] (a) Start");
        [TestMethod] public void CycleIII_P4_iA_End() => AssertPoint(_analysis.Waves[2].SubWaves[3].SubWaves[0].EndPoint, Utc(2022, 1, 24, 13, 0), 32917.17m, "Cycle(III) [4] (a) End");

        [TestMethod] public void CycleIII_P4_iB_Label() => AssertLabel(_analysis.Waves[2].SubWaves[3].SubWaves[1], WaveNumber.B, "Cycle(III) [4] (b)");
        [TestMethod] public void CycleIII_P4_iB_Start() => AssertPoint(_analysis.Waves[2].SubWaves[3].SubWaves[1].StartPoint, Utc(2022, 1, 24, 13, 0), 32917.17m, "Cycle(III) [4] (b) Start");
        [TestMethod] public void CycleIII_P4_iB_End() => AssertPoint(_analysis.Waves[2].SubWaves[3].SubWaves[1].EndPoint, Utc(2022, 3, 28, 19, 0), 48189.84m, "Cycle(III) [4] (b) End");

        [TestMethod] public void CycleIII_P4_iC_Label() => AssertLabel(_analysis.Waves[2].SubWaves[3].SubWaves[2], WaveNumber.C, "Cycle(III) [4] (c)");
        [TestMethod] public void CycleIII_P4_iC_Start() => AssertPoint(_analysis.Waves[2].SubWaves[3].SubWaves[2].StartPoint, Utc(2022, 3, 28, 19, 0), 48189.84m, "Cycle(III) [4] (c) Start");
        [TestMethod] public void CycleIII_P4_iC_End() => AssertPoint(_analysis.Waves[2].SubWaves[3].SubWaves[2].EndPoint, Utc(2022, 11, 21, 21, 0), 15476m, "Cycle(III) [4] (c) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(III) [5]
        //
        // (i): $15'476 → $25'250 (16.02.2023)
        // (ii): $25'250 → $19'549 (10.03.2023)
        // (iii): $19'549 → $109'588 (20.01.2025)
        // (iv): $109'588 → $74'508 (07.04.2025)
        // (v): $74'508 → $126'200 (06.10.2025)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleIII_P5_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[2].SubWaves[4], 5, "Cycle(III) [5] Intermediate");

        [TestMethod] public void CycleIII_P5_i1_Label() => AssertLabel(_analysis.Waves[2].SubWaves[4].SubWaves[0], WaveNumber.One, "Cycle(III) [5] (i)");
        [TestMethod] public void CycleIII_P5_i1_Start() => AssertPoint(_analysis.Waves[2].SubWaves[4].SubWaves[0].StartPoint, Utc(2022, 11, 21, 21, 0), 15476m, "Cycle(III) [5] (i) Start");
        [TestMethod] public void CycleIII_P5_i1_End() => AssertPoint(_analysis.Waves[2].SubWaves[4].SubWaves[0].EndPoint, Utc(2023, 2, 16, 16, 0), 25250m, "Cycle(III) [5] (i) End");

        [TestMethod] public void CycleIII_P5_i2_Label() => AssertLabel(_analysis.Waves[2].SubWaves[4].SubWaves[1], WaveNumber.Two, "Cycle(III) [5] (ii)");
        [TestMethod] public void CycleIII_P5_i2_Start() => AssertPoint(_analysis.Waves[2].SubWaves[4].SubWaves[1].StartPoint, Utc(2023, 2, 16, 16, 0), 25250m, "Cycle(III) [5] (ii) Start");
        [TestMethod] public void CycleIII_P5_i2_End() => AssertPoint(_analysis.Waves[2].SubWaves[4].SubWaves[1].EndPoint, Utc(2023, 3, 10, 10, 0), 19549.09m, "Cycle(III) [5] (ii) End");

        [TestMethod] public void CycleIII_P5_i3_Label() => AssertLabel(_analysis.Waves[2].SubWaves[4].SubWaves[2], WaveNumber.Three, "Cycle(III) [5] (iii)");
        [TestMethod] public void CycleIII_P5_i3_Start() => AssertPoint(_analysis.Waves[2].SubWaves[4].SubWaves[2].StartPoint, Utc(2023, 3, 10, 10, 0), 19549.09m, "Cycle(III) [5] (iii) Start");
        [TestMethod] public void CycleIII_P5_i3_End() => AssertPoint(_analysis.Waves[2].SubWaves[4].SubWaves[2].EndPoint, Utc(2025, 1, 20, 6, 0), 109588m, "Cycle(III) [5] (iii) End");

        [TestMethod] public void CycleIII_P5_i4_Label() => AssertLabel(_analysis.Waves[2].SubWaves[4].SubWaves[3], WaveNumber.Four, "Cycle(III) [5] (iv)");
        [TestMethod] public void CycleIII_P5_i4_Start() => AssertPoint(_analysis.Waves[2].SubWaves[4].SubWaves[3].StartPoint, Utc(2025, 1, 20, 6, 0), 109588m, "Cycle(III) [5] (iv) Start");
        [TestMethod] public void CycleIII_P5_i4_End() => AssertPoint(_analysis.Waves[2].SubWaves[4].SubWaves[3].EndPoint, Utc(2025, 4, 7, 7, 0), 74508m, "Cycle(III) [5] (iv) End");

        [TestMethod] public void CycleIII_P5_i5_Label() => AssertLabel(_analysis.Waves[2].SubWaves[4].SubWaves[4], WaveNumber.Five, "Cycle(III) [5] (v)");
        [TestMethod] public void CycleIII_P5_i5_Start() => AssertPoint(_analysis.Waves[2].SubWaves[4].SubWaves[4].StartPoint, Utc(2025, 4, 7, 7, 0), 74508m, "Cycle(III) [5] (v) Start");
        [TestMethod] public void CycleIII_P5_i5_End() => AssertPoint(_analysis.Waves[2].SubWaves[4].SubWaves[4].EndPoint, Utc(2025, 10, 6, 18, 0), 126199.63m, "Cycle(III) [5] (v) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(IV) [A]
        //
        // (i): $126'200 → $102'000 (10.10.2025)
        // (ii): $102'000 → $116'400 (27.10.2025)
        // (iii): $116'400 → $80'600 (21.11.2025)
        // (iv): $80'600 → $97'924 (14.01.2026)
        // (v): $97'924 → $60'000 (06.02.2026)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleIV_PA_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[3].SubWaves[0], 5, "Cycle(IV) [A] Intermediate");

        [TestMethod] public void CycleIV_PA_i1_Label() => AssertLabel(_analysis.Waves[3].SubWaves[0].SubWaves[0], WaveNumber.One, "Cycle(IV) [A] (i)");
        [TestMethod] public void CycleIV_PA_i1_Start() => AssertPoint(_analysis.Waves[3].SubWaves[0].SubWaves[0].StartPoint, Utc(2025, 10, 6, 18, 0), 126199.63m, "Cycle(IV) [A] (i) Start");
        [TestMethod] public void CycleIV_PA_i1_End() => AssertPoint(_analysis.Waves[3].SubWaves[0].SubWaves[0].EndPoint, Utc(2025, 10, 10, 21, 0), 102000m, "Cycle(IV) [A] (i) End");

        [TestMethod] public void CycleIV_PA_i2_Label() => AssertLabel(_analysis.Waves[3].SubWaves[0].SubWaves[1], WaveNumber.Two, "Cycle(IV) [A] (ii)");
        [TestMethod] public void CycleIV_PA_i2_Start() => AssertPoint(_analysis.Waves[3].SubWaves[0].SubWaves[1].StartPoint, Utc(2025, 10, 10, 21, 0), 102000m, "Cycle(IV) [A] (ii) Start");
        [TestMethod] public void CycleIV_PA_i2_End() => AssertPoint(_analysis.Waves[3].SubWaves[0].SubWaves[1].EndPoint, Utc(2025, 10, 27, 7, 0), 116400m, "Cycle(IV) [A] (ii) End");

        [TestMethod] public void CycleIV_PA_i3_Label() => AssertLabel(_analysis.Waves[3].SubWaves[0].SubWaves[2], WaveNumber.Three, "Cycle(IV) [A] (iii)");
        [TestMethod] public void CycleIV_PA_i3_Start() => AssertPoint(_analysis.Waves[3].SubWaves[0].SubWaves[2].StartPoint, Utc(2025, 10, 27, 7, 0), 116400m, "Cycle(IV) [A] (iii) Start");
        [TestMethod] public void CycleIV_PA_i3_End() => AssertPoint(_analysis.Waves[3].SubWaves[0].SubWaves[2].EndPoint, Utc(2025, 11, 21, 12, 0), 80600m, "Cycle(IV) [A] (iii) End");

        [TestMethod] public void CycleIV_PA_i4_Label() => AssertLabel(_analysis.Waves[3].SubWaves[0].SubWaves[3], WaveNumber.Four, "Cycle(IV) [A] (iv)");
        [TestMethod] public void CycleIV_PA_i4_Start() => AssertPoint(_analysis.Waves[3].SubWaves[0].SubWaves[3].StartPoint, Utc(2025, 11, 21, 12, 0), 80600m, "Cycle(IV) [A] (iv) Start");
        [TestMethod] public void CycleIV_PA_i4_End() => AssertPoint(_analysis.Waves[3].SubWaves[0].SubWaves[3].EndPoint, Utc(2026, 1, 14, 20, 0), 97924.49m, "Cycle(IV) [A] (iv) End");

        [TestMethod] public void CycleIV_PA_i5_Label() => AssertLabel(_analysis.Waves[3].SubWaves[0].SubWaves[4], WaveNumber.Five, "Cycle(IV) [A] (v)");
        [TestMethod] public void CycleIV_PA_i5_Start() => AssertPoint(_analysis.Waves[3].SubWaves[0].SubWaves[4].StartPoint, Utc(2026, 1, 14, 20, 0), 97924.49m, "Cycle(IV) [A] (v) Start");
        [TestMethod] public void CycleIV_PA_i5_End() => AssertPoint(_analysis.Waves[3].SubWaves[0].SubWaves[4].EndPoint, Utc(2026, 2, 6, 0, 0), 60000m, "Cycle(IV) [A] (v) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(IV) [B]
        //
        // (a): $60'000 → $72'271 (08.02.2026)
        // (b): $72'271 → $62'510 (24.02.2026)
        // (c): $62'510 → $76'000 (17.03.2026)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleIV_PB_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[3].SubWaves[1], 3, "Cycle(IV) [B] Intermediate");

        [TestMethod] public void CycleIV_PB_iA_Label() => AssertLabel(_analysis.Waves[3].SubWaves[1].SubWaves[0], WaveNumber.A, "Cycle(IV) [B] (a)");
        [TestMethod] public void CycleIV_PB_iA_Start() => AssertPoint(_analysis.Waves[3].SubWaves[1].SubWaves[0].StartPoint, Utc(2026, 2, 6, 0, 0), 60000m, "Cycle(IV) [B] (a) Start");
        [TestMethod] public void CycleIV_PB_iA_End() => AssertPoint(_analysis.Waves[3].SubWaves[1].SubWaves[0].EndPoint, Utc(2026, 2, 8, 23, 0), 72271.41m, "Cycle(IV) [B] (a) End");

        [TestMethod] public void CycleIV_PB_iB_Label() => AssertLabel(_analysis.Waves[3].SubWaves[1].SubWaves[1], WaveNumber.B, "Cycle(IV) [B] (b)");
        [TestMethod] public void CycleIV_PB_iB_Start() => AssertPoint(_analysis.Waves[3].SubWaves[1].SubWaves[1].StartPoint, Utc(2026, 2, 8, 23, 0), 72271.41m, "Cycle(IV) [B] (b) Start");
        [TestMethod] public void CycleIV_PB_iB_End() => AssertPoint(_analysis.Waves[3].SubWaves[1].SubWaves[1].EndPoint, Utc(2026, 2, 24, 13, 0), 62510.28m, "Cycle(IV) [B] (b) End");

        [TestMethod] public void CycleIV_PB_iC_Label() => AssertLabel(_analysis.Waves[3].SubWaves[1].SubWaves[2], WaveNumber.C, "Cycle(IV) [B] (c)");
        [TestMethod] public void CycleIV_PB_iC_Start() => AssertPoint(_analysis.Waves[3].SubWaves[1].SubWaves[2].StartPoint, Utc(2026, 2, 24, 13, 0), 62510.28m, "Cycle(IV) [B] (c) Start");
        [TestMethod] public void CycleIV_PB_iC_End() => AssertPoint(_analysis.Waves[3].SubWaves[1].SubWaves[2].EndPoint, Utc(2026, 3, 17, 1, 0), 76000m, "Cycle(IV) [B] (c) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(IV) [C]
        //
        // (i): $76'000 → $76'000 (17.03.2026) (in-progress)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleIV_PC_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[3].SubWaves[2], 1, "Cycle(IV) [C] Intermediate");

        [TestMethod] public void CycleIV_PC_i1_Label() => AssertLabel(_analysis.Waves[3].SubWaves[2].SubWaves[0], WaveNumber.One, "Cycle(IV) [C] (i)");
        [TestMethod] public void CycleIV_PC_i1_InProgress() => AssertInProgress(_analysis.Waves[3].SubWaves[2].SubWaves[0], true, "Cycle(IV) [C] (i)");
        [TestMethod] public void CycleIV_PC_i1_Start() => AssertPoint(_analysis.Waves[3].SubWaves[2].SubWaves[0].StartPoint, Utc(2026, 3, 17, 1, 0), 76000m, "Cycle(IV) [C] (i) Start");

        // ══════════════════════════════════════════════════
        // Projections — Cycle (IV): W4 retracement of Cycle III ($3,156.26 → $126,199.63)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleIV_Projection_0786() => AssertProjectionTarget(_analysis.Waves[3], LogRetracement(3156.26m, 126199.63m, 0.786m), 0.786m, "Cycle(IV) 0.786 projection");
        [TestMethod] public void CycleIV_Projection_0618() => AssertProjectionTarget(_analysis.Waves[3], LogRetracement(3156.26m, 126199.63m, 0.618m), 0.618m, "Cycle(IV) 0.618 projection");
        [TestMethod] public void CycleIV_Projection_050() => AssertProjectionTarget(_analysis.Waves[3], LogRetracement(3156.26m, 126199.63m, 0.5m), 0.5m, "Cycle(IV) 0.5 projection");
        [TestMethod] public void CycleIV_Projection_0382() => AssertProjectionTarget(_analysis.Waves[3], LogRetracement(3156.26m, 126199.63m, 0.382m), 0.382m, "Cycle(IV) 0.382 projection");
        [TestMethod] public void CycleIV_Projection_0236() => AssertProjectionTarget(_analysis.Waves[3], LogRetracement(3156.26m, 126199.63m, 0.236m), 0.236m, "Cycle(IV) 0.236 projection");

        // ══════════════════════════════════════════════════
        // Projections — Primary [C] inside Cycle (IV)
        // C extension of A ($126,199.63 → $60,000.00) from B end ($76,000.00)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleIV_C_Projection_2618() { var (a, b, c) = GetCycleIV_ABC(); AssertProjectionTarget(_analysis.Waves[3].SubWaves[2], LogExtension(a, b, c, 2.618m), 2.618m, "Cycle(IV) [C] 2.618 projection"); }
        [TestMethod] public void CycleIV_C_Projection_1618() { var (a, b, c) = GetCycleIV_ABC(); AssertProjectionTarget(_analysis.Waves[3].SubWaves[2], LogExtension(a, b, c, 1.618m), 1.618m, "Cycle(IV) [C] 1.618 projection"); }
        [TestMethod] public void CycleIV_C_Projection_AEqualsC() { var (a, b, c) = GetCycleIV_ABC(); AssertProjectionTarget(_analysis.Waves[3].SubWaves[2], LogExtension(a, b, c, 1.0m), 1.0m, "Cycle(IV) [C] A=C projection"); }
        [TestMethod] public void CycleIV_C_Projection_0618() { var (a, b, c) = GetCycleIV_ABC(); AssertProjectionTarget(_analysis.Waves[3].SubWaves[2], LogExtension(a, b, c, 0.618m), 0.618m, "Cycle(IV) [C] 0.618 projection"); }

        private (decimal aStart, decimal aEnd, decimal bEnd) GetCycleIV_ABC()
        {
            Assert.IsTrue(_analysis.Waves[3].SubWaves.Count >= 3, "Cycle(IV) should have [A],[B],[C]");
            return (_analysis.Waves[3].SubWaves[0].StartPoint.Price,
                    _analysis.Waves[3].SubWaves[0].EndPoint.Price,
                    _analysis.Waves[3].SubWaves[1].EndPoint.Price);
        }
    }
}
