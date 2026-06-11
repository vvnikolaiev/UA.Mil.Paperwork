# Stage 5 — Reopen: per-VM hydration (priority types)

**Goal:** History row "Відкрити" opens the matching editor tab with every field prefilled from the stored snapshot. Regenerating from a reopened tab updates the SAME history entry.

Part of the Reports History feature — see [README.md](README.md). Requires Stage 4.

## Design

Hydrate ViewModels from **Domain `*ReportData`** (not from snapshots): VMs already know Domain types via `BuildReportData()`, and Stage 6 create-from conversions reuse the same entry point. The flow: snapshot → `ReportSnapshotMapper.ToReportData(...)` → typed `LoadReportData(...)`.

## Steps

- New `Mil.Paperwork.UI\ViewModels\Tabs\IReportDataLoadable.cs`:
  ```csharp
  internal interface IReportDataLoadable<in TData> where TData : IReportData
  {
      void LoadReportData(TData data);
  }
  ```
- `Mil.Paperwork.UI\ViewModels\Tabs\HomePageViewModel.cs`:
  - Refactor the `OpenNewReportTab` switch into `CreateReportTab(ReportType)` (pure creation, returns the VM); `OpenNewReportTab` keeps calling it.
  - Subscribe to `HistoryViewModel.OpenHistoryEntryRequested`; handler: `IReportHistoryRepository.GetEntry(id)` → `ReportSnapshotMapper.ToReportData(entry.Snapshot)` → `CreateReportTab(entry.ReportType)` → switch dispatching the typed `LoadReportData(...)` → set `HistoryEntryId = entry.Id` → raise `TabAdded`.
- Implement `IReportDataLoadable<T>` (the exact reverse of each VM's `BuildReportData()`) on the priority VMs in `Mil.Paperwork.UI\ViewModels\Reports\`:
  - `InvoiceReportViewModel : IReportDataLoadable<IInvoceReportData>` — scalar fields; `AssetsCollection` from `Assets` (add `InvoiceAssetViewModel` ctor/factory from `IAssetInfo`); `AssetAcceptance` from Recipient/Transmitter.
  - `CommissioningActReportViewModel : IReportDataLoadable<ICommissioningActReportData>`.
  - `AssetInitialTechnicalStateViewModel : IReportDataLoadable<IInitialTechnicalStateReportData>` (TechnicalState №7 tab).
  - `ResidualValueReportViewModel : IReportDataLoadable<IResidualValueReportData>` — incl. MetalCosts and asset table rows.
  - `AssetTechnicalStateViewModel : IReportDataLoadable<ITechnicalStateReportData>` (WriteOffPackage tab).
- `HistoryViewModel`: enable `OpenEntryCommand` for types whose VM is hydratable; message dialog for not-yet-supported types (Valuation, Dismantling, Handover23, WriteOffOrder — Stage 7).

## Verify

- Draft an invoice with 2 assets (one radiochemical kind) → close tab → History → Відкрити → all fields, both asset rows incl. subtype extras, and both persons restored.
- Generate from the reopened tab → the SAME entry flips to `Generated` with file paths (no duplicate entry).
- Reopen a generated ResidualValue entry → MetalCosts and asset table restored.
- `dotnet build` + `dotnet test`.
