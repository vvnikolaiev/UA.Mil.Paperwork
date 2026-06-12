# Stage 1 — Foundation refactor: DataAccess project, move DataService, unify enums

**Goal:** pure refactor, zero behavior change. App builds and runs identically afterwards.

Part of the Reports History feature — see [README.md](README.md). Target project is `Mil.Paperwork.UI` (Avalonia); never touch the legacy `Mil.Paperwork.WriteOff\` folder.

## 1. Create `Mil.Paperwork.DataAccess`

- New net8.0 classlib at repo root: `Mil.Paperwork.DataAccess\Mil.Paperwork.DataAccess.csproj`.
- References: `Mil.Paperwork.Domain`, `Mil.Paperwork.Infrastructure`.
- Add to `Mil.Paperwork.WriteOff.sln`; add project references from `Mil.Paperwork.UI` and `Mil.Paperwork.Tests`.
- Create `DataAccessServicesRegistrator.cs` mirroring `Mil.Paperwork.Infrastructure\InfrastructureServicesRegistrator` (static `Register(IServiceCollection)`), and call it from `Mil.Paperwork.UI\Configuration\ServiceConfigurator.ConfigureServices`.

## 2. Move DataService

Move from `Mil.Paperwork.Infrastructure\Services\` into `Mil.Paperwork.DataAccess`:

- `IDataService.cs`, `DataService.cs` → `Mil.Paperwork.DataAccess\Services\` (namespace `Mil.Paperwork.DataAccess.Services`).
- `SimpleDataStorageDTO` → `Mil.Paperwork.DataAccess\DataModels\` — first verify it isn't referenced outside `DataService`; if it is, leave it in Infrastructure.
- Broadly-used DTOs (`ProductDTO`, `PersonDTO`, `MeasurementUnitDTO`, `AssetValuationData`, …) STAY in Infrastructure.
- Move the `IDataService` DI registration from `InfrastructureServicesRegistrator` to `DataAccessServicesRegistrator` (keep same lifetime).
- Update `using` directives across `Mil.Paperwork.UI` (many report/dictionary ViewModels inject `IDataService`).
- `DataService` keeps reading `Data/SimpleDataStorage.json` via `IFileStorageService` — path and format unchanged.

## 3. Unify document-type enums on `ReportType`

Today two near-duplicate enums exist:
- `Mil.Paperwork.Infrastructure\Enums\ReportType.cs` — public, 12 values, Ukrainian `[Description]`s, used for report parameter config.
- `Mil.Paperwork.UI\Enums\DocumentTypeEnum.cs` — internal, drives the home-page tiles and `HomePageViewModel.OpenNewReportTab` switch.

Verified 1:1 mapping of active tabs:

| DocumentTypeEnum | ReportType |
|---|---|
| WriteOffOrder | WriteOffOrder |
| ResidualValue | ResidualValueReport |
| WriteOffPackage | WriteOffPackage |
| TechnicalState7 | TechnicalStateReport (TS7 VM already opens `ReportType.TechnicalStateReport` settings) |
| Valuation | AssetValuationReport |
| Dismantling | AssetDismantlingReport |
| Invoice | Invoice |
| CommisioningAct | CommissioningAct |
| HandoverCertificate23 | Handover23Act |
| WriteOff (obsolete, already commented out of home list) | — drop |

Steps:
1. Add explicit numeric values to `ReportType` freezing CURRENT ordinals (`Common = 0` … `WriteOffOrder = 11`). Before committing, check how `ReportType` is serialized in `Data/ReportDataConfig.json` (string names vs numbers) and confirm an existing config still loads.
2. Replace `DocumentTypeEnum` usages in `Mil.Paperwork.UI`: home document list and `OpenNewReportTab(...)` switch in `ViewModels\Tabs\HomePageViewModel.cs`, plus `HomePageDocumentViewModel` (tile VM). Key everything on `ReportType`.
3. Home tile labels come from `ReportType.GetDescription()`. Where the `DocumentTypeEnum` wording was better for a tile (e.g. "Акт тех. стану (№7)", "Пакет зі списання"), update the `ReportType` `[Description]` text — but check other display usages of those descriptions first (e.g. `ReportConfigViewModel.ExportFileTitle`).
4. Delete `Mil.Paperwork.UI\Enums\DocumentTypeEnum.cs`.

## Verify

- `dotnet build` and `dotnet test` pass.
- Run the app: every home tile opens its tab with the right header; products/people/units dictionaries still load from existing `Data/SimpleDataStorage.json`; report configuration screens load existing `Data/ReportDataConfig.json` values.
