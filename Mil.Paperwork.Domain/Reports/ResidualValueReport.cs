using Mil.Paperwork.Domain.DataModels;
using Mil.Paperwork.Domain.Enums;
using Mil.Paperwork.Domain.Helpers;
using Mil.Paperwork.Domain.Resources;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Services;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using System.IO;

namespace Mil.Paperwork.Domain.Reports
{
    internal partial class ResidualValueReport : IResidualValueReport
    {
        private readonly IReportDataService _reportDataService;
        private readonly Dictionary<AssetType, Dictionary<ResidualValueTableColumns, BaseColumnInfo>> _tableMappings = new();

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

            PrepareTheTemplate(reportData, sheet);

            FillTheTable(reportData, sheet);

            var fieldsMap = _reportDataService.GetReportConfig(ReportType.ResidualValueReport);
            fieldsMap.Add(ResidualValueReportHelper.REPORT_DATE_PLACEHOLDER, reportData.ReportDate.ToString(ReportHelper.DATE_FORMAT));
            fieldsMap.Add(ResidualValueReportHelper.TOTAL_RESIDUAL_SUM_PLACEHOLDER, CalculateTotalResidualValue(reportData.Assets).ToString("F2"));
            sheet.MadDataToTheNamedFields(fieldsMap);

            using var reportStream = new MemoryStream();
            package.SaveAs(reportStream);
            _reportBytes = reportStream.ToArray();

            return true;
        }

        private void PrepareTheTemplate(WriteOffReportData reportData, ExcelWorksheet sheet)
        {
            // update columns depending on coefficient count
            var names = sheet.Names;
            var mapping = GetTableMapping(reportData.AssetType);

            var headerCell = names[ResidualValueReportHelper.FIELD_TABLE_HEADER];
            var columnNumberCell = names[ResidualValueReportHelper.FIELD_TABLE_COLUMN_NUMBER];
            var cellCount = names[ResidualValueReportHelper.FIELD_NUMBER_NAMES];

            var headerRow1 = headerCell.Start.Row;
            var headerRow2 = headerRow1 + 1;
            var headerNumbersRow = columnNumberCell.Start.Row;
            var countRowNumber = cellCount.Start.Row;

            var coeffColumns = ResidualValueReportHelper.GetCoefficientColumns(reportData.AssetType);

            var width = 28;

            if (coeffColumns.Count > 0)
            {
                var subCoedffColumnWidth = width / coeffColumns.Count;

                var coeffStartColumn = ResidualValueReportHelper.TABLE_FIRST_COEFF_COLUMN;

                for (int i = 0; i < coeffColumns.Count; i++)
                {
                    var columnNum = coeffStartColumn + i;
                    var columnHeaderText = coeffColumns[i];

                    sheet.Column(columnNum).Width = subCoedffColumnWidth;

                    sheet.FillPseudoTableCell(columnHeaderText, headerRow2, columnNum);
                    sheet.FillPseudoTableCell(string.Empty, countRowNumber, columnNum);
                }

                var endColumn = coeffStartColumn + coeffColumns.Count - 1;

                var coefsText = ResidualValueReportStrings.CoefficientsMergedColumnTitle;
                sheet.FillPseudoTableCell(coefsText, headerRow1, coeffStartColumn, headerRow1, endColumn, true);
                sheet.FillPseudoTableCell("9", headerNumbersRow, coeffStartColumn, headerNumbersRow, endColumn, true);
            }

            // Column Total Coefficient
            AddTableHeaderColumn(reportData.AssetType, ResidualValueTableColumns.TotalWearCoefficient, sheet);
            // Column Total sum
            AddTableHeaderColumn(reportData.AssetType, ResidualValueTableColumns.ResidualValue, sheet);
            // Column Total sum
            AddTableHeaderColumn(reportData.AssetType, ResidualValueTableColumns.ValuationReportReference, sheet);

            // Виправити об'єднані клітинки над таблицею так, щоби вони займали таку ж кількість стовпців
            var lastColumn = mapping[ResidualValueTableColumns.ValuationReportReference];
            sheet.ShrinkMergedNamedRange(ResidualValueReportHelper.RANGE_TO_SHRINK_1_NAME, lastColumn.ColumnIndex);
            sheet.ShrinkMergedNamedRange(ResidualValueReportHelper.RANGE_TO_SHRINK_2_NAME, lastColumn.ColumnIndex);

            // Перемістити верхню праву групу в залежності від кількості стовпців 
            var columnShift = coeffColumns.Count - 4;
            sheet.ShiftMergedCellsInsideNamedRange(ResidualValueReportHelper.RANGE_TO_SHIFT_1_NAME, columnShift);
            sheet.ShiftMergedCellsInsideNamedRange(ResidualValueReportHelper.RANGE_TO_SHIFT_2_NAME, columnShift);
        }

        private void AddTableHeaderColumn(AssetType assetType, ResidualValueTableColumns column, ExcelWorksheet sheet)
        {
            var names = sheet.Names;
            var mapping = GetTableMapping(assetType);
            var col = mapping[column];
            var headerCell = names[ResidualValueReportHelper.FIELD_TABLE_HEADER];
            var columnNumberCell = names[ResidualValueReportHelper.FIELD_TABLE_COLUMN_NUMBER];
            var cellCount = names[ResidualValueReportHelper.FIELD_NUMBER_NAMES];

            var index = col.ColumnIndex;
            var title = col.ColumnTitle;
            var number = col.ColumnNumber;

            var summaryRowNumber = cellCount.Start.Row;
            var headerRow1 = headerCell.Start.Row;
            var headerRow2 = headerRow1 + 1;
            var headerNumbersRow = columnNumberCell.Start.Row;

            sheet.Column(index).Width = 14;
            sheet.FillPseudoTableCell(title, headerRow1, index, headerRow2, index, true);
            sheet.FillPseudoTableCell(number.ToString(), headerNumbersRow, index);
            var summaryCell = sheet.FillPseudoTableCell(string.Empty, summaryRowNumber, index);
            summaryCell.Style.Font.Bold = true;
            if (!string.IsNullOrEmpty(col.Format))
            {
                summaryCell.Style.Numberformat.Format = col.Format;
            }
        }

        private void FillTheTable(WriteOffReportData reportData, ExcelWorksheet sheet)
        {
            var table = GetDataTable(sheet);
            var columnsMapping = GetTableMapping(reportData.AssetType);

            var assets = reportData.Assets;

            int firstRow = table.Address.Start.Row + 1; // Перший рядок таблиці

            if (assets.Count > 1)
            {
                table.AddRow(assets.Count - 1);
            }

            RemoveUnneccessaryColumns(reportData.AssetType, sheet);

            for (int i = 0; i < assets.Count; i++)
            {
                var asset = assets[i];
                var newRow = firstRow + i; // Новий рядок

                var assetName = ReportHelper.GetFullAssetName(asset.Name, asset.SerialNumber);
                var indexationCoefficient = CoefficientsHelper.GetIndexationCoefficient(asset.StartDate, reportData.ReportDate);
                var residualPrice = ResidualPriceHelper.CalculateResidualPriceForItem(asset, asset.Count);

                sheet.Cells[newRow, columnsMapping[ResidualValueTableColumns.Index].ColumnIndex].Value = i + 1; // number
                sheet.Cells[newRow, columnsMapping[ResidualValueTableColumns.Name].ColumnIndex].Value = assetName;
                sheet.Cells[newRow, columnsMapping[ResidualValueTableColumns.MeasurementUnit].ColumnIndex].Value = asset.MeasurementUnit;
                sheet.Cells[newRow, columnsMapping[ResidualValueTableColumns.Count].ColumnIndex].Value = asset.Count;
                sheet.Cells[newRow, columnsMapping[ResidualValueTableColumns.Price].ColumnIndex].Value = asset.Price;

                sheet.Cells[newRow, columnsMapping[ResidualValueTableColumns.IndexationCoefficient].ColumnIndex].Value = indexationCoefficient;
                sheet.Cells[newRow, columnsMapping[ResidualValueTableColumns.CurrencyConversionRate].ColumnIndex].Value = "-";

                FillCoefficients(asset, newRow, sheet);

                sheet.Cells[newRow, columnsMapping[ResidualValueTableColumns.ResidualValue].ColumnIndex].Value = residualPrice;
                sheet.Cells[newRow, columnsMapping[ResidualValueTableColumns.ValuationReportReference].ColumnIndex].Value = "-";

                sheet.AutoFitRowHeight(newRow, ResidualValueReportHelper.TABLE_COLUMN_NAME);
            }

            var names = sheet.Names;
            var cellCount = names[ResidualValueReportHelper.FIELD_NUMBER_NAMES];
            cellCount.Value = ReportHelper.ConvertNamesNumberToReportString(assets.Count);

            var cellSum = sheet.Cells[cellCount.Start.Row, columnsMapping[ResidualValueTableColumns.ResidualValue].ColumnIndex];
            cellSum.Value = CalculateTotalResidualValue(reportData.Assets);
        }

        private Dictionary<ResidualValueTableColumns, BaseColumnInfo> GetTableMapping(AssetType assetType)
        {
            if (_tableMappings.TryGetValue(assetType, out var mapping))
            {
                return mapping;
            }

            var tableMapping = new Dictionary<ResidualValueTableColumns, BaseColumnInfo>
            {
                { ResidualValueTableColumns.Index, new BaseColumnInfo(ResidualValueReportHelper.TABLE_COLUMN_INDEX, 1) },
                { ResidualValueTableColumns.Name, new BaseColumnInfo(ResidualValueReportHelper.TABLE_COLUMN_NAME, 2) },
                { ResidualValueTableColumns.MeasurementUnit, new BaseColumnInfo(ResidualValueReportHelper.TABLE_COLUMN_MEASUREMENT_UNIT, 3) },
                { ResidualValueTableColumns.Count, new BaseColumnInfo(ResidualValueReportHelper.TABLE_COLUMN_COUNT, 4) },
                { ResidualValueTableColumns.Price, new BaseColumnInfo(ResidualValueReportHelper.TABLE_COLUMN_PRICE, 5) },
                { ResidualValueTableColumns.IndexationCoefficient, new BaseColumnInfo(ResidualValueReportHelper.TABLE_COLUMN_INDEXATION_COEFF, 6) },
                { ResidualValueTableColumns.CurrencyConversionRate, new BaseColumnInfo(ResidualValueReportHelper.TABLE_COLUMN_CURRENCY_CONVERSION_RATE, 8) }
            };

            var coeffColumns = ResidualValueReportHelper.GetCoefficientColumns(assetType);

            var totalWearCoeffColumn = ResidualValueReportHelper.TABLE_FIRST_COEFF_COLUMN + coeffColumns.Count;
            var residualSumColumn = totalWearCoeffColumn + 1;
            var valuationReportRefColumn = residualSumColumn + 1;

            var coeffColumnNumber = coeffColumns.Count > 0 ? ResidualValueReportHelper.TABLE_FIRST_COEFF_COLUMN + 1 : ResidualValueReportHelper.TABLE_FIRST_COEFF_COLUMN;
            var residualSumColumnNumber = coeffColumnNumber + 1;
            var valuationReportRefColumnNumber = residualSumColumnNumber + 1;

            tableMapping.Add(ResidualValueTableColumns.TotalWearCoefficient, new BaseColumnInfo(totalWearCoeffColumn, coeffColumnNumber, ResidualValueReportStrings.TotalWearCoefficientColumnTitle));
            tableMapping.Add(ResidualValueTableColumns.ResidualValue, new BaseColumnInfo(residualSumColumn, residualSumColumnNumber, ResidualValueReportStrings.ResidualValueColumnTitle, ResidualValueReportHelper.RESIDUAL_VALUE_SUM_FORMAT));
            tableMapping.Add(ResidualValueTableColumns.ValuationReportReference, new BaseColumnInfo(valuationReportRefColumn, valuationReportRefColumnNumber, ResidualValueReportStrings.ValuationReportReferenceColumnTitle));

            _tableMappings[assetType] = tableMapping;

            return tableMapping;
        }

        private void RemoveUnneccessaryColumns(AssetType assetType, ExcelWorksheet sheet)
        {
            var coeffColumns = ResidualValueReportHelper.GetCoefficientColumns(assetType);
            // оновити таблицю з даними
            var mapping = GetTableMapping(assetType);
            var actualColumnsCount = mapping[ResidualValueTableColumns.ValuationReportReference].ColumnIndex;
            var table = GetDataTable(sheet);

            var countToDelete = table.Columns.Count - actualColumnsCount;
            if (countToDelete > 0)
            {
                var firstCol = ResidualValueReportHelper.TABLE_FIRST_COEFF_COLUMN;
                table.Columns.Delete(firstCol, countToDelete);
            }
        }

        private static decimal CalculateTotalResidualValue(IList<IAssetInfo> assets)
        {
            var totalValue = 0m;

            foreach (var asset in assets)
            {
                var residualPrice = ResidualPriceHelper.CalculateResidualPriceForItem(asset, asset.Count);
                totalValue += residualPrice;
            }

            return totalValue;
        }

        private static ExcelTable GetDataTable(ExcelWorksheet sheet)
        {
            var table = sheet.Tables[0];
            return table;
        }

        private static void FillCoefficients(IAssetInfo asset, int row, ExcelWorksheet sheet)
        {
            var coefficients = asset.GetCoefficients();

            var column = ResidualValueReportHelper.TABLE_FIRST_COEFF_COLUMN;

            if (coefficients != null)
            {
                foreach (var coefficient in coefficients)
                {
                    sheet.Cells[row, column].Value = coefficient;
                    column++;
                }
            }

            sheet.Cells[row, column].Value = asset.TotalWearCoefficient;
        }

    }

    class BaseColumnInfo
    {
        public int ColumnIndex { get; set; }
        public int ColumnNumber { get; set; }
        public string ColumnTitle { get; set; }
        public string Format { get; set; }

        public BaseColumnInfo(int index) : this(index, index, string.Empty, string.Empty)
        {
        }

        public BaseColumnInfo(int index, int number) : this(index, number, string.Empty, string.Empty)
        {
        }

        public BaseColumnInfo(int index, int number, string title) : this(index, number, title, string.Empty)
        {
        }

        public BaseColumnInfo(int index, int number, string title, string format)
        {
            ColumnIndex = index;
            ColumnNumber = number;
            ColumnTitle = title;
            Format = format;
        }
    }
}
