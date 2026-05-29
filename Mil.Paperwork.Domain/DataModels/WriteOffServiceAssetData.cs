namespace Mil.Paperwork.Domain.DataModels
{
    public class WriteOffServiceAssetData
    {
        public string Name { get; set; } = string.Empty;
        public int Count { get; set; }
        public string MeasurementUnit { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}
