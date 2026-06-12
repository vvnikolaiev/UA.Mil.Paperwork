using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Infrastructure.Enums;

namespace Mil.Paperwork.DataAccess.Conversions
{
    public class ResidualValueToInitialTechnicalStateConversion : IReportDataConversion
    {
        public ReportType SourceType => ReportType.ResidualValueReport;

        public ReportType TargetType => ReportType.TechnicalStateReport;

        public IReadOnlyList<IReportData> Convert(IReportData source)
        {
            var result = new List<IReportData>();

            if (source is not IResidualValueReportData residualValueData)
            {
                return result;
            }

            var reportData = new InitialTechnicalStateReportData
            {
                Assets = [.. residualValueData.Assets ?? []]
            };

            result.Add(reportData);

            return result;
        }
    }
}
