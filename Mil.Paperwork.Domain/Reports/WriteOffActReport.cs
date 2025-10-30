using Mil.Paperwork.Domain.DataModels.Parameters;
using Mil.Paperwork.Domain.Helpers;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Services;
using Spire.Doc;
using Spire.Doc.Documents;
using System.IO;

namespace Mil.Paperwork.Domain.Reports
{
    internal class WriteOffActReport : ITechnicalStateReport
    {
        private readonly IReportDataService _reportDataService;

        private byte[] _reportBytes;

        public WriteOffActReport(IReportDataService reportDataService)
        {
            _reportDataService = reportDataService;
        }

        public bool TryCreate(ITechnicalStateReportParameters reportParameters)
        {
            try
            {
                var templatePath = PathsHelper.GetTemplatePath(TechnicalStateReportHelper.WRITEOFF_ACT_TEMPLATE_NAME);

                var document = new Document();
                document.LoadFromFile(templatePath, FileFormat.Docx);

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

        private void FillTheFields(ITechnicalStateReportParameters reportParameters, Document document)
        {
            var asset = reportParameters.AssetInfo;

            var reportConfig = _reportDataService.GetReportParametersDictionary(ReportType.TechnicalStateReport);
            var assetName = ReportHelper.GetFullAssetName(asset.Name, asset.SerialNumber);

            document.ReplaceField(TechnicalStateReportHelper.FIELD_ASSET_NAME, assetName);
            document.ReplaceField(TechnicalStateReportHelper.FIELD_REGISTRATION_NUMBER, asset.TSRegisterNumber);
            document.ReplaceField(TechnicalStateReportHelper.FIELD_DOCUMENT_NUMBER, asset.TSDocumentNumber);
            document.ReplaceField(TechnicalStateReportHelper.FIELD_DOCUMENT_DATE, reportParameters.DocumentDate.ToString(ReportHelper.DATE_FORMAT));
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
                var nameCellParameters = new WordCellParameters(TechnicalStateReportHelper.TABLE_FONT_SIZE, HorizontalAlignment.Left);
                var cellParameters = new WordCellParameters(TechnicalStateReportHelper.TABLE_FONT_SIZE, HorizontalAlignment.Center);
                var row = table.LastRow;
                var asset = reportParameters.AssetInfo;
                var assetName = ReportHelper.GetFullAssetName(asset.Name, asset.SerialNumber);
                var initialCategory = ReportHelper.ConvertCategoryToText(asset.InitialCategory);

                var residualPrice = ResidualPriceHelper.CalculateResidualPriceForItem(asset, reportParameters.EventDate, asset.Count);
                var nomenclatureCode = asset.NomenclatureCode?.ToUpper() ?? string.Empty;

                row.Cells[TechnicalStateReportHelper.WOA_COLUMN_NAME].AddText(assetName, nameCellParameters);
                row.Cells[TechnicalStateReportHelper.WOA_COLUMN_NOMENCLATURE_CODE].AddText(nomenclatureCode, cellParameters);
                row.Cells[TechnicalStateReportHelper.WOA_COLUMN_MEAS_UNIT].AddText(asset.MeasurementUnit, cellParameters);
                row.Cells[TechnicalStateReportHelper.WOA_COLUMN_CATEGORY].AddText(initialCategory, cellParameters);
                row.Cells[TechnicalStateReportHelper.WOA_COLUMN_PRICE_INITIAL].AddPrice(asset.Price, cellParameters);
                row.Cells[TechnicalStateReportHelper.WOA_COLUMN_COUNT].AddNumber(asset.Count, cellParameters);
                row.Cells[TechnicalStateReportHelper.WOA_COLUMN_PRICE_RESIDUAL].AddPrice(residualPrice, cellParameters);

                row.Cells[TechnicalStateReportHelper.COLUMN_FACTORY_NUMBER].AddText(asset.SerialNumber, cellParameters);
            }
        }
    }
}
