﻿using Mil.Paperwork.Domain.DataModels.Parameters;
using Mil.Paperwork.Domain.Helpers;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Services;
using Spire.Doc;

namespace Mil.Paperwork.Domain.Reports.WriteOff
{
    internal class WriteOffConsentSheet : WriteOffPackageReport
    {
        private readonly IReportDataService _reportDataService;

        public override string OutputFileName => WriteOffPackageTemplatesHelper.OUTPUT_CONSENT_SHEET_NAME;

        protected override string TemplatePath => PathsHelper.GetTemplatePath(WriteOffPackageTemplatesHelper.CONSENT_SHEET_TEMPLATE_NAME);
        
        public WriteOffConsentSheet(IReportDataService reportDataService)
        {
            _reportDataService = reportDataService;
        }

        protected override void FillReportData(IWriteOffPackageParameters reportParameters, Document document)
        {
            FillTheFields(reportParameters, document);
        }

        private void FillTheFields(IWriteOffPackageParameters reportParameters, Document document)
        {
            var reportConfig = _reportDataService.GetReportParametersDictionary(ReportType.WriteOffPackage);

            document.ReplaceField(WriteOffPackageTemplatesHelper.FIELD_TOTAL_WRITE_OFF_SUM, reportParameters.TotalWriteOffSum.ToString());

            document.ReplaceFields(reportConfig);
        }
    }
}
