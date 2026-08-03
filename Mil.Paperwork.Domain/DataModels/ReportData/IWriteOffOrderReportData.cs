using Mil.Paperwork.Infrastructure.DataModels;

namespace Mil.Paperwork.Domain.DataModels.ReportData
{
    public interface IWriteOffOrderReportData : IReportData
    {
        string ReportNum { get; set; }
        DateTime ReportDate { get; set; }
        DateTime EventDate { get; set; }
        string EventTime { get; set; }
        string BattleOrder { get; set; }
        DateTime BattleOrderDate { get; set; }
        string BattleOrderLocation { get; set; }
        string SubdivisionName { get; set; }
        string ReporterRank { get; set; }
        string ReporterName { get; set; }
        string CreatorPosition { get; set; }
        string CreatorRank { get; set; }
        string CreatorName { get; set; }
        string MilUnitApproval { get; set; }
        string WhatHappened { get; set; }
        IList<WriteOffServiceData> Services { get; set; }
        IList<PersonDTO> Witnesses { get; set; }
    }
}
