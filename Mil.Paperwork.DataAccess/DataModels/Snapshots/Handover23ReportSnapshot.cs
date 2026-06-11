namespace Mil.Paperwork.DataAccess.DataModels.Snapshots
{
    public class Handover23ReportSnapshot : ReportSnapshotBase
    {
        public string DocumentNumber { get; set; } = string.Empty;
        public DateTime DocumentDate { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public string Supplier { get; set; } = string.Empty;
        public string Receiver { get; set; } = string.Empty;
        public string ReasonDocumentName { get; set; } = string.Empty;
        public string ReasonDocumentNumber { get; set; } = string.Empty;
        public DateTime ReasonDocumentDate { get; set; }
        public string Reason { get; set; } = string.Empty;
        public PersonSnapshot? PersonResponsible { get; set; }
        public PersonSnapshot? PersonReceiver { get; set; }
        public List<AssetSnapshot> Assets { get; set; } = [];
        public string DestinationFolder { get; set; } = string.Empty;
    }
}
