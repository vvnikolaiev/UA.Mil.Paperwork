using Mil.Paperwork.Domain.Calculators;
using Mil.Paperwork.Domain.DataModels.Assets;
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

        public bool TryCreate(IAssetInfo asset, IDictionary<MetalType, decimal> metalCosts, AssetType assetType, DateTime reportDate)
        {
            bool result = false;

            try
            {
                result = TryFillTheReport(asset, metalCosts, assetType, reportDate);

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

        private bool TryFillTheReport(IAssetInfo asset, IDictionary<MetalType, decimal> metalCosts, AssetType assetType, DateTime reportDate)
        {
            if (asset == null)
            {
                return false;
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var templatePath = PathsHelper.GetTemplatePath(ResidualValueReportHelper.REPORT_TEMPLATE_NAME);
            using var package = new ExcelPackage(templatePath);
            var sheet = package.Workbook.Worksheets[0]; // Отримуємо перший аркуш

            PrepareTheTemplate(assetType, sheet);

            FillTheTable(asset, assetType, reportDate, sheet);

            FillTableMetals(metalCosts, sheet);

            var fieldsMap = _reportDataService.GetReportParametersDictionary(ReportType.ResidualValueReport);
            fieldsMap.Add(ResidualValueReportHelper.REPORT_DATE_PLACEHOLDER, reportDate.ToString(ReportHelper.DATE_FORMAT));
            
            var residualPrice = ResidualPriceHelper.CalculateResidualPriceForItem(asset, reportDate, asset.Count);
            fieldsMap.Add(ResidualValueReportHelper.TOTAL_RESIDUAL_SUM_PLACEHOLDER, residualPrice.ToString("N", ReportHelper.PriceNumberFormatInfo));
            
            fieldsMap.Add(ResidualValueReportHelper.ASSET_NAME, ReportHelper.GetFullAssetName(asset.Name, asset.SerialNumber));
            fieldsMap.Add(ResidualValueReportHelper.ASSEMBLY_YEAR, asset.YearManufactured.ToString());
            fieldsMap.Add(ResidualValueReportHelper.COMMISSIONED_YEAR, asset.StartDate.Year.ToString());

            sheet.MapDataToTheNamedFields(fieldsMap);

            using var reportStream = new MemoryStream();
            package.SaveAs(reportStream);
            _reportBytes = reportStream.ToArray();

            return true;
        }

        private void PrepareTheTemplate(AssetType assetType, ExcelWorksheet sheet)
        {
            // update columns depending on coefficient count
            var names = sheet.Names;
            var mapping = GetTableMapping(assetType);

            var headerCell = names[ResidualValueReportHelper.FIELD_TABLE_HEADER];
            var columnNumberCell = names[ResidualValueReportHelper.FIELD_TABLE_COLUMN_NUMBER];

            var headerRow1 = headerCell.Start.Row;
            var headerRow2 = headerRow1 + 1;
            var headerNumbersRow = columnNumberCell.Start.Row;

            var coeffColumns = ResidualValueReportHelper.GetCoefficientColumns(assetType);

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
                    //sheet.FillPseudoTableCell(string.Empty, countRowNumber, columnNum);
                }

                var endColumn = coeffStartColumn + coeffColumns.Count - 1;

                var coefsText = ResidualValueReportStrings.CoefficientsMergedColumnTitle;
                sheet.FillPseudoTableCell(coefsText, headerRow1, coeffStartColumn, headerRow1, endColumn, true);
                sheet.FillPseudoTableCell("9", headerNumbersRow, coeffStartColumn, headerNumbersRow, endColumn, true);
            }

            // Column Total Coefficient
            AddTableHeaderColumn(assetType, ResidualValueTableColumns.TotalWearCoefficient, sheet);
            // Column Total sum
            AddTableHeaderColumn(assetType, ResidualValueTableColumns.ResidualValue, sheet);
            // Column Total sum
            AddTableHeaderColumn(assetType, ResidualValueTableColumns.ValuationReportReference, sheet);

            // Виправити об'єднані клітинки над таблицею так, щоби вони займали таку ж кількість стовпців
            var lastColumn = mapping[ResidualValueTableColumns.ValuationReportReference];
            sheet.ShrinkMergedNamedRange(ResidualValueReportHelper.RANGE_TO_SHRINK_1_NAME, lastColumn.ColumnIndex);
            //sheet.ShrinkMergedNamedRange(ResidualValueReportHelper.RANGE_TO_SHRINK_2_NAME, lastColumn.ColumnIndex);

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
            //var cellCount = names[ResidualValueReportHelper.FIELD_NUMBER_NAMES];

            var index = col.ColumnIndex;
            var title = col.ColumnTitle;
            var number = col.ColumnNumber;

            //var summaryRowNumber = cellCount.Start.Row;
            var headerRow1 = headerCell.Start.Row;
            var headerRow2 = headerRow1 + 1;
            var headerNumbersRow = columnNumberCell.Start.Row;

            sheet.Column(index).Width = 14;
            sheet.FillPseudoTableCell(title, headerRow1, index, headerRow2, index, true);
            sheet.FillPseudoTableCell(number.ToString(), headerNumbersRow, index);
            //var summaryCell = sheet.FillPseudoTableCell(string.Empty, summaryRowNumber, index);
            //summaryCell.Style.Font.Bold = true;
            //if (!string.IsNullOrEmpty(col.Format))
            //{
            //    summaryCell.Style.Numberformat.Format = col.Format;
            //}
        }

        private void FillTheTable(IAssetInfo assetInfo, AssetType assetType, DateTime reportDate, ExcelWorksheet sheet)
        {
            var table = GetDataTable(sheet);
            var columnsMapping = GetTableMapping(assetType);

            var asset = assetInfo;

            int firstRow = table.Address.Start.Row + 1; // Перший рядок таблиці

            RemoveUnneccessaryColumns(assetType, sheet);

            int i = 0;
            
            var newRow = firstRow + i; // Новий рядок

            var assetName = ReportHelper.GetFullAssetName(asset.Name, asset.SerialNumber);
            var yearManufactured = asset.YearManufactured > 1900 ? asset.YearManufactured : asset.StartDate.Year;
            var indexationCoefficient = CoefficientsHelper.GetIndexationCoefficient(yearManufactured, reportDate.Year);
            var residualPrice = ResidualPriceHelper.CalculateResidualPriceForItem(asset, reportDate, asset.Count);

            sheet.Cells[newRow, columnsMapping[ResidualValueTableColumns.Index].ColumnIndex].Value = i + 1; // number
            sheet.Cells[newRow, columnsMapping[ResidualValueTableColumns.Name].ColumnIndex].Value = assetName;
            sheet.Cells[newRow, columnsMapping[ResidualValueTableColumns.MeasurementUnit].ColumnIndex].Value = asset.MeasurementUnit;
            sheet.Cells[newRow, columnsMapping[ResidualValueTableColumns.Count].ColumnIndex].Value = asset.Count;
            sheet.Cells[newRow, columnsMapping[ResidualValueTableColumns.Price].ColumnIndex].Value = asset.Price;

            sheet.Cells[newRow, columnsMapping[ResidualValueTableColumns.IndexationCoefficient].ColumnIndex].Value = indexationCoefficient;
            sheet.Cells[newRow, columnsMapping[ResidualValueTableColumns.CurrencyConversionRate].ColumnIndex].Value = "-";

            FillCoefficients(asset, reportDate, newRow, sheet);

            sheet.Cells[newRow, columnsMapping[ResidualValueTableColumns.ResidualValue].ColumnIndex].Value = residualPrice;
            sheet.Cells[newRow, columnsMapping[ResidualValueTableColumns.ValuationReportReference].ColumnIndex].Value = "-";

            sheet.AutoFitRowHeight(newRow, ResidualValueReportHelper.TABLE_COLUMN_NAME);
            
            var names = sheet.Names;
        }

        private void FillTableMetals(IDictionary<MetalType, decimal> metalCosts, ExcelWorksheet sheet)
        {
            // вводити руками. На жаль.
            // але спробувати замапити таблицю кольорових металів

            var names = sheet.Names;
            var table = names[ResidualValueReportHelper.TABLE_METALS_NAME];

            var startRow = table.Start.Row;
            var startCol = table.Start.Column;

            var colWeight = startCol + ResidualValueReportHelper.TABLE_METALS_COL_WEIGHT;
            var colTotalWeight = startCol + ResidualValueReportHelper.TABLE_METALS_COL_TOTAL_WEIGHT;
            var colPrice = startCol + ResidualValueReportHelper.TABLE_METALS_COL_PRICE;

            foreach (var metalCost in metalCosts)
            {
                if (ResidualValueReportHelper.TABLE_METALS_ROWS.TryGetValue(metalCost.Key, out var metalRow))
                {
                    var row = startRow + metalRow;
                    sheet.Cells[row, colPrice].Value = metalCost.Value;
                }
            }

            //// get actual prices
            //var metals = await PreciousMetalsExchangeApiHelper.GetMetalRatesAsync(reportDate);

            //foreach (var metal in metals)
            //{
            //    if (ResidualValueReportHelper.TABLE_METALS_ROWS.TryGetValue(metal.Key, out var metalRow))
            //    {
            //        var row = startRow + metalRow;
            //        sheet.Cells[row, colPrice].Value = metal.Value.Rate;
            //    }

            //}

            // get from the Product instance!
            //Dictionary<MetalType, decimal> metalContent = new Dictionary<MetalType, decimal>();
            // foreach (var metal in metalContent) ...
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

        private static ExcelTable GetDataTable(ExcelWorksheet sheet)
        {
            var table = sheet.Tables[0];
            return table;
        }

        private static void FillCoefficients(IAssetInfo asset, DateTime reportDate, int row, ExcelWorksheet sheet)
        {
            var residualPriceCalculator = ResidualPriceCalculatorFactory.CreateCalculator(asset.Service);
            var coefficients = residualPriceCalculator.GetCoefficients(asset, reportDate);

            var column = ResidualValueReportHelper.TABLE_FIRST_COEFF_COLUMN;

            if (coefficients != null)
            {
                foreach (var coefficient in coefficients)
                {
                    sheet.Cells[row, column].Value = coefficient;
                    column++;
                }
            }

            var totalWearCoefficient = residualPriceCalculator.CalculateTotalWearCoefficient(asset, reportDate);
            sheet.Cells[row, column].Value = totalWearCoefficient;
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
