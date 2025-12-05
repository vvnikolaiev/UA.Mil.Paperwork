using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
#if WINDOWS
using System.Drawing;
#endif

namespace Mil.Paperwork.Domain.Helpers
{
    internal static class ExcelDocumentHelper
    {
        private const string FIELD_RANGE_PATTERN = @"^FIELD_RANGE_\d+$";
        private const string TEXT_FIELD_FORMAT = "[{0}]";
        private const string TEXT_FIELD_PATTERN = @"\[(.*?)\]";

        public static void AutoFitRowHeight(this ExcelWorksheet worksheet, int rowIndex, int startColumnIndex)
        {
            worksheet.AutoFitRowHeight(rowIndex, startColumnIndex, startColumnIndex);
        }

        public static void AutoFitRowHeight(this ExcelWorksheet worksheet, int rowIndex, int startColumnIndex, int endColumnIndex)
        {
            var row = worksheet.Row(rowIndex);
            var cell = worksheet.Cells[rowIndex, startColumnIndex, rowIndex, endColumnIndex];
            var text = cell.Text;
            var font = cell.Style.Font;

            var width = 0d;
            for (int i = cell.Start.Column; i <= cell.End.Column; i++)
            {
                width += worksheet.Column(i).Width;
            }

            var height = MeasureTextHeight(text, font, width);
            row.Height = height;
        }

        public static double MeasureTextHeight(string text, ExcelFont font, double width)
        {
            if (string.IsNullOrEmpty(text)) return 0.0;

            #if WINDOWS
                return MeasureTextHeightWindows(text, font, width);
            #else
                return MeasureTextHeightCrossPlatform(text, font, width);
            #endif
        }

#if WINDOWS
        private static double MeasureTextHeightWindows(string text, ExcelFont font, double width)
        {
            using (var bitmap = new Bitmap(1, 1))
            using (var graphics = Graphics.FromImage(bitmap))
            {
                var pixelWidth = Convert.ToInt32(width * 7); // 7 pixels per Excel column width
                var fontSize = font.Size * 0.75f;
                var drawingFont = new Font(font.Name, fontSize);
                var size = graphics.MeasureString(text, drawingFont, pixelWidth, new StringFormat { FormatFlags = StringFormatFlags.MeasureTrailingSpaces });

                // 72 DPI and 96 points per inch. Excel height in points with max of 409 per Excel requirements.
                return Math.Min(Convert.ToDouble(size.Height) * 72 / 96, 409);
            }
        }
#else
        private static double MeasureTextHeightCrossPlatform(string text, ExcelFont font, double width)
        {
            var fontSize = font.Size * 0.75;
            var pixelWidth = width * 7; // 7 pixels per Excel column width
            var charsPerLine = Math.Max(1, (int)(pixelWidth / (fontSize * 0.6))); // Approximate chars per line
            var lineCount = Math.Max(1, (int)Math.Ceiling((double)text.Length / charsPerLine));
            var estimatedHeight = lineCount * fontSize * 1.2; // Add 20% for line spacing
            
            // Excel height in points with max of 409 per Excel requirements
            return Math.Min(estimatedHeight, 409);
        }
#endif
        public static void MapDataToTheNamedFields(this ExcelWorksheet sheet, Dictionary<string, string> fieldsMap)
        {
            var namedFields = sheet.GetNamedFieldRanges();

            foreach (var namedField in namedFields)
            {
                if (namedField.Start.Row == 0 || namedField.Start.Column == 0)
                {
                    continue;
                }

                var isFormula = !string.IsNullOrEmpty(namedField.Formula);
                var text = isFormula ? namedField.Formula : namedField.Text;
                var fields = GetFields(text);

                foreach (var field in fields)
                {
                    if (fieldsMap.TryGetValue(field, out var value))
                    {
                        var fieldPlaceholder = string.Format(TEXT_FIELD_FORMAT, field);
                        text = text.Replace(fieldPlaceholder, value);
                    }
                }

                if (isFormula)
                {
                    namedField.Formula = text;
                }
                else
                {
                    namedField.Value = text;
                }

                var mergedCell = sheet.MergedCells[namedField.Start.Row, namedField.Start.Column];
                var range = string.IsNullOrEmpty(mergedCell) ? sheet.Cells[namedField.Start.Row, namedField.Start.Column]
                                                             : sheet.Cells[mergedCell];
                sheet.AutoFitRowHeight(range.Start.Row, range.Start.Column, range.End.Column);
            }
        }

        public static ExcelRange FillPseudoTableCell(this ExcelWorksheet sheet, string value, int startRow, int startColumn)
        {
            var result = sheet.FillPseudoTableCell(value, startRow, startColumn, startRow, startColumn, false);
            return result;
        }

        public static ExcelRange FillPseudoTableCell(this ExcelWorksheet sheet, string value, int startRow, int startColumn, int endRow, int endColumn, bool merge)
        {
            var tableCell = sheet.Cells[startRow, startColumn, endRow, endColumn];
            tableCell.Merge = merge;
            tableCell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            tableCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            tableCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            tableCell.Style.WrapText = true;
            tableCell.Value = value;

            return tableCell;
        }

        public static void SetFormula(this ExcelWorksheet sheet, string formula, int row, int column)
        {
            var сell = sheet.Cells[row, column];
            сell.Formula = formula;
        }

        public static void ShrinkMergedNamedRange(this ExcelWorksheet sheet, string rangeTitleId, int endColumn)
        {
            var rangeTitle = sheet.Names[rangeTitleId];
            for (int i = rangeTitle.Start.Row; i <= rangeTitle.End.Row; i++)
            {
                var titleCellAddress = sheet.MergedCells[i, rangeTitle.Start.Column];
                var mergedCell = sheet.Cells[titleCellAddress];

                var value = mergedCell.Text;
                mergedCell.Merge = false;

                var newMergeRange = sheet.Cells[i, rangeTitle.Start.Column, i, endColumn];
                newMergeRange.Merge = true;
            }
        }

        public static void ShiftMergedCellsInsideNamedRange(this ExcelWorksheet sheet, string rangeTitleId, int colShift)
        {
            if (colShift == 0)
            {
                return;
            }

            var range = sheet.Names[rangeTitleId];

            var allMergedCells = sheet.MergedCells;

            List<ExcelAddress> mergedInsideList;
            var mergedInside = allMergedCells
                .Where(addr => !string.IsNullOrEmpty(addr))
                .Select(addr => new ExcelAddress(addr))
                .Where(m => m.Start.Row >= range.Start.Row &&
                            m.End.Row <= range.End.Row &&
                            m.Start.Column >= range.Start.Column &&
                            m.End.Column <= range.End.Column)
                .DistinctBy(m => m.Address);

            // depending on where we shift columns order ranges to avoid overlapping
            if (colShift < 0)
            {
                mergedInsideList = mergedInside.OrderBy(x => x.Start.Column).ToList();
            }
            else
            {
                mergedInsideList = mergedInside.OrderByDescending(x => x.Start.Column).ToList();
            }

            foreach (var merge in mergedInsideList)
            {
                var oldMergedCells = sheet.Cells[merge.Address];

                var value = oldMergedCells.Text;
                oldMergedCells.Value = string.Empty;
                oldMergedCells.Merge = false;

                var startRow = merge.Start.Row;
                var startColumn = merge.Start.Column + colShift;
                var endRow = merge.End.Row;
                var endColumn = merge.End.Column + colShift;

                var newMergedCells = sheet.Cells[startRow, startColumn, endRow, endColumn];
                newMergedCells.Merge = true;
                newMergedCells.Value = value;


                var matchingNamedRanges = sheet.Names.Where(x => x.Start.Column == merge.Start.Column && x.Start.Row == merge.Start.Row).ToList();
                foreach (var matchingNamedRange in matchingNamedRanges)
                {
                    // Create the new address
                    var newAddr = new ExcelAddress(startRow, startColumn, endRow, endColumn);

                    // Remove and recreate with the new address
                    sheet.Names.Remove(matchingNamedRange.Name);
                    sheet.Names.Add(matchingNamedRange.Name, sheet.Cells[newAddr.Address]);
                }
            }
        }

        public static List<string> GetHeaders(int headerRow, ExcelWorksheet worksheet)
        {
            int lastCol = worksheet.Dimension.End.Column;

            var headers = new List<string>();

            for (int col = 1; col <= lastCol; col++)
            {
                string colName = string.Empty;
                if (headerRow > 0)
                {
                    var cellValue = worksheet.Cells[headerRow, col].Text;
                    colName = cellValue?.Trim() ?? string.Empty;
                }

                if (string.IsNullOrEmpty(colName))
                {
                    colName = $"Column{col}";
                }

                headers.Add(colName);
            }

            return headers;
        }

        private static IEnumerable<string> GetFields(string text)
        {
            var matches = Regex.Matches(text, TEXT_FIELD_PATTERN);
            foreach (Match match in matches)
            {
                yield return match.Groups[1].Value;
            }
        }

        private static IEnumerable<ExcelNamedRange> GetNamedFieldRanges(this ExcelWorksheet sheet)
        {
            var namedRangess = sheet.Names;
            foreach (var namedRange in namedRangess)
            {
                var name = namedRange.Name;
                if (Regex.IsMatch(name, FIELD_RANGE_PATTERN))
                {
                    yield return namedRange;
                }
            }
        }
    }
}
