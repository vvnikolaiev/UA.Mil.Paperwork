using Mil.Paperwork.Domain.Calculators;
using Mil.Paperwork.Domain.DataModels.Parameters;
using Mil.Paperwork.Domain.Helpers;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Services;
using Spire.Doc;
using Spire.Doc.Documents;

namespace Mil.Paperwork.Domain.Reports.WriteOff
{
    internal class BookOfLossesExtract : WriteOffPackageReport
    {
        private const string SummaryRowTotalText = "ВСЬОГО:";
        private readonly IReportDataService _reportDataService;
        private string _milUnit = string.Empty;

        public override string OutputFileName => WriteOffPackageTemplatesHelper.OUTPUT_BOOK_OF_LOSSES_NAME;

        protected override string TemplatePath => PathsHelper.GetTemplatePath(WriteOffPackageTemplatesHelper.EXTRACT_FROM_BOOK_OF_LOSSES_TEMPLATE_NAME);

        public BookOfLossesExtract(IReportDataService reportDataService)
        {
            _reportDataService = reportDataService;
        }

        protected override void FillReportData(IWriteOffPackageParameters reportParameters, Document document)
        {
            FillTheFields(reportParameters, document);
            FillTOCTable(reportParameters, document);
        }

        private void FillTheFields(IWriteOffPackageParameters reportParameters, Document document)
        {
            var reportConfig = ReportParametersHelper.GetFullParametersDictionary(ReportType.WriteOffPackage, _reportDataService);

            var extractData = reportParameters.BookOfLossesExtractData;

            _milUnit = reportConfig.GetValueOrDefault(WriteOffPackageTemplatesHelper.FIELD_BOOK_MIL_UNIT, string.Empty);

            document.ReplaceField(WriteOffPackageTemplatesHelper.FIELD_BOOK_NUM, extractData.Number.ToString());
            document.ReplaceField(WriteOffPackageTemplatesHelper.FIELD_BOOK_YEAR, extractData.Year.ToString());
            document.ReplaceField(WriteOffPackageTemplatesHelper.FIELD_PAGE_NUM, extractData.PageNumber.ToString());
            document.ReplaceField(WriteOffPackageTemplatesHelper.FIELD_RECORD_DATE, extractData.RecordDate.ToString(ReportHelper.DATE_FORMAT));
            document.ReplaceField(WriteOffPackageTemplatesHelper.FIELD_TOTAL_WRITE_OFF_SUM, ReportHelper.GetPriceString(reportParameters.TotalWriteOffSum));
            document.ReplaceField(WriteOffPackageTemplatesHelper.FIELD_ORDEN_NUM, reportParameters.OrdenNumber.ToString());
            document.ReplaceField(WriteOffPackageTemplatesHelper.FIELD_ORDEN_DATE, reportParameters.OrdenDate.ToString(ReportHelper.DATE_FORMAT));

            document.ReplaceFields(reportConfig);
        }

        private void FillTOCTable(IWriteOffPackageParameters parameters, Document document)
        {
            var table = document.GetTable(WriteOffPackageTemplatesHelper.TABLE_BOOK_RECORD_NAME);

            if (table != null)
            {
                var firstRow = table.LastRow;
                var firstRowIndex = firstRow.GetRowIndex();

                var nameCellParameters = new WordCellParameters(WriteOffPackageTemplatesHelper.TABLE_BOOK_RECORD_FONT_SIZE, HorizontalAlignment.Left, isBold: true);
                var cellParameters = new WordCellParameters(WriteOffPackageTemplatesHelper.TABLE_BOOK_RECORD_FONT_SIZE, HorizontalAlignment.Center, isBold: true);

                for (int i = 0; i < parameters.Assets.Count; i++)
                {
                    var asset = parameters.Assets[i];
                    var calculator = ResidualPriceCalculatorFactory.CreateCalculator(asset.Service);

                    TableRow row = table.AddRow(true);

                    var nomenclatureCode = asset.NomenclatureCode?.ToUpper() ?? string.Empty;
                    var category = ReportHelper.ConvertCategoryToText(asset.InitialCategory);
                    var price = ResidualPriceHelper.CalculateResidualPriceForItem(asset, parameters.EventDate);
                    var totalPrice = Math.Round(price * asset.Count, 2);

                    var name = ReportHelper.GetFullAssetName(asset.Name, asset.SerialNumber);

                    row.Cells[WriteOffPackageTemplatesHelper.TABLE_BOOK_COLUMN_ASSET_NAME].AddText(name, nameCellParameters);
                    row.Cells[WriteOffPackageTemplatesHelper.TABLE_BOOK_COLUMN_ASSET_UNIT].AddText(asset.MeasurementUnit, cellParameters);
                    row.Cells[WriteOffPackageTemplatesHelper.TABLE_BOOK_COLUMN_ASSET_COUNT].AddNumber(asset.Count, cellParameters);
                    row.Cells[WriteOffPackageTemplatesHelper.TABLE_BOOK_COLUMN_ASSET_PRICE].AddPrice(price, cellParameters);
                    row.Cells[WriteOffPackageTemplatesHelper.TABLE_BOOK_COLUMN_ASSET_SUM].AddPrice(totalPrice, cellParameters);
                    row.Cells[WriteOffPackageTemplatesHelper.TABLE_BOOK_COLUMN_ASSET_WAR_LOSSES].AddPrice(totalPrice, cellParameters);
                }

                table.Rows.Remove(firstRow);

                var totalCount = parameters.Assets.Sum(x => x.Count);
                var totalSum = parameters.TotalWriteOffSum;


                var dateMergedCell = table.MergeCellsVertically(WriteOffPackageTemplatesHelper.TABLE_BOOK_COLUMN_DATE, firstRowIndex, parameters.Assets.Count);
                var ordenMergedCell = table.MergeCellsVertically(WriteOffPackageTemplatesHelper.TABLE_BOOK_COLUMN_ORDEN, firstRowIndex, parameters.Assets.Count);

                var recordDate = parameters.BookOfLossesExtractData.RecordDate;
                dateMergedCell.AddText(recordDate.ToString(ReportHelper.DATE_FORMAT), cellParameters);
                // get text from Config???
                cellParameters.VerticalAlignment = VerticalAlignment.Middle;
                var ordenText = $"Наказ командира військової частини {_milUnit}\r\n від {parameters.OrdenDate:dd.MM.yyyy}р №{parameters.OrdenNumber}";
                ordenMergedCell.AddText(ordenText, cellParameters);

                AddSummaryRow(totalSum, totalCount, table);
            }
        }

        private static void AddSummaryRow(decimal totalSum, int totalCount, Table table)
        {
            var nameCellParameters = new WordCellParameters(WriteOffPackageTemplatesHelper.TABLE_BOOK_RECORD_FONT_SIZE, HorizontalAlignment.Left, isBold: true);
            var cellParameters = new WordCellParameters(WriteOffPackageTemplatesHelper.TABLE_BOOK_RECORD_FONT_SIZE, HorizontalAlignment.Center, isBold: true);

            var textSummaryRow = table.AddRow(true);
            var countMergedColumns = 3;
            var summaryCell = textSummaryRow.CreateMergedCell(WriteOffPackageTemplatesHelper.TABLE_BOOK_COLUMN_DATE, countMergedColumns);

            textSummaryRow.Cells[WriteOffPackageTemplatesHelper.TABLE_BOOK_COLUMN_DATE].AddText(SummaryRowTotalText, nameCellParameters);
            
            textSummaryRow.Cells[WriteOffPackageTemplatesHelper.TABLE_BOOK_COLUMN_ASSET_COUNT].AddNumber(totalCount, cellParameters);
            textSummaryRow.Cells[WriteOffPackageTemplatesHelper.TABLE_BOOK_COLUMN_ASSET_SUM].AddPrice(totalSum, cellParameters);
            textSummaryRow.Cells[WriteOffPackageTemplatesHelper.TABLE_BOOK_COLUMN_ASSET_WAR_LOSSES].AddPrice(totalSum, cellParameters);
        }
    }
}
