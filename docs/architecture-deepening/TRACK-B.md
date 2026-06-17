# Track B — WordTableFiller for the shared list-row pattern

Independent of A and C. Touches `Mil.Paperwork.Domain/Helpers/WordDocumentHelper.cs`, `Mil.Paperwork.Domain/Reports/QualityStateReport.cs`, `Mil.Paperwork.Domain/Reports/AssetDismantlingReport.cs`.

## Problem

`WordDocumentHelper` exposes cell-level primitives (`GetCell(index).AddText/AddNumber/AddPrice`), not a table-shaped interface. Every Report class re-derives "loop rows, address cells by index" using column indices sourced from a parallel Helper class of int constants.

## Steps

- [x] **B1. Add `WordTableFiller` next to `WordTable`/`WordRow`/`WordCell` in `WordDocumentHelper.cs`.**
  ```csharp
  internal sealed class WordTableColumn<TRow>
  {
      public int Index;
      public WordCellParameters Parameters;
      public Action<WordCell, TRow> Write; // dispatches to AddText/AddNumber/AddPrice
  }

  internal static class WordTableFiller
  {
      public static void Fill<TRow>(WordTable table, IReadOnlyList<TRow> rows, IReadOnlyList<WordTableColumn<TRow>> columns,
          IReadOnlyList<int>? verticalMergeColumns = null, Action<WordTable, IReadOnlyList<TRow>>? addSummaryRow = null)
  }
  ```
  It owns: capture `LastRow`, loop `rows` calling `table.AddRow()` + each column's `Write`, remove the template row, apply `table.MergeCellsVertically` for any `verticalMergeColumns`, then invoke `addSummaryRow` if supplied.

- [x] **B2. Migrate `QualityStateReport.FillTheTable` first (`Reports/QualityStateReport.cs:72-114`).**
  Simplest case: 16 columns, no vertical merge, has a summary row. Replace the manual loop with a column-definition list (reusing the existing `QualityStateReportHelper.COLUMN_*` constants for indices) + a call to `WordTableFiller.Fill`.

- [x] **B3. Migrate `AssetDismantlingReport.FillTheTable` second.**
  Harder case: vertical merge across 7 asset-level columns + summary row with merged cells. Exercises `verticalMergeColumns` and `addSummaryRow`, proving `WordTableFiller` generalizes past the first call site.

## Out of scope

`TechnicalStateReport` (static/fixed-row tables, not a list-fill) and `ResidualValueReport` (Excel, dynamic-column single row) — each is the only example of its shape today; do not force them into `WordTableFiller`.

## Verification

`dotnet test Mil.Paperwork.Tests/Mil.Paperwork.Tests.csproj`; add a byte-level output test for `QualityStateReport` and `AssetDismantlingReport`, similar to `WriteOffOrderReportTests`; manually generate both reports and visually diff the output `.docx` against a pre-refactor copy (summary row text, merged cells, column order).
