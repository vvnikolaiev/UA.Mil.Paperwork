namespace Mil.Paperwork.DataAccess.DataModels.Snapshots
{
    public class InvoiceReportSnapshot : ReportSnapshotBase
    {
        public string DocumentNumber { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; }
        public DateTime DueDate { get; set; }
        public string Reason { get; set; } = string.Empty;
        public PersonSnapshot? Recipient { get; set; }
        public PersonSnapshot? Transmitter { get; set; }
        public PersonSnapshot? HeadOfService { get; set; }
        public List<AssetSnapshot> Assets { get; set; } = [];
        public string DestinationFolder { get; set; } = string.Empty;
    }
}
