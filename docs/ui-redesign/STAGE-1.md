# Стадія 1 — Shell: сайдбар, навігація, палітра, тема

> Самодостатня інструкція. Перед початком прочитай [PLAN.md](PLAN.md) (цільовий дизайн + конвенції коду). Передумова: гілка `vasnik/uiRedesign`, стадії 2–5 ще не виконані.

## Мета

Замінити TabControl-shell на нову оболонку: HeaderBar у зоні системного заголовка, сайдбар-навігація з singleton-сторінками, смужка вкладок лише для відкритих редакторів звітів, нова палітра з Light/Dark ThemeDictionaries і перемикачем теми з персистом. Редактори звітів усередині поки НЕ змінюються (старі кнопки внизу лишаються) — застосунок повністю робочий після стадії.

## Поточний стан (що є зараз)

- `Mil.Paperwork.UI\Windows\MainWindow.axaml` — Window 1050×500, один `TabControl` (`ItemsSource="{Binding Tabs}"`, `SelectedItem="{Binding SelectedTab}"`), Ctrl+Tab/Ctrl+Shift+Tab у `TabControl.KeyBindings`, два inline DataTemplates для AssetValuation/AssetDismantling з рядами кнопок (рядки ~78–105).
- `Mil.Paperwork.UI\ViewModels\MainWindowViewModel.cs` — `ObservableCollection<ITabViewModel> Tabs`, `SelectedTab`, `SelectedTabIndex`, `NextTabCommand`/`PreviousTabCommand` (модуль по `Tabs.Count` — ділення на нуль при 0 вкладках!), `AddHomeTab()` створює `HomePageViewModel` вручну з 10 залежностей, події `TabAdded`/`TabSelectionRequested`/`TabCloseRequested`.
- `Mil.Paperwork.UI\ViewModels\Tabs\HomePageViewModel.cs` — каталог 9 звітів (`ReportItemViewModel`, `GetAllReportTypes()`), команди відкриття довідників/конфігурацій/історії, оркестрація: `CreateReportTab`, `LoadReportDataIntoTab`, `OnOpenHistoryEntryRequested`, `OnCreateFromEntryRequested`, `OnOpenReportSettingsRequested`.
- `Mil.Paperwork.UI\Styles\CommonStyles.axaml:81` — `Button.CloseTab` з `HotKey="Ctrl+W"`.
- `Mil.Paperwork.UI\Assets\Colors.axaml` — плоский ResourceDictionary: palette-кольори `ThemeLight*`/`ThemeDark*` (для `FluentTheme.Palettes` в App.axaml через StaticResource) + семантичні браші (BrushStatusDraft, BrushStatusGenerated, BrushActionAdd/Delete тощо), акцент #cc4d11.
- `Mil.Paperwork.UI\App.axaml` — FluentTheme з Palettes, ViewLocator-фолбек.
- `Mil.Paperwork.UI\App.axaml.cs` + `ServiceConfigurator` — DI bootstrap.
- `Mil.Paperwork.Infrastructure` — `IFileStorageService`, клас `LocalDataPaths` з констами шляхів (`Data/ReportDataConfig.json` тощо).

## Нові файли

| Файл | Зміст |
|---|---|
| `Mil.Paperwork.UI\Enums\NavigationPageType.cs` | enum: Dashboard, ReportCatalog, History, ProductsDictionary, PeopleDictionary, ServicesDictionary, MeasurementUnitsDictionary, Settings |
| `Mil.Paperwork.UI\ViewModels\Shell\NavigationItemViewModel.cs` | ObservableItem: Title, IconKey (string), PageType, IsSelected; групи сайдбара — окрема властивість GroupTitle або список секцій |
| `Mil.Paperwork.UI\Factories\ReportTabFactory.cs` | Перенесена оркестрація з HomePageViewModel: `Create(ReportType)`, `CreateFromHistoryEntry(Guid)`, `CreateFromConversion(Guid, ReportType)`; централізована підписка `OpenReportSettingsRequested`; singleton у ServiceConfigurator |
| `Mil.Paperwork.UI\ViewModels\Tabs\ReportCatalogViewModel.cs` | Каталог усіх типів звітів (перевикористати `ReportItemViewModel`, `GetAllReportTypes()`); на цій стадії тимчасово служить сторінкою «Головна» |
| `Mil.Paperwork.UI\Views\ReportCatalogView.axaml(+.cs)` | Сітка карток типів звітів |
| `Mil.Paperwork.UI\Views\Shell\SidebarView.axaml(+.cs)` | Навігація: список NavigationItemViewModel, секція «ДОВІДНИКИ», «Налаштування» внизу; згортання до ~48px (лише іконки) керується властивістю IsSidebarCollapsed на MainWindowViewModel |
| `Mil.Paperwork.UI\Views\Shell\HeaderBarView.axaml(+.cs)` | «Сендвіч» + логотип + назва зліва; пошук по центру (TextBox присутній, але неактивний до Стадії 2); перемикач теми; резерв праворуч під системні кнопки |
| `Mil.Paperwork.UI\Views\Shell\DocumentTabStrip.axaml(+.cs)` | Горизонтальний ListBox: Header + хрестик (dirty dot додається на Стадії 3); видимий при `OpenDocuments.Count > 0` |
| `Mil.Paperwork.UI\Services\IUserSettingsService.cs`, `UserSettingsService.cs` | Load/Save `UserSettingsDTO` через `IFileStorageService`; на цій стадії — лише Theme (+ заготовка під Favorites/Recents) |
| `Mil.Paperwork.Infrastructure\DataModels\UserSettingsDTO.cs` | `Theme` (string/enum), `FavoriteReportTypes : List<ReportType>`, `RecentReportTypes : List<ReportType>` |

## Змінювані файли

1. **`MainWindowViewModel.cs`** — подвійна модель:
   - `Dictionary<NavigationPageType, ITabViewModel> _pages` (ліниве створення через ReportTabFactory/DI), `SidebarItems`, `SelectedNavigationItem`.
   - `ObservableCollection<IReportTabViewModel> OpenDocuments`, `SelectedDocument`, `object ActiveContent`.
   - `OpenDocument(IReportTabViewModel)`: додає, підписує `TabCloseRequested`, виділяє.
   - Навігація сайдбаром: ставить `ActiveContent` = сторінка, знімає виділення документа; для History — викликати `Refresh()` при кожній навігації (поточний патерн).
   - `TabCloseRequested` від сторінки → навігація на Dashboard (VM живе); від документа → видалення з `OpenDocuments`, fallback.
   - `NextTabCommand`/`PreviousTabCommand` циклять лише `OpenDocuments` з guard `Count == 0` (інакше ділення на нуль).
   - `CloseActiveDocumentCommand` для Ctrl+W.
   - `IsDarkTheme` → `Application.Current.RequestedThemeVariant` + збереження через `IUserSettingsService`.
   - `IsSidebarCollapsed` (сендвіч).
2. **`MainWindow.axaml`** — нове розкладення:
   ```
   Window: Width=1280 Height=800 MinWidth=1100 MinHeight=650,
           ExtendClientAreaToDecorationsHint="True",
           ExtendClientAreaChromeHints="PreferSystemChrome"
   Grid (Auto headerbar / * body)
   ├─ HeaderBarView
   └─ Grid (Auto sidebar / * content)
      ├─ SidebarView
      └─ DockPanel: DocumentTabStrip (Dock.Top), ContentControl Content={Binding ActiveContent}
   ```
   - `Window.KeyBindings`: Ctrl+Tab, Ctrl+Shift+Tab, **Ctrl+W** → CloseActiveDocumentCommand.
   - Inline DataTemplates для AssetValuation/AssetDismantling ЛИШИТИ (їхні кнопки знімає Стадія 3) — всі `Window.DataTemplates` мають лишитись, бо ContentControl у logical tree вікна.
   - Явні DataTemplates для нових VM (ReportCatalogViewModel → ReportCatalogView): ViewLocator (`FullName.Replace("ViewModel","View")`) НЕ зрезолвить через різні namespace.
3. **`HomePageViewModel.cs`** — оркестрація переїжджає в `ReportTabFactory`; сам клас на цій стадії можна лишити (видаляється на Стадії 2), але MainWindow його більше не показує.
4. **`ServiceConfigurator`** — зареєструвати `ReportTabFactory`, `IUserSettingsService`.
5. **`App.axaml.cs`** — у `OnFrameworkInitializationCompleted` завантажити UserSettings і застосувати `RequestedThemeVariant` ДО створення MainWindow.
6. **`Assets\Colors.axaml`** — семантичні браші у `ResourceDictionary.ThemeDictionaries` (`x:Key="Light"` / `x:Key="Dark"`): `BrushSurface`, `BrushSurfaceAlt`, `BrushSidebar`, `BrushCardBackground`, `BrushBorderSubtle`, `BrushAccent` (teal ~#0F766E), `BrushStatusDraftBg/Fg` (amber), `BrushStatusGeneratedBg/Fg` (green), зберегти `BrushActionAdd`/`BrushActionDelete`. **Palette-ключі `Color` (ThemeLight*/ThemeDark*) лишити плоскими** — App.axaml читає їх StaticResource при парсингу.
7. **`App.axaml`** — нові значення палітри FluentTheme: нейтральна сіра шкала + teal-акцент, Light і Dark варіанти.
8. **`Assets\Icons.axaml`** — додати StreamGeometry (24×24, стиль Fluent System Icons): IconDashboard, IconCreateReport, IconHistory, IconProducts, IconPeople, IconServices, IconMeasurementUnits, IconSettings, IconSearch, IconThemeToggle, IconClose, IconSidebarCollapse (сендвіч).
9. **`Mil.Paperwork.Infrastructure\...\LocalDataPaths`** — const `UserSettings = "Data/UserSettings.json"`.
10. **`Styles\CommonStyles.axaml`** — стиль `Button.CloseTab` ЛИШИТИ (кнопки у view ще живі), але прибрати клас CloseTab з шаблонів, хостованих у MainWindow.axaml, щоб Ctrl+W не спрацьовував двічі (window KeyBinding + HotKey).

## ExtendClientArea — вимоги

- Праворуч у HeaderBar — відступ під системні кнопки (Win: ~140px справа; залежить від DPI — перевірити вживу).
- Порожній draggable-простір обабіч центрального пошуку (drag вікна, подвійний клік = maximize, Snap Layouts — через системний chrome).
- macOS: лівий platform-умовний відступ під «світлофори» (RuntimeInformation.IsOSPlatform).
- Maximized: коригувальний верхній відступ (`OffScreenMargin`).
- Фолбек: якщо проблеми — вимкнути hint, HeaderBar стає панеллю під системним заголовком.

## Критерії готовності та перевірка

1. `dotnet build Mil.Paperwork.WriteOff.sln` без помилок; `dotnet test` — зелений.
2. Skill `ui-verify`:
   - Сайдбар навігує: Головна (тимчасово каталог), Історія, всі 4 довідники — singleton (повторна навігація не створює дубль, стан зберігається).
   - Створення звіту з каталогу відкриває вкладку документа; 2 відкриті редактори — Ctrl+Tab циклить між ними; Ctrl+W закриває активний; смужка вкладок зникає, коли документів нема; при 0 документів Ctrl+Tab не падає.
   - «Сендвіч» згортає сайдбар до іконок і назад.
   - Перемикач теми: живе перемикання світла↔темна, всі екрани читабельні в обох; після рестарту тема відновлюється (`Data/UserSettings.json` створюється).
   - Вікно: перетягування за заголовок, подвійний клік maximize, системні кнопки працюють.
   - Редактори звітів усередині — як раніше (старі кнопки працюють).
3. Оновити чекбокс Стадії 1 у PLAN.md і memory `project_ui_redesign_phases.md`.
