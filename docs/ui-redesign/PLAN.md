# UI/UX Redesign — Master Plan

> Гілка: `vasnik/uiRedesign`. Кожна стадія виконується в окремій сесії за своїм файлом `STAGE-N.md` (самодостатній). Після завершення стадії — оновити статус-трекер нижче і memory-файл `project_ui_redesign_phases.md`.

## Статус-трекер

- [x] Стадія 1 — Shell: сайдбар, навігація, палітра, тема ([STAGE-1.md](STAGE-1.md))
- [x] Стадія 2 — Дашборд + бейджі історії + глобальний пошук ([STAGE-2.md](STAGE-2.md))
- [ ] Стадія 3 — Ribbon у редакторах + dirty state ([STAGE-3.md](STAGE-3.md))
- [ ] Стадія 4 — Довідники, конфігурації, Settings hub ([STAGE-4.md](STAGE-4.md))
- [ ] Стадія 5 — Полірування + документація ([STAGE-5.md](STAGE-5.md))

## Контекст

Поточний UI — один `TabControl`: Home-вкладка з 9 квадратними кнопками звітів + 7 текстовими кнопками знизу; решта функціоналу (редактори звітів, історія, 4 довідники, 2 конфігурації) відкривається новими вкладками, кожна з власним рядом текстових кнопок унизу. Це незручно й не масштабується.

Мета — сучасний інтерфейс у стилі канви Word/Налаштувань Windows: бічна навігація, інформативна головна сторінка, ribbon-панелі замість розкиданих кнопок, нова стримана палітра зі світлою та темною темами.

## Цільовий дизайн (затверджено користувачем через мокапи)

### Вікно та заголовок
- HeaderBar **інтегрований у зону системного заголовка** (як Налаштування Windows): `ExtendClientAreaToDecorationsHint="True"` + `ExtendClientAreaChromeHints="PreferSystemChrome"` — кнопки Minimize/Maximize/Close лишаються рідними системними.
- В одному ряду заголовка: кнопка-«сендвіч» (згортання сайдбара) + логотип + назва зліва → глобальний пошук по центру → перемикач теми → системні кнопки праворуч.
- Розмір вікна 1280×800, Min 1100×650 (замість 1050×500).

### Навігація
- **Сайдбар** (згортається до іконок ~48px): Головна, Створити звіт, Історія; група «ДОВІДНИКИ»: Майно, Особи, Служби, Од. виміру; внизу — Налаштування. Сторінки сайдбара — singleton, не вкладки.
- **Вкладки документів**: лише відкриті редактори звітів; смужка вкладок над контентом, видима тільки коли є відкриті документи. Закриття — хрестиком на вкладці. Крапка-індикатор = **незбережені зміни (IsDirty)**: з'являється після редагування, зникає після збереження чернетки/формування.

### Головна (дашборд)
- Великі картки «Створити звіт»: обрані (зірочка-toggle) → нещодавно використані → «Інший звіт…» (повний каталог).
- Метрики: «Чернетки в роботі», «Сформовано цього місяця», «Всього документів». Статусу «на підписанні» НЕ буде — лише наявні Draft/Generated.
- Таблиця останніх 10–15 документів: тип, статус-бейдж (Чернетка amber / Сформовано green), дата, № + майно, посилання на файл/теку; лінк «Уся історія →».

### Редактори звітів
- Замість нижніх текстових кнопок — **ribbon-панель** зверху з групами: «Документ» (Сформувати, Чернетка), «Таблиця» (Додати/Видалити рядок, Імпорт, Очистити), «Звіт» (Параметри, Тека). Великі іконки 24px з підписами, підпис групи знизу, розділювачі. Праворуч — статус «Чернетку збережено о HH:mm».
- Той самий ribbon-патерн — у довідниках і конфігураціях.

### Палітра
- Нейтральні поверхні + один спокійний акцент teal (~#0F766E); статус-бейджі amber/green; обов'язково світла І темна теми з живим перемиканням.

## Архітектурні рішення (спільні для всіх стадій)

### Навігаційна модель
`MainWindowViewModel` розділяє колекцію `Tabs` на:
- **Singleton-сторінки**: кеш `Dictionary<NavigationPageType, ITabViewModel>` + `ObservableCollection<NavigationItemViewModel> SidebarItems`; ліниве створення, перевикористання.
- **Документи**: `ObservableCollection<IReportTabViewModel> OpenDocuments` + `SelectedDocument`.
- **Один хост**: властивість `ActiveContent` + `ContentControl Content="{Binding ActiveContent}"` у MainWindow — наявні `Window.DataTemplates` продовжують резолвитись.

Мапінг подій: `TabAdded` → `OpenDocument(...)`; `TabCloseRequested` від документа → видалення + fallback на попередній документ або Dashboard; від singleton-сторінки (до Стадії 4) → навігація на Dashboard без знищення VM. `ITabViewModel` не змінюється; `IsDirty` — лише в `IReportTabViewModel`.

Оркестрація створення вкладок виноситься з `HomePageViewModel` у `Factories\ReportTabFactory.cs` (singleton в DI). Каталог звітів → `ReportCatalogViewModel`.

### Ribbon — ViewModel-driven
Один реюзабельний `Controls\RibbonToolbar` (UserControl), bound до `ObservableCollection<RibbonGroupViewModel>`:
- `BaseTabViewModel.BuildRibbonGroups()` (protected virtual); `BaseReportTabViewModel` додає «Документ»+«Звіт»; конкретні VM — «Таблицю».
- `RibbonActionViewModel { Caption, IconKey, Command, CommandParameter, IsDestructive }`; іконки резолвляться `ResourceKeyToGeometryConverter` з `Assets\Icons.axaml`.
- Обґрунтування: ~15 view з однаковими рядами кнопок; команди вже існують на VM.

### Теми
- `Assets\Colors.axaml`: семантичні браші → `ResourceDictionary.ThemeDictionaries` (Light/Dark). **Palette-ключі `Color` для `FluentTheme.Palettes` лишити плоскими** (StaticResource при парсингу App.axaml), темізувати лише семантичні браші.
- Персист теми/обраних/нещодавніх: новий `Data/UserSettings.json` через `IFileStorageService` (+ const `LocalDataPaths.UserSettings`). НЕ ReportDataConfig.json і НЕ SimpleDataStorage.json.
- Застосування теми — в `App.OnFrameworkInitializationCompleted` до створення MainWindow.

### Глобальний пошук
HeaderBar TextBox + Enter → навігація на History → `ClearFiltersCommand` → `HistoryViewModel.SearchText = query`. Наявний `MatchesSearchText` вже покриває DocumentNumber, AssetNames, SerialNumbers, NomenclatureCodes.

## Конвенції коду (обов'язкові, з CLAUDE.md та домовленостей)

- Жодних літералів (рядки, числа) у тілах методів — виносити в named const поля.
- Без expression-bodied методів; фігурні дужки на кожному if/else/loop; return через локальну змінну, не inline-обчислення.
- MVVM: `Mil.MVVM.Common` (`ObservableItem`, `DelegateCommand`); DI через `ServiceConfigurator`.
- Коміти — лише після підтвердження користувачем.
- Кросплатформність: Windows x64 + macOS arm64, без Windows-only API без `#if WINDOWS`.

## Верифікація (кожна стадія)

1. `dotnet build Mil.Paperwork.WriteOff.sln` — без помилок.
2. `dotnet test Mil.Paperwork.Tests\Mil.Paperwork.Tests.csproj` — Domain-тести не зачіпаються, мають проходити.
3. Skill `ui-verify` — запуск застосунку, скріншоти, перевірка сценаріїв стадії в обох темах (світла/темна).
