namespace SmarterSystems.ElliottWaves.Analyzer.Interfaces
{
    public enum PointType
    {
        High = 1,
        Low = 2
    }

    public class PivotPoint
    {
        public int Index { get; set; }
        public DateTime Timestamp { get; set; }
        public decimal Price { get; set; }
        public PointType PointType { get; set; }
    }
}
