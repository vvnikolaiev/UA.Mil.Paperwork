# Стадія 4 — Довідники, конфігурації, Settings hub

> Самодостатня інструкція. Перед початком прочитай [PLAN.md](PLAN.md). Передумова: Стадії 1–3 завершені (shell, дашборд, RibbonToolbar існує і працює в редакторах).

## Мета

Перевести 4 довідники та 2 конфігураційні view на той самий ribbon-патерн; зібрати налаштування в один Settings hub з внутрішніми секціями; завершити перетворення сторінок на повноцінні singleton-сторінки (без кнопок «Закрити»).

## Поточний стан після Стадії 3

- Довідники (`Views\Dictionaries\`): ProductsDictionaryView (Додати, Видалити, Зберегти, Оновити, Імпортувати, Експортувати-dropdown, Закрити), PeopleDictionaryView (без експорту), MeasurementUnitsDictionaryView (без імпорту/експорту), ServicesDictionaryView (+ Зберегти тимчасово, Встановити за замовчуванням, форма деталей). Верхні StackPanel-тулбари з текстовими кнопками.
- Конфігурації (`Views\Configuration\`): ReportConfigurationView, CommissionsConfigurationView — тулбар (Зберегти, Зберегти тимчасово, Експортувати, Імпортувати, Оновити, Закрити) + селектор типу звіту/комісії.
- VM: `ViewModels\Dictionaries\*` (AddItemCommand, RemoveItemCommand, SaveCommand, SaveLocalCommand, RefreshCommand, ImportCommand, ExportDataCommand(ExportType), SetDefaultCommand), `ViewModels\Tabs\ReportConfigViewModel.cs`, `CommissionsConfigViewModel.cs`, також існують `SettingsViewModel`/`SettingsTabViewModel` і enum `Enums\SettingsTabType.cs`.
- Сторінки вже відкриваються із сайдбара як singleton (Стадія 1), але мають кнопки «Закрити», які зараз обробляються як навігація на Dashboard.
- З редактора звіту «Параметри» (OpenConfigurationCommand) поки відкриває стару конфігураційну сторінку напряму.

## Нові файли

| Файл | Зміст |
|---|---|
| `Mil.Paperwork.UI\ViewModels\Tabs\SettingsHubViewModel.cs` | Секції: «Загальні» (обгортає SettingsViewModel), «Конфігурація звітів» (ReportConfigViewModel), «Комісії» (CommissionsConfigViewModel); SelectedSection; метод `SelectReportType(ReportType)` — відкрити секцію конфігурації звітів з потрібним типом |
| `Mil.Paperwork.UI\Views\SettingsHubView.axaml(+.cs)` | Внутрішня навігація секцій (вертикальний список ліворуч або горизонтальні таби) + ContentControl |

## Зміни

1. **4 довідники + 2 конфігурації**: верхні тулбари → `RibbonToolbar`. Групи:
   - Довідники: «Записи» (Додати, Видалити-IsDestructive), «Дані» (Зберегти, Зберегти тимчасово де є, Оновити), «Обмін» (Імпорт, Експорт де є; для експорту з варіантами JSON/Excel — або два action, або одна кнопка з flyout).
   - ServicesDictionary додатково: «Служба» (Встановити за замовчуванням).
   - Конфігурації: «Дані» (Зберегти, Зберегти тимчасово, Оновити), «Обмін» (Експорт, Імпорт).
   - Кнопки «Закрити» видалити повністю (сторінки singleton); підписи — named const.
   - VM довідників/конфігів отримують `BuildRibbonGroups()` (успадковуються від спільної бази — перевірити ієрархію, ймовірно BaseTabViewModel).
2. **`MainWindowViewModel.cs`**: NavigationPageType.Settings → SettingsHubViewModel; `OpenReportSettingsRequested` (з редакторів через ReportTabFactory) → навігація на Settings hub + `SelectReportType(reportType)` — поточна поведінка передвибору типу зберігається. Гілку «TabCloseRequested від сторінки» можна видалити (кнопок закриття більше нема).
3. **`BaseTabViewModel.Close()`**: діалог підтвердження закриття лишити тільки для документів (редакторів); для сторінок він більше не викликається.
4. **Свіжість singleton-довідників**: при навігації на сторінку довідника викликати наявний `RefreshCommand`/`Refresh()` (дані могли змінитись після імпорту деінде). УВАГА: не втратити незбережені правки без попередження — якщо у VM є брудний стан, спитати підтвердження (наявний діалоговий сервіс `IDialogService`).
5. **`Assets\Icons.axaml`**: IconExport, IconRefresh, IconSetDefault (зірка) — якщо ще не додані.

## Критерії готовності та перевірка

1. `dotnet build` + `dotnet test` зелені; жодного текстового тулбара в Dictionaries/Configuration views.
2. Skill `ui-verify`:
   - Кожен довідник: додати запис, відредагувати, зберегти, оновити, видалити — все через ribbon; імпорт/експорт працюють (Products: JSON і Excel).
   - ServicesDictionary: «Встановити за замовчуванням» працює, зірочка-індикатор видна.
   - Settings hub: три секції перемикаються; зміни в конфігурації звітів зберігаються.
   - З відкритого редактора звіту «Параметри» → Settings hub з правильним типом звіту; повернення на вкладку редактора — стан форми збережено (документ лишився в OpenDocuments).
   - Навігація між довідниками не губить дані; повторний вхід показує свіжі дані.
   - Обидві теми.
3. Оновити чекбокс Стадії 4 у PLAN.md і memory `project_ui_redesign_phases.md`.
