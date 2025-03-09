namespace Mil.Paperwork.Domain.DataModels
{
    public class WriteOffReportData
    {
        public string DestinationFolder { get; set; }

        public string RegistrationNumber { get; set; }

        public string DocumentNumber { get; set; }

        public string Reason { get; set; }
        
        public DateTime WriteOffDate { get; set; }

        public List<AssetInfo> Assets { get; set; }

    }
}
