namespace Mil.Paperwork.DataAccess.DataModels.Snapshots
{
    public class CommissioningActReportSnapshot : ReportSnapshotBase
    {
        public string DocumentNumber { get; set; } = string.Empty;
        public DateTime DocumentDate { get; set; }
        public ProductSnapshot? Asset { get; set; }
        public string AssetState { get; set; } = string.Empty;
        public List<ProductIdentificationSnapshot> AssetIds { get; set; } = [];
        public string CountText { get; set; } = string.Empty;
        public int Count { get; set; }
        public string CommissioningLocation { get; set; } = string.Empty;
        public string ShortCharacteristic { get; set; } = string.Empty;
        public string AssetCompliance { get; set; } = string.Empty;
        public string CompletionState { get; set; } = string.Empty;
        public string TestResults { get; set; } = string.Empty;
        public string OtherInfo { get; set; } = string.Empty;
        public string Conclusion { get; set; } = string.Empty;
        public string AttachedDocumentation { get; set; } = string.Empty;
        public PersonSnapshot? PersonAccepted { get; set; }
        public PersonSnapshot? PersonHanded { get; set; }
        public string DestinationFolder { get; set; } = string.Empty;
    }
}
