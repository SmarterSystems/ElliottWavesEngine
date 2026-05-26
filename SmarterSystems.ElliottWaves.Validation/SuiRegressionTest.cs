using SmarterSystems.ElliottWaves.Analyzer.Interfaces;

namespace SmarterSystems.ElliottWaves.Validation
{
    [TestClass]
    public sealed class SuiRegressionTest : RegressionTestBase
    {
        private static ElliottWavesAnalysis _analysis = null!;

        [ClassInitialize]
        public static void Setup(TestContext _)
        {
            _analysis = LoadAndAnalyze("SUIUSDT");
        }

        // ══════════════════════════════════════════════════
        // Cycle Degree: 3 waves — CORRECTIVE A-B-C structure
        //
        // Cycle (A): 03.05.2023 $0.10 → 06.01.2025 $5.3687  (corrective A-B-C primary)
        // Cycle (B): 06.01.2025 $5.3687 → 10.10.2025 $0.5597  (corrective A-B-C primary)
        // Cycle (C): 10.10.2025 $0.5597 → in-progress           (A=C target)
        // ══════════════════════════════════════════════════

        [TestMethod] public void Cycle_WaveCount() => AssertWaveCount(_analysis, 3, "SUI Cycle");

        // ── Cycle (A): $0.10 → $5.3687 ──

        [TestMethod] public void CycleA_Label() => AssertLabel(_analysis.Waves[0], WaveNumber.A, "Cycle(A)");
        [TestMethod] public void CycleA_NotInProgress() => AssertInProgress(_analysis.Waves[0], false, "Cycle(A)");
        [TestMethod] public void CycleA_Start() => AssertPoint(_analysis.Waves[0].StartPoint, Utc(2023, 5, 3, 12), 0.10m, "Cycle(A) Start");
        [TestMethod] public void CycleA_End() => AssertPoint(_analysis.Waves[0].EndPoint, Utc(2025, 1, 6, 16), 5.3687m, "Cycle(A) End");

        // ── Cycle (B): $5.3687 → $0.5597 ──

        [TestMethod] public void CycleB_Label() => AssertLabel(_analysis.Waves[1], WaveNumber.B, "Cycle(B)");
        [TestMethod] public void CycleB_NotInProgress() => AssertInProgress(_analysis.Waves[1], false, "Cycle(B)");
        [TestMethod] public void CycleB_Start() => AssertPoint(_analysis.Waves[1].StartPoint, Utc(2025, 1, 6, 16), 5.3687m, "Cycle(B) Start");
        [TestMethod] public void CycleB_End() => AssertPoint(_analysis.Waves[1].EndPoint, Utc(2025, 10, 10, 21), 0.5597m, "Cycle(B) End");

        // ── Cycle (C): $0.5597 → in-progress ──

        [TestMethod] public void CycleC_Label() => AssertLabel(_analysis.Waves[2], WaveNumber.C, "Cycle(C)");
        [TestMethod] public void CycleC_InProgress() => AssertInProgress(_analysis.Waves[2], true, "Cycle(C)");
        [TestMethod] public void CycleC_Start() => AssertPoint(_analysis.Waves[2].StartPoint, Utc(2025, 10, 10, 21), 0.5597m, "Cycle(C) Start");

        // ══════════════════════════════════════════════════
        // Primary Degree — inside Cycle (A): A-B-C (corrective)
        //
        // [A]: $0.10 → $2.00 (03.05.2023) — same day as start
        // [B]: $2.00 → $0.4625 (05.08.2024)
        // [C]: $0.4625 → $5.3687 (06.01.2025)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleA_Primary_WaveCount() => AssertSubWaveCount(_analysis.Waves[0], 3, "Cycle(A) Primary");

        [TestMethod] public void CycleA_PA_Label() => AssertLabel(_analysis.Waves[0].SubWaves[0], WaveNumber.A, "Cycle(A) [A]");
        [TestMethod] public void CycleA_PA_Start() => AssertPoint(_analysis.Waves[0].SubWaves[0].StartPoint, Utc(2023, 5, 3, 12), 0.10m, "Cycle(A) [A] Start");
        [TestMethod] public void CycleA_PA_End() => AssertPoint(_analysis.Waves[0].SubWaves[0].EndPoint, Utc(2024, 3, 27, 17), 2.1829m, "Cycle(A) [A] End");

        [TestMethod] public void CycleA_PB_Label() => AssertLabel(_analysis.Waves[0].SubWaves[1], WaveNumber.B, "Cycle(A) [B]");
        [TestMethod] public void CycleA_PB_Start() => AssertPoint(_analysis.Waves[0].SubWaves[1].StartPoint, Utc(2024, 3, 27, 17), 2.1829m, "Cycle(A) [B] Start");
        [TestMethod] public void CycleA_PB_End() => AssertPoint(_analysis.Waves[0].SubWaves[1].EndPoint, Utc(2024, 8, 5, 13), 0.4625m, "Cycle(A) [B] End");

        [TestMethod] public void CycleA_PC_Label() => AssertLabel(_analysis.Waves[0].SubWaves[2], WaveNumber.C, "Cycle(A) [C]");
        [TestMethod] public void CycleA_PC_Start() => AssertPoint(_analysis.Waves[0].SubWaves[2].StartPoint, Utc(2024, 8, 5, 13), 0.4625m, "Cycle(A) [C] Start");
        [TestMethod] public void CycleA_PC_End() => AssertPoint(_analysis.Waves[0].SubWaves[2].EndPoint, Utc(2025, 1, 6, 16), 5.3687m, "Cycle(A) [C] End");

        // ══════════════════════════════════════════════════
        // Primary Degree — inside Cycle (B): A-B-C (corrective)
        //
        // [A]: $5.3687 → $1.7174 (07.04.2025)
        // [B]: $1.7174 → $4.4436 (28.07.2025)
        // [C]: $4.4436 → $0.5597 (10.10.2025)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleB_Primary_WaveCount() => AssertSubWaveCount(_analysis.Waves[1], 3, "Cycle(B) Primary");

        [TestMethod] public void CycleB_A_Label() => AssertLabel(_analysis.Waves[1].SubWaves[0], WaveNumber.A, "Cycle(B) [A]");
        [TestMethod] public void CycleB_A_Start() => AssertPoint(_analysis.Waves[1].SubWaves[0].StartPoint, Utc(2025, 1, 6, 16), 5.3687m, "Cycle(B) [A] Start");
        [TestMethod] public void CycleB_A_End() => AssertPoint(_analysis.Waves[1].SubWaves[0].EndPoint, Utc(2025, 4, 7, 6), 1.7174m, "Cycle(B) [A] End");

        [TestMethod] public void CycleB_B_Label() => AssertLabel(_analysis.Waves[1].SubWaves[1], WaveNumber.B, "Cycle(B) [B]");
        [TestMethod] public void CycleB_B_Start() => AssertPoint(_analysis.Waves[1].SubWaves[1].StartPoint, Utc(2025, 4, 7, 6), 1.7174m, "Cycle(B) [B] Start");
        [TestMethod] public void CycleB_B_End() => AssertPoint(_analysis.Waves[1].SubWaves[1].EndPoint, Utc(2025, 7, 28, 0), 4.4436m, "Cycle(B) [B] End");

        [TestMethod] public void CycleB_C_Label() => AssertLabel(_analysis.Waves[1].SubWaves[2], WaveNumber.C, "Cycle(B) [C]");
        [TestMethod] public void CycleB_C_Start() => AssertPoint(_analysis.Waves[1].SubWaves[2].StartPoint, Utc(2025, 7, 28, 0), 4.4436m, "Cycle(B) [C] Start");
        [TestMethod] public void CycleB_C_End() => AssertPoint(_analysis.Waves[1].SubWaves[2].EndPoint, Utc(2025, 10, 10, 21), 0.5597m, "Cycle(B) [C] End");

        // ══════════════════════════════════════════════════
        // Primary Degree — inside Cycle (C): in-progress
        //
        // [A]: $0.5597 → $3.0141 (13.10.2025)
        // [B]: $3.0141 → $0.7881 (06.02.2026)
        // [C]: in-progress
        // ══════════════════════════════════════════════════

        [TestMethod]
        public void CycleC_HasSubWaves()
        {
            Assert.IsNotNull(_analysis.Waves[2].SubWaves, "Cycle(C) should have sub-waves");
            Assert.IsTrue(_analysis.Waves[2].SubWaves.Count >= 2, "Cycle(C) should have at least 2 primary waves");
        }

        [TestMethod] public void CycleC_PA_Label() => AssertLabel(_analysis.Waves[2].SubWaves[0], WaveNumber.A, "Cycle(C) [A]");
        [TestMethod] public void CycleC_PA_Start() => AssertPoint(_analysis.Waves[2].SubWaves[0].StartPoint, Utc(2025, 10, 10, 21), 0.5597m, "Cycle(C) [A] Start");
        [TestMethod] public void CycleC_PA_End() => AssertPoint(_analysis.Waves[2].SubWaves[0].EndPoint, Utc(2025, 10, 13, 23), 3.0141m, "Cycle(C) [A] End");

        [TestMethod] public void CycleC_PB_Label() => AssertLabel(_analysis.Waves[2].SubWaves[1], WaveNumber.B, "Cycle(C) [B]");
        [TestMethod] public void CycleC_PB_Start() => AssertPoint(_analysis.Waves[2].SubWaves[1].StartPoint, Utc(2025, 10, 13, 23), 3.0141m, "Cycle(C) [B] Start");
        [TestMethod] public void CycleC_PB_End() => AssertPoint(_analysis.Waves[2].SubWaves[1].EndPoint, Utc(2026, 2, 6, 0), 0.7881m, "Cycle(C) [B] End");

        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(A) [A]
        //
        // (i): $0.10000 → $1.2829 (03.05.2023)
        // (ii): $1.2829 → $0.36200 (19.10.2023)
        // (iii): $0.36200 → $1.9740 (14.02.2024)
        // (iv): $1.9740 → $1.2120 (05.03.2024)
        // (v): $1.2120 → $2.1829 (27.03.2024)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleA_PA_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[0].SubWaves[0], 5, "Cycle(A) [A] Intermediate");

        [TestMethod] public void CycleA_PA_i1_Label() => AssertLabel(_analysis.Waves[0].SubWaves[0].SubWaves[0], WaveNumber.One, "Cycle(A) [A] (i)");
        [TestMethod] public void CycleA_PA_i1_Start() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[0].StartPoint, Utc(2023, 5, 3, 12, 0), 0.1m, "Cycle(A) [A] (i) Start");
        [TestMethod] public void CycleA_PA_i1_End() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[0].EndPoint, Utc(2023, 5, 3, 12, 0), 1.2829m, "Cycle(A) [A] (i) End");

        [TestMethod] public void CycleA_PA_i2_Label() => AssertLabel(_analysis.Waves[0].SubWaves[0].SubWaves[1], WaveNumber.Two, "Cycle(A) [A] (ii)");
        [TestMethod] public void CycleA_PA_i2_Start() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[1].StartPoint, Utc(2023, 5, 3, 12, 0), 1.2829m, "Cycle(A) [A] (ii) Start");
        [TestMethod] public void CycleA_PA_i2_End() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[1].EndPoint, Utc(2023, 10, 19, 10, 0), 0.362m, "Cycle(A) [A] (ii) End");

        [TestMethod] public void CycleA_PA_i3_Label() => AssertLabel(_analysis.Waves[0].SubWaves[0].SubWaves[2], WaveNumber.Three, "Cycle(A) [A] (iii)");
        [TestMethod] public void CycleA_PA_i3_Start() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[2].StartPoint, Utc(2023, 10, 19, 10, 0), 0.362m, "Cycle(A) [A] (iii) Start");
        [TestMethod] public void CycleA_PA_i3_End() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[2].EndPoint, Utc(2024, 2, 14, 12, 0), 1.974m, "Cycle(A) [A] (iii) End");

        [TestMethod] public void CycleA_PA_i4_Label() => AssertLabel(_analysis.Waves[0].SubWaves[0].SubWaves[3], WaveNumber.Four, "Cycle(A) [A] (iv)");
        [TestMethod] public void CycleA_PA_i4_Start() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[3].StartPoint, Utc(2024, 2, 14, 12, 0), 1.974m, "Cycle(A) [A] (iv) Start");
        [TestMethod] public void CycleA_PA_i4_End() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[3].EndPoint, Utc(2024, 3, 5, 19, 0), 1.212m, "Cycle(A) [A] (iv) End");

        [TestMethod] public void CycleA_PA_i5_Label() => AssertLabel(_analysis.Waves[0].SubWaves[0].SubWaves[4], WaveNumber.Five, "Cycle(A) [A] (v)");
        [TestMethod] public void CycleA_PA_i5_Start() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[4].StartPoint, Utc(2024, 3, 5, 19, 0), 1.212m, "Cycle(A) [A] (v) Start");
        [TestMethod] public void CycleA_PA_i5_End() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[4].EndPoint, Utc(2024, 3, 27, 17, 0), 2.1829m, "Cycle(A) [A] (v) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(A) [B]
        //
        // (a): $2.1829 → $0.88560 (13.04.2024)
        // (b): $0.88560 → $1.4448 (21.04.2024)
        // (c): $1.4448 → $0.46250 (05.08.2024)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleA_PB_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[0].SubWaves[1], 3, "Cycle(A) [B] Intermediate");

        [TestMethod] public void CycleA_PB_iA_Label() => AssertLabel(_analysis.Waves[0].SubWaves[1].SubWaves[0], WaveNumber.A, "Cycle(A) [B] (a)");
        [TestMethod] public void CycleA_PB_iA_Start() => AssertPoint(_analysis.Waves[0].SubWaves[1].SubWaves[0].StartPoint, Utc(2024, 3, 27, 17, 0), 2.1829m, "Cycle(A) [B] (a) Start");
        [TestMethod] public void CycleA_PB_iA_End() => AssertPoint(_analysis.Waves[0].SubWaves[1].SubWaves[0].EndPoint, Utc(2024, 4, 13, 20, 0), 0.8856m, "Cycle(A) [B] (a) End");

        [TestMethod] public void CycleA_PB_iB_Label() => AssertLabel(_analysis.Waves[0].SubWaves[1].SubWaves[1], WaveNumber.B, "Cycle(A) [B] (b)");
        [TestMethod] public void CycleA_PB_iB_Start() => AssertPoint(_analysis.Waves[0].SubWaves[1].SubWaves[1].StartPoint, Utc(2024, 4, 13, 20, 0), 0.8856m, "Cycle(A) [B] (b) Start");
        [TestMethod] public void CycleA_PB_iB_End() => AssertPoint(_analysis.Waves[0].SubWaves[1].SubWaves[1].EndPoint, Utc(2024, 4, 21, 1, 0), 1.4448m, "Cycle(A) [B] (b) End");

        [TestMethod] public void CycleA_PB_iC_Label() => AssertLabel(_analysis.Waves[0].SubWaves[1].SubWaves[2], WaveNumber.C, "Cycle(A) [B] (c)");
        [TestMethod] public void CycleA_PB_iC_Start() => AssertPoint(_analysis.Waves[0].SubWaves[1].SubWaves[2].StartPoint, Utc(2024, 4, 21, 1, 0), 1.4448m, "Cycle(A) [B] (c) Start");
        [TestMethod] public void CycleA_PB_iC_End() => AssertPoint(_analysis.Waves[0].SubWaves[1].SubWaves[2].EndPoint, Utc(2024, 8, 5, 13, 0), 0.4625m, "Cycle(A) [B] (c) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(A) [C]
        //
        // (i): $0.46250 → $1.1174 (12.08.2024)
        // (ii): $1.1174 → $0.74150 (02.09.2024)
        // (iii): $0.74150 → $2.3680 (13.10.2024)
        // (iv): $2.3680 → $1.5983 (28.10.2024)
        // (v): $1.5983 → $5.3687 (06.01.2025)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleA_PC_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[0].SubWaves[2], 5, "Cycle(A) [C] Intermediate");

        [TestMethod] public void CycleA_PC_i1_Label() => AssertLabel(_analysis.Waves[0].SubWaves[2].SubWaves[0], WaveNumber.One, "Cycle(A) [C] (i)");
        [TestMethod] public void CycleA_PC_i1_Start() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[0].StartPoint, Utc(2024, 8, 5, 13, 0), 0.4625m, "Cycle(A) [C] (i) Start");
        [TestMethod] public void CycleA_PC_i1_End() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[0].EndPoint, Utc(2024, 8, 12, 15, 0), 1.1174m, "Cycle(A) [C] (i) End");

        [TestMethod] public void CycleA_PC_i2_Label() => AssertLabel(_analysis.Waves[0].SubWaves[2].SubWaves[1], WaveNumber.Two, "Cycle(A) [C] (ii)");
        [TestMethod] public void CycleA_PC_i2_Start() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[1].StartPoint, Utc(2024, 8, 12, 15, 0), 1.1174m, "Cycle(A) [C] (ii) Start");
        [TestMethod] public void CycleA_PC_i2_End() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[1].EndPoint, Utc(2024, 9, 2, 7, 0), 0.7415m, "Cycle(A) [C] (ii) End");

        [TestMethod] public void CycleA_PC_i3_Label() => AssertLabel(_analysis.Waves[0].SubWaves[2].SubWaves[2], WaveNumber.Three, "Cycle(A) [C] (iii)");
        [TestMethod] public void CycleA_PC_i3_Start() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[2].StartPoint, Utc(2024, 9, 2, 7, 0), 0.7415m, "Cycle(A) [C] (iii) Start");
        [TestMethod] public void CycleA_PC_i3_End() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[2].EndPoint, Utc(2024, 10, 13, 23, 0), 2.368m, "Cycle(A) [C] (iii) End");

        [TestMethod] public void CycleA_PC_i4_Label() => AssertLabel(_analysis.Waves[0].SubWaves[2].SubWaves[3], WaveNumber.Four, "Cycle(A) [C] (iv)");
        [TestMethod] public void CycleA_PC_i4_Start() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[3].StartPoint, Utc(2024, 10, 13, 23, 0), 2.368m, "Cycle(A) [C] (iv) Start");
        [TestMethod] public void CycleA_PC_i4_End() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[3].EndPoint, Utc(2024, 10, 28, 17, 0), 1.5983m, "Cycle(A) [C] (iv) End");

        [TestMethod] public void CycleA_PC_i5_Label() => AssertLabel(_analysis.Waves[0].SubWaves[2].SubWaves[4], WaveNumber.Five, "Cycle(A) [C] (v)");
        [TestMethod] public void CycleA_PC_i5_Start() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[4].StartPoint, Utc(2024, 10, 28, 17, 0), 1.5983m, "Cycle(A) [C] (v) Start");
        [TestMethod] public void CycleA_PC_i5_End() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[4].EndPoint, Utc(2025, 1, 6, 16, 0), 5.3687m, "Cycle(A) [C] (v) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(B) [A]
        //
        // (i): $5.3687 → $4.2530 (13.01.2025)
        // (ii): $4.2530 → $5.1396 (18.01.2025)
        // (iii): $5.1396 → $1.9626 (11.03.2025)
        // (iv): $1.9626 → $2.8309 (27.03.2025)
        // (v): $2.8309 → $1.7174 (07.04.2025)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleB_PA_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[1].SubWaves[0], 5, "Cycle(B) [A] Intermediate");

        [TestMethod] public void CycleB_PA_i1_Label() => AssertLabel(_analysis.Waves[1].SubWaves[0].SubWaves[0], WaveNumber.One, "Cycle(B) [A] (i)");
        [TestMethod] public void CycleB_PA_i1_Start() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[0].StartPoint, Utc(2025, 1, 6, 16, 0), 5.3687m, "Cycle(B) [A] (i) Start");
        [TestMethod] public void CycleB_PA_i1_End() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[0].EndPoint, Utc(2025, 1, 13, 14, 0), 4.253m, "Cycle(B) [A] (i) End");

        [TestMethod] public void CycleB_PA_i2_Label() => AssertLabel(_analysis.Waves[1].SubWaves[0].SubWaves[1], WaveNumber.Two, "Cycle(B) [A] (ii)");
        [TestMethod] public void CycleB_PA_i2_Start() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[1].StartPoint, Utc(2025, 1, 13, 14, 0), 4.253m, "Cycle(B) [A] (ii) Start");
        [TestMethod] public void CycleB_PA_i2_End() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[1].EndPoint, Utc(2025, 1, 18, 20, 0), 5.1396m, "Cycle(B) [A] (ii) End");

        [TestMethod] public void CycleB_PA_i3_Label() => AssertLabel(_analysis.Waves[1].SubWaves[0].SubWaves[2], WaveNumber.Three, "Cycle(B) [A] (iii)");
        [TestMethod] public void CycleB_PA_i3_Start() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[2].StartPoint, Utc(2025, 1, 18, 20, 0), 5.1396m, "Cycle(B) [A] (iii) Start");
        [TestMethod] public void CycleB_PA_i3_End() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[2].EndPoint, Utc(2025, 3, 11, 0, 0), 1.9626m, "Cycle(B) [A] (iii) End");

        [TestMethod] public void CycleB_PA_i4_Label() => AssertLabel(_analysis.Waves[1].SubWaves[0].SubWaves[3], WaveNumber.Four, "Cycle(B) [A] (iv)");
        [TestMethod] public void CycleB_PA_i4_Start() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[3].StartPoint, Utc(2025, 3, 11, 0, 0), 1.9626m, "Cycle(B) [A] (iv) Start");
        [TestMethod] public void CycleB_PA_i4_End() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[3].EndPoint, Utc(2025, 3, 27, 22, 0), 2.8309m, "Cycle(B) [A] (iv) End");

        [TestMethod] public void CycleB_PA_i5_Label() => AssertLabel(_analysis.Waves[1].SubWaves[0].SubWaves[4], WaveNumber.Five, "Cycle(B) [A] (v)");
        [TestMethod] public void CycleB_PA_i5_Start() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[4].StartPoint, Utc(2025, 3, 27, 22, 0), 2.8309m, "Cycle(B) [A] (v) Start");
        [TestMethod] public void CycleB_PA_i5_End() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[4].EndPoint, Utc(2025, 4, 7, 6, 0), 1.7174m, "Cycle(B) [A] (v) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(B) [B]
        //
        // (a): $1.7174 → $4.2989 (12.05.2025)
        // (b): $4.2989 → $2.2937 (22.06.2025)
        // (c): $2.2937 → $4.4436 (28.07.2025)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleB_PB_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[1].SubWaves[1], 3, "Cycle(B) [B] Intermediate");

        [TestMethod] public void CycleB_PB_iA_Label() => AssertLabel(_analysis.Waves[1].SubWaves[1].SubWaves[0], WaveNumber.A, "Cycle(B) [B] (a)");
        [TestMethod] public void CycleB_PB_iA_Start() => AssertPoint(_analysis.Waves[1].SubWaves[1].SubWaves[0].StartPoint, Utc(2025, 4, 7, 6, 0), 1.7174m, "Cycle(B) [B] (a) Start");
        [TestMethod] public void CycleB_PB_iA_End() => AssertPoint(_analysis.Waves[1].SubWaves[1].SubWaves[0].EndPoint, Utc(2025, 5, 12, 7, 0), 4.2989m, "Cycle(B) [B] (a) End");

        [TestMethod] public void CycleB_PB_iB_Label() => AssertLabel(_analysis.Waves[1].SubWaves[1].SubWaves[1], WaveNumber.B, "Cycle(B) [B] (b)");
        [TestMethod] public void CycleB_PB_iB_Start() => AssertPoint(_analysis.Waves[1].SubWaves[1].SubWaves[1].StartPoint, Utc(2025, 5, 12, 7, 0), 4.2989m, "Cycle(B) [B] (b) Start");
        [TestMethod] public void CycleB_PB_iB_End() => AssertPoint(_analysis.Waves[1].SubWaves[1].SubWaves[1].EndPoint, Utc(2025, 6, 22, 20, 0), 2.2937m, "Cycle(B) [B] (b) End");

        [TestMethod] public void CycleB_PB_iC_Label() => AssertLabel(_analysis.Waves[1].SubWaves[1].SubWaves[2], WaveNumber.C, "Cycle(B) [B] (c)");
        [TestMethod] public void CycleB_PB_iC_Start() => AssertPoint(_analysis.Waves[1].SubWaves[1].SubWaves[2].StartPoint, Utc(2025, 6, 22, 20, 0), 2.2937m, "Cycle(B) [B] (c) Start");
        [TestMethod] public void CycleB_PB_iC_End() => AssertPoint(_analysis.Waves[1].SubWaves[1].SubWaves[2].EndPoint, Utc(2025, 7, 28, 0, 0), 4.4436m, "Cycle(B) [B] (c) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(B) [C]
        //
        // (i): $4.4436 → $3.2663 (02.08.2025)
        // (ii): $3.2663 → $4.1811 (14.08.2025)
        // (iii): $4.1811 → $3.1101 (01.09.2025)
        // (iv): $3.1101 → $3.9765 (18.09.2025)
        // (v): $3.9765 → $0.55970 (10.10.2025)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleB_PC_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[1].SubWaves[2], 5, "Cycle(B) [C] Intermediate");

        [TestMethod] public void CycleB_PC_i1_Label() => AssertLabel(_analysis.Waves[1].SubWaves[2].SubWaves[0], WaveNumber.One, "Cycle(B) [C] (i)");
        [TestMethod] public void CycleB_PC_i1_Start() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[0].StartPoint, Utc(2025, 7, 28, 0, 0), 4.4436m, "Cycle(B) [C] (i) Start");
        [TestMethod] public void CycleB_PC_i1_End() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[0].EndPoint, Utc(2025, 8, 2, 18, 0), 3.2663m, "Cycle(B) [C] (i) End");

        [TestMethod] public void CycleB_PC_i2_Label() => AssertLabel(_analysis.Waves[1].SubWaves[2].SubWaves[1], WaveNumber.Two, "Cycle(B) [C] (ii)");
        [TestMethod] public void CycleB_PC_i2_Start() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[1].StartPoint, Utc(2025, 8, 2, 18, 0), 3.2663m, "Cycle(B) [C] (ii) Start");
        [TestMethod] public void CycleB_PC_i2_End() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[1].EndPoint, Utc(2025, 8, 14, 3, 0), 4.1811m, "Cycle(B) [C] (ii) End");

        [TestMethod] public void CycleB_PC_i3_Label() => AssertLabel(_analysis.Waves[1].SubWaves[2].SubWaves[2], WaveNumber.Three, "Cycle(B) [C] (iii)");
        [TestMethod] public void CycleB_PC_i3_Start() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[2].StartPoint, Utc(2025, 8, 14, 3, 0), 4.1811m, "Cycle(B) [C] (iii) Start");
        [TestMethod] public void CycleB_PC_i3_End() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[2].EndPoint, Utc(2025, 9, 1, 21, 0), 3.1101m, "Cycle(B) [C] (iii) End");

        [TestMethod] public void CycleB_PC_i4_Label() => AssertLabel(_analysis.Waves[1].SubWaves[2].SubWaves[3], WaveNumber.Four, "Cycle(B) [C] (iv)");
        [TestMethod] public void CycleB_PC_i4_Start() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[3].StartPoint, Utc(2025, 9, 1, 21, 0), 3.1101m, "Cycle(B) [C] (iv) Start");
        [TestMethod] public void CycleB_PC_i4_End() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[3].EndPoint, Utc(2025, 9, 18, 18, 0), 3.9765m, "Cycle(B) [C] (iv) End");

        [TestMethod] public void CycleB_PC_i5_Label() => AssertLabel(_analysis.Waves[1].SubWaves[2].SubWaves[4], WaveNumber.Five, "Cycle(B) [C] (v)");
        [TestMethod] public void CycleB_PC_i5_Start() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[4].StartPoint, Utc(2025, 9, 18, 18, 0), 3.9765m, "Cycle(B) [C] (v) Start");
        [TestMethod] public void CycleB_PC_i5_End() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[4].EndPoint, Utc(2025, 10, 10, 21, 0), 0.5597m, "Cycle(B) [C] (v) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(C) [A]
        //
        // (i): $0.55970 → $2.5214 (10.10.2025)
        // (ii): $2.5214 → $2.1272 (10.10.2025)
        // (iii): $2.1272 → $2.9221 (13.10.2025)
        // (iv): $2.9221 → $2.8095 (13.10.2025)
        // (v): $2.8095 → $3.0141 (13.10.2025)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleC_PA_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[2].SubWaves[0], 5, "Cycle(C) [A] Intermediate");

        [TestMethod] public void CycleC_PA_i1_Label() => AssertLabel(_analysis.Waves[2].SubWaves[0].SubWaves[0], WaveNumber.One, "Cycle(C) [A] (i)");
        [TestMethod] public void CycleC_PA_i1_Start() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[0].StartPoint, Utc(2025, 10, 10, 21, 0), 0.5597m, "Cycle(C) [A] (i) Start");
        [TestMethod] public void CycleC_PA_i1_End() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[0].EndPoint, Utc(2025, 10, 10, 21, 0), 2.5214m, "Cycle(C) [A] (i) End");

        [TestMethod] public void CycleC_PA_i2_Label() => AssertLabel(_analysis.Waves[2].SubWaves[0].SubWaves[1], WaveNumber.Two, "Cycle(C) [A] (ii)");
        [TestMethod] public void CycleC_PA_i2_Start() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[1].StartPoint, Utc(2025, 10, 10, 21, 0), 2.5214m, "Cycle(C) [A] (ii) Start");
        [TestMethod] public void CycleC_PA_i2_End() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[1].EndPoint, Utc(2025, 10, 10, 22, 0), 2.1272m, "Cycle(C) [A] (ii) End");

        [TestMethod] public void CycleC_PA_i3_Label() => AssertLabel(_analysis.Waves[2].SubWaves[0].SubWaves[2], WaveNumber.Three, "Cycle(C) [A] (iii)");
        [TestMethod] public void CycleC_PA_i3_Start() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[2].StartPoint, Utc(2025, 10, 10, 22, 0), 2.1272m, "Cycle(C) [A] (iii) Start");
        [TestMethod] public void CycleC_PA_i3_End() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[2].EndPoint, Utc(2025, 10, 13, 13, 0), 2.9221m, "Cycle(C) [A] (iii) End");

        [TestMethod] public void CycleC_PA_i4_Label() => AssertLabel(_analysis.Waves[2].SubWaves[0].SubWaves[3], WaveNumber.Four, "Cycle(C) [A] (iv)");
        [TestMethod] public void CycleC_PA_i4_Start() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[3].StartPoint, Utc(2025, 10, 13, 13, 0), 2.9221m, "Cycle(C) [A] (iv) Start");
        [TestMethod] public void CycleC_PA_i4_End() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[3].EndPoint, Utc(2025, 10, 13, 14, 0), 2.8095m, "Cycle(C) [A] (iv) End");

        [TestMethod] public void CycleC_PA_i5_Label() => AssertLabel(_analysis.Waves[2].SubWaves[0].SubWaves[4], WaveNumber.Five, "Cycle(C) [A] (v)");
        [TestMethod] public void CycleC_PA_i5_Start() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[4].StartPoint, Utc(2025, 10, 13, 14, 0), 2.8095m, "Cycle(C) [A] (v) Start");
        [TestMethod] public void CycleC_PA_i5_End() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[4].EndPoint, Utc(2025, 10, 13, 23, 0), 3.0141m, "Cycle(C) [A] (v) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(C) [B]
        //
        // (a): $3.0141 → $1.3039 (01.12.2025)
        // (b): $1.3039 → $2.0244 (06.01.2026)
        // (c): $2.0244 → $0.78810 (06.02.2026)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleC_PB_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[2].SubWaves[1], 3, "Cycle(C) [B] Intermediate");

        [TestMethod] public void CycleC_PB_iA_Label() => AssertLabel(_analysis.Waves[2].SubWaves[1].SubWaves[0], WaveNumber.A, "Cycle(C) [B] (a)");
        [TestMethod] public void CycleC_PB_iA_Start() => AssertPoint(_analysis.Waves[2].SubWaves[1].SubWaves[0].StartPoint, Utc(2025, 10, 13, 23, 0), 3.0141m, "Cycle(C) [B] (a) Start");
        [TestMethod] public void CycleC_PB_iA_End() => AssertPoint(_analysis.Waves[2].SubWaves[1].SubWaves[0].EndPoint, Utc(2025, 12, 1, 15, 0), 1.3039m, "Cycle(C) [B] (a) End");

        [TestMethod] public void CycleC_PB_iB_Label() => AssertLabel(_analysis.Waves[2].SubWaves[1].SubWaves[1], WaveNumber.B, "Cycle(C) [B] (b)");
        [TestMethod] public void CycleC_PB_iB_Start() => AssertPoint(_analysis.Waves[2].SubWaves[1].SubWaves[1].StartPoint, Utc(2025, 12, 1, 15, 0), 1.3039m, "Cycle(C) [B] (b) Start");
        [TestMethod] public void CycleC_PB_iB_End() => AssertPoint(_analysis.Waves[2].SubWaves[1].SubWaves[1].EndPoint, Utc(2026, 1, 6, 12, 0), 2.0244m, "Cycle(C) [B] (b) End");

        [TestMethod] public void CycleC_PB_iC_Label() => AssertLabel(_analysis.Waves[2].SubWaves[1].SubWaves[2], WaveNumber.C, "Cycle(C) [B] (c)");
        [TestMethod] public void CycleC_PB_iC_Start() => AssertPoint(_analysis.Waves[2].SubWaves[1].SubWaves[2].StartPoint, Utc(2026, 1, 6, 12, 0), 2.0244m, "Cycle(C) [B] (c) Start");
        [TestMethod] public void CycleC_PB_iC_End() => AssertPoint(_analysis.Waves[2].SubWaves[1].SubWaves[2].EndPoint, Utc(2026, 2, 6, 0, 0), 0.7881m, "Cycle(C) [B] (c) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(C) [C]
        //
        // (i): $0.78810 → $1.0846 (16.03.2026)
        // (ii): $1.0846 → $0.93740 (19.03.2026)
        // (iii): $0.93740 → $0.93740 (19.03.2026) (in-progress)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleC_PC_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[2].SubWaves[2], 3, "Cycle(C) [C] Intermediate");

        [TestMethod] public void CycleC_PC_i1_Label() => AssertLabel(_analysis.Waves[2].SubWaves[2].SubWaves[0], WaveNumber.One, "Cycle(C) [C] (i)");
        [TestMethod] public void CycleC_PC_i1_Start() => AssertPoint(_analysis.Waves[2].SubWaves[2].SubWaves[0].StartPoint, Utc(2026, 2, 6, 0, 0), 0.7881m, "Cycle(C) [C] (i) Start");
        [TestMethod] public void CycleC_PC_i1_End() => AssertPoint(_analysis.Waves[2].SubWaves[2].SubWaves[0].EndPoint, Utc(2026, 3, 16, 22, 0), 1.0846m, "Cycle(C) [C] (i) End");

        [TestMethod] public void CycleC_PC_i2_Label() => AssertLabel(_analysis.Waves[2].SubWaves[2].SubWaves[1], WaveNumber.Two, "Cycle(C) [C] (ii)");
        [TestMethod] public void CycleC_PC_i2_Start() => AssertPoint(_analysis.Waves[2].SubWaves[2].SubWaves[1].StartPoint, Utc(2026, 3, 16, 22, 0), 1.0846m, "Cycle(C) [C] (ii) Start");
        [TestMethod] public void CycleC_PC_i2_End() => AssertPoint(_analysis.Waves[2].SubWaves[2].SubWaves[1].EndPoint, Utc(2026, 3, 19, 15, 0), 0.9374m, "Cycle(C) [C] (ii) End");

        [TestMethod] public void CycleC_PC_i3_Label() => AssertLabel(_analysis.Waves[2].SubWaves[2].SubWaves[2], WaveNumber.Three, "Cycle(C) [C] (iii)");
        [TestMethod] public void CycleC_PC_i3_InProgress() => AssertInProgress(_analysis.Waves[2].SubWaves[2].SubWaves[2], true, "Cycle(C) [C] (iii)");
        [TestMethod] public void CycleC_PC_i3_Start() => AssertPoint(_analysis.Waves[2].SubWaves[2].SubWaves[2].StartPoint, Utc(2026, 3, 19, 15, 0), 0.9374m, "Cycle(C) [C] (iii) Start");

        // ══════════════════════════════════════════════════
        // Projections — Cycle (C): C extension of A ($0.10 → $5.37) from B end ($0.56)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleC_Projection_4236() => AssertProjectionTarget(_analysis.Waves[2], LogExtension(0.10m, 5.3687m, 0.5597m, 4.236m), 4.236m, "Cycle(C) 4.236 projection");
        [TestMethod] public void CycleC_Projection_2618() => AssertProjectionTarget(_analysis.Waves[2], LogExtension(0.10m, 5.3687m, 0.5597m, 2.618m), 2.618m, "Cycle(C) 2.618 projection");
        [TestMethod] public void CycleC_Projection_1618() => AssertProjectionTarget(_analysis.Waves[2], LogExtension(0.10m, 5.3687m, 0.5597m, 1.618m), 1.618m, "Cycle(C) 1.618 projection");
        [TestMethod] public void CycleC_Projection_AEqualsC() => AssertProjectionTarget(_analysis.Waves[2], LogExtension(0.10m, 5.3687m, 0.5597m, 1.0m), 1.0m, "Cycle(C) A=C projection");

        // ══════════════════════════════════════════════════
        // Projections — Primary [C] inside Cycle (C): C extension of A ($0.56 → $3.01) from B end ($0.79)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleC_PC_Projection_4236() => AssertProjectionTarget(_analysis.Waves[2].SubWaves[2], LogExtension(0.5597m, 3.0141m, 0.7881m, 4.236m), 4.236m, "Cycle(C) [C] 4.236 projection");
        [TestMethod] public void CycleC_PC_Projection_2618() => AssertProjectionTarget(_analysis.Waves[2].SubWaves[2], LogExtension(0.5597m, 3.0141m, 0.7881m, 2.618m), 2.618m, "Cycle(C) [C] 2.618 projection");
        [TestMethod] public void CycleC_PC_Projection_1618() => AssertProjectionTarget(_analysis.Waves[2].SubWaves[2], LogExtension(0.5597m, 3.0141m, 0.7881m, 1.618m), 1.618m, "Cycle(C) [C] 1.618 projection");
        [TestMethod] public void CycleC_PC_Projection_AEqualsC() => AssertProjectionTarget(_analysis.Waves[2].SubWaves[2], LogExtension(0.5597m, 3.0141m, 0.7881m, 1.0m), 1.0m, "Cycle(C) [C] A=C projection");
    }
}
