namespace Mil.Paperwork.DataAccess.DataModels.Snapshots
{
    public class DismantlingReportSnapshot : ReportSnapshotBase
    {
        public List<AssetDismantlingSnapshot> Dismantlings { get; set; } = [];
        public string DestinationFolder { get; set; } = string.Empty;
    }
}
