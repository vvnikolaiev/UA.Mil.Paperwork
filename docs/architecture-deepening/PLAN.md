# Architecture deepening plan

Source: architecture review run via the `improve-codebase-architecture` skill on 2026-06-16 (`CONTEXT.md` was written as part of the same session — see repo root). This plan covers the first three review candidates as independent tracks, plus a research-only track for the fourth.

## Context

Two follow-up Explore passes (done after the review, before this plan was written) **narrowed two of the three candidates** versus what the review card originally proposed. Both narrowings are load-bearing:

- **Table-filling (Track B):** Word and Excel table-filling are structurally different (row-wrapper-and-loop vs. direct cell-address-with-dynamic-columns), and even within Word, only two reports (`QualityStateReport`, `AssetDismantlingReport`) share the real "loop a list, add a row, optional summary, optional vertical merge" shape. `TechnicalStateReport` (static/fixed-position rows) and `ResidualValueReport` (Excel, single dynamic-column row) are each the only example of their shape — per the seam principle ("one adapter means a hypothetical seam"), building a shared abstraction for them now would be premature. Track B is scoped to the two Word reports that genuinely share a shape.
- **Snapshot/Mapper (Track C):** Serializing `ReportData` directly (deleting Snapshot+Mapper entirely) is **not safe to attempt** — `IAssetInfo`, `IPerson`, `IProductData`, `IProductIdentification` are polymorphic/interface-typed, `MetalCosts` uses enum-keyed dictionaries, and `DismantlingReportData.ValuationData` is computed-only. None of these round-trip through `System.Text.Json` without custom converters, and the existing JSON files in users' Журнал звітів folders are already persisted in the current Snapshot shape — changing it breaks reopening old history entries. Track C is rescoped to eliminate the boilerplate inside the Mapper bodies via a generic structural-copy utility, keeping hand-written code for mappers that do real work. The wire format does not change, so there is no backward-compatibility risk.

## Tracks

Tracks are independent of each other — any order, no shared files.

- [TRACK-A.md](TRACK-A.md) — Collapse the ReportManager dispatch table
- [TRACK-B.md](TRACK-B.md) — WordTableFiller for the shared list-row pattern
- [TRACK-C.md](TRACK-C.md) — Generic structural-copy utility for Snapshot Mappers
- [TRACK-R.md](TRACK-R.md) — Research only: name the wear-coefficient calculator patterns (no code changes)

## Verification (per track)

- **Track A:** `dotnet build Mil.Paperwork.WriteOff.sln`; run the app and generate one report of each migrated type, confirm dialog message + history entry; no existing automated coverage of `ReportManager`, so this is a manual check.
- **Track B:** `dotnet test Mil.Paperwork.Tests/Mil.Paperwork.Tests.csproj`; add a byte-level output test for `QualityStateReport`/`AssetDismantlingReport` similar to `WriteOffOrderReportTests`; manually diff generated `.docx` against a pre-refactor copy.
- **Track C:** round-trip tests (`ToSnapshot` then `ToReportData` reproduces the original data) plus a raw-JSON diff against pre-refactor output for a fixed sample input, to guarantee old history files still load.
- **Track R:** no automated verification — deliverable is a decision table for the user to review before any consolidation is planned.
