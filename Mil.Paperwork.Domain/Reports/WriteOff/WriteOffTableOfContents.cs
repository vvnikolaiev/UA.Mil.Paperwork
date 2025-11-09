using Mil.Paperwork.Domain.DataModels.Parameters;
using Mil.Paperwork.Domain.Helpers;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Services;
using Spire.Doc;

namespace Mil.Paperwork.Domain.Reports.WriteOff
{
    internal class WriteOffTableOfContents : WriteOffPackageReport
    {
        private readonly IReportDataService _reportDataService;

        public override string OutputFileName => WriteOffPackageTemplatesHelper.OUTPUT_TABLE_OF_CONTENTS_NAME;

        protected override string TemplatePath => PathsHelper.GetTemplatePath(WriteOffPackageTemplatesHelper.WRITE_OFF_TABLE_OF_CONTENTS_TEMPLATE_NAME);

        public WriteOffTableOfContents(IReportDataService reportDataService)
        {
            _reportDataService = reportDataService;
        }

        protected override void FillReportData(IWriteOffPackageParameters reportParameters, Document document)
        {
            FillTheFields(reportParameters, document);
            FillTOCTable(reportParameters, document);
        }

        private void FillTheFields(IWriteOffPackageParameters reportParameters, Document document)
        {
            var reportConfig = ReportParametersHelper.GetFullParametersDictionary(ReportType.WriteOffPackage, _reportDataService);
            
            document.ReplaceField(WriteOffPackageTemplatesHelper.FIELD_ORDEN_NUM, reportParameters.OrdenNumber.ToString());
            document.ReplaceField(WriteOffPackageTemplatesHelper.FIELD_ORDEN_DATE, reportParameters.OrdenDate.ToString(ReportHelper.DATE_FORMAT));

            document.ReplaceFields(reportConfig);
        }

        private void FillTOCTable(IWriteOffPackageParameters reportParameters, Document document)
        {
            var table = document.GetTable(WriteOffPackageTemplatesHelper.TABLE_OF_CONTENTS_NAME);

            if (table != null)
            {
                var fontSize = WriteOffPackageTemplatesHelper.TABLE_OF_CONTENTS_FONT_SIZE;
                // do the work when ready.
            }
        }
    }
}
