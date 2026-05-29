using Mil.Paperwork.Domain.DataModels.Parameters;
using Mil.Paperwork.Domain.Helpers;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Services;

namespace Mil.Paperwork.Domain.Reports
{
    internal class TechnicalStateReport : ITechnicalStateReport
    {
        private readonly IReportDataService _reportDataService;

        private byte[] _reportBytes;

        public TechnicalStateReport(IReportDataService reportDataService)
        {
            _reportDataService = reportDataService;
        }

        public bool TryCreate(ITechnicalStateReportParameters reportParameters)
        {
            try
            {
                var templatePath = PathsHelper.GetTemplatePath(TechnicalStateReportHelper.REPORT11_TEMPLATE_NAME);

                using var document = WordDocument.LoadFromFile(templatePath);

                FillCommission(document);
                FillTheFields(reportParameters, document);
                FillAssetTable(reportParameters, document);
                FillOperationalTable(reportParameters, document);

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

        private void FillTheFields(ITechnicalStateReportParameters reportParameters, WordDocument document)
        {
            var asset = reportParameters.AssetInfo;
            var reportConfig = ReportParametersHelper.GetFullParametersDictionary(ReportType.TechnicalStateReport, _reportDataService);
            var assetName = ReportHelper.GetFullAssetName(asset.Name, asset.SerialNumber);
            var category = ReportHelper.ConvertEventTypeToCategoryText(asset.InitialCategory, reportParameters.EventType);

            document.ReplaceField(TechnicalStateReportHelper.FIELD_ASSET_NAME, assetName);
            document.ReplaceField(TechnicalStateReportHelper.FIELD_REGISTRATION_NUMBER, asset.TSRegisterNumber);
            document.ReplaceField(TechnicalStateReportHelper.FIELD_DOCUMENT_NUMBER, asset.TSDocumentNumber);
            document.ReplaceField(TechnicalStateReportHelper.FIELD_DOCUMENT_DATE, reportParameters.DocumentDate.ToString(ReportHelper.DATE_FORMAT));
            document.ReplaceField(TechnicalStateReportHelper.FIELD_ASSET_RESIDUAL_CATEGORY, category);
            document.ReplaceField(TechnicalStateReportHelper.FIELD_REASON, reportParameters.Reason);
            document.ReplaceField(TechnicalStateReportHelper.FIELD_EVENT_DATE, reportParameters.EventDate.ToString(ReportHelper.DATE_FORMAT));
            document.ReplaceField(TechnicalStateReportHelper.FIELD_ORDEN_NUMBER, reportParameters.OrdenNumber.ToString());
            document.ReplaceField(TechnicalStateReportHelper.FIELD_ORDEN_DATE, reportParameters.OrdenDate.ToString(ReportHelper.DATE_FORMAT));

            document.ReplaceFields(reportConfig);
        }

        private void FillCommission(WordDocument document)
        {
            var dictCommissionFields = ReportParametersHelper.GetCommission(ReportType.TechnicalStateReport, _reportDataService);
            document.ReplaceFields(dictCommissionFields);
        }

        private static void FillAssetTable(ITechnicalStateReportParameters reportParameters, WordDocument document)
        {
            var table = document.GetTable(TechnicalStateReportHelper.TABLE_ASSET_NAME);

            if (table != null)
            {
                var nameCellParameters = new WordCellParameters(TechnicalStateReportHelper.TABLE_FONT_SIZE, WordHorizontalAlignment.Left);
                var cellParameters = new WordCellParameters(TechnicalStateReportHelper.TABLE_FONT_SIZE, WordHorizontalAlignment.Center);
                var row = table.LastRow;
                var asset = reportParameters.AssetInfo;
                var assetName = ReportHelper.GetFullAssetName(asset.Name, asset.SerialNumber);
                var initialCategory = ReportHelper.ConvertCategoryToText(asset.InitialCategory);
                var category = ReportHelper.ConvertEventTypeToCategoryText(asset.InitialCategory, reportParameters.EventType);
                var price = asset.Price * asset.Count;
                var residualPrice = ResidualPriceHelper.CalculateResidualPriceForItem(asset, reportParameters.EventDate, asset.Count);
                var nomenclatureCode = asset.NomenclatureCode?.ToUpper() ?? string.Empty;

                row.GetCell(TechnicalStateReportHelper.COLUMN_NAME).AddText(assetName, nameCellParameters);
                row.GetCell(TechnicalStateReportHelper.COLUMN_NOMENCLATURE_CODE).AddText(nomenclatureCode, cellParameters);
                row.GetCell(TechnicalStateReportHelper.COLUMN_MEAS_UNIT).AddText(asset.MeasurementUnit, cellParameters);
                row.GetCell(TechnicalStateReportHelper.COLUMN_COUNT).AddNumber(asset.Count, cellParameters);
                row.GetCell(TechnicalStateReportHelper.COLUMN_CATEGORY_INITIAL).AddText(initialCategory, cellParameters);
                row.GetCell(TechnicalStateReportHelper.COLUMN_CATEGORY_RESIDUAL).AddText(category, cellParameters);
                row.GetCell(TechnicalStateReportHelper.COLUMN_PRICE_INITIAL).AddPrice(price, cellParameters);
                row.GetCell(TechnicalStateReportHelper.COLUMN_PRICE_RESIDUAL).AddPrice(residualPrice, cellParameters);
                row.GetCell(TechnicalStateReportHelper.COLUMN_FACTORY_NUMBER).AddText(asset.SerialNumber, cellParameters);
            }
        }

        private static void FillOperationalTable(ITechnicalStateReportParameters reportParameters, WordDocument document)
        {
            var table = document.GetTable(TechnicalStateReportHelper.TABLE_OPERATIONAL_INDICATORS_NAME);

            if (table != null)
            {
                var cellParameters = new WordCellParameters(TechnicalStateReportHelper.TABLE_FONT_SIZE, WordHorizontalAlignment.Center);
                const int columnNumber = 1;

                var asset = reportParameters.AssetInfo;
                var comissioningDate = $"{asset.StartDate.Year} рік";
                var monthsOperatedText = ReportHelper.GetYearsOperatedText(asset.StartDate, reportParameters.EventDate);
                var hoursOperatedText = ReportHelper.GetHoursOperatedText(asset.StartDate, reportParameters.EventDate);
                var warrantyPeriodYears = ReportHelper.GetYearsText(asset.WarrantyPeriodMonths / 12);
                var operationalResource = ReportHelper.GetYearsText(asset.ResourceYears);

                table.GetRow(TechnicalStateReportHelper.ROW_COMMISIONING_YEAR).GetCell(columnNumber).AddText(comissioningDate, cellParameters);
                table.GetRow(TechnicalStateReportHelper.ROW_MONTHS_OPERATED).GetCell(columnNumber).AddText(monthsOperatedText, cellParameters);
                table.GetRow(TechnicalStateReportHelper.ROW_HOURS_OPERATED).GetCell(columnNumber).AddText(hoursOperatedText, cellParameters);
                table.GetRow(TechnicalStateReportHelper.ROW_TECHNICAL_RESOURCE).GetCell(columnNumber).AddText("-", cellParameters);
                table.GetRow(TechnicalStateReportHelper.ROW_TECHNICAL_OPERATIONAL_TERM).GetCell(columnNumber).AddText(operationalResource, cellParameters);
                table.GetRow(TechnicalStateReportHelper.ROW_WARRANTY_RESOURCE).GetCell(columnNumber).AddText("-", cellParameters);
                table.GetRow(TechnicalStateReportHelper.ROW_WARRANTY_PERIOD_YEARS).GetCell(columnNumber).AddText(warrantyPeriodYears, cellParameters);
                table.GetRow(TechnicalStateReportHelper.ROW_REPAIR_DESCRIPTION_AND_DATE).GetCell(columnNumber).AddText("Ремонт не проводився", cellParameters);
                table.GetRow(TechnicalStateReportHelper.ROW_IN_OPERATING_SINCE_REPAIR_MONTHS).GetCell(columnNumber).AddText("-", cellParameters);
                table.GetRow(TechnicalStateReportHelper.ROW_OPERATING_RESOURCE_SINCE_REPAIR).GetCell(columnNumber).AddText("-", cellParameters);
                table.GetRow(TechnicalStateReportHelper.ROW_INCOMPLETENESS_RESOURCE).GetCell(columnNumber).AddText("-", cellParameters);
                table.GetRow(TechnicalStateReportHelper.ROW_INCOMPLETENESS_OPERATIONAL_TERM).GetCell(columnNumber).AddText("-", cellParameters);
                table.GetRow(TechnicalStateReportHelper.ROW_INCOMPLETENESS_WARRANTY_RESOURCE).GetCell(columnNumber).AddText("-", cellParameters);
                table.GetRow(TechnicalStateReportHelper.ROW_INCOMPLETENESS_WARRANTY_TERM).GetCell(columnNumber).AddText("-", cellParameters);
            }
        }
    }
}
