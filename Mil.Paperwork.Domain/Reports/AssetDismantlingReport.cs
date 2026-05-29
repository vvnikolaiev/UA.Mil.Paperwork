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

        private static void FillAssetComponentsTable(AssetDismantlingData assetDismantlingData, WordTable table)
        {
            var fontSize = DismantlingReportHelper.TABLE_FONT_SIZE;
            var nameCellParameters = new WordCellParameters(fontSize, WordHorizontalAlignment.Left, isBold: true);
            var cellParameters = new WordCellParameters(fontSize, WordHorizontalAlignment.Center, isBold: true);

            var firstRow = table.LastRow;
            var firstRowIndex = firstRow.GetRowIndex();

            // excluded last to easily form the remaining components range
            var components = assetDismantlingData.AssetComponents.OrderBy(x => x.Exclude).ToArray();

            for (int i = 0; i < components.Length; i++)
            {
                var assetComponent = components[i];
                var row = table.AddRow();

                var rowNumber = i + 1;
                var index = $"1.{rowNumber}";
                var nomenclatureCode = assetComponent.NomenclatureCode?.ToUpper() ?? string.Empty;
                var componentCategory = ReportHelper.ConvertCategoryToText(assetComponent.Category);
                var totalQuantity = assetComponent.Quantity * assetDismantlingData.Count;
                var totalComponentPrice = Math.Round(assetComponent.Price * totalQuantity, 2);

                row.GetCell(DismantlingReportHelper.COLUMN_INDEX).AddText(index, cellParameters);
                row.GetCell(DismantlingReportHelper.COLUMN_COMPONENT_NAME).AddText(assetComponent.Name, nameCellParameters);
                row.GetCell(DismantlingReportHelper.COLUMN_COMPONENT_NOMENCLATURE_CODE).AddText(nomenclatureCode, cellParameters);
                row.GetCell(DismantlingReportHelper.COLUMN_COMPONENT_MEAS_UNIT).AddText(assetComponent.Unit, cellParameters);
                row.GetCell(DismantlingReportHelper.COLUMN_COMPONENT_CATEGORY).AddText(componentCategory, cellParameters);
                row.GetCell(DismantlingReportHelper.COLUMN_COMPONENT_COUNT).AddNumber(totalQuantity, cellParameters);
                row.GetCell(DismantlingReportHelper.COLUMN_COMPONENT_RESIDUAL_PRICE).AddPrice(assetComponent.Price, cellParameters);
                row.GetCell(DismantlingReportHelper.COLUMN_COMPONENT_PRICE_TOTAL).AddPrice(totalComponentPrice, cellParameters);
            }

            table.RemoveRow(firstRow);

            var mergedCells = new Dictionary<int, WordCell>();
            for (int i = DismantlingReportHelper.COLUMN_ASSET_FIRST; i <= DismantlingReportHelper.COLUMN_ASSET_LAST; i++)
            {
                var cell = table.MergeCellsVertically(i, firstRowIndex, assetDismantlingData.AssetComponentsCount);
                mergedCells.Add(i, cell);
            }

            var category = ReportHelper.ConvertCategoryToText(assetDismantlingData.Category);

            mergedCells[DismantlingReportHelper.COLUMN_ASSET_NAME].AddText(assetDismantlingData.Name, nameCellParameters);
            mergedCells[DismantlingReportHelper.COLUMN_ASSET_NOMENCLATURE_CODE].AddText(assetDismantlingData.NomenclatureCode, cellParameters);
            mergedCells[DismantlingReportHelper.COLUMN_ASSET_MEAS_UNIT].AddText(assetDismantlingData.MeasurementUnit, cellParameters);
            mergedCells[DismantlingReportHelper.COLUMN_ASSET_CATEGORY].AddText(category, cellParameters);
            mergedCells[DismantlingReportHelper.COLUMN_ASSET_COUNT].AddNumber(assetDismantlingData.Count, cellParameters);
            mergedCells[DismantlingReportHelper.COLUMN_ASSET_PRICE].AddPrice(assetDismantlingData.Price, cellParameters);
            mergedCells[DismantlingReportHelper.COLUMN_ASSET_PRICE_TOTAL].AddPrice(assetDismantlingData.TotalPrice, cellParameters);

            AddSummaryRow(assetDismantlingData, table);
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
