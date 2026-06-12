# Reports History — Implementation Stages

Feature: persist every report's input data locally, browse past reports in a filterable History tab, reopen them prefilled, and create new reports of other types reusing the data. A future redesign will make History the main screen.

**Target project: `Mil.Paperwork.UI` (Avalonia 12).** The legacy `Mil.Paperwork.WriteOff\` WPF project is out of the sln and doesn't compile — never touch it.

Execute stages in order, one per session. Each stage doc is self-contained: open the stage file, follow it, run its verification, commit.

| Stage | File | Goal | Depends on |
|---|---|---|---|
| 1 | [stage-1-foundation.md](stage-1-foundation.md) | New `Mil.Paperwork.DataAccess` project, move `DataService`, unify enums on `ReportType` | — |
| 2 | [stage-2-history-data-layer.md](stage-2-history-data-layer.md) | Snapshot DTOs, `IReportHistoryRepository`, JSON file storage + index | 1 |
| 3 | [stage-3-save-hooks.md](stage-3-save-hooks.md) | Output-path plumbing, auto-save on generation, Save-draft button | 2 |
| 4 | [stage-4-history-tab.md](stage-4-history-tab.md) | History tab UI: table, filters, status dot, Open file / Remove | 3 |
| 5 | [stage-5-reopen.md](stage-5-reopen.md) | Reopen entry → editor tab prefilled (priority types) | 4 |
| 6 | [stage-6-create-from.md](stage-6-create-from.md) | Cross-type "Create…" conversions | 5 |
| 7 | [stage-7-remaining.md](stage-7-remaining.md) | Remaining types, ReportDataService move, redesign follow-ups | 6 |
| 8 | [stage-8-documentation.md](stage-8-documentation.md) | User documentation: Ukrainian History section in the root README | 7 |

## Global decisions (apply to all stages)

- Storage: one JSON file per history entry + a single rebuildable index file, under `Data/History/` next to the exe (same pattern as `Data/ReportDataConfig.json`).
- History keyed on the existing `Mil.Paperwork.Infrastructure\Enums\ReportType` (after Stage 1 unification). Never introduce a parallel document-type enum.
- Entry statuses: `Draft` (manual save) and `Generated` (auto-saved on successful generation, with output file paths). Re-saving from the same tab upserts the same entry via tracked `HistoryEntryId`.
- Every entry JSON carries `SchemaVersion` (current = 1). The index is a cache: any read path must tolerate a missing/corrupt index and rebuild from entry files, skipping unreadable ones.
- History persistence failures must never break report generation (separate try/catch).
- Code style: no comments, I-prefixed interfaces, manual DI registration via registrator classes, System.Text.Json.
