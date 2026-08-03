using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Domain.Helpers;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Services;

namespace Mil.Paperwork.Domain.Reports
{
    internal class EASReport : IReport
    {
        private readonly IReportDataService _reportDataService;
        private byte[] _reportBytes;

        public EASReport(IReportDataService reportDataService)
        {
            _reportDataService = reportDataService;
        }

        public bool TryCreate(IEASReportData reportData)
        {
            try
            {
                var templatePath = PathsHelper.GetTemplatePath(EASHelper.REPORT_TEMPLATE_NAME);

                using var document = WordDocument.LoadFromFile(templatePath);

                var reportConfig = ReportParametersHelper.GetFullParametersDictionary(ReportType.EAS, _reportDataService);
                var milUnit = reportConfig.GetValueOrDefault(WriteOffOrderHelper.MilUnitConfigKey, string.Empty);

                FillSimpleFields(reportData, reportConfig, milUnit, document);
                FillWitnessesBlock(reportData, milUnit, document);
                FillHeadsOfServicesBlock(reportData, milUnit, document);
                FillAssetsTable(reportData, document);

                _reportBytes = document.GetBytes();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return false;
            }
        }

        public byte[] GetReportBytes()
        {
            return _reportBytes;
        }

        private void FillSimpleFields(IEASReportData reportData, Dictionary<string, string> reportConfig, string milUnit, WordDocument document)
        {
            document.ReplaceField(EASHelper.FIELD_REPORT_NUM, reportData.ReportNum);
            document.ReplaceField(EASHelper.FIELD_REPORT_DATE, reportData.ReportDate.ToString(ReportHelper.DATE_FORMAT));
            document.ReplaceField(EASHelper.FIELD_EVENT_DATE, reportData.EventDate.ToString(ReportHelper.DATE_FORMAT));
            document.ReplaceField(EASHelper.FIELD_EVENT_TIME, reportData.EventTime);
            document.ReplaceField(EASHelper.FIELD_BATTLE_ORDER, reportData.BattleOrder);
            document.ReplaceField(EASHelper.FIELD_BATTLE_ORDER_DATE, reportData.BattleOrderDate.ToString(ReportHelper.DATE_FORMAT));
            document.ReplaceField(EASHelper.FIELD_SUBDIVISION_NAME, reportData.SubdivisionName);
            document.ReplaceField(EASHelper.FIELD_REPORTER_RANK, reportData.ReporterRank);
            document.ReplaceField(EASHelper.FIELD_REPORTER_NAME, reportData.ReporterName);
            document.ReplaceField(EASHelper.FIELD_WHAT_HAPPENED, reportData.WhatHappened);
            document.ReplaceField(EASHelper.FIELD_ORDEN_NUM, reportData.OrdenNum);
            document.ReplaceField(EASHelper.FIELD_ORDEN_DATE, reportData.OrdenDate.ToString(ReportHelper.DATE_FORMAT));
            document.ReplaceField(EASHelper.FIELD_EVENT_WITNESSES_TEXT, EASHelper.BuildWitnessesText(reportData.Witnesses, milUnit));

            document.ReplaceFields(reportConfig);
            document.ReplaceFields(ReportParametersHelper.GetCommission(ReportType.EAS, _reportDataService));
        }

        private static void FillWitnessesBlock(IEASReportData reportData, string milUnit, WordDocument document)
        {
            var blockParagraphs = EASHelper.BuildWitnessesBlock(reportData.Witnesses, milUnit);
            document.ReplaceFieldWithBlock(EASHelper.FIELD_EVENT_WITNESSES_BLOCK, blockParagraphs);
        }

        private static void FillHeadsOfServicesBlock(IEASReportData reportData, string milUnit, WordDocument document)
        {
            var blockParagraphs = EASHelper.BuildHeadsOfServicesBlock(reportData.Services, milUnit);
            document.ReplaceFieldWithBlock(EASHelper.FIELD_HEADS_OF_SERVICES_BLOCK, blockParagraphs);
        }

        private static void FillAssetsTable(IEASReportData reportData, WordDocument document)
        {
            var table = document.GetTable(EASHelper.ASSETS_TABLE_NAME);
            if (table != null)
            {
                EASHelper.FillAssetsTable(table, reportData.Services);
            }
        }
    }
}
