namespace Mil.Paperwork.Domain.DataModels.ReportData
{
    public class WriteOffOrderReportData : IWriteOffOrderReportData
    {
        public string ReportNum { get; set; } = string.Empty;
        public DateTime ReportDate { get; set; } = DateTime.Today;
        public DateTime EventDate { get; set; } = DateTime.Today;
        public string EventTime { get; set; } = string.Empty;
        public string BattleOrder { get; set; } = string.Empty;
        public DateTime BattleOrderDate { get; set; } = DateTime.Today;
        public string BattleOrderLocation { get; set; } = string.Empty;
        public string SubdivisionName { get; set; } = string.Empty;
        public string ReporterRank { get; set; } = string.Empty;
        public string ReporterName { get; set; } = string.Empty;
        public string CreatorPosition { get; set; } = string.Empty;
        public string CreatorRank { get; set; } = string.Empty;
        public string CreatorName { get; set; } = string.Empty;
        public string MilUnitApproval { get; set; } = string.Empty;
        public string WhatHappened { get; set; } = string.Empty;
        public IList<WriteOffServiceData> Services { get; set; } = [];

        public string DestinationFolder { get; set; } = string.Empty;

        public string GetDestinationPath() => DestinationFolder;
    }
}
