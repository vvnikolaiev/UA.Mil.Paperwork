using Mil.Paperwork.Domain.DataModels.Parameters;
using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Domain.Helpers;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Services;
using Spire.Doc;
using Spire.Doc.Documents;
using System.IO;

namespace Mil.Paperwork.Domain.Reports
{
    internal class QualityStateReport : IQualityStateReport
    {
        private readonly IReportDataService _reportDataService;
        
        private byte[] _reportBytes;

        public QualityStateReport(IReportDataService reportDataService)
        {
            _reportDataService = reportDataService;
        }

        public bool TryCreate(ICommonWriteOffReportData reportData)
        {
            try
            {
                var templatePath = PathsHelper.GetTemplatePath(QualityStateReportHelper.REPORT_TEMPLATE_NAME);

                var document = new Document();
                document.LoadFromFile(templatePath);

                FillCommission(document);
                FillTheFields(reportData, document);

                var table = document.GetTable(QualityStateReportHelper.TABLE_ASSETS_NAME);

                if (table != null)
                {
                    FillTheTable(reportData, table);
                }

                using var memoryStream = new MemoryStream();
                document.SaveToStream(memoryStream, FileFormat.Docx);
                _reportBytes = memoryStream.ToArray();

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

        private void FillCommission(Document document)
        {
            var dictCommissionFields = ReportParametersHelper.GetCommission(ReportType.QualityStateReport, _reportDataService);

            document.ReplaceFields(dictCommissionFields);
        }

        private void FillTheFields(ICommonWriteOffReportData reportData, Document document)
        {
            var reportConfig = ReportParametersHelper.GetFullParametersDictionary(ReportType.QualityStateReport, _reportDataService);

            document.ReplaceFields(reportConfig);

            document.ReplaceField(QualityStateReportHelper.FIELD_REGISTRATION_NUMBER, reportData.RegistrationNumber);
            document.ReplaceField(QualityStateReportHelper.FIELD_DOCUMENT_NUMBER, reportData.DocumentNumber);
            document.ReplaceField(QualityStateReportHelper.FIELD_DOCUMENT_DATE, reportData.DocumentDate.ToString(ReportHelper.DATE_FORMAT));

            document.ReplaceField(QualityStateReportHelper.FIELD_REASON, reportData.Reason);
            document.ReplaceField(QualityStateReportHelper.FIELD_EVENT_DATE, reportData.EventDate.ToString(ReportHelper.DATE_FORMAT));
            document.ReplaceField(QualityStateReportHelper.FIELD_ORDEN_NUMBER, reportData.OrdenNumber.ToString());
            document.ReplaceField(QualityStateReportHelper.FIELD_ORDEN_DATE, reportData.OrdenDate.ToString(ReportHelper.DATE_FORMAT));
        }

        private static void FillTheTable(ICommonWriteOffReportData reportData, Table table)
        {
            var firstRow = table.LastRow;

            var nameCellParameters = new WordCellParameters(QualityStateReportHelper.TABLE_FONT_SIZE, HorizontalAlignment.Left);
            var cellParameters = new WordCellParameters(QualityStateReportHelper.TABLE_FONT_SIZE, HorizontalAlignment.Center);

            for (int i = 0; i < reportData.Assets.Count; i++)
            {
                var asset = reportData.Assets[i];
                TableRow row = table.AddRow();

                // TODO: optimize. Make a mapper.

                var residualPrice = ResidualPriceHelper.CalculateResidualPriceForItem(asset, reportData.EventDate);

                var initialCategory = ReportHelper.ConvertCategoryToText(asset.InitialCategory);
                var residualCategory = ReportHelper.ConvertEventTypeToCategoryText(asset.InitialCategory, reportData.EventType);
                
                var assetName = ReportHelper.GetFullAssetName(asset.Name, asset.SerialNumber);
                var nomenclatureCode = asset.NomenclatureCode?.ToUpper() ?? string.Empty;
                var monthsOperated = (int)((reportData.EventDate - asset.StartDate).TotalDays / 30);
                
                var exploitationNorm = asset.ResourceYears != 0 ? asset.ResourceYears * 12 : 60;

                row.Cells[QualityStateReportHelper.COLUMN_INDEX].AddNumber(i + 1, cellParameters);
                row.Cells[QualityStateReportHelper.COLUMN_NAME].AddText(assetName, nameCellParameters);
                row.Cells[QualityStateReportHelper.COLUMN_NOMENCLATURE_CODE].AddText(nomenclatureCode, cellParameters);
                row.Cells[QualityStateReportHelper.COLUMN_MEASUREMENT_UNIT].AddText(asset.MeasurementUnit, cellParameters);
                row.Cells[QualityStateReportHelper.COLUMN_CATEGORY].AddText(initialCategory, cellParameters);
                row.Cells[QualityStateReportHelper.COLUMN_COUNT].AddNumber(asset.Count, cellParameters);
                row.Cells[QualityStateReportHelper.COLUMN_PRICE].AddPrice(asset.Price, cellParameters);
                row.Cells[QualityStateReportHelper.COLUMN_TOTAL_PRICE].AddPrice(asset.Count * asset.Price, cellParameters);
                row.Cells[QualityStateReportHelper.COLUMN_EXPLOITATION_FACT].AddNumber(monthsOperated, cellParameters);
                row.Cells[QualityStateReportHelper.COLUMN_EXPLOITATION_NORM].AddNumber(exploitationNorm, cellParameters);
                row.Cells[QualityStateReportHelper.COLUMN_EXPLOITATION_NAME].AddText(assetName, nameCellParameters);
                row.Cells[QualityStateReportHelper.COLUMN_EXPLOITATION_NOMENCLATURE_CODE].AddText(nomenclatureCode, cellParameters);
                row.Cells[QualityStateReportHelper.COLUMN_EXPLOITATION_MEASUREMENT_UNIT].AddText(asset.MeasurementUnit, cellParameters);
                row.Cells[QualityStateReportHelper.COLUMN_EXPLOITATION_CATEGORY].AddText(residualCategory, cellParameters);
                row.Cells[QualityStateReportHelper.COLUMN_EXPLOITATION_COUNT].AddNumber(asset.Count, cellParameters);
                row.Cells[QualityStateReportHelper.COLUMN_EXPLOITATION_PRICE].AddPrice(residualPrice, cellParameters);
                row.Cells[QualityStateReportHelper.COLUMN_EXPLOITATION_TOTAL_PRICE].AddPrice(residualPrice * asset.Count, cellParameters);
            }

            table.Rows.Remove(firstRow);

            AddSummaryRow(reportData, table);
        }

        private static void AddSummaryRow(ICommonWriteOffReportData reportData, Table table)
        {
            var cellParameters = new WordCellParameters(QualityStateReportHelper.TABLE_FONT_SIZE, HorizontalAlignment.Left);
            var totalSumClear = ResidualPriceHelper.CalculateTotalReportSum(reportData.Assets, reportData.EventDate, false);
            var totalSum = ResidualPriceHelper.CalculateTotalReportSum(reportData.Assets, reportData.EventDate, true);

            var totalSumText = ReportHelper.ConvertTotalSumToUkrainianString(totalSum);
            var totalItemsText = ReportHelper.ConvertNamesNumberToReportString(reportData.Assets.Count);

            // last united string row 
            var textSummaryRow = table.AddRow(false);
            var summaryCell = textSummaryRow.CreateMergedCell(0, textSummaryRow.Cells.Count);
            var summaryText = string.Format(QualityStateReportHelper.TOTAL_TEXT_FORMAT, totalItemsText, totalSumText);
            textSummaryRow.Cells[0].AddText(summaryText, cellParameters);
        }
    }
}
