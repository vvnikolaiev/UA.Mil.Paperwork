using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Mil.Paperwork.Domain.DataModels.Parameters;
using System.IO;
using System.Text;

namespace Mil.Paperwork.Domain.Helpers
{
    // ---------------------------------------------------------------------------
    // WordDocument — top-level wrapper. Report classes use ONLY these four types.
    // ---------------------------------------------------------------------------

    internal sealed class WordDocument : IDisposable
    {
        private readonly MemoryStream _stream;
        private WordprocessingDocument? _wdoc;
        private readonly Body _body;
        private bool _finalized;

        private WordDocument(MemoryStream stream, WordprocessingDocument wdoc)
        {
            _stream = stream;
            _wdoc = wdoc;
            _body = wdoc.MainDocumentPart!.Document.Body!;
        }

        public static WordDocument LoadFromFile(string path)
        {
            var bytes = File.ReadAllBytes(path);
            var stream = new MemoryStream(bytes.Length + 65536);
            stream.Write(bytes, 0, bytes.Length);
            stream.Position = 0;
            var wdoc = WordprocessingDocument.Open(stream, isEditable: true);
            return new WordDocument(stream, wdoc);
        }

        public WordTable? GetTable(string tableName)
        {
            var table = _body.Descendants<Table>()
                .FirstOrDefault(t => t.GetFirstChild<TableProperties>()
                                      ?.GetFirstChild<TableCaption>()?.Val?.Value == tableName);
            return table != null ? new WordTable(table) : null;
        }

        public WordTable? GetTableByIndex(int index)
        {
            var table = _body.Descendants<Table>().ElementAtOrDefault(index);
            return table != null ? new WordTable(table) : null;
        }

        public void ReplaceField(string fieldName, string? value)
        {
            value ??= string.Empty;
            var cleanName = fieldName.Trim('«', '»');

            foreach (var para in _body.Descendants<Paragraph>())
            {
                // Handle simple fields (w:fldSimple) — used in some table cells
                foreach (var fld in para.Elements<SimpleField>())
                {
                    var instr = fld.Instruction?.Value ?? string.Empty;
                    var fldParts = instr.Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    if (!fldParts.Contains("MERGEFIELD") || !fldParts.Contains(cleanName)) continue;

                    var existingRPr = fld.GetFirstChild<Run>()?.GetFirstChild<RunProperties>();
                    fld.Elements<Run>().ToList().ForEach(r => r.Remove());

                    var newRun = new Run();
                    if (existingRPr != null)
                        newRun.Append((RunProperties)existingRPr.CloneNode(true));
                    newRun.Append(new Text(value) { Space = SpaceProcessingModeValues.Preserve });
                    fld.Append(newRun);
                }

                // Handle complex fields (w:fldChar begin/separate/end)
                var runs = para.Elements<Run>().ToList();
                for (int i = 0; i < runs.Count; i++)
                {
                    if (GetFldCharType(runs[i]) != FieldCharValues.Begin) continue;

                    // Collect instrText between Begin and Separate
                    int sepIdx = -1;
                    var instrBuilder = new StringBuilder();
                    for (int j = i + 1; j < runs.Count; j++)
                    {
                        var charType = GetFldCharType(runs[j]);
                        if (charType == FieldCharValues.Separate) { sepIdx = j; break; }
                        if (charType == FieldCharValues.End) break;
                        instrBuilder.Append(runs[j].GetFirstChild<FieldCode>()?.InnerText ?? string.Empty);
                    }
                    if (sepIdx == -1) continue;

                    var instrParts = instrBuilder.ToString()
                        .Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    if (!instrParts.Contains("MERGEFIELD") || !instrParts.Contains(cleanName)) continue;

                    // Find End run
                    int endIdx = -1;
                    for (int j = sepIdx + 1; j < runs.Count; j++)
                    {
                        if (GetFldCharType(runs[j]) == FieldCharValues.End) { endIdx = j; break; }
                    }
                    if (endIdx == -1) continue;

                    // Clone formatting from first display run
                    var displayRuns = runs.Skip(sepIdx + 1).Take(endIdx - sepIdx - 1).ToList();
                    var existingRPr = displayRuns.FirstOrDefault()?.GetFirstChild<RunProperties>();

                    var newRun = new Run();
                    if (existingRPr != null)
                        newRun.Append((RunProperties)existingRPr.CloneNode(true));
                    newRun.Append(new Text(value) { Space = SpaceProcessingModeValues.Preserve });

                    foreach (var dr in displayRuns) dr.Remove();
                    runs[endIdx].InsertBeforeSelf(newRun);
                    break;
                }
            }
        }

        public void ReplaceFields(Dictionary<string, string> fieldsMap)
        {
            if (fieldsMap == null) return;
            foreach (var field in fieldsMap)
                ReplaceField(field.Key, field.Value);
        }

        public void ReplaceFieldWithBlock(string fieldName, IList<BlockParagraph> paragraphs)
        {
            var cleanName = fieldName.Trim('«', '»');
            Paragraph? targetParagraph = null;

            foreach (var para in _body.Descendants<Paragraph>())
            {
                if (ContainsMergeField(para, cleanName))
                {
                    targetParagraph = para;
                    break;
                }
            }

            if (targetParagraph == null)
                return;

            var pPr = targetParagraph.GetFirstChild<ParagraphProperties>();

            foreach (var bp in paragraphs)
            {
                var newPara = new Paragraph();

                var newPPr = new ParagraphProperties();
                if (pPr != null)
                {
                    var clonedSpacing = pPr.GetFirstChild<SpacingBetweenLines>();
                    if (clonedSpacing != null)
                        newPPr.Append((SpacingBetweenLines)clonedSpacing.CloneNode(true));
                }

                if (bp.IndentLevel > 0)
                {
                    newPPr.Append(new Indentation { Left = (bp.IndentLevel * 720).ToString() });
                }

                if (newPPr.HasChildren)
                    newPara.Append(newPPr);

                if (!string.IsNullOrEmpty(bp.Text))
                {
                    var rPr = new RunProperties();
                    rPr.Append(new RunFonts
                    {
                        Ascii = WordDocumentHelper.DOCUMENT_FONT_NAME,
                        HighAnsi = WordDocumentHelper.DOCUMENT_FONT_NAME,
                        ComplexScript = WordDocumentHelper.DOCUMENT_FONT_NAME
                    });
                    rPr.Append(new FontSize { Val = "24" });
                    rPr.Append(new FontSizeComplexScript { Val = "24" });
                    if (bp.IsBold)
                        rPr.Append(new Bold());

                    var run = new Run();
                    run.Append(rPr);
                    run.Append(new Text(bp.Text) { Space = SpaceProcessingModeValues.Preserve });
                    newPara.Append(run);
                }

                targetParagraph.InsertBeforeSelf(newPara);
            }

            targetParagraph.Remove();
        }

        private static bool ContainsMergeField(Paragraph para, string fieldName)
        {
            foreach (var fld in para.Elements<SimpleField>())
            {
                var instr = fld.Instruction?.Value ?? string.Empty;
                var parts = instr.Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Contains("MERGEFIELD") && parts.Contains(fieldName))
                    return true;
            }

            var runs = para.Elements<Run>().ToList();
            for (int i = 0; i < runs.Count; i++)
            {
                if (GetFldCharType(runs[i]) != FieldCharValues.Begin) continue;

                var instrBuilder = new StringBuilder();
                for (int j = i + 1; j < runs.Count; j++)
                {
                    var charType = GetFldCharType(runs[j]);
                    if (charType == FieldCharValues.Separate || charType == FieldCharValues.End) break;
                    instrBuilder.Append(runs[j].GetFirstChild<FieldCode>()?.InnerText ?? string.Empty);
                }

                var instrParts = instrBuilder.ToString()
                    .Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                if (instrParts.Contains("MERGEFIELD") && instrParts.Contains(fieldName))
                    return true;
            }

            return false;
        }

        public byte[] GetBytes()
        {
            _wdoc!.MainDocumentPart!.Document.Save();
            _wdoc.Dispose();
            _wdoc = null;
            _finalized = true;
            return _stream.ToArray();
        }

        public void Dispose()
        {
            if (!_finalized)
            {
                _wdoc?.Dispose();
                _finalized = true;
            }
            _stream.Dispose();
        }

        private static FieldCharValues? GetFldCharType(Run run)
            => run.GetFirstChild<FieldChar>()?.FieldCharType?.Value;
    }

    // ---------------------------------------------------------------------------
    // WordTable
    // ---------------------------------------------------------------------------

    internal sealed class WordTable
    {
        private readonly Table _table;

        internal WordTable(Table table) { _table = table; }

        public WordRow LastRow => new WordRow(_table.Elements<TableRow>().Last(), _table);

        public WordRow GetRow(int index)
            => new WordRow(_table.Elements<TableRow>().ElementAt(index), _table);

        public WordRow AddRow()
        {
            var lastRow = _table.Elements<TableRow>().Last();
            var newRow = (TableRow)lastRow.CloneNode(deep: true);

            // Clear all run content from cloned cells
            foreach (var cell in newRow.Elements<TableCell>())
                foreach (var para in cell.Elements<Paragraph>())
                    foreach (var run in para.Elements<Run>().ToList())
                        run.Remove();

            // Remove duplicate paraId / textId attributes (w14 namespace)
            const string w14ns = "http://schemas.microsoft.com/office/word/2010/wordml";
            foreach (var el in newRow.Descendants<OpenXmlElement>())
            {
                el.RemoveAttribute("paraId", w14ns);
                el.RemoveAttribute("textId", w14ns);
            }

            _table.Append(newRow);
            return new WordRow(newRow, _table);
        }

        public void RemoveRow(WordRow row) => row.Remove();

        public WordCell MergeCellsVertically(int columnIndex, int startRowIndex, int rowCount)
        {
            var rows = _table.Elements<TableRow>().ToList();
            TableCell? startCell = null;

            for (int i = startRowIndex; i < startRowIndex + rowCount && i < rows.Count; i++)
            {
                var cells = rows[i].Elements<TableCell>().ToList();
                if (columnIndex >= cells.Count) continue;
                var cell = cells[columnIndex];

                var tcPr = cell.GetFirstChild<TableCellProperties>();
                if (tcPr == null) { tcPr = new TableCellProperties(); cell.InsertAt(tcPr, 0); }

                tcPr.RemoveAllChildren<VerticalMerge>();
                tcPr.Append(i == startRowIndex
                    ? new VerticalMerge { Val = MergedCellValues.Restart }
                    : new VerticalMerge());

                if (i == startRowIndex) startCell = cell;
            }

            return new WordCell(startCell!);
        }
    }

    // ---------------------------------------------------------------------------
    // WordRow
    // ---------------------------------------------------------------------------

    internal sealed class WordRow
    {
        private readonly TableRow _row;
        private readonly Table _table;

        internal WordRow(TableRow row, Table table) { _row = row; _table = table; }

        internal void Remove() => _row.Remove();

        public int GetRowIndex() => _table.Elements<TableRow>().ToList().IndexOf(_row);

        public int CellCount => _row.Elements<TableCell>()
            .Sum(c => (int)(c.TableCellProperties?.GridSpan?.Val?.Value ?? 1));

        public WordCell GetCell(int logicalColumnIndex)
        {
            var cells = _row.Elements<TableCell>().ToList();
            int currentColumn = 0;

            foreach (var cell in cells)
            {
                int gridSpan = (int)(cell.TableCellProperties?.GridSpan?.Val?.Value ?? 1);
                if (currentColumn == logicalColumnIndex)
                    return new WordCell(cell);
                if (logicalColumnIndex < currentColumn + gridSpan)
                    return WordCell.NoOp;  // inside a merged cell — writes are silently discarded
                currentColumn += gridSpan;
            }

            return cells.Count > 0 ? new WordCell(cells.Last()) : WordCell.NoOp;
        }

        public WordCell CreateMergedCell(int firstColumn, int count)
        {
            var cells = _row.Elements<TableCell>().ToList();
            if (firstColumn >= cells.Count) return WordCell.NoOp;

            var startCell = cells[firstColumn];

            var tcPr = startCell.GetFirstChild<TableCellProperties>();
            if (tcPr == null) { tcPr = new TableCellProperties(); startCell.InsertAt(tcPr, 0); }

            tcPr.RemoveAllChildren<GridSpan>();
            if (count > 1)
                tcPr.Append(new GridSpan { Val = count });

            // Remove continuation cells
            int lastIdx = Math.Min(firstColumn + count - 1, cells.Count - 1);
            for (int i = firstColumn + 1; i <= lastIdx; i++)
                cells[i].Remove();

            return new WordCell(startCell);
        }
    }

    // ---------------------------------------------------------------------------
    // WordCell
    // ---------------------------------------------------------------------------

    internal sealed class WordCell
    {
        private const string FontName = WordDocumentHelper.DOCUMENT_FONT_NAME;

        private readonly TableCell? _cell;
        private readonly bool _isNoOp;

        private WordCell(bool isNoOp) { _isNoOp = isNoOp; }
        internal WordCell(TableCell cell) { _cell = cell; _isNoOp = false; }

        public static readonly WordCell NoOp = new WordCell(isNoOp: true);

        public void AddText(string text, WordCellParameters parameters)
        {
            if (_isNoOp) return;

            var para = _cell!.GetFirstChild<Paragraph>();
            if (para == null) { para = new Paragraph(); _cell.Append(para); }

            // Clear existing runs
            para.Elements<Run>().ToList().ForEach(r => r.Remove());

            // Paragraph alignment
            if (parameters.HorizontalAlignment.HasValue)
            {
                var pPr = para.GetFirstChild<ParagraphProperties>() ?? new ParagraphProperties();
                pPr.RemoveAllChildren<Justification>();
                pPr.Append(new Justification { Val = ToJustification(parameters.HorizontalAlignment.Value) });
                if (para.GetFirstChild<ParagraphProperties>() == null)
                    para.InsertAt(pPr, 0);
            }

            // Cell vertical alignment
            if (parameters.VerticalAlignment.HasValue)
            {
                var tcPr = _cell.GetFirstChild<TableCellProperties>();
                if (tcPr == null) { tcPr = new TableCellProperties(); _cell.InsertAt(tcPr, 0); }
                tcPr.RemoveAllChildren<TableCellVerticalAlignment>();
                tcPr.Append(new TableCellVerticalAlignment { Val = ToVerticalAlignment(parameters.VerticalAlignment.Value) });
            }

            // Run with font formatting
            var rPr = new RunProperties();
            rPr.Append(new RunFonts { Ascii = FontName, HighAnsi = FontName, ComplexScript = FontName });
            rPr.Append(new FontSize { Val = (parameters.FontSize * 2).ToString() });
            rPr.Append(new FontSizeComplexScript { Val = (parameters.FontSize * 2).ToString() });
            if (parameters.IsBold) rPr.Append(new Bold());

            var run = new Run();
            run.Append(rPr);
            run.Append(new Text(text) { Space = SpaceProcessingModeValues.Preserve });
            para.Append(run);
        }

        public void AddNumber(int value, WordCellParameters parameters)
            => AddText(value.ToString(), parameters);

        public void AddPrice(decimal value, WordCellParameters parameters)
            => AddText(ReportHelper.GetPriceString(value), parameters);

        private static JustificationValues ToJustification(WordHorizontalAlignment alignment) => alignment switch
        {
            WordHorizontalAlignment.Left  => JustificationValues.Left,
            WordHorizontalAlignment.Right => JustificationValues.Right,
            _                             => JustificationValues.Center
        };

        private static TableVerticalAlignmentValues ToVerticalAlignment(WordVerticalAlignment alignment) => alignment switch
        {
            WordVerticalAlignment.Top    => TableVerticalAlignmentValues.Top,
            WordVerticalAlignment.Bottom => TableVerticalAlignmentValues.Bottom,
            _                            => TableVerticalAlignmentValues.Center
        };
    }

    // ---------------------------------------------------------------------------
    // WordDocumentHelper — constants used by both WordCell and report helpers
    // ---------------------------------------------------------------------------

    internal static class WordDocumentHelper
    {
        public const string DOCUMENT_FONT_NAME = "Times New Roman";
    }

    // ---------------------------------------------------------------------------
    // BlockParagraph — used by ReplaceFieldWithBlock for multi-paragraph content
    // ---------------------------------------------------------------------------

    internal record BlockParagraph(string Text, bool IsBold, int IndentLevel);
}
