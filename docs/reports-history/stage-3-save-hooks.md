# Stage 3 — Output-path plumbing + save hooks (auto-save + draft)

**Goal:** every successful generation writes a `Generated` history entry with the real output file paths; every report tab gets a "Зберегти чернетку" button writing a `Draft` entry. Same tab re-saves upsert the same entry.

Part of the Reports History feature — see [README.md](README.md). Requires Stage 2 (repository + mappers exist).

## 1. Surface the generated file path

Today the final filename exists only inside the report services, and `FileStorageService.SaveFile` may silently rename on collision (`GetUniqueFileName`).

- `Mil.Paperwork.Infrastructure\Services\IFileStorageService.cs` + `FileStorageService.cs`: `void SaveFile(...)` → `string SaveFile(...)` returning the actual path written.
- New `Mil.Paperwork.Domain\Services\ReportGenerationResult.cs`:
  ```csharp
  public class ReportGenerationResult
  {
      public bool Success { get; init; }
      public IReadOnlyList<string> OutputFiles { get; init; }
      public static implicit operator bool(ReportGenerationResult result) => result?.Success ?? false;
  }
  ```
- Change `IReportService<T>.TryGenerateReport` to return `ReportGenerationResult`; update all ~11 services in `Mil.Paperwork.Domain\Services\` (`InvoiceReportService`, `ResidualValueReportService`, `TechnicalStateReportService`, `QualityStateReportService`, `WriteOffActReportService`, `AssetValuationReportService`, `AssetDismantlingReportService`, `CommissioningActService`, `Handover23ReportService`, `WriteOffOrderReportService`, `WriteOffReportPackageService` — the package service aggregates child services' file lists). The implicit bool operator keeps `TextFormatHelper.GetReportStatusMessage(name, result)` call sites compiling unchanged.

## 2. History write service

In `Mil.Paperwork.DataAccess\Services\`: `IReportHistoryService` + `ReportHistoryService`:
```csharp
Guid SaveDraft(ReportType type, IReportData data, Guid? entryId);
Guid SaveGenerated(ReportType type, IReportData data, IReadOnlyList<string> files, Guid? entryId);
```
Builds the envelope via `ReportSnapshotMapper` (DocumentNumber/Summary/denormalized search fields per type), upserts via `IReportHistoryRepository` (existing `entryId` → update, preserve `CreatedAt`; null → new Guid). Register in `DataAccessServicesRegistrator`.

## 3. Hook generation — `Mil.Paperwork.UI\Managers\ReportManager.cs`

- Inject `IReportHistoryService`.
- Each `Generate*` method gains optional `Guid? historyEntryId = null` and, on success, calls `SaveGenerated(...)` in its OWN try/catch — the methods are `async void`; a history failure must never break generation or crash the UI thread.

## 4. Draft hook — `Mil.Paperwork.UI\ViewModels\Tabs\BaseReportTabViewModel.cs`

```csharp
public Guid? HistoryEntryId { get; protected set; }
public IDelegateCommand SaveDraftCommand { get; }
protected abstract ReportType HistoryReportType { get; }
protected abstract IReportData BuildReportData();
```
`SaveDraftCommand` → `HistoryEntryId = _reportHistoryService.SaveDraft(HistoryReportType, BuildReportData(), HistoryEntryId)`.

In each report VM under `Mil.Paperwork.UI\ViewModels\Reports\` (Invoice, ResidualValue, AssetInitialTechnicalState, AssetTechnicalState, CommissioningAct, AssetValuation, AssetDismantling, Handover23Act, WriteOffOrder):
- Extract the inline `new *ReportData {...}` from `GenerateReport(folderName)` into `BuildReportData()`.
- `BuildReportData()` must run WITHOUT validation (empty fields, no folder, empty asset lists — that's what makes incomplete drafts saveable) and be side-effect-free (e.g. `_dataService.AlterPeople(...)` stays in the generate path — see `InvoiceReportViewModel.GenerateReport`).
- Pass `HistoryEntryId` to the `ReportManager.Generate*` call.
- Watch `DateTimeOffset` (VM) vs `DateTime` (data) conversions.

## 5. Draft button

Add "Зберегти чернетку" (`SaveDraftCommand`) to each report view's button bar: `Mil.Paperwork.UI\Views\Reports\*.axaml`; for Valuation/Dismantling the button bars are inline in `Windows\MainWindow.axaml` DataTemplates.

## Verify

- Run app → generate an invoice → `Data/History/Entries/{id}.json` exists with status `Generated` and a real `.docx` path (also when a name collision forces a rename).
- Fill a tab partially → "Зберегти чернетку" twice → single entry, same id, `ModifiedAt` changed.
- Generate from the same tab after drafting → same entry flips to `Generated`.
- Unit test `ReportHistoryService` upsert semantics. `dotnet build` + `dotnet test`.
