# Track C — Generic structural-copy utility for Snapshot Mappers

Independent of A and B. Touches `Mil.Paperwork.DataAccess/Mappers/`.

## Problem

Every report type that supports history has a hand-written Mapper doing manual field-by-field `ToSnapshot`/`ToReportData` copying. Most mappers do nothing but restate the same field list twice — but a few do real polymorphic/enum bridging that must stay hand-written. See `PLAN.md` Context section for why direct `ReportData` serialization was ruled out.

## Steps

- [ ] **C1. Build `StructuralMapper.Copy<TSource, TDest>` as an internal reflection-based utility.**
  Copies all public properties from `TSource` to a new `TDest` by matching property name: scalar properties direct-assigned, `IList<TItem>`/nested-object properties recursed (or via a caller-supplied sub-mapper delegate — confirm the simpler design works against C2 before generalizing further). Must produce output **byte-identical** to what the current hand-written mapper produces — no field drops, no renames.

- [ ] **C2. Prove it against `WriteOffOrderSnapshotMapper` first.**
  Confirmed mechanical 1:1 copying at every level — top-level fields plus three nested list conversions (`Services`→`WriteOffServiceSnapshot`, nested `Assets`→`WriteOffServiceAssetSnapshot`, `Witnesses`→`WriteOffWitnessSnapshot`) — zero enum conversion, zero polymorphism, zero computed-property exclusion. Replace its body with `StructuralMapper.Copy` calls. Write a round-trip test (`ToSnapshot` then `ToReportData` reproduces the original `IWriteOffOrderReportData`) **and** a raw-JSON diff against pre-refactor output, to lock in the wire format is unchanged.

- [ ] **C3. Roll out to the remaining "pure copy" mappers, one file per step.**
  Candidates: `InvoiceSnapshotMapper`, `Handover23SnapshotMapper`, `CommissioningActSnapshotMapper`, `ValuationSnapshotMapper`, `TechnicalStateSnapshotMapper`, `InitialTechnicalStateSnapshotMapper`, `WriteOffPackageSnapshotMapper`. **Confirm each is genuinely a pure-copy case before migrating** — these were not individually verified during research, unlike `WriteOffOrderSnapshotMapper`; check for enum/interface fields first. Each file is its own independent, low-risk step with its own round-trip + JSON-diff test.

## Explicitly stays hand-written (do not migrate)

- `AssetSnapshotMapper` — discriminates `IAssetInfo` → `AssetInfo`/`ConnectivityAssetInfo`/`RadiochemicalAssetInfo` via a `Kind` enum; real polymorphic bridging.
- `ResidualValueSnapshotMapper` — converts `IDictionary<MetalType, decimal>` to `Dictionary<string, decimal>`; real conversion logic.
- `DismantlingSnapshotMapper` — must skip the computed-only `ValuationData` property.
- `PersonSnapshotMapper`, `ProductSnapshotMapper`, `ProductIdentificationSnapshotMapper` — bridge interface types (`IPerson`, `IProductData`, `IProductIdentification`), same category as `AssetSnapshotMapper`.

## Verification

Round-trip tests (`ToSnapshot` then `ToReportData` reproduces the original data) plus a raw-JSON diff against pre-refactor output for a fixed sample input — must match byte-for-byte, to guarantee old history files still load.
