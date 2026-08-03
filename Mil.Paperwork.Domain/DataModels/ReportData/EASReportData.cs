using Mil.Paperwork.Infrastructure.DataModels;

namespace Mil.Paperwork.Domain.DataModels.ReportData
{
    public class EASReportData : IEASReportData
    {
        public string ReportNum { get; set; } = string.Empty;
        public DateTime ReportDate { get; set; } = DateTime.Today;
        public DateTime EventDate { get; set; } = DateTime.Today;
        public string EventTime { get; set; } = string.Empty;
        public string BattleOrder { get; set; } = string.Empty;
        public DateTime BattleOrderDate { get; set; } = DateTime.Today;
        public string SubdivisionName { get; set; } = string.Empty;
        public string ReporterRank { get; set; } = string.Empty;
        public string ReporterName { get; set; } = string.Empty;
        public string WhatHappened { get; set; } = string.Empty;
        public string OrdenNum { get; set; } = string.Empty;
        public DateTime OrdenDate { get; set; } = DateTime.Today;
        public IList<EASServiceData> Services { get; set; } = [];
        public IList<PersonDTO> Witnesses { get; set; } = [];

        public string DestinationFolder { get; set; } = string.Empty;

        public string GetDestinationPath()
        {
            return DestinationFolder;
        }
    }
}
