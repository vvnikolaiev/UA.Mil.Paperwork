namespace Mil.Paperwork.DataAccess.DataModels.Snapshots
{
    public class InitialTechnicalStateReportSnapshot : ReportSnapshotBase
    {
        public int EventType { get; set; }
        public List<AssetSnapshot> Assets { get; set; } = [];
        public PersonSnapshot? PersonAccepted { get; set; }
        public PersonSnapshot? PersonHanded { get; set; }
        public string DestinationFolder { get; set; } = string.Empty;
    }
}
