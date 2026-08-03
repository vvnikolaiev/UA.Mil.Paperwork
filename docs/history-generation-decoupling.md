# Decouple history from report generation, then fix the WriteOff Package draft

Branch: `vasnik/historyDecoupling` (from `master`).

## Context

Saving a draft of «Пакет списання» loses `Reason` and resets the three generate-checkboxes (and the
reg/doc numbers beside them) to their defaults. The nine fields the tab's layout binds
(`TechnicalStateViewLayout`, `TechnicalStateReportView.axaml:138-270`) have nowhere to be stored:
`WriteOffPackageReportSnapshot` holds only assets, dates, orden number/date, the book-of-losses extract,
service key and destination folder.

The obvious fix — adding those fields to `IWriteOffPackageReportData` — is wrong: that interface is the
**generation contract** consumed by `WriteOffReportPackageService` to produce one document. Whether the
tab should also emit an АЗЯС instead of an АТС is not that document's data.

The reason there is no better place to put them is structural. `ReportManager.RunReportAsync`
(`Mil.Paperwork.UI/Managers/ReportManager.cs:189-211`) fuses three responsibilities:

1. generate — `service.TryGenerateReport(reportData)`
2. record history — `TrySaveGeneratedToHistory(reportType, reportData, …)`, **reusing the generation payload**
3. report status — `_dialogService.ShowMessageAsync(…)`

Because (2) is forced to reuse (1)'s payload, a tab whose state is broader than any single document it
emits cannot record its own state. «Пакет списання» emits up to four documents; that is the bug.

Secondary damage from the same coupling:

- Two `RunReportAsync` overloads, so "does this record history?" is encoded by which one you call.
- `GenerateDismantlingReport` (lines 134-152) hand-rolls the whole flow because there is no seam for
  multi-service generation.
- `TrySaveGeneratedToHistory` swallows every exception (`catch (Exception) { }`), which is how the
  `EASReportSnapshot` polymorphism bug stayed invisible until a user hit it.
- Drafts and generated entries reach history by two different routes for the same `ReportType`.

Outcome: `ReportManager` orchestrates generation and nothing else; the tab owns its history payload; the
«Пакет списання» fix then becomes local, with `IWriteOffPackageReportData` untouched.

**Decisions taken:** shed both history *and* the status dialog from `ReportManager`; accept the async
ripple through the tab `GenerateReport` overrides; surface history-write failures instead of swallowing them.

## Stage 1 — `ReportManager` sheds history and messaging

`Mil.Paperwork.UI/Managers/ReportManager.cs`

- **Delete `GenerateWriteOffReport(ObsoleteWriteOffReportData)`** (lines 66-92) — verified zero callers.
  Then check whether `ObsoleteWriteOffReportData` is now unreferenced and delete it too.
- Drop the `IReportHistoryService` and `IDialogService` dependencies and `TrySaveGeneratedToHistory`.
- Collapse both `RunReportAsync` overloads into one generic helper. Drop the `reportType` and
  `historyEntryId` parameters — nothing in the manager needs them any more.
- `Generate*` become **synchronous** and return `ReportGenerationResult` (`Mil.Paperwork.Domain.Services`,
  already visible to the UI). They were only `async` to await the dialog; with messaging gone the
  asynchrony belongs in the UI layer.
- Let exceptions propagate — Stage 2's funnel catches them. Do not swallow.
- `GenerateDismantlingReport` composes its two services' results through the existing
  `ReportGenerationResult.FromResult` and returns, instead of hand-rolling generation + history + dialog.

Note the second overload exists because `GenerateCommissioningAct(IList<ICommissioningActReportData>)`
passes a *list* as `TData`, which cannot satisfy `where TData : IReportData`. Keep that call working —
either a non-generic arm for the list case or relax the constraint; do not reintroduce two idioms.

## Stage 2 — the tab owns history and status

`Mil.Paperwork.UI/ViewModels/Tabs/BaseReportTabViewModel.cs` already holds every piece needed:
`_reportHistoryService`, `HistoryEntryId`, `EnsureHistoryEntryId()`, `HistoryReportType`, and the
abstract `BuildReportData()`. Give it **one** funnel so no tab hand-rolls the flow:

```csharp
protected async Task<ReportGenerationResult> RunReportAsync(
    string displayName, string errorPrefix, Func<ReportGenerationResult> generate)
```

— runs the generation, shows `TextFormatHelper.GetReportStatusMessage(displayName, result)`, and on
exception shows `$"{errorPrefix}: {ex.Message}"`. This preserves today's messaging exactly; the display
names stay in `TextFormatHelper` and the error prefixes move from `ReportManager` to the call sites.

Plus:

```csharp
protected async Task RecordGeneratedAsync(ReportGenerationResult result)
```

— on success, `_reportHistoryService.SaveGenerated(HistoryReportType, BuildReportData(),
result.OutputFiles, EnsureHistoryEntryId())`. On failure it now **shows a message** rather than
swallowing, worded so it is clear the documents were produced and only the history entry failed.

Each of the 16 `_reportManager.Generate*` call sites becomes: await the funnel, then
`RecordGeneratedAsync` for the one primary document per tab. Preserve today's mapping of which document
is primary — `GenerateWriteOffPackage` for «Пакет списання», `GenerateInitialTechnicalStateReport` for
«Тех. стан», `GenerateDismantlingReport` for розукомплектація, and the single call elsewhere; the invoice
and commissioning acts generated as side documents do not record history today and must not start.

The tab `GenerateReport(…)` overrides become `async Task`, as do the private `Generate*Report` helpers
they call. `BaseReportTabViewModel` and `AssetInitialTechnicalStateViewModel` signatures change accordingly.

**Behaviour change to confirm during verification:** today «Пакет списання» records history only when the
`GenerateWriteOffPackage` checkbox is ticked, because that is the only call passing a history id. Once the
payload is tab state rather than document data, recording once per generate action regardless is the
correct behaviour — but it is a change, so call it out rather than letting it slip in silently.

## Stage 3 — fix the «Пакет списання» draft bug

Now local, because `BuildReportData()` is the single history payload for **both** the draft and the
generated paths, and nothing forces it to be the document's own type.

- New `Mil.Paperwork.Domain/DataModels/ReportData/WriteOffPackageTabData.cs` implementing `IReportData`,
  composing the untouched `WriteOffPackageReportData` plus the nine missing fields: `Reason`, `EventType`,
  `GenerateWriteOffPackage`, `GenerateWriteOffActs` + `WriteOffRegNumber`/`WriteOffDocNumber`,
  `GenerateQualityStateReportInstead` + `QSRRegNumber`/`QSRDocNumber`.
- `AssetTechnicalStateViewModel.BuildReportData()` returns it; `GenerateWriteOffReports` keeps building a
  plain `WriteOffPackageReportData` for `WriteOffReportPackageService`. `LoadReportData` restores all nine.
- `WriteOffPackageReportSnapshot` gains the nine fields — it is a history record, holding tab state is its
  job. Keep the type name so existing draft JSON still deserializes; absent keys fall back to the property
  initializers, which match the ViewModel defaults (`GenerateWriteOffPackage`/`GenerateWriteOffActs` true,
  QSR-instead false), so old drafts open as they do today rather than with everything switched off.
- `WriteOffPackageSnapshotMapper` maps `WriteOffPackageTabData` ↔ snapshot, delegating the document half
  to the `ToBookExtractSnapshot`/`ToBookExtractData` helpers already there.
- Update the dispatch arms: `ReportSnapshotMapper.ToSnapshot`/`ToReportData`, `ReportTabFactory` (line 177),
  and `IReportDataLoadable<…>` on the ViewModel.
- Conversions: `ResidualValueToWriteOffPackageConversion` produces the tab type;
  `WriteOffPackageToEASConversion` unwraps the document half.

`IWriteOffPackageReportData` and `WriteOffPackageReportData` are **not** modified in this stage.

## Verification

1. `dotnet build Mil.Paperwork.WriteOff.sln` — clean. (Close the running app first; a live
   `Mil.Paperwork.UI` and Visual Studio hold `bin` DLLs and produce MSB3021/3027 copy errors that are not
   compile errors. `-t:Compile` checks code without the copy step.)
2. `dotnet test Mil.Paperwork.Tests\Mil.Paperwork.Tests.csproj` — existing suite green.
3. New tests: a `WriteOffPackageTabData` round-trip asserting all nine fields survive
   `ReportSnapshotMapper.ToSnapshot` → `ToReportData` (mirroring the existing
   `WriteOffPackage_RoundTrip_PreservesBookExtract`), and a legacy-snapshot test proving JSON without the
   new keys restores the ViewModel's defaults rather than all-false.
4. Live run via the `ui-verify` skill — the part that actually proves the bug is fixed:
   - «Пакет списання»: type a Reason, untick «Формувати Акти Списання», tick «Формувати АЗЯС замість АТС»,
     fill both reg/doc number pairs → Save draft → close the tab → reopen from History. Every field and
     checkbox must come back as left.
   - Generate the package and confirm the same round-trip from a *generated* history entry, not just a draft.
   - Generate with the package checkbox unticked and confirm a history entry now appears (the Stage 2
     behaviour change).
   - Regression on the other tabs, since Stage 2 touches all 16 call sites: generate from «Тех. стан (№7)»,
     «Оцінка майна», «Накладна» and ЄАС and confirm the status dialog still appears with the same wording
     and history still records exactly one entry per action.
   - Open an existing pre-change draft and confirm it still loads (Stage 3 back-compat).

## Note on the EAS branch

This work is independent of `vasnik/eas`. Stage 3 touches `WriteOffPackageSnapshotMapper`,
`ReportSnapshotMapper` and `ReportTabFactory`, which `vasnik/eas` also modified — expect conflicts when
the two branches meet, concentrated in the snapshot dispatch arms.
