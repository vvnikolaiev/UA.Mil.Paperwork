using Mil.Paperwork.Domain.DataModels.Parameters;
using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Domain.Helpers;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Services;

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

                using var document = WordDocument.LoadFromFile(templatePath);

                FillCommission(document);
                FillTheFields(reportParameters, document);
                FillAssetTable(reportParameters, document);

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
            var dictCommissionFields = ReportParametersHelper.GetCommission(ReportType.WriteOffAct, _reportDataService);
            document.ReplaceFields(dictCommissionFields);
        }

        private void FillTheFields(ICommonWriteOffReportData reportParameters, WordDocument document)
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

        private static void FillAssetTable(ICommonWriteOffReportData reportParameters, WordDocument document)
        {
            var table = document.GetTable(TechnicalStateReportHelper.TABLE_ASSET_NAME);

            if (table != null)
            {
                var firstRow = table.LastRow;
                var nameCellParameters = new WordCellParameters(TechnicalStateReportHelper.TABLE_FONT_SIZE, WordHorizontalAlignment.Left);
                var cellParameters = new WordCellParameters(TechnicalStateReportHelper.TABLE_FONT_SIZE, WordHorizontalAlignment.Center);

                for (int i = 0; i < reportParameters.Assets.Count; i++)
                {
                    var asset = reportParameters.Assets[i];
                    var row = table.AddRow();

                    var assetName = ReportHelper.GetFullAssetName(asset.Name, asset.SerialNumber);
                    var initialCategory = ReportHelper.ConvertCategoryToText(asset.InitialCategory);
                    var residualPrice = ResidualPriceHelper.CalculateResidualPriceForItem(asset, reportParameters.EventDate, asset.Count);
                    var nomenclatureCode = asset.NomenclatureCode?.ToUpper() ?? string.Empty;

                    row.GetCell(TechnicalStateReportHelper.WOA_COLUMN_INDEX).AddNumber(i + 1, cellParameters);
                    row.GetCell(TechnicalStateReportHelper.WOA_COLUMN_NAME).AddText(assetName, nameCellParameters);
                    row.GetCell(TechnicalStateReportHelper.WOA_COLUMN_NOMENCLATURE_CODE).AddText(nomenclatureCode, cellParameters);
                    row.GetCell(TechnicalStateReportHelper.WOA_COLUMN_MEAS_UNIT).AddText(asset.MeasurementUnit, cellParameters);
                    row.GetCell(TechnicalStateReportHelper.WOA_COLUMN_CATEGORY).AddText(initialCategory, cellParameters);
                    row.GetCell(TechnicalStateReportHelper.WOA_COLUMN_PRICE_INITIAL).AddPrice(asset.Price, cellParameters);
                    row.GetCell(TechnicalStateReportHelper.WOA_COLUMN_COUNT).AddNumber(asset.Count, cellParameters);
                    row.GetCell(TechnicalStateReportHelper.WOA_COLUMN_PRICE_RESIDUAL).AddPrice(residualPrice, cellParameters);
                    row.GetCell(TechnicalStateReportHelper.COLUMN_FACTORY_NUMBER).AddText(asset.SerialNumber, cellParameters);
                }

                table.RemoveRow(firstRow);

                AddSummaryRow(reportParameters, table);
            }
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
