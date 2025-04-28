using Mil.Paperwork.Domain.DataModels;
using Mil.Paperwork.Domain.Helpers;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Services;
using OfficeOpenXml;
using System.IO;

namespace Mil.Paperwork.Domain.Reports
{
    internal partial class ResidualValueReport : IResidualValueReport
    {
        private readonly IReportDataService _reportDataService;

        private byte[] _reportBytes;

        public ResidualValueReport(IReportDataService reportDataService)
        {
            _reportDataService = reportDataService;
        }

        public bool TryCreate(WriteOffReportData reportData)
        {
            bool result = false;

            try
            {
                result = TryFillTheReport(reportData);

                Console.WriteLine("Дані додано успішно!");
            }
            catch
            {
                result = false;
            }

            return result;
        }

        public byte[] GetReportBytes()
        {
            return _reportBytes;
        }

        private bool TryFillTheReport(WriteOffReportData reportData)
        {
            if (reportData.Assets.Count == 0)
            {
                return false;
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var templatePath = PathsHelper.GetTemplatePath(ResidualValueReportHelper.REPORT_TEMPLATE_NAME);
            using var package = new ExcelPackage(templatePath);
            var sheet = package.Workbook.Worksheets[0]; // Отримуємо перший аркуш

            FillTheTable(reportData, sheet);

            var fieldsMap = _reportDataService.GetReportConfig(ReportType.ResidualValueReport);
            sheet.MadDataToTheNamedFields(fieldsMap);

            using var reportStream = new MemoryStream();
            package.SaveAs(reportStream);
            _reportBytes = reportStream.ToArray();

            return true;
        }

        private static void FillTheTable(WriteOffReportData reportData, ExcelWorksheet sheet)
        {
            var table = sheet.Tables[0]; // Отримуємо першу таблицю

            //sheet.Cells[15, ResidualValueReportHelper.TABLE_COLUMN_WEAR_AND_TEAR_COEFF].Delete(eShiftTypeDelete.Left);
            //sheet.Cells[16, ResidualValueReportHelper.TABLE_COLUMN_WEAR_AND_TEAR_COEFF].Delete(eShiftTypeDelete.Left);

            //table.Columns.Add(4);
            //table.Columns.Delete(ResidualValueReportHelper.TABLE_COLUMN_WEAR_AND_TEAR_COEFF);

            //sheet.Cells[15, ResidualValueReportHelper.TABLE_COLUMN_COEFF_TS].Delete(eShiftTypeDelete.Left);
            //sheet.Cells[16, ResidualValueReportHelper.TABLE_COLUMN_COEFF_TS].Delete(eShiftTypeDelete.Left);

            //table.Columns.Delete(ResidualValueReportHelper.TABLE_COLUMN_COEFF_TS);

            var assets = reportData.Assets;

            int firstRow = table.Address.Start.Row + 1; // Перший рядок таблиці

            if (assets.Count > 1)
            {
                table.AddRow(assets.Count - 1);
            }

            for (int i = 0; i < assets.Count; i++)
            {
                var asset = assets[i];
                var newRow = firstRow + i; // Новий рядок

                var assetName = ReportHelper.GetFullAssetName(asset.Name, asset.SerialNumber);

                // Додаємо дані (припустимо, що в таблиці 3 колонки)
                sheet.Cells[newRow, ResidualValueReportHelper.TABLE_COLUMN_INDEX].Value = i + 1; // number
                sheet.Cells[newRow, ResidualValueReportHelper.TABLE_COLUMN_NAME].Value = assetName;
                sheet.Cells[newRow, ResidualValueReportHelper.TABLE_COLUMN_MEASUREMENT_UNIT].Value = asset.MeasurementUnit;
                sheet.Cells[newRow, ResidualValueReportHelper.TABLE_COLUMN_COUNT].Value = asset.Count;
                sheet.Cells[newRow, ResidualValueReportHelper.TABLE_COLUMN_PRICE].Value = asset.Price;

                sheet.Cells[newRow, ResidualValueReportHelper.TABLE_COLUMN_INDEXATION_COEFF].Value = CoefficientsHelper.GetIndexationCoefficient(asset.StartDate, reportData.ReportDate);
                sheet.Cells[newRow, ResidualValueReportHelper.TABLE_COLUMN_CURRENCY_CONVERSION_RATE].Value = "-";
                sheet.Cells[newRow, ResidualValueReportHelper.TABLE_COLUMN_COEFF_E].Value = CoefficientsHelper.GetExploitationCoefficient(asset.StartDate, reportData.ReportDate);
                sheet.Cells[newRow, ResidualValueReportHelper.TABLE_COLUMN_COEFF_R].Value = CoefficientsHelper.GetWorkCoefficient(100);
                //sheet.Cells[newRow, ResidualValueReportHelper.TABLE_COLUMN_WEAR_AND_TEAR_COEFF].Value = asset.WearAndTearCoeff;
                sheet.Cells[newRow, ResidualValueReportHelper.TABLE_COLUMN_WEAR_AND_TEAR_COEFF].Value = 0.8;
                sheet.Cells[newRow, ResidualValueReportHelper.TABLE_COLUMN_COEFF_TS].Value = CoefficientsHelper.GetTechnicalStateCoefficient(asset.Category);
                sheet.Cells[newRow, ResidualValueReportHelper.TABLE_COLUMN_VALUATION_REPORT_REFERENCE].Value = "-";

                ExcelDocumentHelper.AutoFitRowHeight(sheet, newRow, ResidualValueReportHelper.TABLE_COLUMN_NAME);
            }

            var names = sheet.Names;
            var cellCount = names[ResidualValueReportHelper.FIELD_NUMBER_NAMES];
            var cellSum = names[ResidualValueReportHelper.FIELD_SUM];
            cellCount.Value = ReportHelper.ConvertNamesNumberToReportString(assets.Count);
            cellSum.Formula = string.Format(ResidualValueReportHelper.TABLE_COLUMN_SUM_FORMULA, table.Address.Start.Row + 1, table.Address.End.Row);
        }
    }
}
