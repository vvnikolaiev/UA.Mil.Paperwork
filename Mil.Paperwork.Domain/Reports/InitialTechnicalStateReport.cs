﻿using Mil.Paperwork.Domain.DataModels.Assets;
using Mil.Paperwork.Domain.Enums;
using Mil.Paperwork.Domain.Helpers;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Services;
using Spire.Doc;
using Spire.Doc.Documents;
using System.IO;

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

        public bool TryCreate(IAssetInfo assetInfo, EventType eventType)
        {
            try
            {
                var templatePath = PathsHelper.GetTemplatePath(TechnicalStateReportHelper.REPORT7_TEMPLATE_NAME);

                var document = new Document();
                document.LoadFromFile(templatePath, FileFormat.Docx);

                FillTheFields(assetInfo, eventType, document);
                FillAssetTable(assetInfo, eventType, document);
                FillOperationalTable(assetInfo, document);

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

        private void FillTheFields(IAssetInfo asset, EventType eventType, Document document)
        {
            var reportConfig = _reportDataService.GetReportParametersDictionary(ReportType.TechnicalStateReport);
            var assetName = ReportHelper.GetFullAssetName(asset.Name, asset.SerialNumber);
            var sInitialCategory = ReportHelper.ConvertCategoryToText(asset.InitialCategory);
            var residualCategory = ReportHelper.ConvertEventTypeToCategory(asset.InitialCategory, eventType);
            var sResidualCategory = ReportHelper.ConvertCategoryToText(residualCategory);
            ReportHelper.ConvertCategoryToFullText(asset.InitialCategory, residualCategory, out var sInitialText, out var sResidualText);

            document.ReplaceField(TechnicalStateReportHelper.FIELD_ASSET_NAME, assetName);
            document.ReplaceField(TechnicalStateReportHelper.FIELD_REGISTRATION_NUMBER, asset.TSRegisterNumber);
            document.ReplaceField(TechnicalStateReportHelper.FIELD_DOCUMENT_NUMBER, asset.TSDocumentNumber);
            document.ReplaceField(TechnicalStateReportHelper.FIELD_ASSET_INITIAL_CATEGORY, sInitialCategory);
            document.ReplaceField(TechnicalStateReportHelper.FIELD_ASSET_RESIDUAL_CATEGORY, sResidualCategory);

            document.ReplaceField(TechnicalStateReportHelper.FIELD_ASSET_INITIAL_CATEGORY_TEXT, sInitialText);
            document.ReplaceField(TechnicalStateReportHelper.FIELD_ASSET_RESIDUAL_CATEGORY_TEXT, sResidualText);

            document.ReplaceFields(reportConfig);
        }

        private static void FillAssetTable(IAssetInfo asset, EventType eventType, Document document)
        {
            var tables = document.Sections[0].Tables.Cast<Table>().ToList();
            var table = tables.FirstOrDefault(x => x.Title == TechnicalStateReportHelper.TABLE_ASSET_NAME);
            if (table != null)
            {
                // TODO: optimize. Make a mapper.
                var row = table.LastRow;

                var assetName = ReportHelper.GetFullAssetName(asset.Name, asset.SerialNumber);

                var initialCategory = ReportHelper.ConvertCategoryToText(asset.InitialCategory);
                var category = ReportHelper.ConvertEventTypeToCategoryText(asset.InitialCategory, eventType);

                var totalPrice = asset.Price * asset.Count;
                var nomenclatureCode = asset.NomenclatureCode?.ToUpper() ?? string.Empty;

                row.Cells[TechnicalStateReportHelper.COLUMN_NAME].AddText(assetName, 12, HorizontalAlignment.Left);
                row.Cells[TechnicalStateReportHelper.COLUMN_NOMENCLATURE_CODE].AddText(nomenclatureCode);
                row.Cells[TechnicalStateReportHelper.COLUMN_MEAS_UNIT].AddText(asset.MeasurementUnit);
                row.Cells[TechnicalStateReportHelper.COLUMN_COUNT].AddNumber(asset.Count);
                row.Cells[TechnicalStateReportHelper.COLUMN_CATEGORY_INITIAL].AddText(initialCategory);
                row.Cells[TechnicalStateReportHelper.COLUMN_CATEGORY_RESIDUAL].AddText(initialCategory);
                row.Cells[TechnicalStateReportHelper.COLUMN_PRICE_INITIAL].AddPrice(asset.Price);
                row.Cells[TechnicalStateReportHelper.COLUMN_PRICE_RESIDUAL].AddPrice(totalPrice);
                row.Cells[TechnicalStateReportHelper.COLUMN_FACTORY_NUMBER].AddText(asset.SerialNumber);
                row.Cells[TechnicalStateReportHelper.COLUMN_MANUFACTURER].AddText("-");
                row.Cells[TechnicalStateReportHelper.COLUMN_PASSPORT_NUMBER].AddText("-");
            }
        }

        private static void FillOperationalTable(IAssetInfo asset, Document document)
        {
            var tables = document.Sections[0].Tables.Cast<Table>().ToList();
            var table = tables.FirstOrDefault(x => x.Title == TechnicalStateReportHelper.TABLE_OPERATIONAL_INDICATORS_NAME);
            if (table != null)
            {
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

                var warrantyPeriodMonths = ReportHelper.GetMonthsText(asset.WarrantyPeriodMonths);
                var warrantyPeriodYears = ReportHelper.GetWarrantyPeriodText(asset.WarrantyPeriodMonths / 12);

                // add feminine/masculine/neutral form????

                cellTechnicalResource.AddText("-");
                cellTechnicalOperationalTerm.AddText(warrantyPeriodMonths);
                cellWarrantyResource.AddText("-");
                cellWarrantyPeriodYears.AddText(warrantyPeriodYears);
                cellRepairDescriptionAndDate.AddText("-");
                cellInOperatingSinceRepairMonths.AddText("-");
                cellOperatingResourceSinceRepair.AddText("-");
                cellIncompletenessResource.AddText("-");
                cellIncompletenessOperationalTerm.AddText("-");
                cellIncompletenessWarrantyResource.AddText("-");
                cellIncompletenessWarrantyTerm.AddText("-");
            }
        }
    }
}
