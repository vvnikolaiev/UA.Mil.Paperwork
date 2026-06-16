# Track R — Research only: name the wear-coefficient calculator patterns

No code changes. Output is a written decision table, not a refactor. Corresponds to architecture-review candidate 4.

## Problem

18 `IResidualPriceCalculator` implementations exist in `Mil.Paperwork.Domain/Calculators/`, but most fall into one shape: override `CalculateTotalWearCoefficient` with a `(category, isLocal)` → coefficient switch. The codebase doesn't name that shape, so each nomenclature gets a bespoke class even when its behaviour is structurally identical to several siblings.

## Steps

- [ ] **R1.** For each of the 18 implementations, record: which methods it overrides beyond `DefaultResidualPriceCalculator`, and whether its `CalculateTotalWearCoefficient` is a flat `(category, isLocal) → multiplier` switch (candidate for consolidation into a `CategoryMultiplierCalculator(table)`) or something structurally different (must stay bespoke).

- [ ] **R2.** Cross-check that table against Додаток 3 (КМУ 759-98-п) — confirm which "looks identical in code" calculators are actually supposed to be identical per methodology, versus which only look alike today by coincidence. Flag anything where consolidating would risk silently changing a legally-defined coefficient.

- [ ] **R3.** Hand the resulting table back to the user before any consolidation work is planned — this track ends in a decision document, not a plan for code changes.
