using Mil.Paperwork.Infrastructure.Enums;

namespace Mil.Paperwork.DataAccess.DataModels.Snapshots
{
    public class ResidualValueReportSnapshot : ReportSnapshotBase
    {
        public AssetType AssetType { get; set; }
        public List<AssetSnapshot> Assets { get; set; } = [];
        public Dictionary<string, decimal> MetalCosts { get; set; } = [];
        public DateTime EventDate { get; set; }
        public int? EventReportNumber { get; set; }
        public string DestinationFolder { get; set; } = string.Empty;
    }
}
