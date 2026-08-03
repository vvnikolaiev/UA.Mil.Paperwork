using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Infrastructure.Enums;

namespace Mil.Paperwork.DataAccess.Conversions
{
    public class ResidualValueToWriteOffPackageConversion : IReportDataConversion
    {
        public ReportType SourceType => ReportType.ResidualValueReport;

        public ReportType TargetType => ReportType.WriteOffPackage;

        public IReadOnlyList<IReportData> Convert(IReportData source)
        {
            var result = new List<IReportData>();

            if (source is not IResidualValueReportData residualValueData)
            {
                return result;
            }

            var today = DateTime.Now.Date;
            var packageData = new WriteOffPackageReportData
            {
                Assets = [.. residualValueData.Assets ?? []],
                EventDate = residualValueData.EventDate,
                DocumentDate = today,
                OrdenDate = today,
                ServiceKey = residualValueData.ServiceKey
            };

            var reportData = new WriteOffPackageTabData
            {
                PackageData = packageData
            };

            result.Add(reportData);

            return result;
        }
    }
}
