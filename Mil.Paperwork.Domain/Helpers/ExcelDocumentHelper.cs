using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using System.Text.RegularExpressions;

namespace Mil.Paperwork.Domain.Helpers
{
    internal static class ExcelDocumentHelper
    {
        private const string FIELD_RANGE_PATTERN = @"^FIELD_RANGE_\d+$";
        private const string TEXT_FIELD_FORMAT = "[{0}]";
        private const string TEXT_FIELD_PATTERN = @"\[(.*?)\]";

        public static void AutoFitRowHeight(ExcelWorksheet worksheet, int rowIndex, int columnIndex)
        {
            var row = worksheet.Row(rowIndex);
            var cell = worksheet.Cells[rowIndex, columnIndex];
            var text = cell.Text;
            var font = cell.Style.Font;
            var width = worksheet.Column(columnIndex).Width;

            var height = MeasureTextHeight(text, font, width);
            row.Height = height;
        }

        public static double MeasureTextHeight(string text, ExcelFont font, double width)
        {
            if (string.IsNullOrEmpty(text)) return 0.0;

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

        public static void MadDataToTheNamedFields(this ExcelWorksheet sheet, Dictionary<string, string> fieldsMap)
        {
            var namedFields = sheet.GetNamedFieldRanges();

            foreach (var namedField in namedFields)
            {
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

        private static IEnumerable<string> GetFields(string text)
        {
            var matches = Regex.Matches(text, TEXT_FIELD_PATTERN);
            foreach (Match match in matches)
            {
                yield return match.Groups[1].Value;
            }
        }
    }
}
