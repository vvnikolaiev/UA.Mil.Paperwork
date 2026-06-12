namespace Mil.Paperwork.DataAccess.DataModels.Snapshots
{
    public class ValuationReportSnapshot : ReportSnapshotBase
    {
        public List<AssetValuationSnapshot> ValuationData { get; set; } = [];
        public string DestinationFolder { get; set; } = string.Empty;
    }
}
