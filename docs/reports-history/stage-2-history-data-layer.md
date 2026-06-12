# Stage 2 — History data layer: snapshots, repository, index

**Goal:** compiling, unit-tested persistence layer in `Mil.Paperwork.DataAccess`. No UI changes.

Part of the Reports History feature — see [README.md](README.md). Requires Stage 1 (DataAccess project exists, history keyed on `ReportType`).

## Why dedicated snapshot DTOs

Domain `*ReportData` classes (`Mil.Paperwork.Domain\DataModels\ReportData\*`) hold interface-typed members (`IList<IAssetInfo>` with subtypes `AssetInfo`/`RadiochemicalAssetInfo`/`ConnectivityAssetInfo`, `IProductData`, `IPerson`, `IBookExtractData`) that raw System.Text.Json cannot round-trip. Purpose-built snapshots also decouple the stored schema from domain refactoring and ease a future DB migration.

## Files to create (all in `Mil.Paperwork.DataAccess`)

- `Enums\HistoryEntryStatus.cs` — `Draft = 1, Generated = 2`.
- `DataModels\History\ReportHistoryEntry.cs` — envelope: `int SchemaVersion, Guid Id, ReportType ReportType, HistoryEntryStatus Status, DateTime CreatedAt, DateTime ModifiedAt, string DocumentNumber, string Summary, List<string> GeneratedFiles, ReportSnapshotBase Snapshot`.
- `DataModels\History\ReportHistoryIndexEntry.cs` — envelope metadata minus `Snapshot`, plus denormalized search fields: `List<string> AssetNames, SerialNumbers, NomenclatureCodes`.
- `DataModels\Snapshots\ReportSnapshotBase.cs` — abstract; `[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]` + one `[JsonDerivedType(typeof(...), "invoice")]` per snapshot type. This is the ONLY polymorphic hierarchy.
- `DataModels\Snapshots\AssetSnapshot.cs` — FLAT (no inheritance): all `AssetInfo` props (Name, ShortName, MeasurementUnit, SerialNumber, NomenclatureCode, InitialCategory, Price, Count, StartDate, EventType as int, TSRegisterNumber, TSDocumentNumber, WarrantyPeriodMonths, YearManufactured, ResourceYears) + `AssetSnapshotKind Kind` enum (`Default/Radiochemical/Connectivity`) + subtype extras as nullables. Flat-with-kind is deliberate: version-tolerant, trivially mappable.
- `DataModels\Snapshots\PersonSnapshot.cs`, `ProductSnapshot.cs`, `ProductIdentificationSnapshot.cs`.
- Per-type snapshots mirroring the Domain ReportData classes: `InvoiceReportSnapshot`, `ResidualValueReportSnapshot` (MetalCosts as `Dictionary<string, decimal>` — STJ can't use enum keys), `InitialTechnicalStateReportSnapshot`, `TechnicalStateReportSnapshot`, `WriteOffPackageReportSnapshot` (incl. optional book-extract snapshot), `CommissioningActReportSnapshot` (flatten `IProductData Asset` → `ProductSnapshot`, persons → `PersonSnapshot`), `ValuationReportSnapshot`, `DismantlingReportSnapshot`, `Handover23ReportSnapshot`, `WriteOffOrderReportSnapshot`.
- `Mappers\ReportSnapshotMapper.cs` (+ internal per-type mappers):
  - `ReportSnapshotBase ToSnapshot(ReportType type, IReportData data)`
  - `IReportData ToReportData(ReportSnapshotBase snapshot)`
  - `ReportHistoryIndexEntry ToIndexEntry(ReportHistoryEntry entry)` (extracts denormalized asset names/serials/codes per type)
  - Asset reverse mapping switches on `Kind` to instantiate the correct `AssetInfo` subtype.
- `Repositories\IReportHistoryRepository.cs`:
  ```csharp
  IReadOnlyList<ReportHistoryIndexEntry> GetIndex();
  ReportHistoryEntry? GetEntry(Guid id);
  void Save(ReportHistoryEntry entry);
  void Delete(Guid id);
  void RebuildIndex();
  ```
- `Repositories\JsonReportHistoryRepository.cs` — layout: `Data/History/index.json` + `Data/History/Entries/{id}.json` next to the exe (same base as `Data/SimpleDataStorage.json`). Does its own `File`/`Directory` IO (existing `IFileStorageService.ReadJsonFile` throws on missing files and has no enumerate/delete). Shared `Helpers\HistoryJsonOptions.cs` (WriteIndented + polymorphic options). `GetIndex()` falls back to `RebuildIndex()` (scan entry files, skip unreadable) when index is missing/corrupt. `Save` = upsert entry file + index row, sets `ModifiedAt` and `SchemaVersion = HistorySchema.CurrentVersion`.
- `Helpers\HistorySchema.cs` — `public const int CurrentVersion = 1;`
- Register `IReportHistoryRepository` → `JsonReportHistoryRepository` (singleton) in `DataAccessServicesRegistrator`.

## Verify

xUnit tests in `Mil.Paperwork.Tests\History\`:
- `JsonReportHistoryRepositoryTests` against a temp directory: save/load/delete; index row appears/disappears; delete `index.json` → `GetIndex()` rebuilds; one corrupt entry file → skipped, others returned.
- `ReportSnapshotMapperTests`: ReportData → snapshot → ReportData round-trip for every report type, including all three asset kinds and string-keyed MetalCosts.
- Serialize a `ReportHistoryEntry` containing each snapshot type and deserialize (polymorphic discriminator works).

`dotnet build` + `dotnet test` pass.
