using Mil.Paperwork.Domain.DataModels.Assets;
using Mil.Paperwork.Domain.DataModels.Parameters;
using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Domain.Helpers;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Services;

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

                using var document = WordDocument.LoadFromFile(templatePath);

                FillCommission(document);
                FillTheFields(reportData, document);

                var table = document.GetTable(QualityStateReportHelper.TABLE_ASSETS_NAME);
                if (table != null)
                    FillTheTable(reportData, table);

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

        private void FillCommission(WordDocument document)
        {
            var dictCommissionFields = ReportParametersHelper.GetCommission(ReportType.QualityStateReport, _reportDataService);
            document.ReplaceFields(dictCommissionFields);
        }

        private void FillTheFields(ICommonWriteOffReportData reportData, WordDocument document)
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

        private sealed record AssetRow(
            int Index, string AssetName, string NomenclatureCode, string MeasurementUnit, string InitialCategory,
            int Count, decimal Price, decimal TotalPrice, int MonthsOperated, int ExploitationNorm,
            string ResidualCategory, decimal ResidualPrice, decimal ResidualTotalPrice);

        private static AssetRow BuildAssetRow(IAssetInfo asset, int index, ICommonWriteOffReportData reportData)
        {
            var residualPrice = ResidualPriceHelper.CalculateResidualPriceForItem(asset, reportData.EventDate);
            var initialCategory = ReportHelper.ConvertCategoryToText(asset.InitialCategory);
            var residualCategory = ReportHelper.ConvertEventTypeToCategoryText(asset.InitialCategory, reportData.EventType);
            var assetName = ReportHelper.GetFullAssetName(asset.Name, asset.SerialNumber);
            var nomenclatureCode = asset.NomenclatureCode?.ToUpper() ?? string.Empty;
            var monthsOperated = (int)((reportData.EventDate - asset.StartDate).TotalDays / 30);
            var exploitationNorm = asset.ResourceYears != 0 ? asset.ResourceYears * 12 : 60;

            var result = new AssetRow(index + 1, assetName, nomenclatureCode, asset.MeasurementUnit, initialCategory,
                asset.Count, asset.Price, asset.Count * asset.Price, monthsOperated, exploitationNorm,
                residualCategory, residualPrice, residualPrice * asset.Count);
            return result;
        }

        private static void FillTheTable(ICommonWriteOffReportData reportData, WordTable table)
        {
            var nameCellParameters = new WordCellParameters(QualityStateReportHelper.TABLE_FONT_SIZE, WordHorizontalAlignment.Left);
            var cellParameters = new WordCellParameters(QualityStateReportHelper.TABLE_FONT_SIZE, WordHorizontalAlignment.Center);

            var rows = reportData.Assets
                .Select((asset, i) => BuildAssetRow(asset, i, reportData))
                .ToList();

            var columns = new List<(int Index, Action<WordCell, AssetRow> Write)>
            {
                (QualityStateReportHelper.COLUMN_INDEX, (cell, row) => cell.AddNumber(row.Index, cellParameters)),
                (QualityStateReportHelper.COLUMN_NAME, (cell, row) => cell.AddText(row.AssetName, nameCellParameters)),
                (QualityStateReportHelper.COLUMN_NOMENCLATURE_CODE, (cell, row) => cell.AddText(row.NomenclatureCode, cellParameters)),
                (QualityStateReportHelper.COLUMN_MEASUREMENT_UNIT, (cell, row) => cell.AddText(row.MeasurementUnit, cellParameters)),
                (QualityStateReportHelper.COLUMN_CATEGORY, (cell, row) => cell.AddText(row.InitialCategory, cellParameters)),
                (QualityStateReportHelper.COLUMN_COUNT, (cell, row) => cell.AddNumber(row.Count, cellParameters)),
                (QualityStateReportHelper.COLUMN_PRICE, (cell, row) => cell.AddPrice(row.Price, cellParameters)),
                (QualityStateReportHelper.COLUMN_TOTAL_PRICE, (cell, row) => cell.AddPrice(row.TotalPrice, cellParameters)),
                (QualityStateReportHelper.COLUMN_EXPLOITATION_FACT, (cell, row) => cell.AddNumber(row.MonthsOperated, cellParameters)),
                (QualityStateReportHelper.COLUMN_EXPLOITATION_NORM, (cell, row) => cell.AddNumber(row.ExploitationNorm, cellParameters)),
                (QualityStateReportHelper.COLUMN_EXPLOITATION_NAME, (cell, row) => cell.AddText(row.AssetName, nameCellParameters)),
                (QualityStateReportHelper.COLUMN_EXPLOITATION_NOMENCLATURE_CODE, (cell, row) => cell.AddText(row.NomenclatureCode, cellParameters)),
                (QualityStateReportHelper.COLUMN_EXPLOITATION_MEASUREMENT_UNIT, (cell, row) => cell.AddText(row.MeasurementUnit, cellParameters)),
                (QualityStateReportHelper.COLUMN_EXPLOITATION_CATEGORY, (cell, row) => cell.AddText(row.ResidualCategory, cellParameters)),
                (QualityStateReportHelper.COLUMN_EXPLOITATION_COUNT, (cell, row) => cell.AddNumber(row.Count, cellParameters)),
                (QualityStateReportHelper.COLUMN_EXPLOITATION_PRICE, (cell, row) => cell.AddPrice(row.ResidualPrice, cellParameters)),
                (QualityStateReportHelper.COLUMN_EXPLOITATION_TOTAL_PRICE, (cell, row) => cell.AddPrice(row.ResidualTotalPrice, cellParameters)),
            };

            WordTableFiller.Fill(table, rows, columns, addSummaryRow: t => AddSummaryRow(reportData, t));
        }

        private static void AddSummaryRow(ICommonWriteOffReportData reportData, WordTable table)
        {
            var cellParameters = new WordCellParameters(QualityStateReportHelper.TABLE_FONT_SIZE, WordHorizontalAlignment.Left);
            var totalSum = ResidualPriceHelper.CalculateTotalReportSum(reportData.Assets, reportData.EventDate, true);

            var totalSumText = ReportHelper.ConvertTotalSumToUkrainianString(totalSum);
            var totalItemsText = ReportHelper.ConvertNamesNumberToReportString(reportData.Assets.Count);

            var textSummaryRow = table.AddRow();
            textSummaryRow.CreateMergedCell(0, textSummaryRow.CellCount);
            var summaryText = string.Format(QualityStateReportHelper.TOTAL_TEXT_FORMAT, totalItemsText, totalSumText);
            textSummaryRow.GetCell(0).AddText(summaryText, cellParameters);
        }
    }
}
