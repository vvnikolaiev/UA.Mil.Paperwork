# Стадія 2 — Дашборд + бейджі історії + глобальний пошук

> Самодостатня інструкція. Перед початком прочитай [PLAN.md](PLAN.md). Передумова: Стадія 1 завершена (shell із сайдбаром, ThemeDictionaries, `UserSettingsService`, `ReportTabFactory` існують).

## Мета

Справжня сторінка «Головна»: картки створення звітів (обрані ★ + нещодавні + «Інший звіт…»), метрики, таблиця останніх документів. Рестайл History (бейджі, чіпи фільтрів). Робочий глобальний пошук у HeaderBar. Видалення HomePageViewModel/HomePageView.

## Поточний стан після Стадії 1

- Сторінкою «Головна» тимчасово служить ReportCatalogViewModel.
- `UserSettingsService` зберігає Theme; поля FavoriteReportTypes/RecentReportTypes у DTO вже є, але не використовуються.
- History: `Mil.Paperwork.UI\Views\HistoryView.axaml` + `ViewModels\Tabs\HistoryViewModel.cs` — фільтри (тип, дати, пошук `MatchesSearchText` по DocumentNumber/AssetNames/SerialNumbers/NomenclatureCodes), DataGrid зі статус-крапками (Ellipse, BrushStatusDraft/Generated), контекстне меню (Відкрити, Відкрити файл, Видалити, Створити…), `OpenFileFolderCommand`, відкриття файлів через Process.Start.
- Дані історії: `Mil.Paperwork.DataAccess\DataModels\History\ReportHistoryIndexEntry.cs` (Status Draft=1/Generated=2, ModifiedAt, DocumentNumber, Summary, GeneratedFiles, AssetNames, SerialNumbers, NomenclatureCodes), `IReportHistoryRepository.GetIndex()`.
- HeaderBar: пошуковий TextBox присутній, неактивний.

## Нові файли

| Файл | Зміст |
|---|---|
| `Mil.Paperwork.UI\ViewModels\Tabs\DashboardViewModel.cs` | Метрики + картки + останні записи; залежності: `IReportHistoryRepository`, `IUserSettingsService`, `ReportTabFactory` |
| `Mil.Paperwork.UI\Views\DashboardView.axaml(+.cs)` | Розкладення згідно з мокапом: ряд карток → ряд метрик → таблиця останніх документів |
| `Mil.Paperwork.UI\ViewModels\Dashboard\ReportCardViewModel.cs` | ReportType, Title, IconKey, IsFavorite (зірочка-toggle, пише в UserSettings одразу), CreateCommand |
| `Mil.Paperwork.UI\ViewModels\Dashboard\MetricCardViewModel.cs` | Label, Value |
| `Mil.Paperwork.UI\Helpers\ShellOpenHelper.cs` | Винесена з HistoryViewModel логіка відкриття файлу/теки (Process.Start, UseShellExecute — кросплатформно); HistoryViewModel і Dashboard використовують спільно |
| `Mil.Paperwork.UI\Styles\BadgeStyles.axaml` | Стиль статус-бейджа: Border з заокругленням, фон/текст із `BrushStatusDraftBg/Fg` (amber) та `BrushStatusGeneratedBg/Fg` (green) |

## Логіка дашборда

- Метрики з `IReportHistoryRepository.GetIndex()`:
  - «Чернетки в роботі» = `Status == Draft`;
  - «Сформовано цього місяця» = `Status == Generated` і `ModifiedAt` у поточному місяці;
  - «Всього документів» = кількість записів.
- Останні документи: топ-12 за `ModifiedAt` desc. Колонки: Документ (тип), Стан (бейдж), Змінено, № + майно (`NumberSummaryText`-патерн з HistoryEntryViewModel), Файл (відкрити файл/теку через ShellOpenHelper). Подвійний клік / Enter — відкрити запис у редакторі (через ReportTabFactory, як у History).
- **Статус «на підписанні» НЕ додається** — лише наявні Draft/Generated (рішення користувача; якщо знадобиться — додавати enum-значення адитивно).
- Ряд карток: спершу обрані (FavoriteReportTypes), потім нещодавні (RecentReportTypes, без дублів з обраними), разом до 3–4 шт + картка «Інший звіт…» → навігація на ReportCatalog.
- `RecentReportTypes` оновлюється в `MainWindowViewModel.OpenDocument()` (або ReportTabFactory) при кожному створенні звіту: тип на початок списку, без дублів, ліміт ~5.
- Оновлення метрик/таблиці — при кожній навігації на Dashboard (патерн `HistoryViewModel.Refresh()`).
- Усі підписи метрик/карток — named const поля (конвенція).

## Змінювані файли

1. **`MainWindowViewModel.cs`**: сторінка Dashboard стає «Головною» (NavigationPageType.Dashboard); `ExecuteGlobalSearch(query)`: навігація на History → `ClearFiltersCommand.Execute` → `HistoryViewModel.SearchText = query`. Жодної нової пошукової логіки — наявний `MatchesSearchText` усе покриває.
2. **`Views\Shell\HeaderBarView.axaml`**: активувати пошук — Enter/кнопка викликає `ExecuteGlobalSearch`.
3. **`HistoryView.axaml`**: статус-крапки (Ellipse) → бейджі з BadgeStyles; панель фільтрів → компактні чіпи (ComboBox типу, дати, пошук, «Очистити»); кнопку «Закрити» видалити (singleton-сторінка). Решта (контекстне меню, сортування, колонки) — без змін.
4. **`HistoryEntryViewModel.cs`**: властивість для тексту/класу бейджа (StatusText вже може існувати — перевірити; інакше додати).
5. **`HistoryViewModel.cs`**: винести відкриття файлу/теки у `ShellOpenHelper`, використати його.
6. **Видалити** `ViewModels\Tabs\HomePageViewModel.cs` + `Views\HomePageView.axaml(+.cs)` + їх DataTemplate/реєстрації. Перед видаленням переконатися, що вся оркестрація вже у `ReportTabFactory`, а команди відкриття довідників — у сайдбарі.
7. **`App.axaml`** — підключити `BadgeStyles.axaml`; DataTemplate для DashboardViewModel.

## Критерії готовності та перевірка

1. `dotnet build` + `dotnet test` зелені; решток посилань на HomePageViewModel нема (`grep HomePage` порожній).
2. Skill `ui-verify`:
   - Дашборд: метрики відповідають реальному вмісту історії (звірити з Data-файлами історії); таблиця показує останні записи з бейджами.
   - Зірочка на картці: toggle персистить після рестарту (UserSettings.json).
   - Створення звіту з картки відкриває редактор; тип з'являється у «нещодавніх» після рестарту.
   - «Інший звіт…» → каталог; «Уся історія →» → History.
   - Глобальний пошук: ввести серійник → History відкрита з відфільтрованим списком.
   - History: бейджі замість крапок, фільтри/сортування/контекстне меню працюють як раніше.
   - Обидві теми читабельні (бейджі мають окремі Light/Dark кольори).
3. Оновити чекбокс Стадії 2 у PLAN.md і memory `project_ui_redesign_phases.md`.
