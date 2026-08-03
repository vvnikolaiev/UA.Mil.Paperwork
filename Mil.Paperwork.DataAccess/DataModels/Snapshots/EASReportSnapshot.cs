namespace Mil.Paperwork.DataAccess.DataModels.Snapshots
{
    public class EASReportSnapshot : ReportSnapshotBase
    {
        public string ReportNum { get; set; } = string.Empty;
        public DateTime ReportDate { get; set; }
        public DateTime EventDate { get; set; }
        public string EventTime { get; set; } = string.Empty;
        public string BattleOrder { get; set; } = string.Empty;
        public DateTime BattleOrderDate { get; set; }
        public string SubdivisionName { get; set; } = string.Empty;
        public string ReporterRank { get; set; } = string.Empty;
        public string ReporterName { get; set; } = string.Empty;
        public string WhatHappened { get; set; } = string.Empty;
        public string OrdenNum { get; set; } = string.Empty;
        public DateTime OrdenDate { get; set; }
        public List<EASServiceSnapshot> Services { get; set; } = [];
        public List<PersonSnapshot> Witnesses { get; set; } = [];
        public string DestinationFolder { get; set; } = string.Empty;
    }
}
