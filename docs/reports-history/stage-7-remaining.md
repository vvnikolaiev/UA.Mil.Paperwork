# Stage 7 — Remaining types & follow-ups (additive, schedule as needed)

Part of the Reports History feature — see [README.md](README.md). Requires Stage 6.

Each item below is independent and small; pick them up individually.

## Reopen hydration for remaining types

Implement `IReportDataLoadable<T>` (reverse of `BuildReportData()`) on:
- `AssetValuationViewModel` (`IAssetValuationReportData`)
- `AssetDismantlingViewModel` (`IDismantlingReportData`)
- `Handover23ActViewModel` (`IHandoverReportData`)
- `WriteOffOrderViewModel` (`IWriteOffOrderReportData` — incl. Services/Witnesses collections)

Then remove the "not supported" fallback in `HistoryViewModel.OpenEntryCommand`.

## Additional conversions

One class + one registration each (see Stage 6 pattern). Candidates — validate usefulness with the user first:
- CommissioningAct → Invoice / InitialTechnicalState (reverse flows)
- WriteOffPackage/TechnicalState → ResidualValue
- Handover23 ↔ Invoice (both are transfer documents with assets + two persons)

## Data-layer consolidation

- Move `ReportDataService` (+ `ReportDataConfig` models if cleanly separable) from Infrastructure into `Mil.Paperwork.DataAccess`, completing the "all database code in the data layer" goal from Stage 1.

## History-as-main-screen redesign

When the redesign starts: `HistoryView` is already self-contained — promote it to the startup tab/main area; home tiles become a "create new" affordance alongside the table. Filters and the Create-from submenu already carry the primary workflows.
