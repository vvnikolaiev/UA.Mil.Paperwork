using Mil.Paperwork.Domain.DataModels.Parameters;
using Mil.Paperwork.Domain.Helpers;
using Mil.Paperwork.Infrastructure.DataModels;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Services;

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

                using var document = WordDocument.LoadFromFile(templatePath);

                FillCommission(document);
                FillTheFields(assetValuationData, document);

                var table = document.GetTable(ValuationReportHelper.TABLE_ASSET_NAME);
                if (table != null)
                    FillAssetComponentsTable(assetValuationData, table);

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
            var dictCommissionFields = ReportParametersHelper.GetCommission(ReportType.AssetValuationReport, _reportDataService);
            document.ReplaceFields(dictCommissionFields);
        }

        private void FillTheFields(IAssetValuationData assetValuationData, WordDocument document)
        {
            var reportConfig = ReportParametersHelper.GetFullParametersDictionary(ReportType.AssetValuationReport, _reportDataService);
            var assetName = ReportHelper.GetFullAssetName(assetValuationData.Name, assetValuationData.SerialNumber);
            var valuationDate = assetValuationData.ValuationDate.ToString(ReportHelper.DATE_FORMAT);

            document.ReplaceField(ValuationReportHelper.FIELD_ASSET_NAME, assetName);
            document.ReplaceField(ValuationReportHelper.FIELD_VALUATION_DATE, valuationDate);
            document.ReplaceField(ValuationReportHelper.FIELD_VALUATION_SOURCES, assetValuationData.Description);

            document.ReplaceFields(reportConfig);
        }

        private static void FillAssetComponentsTable(IAssetValuationData assetValuationData, WordTable table)
        {
            var firstRow = table.LastRow;

            var nameCellParameters = new WordCellParameters(ValuationReportHelper.TABLE_FONT_SIZE, WordHorizontalAlignment.Left);
            var cellParameters = new WordCellParameters(ValuationReportHelper.TABLE_FONT_SIZE, WordHorizontalAlignment.Center);

            for (int i = 0; i < assetValuationData.AssetComponentsCount; i++)
            {
                var assetComponent = assetValuationData.AssetComponents[i];
                var row = table.AddRow();

                var nomenclatureCode = assetComponent.NomenclatureCode?.ToUpper() ?? string.Empty;
                var totalPrice = Math.Round(assetComponent.Price * assetComponent.Quantity, 2);

                row.GetCell(ValuationReportHelper.COLUMN_INDEX).AddNumber(i + 1, cellParameters);
                row.GetCell(ValuationReportHelper.COLUMN_NAME).AddText(assetComponent.Name, nameCellParameters);
                row.GetCell(ValuationReportHelper.COLUMN_NOMENCLATURE_CODE).AddText(nomenclatureCode, cellParameters);
                row.GetCell(ValuationReportHelper.COLUMN_MEAS_UNIT).AddText(assetComponent.Unit, cellParameters);
                row.GetCell(ValuationReportHelper.COLUMN_COUNT).AddNumber(assetComponent.Quantity, cellParameters);
                row.GetCell(ValuationReportHelper.COLUMN_PRICE_INITIAL).AddPrice(assetComponent.Price, cellParameters);
                row.GetCell(ValuationReportHelper.COLUMN_PRICE_TOTAL).AddPrice(totalPrice, cellParameters);
            }

            table.RemoveRow(firstRow);

            AddSummaryRow(assetValuationData, table);
        }

        private static void AddSummaryRow(IAssetValuationData assetValuationData, WordTable table)
        {
            var nameCellParameters = new WordCellParameters(ValuationReportHelper.TABLE_FONT_SIZE, WordHorizontalAlignment.Left);
            var totalSum = assetValuationData.AssetComponents.Sum(x => Math.Round(x.Price * x.Quantity, 2));

            var totalSumText = ReportHelper.ConvertTotalSumToUkrainianString(totalSum);
            var totalItemsText = ReportHelper.ConvertNamesNumberToReportString(assetValuationData.AssetComponentsCount);

            var textSummaryRow = table.AddRow();
            textSummaryRow.CreateMergedCell(0, textSummaryRow.CellCount);
            var summaryText = string.Format(ValuationReportHelper.TOTAL_TEXT_FORMAT, totalItemsText, totalSumText);
            textSummaryRow.GetCell(0).AddText(summaryText, nameCellParameters);
        }
    }
}
