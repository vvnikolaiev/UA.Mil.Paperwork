namespace Mil.Paperwork.Domain.DataModels
{
    public class TechnicalStateReportData : ITechnicalStateReportData
    {
        public string Reason { get; set; }

        public DateTime ReportDate { get; set; }

        public IList<AssetInfo> Assets { get; set; }

        public string DestinationFolder { get; set; }

        public string GetDestinationPath()
        {
            return DestinationFolder;
        }
    }
}
