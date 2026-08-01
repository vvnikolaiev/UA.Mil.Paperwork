namespace Mil.Paperwork.DataAccess.DataModels.Snapshots
{
    public class EASAssetSnapshot
    {
        public string Name { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string MeasurementUnit { get; set; } = string.Empty;
        public int Category { get; set; }
        public int Count { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal ResidualPrice { get; set; }
    }
}
