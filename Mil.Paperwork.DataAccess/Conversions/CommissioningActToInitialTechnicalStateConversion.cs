using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Infrastructure.Enums;

namespace Mil.Paperwork.DataAccess.Conversions
{
    public class CommissioningActToInitialTechnicalStateConversion : IReportDataConversion
    {
        public ReportType SourceType => ReportType.CommissioningAct;

        public ReportType TargetType => ReportType.TechnicalStateReport;

        public IReadOnlyList<IReportData> Convert(IReportData source)
        {
            var result = new List<IReportData>();

            if (source is not ICommissioningActReportData actData || actData.Asset == null)
            {
                return result;
            }

            var reportData = new InitialTechnicalStateReportData
            {
                Assets = [ConversionHelper.ToAssetInfo(actData)],
                PersonAccepted = actData.PersonAccepted,
                PersonHanded = actData.PersonHanded
            };

            result.Add(reportData);

            return result;
        }
    }
}
