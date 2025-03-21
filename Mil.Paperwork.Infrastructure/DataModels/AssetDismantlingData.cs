namespace Mil.Paperwork.Infrastructure.DataModels
{
    public class AssetDismantlingData : AssetValuationData
    {
        public string RegistrationNumber { get; set; }
        public string DocumentNumber { get; set; }
        public string Reason { get; set; }

        public int Category { get; set; } = 2;
        public string MeasurementUnit { get; set; } = "к-т";
    }
}
