# Stage 6 — Create-from: cross-type conversion registry

**Goal:** "Створити…" submenu on each History row opens prefilled NEW draft tab(s) of another report type, reusing every field that maps (assets, persons, dates, document numbers).

Part of the Reports History feature — see [README.md](README.md). Requires Stage 5 (`IReportDataLoadable`, `CreateReportTab(ReportType)`).

## Conversion layer (in `Mil.Paperwork.DataAccess\Conversions\`)

```csharp
public interface IReportDataConversion
{
    ReportType SourceType { get; }
    ReportType TargetType { get; }
    IReadOnlyList<IReportData> Convert(IReportData source);
}

public class ReportConversionRegistry
{
    public IReadOnlyList<ReportType> GetTargets(ReportType sourceType);
    public IReadOnlyList<IReportData> Convert(ReportType sourceType, ReportType targetType, IReportData source);
}
```

`Convert` returns a list because Invoice→CommissioningAct is per-asset: one `CommissioningActReportData` per invoice asset → one tab per result.

Priority conversions (each its own class, registered manually in `DataAccessServicesRegistrator`):
- `InvoiceToCommissioningActConversion` — per asset: asset→`IProductData`, SerialNumber→AssetIds, Count, DocumentNumber/DateCreated, Transmitter/Recipient→PersonHanded/PersonAccepted.
- `InvoiceToInitialTechnicalStateConversion` — Assets list as-is; Recipient/Transmitter→PersonAccepted/PersonHanded.
- `ResidualValueToWriteOffPackageConversion` — Assets + EventDate.
- `ResidualValueToInitialTechnicalStateConversion` — Assets.

Field mapping reference: compare source/target classes in `Mil.Paperwork.Domain\DataModels\ReportData\`.

## UI wiring

- `HistoryEntryViewModel` exposes `CreateTargets` (from `ReportConversionRegistry.GetTargets`, display via `ReportType.GetDescription()`) rendered as MenuFlyout subitems under "Створити…"; hide the submenu when empty.
- `HistoryViewModel` raises `CreateFromRequested(Guid entryId, ReportType target)`.
- `HomePageViewModel` handles it: `GetEntry` → `ToReportData` → `registry.Convert(...)` → for EACH result: `CreateReportTab(target)` + typed `LoadReportData(...)` with `HistoryEntryId = null` (a new independent draft — saving/generating it creates its own history entry).

## Verify

- Unit tests per conversion class: field-by-field mapping, multi-asset invoice produces N commissioning acts.
- Manual: Invoice with 3 assets → Створити → "Акт введення в експлуатацію" → 3 prefilled tabs; ResidualValue entry → WriteOffPackage tab carries assets and event date; generating from a created tab writes a NEW history entry, source entry untouched.
- `dotnet build` + `dotnet test`.
