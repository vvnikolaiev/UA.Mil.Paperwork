# Track R — Research only: name the wear-coefficient calculator patterns

No code changes. Output is a decision document. Corresponds to architecture-review candidate 4.

## R1/R2 finding — the original premise doesn't hold

The architecture review assumed most of the 18 `IResidualPriceCalculator` implementations were duplicated *real* logic — each overriding `CalculateTotalWearCoefficient` with a `(category, isLocal) → multiplier` switch, ripe for collapsing into one `CategoryMultiplierCalculator(table)`. Reading all 18 shows that's not what's there:

| Shape | Count | Calculators | `CalculateTotalWearCoefficient` |
|---|---|---|---|
| **Empty stub** | 15 | Ammunition, ArmoredVehicles, Artillery, AutomotiveProperty, ElectronicWarfare, EngineeringAmmunition, Engineering, FoodService, Fuel, MeasuringEquipment, MissileAirDefense, Naval, SmallArms, Topographic, UAV | Flat `=> 1m`. `GetColumnHeaders()` is correctly filled in per Додаток 3, but `GetCoefficients`/`CalculateTotalWearCoefficient` have **zero logic** — not a `(category, isLocal)` switch, just an unconditional constant. |
| **Real, category-table** | 1 | Radiochemical (п.13, РХБЗ) | A genuine `(category, isLocal) → decimal` switch (1 / 0.8 / 0.6 / 0.4 / 0.3-0.2 by category 1–5). The *only* calculator with implemented coefficient logic. Matches Додаток 3's "Таблична (категорія стану)" shape — no independent columns, which is why it doesn't override `GetColumnHeaders`. |
| **Real, multi-factor (disabled)** | 1 | Connectivity (п.22, зв'язок) | Real calls into `CoefficientsHelper.GetExploitationCoefficient`/`GetWorkCoefficient`/`GetTechnicalStateCoefficient`, computing Ке×Кр×Ктс — but the actual returned list is hardcoded to `{1,1,1,1}` with a comment `ЗМІНИ ДО МЕТОДИКИ ОБЧИСЛЮВАННЯ` ("changes to the calculation methodology") sitting above the real, commented-out computation. Structurally matches Додаток 3 п.22 (Ке×Кр×Кзб×Ктс) — just not switched on yet. |

So **17 of 18 calculators have no coefficient logic to consolidate** — they're scaffolding for unwritten Phase 2 work (confirmed against `reference_methodology_residual_value` notes: "Фаза 2 (реальні формули для небойових втрат) — не реалізовано"). There is nothing to collapse today; collapsing empty stubs into a shared class would just be renaming the same no-op.

## Where real consolidation will matter — once Phase 2 exists

`CoefficientsHelper.cs` already has three reusable, methodology-driven component functions: `GetExploitationCoefficient` (Ке), `GetWorkCoefficient` (Кр), `GetTechnicalStateCoefficient` (Ктс) — exactly the pieces Connectivity's disabled code already calls. Cross-referencing the 15 stub calculators' headers against Додаток 3 shows several share an **identical** coefficient set and order, not just a superficial resemblance:

| Formula shape (Додаток 3) | Nomenclatures | Components needed |
|---|---|---|
| Кз × Ке × Кр | MissileAirDefense (п.1), Artillery (п.3), ElectronicWarfare (п.23) | Ке, Кр already in `CoefficientsHelper`; Кз (storage) not yet written |
| Кб × Кд × Кз | ArmoredVehicles (п.6), Engineering (п.7), AutomotiveProperty (п.10) | Кб, Кд, Кз all unwritten |
| Кз × Ке × Кк | SmallArms (п.4), Ammunition (п.5) | Ке in helper; Кз, Кк unwritten |
| Ке × Кх × Ктс | Topographic (п.14), MeasuringEquipment (п.18) | Ке, Ктс in helper; Кх unwritten |
| Ке × Кзб × Ктс | UAV (п.25) | shares Ке, Ктс with Connectivity/above; Кзб partially named in Connectivity already |
| (one-off) | EngineeringAmmunition, FoodService, Fuel, Naval | each formula shape used by only one nomenclature today |

**Recommendation:** when Phase 2 is implemented, consolidate at the **coefficient-component level** (grow `CoefficientsHelper` with Кз/Кб/Кд/Кк/Кх/Кзб/etc., reusing Ке/Кр/Ктс where the same formula already needs them), not by forcing all calculators through one generic `(category, isLocal)` shape. Radiochemical's table-lookup shape is genuinely different from the multiplicative shapes above and should stay its own calculator. Nomenclatures sharing an *exact* coefficient set and order (the three rows above with 2–3 members) could plausibly share one parameterized calculator class once real — but that's a Phase 2 design decision, not something to build against today's empty stubs.

## Steps

- [x] **R1.** Recorded above — which calculators override what, and which have real vs. stub logic.
- [x] **R2.** Cross-checked against Додаток 3 (КМУ 759-98-п) — column headers on all 18 already match the methodology correctly (verified independently of the existing `ResidualValueCalculatorsTests`, which checks the same thing). The "looks identical" grouping in the table above is genuine per the law, not coincidental.
- [x] **R3.** Decision handed back to the user (this document) — **no consolidation work is recommended right now**, since 15 of 18 calculators have no logic yet to consolidate. Revisit when Phase 2 (real non-combat-loss coefficient formulas) is implemented.
