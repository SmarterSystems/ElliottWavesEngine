using SmarterSystems.ElliottWaves.Analyzer.Interfaces;

namespace SmarterSystems.ElliottWaves.Validation
{
    [TestClass]
    public sealed class XrpRegressionTest : RegressionTestBase
    {
        private static ElliottWavesAnalysis _analysis = null!;

        [ClassInitialize]
        public static void Setup(TestContext _)
        {
            _analysis = LoadAndAnalyze("XRPUSDT");
        }

        // ══════════════════════════════════════════════════
        // Cycle Degree: 2 waves — CORRECTIVE A-B structure
        //
        // Cycle (A): 13.03.2020 $0.10129 → 18.07.2025 $3.6607  (corrective A-B-C primary)
        // Cycle (B): 18.07.2025 $3.6607 → in-progress
        // ══════════════════════════════════════════════════

        [TestMethod] public void Cycle_WaveCount() => AssertWaveCount(_analysis, 2, "XRP Cycle");

        // ── Cycle (A): $0.10129 → $3.6607 ──

        [TestMethod] public void CycleA_Label() => AssertLabel(_analysis.Waves[0], WaveNumber.A, "Cycle(A)");
        [TestMethod] public void CycleA_NotInProgress() => AssertInProgress(_analysis.Waves[0], false, "Cycle(A)");
        [TestMethod] public void CycleA_Start() => AssertPoint(_analysis.Waves[0].StartPoint, Utc(2020, 3, 13, 2), 0.10129m, "Cycle(A) Start");
        [TestMethod] public void CycleA_End() => AssertPoint(_analysis.Waves[0].EndPoint, Utc(2025, 7, 18, 1), 3.6607m, "Cycle(A) End");

        // ── Cycle (B): $3.6607 → in-progress ──

        [TestMethod] public void CycleB_Label() => AssertLabel(_analysis.Waves[1], WaveNumber.B, "Cycle(B)");
        [TestMethod] public void CycleB_InProgress() => AssertInProgress(_analysis.Waves[1], true, "Cycle(B)");
        [TestMethod] public void CycleB_Start() => AssertPoint(_analysis.Waves[1].StartPoint, Utc(2025, 7, 18, 1), 3.6607m, "Cycle(B) Start");

        // ══════════════════════════════════════════════════
        // Primary Degree — inside Cycle (A): A-B-C (corrective)
        //
        // [A]: $0.10129 → $1.96689 (14.04.2021)
        // [B]: $1.96689 → $0.3105 (17.06.2022)
        // [C]: $0.3105 → $3.6607 (18.07.2025)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleA_Primary_WaveCount() => AssertSubWaveCount(_analysis.Waves[0], 3, "Cycle(A) Primary");

        [TestMethod] public void CycleA_PA_Label() => AssertLabel(_analysis.Waves[0].SubWaves[0], WaveNumber.A, "Cycle(A) [A]");
        [TestMethod] public void CycleA_PA_Start() => AssertPoint(_analysis.Waves[0].SubWaves[0].StartPoint, Utc(2020, 3, 13, 2), 0.10129m, "Cycle(A) [A] Start");
        [TestMethod] public void CycleA_PA_End() => AssertPoint(_analysis.Waves[0].SubWaves[0].EndPoint, Utc(2021, 4, 14, 5), 1.96689m, "Cycle(A) [A] End");

        [TestMethod] public void CycleA_PB_Label() => AssertLabel(_analysis.Waves[0].SubWaves[1], WaveNumber.B, "Cycle(A) [B]");
        [TestMethod] public void CycleA_PB_Start() => AssertPoint(_analysis.Waves[0].SubWaves[1].StartPoint, Utc(2021, 4, 14, 5), 1.96689m, "Cycle(A) [B] Start");
        [TestMethod] public void CycleA_PB_End() => AssertPoint(_analysis.Waves[0].SubWaves[1].EndPoint, Utc(2022, 6, 18, 20), 0.2872m, "Cycle(A) [B] End");

        [TestMethod] public void CycleA_PC_Label() => AssertLabel(_analysis.Waves[0].SubWaves[2], WaveNumber.C, "Cycle(A) [C]");
        [TestMethod] public void CycleA_PC_Start() => AssertPoint(_analysis.Waves[0].SubWaves[2].StartPoint, Utc(2022, 6, 18, 20), 0.2872m, "Cycle(A) [C] Start");
        [TestMethod] public void CycleA_PC_End() => AssertPoint(_analysis.Waves[0].SubWaves[2].EndPoint, Utc(2025, 7, 18, 1), 3.6607m, "Cycle(A) [C] End");

        // ══════════════════════════════════════════════════
        // Primary Degree — inside Cycle (B): in-progress
        //
        // [A]: $3.6607 → $1.1172 (06.02.2026)
        // ══════════════════════════════════════════════════

        [TestMethod]
        public void CycleB_HasSubWaves()
        {
            Assert.IsNotNull(_analysis.Waves[1].SubWaves, "Cycle(B) should have sub-waves");
            Assert.IsTrue(_analysis.Waves[1].SubWaves.Count >= 1, "Cycle(B) should have at least 1 primary wave");
        }

        [TestMethod] public void CycleB_PA_Label() => AssertLabel(_analysis.Waves[1].SubWaves[0], WaveNumber.A, "Cycle(B) [A]");
        [TestMethod] public void CycleB_PA_Start() => AssertPoint(_analysis.Waves[1].SubWaves[0].StartPoint, Utc(2025, 7, 18, 1), 3.6607m, "Cycle(B) [A] Start");
        [TestMethod] public void CycleB_PA_End() => AssertPoint(_analysis.Waves[1].SubWaves[0].EndPoint, Utc(2026, 2, 6, 0), 1.1172m, "Cycle(B) [A] End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(A) [A]
        //
        // (i): $0.10129 → $0.16614 (13.03.2020)
        // (ii): $0.16614 → $0.12815 (16.03.2020)
        // (iii): $0.12815 → $0.78068 (24.11.2020)
        // (iv): $0.78068 → $0.17351 (29.12.2020)
        // (v): $0.17351 → $1.9669 (14.04.2021)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleA_PA_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[0].SubWaves[0], 5, "Cycle(A) [A] Intermediate");

        [TestMethod] public void CycleA_PA_i1_Label() => AssertLabel(_analysis.Waves[0].SubWaves[0].SubWaves[0], WaveNumber.One, "Cycle(A) [A] (i)");
        [TestMethod] public void CycleA_PA_i1_Start() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[0].StartPoint, Utc(2020, 3, 13, 2, 0), 0.10129m, "Cycle(A) [A] (i) Start");
        [TestMethod] public void CycleA_PA_i1_End() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[0].EndPoint, Utc(2020, 3, 13, 13, 0), 0.16614m, "Cycle(A) [A] (i) End");

        [TestMethod] public void CycleA_PA_i2_Label() => AssertLabel(_analysis.Waves[0].SubWaves[0].SubWaves[1], WaveNumber.Two, "Cycle(A) [A] (ii)");
        [TestMethod] public void CycleA_PA_i2_Start() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[1].StartPoint, Utc(2020, 3, 13, 13, 0), 0.16614m, "Cycle(A) [A] (ii) Start");
        [TestMethod] public void CycleA_PA_i2_End() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[1].EndPoint, Utc(2020, 3, 16, 11, 0), 0.12815m, "Cycle(A) [A] (ii) End");

        [TestMethod] public void CycleA_PA_i3_Label() => AssertLabel(_analysis.Waves[0].SubWaves[0].SubWaves[2], WaveNumber.Three, "Cycle(A) [A] (iii)");
        [TestMethod] public void CycleA_PA_i3_Start() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[2].StartPoint, Utc(2020, 3, 16, 11, 0), 0.12815m, "Cycle(A) [A] (iii) Start");
        [TestMethod] public void CycleA_PA_i3_End() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[2].EndPoint, Utc(2020, 11, 24, 5, 0), 0.78068m, "Cycle(A) [A] (iii) End");

        [TestMethod] public void CycleA_PA_i4_Label() => AssertLabel(_analysis.Waves[0].SubWaves[0].SubWaves[3], WaveNumber.Four, "Cycle(A) [A] (iv)");
        [TestMethod] public void CycleA_PA_i4_Start() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[3].StartPoint, Utc(2020, 11, 24, 5, 0), 0.78068m, "Cycle(A) [A] (iv) Start");
        [TestMethod] public void CycleA_PA_i4_End() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[3].EndPoint, Utc(2020, 12, 29, 16, 0), 0.17351m, "Cycle(A) [A] (iv) End");

        [TestMethod] public void CycleA_PA_i5_Label() => AssertLabel(_analysis.Waves[0].SubWaves[0].SubWaves[4], WaveNumber.Five, "Cycle(A) [A] (v)");
        [TestMethod] public void CycleA_PA_i5_Start() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[4].StartPoint, Utc(2020, 12, 29, 16, 0), 0.17351m, "Cycle(A) [A] (v) Start");
        [TestMethod] public void CycleA_PA_i5_End() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[4].EndPoint, Utc(2021, 4, 14, 5, 0), 1.96689m, "Cycle(A) [A] (v) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(A) [B]
        //
        // (a): $1.9669 → $0.50900 (22.06.2021)
        // (b): $0.50900 → $1.4151 (06.09.2021)
        // (c): $1.4151 → $0.28720 (18.06.2022)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleA_PB_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[0].SubWaves[1], 3, "Cycle(A) [B] Intermediate");

        [TestMethod] public void CycleA_PB_iA_Label() => AssertLabel(_analysis.Waves[0].SubWaves[1].SubWaves[0], WaveNumber.A, "Cycle(A) [B] (a)");
        [TestMethod] public void CycleA_PB_iA_Start() => AssertPoint(_analysis.Waves[0].SubWaves[1].SubWaves[0].StartPoint, Utc(2021, 4, 14, 5, 0), 1.96689m, "Cycle(A) [B] (a) Start");
        [TestMethod] public void CycleA_PB_iA_End() => AssertPoint(_analysis.Waves[0].SubWaves[1].SubWaves[0].EndPoint, Utc(2021, 6, 22, 13, 0), 0.509m, "Cycle(A) [B] (a) End");

        [TestMethod] public void CycleA_PB_iB_Label() => AssertLabel(_analysis.Waves[0].SubWaves[1].SubWaves[1], WaveNumber.B, "Cycle(A) [B] (b)");
        [TestMethod] public void CycleA_PB_iB_Start() => AssertPoint(_analysis.Waves[0].SubWaves[1].SubWaves[1].StartPoint, Utc(2021, 6, 22, 13, 0), 0.509m, "Cycle(A) [B] (b) Start");
        [TestMethod] public void CycleA_PB_iB_End() => AssertPoint(_analysis.Waves[0].SubWaves[1].SubWaves[1].EndPoint, Utc(2021, 9, 6, 21, 0), 1.4151m, "Cycle(A) [B] (b) End");

        [TestMethod] public void CycleA_PB_iC_Label() => AssertLabel(_analysis.Waves[0].SubWaves[1].SubWaves[2], WaveNumber.C, "Cycle(A) [B] (c)");
        [TestMethod] public void CycleA_PB_iC_Start() => AssertPoint(_analysis.Waves[0].SubWaves[1].SubWaves[2].StartPoint, Utc(2021, 9, 6, 21, 0), 1.4151m, "Cycle(A) [B] (c) Start");
        [TestMethod] public void CycleA_PB_iC_End() => AssertPoint(_analysis.Waves[0].SubWaves[1].SubWaves[2].EndPoint, Utc(2022, 6, 18, 20, 0), 0.2872m, "Cycle(A) [B] (c) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(A) [C]
        //
        // (i): $0.28720 → $0.55900 (23.09.2022)
        // (ii): $0.55900 → $0.30000 (02.01.2023)
        // (iii): $0.30000 → $3.4000 (16.01.2025)
        // (iv): $3.4000 → $1.6134 (07.04.2025)
        // (v): $1.6134 → $3.6607 (18.07.2025)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleA_PC_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[0].SubWaves[2], 5, "Cycle(A) [C] Intermediate");

        [TestMethod] public void CycleA_PC_i1_Label() => AssertLabel(_analysis.Waves[0].SubWaves[2].SubWaves[0], WaveNumber.One, "Cycle(A) [C] (i)");
        [TestMethod] public void CycleA_PC_i1_Start() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[0].StartPoint, Utc(2022, 6, 18, 20, 0), 0.2872m, "Cycle(A) [C] (i) Start");
        [TestMethod] public void CycleA_PC_i1_End() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[0].EndPoint, Utc(2022, 9, 23, 5, 0), 0.559m, "Cycle(A) [C] (i) End");

        [TestMethod] public void CycleA_PC_i2_Label() => AssertLabel(_analysis.Waves[0].SubWaves[2].SubWaves[1], WaveNumber.Two, "Cycle(A) [C] (ii)");
        [TestMethod] public void CycleA_PC_i2_Start() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[1].StartPoint, Utc(2022, 9, 23, 5, 0), 0.559m, "Cycle(A) [C] (ii) Start");
        [TestMethod] public void CycleA_PC_i2_End() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[1].EndPoint, Utc(2023, 1, 2, 1, 0), 0.3m, "Cycle(A) [C] (ii) End");

        [TestMethod] public void CycleA_PC_i3_Label() => AssertLabel(_analysis.Waves[0].SubWaves[2].SubWaves[2], WaveNumber.Three, "Cycle(A) [C] (iii)");
        [TestMethod] public void CycleA_PC_i3_Start() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[2].StartPoint, Utc(2023, 1, 2, 1, 0), 0.3m, "Cycle(A) [C] (iii) Start");
        [TestMethod] public void CycleA_PC_i3_End() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[2].EndPoint, Utc(2025, 1, 16, 16, 0), 3.4m, "Cycle(A) [C] (iii) End");

        [TestMethod] public void CycleA_PC_i4_Label() => AssertLabel(_analysis.Waves[0].SubWaves[2].SubWaves[3], WaveNumber.Four, "Cycle(A) [C] (iv)");
        [TestMethod] public void CycleA_PC_i4_Start() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[3].StartPoint, Utc(2025, 1, 16, 16, 0), 3.4m, "Cycle(A) [C] (iv) Start");
        [TestMethod] public void CycleA_PC_i4_End() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[3].EndPoint, Utc(2025, 4, 7, 6, 0), 1.6134m, "Cycle(A) [C] (iv) End");

        [TestMethod] public void CycleA_PC_i5_Label() => AssertLabel(_analysis.Waves[0].SubWaves[2].SubWaves[4], WaveNumber.Five, "Cycle(A) [C] (v)");
        [TestMethod] public void CycleA_PC_i5_Start() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[4].StartPoint, Utc(2025, 4, 7, 6, 0), 1.6134m, "Cycle(A) [C] (v) Start");
        [TestMethod] public void CycleA_PC_i5_End() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[4].EndPoint, Utc(2025, 7, 18, 1, 0), 3.6607m, "Cycle(A) [C] (v) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(B) [A]
        //
        // (i): $3.6607 → $2.7280 (03.08.2025)
        // (ii): $2.7280 → $3.3825 (08.08.2025)
        // (iii): $3.3825 → $1.2543 (10.10.2025)
        // (iv): $1.2543 → $2.6975 (27.10.2025)
        // (v): $2.6975 → $1.1172 (06.02.2026)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleB_PA_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[1].SubWaves[0], 5, "Cycle(B) [A] Intermediate");

        [TestMethod] public void CycleB_PA_i1_Label() => AssertLabel(_analysis.Waves[1].SubWaves[0].SubWaves[0], WaveNumber.One, "Cycle(B) [A] (i)");
        [TestMethod] public void CycleB_PA_i1_Start() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[0].StartPoint, Utc(2025, 7, 18, 1, 0), 3.6607m, "Cycle(B) [A] (i) Start");
        [TestMethod] public void CycleB_PA_i1_End() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[0].EndPoint, Utc(2025, 8, 3, 0, 0), 2.728m, "Cycle(B) [A] (i) End");

        [TestMethod] public void CycleB_PA_i2_Label() => AssertLabel(_analysis.Waves[1].SubWaves[0].SubWaves[1], WaveNumber.Two, "Cycle(B) [A] (ii)");
        [TestMethod] public void CycleB_PA_i2_Start() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[1].StartPoint, Utc(2025, 8, 3, 0, 0), 2.728m, "Cycle(B) [A] (ii) Start");
        [TestMethod] public void CycleB_PA_i2_End() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[1].EndPoint, Utc(2025, 8, 8, 4, 0), 3.3825m, "Cycle(B) [A] (ii) End");

        [TestMethod] public void CycleB_PA_i3_Label() => AssertLabel(_analysis.Waves[1].SubWaves[0].SubWaves[2], WaveNumber.Three, "Cycle(B) [A] (iii)");
        [TestMethod] public void CycleB_PA_i3_Start() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[2].StartPoint, Utc(2025, 8, 8, 4, 0), 3.3825m, "Cycle(B) [A] (iii) Start");
        [TestMethod] public void CycleB_PA_i3_End() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[2].EndPoint, Utc(2025, 10, 10, 21, 0), 1.2543m, "Cycle(B) [A] (iii) End");

        [TestMethod] public void CycleB_PA_i4_Label() => AssertLabel(_analysis.Waves[1].SubWaves[0].SubWaves[3], WaveNumber.Four, "Cycle(B) [A] (iv)");
        [TestMethod] public void CycleB_PA_i4_Start() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[3].StartPoint, Utc(2025, 10, 10, 21, 0), 1.2543m, "Cycle(B) [A] (iv) Start");
        [TestMethod] public void CycleB_PA_i4_End() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[3].EndPoint, Utc(2025, 10, 27, 18, 0), 2.6975m, "Cycle(B) [A] (iv) End");

        [TestMethod] public void CycleB_PA_i5_Label() => AssertLabel(_analysis.Waves[1].SubWaves[0].SubWaves[4], WaveNumber.Five, "Cycle(B) [A] (v)");
        [TestMethod] public void CycleB_PA_i5_Start() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[4].StartPoint, Utc(2025, 10, 27, 18, 0), 2.6975m, "Cycle(B) [A] (v) Start");
        [TestMethod] public void CycleB_PA_i5_End() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[4].EndPoint, Utc(2026, 2, 6, 0, 0), 1.1172m, "Cycle(B) [A] (v) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(B) [B]
        //
        // (a): $1.1172 → $1.5442 (06.02.2026)
        // (b): $1.5442 → $1.3421 (11.02.2026)
        // (c): $1.3421 → $1.6714 (15.02.2026)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleB_PB_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[1].SubWaves[1], 3, "Cycle(B) [B] Intermediate");

        [TestMethod] public void CycleB_PB_iA_Label() => AssertLabel(_analysis.Waves[1].SubWaves[1].SubWaves[0], WaveNumber.A, "Cycle(B) [B] (a)");
        [TestMethod] public void CycleB_PB_iA_Start() => AssertPoint(_analysis.Waves[1].SubWaves[1].SubWaves[0].StartPoint, Utc(2026, 2, 6, 0, 0), 1.1172m, "Cycle(B) [B] (a) Start");
        [TestMethod] public void CycleB_PB_iA_End() => AssertPoint(_analysis.Waves[1].SubWaves[1].SubWaves[0].EndPoint, Utc(2026, 2, 6, 15, 0), 1.5442m, "Cycle(B) [B] (a) End");

        [TestMethod] public void CycleB_PB_iB_Label() => AssertLabel(_analysis.Waves[1].SubWaves[1].SubWaves[1], WaveNumber.B, "Cycle(B) [B] (b)");
        [TestMethod] public void CycleB_PB_iB_Start() => AssertPoint(_analysis.Waves[1].SubWaves[1].SubWaves[1].StartPoint, Utc(2026, 2, 6, 15, 0), 1.5442m, "Cycle(B) [B] (b) Start");
        [TestMethod] public void CycleB_PB_iB_End() => AssertPoint(_analysis.Waves[1].SubWaves[1].SubWaves[1].EndPoint, Utc(2026, 2, 11, 17, 0), 1.3421m, "Cycle(B) [B] (b) End");

        [TestMethod] public void CycleB_PB_iC_Label() => AssertLabel(_analysis.Waves[1].SubWaves[1].SubWaves[2], WaveNumber.C, "Cycle(B) [B] (c)");
        [TestMethod] public void CycleB_PB_iC_Start() => AssertPoint(_analysis.Waves[1].SubWaves[1].SubWaves[2].StartPoint, Utc(2026, 2, 11, 17, 0), 1.3421m, "Cycle(B) [B] (c) Start");
        [TestMethod] public void CycleB_PB_iC_End() => AssertPoint(_analysis.Waves[1].SubWaves[1].SubWaves[2].EndPoint, Utc(2026, 2, 15, 8, 0), 1.6714m, "Cycle(B) [B] (c) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(B) [C]
        //
        // (i): $1.6714 → $1.2700 (28.02.2026)
        // (ii): $1.2700 → $1.6070 (17.03.2026)
        // (iii): $1.6070 → $1.6070 (17.03.2026) (in-progress)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleB_PC_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[1].SubWaves[2], 3, "Cycle(B) [C] Intermediate");

        [TestMethod] public void CycleB_PC_i1_Label() => AssertLabel(_analysis.Waves[1].SubWaves[2].SubWaves[0], WaveNumber.One, "Cycle(B) [C] (i)");
        [TestMethod] public void CycleB_PC_i1_Start() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[0].StartPoint, Utc(2026, 2, 15, 8, 0), 1.6714m, "Cycle(B) [C] (i) Start");
        [TestMethod] public void CycleB_PC_i1_End() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[0].EndPoint, Utc(2026, 2, 28, 10, 0), 1.27m, "Cycle(B) [C] (i) End");

        [TestMethod] public void CycleB_PC_i2_Label() => AssertLabel(_analysis.Waves[1].SubWaves[2].SubWaves[1], WaveNumber.Two, "Cycle(B) [C] (ii)");
        [TestMethod] public void CycleB_PC_i2_Start() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[1].StartPoint, Utc(2026, 2, 28, 10, 0), 1.27m, "Cycle(B) [C] (ii) Start");
        [TestMethod] public void CycleB_PC_i2_End() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[1].EndPoint, Utc(2026, 3, 17, 2, 0), 1.607m, "Cycle(B) [C] (ii) End");

        [TestMethod] public void CycleB_PC_i3_Label() => AssertLabel(_analysis.Waves[1].SubWaves[2].SubWaves[2], WaveNumber.Three, "Cycle(B) [C] (iii)");
        [TestMethod] public void CycleB_PC_i3_InProgress() => AssertInProgress(_analysis.Waves[1].SubWaves[2].SubWaves[2], true, "Cycle(B) [C] (iii)");
        [TestMethod] public void CycleB_PC_i3_Start() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[2].StartPoint, Utc(2026, 3, 17, 2, 0), 1.607m, "Cycle(B) [C] (iii) Start");

        // ══════════════════════════════════════════════════
        // Projections — Cycle (B): B retracement of Cycle A ($0.10 → $3.66)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleB_Projection_0786() => AssertProjectionTarget(_analysis.Waves[1], LogRetracement(0.10129m, 3.6607m, 0.786m), 0.786m, "Cycle(B) 0.786 projection");
        [TestMethod] public void CycleB_Projection_0618() => AssertProjectionTarget(_analysis.Waves[1], LogRetracement(0.10129m, 3.6607m, 0.618m), 0.618m, "Cycle(B) 0.618 projection");
        [TestMethod] public void CycleB_Projection_050() => AssertProjectionTarget(_analysis.Waves[1], LogRetracement(0.10129m, 3.6607m, 0.5m), 0.5m, "Cycle(B) 0.5 projection");
        [TestMethod] public void CycleB_Projection_0382() => AssertProjectionTarget(_analysis.Waves[1], LogRetracement(0.10129m, 3.6607m, 0.382m), 0.382m, "Cycle(B) 0.382 projection");

        // ══════════════════════════════════════════════════
        // Projections — Primary [C] inside Cycle (B): C extension of A ($3.66 → $1.12) from B end ($1.67)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleB_PC_Projection_2618() => AssertProjectionTarget(_analysis.Waves[1].SubWaves[2], LogExtension(3.6607m, 1.1172m, 1.67m, 2.618m), 2.618m, "Cycle(B) [C] 2.618 projection");
        [TestMethod] public void CycleB_PC_Projection_1618() => AssertProjectionTarget(_analysis.Waves[1].SubWaves[2], LogExtension(3.6607m, 1.1172m, 1.67m, 1.618m), 1.618m, "Cycle(B) [C] 1.618 projection");
        [TestMethod] public void CycleB_PC_Projection_AEqualsC() => AssertProjectionTarget(_analysis.Waves[1].SubWaves[2], LogExtension(3.6607m, 1.1172m, 1.67m, 1.0m), 1.0m, "Cycle(B) [C] A=C projection");
        [TestMethod] public void CycleB_PC_Projection_0618() => AssertProjectionTarget(_analysis.Waves[1].SubWaves[2], LogExtension(3.6607m, 1.1172m, 1.67m, 0.618m), 0.618m, "Cycle(B) [C] 0.618 projection");
    }
}
