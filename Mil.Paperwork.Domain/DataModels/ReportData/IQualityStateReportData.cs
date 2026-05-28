using Mil.Paperwork.Domain.DataModels.Assets;
using Mil.Paperwork.Domain.Enums;

namespace Mil.Paperwork.Domain.DataModels.ReportData
{
    public interface ICommonWriteOffReportData : IReportData
    {
        public EventType EventType { get; set; }
        DateTime EventDate { get; }
        string RegistrationNumber { get; set; }
        DateTime DocumentDate { get; set; }
        string DocumentNumber { get; set; }
        IList<IAssetInfo> Assets { get; set; }
        string Reason { get; }
        int OrdenNumber { get; }
        DateTime OrdenDate { get; }
    }
}
