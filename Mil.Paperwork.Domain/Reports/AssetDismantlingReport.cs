using Mil.Paperwork.Domain.DataModels.Parameters;
using Mil.Paperwork.Domain.Helpers;
using Mil.Paperwork.Infrastructure.DataModels;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Services;

namespace Mil.Paperwork.Domain.Reports
{
    internal class AssetDismantlingReport : IAssetDismantlingReport
    {
        private readonly IReportDataService _reportDataService;

        private byte[] _reportBytes;

        public AssetDismantlingReport(IReportDataService reportDataService)
        {
            _reportDataService = reportDataService;
        }

        public bool TryCreate(AssetDismantlingData assetDismantlingData)
        {
            try
            {
                var templatePath = PathsHelper.GetTemplatePath(DismantlingReportHelper.REPORT_TEMPLATE_NAME);

                using var document = WordDocument.LoadFromFile(templatePath);

                FillCommission(document);
                FillTheFields(assetDismantlingData, document);

                var table = document.GetTable(DismantlingReportHelper.TABLE_ASSET_CONFIGURATION_NAME);
                if (table != null)
                    FillAssetComponentsTable(assetDismantlingData, table);

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
            var dictCommissionFields = ReportParametersHelper.GetCommission(ReportType.AssetDismantlingReport, _reportDataService);
            document.ReplaceFields(dictCommissionFields);
        }

        private void FillTheFields(AssetDismantlingData assetDismantlingData, WordDocument document)
        {
            var reportConfig = ReportParametersHelper.GetFullParametersDictionary(ReportType.AssetDismantlingReport, _reportDataService);
            var assetName = ReportHelper.GetFullAssetName(assetDismantlingData.Name, assetDismantlingData.SerialNumber);

            var excludedItemsQuantity = assetDismantlingData.AssetComponents.Count(x => x.Exclude);
            var remainsQuantity = assetDismantlingData.AssetComponentsCount - excludedItemsQuantity;
            var remainsRange = string.Format(DismantlingReportHelper.REMAINS_RANGE_TEXT_FORMAT, remainsQuantity);

            document.ReplaceField(DismantlingReportHelper.FIELD_REGISTRATION_NUMBER, assetDismantlingData.RegistrationNumber);
            document.ReplaceField(DismantlingReportHelper.FIELD_DOCUMENT_NUMBER, assetDismantlingData.DocumentNumber);
            document.ReplaceField(DismantlingReportHelper.FIELD_ASSET_NAME, assetName);
            document.ReplaceField(DismantlingReportHelper.FIELD_DISMANTLING_REASON, assetDismantlingData.Reason);
            document.ReplaceField(DismantlingReportHelper.FIELD_REMAINS_RANGE, remainsRange);

            document.ReplaceFields(reportConfig);
        }

        private sealed record ComponentRow(
            string Index, string Name, string NomenclatureCode, string Unit, string Category, int TotalQuantity, decimal ResidualPrice, decimal TotalPrice);

        private static ComponentRow BuildComponentRow(AssetComponent assetComponent, int rowNumber, AssetDismantlingData assetDismantlingData)
        {
            var nomenclatureCode = assetComponent.NomenclatureCode?.ToUpper() ?? string.Empty;
            var componentCategory = ReportHelper.ConvertCategoryToText(assetComponent.Category);
            var totalQuantity = assetComponent.Quantity * assetDismantlingData.Count;
            var totalComponentPrice = Math.Round(assetComponent.Price * totalQuantity, 2);

            var result = new ComponentRow($"1.{rowNumber}", assetComponent.Name, nomenclatureCode, assetComponent.Unit,
                componentCategory, totalQuantity, assetComponent.Price, totalComponentPrice);
            return result;
        }

        private static void FillAssetComponentsTable(AssetDismantlingData assetDismantlingData, WordTable table)
        {
            var fontSize = DismantlingReportHelper.TABLE_FONT_SIZE;
            var nameCellParameters = new WordCellParameters(fontSize, WordHorizontalAlignment.Left, isBold: true);
            var cellParameters = new WordCellParameters(fontSize, WordHorizontalAlignment.Center, isBold: true);

            // excluded last to easily form the remaining components range
            var rows = assetDismantlingData.AssetComponents
                .OrderBy(x => x.Exclude)
                .Select((component, i) => BuildComponentRow(component, i + 1, assetDismantlingData))
                .ToList();

            var columns = new List<(int Index, Action<WordCell, ComponentRow> Write)>
            {
                (DismantlingReportHelper.COLUMN_INDEX, (cell, row) => cell.AddText(row.Index, cellParameters)),
                (DismantlingReportHelper.COLUMN_COMPONENT_NAME, (cell, row) => cell.AddText(row.Name, nameCellParameters)),
                (DismantlingReportHelper.COLUMN_COMPONENT_NOMENCLATURE_CODE, (cell, row) => cell.AddText(row.NomenclatureCode, cellParameters)),
                (DismantlingReportHelper.COLUMN_COMPONENT_MEAS_UNIT, (cell, row) => cell.AddText(row.Unit, cellParameters)),
                (DismantlingReportHelper.COLUMN_COMPONENT_CATEGORY, (cell, row) => cell.AddText(row.Category, cellParameters)),
                (DismantlingReportHelper.COLUMN_COMPONENT_COUNT, (cell, row) => cell.AddNumber(row.TotalQuantity, cellParameters)),
                (DismantlingReportHelper.COLUMN_COMPONENT_RESIDUAL_PRICE, (cell, row) => cell.AddPrice(row.ResidualPrice, cellParameters)),
                (DismantlingReportHelper.COLUMN_COMPONENT_PRICE_TOTAL, (cell, row) => cell.AddPrice(row.TotalPrice, cellParameters)),
            };

            var category = ReportHelper.ConvertCategoryToText(assetDismantlingData.Category);

            // columns OPERATING_YEARS_NORM/OPERATING_YEARS are merged but intentionally left blank (matches prior behavior)
            var verticalMergeColumns = new List<(int Index, Action<WordCell> Write)>
            {
                (DismantlingReportHelper.COLUMN_ASSET_NAME, cell => cell.AddText(assetDismantlingData.Name, nameCellParameters)),
                (DismantlingReportHelper.COLUMN_ASSET_NOMENCLATURE_CODE, cell => cell.AddText(assetDismantlingData.NomenclatureCode, cellParameters)),
                (DismantlingReportHelper.COLUMN_ASSET_MEAS_UNIT, cell => cell.AddText(assetDismantlingData.MeasurementUnit, cellParameters)),
                (DismantlingReportHelper.COLUMN_ASSET_CATEGORY, cell => cell.AddText(category, cellParameters)),
                (DismantlingReportHelper.COLUMN_ASSET_COUNT, cell => cell.AddNumber(assetDismantlingData.Count, cellParameters)),
                (DismantlingReportHelper.COLUMN_ASSET_PRICE, cell => cell.AddPrice(assetDismantlingData.Price, cellParameters)),
                (DismantlingReportHelper.COLUMN_ASSET_PRICE_TOTAL, cell => cell.AddPrice(assetDismantlingData.TotalPrice, cellParameters)),
                (DismantlingReportHelper.COLUMN_ASSET_OPERATING_YEARS_NORM, _ => { }),
                (DismantlingReportHelper.COLUMN_ASSET_OPERATING_YEARS, _ => { }),
            };

            WordTableFiller.Fill(table, rows, columns, verticalMergeColumns, t => AddSummaryRow(assetDismantlingData, t));
        }

        private static void AddSummaryRow(AssetDismantlingData assetDismantlingData, WordTable table)
        {
            var cellParameters = new WordCellParameters(DismantlingReportHelper.TABLE_FONT_SIZE, WordHorizontalAlignment.Left);

            var textSummaryRow = table.AddRow();

            var assetCellsCount = DismantlingReportHelper.COLUMN_ASSET_LAST - DismantlingReportHelper.COLUMN_ASSET_FIRST + 1;
            textSummaryRow.CreateMergedCell(DismantlingReportHelper.COLUMN_ASSET_FIRST, assetCellsCount);

            var totalAssetsPriceText = ReportHelper.ConvertTotalSumToUkrainianString(assetDismantlingData.TotalPrice);
            var totalAssetsText = ReportHelper.ConvertNamesNumberToReportString(1);
            var assetSummaryText = string.Format(DismantlingReportHelper.TOTAL_TEXT_FORMAT, totalAssetsText, totalAssetsPriceText);
            textSummaryRow.GetCell(DismantlingReportHelper.COLUMN_ASSET_FIRST).AddText(assetSummaryText, cellParameters);

            var componentCellsCount = DismantlingReportHelper.COLUMN_COMPONENT_LAST - DismantlingReportHelper.COLUMN_COMPONENT_FIRST + 1;
            textSummaryRow.CreateMergedCell(DismantlingReportHelper.COLUMN_COMPONENT_FIRST, componentCellsCount);

            var totalItemsText = ReportHelper.ConvertNamesNumberToReportString(assetDismantlingData.AssetComponentsCount);
            var componentsSummaryText = string.Format(DismantlingReportHelper.TOTAL_TEXT_FORMAT, totalItemsText, totalAssetsPriceText);
            textSummaryRow.GetCell(DismantlingReportHelper.COLUMN_COMPONENT_FIRST).AddText(componentsSummaryText, cellParameters);
        }
    }
}
