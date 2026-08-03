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
        public string ServiceKey { get; set; } = string.Empty;
        public string DestinationFolder { get; set; } = string.Empty;

        public string Reason { get; set; } = string.Empty;
        public int EventType { get; set; }
        public bool GenerateWriteOffPackage { get; set; } = true;
        public bool GenerateWriteOffActs { get; set; } = true;
        public string WriteOffRegNumber { get; set; } = string.Empty;
        public string WriteOffDocNumber { get; set; } = string.Empty;
        public bool GenerateQualityStateReportInstead { get; set; }
        public string QSRRegNumber { get; set; } = string.Empty;
        public string QSRDocNumber { get; set; } = string.Empty;
    }
}
