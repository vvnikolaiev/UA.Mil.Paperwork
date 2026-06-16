# Track A — Collapse the ReportManager dispatch table

Independent of B and C. Touches only `Mil.Paperwork.UI/Managers/ReportManager.cs` (325 lines, 13 public `Generate*` methods).

## Problem

ReportManager's interface is 13 near-identical methods that repeat the same try / `TryGenerateReport` / `TrySaveGeneratedToHistory` / `GetReportStatusMessage` / `ShowMessageAsync` chain.

## Steps

- [ ] **A1. Add a generic pipeline method.**
  Add a private method to `ReportManager`:
  ```csharp
  private async Task RunReportAsync<TData>(
      IReportService<TData> service, TData reportData, ReportType reportType,
      string displayName, Guid? historyEntryId, string errorPrefix)
  ```
  It performs exactly what all 11 of the simple `Generate*` methods do today: `try { TryGenerateReport → TrySaveGeneratedToHistory → GetReportStatusMessage → ShowMessageAsync } catch { ShowMessageAsync($"{errorPrefix}: {ex.Message}") }`. Reuse the existing `TrySaveGeneratedToHistory` helper unchanged (`ReportManager.cs:309-323`).

- [ ] **A2. Migrate the 11 single-report methods one at a time.**
  `GenerateResidualValueReport`, `GenerateInitialTechnicalStateReport`, `GenerateTechnicalStateReport`, `GenerateQualityStateReport`, `GenerateWriteOffActReport`, `GenerateWriteOffPackage`, `GenerateValuationReport`, `GenerateCommissioningAct` (both overloads), `GenerateInvoice`, `GenerateHandover23Act`, `GenerateWriteOffOrder` each become a one-line call into `RunReportAsync`. Public signatures stay unchanged. Do these as separate, independently buildable edits so a regression is easy to bisect.

- [ ] **A3. Leave the two aggregate methods hand-written.**
  `GenerateWriteOffReport` (calls 5 services, concatenates 5 status lines into one message) and `GenerateDismantlingReport` (calls 2 services, merges their `OutputFiles`) are a genuinely different shape. Leave them as-is — not part of this track.

## Verification

`dotnet build Mil.Paperwork.WriteOff.sln`; run the app (`dotnet run --project Mil.Paperwork.UI`) and generate one report of each migrated type to confirm the dialog message and history entry still appear correctly.
