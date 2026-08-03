using Mil.Paperwork.Infrastructure.DataModels;

namespace Mil.Paperwork.Domain.DataModels.ReportData
{
    public interface IEASReportData : IReportData
    {
        string ReportNum { get; set; }
        DateTime ReportDate { get; set; }
        DateTime EventDate { get; set; }
        string EventTime { get; set; }
        string BattleOrder { get; set; }
        DateTime BattleOrderDate { get; set; }
        string SubdivisionName { get; set; }
        string ReporterRank { get; set; }
        string ReporterName { get; set; }
        string WhatHappened { get; set; }
        string OrdenNum { get; set; }
        DateTime OrdenDate { get; set; }
        IList<EASServiceData> Services { get; set; }
        IList<PersonDTO> Witnesses { get; set; }
    }
}
