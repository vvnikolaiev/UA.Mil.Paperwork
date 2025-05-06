namespace Mil.Paperwork.Infrastructure.DataModels
{
    public class AssetDismantlingData : AssetValuationData
    {
        public string RegistrationNumber { get; set; }
        public string DocumentNumber { get; set; }
        public string Reason { get; set; }

        public int Category { get; set; } = 2;
        public int Count => GetCount();
        public string MeasurementUnit { get; set; } = "к-т";

        public decimal TotalPrice => Price * Count;

        private int GetCount()
        {
            var count = SerialNumber?.Split(',').Length ?? 1;
            
            return count;
        }
    }
}
