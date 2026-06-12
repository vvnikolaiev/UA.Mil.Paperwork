# Stage 4 — History tab UI: table, filters, status dot, Open file / Remove

**Goal:** browsable History tab opened from the home page. A future redesign will make this the main screen — keep the view self-contained (no dependency on tab chrome).

Part of the Reports History feature — see [README.md](README.md). Requires Stage 3 (entries are being written).

## Create

- `Mil.Paperwork.UI\ViewModels\Tabs\HistoryViewModel.cs` — extends `BaseTabViewModel`, `Header => "Історія"`.
  - Loads `IReportHistoryRepository.GetIndex()` into `ObservableCollection<HistoryEntryViewModel>`.
  - Filter state: `ReportType?` dropdown (+"Всі"), date from/to, search text — in-memory, case-insensitive `Contains` over `DocumentNumber + AssetNames + SerialNumbers + NomenclatureCodes` (denormalized in the index, no entry-file reads while filtering).
  - Commands: `RefreshCommand`; `RemoveEntryCommand` — Yes/No confirmation via `IDialogService` (same pattern as `BaseTabViewModel.Close()`), then `repository.Delete(id)`; `OpenGeneratedFileCommand` — `Process.Start(new ProcessStartInfo(path) { UseShellExecute = true })` on the first `GeneratedFiles` entry (disabled for drafts); `OpenEntryCommand` — raises `event EventHandler<Guid> OpenHistoryEntryRequested` (handled in Stage 5; until then show "not supported yet" message); placeholder `event` for Create-from (Stage 6).
- `Mil.Paperwork.UI\ViewModels\History\HistoryEntryViewModel.cs` — row wrapper over `ReportHistoryIndexEntry`: type description (`ReportType.GetDescription()`), date, document number/summary, status, output path, per-row commands.
- `Mil.Paperwork.UI\Views\HistoryView.axaml(+.cs)`:
  - Filter row: ComboBox + two DatePickers + search TextBox.
  - `DataGrid` (already a dependency) with columns: status dot, type, date, number/summary, output path, actions.
  - **Status dot:** small `Ellipse`, green = `Generated`, yellow = `Draft`, `ToolTip` with the status text. Add the two brushes to the shared color resource dictionaries (recently extracted, commit `8e88333`) — do not hardcode hex in the view.
  - Action column: Button opening a `MenuFlyout` — Відкрити / Видалити / Створити… (submenu placeholder until Stage 6).

## Modify

- `Mil.Paperwork.UI\Windows\MainWindow.axaml` — explicit `DataTemplate` mapping `HistoryViewModel` → `HistoryView`. Required: the `ViewLocator` convention would resolve `ViewModels.Tabs.HistoryViewModel` to nonexistent `Views.Tabs.HistoryView`; all existing tabs use explicit templates.
- `Mil.Paperwork.UI\ViewModels\Tabs\HomePageViewModel.cs` — `OpenHistoryCommand` with a cached single `HistoryViewModel` instance (dedicated field; it is not a settings tab), raise `TabAdded` / select existing.
- `Mil.Paperwork.UI\Views\HomePageView.axaml` — "Історія" button/tile.

## Verify

Run the app; create draft + generated entries of several types; then:
- Type filter, date range, and free-text search (by serial number, asset name, document number) each narrow the table correctly.
- Status dots: green for generated, yellow for drafts; tooltips correct.
- Видалити asks for confirmation, removes the row, deletes `Data/History/Entries/{id}.json` and the index row.
- Відкрити (file) launches the generated document; disabled/hidden for drafts.
- Reopening the History tab from home reuses the same tab instance.
