using Mil.Paperwork.Domain.DataModels.Assets;
using Mil.Paperwork.Domain.DataModels.Parameters;
using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Domain.Helpers;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Services;

namespace Mil.Paperwork.Domain.Reports
{
    internal class InvoiceReport : IReport
    {
        private readonly IReportDataService _reportDataService;

        private byte[] _reportBytes;

        public InvoiceReport(IReportDataService reportDataService)
        {
            _reportDataService = reportDataService;
        }

        public bool TryCreate(IInvoceReportData reportData)
        {
            try
            {
                var templatePath = PathsHelper.GetTemplatePath(InvoiceReportHelper.REPORT_TEMPLATE_NAME);

                using var document = WordDocument.LoadFromFile(templatePath);

                FillTheFields(reportData, document);

                var table = document.GetTable(InvoiceReportHelper.TABLE_ASSETS_NAME);
                if (table != null)
                    FillAssetsTable(reportData.Assets, table);

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

        private void FillTheFields(IInvoceReportData reportData, WordDocument document)
        {
            var reportConfig = ReportParametersHelper.GetFullParametersDictionary(ReportType.Invoice, _reportDataService);

            document.ReplaceField(InvoiceReportHelper.FIELD_DOCUMENT_NUMBER, reportData.DocumentNumber);
            document.ReplaceField(InvoiceReportHelper.FIELD_DUE_DAY, reportData.DueDate.ToString("dd"));
            document.ReplaceField(InvoiceReportHelper.FIELD_DUE_MONTH, reportData.DueDate.ToString("MM"));
            document.ReplaceField(InvoiceReportHelper.FIELD_DUE_YEAR, reportData.DueDate.ToString("yyyy"));
            document.ReplaceField(InvoiceReportHelper.FIELD_DATE_DAY, reportData.DateCreated.ToString("dd"));
            document.ReplaceField(InvoiceReportHelper.FIELD_DATE_MONTH, reportData.DateCreated.ToString("MM"));
            document.ReplaceField(InvoiceReportHelper.FIELD_DATE_YEAR, reportData.DateCreated.ToString("yyyy"));
            document.ReplaceField(InvoiceReportHelper.FIELD_DATE, reportData.DateCreated.ToString(ReportHelper.DATE_FORMAT));
            document.ReplaceField(InvoiceReportHelper.FIELD_REASON, reportData.Reason);
            document.ReplaceField(InvoiceReportHelper.FIELD_RECIPIENT_SURNAME, reportData.Recipient?.LastName);
            document.ReplaceField(InvoiceReportHelper.FIELD_PERSON_TRANSMITTER, reportData.Transmitter?.FullName);
            document.ReplaceField(InvoiceReportHelper.FIELD_PERSON_TRANSMITTER_RANK, reportData.Transmitter?.Rank);
            document.ReplaceField(InvoiceReportHelper.FIELD_PERSON_TRANSMITTER_POSITION, reportData.Transmitter?.Position);
            document.ReplaceField(InvoiceReportHelper.FIELD_PERSON_RECIPIENT, reportData.Recipient?.FullName);
            document.ReplaceField(InvoiceReportHelper.FIELD_PERSON_RECIPIENT_RANK, reportData.Recipient?.Rank);
            document.ReplaceField(InvoiceReportHelper.FIELD_PERSON_RECIPIENT_POSITION, reportData.Recipient?.Position);

            var totalCount = GetTotalCount(reportData.Assets);
            var totalSum = GetTotalSum(reportData.Assets);
            var totalSumRouded = Math.Truncate(totalSum);
            var totalCents = Math.Round((totalSum - totalSumRouded) * 100, 0);

            document.ReplaceField(InvoiceReportHelper.FIELD_ASSETS_COUNT_TEXT, ReportHelper.ConvertNumberToWords(totalCount, NounGender.Feminine));
            document.ReplaceField(InvoiceReportHelper.FIELD_UNITS_TEXT, ReportHelper.GetUnitsText(totalCount));
            document.ReplaceField(InvoiceReportHelper.FIELD_TOTAL_SUM_TEXT, ReportHelper.ConvertNumberToWords((int)totalSumRouded, NounGender.Feminine));
            document.ReplaceField(InvoiceReportHelper.FIELD_CENTS, totalCents.ToString());

            document.ReplaceFields(reportConfig);
        }

        private static void FillAssetsTable(IList<IAssetInfo> assets, WordTable table)
        {
            var firstRow = table.LastRow;

            var nameCellParameters = new WordCellParameters(InvoiceReportHelper.TABLE_FONT_SIZE, WordHorizontalAlignment.Left);
            var cellParameters = new WordCellParameters(InvoiceReportHelper.TABLE_FONT_SIZE, WordHorizontalAlignment.Center);

            for (int i = 0; i < assets.Count; i++)
            {
                var asset = assets[i];
                var row = table.AddRow();

                var nomenclatureCode = asset.NomenclatureCode?.ToUpper() ?? string.Empty;
                var category = ReportHelper.ConvertCategoryToText(asset.InitialCategory);
                var totalPrice = Math.Round(asset.Price * asset.Count, 2);
                var serialNumber = !string.IsNullOrEmpty(asset.SerialNumber) ? $"s/n: {asset.SerialNumber}" : "-";

                row.GetCell(InvoiceReportHelper.COLUMN_INDEX).AddNumber(i + 1, cellParameters);
                row.GetCell(InvoiceReportHelper.COLUMN_ASSET_NAME).AddText(asset.Name, nameCellParameters);
                row.GetCell(InvoiceReportHelper.COLUMN_NOMENCLATURE_CODE).AddText(nomenclatureCode, cellParameters);
                row.GetCell(InvoiceReportHelper.COLUMN_MEASUREMENT_UNIT).AddText(asset.MeasurementUnit, cellParameters);
                row.GetCell(InvoiceReportHelper.COLUMN_CATEGORY).AddText(category, cellParameters);
                row.GetCell(InvoiceReportHelper.COLUMN_PRICE).AddPrice(asset.Price, cellParameters);
                row.GetCell(InvoiceReportHelper.COLUMN_COUNT_OUT).AddNumber(asset.Count, cellParameters);
                row.GetCell(InvoiceReportHelper.COLUMN_COUNT_IN).AddNumber(asset.Count, cellParameters);
                row.GetCell(InvoiceReportHelper.COLUMN_TOTAL_PRICE).AddPrice(totalPrice, cellParameters);
                row.GetCell(InvoiceReportHelper.COLUMN_NOTE).AddText(serialNumber, cellParameters);
            }

            table.RemoveRow(firstRow);

            AddSummaryRow(GetTotalSum(assets), GetTotalCount(assets), table);
        }

        private static void AddSummaryRow(decimal totalSum, int totalCount, WordTable table)
        {
            var nameCellParameters = new WordCellParameters(InvoiceReportHelper.TABLE_FONT_SIZE, WordHorizontalAlignment.Left);
            var cellParameters = new WordCellParameters(InvoiceReportHelper.TABLE_FONT_SIZE, WordHorizontalAlignment.Center);

            var textSummaryRow = table.AddRow();
            var countMergedColumns = InvoiceReportHelper.COLUMN_COUNT_IN - InvoiceReportHelper.COLUMN_INDEX;
            textSummaryRow.CreateMergedCell(InvoiceReportHelper.COLUMN_INDEX, countMergedColumns);

            textSummaryRow.GetCell(0).AddText(InvoiceReportHelper.SummaryRowTotalText, nameCellParameters);
            textSummaryRow.GetCell(InvoiceReportHelper.COLUMN_COUNT_IN).AddNumber(totalCount, cellParameters);
            textSummaryRow.GetCell(InvoiceReportHelper.COLUMN_COUNT_OUT).AddNumber(totalCount, cellParameters);
            textSummaryRow.GetCell(InvoiceReportHelper.COLUMN_TOTAL_PRICE).AddPrice(totalSum, cellParameters);
        }

        private static decimal GetTotalSum(IList<IAssetInfo> assets)
            => Math.Round(assets.Select(x => x.Price * x.Count).Sum(), 2);

        private static int GetTotalCount(IList<IAssetInfo> assets)
            => assets.Select(x => x.Count).Sum();
    }
}
