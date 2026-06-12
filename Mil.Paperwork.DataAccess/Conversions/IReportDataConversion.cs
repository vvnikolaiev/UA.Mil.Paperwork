using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Infrastructure.Enums;

namespace Mil.Paperwork.DataAccess.Conversions
{
    public interface IReportDataConversion
    {
        ReportType SourceType { get; }

        ReportType TargetType { get; }

        IReadOnlyList<IReportData> Convert(IReportData source);
    }
}
