using Mil.Paperwork.Domain.DataModels.Parameters;
using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Domain.Helpers;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Services;
using Spire.Doc;
using Spire.Doc.Documents;

namespace Mil.Paperwork.Domain.Reports
{
    internal class WriteOffActReport : IWriteOffActReport
    {
        private readonly IReportDataService _reportDataService;

        private byte[] _reportBytes;

        public WriteOffActReport(IReportDataService reportDataService)
        {
            _reportDataService = reportDataService;
        }

        public bool TryCreate(ICommonWriteOffReportData reportParameters)
        {
            try
            {
                var templatePath = PathsHelper.GetTemplatePath(TechnicalStateReportHelper.WRITEOFF_ACT_TEMPLATE_NAME);

                var document = new Document();
                document.LoadFromFile(templatePath, FileFormat.Docx);

                FillCommission(document);
                FillTheFields(reportParameters, document);
                FillAssetTable(reportParameters, document);

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

        private void FillCommission(Document document)
        {
            var dictCommissionFields = ReportParametersHelper.GetCommission(ReportType.WriteOffAct, _reportDataService);

            document.ReplaceFields(dictCommissionFields);
        }

        private void FillTheFields(ICommonWriteOffReportData reportParameters, Document document)
        {
            var reportConfig = ReportParametersHelper.GetFullParametersDictionary(ReportType.WriteOffAct, _reportDataService);

            document.ReplaceField(TechnicalStateReportHelper.FIELD_REGISTRATION_NUMBER, reportParameters.RegistrationNumber);
            document.ReplaceField(TechnicalStateReportHelper.FIELD_DOCUMENT_NUMBER, reportParameters.DocumentNumber);
            document.ReplaceField(TechnicalStateReportHelper.FIELD_DOCUMENT_DATE, reportParameters.DocumentDate.ToString(ReportHelper.DATE_FORMAT));
            document.ReplaceField(TechnicalStateReportHelper.FIELD_REASON, reportParameters.Reason);
            document.ReplaceField(TechnicalStateReportHelper.FIELD_EVENT_DATE, reportParameters.EventDate.ToString(ReportHelper.DATE_FORMAT));
            document.ReplaceField(TechnicalStateReportHelper.FIELD_ORDEN_NUMBER, reportParameters.OrdenNumber.ToString());
            document.ReplaceField(TechnicalStateReportHelper.FIELD_ORDEN_DATE, reportParameters.OrdenDate.ToString(ReportHelper.DATE_FORMAT));

            document.ReplaceFields(reportConfig);
        }

        private static void FillAssetTable(ICommonWriteOffReportData reportParameters, Document document)
        {
            var table = document.GetTable(TechnicalStateReportHelper.TABLE_ASSET_NAME);

            if (table != null)
            {
                var firstRow = table.LastRow;
                var nameCellParameters = new WordCellParameters(TechnicalStateReportHelper.TABLE_FONT_SIZE, HorizontalAlignment.Left);
                var cellParameters = new WordCellParameters(TechnicalStateReportHelper.TABLE_FONT_SIZE, HorizontalAlignment.Center);

                for (int i = 0; i < reportParameters.Assets.Count; i++)
                {
                    var asset = reportParameters.Assets[i];
                    TableRow row = table.AddRow();

                    var assetName = ReportHelper.GetFullAssetName(asset.Name, asset.SerialNumber);
                    var initialCategory = ReportHelper.ConvertCategoryToText(asset.InitialCategory);

                    var residualPrice = ResidualPriceHelper.CalculateResidualPriceForItem(asset, reportParameters.EventDate, asset.Count);
                    var nomenclatureCode = asset.NomenclatureCode?.ToUpper() ?? string.Empty;

                    row.Cells[TechnicalStateReportHelper.WOA_COLUMN_INDEX].AddNumber(i + 1, cellParameters);
                    row.Cells[TechnicalStateReportHelper.WOA_COLUMN_NAME].AddText(assetName, nameCellParameters);
                    row.Cells[TechnicalStateReportHelper.WOA_COLUMN_NOMENCLATURE_CODE].AddText(nomenclatureCode, cellParameters);
                    row.Cells[TechnicalStateReportHelper.WOA_COLUMN_MEAS_UNIT].AddText(asset.MeasurementUnit, cellParameters);
                    row.Cells[TechnicalStateReportHelper.WOA_COLUMN_CATEGORY].AddText(initialCategory, cellParameters);
                    row.Cells[TechnicalStateReportHelper.WOA_COLUMN_PRICE_INITIAL].AddPrice(asset.Price, cellParameters);
                    row.Cells[TechnicalStateReportHelper.WOA_COLUMN_COUNT].AddNumber(asset.Count, cellParameters);
                    row.Cells[TechnicalStateReportHelper.WOA_COLUMN_PRICE_RESIDUAL].AddPrice(residualPrice, cellParameters);

                    row.Cells[TechnicalStateReportHelper.COLUMN_FACTORY_NUMBER].AddText(asset.SerialNumber, cellParameters);
                }

                table.Rows.Remove(firstRow);

                AddSummaryRow(reportParameters, table);
            }
        }

        private static void AddSummaryRow(ICommonWriteOffReportData reportData, Table table)
        {
            var cellParameters = new WordCellParameters(QualityStateReportHelper.TABLE_FONT_SIZE, HorizontalAlignment.Left);
            var totalSumClear = ResidualPriceHelper.CalculateTotalReportSum(reportData.Assets, reportData.EventDate, false);
            var totalSum = ResidualPriceHelper.CalculateTotalReportSum(reportData.Assets, reportData.EventDate, true);

            var totalSumText = ReportHelper.ConvertTotalSumToUkrainianString(totalSum);
            var totalItemsText = ReportHelper.ConvertNamesNumberToReportString(reportData.Assets.Count);

            // last united string row 
            var textSummaryRow = table.AddRow(false);
            var summaryCell = textSummaryRow.CreateMergedCell(0, textSummaryRow.Cells.Count);
            var summaryText = string.Format(QualityStateReportHelper.TOTAL_TEXT_FORMAT, totalItemsText, totalSumText);
            textSummaryRow.Cells[0].AddText(summaryText, cellParameters);
        }
    }
}
