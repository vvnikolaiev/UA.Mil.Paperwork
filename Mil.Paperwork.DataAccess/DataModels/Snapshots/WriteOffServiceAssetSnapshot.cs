namespace Mil.Paperwork.DataAccess.DataModels.Snapshots
{
    public class WriteOffServiceAssetSnapshot
    {
        public string Name { get; set; } = string.Empty;
        public int Count { get; set; }
        public string MeasurementUnit { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}
