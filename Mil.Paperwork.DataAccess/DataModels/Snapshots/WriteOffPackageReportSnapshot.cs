namespace Mil.Paperwork.DataAccess.DataModels.Snapshots
{
    public class WriteOffPackageReportSnapshot : ReportSnapshotBase
    {
        public List<AssetSnapshot> Assets { get; set; } = [];
        public DateTime DocumentDate { get; set; }
        public DateTime EventDate { get; set; }
        public int OrdenNumber { get; set; }
        public DateTime OrdenDate { get; set; }
        public BookExtractSnapshot? BookOfLossesExtract { get; set; }
        public string DestinationFolder { get; set; } = string.Empty;
    }
}
