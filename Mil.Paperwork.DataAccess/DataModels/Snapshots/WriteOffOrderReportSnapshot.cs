namespace Mil.Paperwork.DataAccess.DataModels.Snapshots
{
    public class WriteOffOrderReportSnapshot : ReportSnapshotBase
    {
        public string ReportNum { get; set; } = string.Empty;
        public DateTime ReportDate { get; set; }
        public DateTime EventDate { get; set; }
        public string EventTime { get; set; } = string.Empty;
        public string BattleOrder { get; set; } = string.Empty;
        public DateTime BattleOrderDate { get; set; }
        public string BattleOrderLocation { get; set; } = string.Empty;
        public string SubdivisionName { get; set; } = string.Empty;
        public string ReporterRank { get; set; } = string.Empty;
        public string ReporterName { get; set; } = string.Empty;
        public string CreatorPosition { get; set; } = string.Empty;
        public string CreatorRank { get; set; } = string.Empty;
        public string CreatorName { get; set; } = string.Empty;
        public string MilUnitApproval { get; set; } = string.Empty;
        public string WhatHappened { get; set; } = string.Empty;
        public List<WriteOffServiceSnapshot> Services { get; set; } = [];
        public List<PersonSnapshot> Witnesses { get; set; } = [];
        public string DestinationFolder { get; set; } = string.Empty;
    }
}
