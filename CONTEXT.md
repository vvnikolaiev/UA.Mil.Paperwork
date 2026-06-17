# CONTEXT.md

Domain glossary for Mil.Paperwork.WriteOff. This file names the concepts the codebase is built around, so that code, conversations, and architecture reviews can refer to them consistently. It complements `CLAUDE.md` (commands, solution layout, dev workflow) — this file is about *what the domain means*, not how to build it.

## What this tool does

Generates official Ukrainian military paperwork (Word/Excel) for **списання майна** (write-off of property/equipment): the multi-document procedure a military unit follows to formally retire, dismantle, value, or hand over assets. Before this tool, each document was hand-copied field-by-field from the same source data — the tool's purpose is to enter the data once and generate every required document from it.

## Core domain concepts

**Report** (Звіт)
A single generated document — a filled-in Word or Excel file corresponding to one official form (e.g. "Акт якісного стану", "Відомість залишкової вартості"). Identified by `ReportType`. Eleven report types exist today (see `Mil.Paperwork.Infrastructure/Enums/ReportType.cs`):

| ReportType | Ukrainian name | Format |
|---|---|---|
| `QualityStateReport` | Акт якісного стану | Word |
| `TechnicalStateReport` | Акт тех. стану (№7) | Word |
| `WriteOffAct` | Акт списання | Word |
| `ResidualValueReport` | Відомість залишкової вартості | Excel |
| `AssetValuationReport` | Акт оцінки | Word |
| `AssetDismantlingReport` | Розукомплектування | Word |
| `Invoice` | Накладна (вимога) | Word |
| `CommissioningAct` | Акт введення в експлуатацію | Word |
| `Handover23Act` | Акт прийому-передачі ОЗ | Word |
| `WriteOffPackage` | Пакет зі списання | combines several of the above |
| `WriteOffOrder` | Наказ про списання | Word |

**Asset** (Майно / Засіб)
A piece of military property being written off, valued, or dismantled — identified by `AssetType` (a nomenclature category) plus instance data (name, serial number, price, count, dates). Drives which `IResidualPriceCalculator` applies.

**Residual value / wear coefficient** (Залишкова вартість / Коефіцієнт зносу)
The depreciated value of an asset at write-off time, computed per the methodology in Додаток 3 (КМУ 759-98-п). Each `AssetType` nomenclature has its own coefficient rules — see `IResidualPriceCalculator` implementations in `Mil.Paperwork.Domain/Calculators/`.

**Commission** (Комісія)
The group of named officers who sign off on a report. Configured once in `Data/ReportDataConfig.json` and reused across reports — filled into every document's signature block.

**Report Data / ReportData** (per-report input)
The in-memory payload a user fills in for one report (e.g. `IResidualValueReportData`). Implements a per-`ReportType` interface in `Mil.Paperwork.Domain/DataModels/ReportData/`. This is the seam between the UI and report generation — UI ViewModels build a `ReportData`, hand it to `ReportManager`.

**Template** (Шаблон)
The source `.docx`/`.xlsx` file with merge fields (`«FIELD_NAME»` in Word, `[FIELD_NAME]` named ranges in Excel) that gets filled per report. Lives in `Mil.Paperwork.Domain/Templates/`. See `CLAUDE.md` for the field-substitution mechanics.

**Config** (semi-static unit data)
`Data/ReportDataConfig.json` — military unit identity, commission members, and per-report-type field defaults that rarely change. Distinct from data entered per-document (assets, dates, counts).

**Журнал звітів (Report History)**
An auto-saved log of every generated report, keyed by `ReportType` plus the full `ReportData` used to produce it. Lets a user reopen, re-edit, or convert a past report into a different report type ("Створити…" conversions, see README). Persisted as a **Snapshot** (one per report type, in `Mil.Paperwork.DataAccess/DataModels/Snapshots/`) via a **Mapper** (`ToSnapshot`/`ToReportData`, in `Mil.Paperwork.DataAccess/Mappers/`).

**Чернетка (Draft)**
A report-history entry saved before generation completes (status "Чернетка" vs "Сформовано") — lets a user resume an in-progress report.

**Write-off package** (Пакет зі списання)
A `ReportType` that is itself a bundle: generating it triggers several underlying reports (quality state, technical state, residual value, valuation, dismantling) in one user action. See `ReportManager.GenerateWriteOffReport`.

## Module map (domain framing, not build layout — see CLAUDE.md for that)

- **ReportManager** — the single seam between UI and report generation. Receives a `ReportData`, hands it to the matching `IReportService<T>`, shows the result, and (for most report types) records the result to Report History.
- **ReportService** (one per `ReportType`) — orchestrates: build the `Report`, call `TryCreate`, save the output file(s).
- **Report** (one per `ReportType`) — owns one template, fills its fields and tables, returns bytes. Currently does its own cell-by-cell table addressing (see architecture review, candidate 2).
- **ReportHelper** (one per `ReportType`) — holds the field-name and column-index constants a `Report` needs to address its template.
- **ResidualPriceCalculator** (one per nomenclature-with-distinct-coefficients) — pure calculation of wear coefficients for one `AssetType` family.
- **ReportHistoryService** — persists/restores `ReportData` as `Snapshot`s for the Журнал.

## Where domain rules live (not just code)

- **Додаток 3, КМУ 759-98-п** — the legal methodology for residual-value wear coefficients per nomenclature. Source of truth for `IResidualPriceCalculator` behavior; don't change coefficient logic without checking against it.
- `Data/ReportDataConfig.json` — source of truth for unit-specific, semi-static report data (edited by end users, not just code).

## Notes for future architecture reviews

- No ADRs exist yet in this repo (`docs/adr/` does not exist as of 2026-06-16).
- A legacy, unbuilt project (`Mil.Paperwork.WriteOff/`, not referenced in `Mil.Paperwork.WriteOff.sln`) still sits in the repo root. Its classes (including an old `ReportManager.cs`) are dead code — do not confuse them with the real implementations under `Mil.Paperwork.UI/` and `Mil.Paperwork.Domain/`.
