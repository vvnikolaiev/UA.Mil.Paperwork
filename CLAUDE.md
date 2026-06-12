# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

A Ukrainian military paperwork automation tool that generates official military documents (Word/Excel) for write-off (списання майна) procedures. Built with .NET 8 and Avalonia UI for cross-platform desktop use (Windows x64, macOS arm64).

## Commands

```bash
# Build the solution
dotnet build Mil.Paperwork.WriteOff.sln

# Run all tests
dotnet test Mil.Paperwork.Tests\Mil.Paperwork.Tests.csproj

# Run a single test class
dotnet test Mil.Paperwork.Tests\Mil.Paperwork.Tests.csproj --filter "ClassName~ResidualPriceHelperTests"

# Run a single test method
dotnet test Mil.Paperwork.Tests\Mil.Paperwork.Tests.csproj --filter "FullyQualifiedName~GetIndexationCoefficient_Returns1"

# Run the app
dotnet run --project Mil.Paperwork.UI

# Publish for Windows
dotnet publish Mil.Paperwork.UI --runtime win-x64 --configuration Release

# Publish for macOS
dotnet publish Mil.Paperwork.UI --runtime osx-arm64 --configuration Release
```

**Releasing**: push a git tag to trigger CI/CD — creates a GitHub Release with Windows MSIX and macOS app bundles:
```bash
git tag 1.2.3.4
git push origin --tags
```

## Solution Structure

| Project | Role |
|---|---|
| `Mil.Paperwork.UI` | Avalonia UI shell — Views, ViewModels, DI wiring |
| `Mil.Paperwork.Domain` | Business logic — report services, helpers, calculators, report data models, Word/Excel templates |
| `Mil.Paperwork.Infrastructure` | Data persistence — JSON file storage, config loading, DTOs |
| `Mil.Paperwork.DataAccess` | Data access services on top of Infrastructure (`IDataService` for products/people/units) |
| `Mil.MVVM.Common` | Base MVVM classes (`ObservableItem`, `DelegateCommand`) |
| `Mil.Paperwork.Common` | Shared utilities (common data models; partially superseded by `Mil.MVVM.Common`) |
| `Mil.Paperwork.Tests` | xUnit tests targeting `Mil.Paperwork.Domain` |
| `Mil.Paperwork.Package` | MSIX packaging project for Windows Store sideloading |

## Architecture

### Dependency Injection

DI is bootstrapped in `Mil.Paperwork.UI/App.axaml.cs` via `ServiceConfigurator`, which calls:
- `InfrastructureServicesRegistrator.Register()` — registers `IFileStorageService`, `IReportDataService`
- `DataAccessServicesRegistrator.Register()` — registers `IDataService`
- `DomainServicesRegistrator.Register()` — registers all `*ReportService` singletons and `IExportService`/`IImportService`

### Report Generation Flow

```
UI ViewModel → ReportManager → IReportService<TData>.TryGenerateReport(data)
                                    └─ creates IReport → fills template → saves file
```

- `ReportManager` (`Mil.Paperwork.UI/Managers/ReportManager.cs`) is the single entry point the UI calls. It holds all report services and dispatches calls, then shows a status dialog.
- Each report type has a dedicated service (`QualityStateReportService`, `ResidualValueReportService`, etc.) implementing `IReportService<T>`.
- `IReport.GetReportBytes()` returns the generated document as bytes.

### Template System

**Word templates** (`.docx`, in `Mil.Paperwork.Domain/Templates/`):
- Fields use `«FIELD_NAME»` merge-field syntax (guillemet brackets, not curly braces).
- `WordDocumentHelper.ReplaceFields(document, fieldsMap)` performs substitution via FreeSpire.Doc.
- Tables are identified by their `Title` property and manipulated programmatically.

**Excel templates** (`.xlsx`, in `Mil.Paperwork.Domain/Templates/`):
- Named ranges follow the pattern `FIELD_RANGE_N` (e.g. `FIELD_RANGE_1`, `FIELD_RANGE_2`).
- Cell text uses `[FIELD_NAME]` placeholders; `ExcelDocumentHelper.MapDataToTheNamedFields()` substitutes them.
- Scope must be set to **Workbook** (not Sheet) when adding new named ranges.

### Data & Configuration

- `Data/ReportDataConfig.json` — semi-static per-unit config: military unit info, commission members, per-report-type field values. Edited by users or via the Settings UI.
- `Data/SimpleDataStorage.json` — products, people, measurement units entered at runtime.
- `ReportDataService` loads and caches config; `DataService` handles products/people CRUD.

### Residual Price Calculators

Asset-type-specific calculators implement `IResidualPriceCalculator`:
- `DefaultResidualPriceCalculator`
- `RadiochemicalResidualPriceCalculator`
- `ConnectivityResidualPriceCalculator`

Each provides `GetCoefficients(asset, date)` and `CalculateTotalWearCoefficient(asset, date)`.

### Platform-specific Code

`ExcelDocumentHelper` uses `#if WINDOWS` guards for `System.Drawing`-based text height measurement, with a cross-platform fallback estimator.

## Adding a New Report Type

1. Add a `.docx` or `.xlsx` template to `Mil.Paperwork.Domain/Templates/`.
2. Define `IXxxReportData` interface and `XxxReportData` implementation in `Domain/DataModels/ReportData/`.
3. Create `XxxReport` implementing `IReport` in `Domain/Reports/`.
4. Create `XxxHelper` in `Domain/Helpers/` for template-filling logic.
5. Create `XxxReportService : IReportService<IXxxReportData>` in `Domain/Services/`.
6. Register the service in `DomainServicesRegistrator.Register()`.
7. Add a method to `ReportManager` and wire it to a new ViewModel + View in `Mil.Paperwork.UI`.
8. Add a config section key in `Data/ReportDataConfig.json` matching the new `ReportType` enum value.
