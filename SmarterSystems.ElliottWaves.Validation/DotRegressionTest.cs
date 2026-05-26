using SmarterSystems.ElliottWaves.Analyzer.Interfaces;

namespace SmarterSystems.ElliottWaves.Validation
{
    [TestClass]
    public sealed class DotRegressionTest : RegressionTestBase
    {
        private static ElliottWavesAnalysis _analysis = null!;

        [ClassInitialize]
        public static void Setup(TestContext _)
        {
            _analysis = LoadAndAnalyze("DOTUSDT");
        }

        // ══════════════════════════════════════════════════
        // Cycle Degree: 3 waves — CORRECTIVE A-B-C structure
        //
        // Cycle (A): 18.08.2020 $2.00 → 04.11.2021 $55.09  (impulse 5-wave)
        // Cycle (B): 04.11.2021 $55.09 → 10.10.2025 $0.633  (corrective A-B-C)
        // Cycle (C): 10.10.2025 $0.633 → in-progress         (impulse, A=C target)
        // ══════════════════════════════════════════════════

        [TestMethod] public void Cycle_WaveCount() => AssertWaveCount(_analysis, 3, "DOT Cycle");

        // ── Cycle (A): $2.00 → $55.09 ──

        [TestMethod] public void CycleA_Label() => AssertLabel(_analysis.Waves[0], WaveNumber.A, "Cycle(A)");
        [TestMethod] public void CycleA_NotInProgress() => AssertInProgress(_analysis.Waves[0], false, "Cycle(A)");
        [TestMethod] public void CycleA_Start() => AssertPoint(_analysis.Waves[0].StartPoint, Utc(2020, 8, 18, 23), 2.00m, "Cycle(A) Start");
        [TestMethod] public void CycleA_End() => AssertPoint(_analysis.Waves[0].EndPoint, Utc(2021, 11, 4, 13), 55.09m, "Cycle(A) End");

        // ── Cycle (B): $55.09 → $0.633 ──

        [TestMethod] public void CycleB_Label() => AssertLabel(_analysis.Waves[1], WaveNumber.B, "Cycle(B)");
        [TestMethod] public void CycleB_NotInProgress() => AssertInProgress(_analysis.Waves[1], false, "Cycle(B)");
        [TestMethod] public void CycleB_Start() => AssertPoint(_analysis.Waves[1].StartPoint, Utc(2021, 11, 4, 13), 55.09m, "Cycle(B) Start");
        [TestMethod] public void CycleB_End() => AssertPoint(_analysis.Waves[1].EndPoint, Utc(2025, 10, 10, 21), 0.633m, "Cycle(B) End");

        // ── Cycle (C): $0.633 → in-progress ──

        [TestMethod] public void CycleC_Label() => AssertLabel(_analysis.Waves[2], WaveNumber.C, "Cycle(C)");
        [TestMethod] public void CycleC_InProgress() => AssertInProgress(_analysis.Waves[2], true, "Cycle(C)");
        [TestMethod] public void CycleC_Start() => AssertPoint(_analysis.Waves[2].StartPoint, Utc(2025, 10, 10, 21), 0.633m, "Cycle(C) Start");

        // ══════════════════════════════════════════════════
        // Primary Degree — inside Cycle (A): 5 waves (impulse)
        //
        // [1]: $2.00 → $6.80 (27.08.2020)
        // [2]: $6.80 → $3.5321 (05.09.2020)
        // [3]: $3.5321 → $49.78 (15.05.2021)
        // [4]: $49.78 → $10.673 (21.07.2021)
        // [5]: $10.673 → $55.09 (04.11.2021)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleA_Primary_WaveCount() => AssertSubWaveCount(_analysis.Waves[0], 5, "Cycle(A) Primary");

        [TestMethod] public void CycleA_P1_Label() => AssertLabel(_analysis.Waves[0].SubWaves[0], WaveNumber.One, "Cycle(A) [1]");
        [TestMethod] public void CycleA_P1_Start() => AssertPoint(_analysis.Waves[0].SubWaves[0].StartPoint, Utc(2020, 8, 18, 23), 2.00m, "Cycle(A) [1] Start");
        [TestMethod] public void CycleA_P1_End() => AssertPoint(_analysis.Waves[0].SubWaves[0].EndPoint, Utc(2020, 9, 1, 6), 6.8619m, "Cycle(A) [1] End");

        [TestMethod] public void CycleA_P2_Label() => AssertLabel(_analysis.Waves[0].SubWaves[1], WaveNumber.Two, "Cycle(A) [2]");
        [TestMethod] public void CycleA_P2_Start() => AssertPoint(_analysis.Waves[0].SubWaves[1].StartPoint, Utc(2020, 9, 1, 6), 6.8619m, "Cycle(A) [2] Start");
        [TestMethod] public void CycleA_P2_End() => AssertPoint(_analysis.Waves[0].SubWaves[1].EndPoint, Utc(2020, 9, 5, 18), 3.5321m, "Cycle(A) [2] End");

        [TestMethod] public void CycleA_P3_Label() => AssertLabel(_analysis.Waves[0].SubWaves[2], WaveNumber.Three, "Cycle(A) [3]");
        [TestMethod] public void CycleA_P3_Start() => AssertPoint(_analysis.Waves[0].SubWaves[2].StartPoint, Utc(2020, 9, 5, 18), 3.5321m, "Cycle(A) [3] Start");
        [TestMethod] public void CycleA_P3_End() => AssertPoint(_analysis.Waves[0].SubWaves[2].EndPoint, Utc(2021, 5, 15, 3), 49.78m, "Cycle(A) [3] End");

        [TestMethod] public void CycleA_P4_Label() => AssertLabel(_analysis.Waves[0].SubWaves[3], WaveNumber.Four, "Cycle(A) [4]");
        [TestMethod] public void CycleA_P4_Start() => AssertPoint(_analysis.Waves[0].SubWaves[3].StartPoint, Utc(2021, 5, 15, 3), 49.78m, "Cycle(A) [4] Start");
        [TestMethod] public void CycleA_P4_End() => AssertPoint(_analysis.Waves[0].SubWaves[3].EndPoint, Utc(2021, 7, 20, 10), 10.373m, "Cycle(A) [4] End");

        [TestMethod] public void CycleA_P5_Label() => AssertLabel(_analysis.Waves[0].SubWaves[4], WaveNumber.Five, "Cycle(A) [5]");
        [TestMethod] public void CycleA_P5_Start() => AssertPoint(_analysis.Waves[0].SubWaves[4].StartPoint, Utc(2021, 7, 20, 10), 10.373m, "Cycle(A) [5] Start");
        [TestMethod] public void CycleA_P5_End() => AssertPoint(_analysis.Waves[0].SubWaves[4].EndPoint, Utc(2021, 11, 4, 13), 55.09m, "Cycle(A) [5] End");

        // ══════════════════════════════════════════════════
        // Primary Degree — inside Cycle (B): A-B-C (corrective)
        //
        // [A]: $55.09 → $3.562 (19.10.2023)
        // [B]: $3.562 → $11.649 (04.12.2024)
        // [C]: $11.649 → $0.633 (10.10.2025)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleB_Primary_WaveCount() => AssertSubWaveCount(_analysis.Waves[1], 3, "Cycle(B) Primary");

        [TestMethod] public void CycleB_A_Label() => AssertLabel(_analysis.Waves[1].SubWaves[0], WaveNumber.A, "Cycle(B) [A]");
        [TestMethod] public void CycleB_A_Start() => AssertPoint(_analysis.Waves[1].SubWaves[0].StartPoint, Utc(2021, 11, 4, 13), 55.09m, "Cycle(B) [A] Start");
        [TestMethod] public void CycleB_A_End() => AssertPoint(_analysis.Waves[1].SubWaves[0].EndPoint, Utc(2023, 10, 19, 1), 3.562m, "Cycle(B) [A] End");

        [TestMethod] public void CycleB_B_Label() => AssertLabel(_analysis.Waves[1].SubWaves[1], WaveNumber.B, "Cycle(B) [B]");
        [TestMethod] public void CycleB_B_Start() => AssertPoint(_analysis.Waves[1].SubWaves[1].StartPoint, Utc(2023, 10, 19, 1), 3.562m, "Cycle(B) [B] Start");
        [TestMethod] public void CycleB_B_End() => AssertPoint(_analysis.Waves[1].SubWaves[1].EndPoint, Utc(2024, 12, 4, 13), 11.649m, "Cycle(B) [B] End");

        [TestMethod] public void CycleB_C_Label() => AssertLabel(_analysis.Waves[1].SubWaves[2], WaveNumber.C, "Cycle(B) [C]");
        [TestMethod] public void CycleB_C_Start() => AssertPoint(_analysis.Waves[1].SubWaves[2].StartPoint, Utc(2024, 12, 4, 13), 11.649m, "Cycle(B) [C] Start");
        [TestMethod] public void CycleB_C_End() => AssertPoint(_analysis.Waves[1].SubWaves[2].EndPoint, Utc(2025, 10, 10, 21), 0.633m, "Cycle(B) [C] End");

        // ══════════════════════════════════════════════════
        // Primary Degree — inside Cycle (C): in-progress
        //
        // [1]: $0.633 → $3.533 (08.11.2025)
        // [2]: $3.533 → $1.101 (06.02.2026)
        // [3]: in-progress
        // ══════════════════════════════════════════════════

        [TestMethod]
        public void CycleC_HasSubWaves()
        {
            Assert.IsNotNull(_analysis.Waves[2].SubWaves, "Cycle(C) should have sub-waves");
            Assert.IsTrue(_analysis.Waves[2].SubWaves.Count >= 2, "Cycle(C) should have at least 2 primary waves");
        }

        [TestMethod] public void CycleC_P1_Label() => AssertLabel(_analysis.Waves[2].SubWaves[0], WaveNumber.One, "Cycle(C) [1]");
        [TestMethod] public void CycleC_P1_Start() => AssertPoint(_analysis.Waves[2].SubWaves[0].StartPoint, Utc(2025, 10, 10, 21), 0.633m, "Cycle(C) [1] Start");
        [TestMethod] public void CycleC_P1_End() => AssertPoint(_analysis.Waves[2].SubWaves[0].EndPoint, Utc(2025, 11, 8, 2), 3.533m, "Cycle(C) [1] End");

        [TestMethod] public void CycleC_P2_Label() => AssertLabel(_analysis.Waves[2].SubWaves[1], WaveNumber.Two, "Cycle(C) [2]");
        [TestMethod] public void CycleC_P2_Start() => AssertPoint(_analysis.Waves[2].SubWaves[1].StartPoint, Utc(2025, 11, 8, 2), 3.533m, "Cycle(C) [2] Start");
        [TestMethod] public void CycleC_P2_End() => AssertPoint(_analysis.Waves[2].SubWaves[1].EndPoint, Utc(2026, 2, 6, 0), 1.101m, "Cycle(C) [2] End");

        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(A) [1]
        //
        // (i): $2.0000 → $4.4400 (18.08.2020)
        // (ii): $4.4400 → $2.6000 (20.08.2020)
        // (iii): $2.6000 → $6.8000 (27.08.2020)
        // (iv): $6.8000 → $5.2001 (27.08.2020)
        // (v): $5.2001 → $6.8619 (01.09.2020)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleA_P1_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[0].SubWaves[0], 5, "Cycle(A) [1] Intermediate");

        [TestMethod] public void CycleA_P1_i1_Label() => AssertLabel(_analysis.Waves[0].SubWaves[0].SubWaves[0], WaveNumber.One, "Cycle(A) [1] (i)");
        [TestMethod] public void CycleA_P1_i1_Start() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[0].StartPoint, Utc(2020, 8, 18, 23, 0), 2m, "Cycle(A) [1] (i) Start");
        [TestMethod] public void CycleA_P1_i1_End() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[0].EndPoint, Utc(2020, 8, 18, 23, 0), 4.44m, "Cycle(A) [1] (i) End");

        [TestMethod] public void CycleA_P1_i2_Label() => AssertLabel(_analysis.Waves[0].SubWaves[0].SubWaves[1], WaveNumber.Two, "Cycle(A) [1] (ii)");
        [TestMethod] public void CycleA_P1_i2_Start() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[1].StartPoint, Utc(2020, 8, 18, 23, 0), 4.44m, "Cycle(A) [1] (ii) Start");
        [TestMethod] public void CycleA_P1_i2_End() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[1].EndPoint, Utc(2020, 8, 20, 22, 0), 2.6m, "Cycle(A) [1] (ii) End");

        [TestMethod] public void CycleA_P1_i3_Label() => AssertLabel(_analysis.Waves[0].SubWaves[0].SubWaves[2], WaveNumber.Three, "Cycle(A) [1] (iii)");
        [TestMethod] public void CycleA_P1_i3_Start() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[2].StartPoint, Utc(2020, 8, 20, 22, 0), 2.6m, "Cycle(A) [1] (iii) Start");
        [TestMethod] public void CycleA_P1_i3_End() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[2].EndPoint, Utc(2020, 8, 27, 7, 0), 6.8m, "Cycle(A) [1] (iii) End");

        [TestMethod] public void CycleA_P1_i4_Label() => AssertLabel(_analysis.Waves[0].SubWaves[0].SubWaves[3], WaveNumber.Four, "Cycle(A) [1] (iv)");
        [TestMethod] public void CycleA_P1_i4_Start() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[3].StartPoint, Utc(2020, 8, 27, 7, 0), 6.8m, "Cycle(A) [1] (iv) Start");
        [TestMethod] public void CycleA_P1_i4_End() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[3].EndPoint, Utc(2020, 8, 27, 17, 0), 5.2001m, "Cycle(A) [1] (iv) End");

        [TestMethod] public void CycleA_P1_i5_Label() => AssertLabel(_analysis.Waves[0].SubWaves[0].SubWaves[4], WaveNumber.Five, "Cycle(A) [1] (v)");
        [TestMethod] public void CycleA_P1_i5_Start() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[4].StartPoint, Utc(2020, 8, 27, 17, 0), 5.2001m, "Cycle(A) [1] (v) Start");
        [TestMethod] public void CycleA_P1_i5_End() => AssertPoint(_analysis.Waves[0].SubWaves[0].SubWaves[4].EndPoint, Utc(2020, 9, 1, 6, 0), 6.8619m, "Cycle(A) [1] (v) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(A) [2]
        //
        // (a): $6.8619 → $4.5000 (04.09.2020)
        // (b): $4.5000 → $5.4500 (04.09.2020)
        // (c): $5.4500 → $3.5321 (05.09.2020)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleA_P2_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[0].SubWaves[1], 3, "Cycle(A) [2] Intermediate");

        [TestMethod] public void CycleA_P2_iA_Label() => AssertLabel(_analysis.Waves[0].SubWaves[1].SubWaves[0], WaveNumber.A, "Cycle(A) [2] (a)");
        [TestMethod] public void CycleA_P2_iA_Start() => AssertPoint(_analysis.Waves[0].SubWaves[1].SubWaves[0].StartPoint, Utc(2020, 9, 1, 6, 0), 6.8619m, "Cycle(A) [2] (a) Start");
        [TestMethod] public void CycleA_P2_iA_End() => AssertPoint(_analysis.Waves[0].SubWaves[1].SubWaves[0].EndPoint, Utc(2020, 9, 4, 14, 0), 4.5m, "Cycle(A) [2] (a) End");

        [TestMethod] public void CycleA_P2_iB_Label() => AssertLabel(_analysis.Waves[0].SubWaves[1].SubWaves[1], WaveNumber.B, "Cycle(A) [2] (b)");
        [TestMethod] public void CycleA_P2_iB_Start() => AssertPoint(_analysis.Waves[0].SubWaves[1].SubWaves[1].StartPoint, Utc(2020, 9, 4, 14, 0), 4.5m, "Cycle(A) [2] (b) Start");
        [TestMethod] public void CycleA_P2_iB_End() => AssertPoint(_analysis.Waves[0].SubWaves[1].SubWaves[1].EndPoint, Utc(2020, 9, 4, 21, 0), 5.45m, "Cycle(A) [2] (b) End");

        [TestMethod] public void CycleA_P2_iC_Label() => AssertLabel(_analysis.Waves[0].SubWaves[1].SubWaves[2], WaveNumber.C, "Cycle(A) [2] (c)");
        [TestMethod] public void CycleA_P2_iC_Start() => AssertPoint(_analysis.Waves[0].SubWaves[1].SubWaves[2].StartPoint, Utc(2020, 9, 4, 21, 0), 5.45m, "Cycle(A) [2] (c) Start");
        [TestMethod] public void CycleA_P2_iC_End() => AssertPoint(_analysis.Waves[0].SubWaves[1].SubWaves[2].EndPoint, Utc(2020, 9, 5, 18, 0), 3.5321m, "Cycle(A) [2] (c) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(A) [3]
        //
        // (i): $3.5321 → $10.6800 (07.01.2021)
        // (ii): $10.6800 → $7.1642 (11.01.2021)
        // (iii): $7.1642 → $48.3600 (17.04.2021)
        // (iv): $48.3600 → $26.6355 (23.04.2021)
        // (v): $26.6355 → $49.7800 (15.05.2021)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleA_P3_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[0].SubWaves[2], 5, "Cycle(A) [3] Intermediate");

        [TestMethod] public void CycleA_P3_i1_Label() => AssertLabel(_analysis.Waves[0].SubWaves[2].SubWaves[0], WaveNumber.One, "Cycle(A) [3] (i)");
        [TestMethod] public void CycleA_P3_i1_Start() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[0].StartPoint, Utc(2020, 9, 5, 18, 0), 3.5321m, "Cycle(A) [3] (i) Start");
        [TestMethod] public void CycleA_P3_i1_End() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[0].EndPoint, Utc(2021, 1, 7, 1, 0), 10.68m, "Cycle(A) [3] (i) End");

        [TestMethod] public void CycleA_P3_i2_Label() => AssertLabel(_analysis.Waves[0].SubWaves[2].SubWaves[1], WaveNumber.Two, "Cycle(A) [3] (ii)");
        [TestMethod] public void CycleA_P3_i2_Start() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[1].StartPoint, Utc(2021, 1, 7, 1, 0), 10.68m, "Cycle(A) [3] (ii) Start");
        [TestMethod] public void CycleA_P3_i2_End() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[1].EndPoint, Utc(2021, 1, 11, 3, 0), 7.1642m, "Cycle(A) [3] (ii) End");

        [TestMethod] public void CycleA_P3_i3_Label() => AssertLabel(_analysis.Waves[0].SubWaves[2].SubWaves[2], WaveNumber.Three, "Cycle(A) [3] (iii)");
        [TestMethod] public void CycleA_P3_i3_Start() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[2].StartPoint, Utc(2021, 1, 11, 3, 0), 7.1642m, "Cycle(A) [3] (iii) Start");
        [TestMethod] public void CycleA_P3_i3_End() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[2].EndPoint, Utc(2021, 4, 17, 9, 0), 48.36m, "Cycle(A) [3] (iii) End");

        [TestMethod] public void CycleA_P3_i4_Label() => AssertLabel(_analysis.Waves[0].SubWaves[2].SubWaves[3], WaveNumber.Four, "Cycle(A) [3] (iv)");
        [TestMethod] public void CycleA_P3_i4_Start() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[3].StartPoint, Utc(2021, 4, 17, 9, 0), 48.36m, "Cycle(A) [3] (iv) Start");
        [TestMethod] public void CycleA_P3_i4_End() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[3].EndPoint, Utc(2021, 4, 23, 2, 0), 26.6355m, "Cycle(A) [3] (iv) End");

        [TestMethod] public void CycleA_P3_i5_Label() => AssertLabel(_analysis.Waves[0].SubWaves[2].SubWaves[4], WaveNumber.Five, "Cycle(A) [3] (v)");
        [TestMethod] public void CycleA_P3_i5_Start() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[4].StartPoint, Utc(2021, 4, 23, 2, 0), 26.6355m, "Cycle(A) [3] (v) Start");
        [TestMethod] public void CycleA_P3_i5_End() => AssertPoint(_analysis.Waves[0].SubWaves[2].SubWaves[4].EndPoint, Utc(2021, 5, 15, 3, 0), 49.78m, "Cycle(A) [3] (v) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(A) [4]
        //
        // (a): $49.7800 → $13.6380 (23.05.2021)
        // (b): $13.6380 → $28.6000 (03.06.2021)
        // (c): $28.6000 → $10.3730 (20.07.2021)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleA_P4_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[0].SubWaves[3], 3, "Cycle(A) [4] Intermediate");

        [TestMethod] public void CycleA_P4_iA_Label() => AssertLabel(_analysis.Waves[0].SubWaves[3].SubWaves[0], WaveNumber.A, "Cycle(A) [4] (a)");
        [TestMethod] public void CycleA_P4_iA_Start() => AssertPoint(_analysis.Waves[0].SubWaves[3].SubWaves[0].StartPoint, Utc(2021, 5, 15, 3, 0), 49.78m, "Cycle(A) [4] (a) Start");
        [TestMethod] public void CycleA_P4_iA_End() => AssertPoint(_analysis.Waves[0].SubWaves[3].SubWaves[0].EndPoint, Utc(2021, 5, 23, 16, 0), 13.638m, "Cycle(A) [4] (a) End");

        [TestMethod] public void CycleA_P4_iB_Label() => AssertLabel(_analysis.Waves[0].SubWaves[3].SubWaves[1], WaveNumber.B, "Cycle(A) [4] (b)");
        [TestMethod] public void CycleA_P4_iB_Start() => AssertPoint(_analysis.Waves[0].SubWaves[3].SubWaves[1].StartPoint, Utc(2021, 5, 23, 16, 0), 13.638m, "Cycle(A) [4] (b) Start");
        [TestMethod] public void CycleA_P4_iB_End() => AssertPoint(_analysis.Waves[0].SubWaves[3].SubWaves[1].EndPoint, Utc(2021, 6, 3, 9, 0), 28.6m, "Cycle(A) [4] (b) End");

        [TestMethod] public void CycleA_P4_iC_Label() => AssertLabel(_analysis.Waves[0].SubWaves[3].SubWaves[2], WaveNumber.C, "Cycle(A) [4] (c)");
        [TestMethod] public void CycleA_P4_iC_Start() => AssertPoint(_analysis.Waves[0].SubWaves[3].SubWaves[2].StartPoint, Utc(2021, 6, 3, 9, 0), 28.6m, "Cycle(A) [4] (c) Start");
        [TestMethod] public void CycleA_P4_iC_End() => AssertPoint(_analysis.Waves[0].SubWaves[3].SubWaves[2].EndPoint, Utc(2021, 7, 20, 10, 0), 10.373m, "Cycle(A) [4] (c) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(A) [5]
        //
        // (i): $10.3730 → $19.9000 (01.08.2021)
        // (ii): $19.9000 → $16.8110 (03.08.2021)
        // (iii): $16.8110 → $38.7700 (14.09.2021)
        // (iv): $38.7700 → $25.5000 (21.09.2021)
        // (v): $25.5000 → $55.0900 (04.11.2021)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleA_P5_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[0].SubWaves[4], 5, "Cycle(A) [5] Intermediate");

        [TestMethod] public void CycleA_P5_i1_Label() => AssertLabel(_analysis.Waves[0].SubWaves[4].SubWaves[0], WaveNumber.One, "Cycle(A) [5] (i)");
        [TestMethod] public void CycleA_P5_i1_Start() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[0].StartPoint, Utc(2021, 7, 20, 10, 0), 10.373m, "Cycle(A) [5] (i) Start");
        [TestMethod] public void CycleA_P5_i1_End() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[0].EndPoint, Utc(2021, 8, 1, 17, 0), 19.9m, "Cycle(A) [5] (i) End");

        [TestMethod] public void CycleA_P5_i2_Label() => AssertLabel(_analysis.Waves[0].SubWaves[4].SubWaves[1], WaveNumber.Two, "Cycle(A) [5] (ii)");
        [TestMethod] public void CycleA_P5_i2_Start() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[1].StartPoint, Utc(2021, 8, 1, 17, 0), 19.9m, "Cycle(A) [5] (ii) Start");
        [TestMethod] public void CycleA_P5_i2_End() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[1].EndPoint, Utc(2021, 8, 3, 12, 0), 16.811m, "Cycle(A) [5] (ii) End");

        [TestMethod] public void CycleA_P5_i3_Label() => AssertLabel(_analysis.Waves[0].SubWaves[4].SubWaves[2], WaveNumber.Three, "Cycle(A) [5] (iii)");
        [TestMethod] public void CycleA_P5_i3_Start() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[2].StartPoint, Utc(2021, 8, 3, 12, 0), 16.811m, "Cycle(A) [5] (iii) Start");
        [TestMethod] public void CycleA_P5_i3_End() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[2].EndPoint, Utc(2021, 9, 14, 13, 0), 38.77m, "Cycle(A) [5] (iii) End");

        [TestMethod] public void CycleA_P5_i4_Label() => AssertLabel(_analysis.Waves[0].SubWaves[4].SubWaves[3], WaveNumber.Four, "Cycle(A) [5] (iv)");
        [TestMethod] public void CycleA_P5_i4_Start() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[3].StartPoint, Utc(2021, 9, 14, 13, 0), 38.77m, "Cycle(A) [5] (iv) Start");
        [TestMethod] public void CycleA_P5_i4_End() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[3].EndPoint, Utc(2021, 9, 21, 21, 0), 25.5m, "Cycle(A) [5] (iv) End");

        [TestMethod] public void CycleA_P5_i5_Label() => AssertLabel(_analysis.Waves[0].SubWaves[4].SubWaves[4], WaveNumber.Five, "Cycle(A) [5] (v)");
        [TestMethod] public void CycleA_P5_i5_Start() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[4].StartPoint, Utc(2021, 9, 21, 21, 0), 25.5m, "Cycle(A) [5] (v) Start");
        [TestMethod] public void CycleA_P5_i5_End() => AssertPoint(_analysis.Waves[0].SubWaves[4].SubWaves[4].EndPoint, Utc(2021, 11, 4, 13, 0), 55.09m, "Cycle(A) [5] (v) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(B) [A]
        //
        // (i): $55.0900 → $32.2100 (28.11.2021)
        // (ii): $32.2100 → $39.3500 (30.11.2021)
        // (iii): $39.3500 → $4.2240 (29.12.2022)
        // (iv): $4.2240 → $7.9000 (19.02.2023)
        // (v): $7.9000 → $3.5620 (19.10.2023)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleB_PA_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[1].SubWaves[0], 5, "Cycle(B) [A] Intermediate");

        [TestMethod] public void CycleB_PA_i1_Label() => AssertLabel(_analysis.Waves[1].SubWaves[0].SubWaves[0], WaveNumber.One, "Cycle(B) [A] (i)");
        [TestMethod] public void CycleB_PA_i1_Start() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[0].StartPoint, Utc(2021, 11, 4, 13, 0), 55.09m, "Cycle(B) [A] (i) Start");
        [TestMethod] public void CycleB_PA_i1_End() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[0].EndPoint, Utc(2021, 11, 28, 18, 0), 32.21m, "Cycle(B) [A] (i) End");

        [TestMethod] public void CycleB_PA_i2_Label() => AssertLabel(_analysis.Waves[1].SubWaves[0].SubWaves[1], WaveNumber.Two, "Cycle(B) [A] (ii)");
        [TestMethod] public void CycleB_PA_i2_Start() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[1].StartPoint, Utc(2021, 11, 28, 18, 0), 32.21m, "Cycle(B) [A] (ii) Start");
        [TestMethod] public void CycleB_PA_i2_End() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[1].EndPoint, Utc(2021, 11, 30, 17, 0), 39.35m, "Cycle(B) [A] (ii) End");

        [TestMethod] public void CycleB_PA_i3_Label() => AssertLabel(_analysis.Waves[1].SubWaves[0].SubWaves[2], WaveNumber.Three, "Cycle(B) [A] (iii)");
        [TestMethod] public void CycleB_PA_i3_Start() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[2].StartPoint, Utc(2021, 11, 30, 17, 0), 39.35m, "Cycle(B) [A] (iii) Start");
        [TestMethod] public void CycleB_PA_i3_End() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[2].EndPoint, Utc(2022, 12, 29, 2, 0), 4.224m, "Cycle(B) [A] (iii) End");

        [TestMethod] public void CycleB_PA_i4_Label() => AssertLabel(_analysis.Waves[1].SubWaves[0].SubWaves[3], WaveNumber.Four, "Cycle(B) [A] (iv)");
        [TestMethod] public void CycleB_PA_i4_Start() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[3].StartPoint, Utc(2022, 12, 29, 2, 0), 4.224m, "Cycle(B) [A] (iv) Start");
        [TestMethod] public void CycleB_PA_i4_End() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[3].EndPoint, Utc(2023, 2, 19, 16, 0), 7.9m, "Cycle(B) [A] (iv) End");

        [TestMethod] public void CycleB_PA_i5_Label() => AssertLabel(_analysis.Waves[1].SubWaves[0].SubWaves[4], WaveNumber.Five, "Cycle(B) [A] (v)");
        [TestMethod] public void CycleB_PA_i5_Start() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[4].StartPoint, Utc(2023, 2, 19, 16, 0), 7.9m, "Cycle(B) [A] (v) Start");
        [TestMethod] public void CycleB_PA_i5_End() => AssertPoint(_analysis.Waves[1].SubWaves[0].SubWaves[4].EndPoint, Utc(2023, 10, 19, 1, 0), 3.562m, "Cycle(B) [A] (v) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(B) [B]
        //
        // (a): $3.5620 → $11.8890 (14.03.2024)
        // (b): $11.8890 → $3.5900 (05.08.2024)
        // (c): $3.5900 → $11.6490 (04.12.2024)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleB_PB_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[1].SubWaves[1], 3, "Cycle(B) [B] Intermediate");

        [TestMethod] public void CycleB_PB_iA_Label() => AssertLabel(_analysis.Waves[1].SubWaves[1].SubWaves[0], WaveNumber.A, "Cycle(B) [B] (a)");
        [TestMethod] public void CycleB_PB_iA_Start() => AssertPoint(_analysis.Waves[1].SubWaves[1].SubWaves[0].StartPoint, Utc(2023, 10, 19, 1, 0), 3.562m, "Cycle(B) [B] (a) Start");
        [TestMethod] public void CycleB_PB_iA_End() => AssertPoint(_analysis.Waves[1].SubWaves[1].SubWaves[0].EndPoint, Utc(2024, 3, 14, 10, 0), 11.889m, "Cycle(B) [B] (a) End");

        [TestMethod] public void CycleB_PB_iB_Label() => AssertLabel(_analysis.Waves[1].SubWaves[1].SubWaves[1], WaveNumber.B, "Cycle(B) [B] (b)");
        [TestMethod] public void CycleB_PB_iB_Start() => AssertPoint(_analysis.Waves[1].SubWaves[1].SubWaves[1].StartPoint, Utc(2024, 3, 14, 10, 0), 11.889m, "Cycle(B) [B] (b) Start");
        [TestMethod] public void CycleB_PB_iB_End() => AssertPoint(_analysis.Waves[1].SubWaves[1].SubWaves[1].EndPoint, Utc(2024, 8, 5, 6, 0), 3.59m, "Cycle(B) [B] (b) End");

        [TestMethod] public void CycleB_PB_iC_Label() => AssertLabel(_analysis.Waves[1].SubWaves[1].SubWaves[2], WaveNumber.C, "Cycle(B) [B] (c)");
        [TestMethod] public void CycleB_PB_iC_Start() => AssertPoint(_analysis.Waves[1].SubWaves[1].SubWaves[2].StartPoint, Utc(2024, 8, 5, 6, 0), 3.59m, "Cycle(B) [B] (c) Start");
        [TestMethod] public void CycleB_PB_iC_End() => AssertPoint(_analysis.Waves[1].SubWaves[1].SubWaves[2].EndPoint, Utc(2024, 12, 4, 13, 0), 11.649m, "Cycle(B) [B] (c) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(B) [C]
        //
        // (i): $11.6490 → $7.5000 (09.12.2024)
        // (ii): $7.5000 → $9.6690 (12.12.2024)
        // (iii): $9.6690 → $3.0070 (22.06.2025)
        // (iv): $3.0070 → $4.8820 (19.09.2025)
        // (v): $4.8820 → $0.63300 (10.10.2025)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleB_PC_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[1].SubWaves[2], 5, "Cycle(B) [C] Intermediate");

        [TestMethod] public void CycleB_PC_i1_Label() => AssertLabel(_analysis.Waves[1].SubWaves[2].SubWaves[0], WaveNumber.One, "Cycle(B) [C] (i)");
        [TestMethod] public void CycleB_PC_i1_Start() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[0].StartPoint, Utc(2024, 12, 4, 13, 0), 11.649m, "Cycle(B) [C] (i) Start");
        [TestMethod] public void CycleB_PC_i1_End() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[0].EndPoint, Utc(2024, 12, 9, 21, 0), 7.5m, "Cycle(B) [C] (i) End");

        [TestMethod] public void CycleB_PC_i2_Label() => AssertLabel(_analysis.Waves[1].SubWaves[2].SubWaves[1], WaveNumber.Two, "Cycle(B) [C] (ii)");
        [TestMethod] public void CycleB_PC_i2_Start() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[1].StartPoint, Utc(2024, 12, 9, 21, 0), 7.5m, "Cycle(B) [C] (ii) Start");
        [TestMethod] public void CycleB_PC_i2_End() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[1].EndPoint, Utc(2024, 12, 12, 5, 0), 9.669m, "Cycle(B) [C] (ii) End");

        [TestMethod] public void CycleB_PC_i3_Label() => AssertLabel(_analysis.Waves[1].SubWaves[2].SubWaves[2], WaveNumber.Three, "Cycle(B) [C] (iii)");
        [TestMethod] public void CycleB_PC_i3_Start() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[2].StartPoint, Utc(2024, 12, 12, 5, 0), 9.669m, "Cycle(B) [C] (iii) Start");
        [TestMethod] public void CycleB_PC_i3_End() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[2].EndPoint, Utc(2025, 6, 22, 20, 0), 3.007m, "Cycle(B) [C] (iii) End");

        [TestMethod] public void CycleB_PC_i4_Label() => AssertLabel(_analysis.Waves[1].SubWaves[2].SubWaves[3], WaveNumber.Four, "Cycle(B) [C] (iv)");
        [TestMethod] public void CycleB_PC_i4_Start() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[3].StartPoint, Utc(2025, 6, 22, 20, 0), 3.007m, "Cycle(B) [C] (iv) Start");
        [TestMethod] public void CycleB_PC_i4_End() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[3].EndPoint, Utc(2025, 9, 19, 1, 0), 4.882m, "Cycle(B) [C] (iv) End");

        [TestMethod] public void CycleB_PC_i5_Label() => AssertLabel(_analysis.Waves[1].SubWaves[2].SubWaves[4], WaveNumber.Five, "Cycle(B) [C] (v)");
        [TestMethod] public void CycleB_PC_i5_Start() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[4].StartPoint, Utc(2025, 9, 19, 1, 0), 4.882m, "Cycle(B) [C] (v) Start");
        [TestMethod] public void CycleB_PC_i5_End() => AssertPoint(_analysis.Waves[1].SubWaves[2].SubWaves[4].EndPoint, Utc(2025, 10, 10, 21, 0), 0.633m, "Cycle(B) [C] (v) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(C) [1]
        //
        // (i): $0.63300 → $2.8860 (10.10.2025)
        // (ii): $2.8860 → $2.3430 (04.11.2025)
        // (iii): $2.3430 → $3.3400 (07.11.2025)
        // (iv): $3.3400 → $3.1910 (08.11.2025)
        // (v): $3.1910 → $3.5330 (08.11.2025)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleC_P1_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[2].SubWaves[0], 5, "Cycle(C) [1] Intermediate");

        [TestMethod] public void CycleC_P1_i1_Label() => AssertLabel(_analysis.Waves[2].SubWaves[0].SubWaves[0], WaveNumber.One, "Cycle(C) [1] (i)");
        [TestMethod] public void CycleC_P1_i1_Start() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[0].StartPoint, Utc(2025, 10, 10, 21, 0), 0.633m, "Cycle(C) [1] (i) Start");
        [TestMethod] public void CycleC_P1_i1_End() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[0].EndPoint, Utc(2025, 10, 10, 21, 0), 2.886m, "Cycle(C) [1] (i) End");

        [TestMethod] public void CycleC_P1_i2_Label() => AssertLabel(_analysis.Waves[2].SubWaves[0].SubWaves[1], WaveNumber.Two, "Cycle(C) [1] (ii)");
        [TestMethod] public void CycleC_P1_i2_Start() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[1].StartPoint, Utc(2025, 10, 10, 21, 0), 2.886m, "Cycle(C) [1] (ii) Start");
        [TestMethod] public void CycleC_P1_i2_End() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[1].EndPoint, Utc(2025, 11, 4, 20, 0), 2.343m, "Cycle(C) [1] (ii) End");

        [TestMethod] public void CycleC_P1_i3_Label() => AssertLabel(_analysis.Waves[2].SubWaves[0].SubWaves[2], WaveNumber.Three, "Cycle(C) [1] (iii)");
        [TestMethod] public void CycleC_P1_i3_Start() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[2].StartPoint, Utc(2025, 11, 4, 20, 0), 2.343m, "Cycle(C) [1] (iii) Start");
        [TestMethod] public void CycleC_P1_i3_End() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[2].EndPoint, Utc(2025, 11, 7, 18, 0), 3.34m, "Cycle(C) [1] (iii) End");

        [TestMethod] public void CycleC_P1_i4_Label() => AssertLabel(_analysis.Waves[2].SubWaves[0].SubWaves[3], WaveNumber.Four, "Cycle(C) [1] (iv)");
        [TestMethod] public void CycleC_P1_i4_Start() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[3].StartPoint, Utc(2025, 11, 7, 18, 0), 3.34m, "Cycle(C) [1] (iv) Start");
        [TestMethod] public void CycleC_P1_i4_End() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[3].EndPoint, Utc(2025, 11, 8, 1, 0), 3.191m, "Cycle(C) [1] (iv) End");

        [TestMethod] public void CycleC_P1_i5_Label() => AssertLabel(_analysis.Waves[2].SubWaves[0].SubWaves[4], WaveNumber.Five, "Cycle(C) [1] (v)");
        [TestMethod] public void CycleC_P1_i5_Start() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[4].StartPoint, Utc(2025, 11, 8, 1, 0), 3.191m, "Cycle(C) [1] (v) Start");
        [TestMethod] public void CycleC_P1_i5_End() => AssertPoint(_analysis.Waves[2].SubWaves[0].SubWaves[4].EndPoint, Utc(2025, 11, 8, 2, 0), 3.533m, "Cycle(C) [1] (v) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(C) [2]
        //
        // (a): $3.5330 → $1.6530 (26.12.2025)
        // (b): $1.6530 → $2.3430 (13.01.2026)
        // (c): $2.3430 → $1.1010 (06.02.2026)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleC_P2_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[2].SubWaves[1], 3, "Cycle(C) [2] Intermediate");

        [TestMethod] public void CycleC_P2_iA_Label() => AssertLabel(_analysis.Waves[2].SubWaves[1].SubWaves[0], WaveNumber.A, "Cycle(C) [2] (a)");
        [TestMethod] public void CycleC_P2_iA_Start() => AssertPoint(_analysis.Waves[2].SubWaves[1].SubWaves[0].StartPoint, Utc(2025, 11, 8, 2, 0), 3.533m, "Cycle(C) [2] (a) Start");
        [TestMethod] public void CycleC_P2_iA_End() => AssertPoint(_analysis.Waves[2].SubWaves[1].SubWaves[0].EndPoint, Utc(2025, 12, 26, 0, 0), 1.653m, "Cycle(C) [2] (a) End");

        [TestMethod] public void CycleC_P2_iB_Label() => AssertLabel(_analysis.Waves[2].SubWaves[1].SubWaves[1], WaveNumber.B, "Cycle(C) [2] (b)");
        [TestMethod] public void CycleC_P2_iB_Start() => AssertPoint(_analysis.Waves[2].SubWaves[1].SubWaves[1].StartPoint, Utc(2025, 12, 26, 0, 0), 1.653m, "Cycle(C) [2] (b) Start");
        [TestMethod] public void CycleC_P2_iB_End() => AssertPoint(_analysis.Waves[2].SubWaves[1].SubWaves[1].EndPoint, Utc(2026, 1, 13, 16, 0), 2.343m, "Cycle(C) [2] (b) End");

        [TestMethod] public void CycleC_P2_iC_Label() => AssertLabel(_analysis.Waves[2].SubWaves[1].SubWaves[2], WaveNumber.C, "Cycle(C) [2] (c)");
        [TestMethod] public void CycleC_P2_iC_Start() => AssertPoint(_analysis.Waves[2].SubWaves[1].SubWaves[2].StartPoint, Utc(2026, 1, 13, 16, 0), 2.343m, "Cycle(C) [2] (c) Start");
        [TestMethod] public void CycleC_P2_iC_End() => AssertPoint(_analysis.Waves[2].SubWaves[1].SubWaves[2].EndPoint, Utc(2026, 2, 6, 0, 0), 1.101m, "Cycle(C) [2] (c) End");


        // ══════════════════════════════════════════════════
        // Intermediate Degree — inside Cycle(C) [3]
        //
        // (i): $1.1010 → $1.1010 (06.02.2026) (in-progress)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleC_P3_Intermediate_WaveCount() => AssertSubWaveCount(_analysis.Waves[2].SubWaves[2], 1, "Cycle(C) [3] Intermediate");

        [TestMethod] public void CycleC_P3_i1_Label() => AssertLabel(_analysis.Waves[2].SubWaves[2].SubWaves[0], WaveNumber.One, "Cycle(C) [3] (i)");
        [TestMethod] public void CycleC_P3_i1_InProgress() => AssertInProgress(_analysis.Waves[2].SubWaves[2].SubWaves[0], true, "Cycle(C) [3] (i)");
        [TestMethod] public void CycleC_P3_i1_Start() => AssertPoint(_analysis.Waves[2].SubWaves[2].SubWaves[0].StartPoint, Utc(2026, 2, 6, 0, 0), 1.101m, "Cycle(C) [3] (i) Start");

        // ══════════════════════════════════════════════════
        // Projections — Cycle (C): C extension of A ($2.00 → $55.09) from B end ($0.633)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleC_Projection_2618() => AssertProjectionTarget(_analysis.Waves[2], LogExtension(2.00m, 55.09m, 0.633m, 2.618m), 2.618m, "Cycle(C) 2.618 projection");
        [TestMethod] public void CycleC_Projection_1618() => AssertProjectionTarget(_analysis.Waves[2], LogExtension(2.00m, 55.09m, 0.633m, 1.618m), 1.618m, "Cycle(C) 1.618 projection");
        [TestMethod] public void CycleC_Projection_AEqualsC() => AssertProjectionTarget(_analysis.Waves[2], LogExtension(2.00m, 55.09m, 0.633m, 1.0m), 1.0m, "Cycle(C) A=C projection");
        [TestMethod] public void CycleC_Projection_0618() => AssertProjectionTarget(_analysis.Waves[2], LogExtension(2.00m, 55.09m, 0.633m, 0.618m), 0.618m, "Cycle(C) 0.618 projection");

        // ══════════════════════════════════════════════════
        // Projections — Primary [3] inside Cycle (C): W3 extension of P1 ($0.633 → $3.533) from P2 end ($1.101)
        // ══════════════════════════════════════════════════

        [TestMethod] public void CycleC_P3_Projection_4236() => AssertProjectionTarget(_analysis.Waves[2].SubWaves[2], LogExtension(0.633m, 3.533m, 1.101m, 4.236m), 4.236m, "Cycle(C) [3] 4.236 projection");
        [TestMethod] public void CycleC_P3_Projection_2618() => AssertProjectionTarget(_analysis.Waves[2].SubWaves[2], LogExtension(0.633m, 3.533m, 1.101m, 2.618m), 2.618m, "Cycle(C) [3] 2.618 projection");
        [TestMethod] public void CycleC_P3_Projection_1618() => AssertProjectionTarget(_analysis.Waves[2].SubWaves[2], LogExtension(0.633m, 3.533m, 1.101m, 1.618m), 1.618m, "Cycle(C) [3] 1.618 projection");
        [TestMethod] public void CycleC_P3_Projection_100() => AssertProjectionTarget(_analysis.Waves[2].SubWaves[2], LogExtension(0.633m, 3.533m, 1.101m, 1.0m), 1.0m, "Cycle(C) [3] 1.0 projection");
    }
}
