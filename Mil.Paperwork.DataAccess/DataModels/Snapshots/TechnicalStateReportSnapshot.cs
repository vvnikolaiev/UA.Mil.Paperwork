namespace Mil.Paperwork.DataAccess.DataModels.Snapshots
{
    public class TechnicalStateReportSnapshot : ReportSnapshotBase
    {
        public int EventType { get; set; }
        public List<AssetSnapshot> Assets { get; set; } = [];
        public DateTime DocumentDate { get; set; }
        public string Reason { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public int OrdenNumber { get; set; }
        public DateTime OrdenDate { get; set; }
        public bool GenerateWriteOffActs { get; set; }
        public string DestinationFolder { get; set; } = string.Empty;
    }
}
