using Mil.Paperwork.Domain.DataModels.Assets;
using Mil.Paperwork.Domain.DataModels.Parameters;
using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Domain.Helpers;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Services;
using Spire.Doc;
using Spire.Doc.Documents;

namespace Mil.Paperwork.Domain.Reports
{
    internal class Handover23ActReport : IReport
    {
        private readonly IReportDataService _reportDataService;

        private byte[] _reportBytes;

        public Handover23ActReport(IReportDataService reportDataService)
        {
            _reportDataService = reportDataService;
        }

        public bool TryCreate(IHandoverReportData reportData)
        {
            try
            {
                var templatePath = PathsHelper.GetTemplatePath(Handover23ActHelper.REPORT_TEMPLATE_NAME);

                var document = new Document();
                document.LoadFromFile(templatePath);

                FillTheFields(reportData, document);

                var table = document.GetTable(Handover23ActHelper.TABLE_ASSETS_NAME);

                if (table != null)
                {
                    FillAssetsTable(reportData.Assets, table);
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

        private void FillTheFields(IHandoverReportData reportData, Document document)
        {
            var reportConfig = ReportParametersHelper.GetFullParametersDictionary(ReportType.Handover23Act, _reportDataService);

            var sDateStart = reportData.DateStart?.ToString(ReportHelper.DATE_FORMAT) ?? string.Empty;
            var sDateEnd = reportData.DateEnd?.ToString(ReportHelper.DATE_FORMAT) ?? string.Empty;
            var docProps = Handover23ActHelper.GetReasonDocProps(reportData.ReasonDocumentNumber, reportData.ReasonDocumentDate);

            document.ReplaceField(Handover23ActHelper.FIELD_DOC_NUMBER, reportData.DocumentNumber);
            document.ReplaceField(Handover23ActHelper.FIELD_DOC_DATE, reportData.DocumentDate.ToString(ReportHelper.DATE_FORMAT));

            document.ReplaceField(Handover23ActHelper.FIELD_DATE_START, sDateStart);
            document.ReplaceField(Handover23ActHelper.FIELD_DATE_END, sDateEnd);

            document.ReplaceField(Handover23ActHelper.FIELD_SUPPLIER, reportData.Supplier);
            document.ReplaceField(Handover23ActHelper.FIELD_RECEIVER, reportData.Receiver);

            document.ReplaceField(Handover23ActHelper.FIELD_REASON_DOCUMENT_NAME, reportData.ReasonDocumentName);
            document.ReplaceField(Handover23ActHelper.FIELD_REASON_DOCUMENT_PROPS, docProps);

            document.ReplaceField(Handover23ActHelper.FIELD_MATERIALLY_RESPONSIBLE, reportData.PersonResponsible.FullName);
            document.ReplaceField(Handover23ActHelper.FIELD_MATERIALLY_RESPONSIBLE_POSITION, reportData.PersonResponsible?.Position);

            document.ReplaceField(Handover23ActHelper.FIELD_PERSON_RECIPIENT_NAME, reportData.PersonReceiver?.FullName);
            document.ReplaceField(Handover23ActHelper.FIELD_PERSON_RECIPIENT_POSITION, reportData.PersonReceiver?.Position);

            document.ReplaceFields(reportConfig);
        }

        private static void FillAssetsTable(IList<IAssetInfo> assets, Table table)
        {
            var firstRow = table.LastRow;

            var nameCellParameters = new WordCellParameters(Handover23ActHelper.TABLE_FONT_SIZE, HorizontalAlignment.Left);
            var cellParameters = new WordCellParameters(Handover23ActHelper.TABLE_FONT_SIZE, HorizontalAlignment.Center);

            for (int i = 0; i < assets.Count; i++)
            {
                var asset = assets[i];
                TableRow row = table.AddRow();

                var nomenclatureCode = asset.NomenclatureCode?.ToUpper() ?? string.Empty;
                var category = ReportHelper.ConvertCategoryToText(asset.InitialCategory);
                var totalPrice = Math.Round(asset.Price * asset.Count, 2);

                var serialNumber = !string.IsNullOrEmpty(asset.SerialNumber) ? $"s/n: {asset.SerialNumber}" : "-";

                row.Cells[Handover23ActHelper.COLUMN_INDEX].AddNumber(i + 1, cellParameters);
                row.Cells[Handover23ActHelper.COLUMN_ASSET_NAME].AddText(asset.Name, nameCellParameters);
                row.Cells[Handover23ActHelper.COLUMN_NOMENCLATURE_CODE].AddText(nomenclatureCode, cellParameters);
                row.Cells[Handover23ActHelper.COLUMN_BATCH_NUMBER].AddText("-", cellParameters);
                row.Cells[Handover23ActHelper.COLUMN_PRICE].AddPrice(asset.Price, cellParameters);
                row.Cells[Handover23ActHelper.COLUMN_COUNT_OUT].AddNumber(asset.Count, cellParameters);
                row.Cells[Handover23ActHelper.COLUMN_CATEGORY_OUT].AddText(category, cellParameters);
                row.Cells[Handover23ActHelper.COLUMN_COUNT_IN].AddNumber(asset.Count, cellParameters);
                row.Cells[Handover23ActHelper.COLUMN_CATEGORY_IN].AddText(category, cellParameters);
                row.Cells[Handover23ActHelper.COLUMN_TOTAL_PRICE].AddPrice(totalPrice, cellParameters);
                row.Cells[Handover23ActHelper.COLUMN_WEAR_N_TEAR].AddText("-", cellParameters);
                row.Cells[Handover23ActHelper.COLUMN_WEAR_N_TEAR_TOTAL].AddText("-", cellParameters);
                row.Cells[Handover23ActHelper.COLUMN_YEAR_MANUFACTURED].AddPrice(asset.YearManufactured, cellParameters);
                row.Cells[Handover23ActHelper.COLUMN_SERIAL_NUMBER].AddText(serialNumber, cellParameters);
                row.Cells[Handover23ActHelper.COLUMN_PASSPORT_NUMBER].AddText("-", cellParameters);
            }

            table.Rows.Remove(firstRow);

            var totalCount = GetTotalCount(assets);
            var totalSum = GetTotalSum(assets);

            AddSummaryRow(totalSum, totalCount, table);
        }

        private static void AddSummaryRow(decimal totalSum, int totalCount, Table table)
        {
            var nameCellParameters = new WordCellParameters(Handover23ActHelper.TABLE_FONT_SIZE, HorizontalAlignment.Left);
            var cellParameters = new WordCellParameters(Handover23ActHelper.TABLE_FONT_SIZE, HorizontalAlignment.Center);

            var textSummaryRow = table.AddRow(true);
            var countMergedColumns = Handover23ActHelper.COLUMN_COUNT_IN - Handover23ActHelper.COLUMN_INDEX;
            var summaryCell = textSummaryRow.CreateMergedCell(Handover23ActHelper.COLUMN_INDEX, countMergedColumns);

            textSummaryRow.Cells[0].AddText(Handover23ActHelper.SummaryRowTotalText, nameCellParameters);
            textSummaryRow.Cells[Handover23ActHelper.COLUMN_COUNT_IN].AddNumber(totalCount, cellParameters);
            textSummaryRow.Cells[Handover23ActHelper.COLUMN_COUNT_OUT].AddNumber(totalCount, cellParameters);
            textSummaryRow.Cells[Handover23ActHelper.COLUMN_TOTAL_PRICE].AddPrice(totalSum, cellParameters);
        }

        private static decimal GetTotalSum(IList<IAssetInfo> assets)
        {
            return Math.Round(assets.Select(x => x.Price * x.Count).Sum(), 2);
        }

        private static int GetTotalCount(IList<IAssetInfo> assets)
        {
            return assets.Select(x => x.Count).Sum();
        }
    }
}
