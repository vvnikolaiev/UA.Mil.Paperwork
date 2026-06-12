using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Infrastructure.Enums;

namespace Mil.Paperwork.DataAccess.Conversions
{
    public class ReportConversionRegistry
    {
        private readonly List<IReportDataConversion> _conversions;

        public ReportConversionRegistry(IEnumerable<IReportDataConversion> conversions)
        {
            _conversions = [.. conversions];
        }

        public IReadOnlyList<ReportType> GetTargets(ReportType sourceType)
        {
            var targets = _conversions
                .Where(conversion => conversion.SourceType == sourceType)
                .Select(conversion => conversion.TargetType)
                .Distinct()
                .ToList();

            return targets;
        }

        public IReadOnlyList<IReportData> Convert(ReportType sourceType, ReportType targetType, IReportData source)
        {
            var conversion = _conversions.FirstOrDefault(item => item.SourceType == sourceType && item.TargetType == targetType);

            IReadOnlyList<IReportData> result = conversion != null
                ? conversion.Convert(source)
                : [];

            return result;
        }
    }
}
