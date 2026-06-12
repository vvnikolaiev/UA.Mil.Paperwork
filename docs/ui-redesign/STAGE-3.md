# Стадія 3 — Ribbon у редакторах звітів + dirty state

> Самодостатня інструкція. Перед початком прочитай [PLAN.md](PLAN.md). Передумова: Стадії 1–2 завершені (shell, DocumentTabStrip, дашборд).

## Мета

Зняти ряди текстових кнопок з усіх 9 редакторів звітів і замінити їх реюзабельною ribbon-панеллю зверху. Додати IsDirty: крапка на вкладці документа, статус «Чернетку збережено о HH:mm» у ribbon.

## Поточний стан після Стадії 2

- Усі 9 report views мають унизу `StackPanel Classes="buttons"` з текстовими кнопками: Згенерувати/Створити документ, Зберегти чернетку, Додати/Видалити рядок, Імпортувати таблицю, Очистити таблицю, Налаштування, Закрити (Ctrl+W — клас `Button.CloseTab`).
- Report views: `Mil.Paperwork.UI\Views\Reports\` — WriteOffOrderView, ResidualValueReportView, AssetValuationView, AssetDismantlingView, InvoiceView, CommissioningActReportView, Handover23ActView, TechnicalStateReportView (+ пакет WriteOffPackage/AssetTechnicalState). Два з них (AssetValuation, AssetDismantling) мають inline DataTemplates з кнопками прямо у `MainWindow.axaml` (~рядки 78–105).
- VM: `BaseTabViewModel` (Header, Close), `BaseReportTabViewModel` (GenerateReportCommand, SaveDraftCommand, CloseTabCommand); табличні команди на `AssetsTableViewModel` (AddRowCommand, RemoveRowCommand, ClearCommand) або на конкретних VM (AddWitnessCommand, AddServiceCommand, ImportRowsCommand, SelectFolderCommand, OpenConfigurationCommand).
- Ctrl+W вже на window-level KeyBinding (Стадія 1); стиль `Button.CloseTab` ще існує.
- `Assets\Icons.axaml` — IconAddTableRow, IconRemoveTableRow + іконки Стадії 1.

## Нові файли

| Файл | Зміст |
|---|---|
| `Mil.Paperwork.UI\Controls\RibbonToolbar.axaml(+.cs)` | UserControl: зовнішній ItemsControl (групи, горизонтально) → внутрішній ItemsControl (дії); кнопка ~64px завширшки: іконка 24px зверху, підпис знизу; підпис групи дрібним текстом під кнопками; вертикальні розділювачі між групами; праворуч — ContentPresenter для статусу. StyledProperty: `Groups`, `StatusContent` |
| `Mil.Paperwork.UI\ViewModels\Ribbon\RibbonGroupViewModel.cs` | GroupTitle, ObservableCollection<RibbonActionViewModel> |
| `Mil.Paperwork.UI\ViewModels\Ribbon\RibbonActionViewModel.cs` | Caption, IconKey (string), Command, CommandParameter, IsDestructive (червоне забарвлення іконки — BrushActionDelete) |
| `Mil.Paperwork.UI\Converters\ResourceKeyToGeometryConverter.cs` | string IconKey → StreamGeometry lookup з ресурсів застосунку; VM не тримають Avalonia-геометрію |

## Модель ribbon у VM

- `BaseTabViewModel`: `IList<RibbonGroupViewModel> RibbonGroups` (ліниво з `protected virtual IList<RibbonGroupViewModel> BuildRibbonGroups()`).
- `BaseReportTabViewModel.BuildRibbonGroups()`: група «Документ» (Сформувати → GenerateReportCommand, Чернетка → SaveDraftCommand) + група «Звіт» (Параметри → OpenConfigurationCommand де є, Тека → SelectFolderCommand де є).
- Конкретні VM додають свою групу «Таблиця» (Додати/Видалити рядок, Імпорт, Очистити) з наявних команд; WriteOffOrder — свої (Додати службу, Додати/Видалити свідка).
- Усі підписи («Документ», «Сформувати»…) — named const поля; без expression-bodied методів; return через локальну змінну.

## Dirty state

- `IReportTabViewModel` (або BaseReportTabViewModel): `bool IsDirty`, `DateTime? LastDraftSavedAt`, текст статусу «Чернетку збережено о HH:mm».
- Встановлення: хук у `OnPropertyChanged` (або підписки на зміни полів/колекцій) ставить `IsDirty = true`.
- **Обов'язково guard `SuspendDirtyTracking`** на час `LoadReportData`/ініціалізації, інакше відкриття чернетки одразу позначиться брудним.
- Скидання: успішний SaveDraft або Generate → `IsDirty = false`, `LastDraftSavedAt = DateTime.Now`.
- `DocumentTabStrip`: у шаблон вкладки додати крапку (Ellipse, amber `BrushStatusDraftBg`-тон), видиму при `IsDirty`.

## Змінювані файли

1. **Усі 9 report views** (`Views\Reports\*.axaml`): прибрати нижній `StackPanel Classes="buttons"`, додати `<controls:RibbonToolbar DockPanel.Dock="Top" Groups="{Binding RibbonGroups}" .../>` зверху. Кнопку «Закрити» НЕ переносити в ribbon — закриття лише хрестиком вкладки та Ctrl+W.
2. **`MainWindow.axaml`**: inline DataTemplates AssetValuation/AssetDismantling скоротити до самих view (`<reportViews:AssetValuationView/>`), кнопкові панелі звідти видалити.
3. **`BaseTabViewModel.cs` / `BaseReportTabViewModel.cs`**: RibbonGroups, IsDirty, LastDraftSavedAt, SuspendDirtyTracking.
4. **`Styles\CommonStyles.axaml`**: видалити стиль `Button.CloseTab` і клас з усіх view (останні використання зникають разом з кнопками).
5. **`Assets\Icons.axaml`**: додати IconGenerateDocument (документ+галочка), IconSaveDraft (дискета), IconImport, IconClearTable, IconFolderOpen, IconParameters (слайдери), IconDelete; перевикористати IconAddTableRow/IconRemoveTableRow.
6. **`App.axaml`**: зареєструвати конвертер/стилі ribbon.

## Критерії готовності та перевірка

1. `dotnet build` + `dotnet test` зелені; `grep "Classes=\"buttons\"" Views\Reports` — порожньо; `grep CloseTab` — порожньо.
2. Skill `ui-verify`, для КОЖНОГО з 9 типів звітів:
   - Кожна колишня кнопка доступна і працює з ribbon (згенерувати документ, зберегти чернетку, додати/видалити рядок, імпорт, очистити, параметри).
   - Редагування поля → крапка на вкладці з'являється; збереження чернетки → крапка зникає, статус у ribbon показує час.
   - Відкриття наявної чернетки з історії НЕ позначає вкладку брудною.
   - Ctrl+W закриває активний документ (з підтвердженням, якщо є).
   - Ribbon читабельний в обох темах; IsDestructive-дії (Видалити, Очистити) — червона іконка.
3. Оновити чекбокс Стадії 3 у PLAN.md і memory `project_ui_redesign_phases.md`.
