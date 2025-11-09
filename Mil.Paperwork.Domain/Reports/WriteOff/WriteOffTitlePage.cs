using Mil.Paperwork.Domain.DataModels.Parameters;
using Mil.Paperwork.Domain.Helpers;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Services;
using Spire.Doc;

namespace Mil.Paperwork.Domain.Reports.WriteOff
{
    internal class WriteOffTitlePage : WriteOffPackageReport
    {
        private readonly IReportDataService _reportDataService;

        public override string OutputFileName => WriteOffPackageTemplatesHelper.OUTPUT_TITLE_PAGE_NAME;

        protected override string TemplatePath => PathsHelper.GetTemplatePath(WriteOffPackageTemplatesHelper.WRITE_OFF_TITLE_PAGE_TEMPLATE_NAME);

        public WriteOffTitlePage(IReportDataService reportDataService)
        {
            _reportDataService = reportDataService;
        }

        protected override void FillReportData(IWriteOffPackageParameters reportParameters, Document document)
        {
            FillTheFields(reportParameters, document);
        }

        private void FillTheFields(IWriteOffPackageParameters reportParameters, Document document)
        {
            var reportConfig = ReportParametersHelper.GetFullParametersDictionary(ReportType.WriteOffPackage, _reportDataService);

            document.ReplaceField(WriteOffPackageTemplatesHelper.FIELD_TOTAL_WRITE_OFF_SUM, ReportHelper.GetPriceString(reportParameters.TotalWriteOffSum));

            document.ReplaceFields(reportConfig);
        }
    }
}
