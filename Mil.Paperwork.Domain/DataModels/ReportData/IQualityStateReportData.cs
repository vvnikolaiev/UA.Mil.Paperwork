using Mil.Paperwork.Domain.DataModels.Assets;
using Mil.Paperwork.Domain.Enums;

namespace Mil.Paperwork.Domain.DataModels.ReportData
{
    public interface IQualityStateReportData : IReportData
    {
        public EventType EventType { get; set; }
        DateTime EventDate { get; }
        public string RegistrationNumber { get; set; }

        public string DocumentNumber { get; set; }
        IList<IAssetInfo> Assets { get; set; }
    }
}
