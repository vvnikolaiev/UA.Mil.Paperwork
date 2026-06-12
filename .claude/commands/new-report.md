Scaffold a new report type for the Mil.Paperwork.WriteOff project, following the 8-step process defined in CLAUDE.md. The report name (PascalCase, e.g. `MyNewReport`) is: $ARGUMENTS

Work through each step in order. Read existing analogues before writing code (e.g. `QualityStateReport` / `QualityStateReportService` / `QualityStateReportHelper`). Apply all code conventions from memory: no expression-body methods, braces on every if/else/loop, return local variables, all literals as named const fields.

## Step 1 — Template
Check `Mil.Paperwork.Domain/Templates/` for an existing `.docx` or `.xlsx` template named after the report. If none exists, tell the user they must add one before proceeding with the remaining steps.

## Step 2 — Data model
Create two files in `Mil.Paperwork.Domain/DataModels/ReportData/`:
- `I{Name}ReportData.cs` — interface extending `IReportData`
- `{Name}ReportData.cs` — implementation with a `GetDestinationPath()` returning `DestinationFolder`

## Step 3 — Report class
Create `Mil.Paperwork.Domain/Reports/{Name}Report.cs` implementing `IReport`:
- Constructor accepts `IReportDataService`
- `TryCreate(I{Name}ReportData reportData)` loads the template via `PathsHelper.GetTemplatePath(...)`, fills fields, stores bytes, returns bool
- `GetReportBytes()` returns the stored bytes
- Keep field-filling in private methods (`FillTheFields`, `FillCommission`, etc.) matching the pattern in `QualityStateReport`

## Step 4 — Helper
Create `Mil.Paperwork.Domain/Helpers/{Name}ReportHelper.cs` with:
- `public const string REPORT_TEMPLATE_NAME` — the template file name
- `public const string OUTPUT_REPORT_NAME_TEMPLATE` — output file name format string (include `{0}` for document number)
- All field name consts and column index consts needed by the report class
- No magic strings or numbers in method bodies

## Step 5 — Service
Create `Mil.Paperwork.Domain/Services/{Name}ReportService.cs`:
- Implement `IReportService<I{Name}ReportData>`
- Constructor takes `IReportDataService` and `IFileStorageService`
- `TryGenerateReport` creates the report, calls `SaveReport`, returns `ReportGenerationResult.FromResult(...)`
- Private `SaveReport` follows the exact same pattern as `QualityStateReportService.SaveReport`

## Step 6 — DI registration
Add `services.AddSingleton<{Name}ReportService>();` to `Mil.Paperwork.Domain/DomainServicesRegistrator.cs`.

## Step 7 — ReportManager + UI wiring
In `Mil.Paperwork.UI/Managers/ReportManager.cs`:
- Add a private field `IReportService<I{Name}ReportData> _{camelCaseName}ReportService`
- Add the concrete type to the constructor parameters and assign in the constructor body
- Add `public async void Generate{Name}(I{Name}ReportData reportData, Guid? historyEntryId = null)` following the existing `GenerateResidualValueReport` pattern (call service, `TrySaveGeneratedToHistory`, show status dialog)

Then ask the user: "Do you want me to scaffold the ViewModel and View now, or will you handle the UI wiring manually?" Do not create ViewModel/View files without confirmation.

## Step 8 — Config key
Remind the user to:
- Add the report type value to the `ReportType` enum in `Mil.Paperwork.Infrastructure/Enums/`
- Add a matching config section in `Data/ReportDataConfig.json`

After completing all steps, print a summary checklist of what was created/modified and what still requires manual action (template file, config key, enum value, UI wiring if deferred).
