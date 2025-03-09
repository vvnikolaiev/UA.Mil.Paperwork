using Spire.Doc.Documents;
using Spire.Doc;
using Spire.Doc.Fields;

namespace Mil.Paperwork.Domain.Helpers
{
    internal static class WordDocumentHelper
    {
        public const string DOCUMENT_FONT_NAME = "Times New Roman";

        private const string StartFieldSymbol = "«";
        private const string EndFieldSymbol = "»";

        public static void ReplaceFields(this Document document, Dictionary<string, string> fieldsMap)
        {
            if (fieldsMap != null)
            {
                foreach (var field in fieldsMap)
                {
                    document.ReplaceField(field.Key, field.Value);
                }
            }
        }

        public static void ReplaceField(this Document document, string fieldName, string value)
        {
            if (!string.IsNullOrEmpty(fieldName))
            {
                if (!fieldName.StartsWith(StartFieldSymbol))
                {
                    fieldName = StartFieldSymbol + fieldName;
                }
                if (!fieldName.EndsWith(EndFieldSymbol))
                {
                    fieldName += EndFieldSymbol;
                }


                document.Replace(fieldName, value, false, true);
            }
        }

        public static void AddNumber(this TableCell cell, int value, int fontSize = 12, HorizontalAlignment horizontalAlignment = HorizontalAlignment.Center)
        {
            cell.AddText(value.ToString(), fontSize, horizontalAlignment);
        }

        public static void AddPrice(this TableCell cell, decimal value, int fontSize = 12, HorizontalAlignment horizontalAlignment = HorizontalAlignment.Center)
        {
            cell.AddText(value.ToString("F2"), fontSize, horizontalAlignment);
        }

        public static void AddText(this TableCell cell, string text, int fontSize = 12, HorizontalAlignment horizontalAlignment = HorizontalAlignment.Center)
        {
            cell.Paragraphs.Clear(); // Clear the cell before appending text
            cell.AddParagraph().AppendText(text);

            foreach (Paragraph paragraph in cell.Paragraphs)
            {
                paragraph.Format.HorizontalAlignment = horizontalAlignment;
                paragraph.Format.BeforeSpacing = 0;
                paragraph.Format.AfterSpacing = 0;

                foreach (DocumentObject obj in paragraph.ChildObjects)
                {
                    if (obj is TextRange textRange)
                    {
                        textRange.CharacterFormat.FontSize = fontSize;
                        textRange.CharacterFormat.FontName = DOCUMENT_FONT_NAME;
                    }
                }
            }
        }

        public static TableCell CreateMergedCell(this TableRow tableRow, int firstColumn, int count)
        {
            tableRow.Cells[firstColumn].CellFormat.HorizontalMerge = CellMerge.Start;
            for (int i = firstColumn + 1; i < count; i++)
            {
                tableRow.Cells[i].CellFormat.HorizontalMerge = CellMerge.Continue;
            }

            return tableRow.Cells[firstColumn];
        }
    }
}
