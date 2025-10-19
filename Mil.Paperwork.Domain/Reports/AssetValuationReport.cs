using Mil.Paperwork.Domain.Helpers;
using Mil.Paperwork.Infrastructure.DataModels;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Services;
using Spire.Doc;
using Spire.Doc.Documents;
using System.IO;

namespace Mil.Paperwork.Domain.Reports
{
    internal class AssetValuationReport : IAssetValuationReport
    {
        private readonly IReportDataService _reportDataService;

        private byte[] _reportBytes;

        public AssetValuationReport(IReportDataService reportDataService)
        {
            _reportDataService = reportDataService;
        }

        public bool TryCreate(IAssetValuationData assetValuationData)
        {
            try
            {
                var templatePath = PathsHelper.GetTemplatePath(ValuationReportHelper.REPORT_TEMPLATE_NAME);

                var document = new Document();
                document.LoadFromFile(templatePath, FileFormat.Docx);

                FillTheFields(assetValuationData, document);

                var table = document.GetTable(ValuationReportHelper.TABLE_ASSET_NAME);

                if (table != null)
                {
                    FillAssetComponentsTable(assetValuationData, table);
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

        private void FillTheFields(IAssetValuationData assetValuationData, Document document)
        {
            var reportConfig = _reportDataService.GetReportParametersDictionary(ReportType.AssetValuationReport);
            var assetName = ReportHelper.GetFullAssetName(assetValuationData.Name, assetValuationData.SerialNumber);
            var valuatuionDate = assetValuationData.ValuationDate.ToString(ReportHelper.DATE_FORMAT);

            document.ReplaceField(ValuationReportHelper.FIELD_ASSET_NAME, assetName);
            document.ReplaceField(ValuationReportHelper.FIELD_VALUATION_DATE, valuatuionDate);
            document.ReplaceField(ValuationReportHelper.FIELD_VALUATION_SOURCES, assetValuationData.Description);

            document.ReplaceFields(reportConfig);
        }

        private static void FillAssetComponentsTable(IAssetValuationData assetValuationData, Table table)
        {
            var firstRow = table.LastRow;

            for (int i = 0; i < assetValuationData.AssetComponentsCount; i++)
            {
                var assetComponent = assetValuationData.AssetComponents[i];
                TableRow row = table.AddRow();

                var fontSize = ValuationReportHelper.TABLE_FONT_SIZE;

                var nomenclatureCode = assetComponent.NomenclatureCode?.ToUpper() ?? string.Empty;
                var totalPrice = Math.Round(assetComponent.Price * assetComponent.Quantity, 2);

                row.Cells[ValuationReportHelper.COLUMN_INDEX].AddNumber(i + 1, fontSize);
                row.Cells[ValuationReportHelper.COLUMN_NAME].AddText(assetComponent.Name, fontSize, horizontalAlignment: HorizontalAlignment.Left);
                row.Cells[ValuationReportHelper.COLUMN_NOMENCLATURE_CODE].AddText(nomenclatureCode, fontSize);
                row.Cells[ValuationReportHelper.COLUMN_MEAS_UNIT].AddText(assetComponent.Unit, fontSize);
                row.Cells[ValuationReportHelper.COLUMN_COUNT].AddNumber(assetComponent.Quantity, fontSize);
                row.Cells[ValuationReportHelper.COLUMN_PRICE_INITIAL].AddPrice(assetComponent.Price, fontSize);
                row.Cells[ValuationReportHelper.COLUMN_PRICE_TOTAL].AddPrice(totalPrice, fontSize);
            }

            table.Rows.Remove(firstRow);

            AddSummaryRow(assetValuationData, table);
        }

        private static void AddSummaryRow(IAssetValuationData assetValuationData, Table table)
        {
            var totalSum = assetValuationData.AssetComponents.Sum(x => Math.Round(x.Price * x.Quantity, 2));

            var totalSumText = ReportHelper.ConvertTotalSumToUkrainianString(totalSum);
            var totalItemsText = ReportHelper.ConvertNamesNumberToReportString(assetValuationData.AssetComponentsCount);

            // last united string row 
            var textSummaryRow = table.AddRow(false);
            var summaryCell = textSummaryRow.CreateMergedCell(0, textSummaryRow.Cells.Count);
            var summaryText = string.Format(ValuationReportHelper.TOTAL_TEXT_FORMAT, totalItemsText, totalSumText);
            textSummaryRow.Cells[0].AddText(summaryText, ValuationReportHelper.TABLE_FONT_SIZE, HorizontalAlignment.Left);
        }
    }
}
