using Mil.Paperwork.Domain.DataModels.Assets;
using Mil.Paperwork.Domain.DataModels.Parameters;
using Mil.Paperwork.Domain.Enums;
using Mil.Paperwork.Domain.Helpers;
using Mil.Paperwork.Infrastructure.DataModels;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Services;

namespace Mil.Paperwork.Domain.Reports
{
    internal class InitialTechnicalStateReport : IInitialTechnicalStateReport
    {
        private readonly IReportDataService _reportDataService;

        private byte[] _reportBytes;

        public InitialTechnicalStateReport(IReportDataService reportDataService)
        {
            _reportDataService = reportDataService;
        }

        public bool TryCreate(IAssetInfo assetInfo, IPerson personAccepted, IPerson personHanded, EventType eventType)
        {
            try
            {
                var templatePath = PathsHelper.GetTemplatePath(TechnicalStateReportHelper.REPORT7_TEMPLATE_NAME);

                using var document = WordDocument.LoadFromFile(templatePath);

                FillCommission(document);
                FillTheFields(assetInfo, personAccepted, personHanded, eventType, document);
                FillAssetTable(assetInfo, eventType, document);
                FillOperationalTable(assetInfo, document);

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
            var dictCommissionFields = ReportParametersHelper.GetCommission(ReportType.TechnicalStateReport, _reportDataService);
            document.ReplaceFields(dictCommissionFields);
        }

        private void FillTheFields(IAssetInfo asset, IPerson personAccepted, IPerson personHanded, EventType eventType, WordDocument document)
        {
            var reportConfig = ReportParametersHelper.GetFullParametersDictionary(ReportType.TechnicalStateReport, _reportDataService);
            var assetName = ReportHelper.GetFullAssetName(asset.Name, asset.SerialNumber);
            var sInitialCategory = ReportHelper.ConvertCategoryToText(asset.InitialCategory);
            var residualCategory = ReportHelper.ConvertEventTypeToCategory(asset.InitialCategory, eventType);
            var sResidualCategory = ReportHelper.ConvertCategoryToText(residualCategory);
            ReportHelper.ConvertCategoryToFullText(asset.InitialCategory, residualCategory, out var sInitialText, out var sResidualText);

            document.ReplaceField(TechnicalStateReportHelper.FIELD_ASSET_NAME, assetName);
            document.ReplaceField(TechnicalStateReportHelper.FIELD_REGISTRATION_NUMBER, asset.TSRegisterNumber);
            document.ReplaceField(TechnicalStateReportHelper.FIELD_DOCUMENT_NUMBER, asset.TSDocumentNumber);
            document.ReplaceField(TechnicalStateReportHelper.FIELD_DOCUMENT_DATE, asset.StartDate.ToString(ReportHelper.DATE_FORMAT));
            document.ReplaceField(TechnicalStateReportHelper.FIELD_ASSET_INITIAL_CATEGORY, sInitialCategory);
            document.ReplaceField(TechnicalStateReportHelper.FIELD_ASSET_RESIDUAL_CATEGORY, sResidualCategory);

            document.ReplaceField(TechnicalStateReportHelper.FIELD_ASSET_INITIAL_CATEGORY_TEXT, sInitialText);
            document.ReplaceField(TechnicalStateReportHelper.FIELD_ASSET_RESIDUAL_CATEGORY_TEXT, sResidualText);

            document.ReplaceField(CommissioningActHelper.FIELD_ACCEPTED_PERSON_RANK, personAccepted?.Rank ?? string.Empty);
            document.ReplaceField(CommissioningActHelper.FIELD_ACCEPTED_PERSON_NAME, personAccepted?.FullName ?? string.Empty);
            document.ReplaceField(CommissioningActHelper.FIELD_HANDED_PERSON_RANK, personHanded?.Rank ?? string.Empty);
            document.ReplaceField(CommissioningActHelper.FIELD_HANDED_PERSON_NAME, personHanded?.FullName ?? string.Empty);

            document.ReplaceFields(reportConfig);
        }

        private static void FillAssetTable(IAssetInfo asset, EventType eventType, WordDocument document)
        {
            var table = document.GetTable(TechnicalStateReportHelper.TABLE_ASSET_NAME);

            if (table != null)
            {
                var nameCellParameters = new WordCellParameters(TechnicalStateReportHelper.TABLE_FONT_SIZE, WordHorizontalAlignment.Left);
                var cellParameters = new WordCellParameters(TechnicalStateReportHelper.TABLE_FONT_SIZE, WordHorizontalAlignment.Center);
                var row = table.LastRow;

                var assetName = ReportHelper.GetFullAssetName(asset.Name, asset.SerialNumber);
                var initialCategory = ReportHelper.ConvertCategoryToText(asset.InitialCategory);
                var category = ReportHelper.ConvertEventTypeToCategoryText(asset.InitialCategory, eventType);
                var totalPrice = asset.Price * asset.Count;
                var nomenclatureCode = asset.NomenclatureCode?.ToUpper() ?? string.Empty;

                row.GetCell(TechnicalStateReportHelper.COLUMN_NAME).AddText(assetName, nameCellParameters);
                row.GetCell(TechnicalStateReportHelper.COLUMN_NOMENCLATURE_CODE).AddText(nomenclatureCode, cellParameters);
                row.GetCell(TechnicalStateReportHelper.COLUMN_MEAS_UNIT).AddText(asset.MeasurementUnit, cellParameters);
                row.GetCell(TechnicalStateReportHelper.COLUMN_COUNT).AddNumber(asset.Count, cellParameters);
                row.GetCell(TechnicalStateReportHelper.COLUMN_CATEGORY_INITIAL).AddText(initialCategory, cellParameters);
                row.GetCell(TechnicalStateReportHelper.COLUMN_CATEGORY_RESIDUAL).AddText(initialCategory, cellParameters);
                row.GetCell(TechnicalStateReportHelper.COLUMN_PRICE_INITIAL).AddPrice(asset.Price, cellParameters);
                row.GetCell(TechnicalStateReportHelper.COLUMN_PRICE_RESIDUAL).AddPrice(totalPrice, cellParameters);
                row.GetCell(TechnicalStateReportHelper.COLUMN_FACTORY_NUMBER).AddText(asset.SerialNumber, cellParameters);
            }
        }

        private static void FillOperationalTable(IAssetInfo asset, WordDocument document)
        {
            var table = document.GetTable(TechnicalStateReportHelper.TABLE_OPERATIONAL_INDICATORS_NAME);

            if (table != null)
            {
                var cellParameters = new WordCellParameters(TechnicalStateReportHelper.TABLE_FONT_SIZE, WordHorizontalAlignment.Center);
                const int columnNumber = 1;

                var warrantyPeriodYears = ReportHelper.GetYearsText(asset.WarrantyPeriodMonths / 12);
                var operationalResource = ReportHelper.GetYearsText(asset.ResourceYears);

                table.GetRow(TechnicalStateReportHelper.ROW_TECHNICAL_RESOURCE).GetCell(columnNumber).AddText("-", cellParameters);
                table.GetRow(TechnicalStateReportHelper.ROW_TECHNICAL_OPERATIONAL_TERM).GetCell(columnNumber).AddText(operationalResource, cellParameters);
                table.GetRow(TechnicalStateReportHelper.ROW_WARRANTY_RESOURCE).GetCell(columnNumber).AddText("-", cellParameters);
                table.GetRow(TechnicalStateReportHelper.ROW_WARRANTY_PERIOD_YEARS).GetCell(columnNumber).AddText(warrantyPeriodYears, cellParameters);
                table.GetRow(TechnicalStateReportHelper.ROW_REPAIR_DESCRIPTION_AND_DATE).GetCell(columnNumber).AddText("-", cellParameters);
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
