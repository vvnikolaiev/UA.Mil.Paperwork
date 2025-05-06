using Mil.Paperwork.Domain.DataModels;
using Mil.Paperwork.Domain.Helpers;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Services;
using Spire.Doc;
using Spire.Doc.Documents;
using System.IO;

namespace Mil.Paperwork.Domain.Reports
{
    internal class QualityStateReport : IReport
    {
        private readonly IReportDataService _reportDataService;
        
        private byte[] _reportBytes;

        public QualityStateReport(IReportDataService reportDataService)
        {
            _reportDataService = reportDataService;
        }

        public bool TryCreate(WriteOffReportData reportData)
        {
            try
            {
                var templatePath = PathsHelper.GetTemplatePath(QualityStateReportHelper.REPORT_TEMPLATE_NAME);

                var document = new Document();
                document.LoadFromFile(templatePath);

                FillTheFields(reportData, document);

                var tables = document.Sections[0].Tables.Cast<Table>().ToList();
                var table = tables.FirstOrDefault(x => x.Title == QualityStateReportHelper.TABLE_ASSETS_NAME);

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

        private void FillTheFields(WriteOffReportData reportData, Document document)
        {
            var reportConfig = _reportDataService.GetReportConfig(ReportType.QualityStateReport);

            document.ReplaceFields(reportConfig);

            document.ReplaceField(QualityStateReportHelper.FIELD_REGISTRATION_NUMBER, reportData.RegistrationNumber);
            document.ReplaceField(QualityStateReportHelper.FIELD_DOCUMENT_NUMBER, reportData.DocumentNumber);
        }

        private static void FillTheTable(WriteOffReportData reportData, Table table)
        {
            var firstRow = table.LastRow;

            for (int i = 0; i < reportData.Assets.Count; i++)
            {
                var asset = reportData.Assets[i];
                TableRow row = table.AddRow();

                var fontSize = QualityStateReportHelper.TABLE_FONT_SIZE;

                // TODO: optimize. Make a mapper.

                var residualPrice = ResidualPriceHelper.CalculateResidualPriceForItem(asset);
                // TODO: use Initial Category instead
                var initialCategory = ReportHelper.ConvertCategoryToText(2);
                // TODO: calculate depending on AssetState
                var category = ReportHelper.ConvertCategoryToText(asset.Category);
                
                var assetName = ReportHelper.GetFullAssetName(asset.Name, asset.SerialNumber);
                var nomenclatureCode = asset.NomenclatureCode?.ToUpper() ?? string.Empty;
                var monthsOperated = (int)((reportData.ReportDate - asset.StartDate).TotalDays / 30);

                row.Cells[QualityStateReportHelper.COLUMN_INDEX].AddNumber(i + 1, fontSize);
                row.Cells[QualityStateReportHelper.COLUMN_NAME].AddText(assetName, fontSize, HorizontalAlignment.Left);
                row.Cells[QualityStateReportHelper.COLUMN_NOMENCLATURE_CODE].AddText(nomenclatureCode, fontSize);
                row.Cells[QualityStateReportHelper.COLUMN_MEASUREMENT_UNIT].AddText(asset.MeasurementUnit, fontSize);
                row.Cells[QualityStateReportHelper.COLUMN_CATEGORY].AddText(initialCategory, fontSize);
                row.Cells[QualityStateReportHelper.COLUMN_COUNT].AddNumber(asset.Count, fontSize);
                row.Cells[QualityStateReportHelper.COLUMN_PRICE].AddPrice(asset.Price, fontSize);
                row.Cells[QualityStateReportHelper.COLUMN_TOTAL_PRICE].AddPrice(asset.Count * asset.Price, fontSize);
                row.Cells[QualityStateReportHelper.COLUMN_EXPLOITATION_FACT].AddNumber(monthsOperated);
                row.Cells[QualityStateReportHelper.COLUMN_EXPLOITATION_NORM].AddNumber(60); // ???
                row.Cells[QualityStateReportHelper.COLUMN_EXPLOITATION_NAME].AddText(assetName, fontSize, HorizontalAlignment.Left);
                row.Cells[QualityStateReportHelper.COLUMN_EXPLOITATION_NOMENCLATURE_CODE].AddText(nomenclatureCode, fontSize);
                row.Cells[QualityStateReportHelper.COLUMN_EXPLOITATION_MEASUREMENT_UNIT].AddText(asset.MeasurementUnit, fontSize);
                row.Cells[QualityStateReportHelper.COLUMN_EXPLOITATION_CATEGORY].AddText(category, fontSize);
                row.Cells[QualityStateReportHelper.COLUMN_EXPLOITATION_COUNT].AddNumber(asset.Count, fontSize);
                row.Cells[QualityStateReportHelper.COLUMN_EXPLOITATION_PRICE].AddPrice(residualPrice, fontSize);
                row.Cells[QualityStateReportHelper.COLUMN_EXPLOITATION_TOTAL_PRICE].AddPrice(residualPrice * asset.Count, fontSize);
            }

            table.Rows.Remove(firstRow);

            AddSummaryRow(reportData, table);
        }

        private static void AddSummaryRow(WriteOffReportData reportData, Table table)
        {
            var totalSumClear = ResidualPriceHelper.CalculateTotalReportSum(reportData, false);
            var totalSum = ResidualPriceHelper.CalculateTotalReportSum(reportData, true);

            var totalSumText = ReportHelper.ConvertTotalSumToUkrainianString(totalSum);
            var totalItemsText = ReportHelper.ConvertNamesNumberToReportString(reportData.Assets.Count);

            // last united string row 
            var textSummaryRow = table.AddRow(false);
            var summaryCell = textSummaryRow.CreateMergedCell(0, textSummaryRow.Cells.Count);
            var summaryText = string.Format(QualityStateReportHelper.TOTAL_TEXT_FORMAT, totalItemsText, totalSumText);
            textSummaryRow.Cells[0].AddText(summaryText, QualityStateReportHelper.TABLE_FONT_SIZE, HorizontalAlignment.Left);
        }
    }
}
