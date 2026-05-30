using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Domain.Helpers;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Services;

namespace Mil.Paperwork.Domain.Reports
{
    internal class WriteOffOrderReport : IReport
    {
        private readonly IReportDataService _reportDataService;
        private byte[] _reportBytes;

        public WriteOffOrderReport(IReportDataService reportDataService)
        {
            _reportDataService = reportDataService;
        }

        public bool TryCreate(IWriteOffOrderReportData reportData)
        {
            try
            {
                var templatePath = PathsHelper.GetTemplatePath(WriteOffOrderHelper.REPORT_TEMPLATE_NAME);

                using var document = WordDocument.LoadFromFile(templatePath);

                FillSimpleFields(reportData, document);
                FillServicesBlock(reportData, document);
                FillWitnessesBlock(reportData, document);

                _reportBytes = document.GetBytes();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return false;
            }
        }

        public byte[] GetReportBytes() => _reportBytes;

        private void FillSimpleFields(IWriteOffOrderReportData reportData, WordDocument document)
        {
            var reportConfig = ReportParametersHelper.GetFullParametersDictionary(ReportType.WriteOffOrder, _reportDataService);

            var totalSum = WriteOffOrderHelper.CalculateTotalSum(reportData.Services);
            var totalSumText = ReportHelper.ConvertTotalSumToUkrainianString(totalSum);
            var toHeads = WriteOffOrderHelper.BuildToHeadsOfServices(reportData.Services);

            document.ReplaceField(WriteOffOrderHelper.FIELD_REPORT_NUM, reportData.ReportNum);
            document.ReplaceField(WriteOffOrderHelper.FIELD_REPORT_DATE, reportData.ReportDate.ToString(ReportHelper.DATE_FORMAT));
            document.ReplaceField(WriteOffOrderHelper.FIELD_EVENT_DATE, reportData.EventDate.ToString(ReportHelper.DATE_FORMAT));
            document.ReplaceField(WriteOffOrderHelper.FIELD_EVENT_TIME, reportData.EventTime);
            document.ReplaceField(WriteOffOrderHelper.FIELD_BATTLE_ORDER, reportData.BattleOrder);
            document.ReplaceField(WriteOffOrderHelper.FIELD_BATTLE_ORDER_DATE, reportData.BattleOrderDate.ToString(ReportHelper.DATE_FORMAT));
            document.ReplaceField(WriteOffOrderHelper.FIELD_BATTLE_ORDER_LOCATION, reportData.BattleOrderLocation);
            document.ReplaceField(WriteOffOrderHelper.FIELD_SUBDIVISION_NAME, reportData.SubdivisionName);
            document.ReplaceField(WriteOffOrderHelper.FIELD_REPORTER_RANK, reportData.ReporterRank);
            document.ReplaceField(WriteOffOrderHelper.FIELD_REPORTER_NAME, reportData.ReporterName);
            document.ReplaceField(WriteOffOrderHelper.FIELD_CREATOR_POSITION, reportData.CreatorPosition);
            document.ReplaceField(WriteOffOrderHelper.FIELD_CREATOR_RANK, reportData.CreatorRank);
            document.ReplaceField(WriteOffOrderHelper.FIELD_CREATOR_NAME, reportData.CreatorName);
            document.ReplaceField(WriteOffOrderHelper.FIELD_MIL_UNIT_APPROVAL, reportData.MilUnitApproval);
            document.ReplaceField(WriteOffOrderHelper.FIELD_WHAT_HAPPENED, reportData.WhatHappened);
            document.ReplaceField(WriteOffOrderHelper.FIELD_TOTAL_SUM, ReportHelper.GetPriceString(totalSum));
            document.ReplaceField(WriteOffOrderHelper.FIELD_TOTAL_SUM_TEXT, totalSumText);
            document.ReplaceField(WriteOffOrderHelper.FIELD_TO_HEADS_OF_SERVICES, toHeads);

            document.ReplaceFields(reportConfig);
        }

        private static void FillServicesBlock(IWriteOffOrderReportData reportData, WordDocument document)
        {
            var blockParagraphs = WriteOffOrderHelper.BuildServicesBlock(reportData.Services);
            document.ReplaceFieldWithBlock(WriteOffOrderHelper.FIELD_SERVICES_BLOCK, blockParagraphs);
        }

        private static void FillWitnessesBlock(IWriteOffOrderReportData reportData, WordDocument document)
        {
            var blockParagraphs = WriteOffOrderHelper.BuildWitnessesBlock(reportData.Witnesses);
            document.ReplaceFieldWithBlock(WriteOffOrderHelper.FIELD_EVENT_WITNESSES, blockParagraphs);
        }
    }
}
