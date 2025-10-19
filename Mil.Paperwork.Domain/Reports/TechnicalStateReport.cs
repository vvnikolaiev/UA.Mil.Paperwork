using Mil.Paperwork.Domain.DataModels.Parameters;
using Mil.Paperwork.Domain.Helpers;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Services;
using Spire.Doc;
using Spire.Doc.Documents;
using System.IO;

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

                var document = new Document();
                document.LoadFromFile(templatePath, FileFormat.Docx);

                FillTheFields(reportParameters, document);
                FillAssetTable(reportParameters, document);
                FillOperationalTable(reportParameters, document);

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

        private void FillTheFields(ITechnicalStateReportParameters reportParameters, Document document)
        {
            var asset = reportParameters.AssetInfo;

            var reportConfig = _reportDataService.GetReportParametersDictionary(ReportType.TechnicalStateReport);
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

        private static void FillAssetTable(ITechnicalStateReportParameters reportParameters, Document document)
        {
            var table = document.GetTable(TechnicalStateReportHelper.TABLE_ASSET_NAME);

            if (table != null)
            {
                // TODO: optimize. Make a mapper.
                var fontSize = TechnicalStateReportHelper.TABLE_FONT_SIZE;
                var row = table.LastRow;
                var asset = reportParameters.AssetInfo;
                var assetName = ReportHelper.GetFullAssetName(asset.Name, asset.SerialNumber);

                var initialCategory = ReportHelper.ConvertCategoryToText(asset.InitialCategory);
                var category = ReportHelper.ConvertEventTypeToCategoryText(asset.InitialCategory, reportParameters.EventType);

                var price = asset.Price * asset.Count;
                
                var residualPrice = ResidualPriceHelper.CalculateResidualPriceForItem(asset, reportParameters.EventDate, asset.Count);
                var nomenclatureCode = asset.NomenclatureCode?.ToUpper() ?? string.Empty;

                row.Cells[TechnicalStateReportHelper.COLUMN_NAME].AddText(assetName, fontSize, HorizontalAlignment.Left);
                row.Cells[TechnicalStateReportHelper.COLUMN_NOMENCLATURE_CODE].AddText(nomenclatureCode, fontSize);
                row.Cells[TechnicalStateReportHelper.COLUMN_MEAS_UNIT].AddText(asset.MeasurementUnit, fontSize);
                row.Cells[TechnicalStateReportHelper.COLUMN_COUNT].AddNumber(asset.Count, fontSize);
                row.Cells[TechnicalStateReportHelper.COLUMN_CATEGORY_INITIAL].AddText(initialCategory, fontSize);
                row.Cells[TechnicalStateReportHelper.COLUMN_CATEGORY_RESIDUAL].AddText(category, fontSize);
                row.Cells[TechnicalStateReportHelper.COLUMN_PRICE_INITIAL].AddPrice(price, fontSize);
                row.Cells[TechnicalStateReportHelper.COLUMN_PRICE_RESIDUAL].AddPrice(residualPrice, fontSize);

                row.Cells[TechnicalStateReportHelper.COLUMN_FACTORY_NUMBER].AddText(asset.SerialNumber, fontSize);
            }
        }

        private static void FillOperationalTable(ITechnicalStateReportParameters reportParameters, Document document)
        {
            var table = document.GetTable(TechnicalStateReportHelper.TABLE_OPERATIONAL_INDICATORS_NAME);

            if (table != null)
            {
                var fontSize = TechnicalStateReportHelper.TABLE_FONT_SIZE;

                // TODO: optimize. Make a mapper.
                var columnNumber = 1;
                var cellCommisioningYear = table.Rows[TechnicalStateReportHelper.ROW_COMMISIONING_YEAR].Cells[columnNumber];
                var cellMonthsOperated = table.Rows[TechnicalStateReportHelper.ROW_MONTHS_OPERATED].Cells[columnNumber];
                var cellHoursOperated = table.Rows[TechnicalStateReportHelper.ROW_HOURS_OPERATED].Cells[columnNumber];
                var cellTechnicalResource = table.Rows[TechnicalStateReportHelper.ROW_TECHNICAL_RESOURCE].Cells[columnNumber];
                var cellTechnicalOperationalTerm = table.Rows[TechnicalStateReportHelper.ROW_TECHNICAL_OPERATIONAL_TERM].Cells[columnNumber];
                var cellWarrantyResource = table.Rows[TechnicalStateReportHelper.ROW_WARRANTY_RESOURCE].Cells[columnNumber];
                var cellWarrantyPeriodYears = table.Rows[TechnicalStateReportHelper.ROW_WARRANTY_PERIOD_YEARS].Cells[columnNumber];
                var cellRepairDescriptionAndDate = table.Rows[TechnicalStateReportHelper.ROW_REPAIR_DESCRIPTION_AND_DATE].Cells[columnNumber];
                var cellInOperatingSinceRepairMonths = table.Rows[TechnicalStateReportHelper.ROW_IN_OPERATING_SINCE_REPAIR_MONTHS].Cells[columnNumber];
                var cellOperatingResourceSinceRepair = table.Rows[TechnicalStateReportHelper.ROW_OPERATING_RESOURCE_SINCE_REPAIR].Cells[columnNumber];
                var cellIncompletenessResource = table.Rows[TechnicalStateReportHelper.ROW_INCOMPLETENESS_RESOURCE].Cells[columnNumber];
                var cellIncompletenessOperationalTerm = table.Rows[TechnicalStateReportHelper.ROW_INCOMPLETENESS_OPERATIONAL_TERM].Cells[columnNumber];
                var cellIncompletenessWarrantyResource = table.Rows[TechnicalStateReportHelper.ROW_INCOMPLETENESS_WARRANTY_RESOURCE].Cells[columnNumber];
                var cellIncompletenessWarrantyTerm = table.Rows[TechnicalStateReportHelper.ROW_INCOMPLETENESS_WARRANTY_TERM].Cells[columnNumber];

                var asset = reportParameters.AssetInfo;
                var comissioningDate = $"{asset.StartDate.Year} рік";
                var monthsOperatedText = ReportHelper.GetYearsOperatedText(asset.StartDate, reportParameters.EventDate);
                var hoursOperatedText = ReportHelper.GetHoursOperatedText(asset.StartDate, reportParameters.EventDate); // годин
                var warrantyPeriodYears = ReportHelper.GetYearsText(asset.WarrantyPeriodMonths / 12);
                var operationalResource = ReportHelper.GetYearsText(asset.ResourceYears);

                cellCommisioningYear.AddText(comissioningDate, fontSize);
                cellMonthsOperated.AddText(monthsOperatedText, fontSize);
                cellHoursOperated.AddText(hoursOperatedText, fontSize);
                cellTechnicalResource.AddText("-", fontSize);
                cellTechnicalOperationalTerm.AddText(operationalResource, fontSize);
                cellWarrantyResource.AddText("-", fontSize);
                cellWarrantyPeriodYears.AddText(warrantyPeriodYears, fontSize);
                cellRepairDescriptionAndDate.AddText("Ремонт не проводився", fontSize);
                cellInOperatingSinceRepairMonths.AddText("-", fontSize);
                cellOperatingResourceSinceRepair.AddText("-", fontSize);
                cellIncompletenessResource.AddText("-", fontSize);
                cellIncompletenessOperationalTerm.AddText("-", fontSize);
                cellIncompletenessWarrantyResource.AddText("-", fontSize);
                cellIncompletenessWarrantyTerm.AddText("-", fontSize);
            }
        }
    }
}
