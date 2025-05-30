using Mil.Paperwork.Domain.Helpers;
using Mil.Paperwork.Infrastructure.DataModels;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Services;
using Spire.Doc;
using Spire.Doc.Documents;
using System.IO;

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

                var document = new Document();
                document.LoadFromFile(templatePath, FileFormat.Docx);

                FillTheFields(assetDismantlingData, document);

                var tables = document.Sections[0].Tables.Cast<Table>().ToList();
                var table = tables.FirstOrDefault(x => x.Title == DismantlingReportHelper.TABLE_ASSET_CONFIGURATION_NAME);

                if (table != null)
                {
                    FillAssetComponentsTable(assetDismantlingData, table);
                }

                using var reportStream = new MemoryStream();
                document.SaveToStream(reportStream, FileFormat.Docx);
                _reportBytes = reportStream.ToArray();

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

        private void FillTheFields(AssetDismantlingData assetDismantlingDate, Document document)
        {
            var reportConfig = _reportDataService.GetReportParametersDictionary(ReportType.AssetDismantlingReport);
            var assetName = ReportHelper.GetFullAssetName(assetDismantlingDate.Name, assetDismantlingDate.SerialNumber);
            var valuatuionDate = assetDismantlingDate.ValuationDate.ToString(ReportHelper.DATE_FORMAT);

            var excludedItemsQuantity = assetDismantlingDate.AssetComponents.Count(x => x.Exclude);
            var remainsQuantity = assetDismantlingDate.AssetComponentsCount - excludedItemsQuantity;
            var remainsRange = string.Format(DismantlingReportHelper.REMAINS_RANGE_TEXT_FORMAT, remainsQuantity);

            document.ReplaceField(DismantlingReportHelper.FIELD_REGISTRATION_NUMBER, assetDismantlingDate.RegistrationNumber);
            document.ReplaceField(DismantlingReportHelper.FIELD_DOCUMENT_NUMBER, assetDismantlingDate.DocumentNumber);
            document.ReplaceField(DismantlingReportHelper.FIELD_ASSET_NAME, assetName);
            document.ReplaceField(DismantlingReportHelper.FIELD_DISMANTLING_REASON, assetDismantlingDate.Reason);
            document.ReplaceField(DismantlingReportHelper.FIELD_REMAINS_RANGE, remainsRange);

            document.ReplaceFields(reportConfig);
        }

        private static void FillAssetComponentsTable(AssetDismantlingData assetDismantlingData, Table table)
        {
            var fontSize = DismantlingReportHelper.TABLE_FONT_SIZE;
            var firstRow = table.LastRow;
            var firstRowIndex = firstRow.GetRowIndex();

            // excluded last to easily form the remaining components range
            var components = assetDismantlingData.AssetComponents.OrderBy(x => x.Exclude).ToArray();

            for (int i = 0; i < components.Length; i++)
            {
                var assetComponent = components[i];
                TableRow row = table.AddRow();

                var rowNumber = i + 1;
                var index = $"1.{rowNumber}";
                var nomenclatureCode = assetComponent.NomenclatureCode?.ToUpper() ?? string.Empty;
                var componentCategory = ReportHelper.ConvertCategoryToText(assetComponent.Category);
                
                var totalQuantity = assetComponent.Quantity * assetDismantlingData.Count;
                var totalComponentPrice = Math.Round(assetComponent.Price * totalQuantity, 2);

                row.Cells[DismantlingReportHelper.COLUMN_INDEX].AddText(index, fontSize);
                row.Cells[DismantlingReportHelper.COLUMN_COMPONENT_NAME].AddText(assetComponent.Name, fontSize, horizontalAlignment: HorizontalAlignment.Left);
                row.Cells[DismantlingReportHelper.COLUMN_COMPONENT_NOMENCLATURE_CODE].AddText(nomenclatureCode, fontSize);
                row.Cells[DismantlingReportHelper.COLUMN_COMPONENT_MEAS_UNIT].AddText(assetComponent.Unit, fontSize);
                row.Cells[DismantlingReportHelper.COLUMN_COMPONENT_CATEGORY].AddText(componentCategory, fontSize);
                row.Cells[DismantlingReportHelper.COLUMN_COMPONENT_COUNT].AddNumber(totalQuantity, fontSize);
                row.Cells[DismantlingReportHelper.COLUMN_COMPONENT_RESIDUAL_PRICE].AddPrice(assetComponent.Price, fontSize);
                row.Cells[DismantlingReportHelper.COLUMN_COMPONENT_PRICE_TOTAL].AddPrice(totalComponentPrice, fontSize);
            }

            table.Rows.Remove(firstRow);

            // UNITE rows of the first half of the table

            var lastRow = table.LastRow; // if it doesn't work use first available row with data

            var mergedCells = new Dictionary<int, TableCell>();
            // Merge cells verrtically cause in the Left side of the table we have just 1 item
            for (int i = DismantlingReportHelper.COLUMN_ASSET_FIRST; i <= DismantlingReportHelper.COLUMN_ASSET_LAST; i++)
            {
                var cell = table.MergeCellsVertically(i, firstRowIndex, assetDismantlingData.AssetComponentsCount);
                mergedCells.Add(i, cell);
            }
            
            var category = ReportHelper.ConvertCategoryToText(assetDismantlingData.Category);
            
            mergedCells[DismantlingReportHelper.COLUMN_ASSET_NAME].AddText(assetDismantlingData.Name, fontSize, horizontalAlignment: HorizontalAlignment.Left);
            mergedCells[DismantlingReportHelper.COLUMN_ASSET_NOMENCLATURE_CODE].AddText(assetDismantlingData.NomenclatureCode, fontSize);
            mergedCells[DismantlingReportHelper.COLUMN_ASSET_MEAS_UNIT].AddText(assetDismantlingData.MeasurementUnit, fontSize);
            mergedCells[DismantlingReportHelper.COLUMN_ASSET_CATEGORY].AddText(category, fontSize);
            mergedCells[DismantlingReportHelper.COLUMN_ASSET_COUNT].AddNumber(assetDismantlingData.Count, fontSize);
            mergedCells[DismantlingReportHelper.COLUMN_ASSET_PRICE].AddPrice(assetDismantlingData.Price, fontSize);
            mergedCells[DismantlingReportHelper.COLUMN_ASSET_PRICE_TOTAL].AddPrice(assetDismantlingData.TotalPrice, fontSize);

            AddSummaryRow(assetDismantlingData, table);
        }

        private static void AddSummaryRow(AssetDismantlingData assetDismantlingData, Table table)
        {
            var textSummaryRow = table.AddRow(false);

            // first half of the table (asset itself)
            var assetCellsCount = DismantlingReportHelper.COLUMN_ASSET_LAST - DismantlingReportHelper.COLUMN_ASSET_FIRST + 1;
            var summaryAssetCell = textSummaryRow.CreateMergedCell(DismantlingReportHelper.COLUMN_ASSET_FIRST, assetCellsCount);

            var totalAssetsPriceText = ReportHelper.ConvertTotalSumToUkrainianString(assetDismantlingData.TotalPrice);
            var totalAssetsText = ReportHelper.ConvertNamesNumberToReportString(1);
            var assetSummaryText = string.Format(DismantlingReportHelper.TOTAL_TEXT_FORMAT, totalAssetsText, totalAssetsPriceText);
            textSummaryRow.Cells[DismantlingReportHelper.COLUMN_ASSET_FIRST].AddText(assetSummaryText, DismantlingReportHelper.TABLE_FONT_SIZE, HorizontalAlignment.Left);

            // second part of the table (asset components)
            var componentCellsCount = DismantlingReportHelper.COLUMN_COMPONENT_LAST - DismantlingReportHelper.COLUMN_COMPONENT_FIRST + 1;
            var summaryComponentsCell = textSummaryRow.CreateMergedCell(DismantlingReportHelper.COLUMN_COMPONENT_FIRST, componentCellsCount);

            var totalItemsText = ReportHelper.ConvertNamesNumberToReportString(assetDismantlingData.AssetComponentsCount);
            var componentsSummaryText = string.Format(DismantlingReportHelper.TOTAL_TEXT_FORMAT, totalItemsText, totalAssetsPriceText);
            textSummaryRow.Cells[DismantlingReportHelper.COLUMN_COMPONENT_FIRST].AddText(componentsSummaryText, DismantlingReportHelper.TABLE_FONT_SIZE, HorizontalAlignment.Left);
        }
    }
}
