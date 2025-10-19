using Mil.Paperwork.Domain.DataModels.Parameters;
using Mil.Paperwork.Domain.Helpers;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Services;
using Spire.Doc;

namespace Mil.Paperwork.Domain.Reports.WriteOff
{
    internal class BookOfLossesExtract : WriteOffPackageReport
    {
        private readonly IReportDataService _reportDataService;

        public override string OutputFileName => WriteOffPackageTemplatesHelper.OUTPUT_BOOK_OF_LOSSES_NAME;

        protected override string TemplatePath => PathsHelper.GetTemplatePath(WriteOffPackageTemplatesHelper.EXTRACT_FROM_BOOK_OF_LOSSES_TEMPLATE_NAME);

        public BookOfLossesExtract(IReportDataService reportDataService)
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
            var reportConfig = _reportDataService.GetReportParametersDictionary(ReportType.WriteOffPackage);

            var extractData = reportParameters.BookOfLossesExtractData;

            document.ReplaceField(WriteOffPackageTemplatesHelper.FIELD_BOOK_NUM, extractData.Number.ToString());
            document.ReplaceField(WriteOffPackageTemplatesHelper.FIELD_BOOK_YEAR, extractData.Year.ToString());
            document.ReplaceField(WriteOffPackageTemplatesHelper.FIELD_PAGE_NUM, extractData.PageNumber.ToString());
            document.ReplaceField(WriteOffPackageTemplatesHelper.FIELD_RECORD_DATE, extractData.RecordDate.ToString(ReportHelper.DATE_FORMAT));
            document.ReplaceField(WriteOffPackageTemplatesHelper.FIELD_TOTAL_WRITE_OFF_SUM, reportParameters.TotalWriteOffSum.ToString());
            document.ReplaceField(WriteOffPackageTemplatesHelper.FIELD_ORDEN_NUM, reportParameters.OrdenNumber.ToString());
            document.ReplaceField(WriteOffPackageTemplatesHelper.FIELD_ORDEN_DATE, reportParameters.OrdenDate.ToString(ReportHelper.DATE_FORMAT));

            document.ReplaceFields(reportConfig);
        }

        private void FillTOCTable(IWriteOffPackageParameters reportParameters, Document document)
        {
            var table = document.GetTable(WriteOffPackageTemplatesHelper.TABLE_BOOK_RECORD_NAME);

            if (table != null)
            {
                var fontSize = WriteOffPackageTemplatesHelper.TABLE_BOOK_RECORD_FONT_SIZE;
                // do the work when ready.
            }
        }
    }
}
