using Mil.Paperwork.DataAccess.DataModels.Snapshots;
using Mil.Paperwork.DataAccess.Enums;
using Mil.Paperwork.Infrastructure.Enums;

namespace Mil.Paperwork.DataAccess.DataModels.History
{
    public class ReportHistoryEntry
    {
        public int SchemaVersion { get; set; }
        public Guid Id { get; set; }
        public ReportType ReportType { get; set; }
        public HistoryEntryStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public string DocumentNumber { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public List<string> GeneratedFiles { get; set; } = [];
        public ReportSnapshotBase? Snapshot { get; set; }
    }
}
