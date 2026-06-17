using Avalonia;
using Avalonia.Styling;
using Mil.MVVM.Common;
using Mil.Paperwork.DataAccess.Conversions;
using Mil.Paperwork.DataAccess.Repositories;
using Mil.Paperwork.DataAccess.Services;
using Mil.Paperwork.Domain.Services;
using Mil.Paperwork.Infrastructure.DataModels;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.UI.Enums;
using Mil.Paperwork.UI.Factories;
using Mil.Paperwork.UI.Helpers;
using Mil.Paperwork.UI.Services;
using Mil.Paperwork.UI.ViewModels.Dictionaries;
using Mil.Paperwork.UI.ViewModels.History;
using Mil.Paperwork.UI.ViewModels.Shell;
using Mil.Paperwork.UI.ViewModels.Tabs;
using Mil.Paperwork.UI.ViewModels.Dashboard;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Mil.Paperwork.UI.ViewModels
{
    internal class MainWindowViewModel : ObservableItem
    {
        private const string NavTitleDashboard = "Головна";
        private const string NavTitleHistory = "Історія";
        private const string NavTitleProducts = "Майно";
        private const string NavTitlePeople = "Особи";
        private const string NavTitleServices = "Служби";
        private const string NavTitleMeasurementUnits = "Од. виміру";
        private const string NavTitleSettings = "Налаштування";

        private const string ThemeTooltipAuto = "Тема: автоматична (за системою)";
        private const string ThemeTooltipLight = "Тема: світла";
        private const string ThemeTooltipDark = "Тема: темна";

        private readonly ReportTabFactory _reportTabFactory;
        private readonly IUserSettingsService _userSettingsService;
        private readonly IDataService _dataService;
        private readonly IReportDataService _reportDataService;
        private readonly IReportHistoryRepository _reportHistoryRepository;
        private readonly ReportConversionRegistry _conversionRegistry;
        private readonly IExportService _exportService;
        private readonly IImportService _importService;
        private readonly IDialogService _dialogService;

        private readonly Dictionary<NavigationPageType, ITabViewModel> _pages = [];

        private object? _activeContent;
        private IReportTabViewModel? _selectedDocument;
        private bool _isSidebarCollapsed;
        private ThemeMode _themeMode;

        public ObservableCollection<NavigationItemViewModel> SidebarMainItems { get; } = [];
        public ObservableCollection<NavigationItemViewModel> SidebarDictionaryItems { get; } = [];
        public ObservableCollection<NavigationItemViewModel> SidebarFooterItems { get; } = [];

        public ObservableCollection<IReportTabViewModel> OpenDocuments { get; } = [];

        public object? ActiveContent
        {
            get => _activeContent;
            private set => SetProperty(ref _activeContent, value);
        }

        public IReportTabViewModel? SelectedDocument
        {
            get => _selectedDocument;
            set
            {
                if (SetProperty(ref _selectedDocument, value) && value != null)
                {
                    ActivateDocument(value);
                }
            }
        }

        public bool HasOpenDocuments => OpenDocuments.Count > 0;

        public bool IsSidebarCollapsed
        {
            get => _isSidebarCollapsed;
            set => SetProperty(ref _isSidebarCollapsed, value);
        }

        public ThemeMode ThemeMode
        {
            get => _themeMode;
            set
            {
                if (SetProperty(ref _themeMode, value))
                {
                    ApplyTheme(value);
                    OnPropertyChanged(nameof(ThemeIconKey));
                    OnPropertyChanged(nameof(ThemeTooltip));
                }
            }
        }

        public string ThemeIconKey
        {
            get
            {
                var iconKey = _themeMode switch
                {
                    ThemeMode.Light => IconKeys.ThemeLight,
                    ThemeMode.Dark => IconKeys.ThemeDark,
                    _ => IconKeys.ThemeAuto
                };

                return iconKey;
            }
        }

        public string ThemeTooltip
        {
            get
            {
                var tooltip = _themeMode switch
                {
                    ThemeMode.Light => ThemeTooltipLight,
                    ThemeMode.Dark => ThemeTooltipDark,
                    _ => ThemeTooltipAuto
                };

                return tooltip;
            }
        }

        private const int RecentReportTypesLimit = 5;

        public IDelegateCommand<NavigationItemViewModel> NavigateCommand { get; }
        public ICommand NextTabCommand { get; }
        public ICommand PreviousTabCommand { get; }
        public ICommand CloseActiveDocumentCommand { get; }
        public ICommand ToggleSidebarCommand { get; }
        public ICommand ToggleThemeCommand { get; }

        public MainWindowViewModel(
            ReportTabFactory reportTabFactory,
            IUserSettingsService userSettingsService,
            IDataService dataService,
            IReportDataService reportDataService,
            IReportHistoryRepository reportHistoryRepository,
            ReportConversionRegistry conversionRegistry,
            IExportService exportService,
            IImportService importService,
            IDialogService dialogService)
        {
            _reportTabFactory = reportTabFactory;
            _userSettingsService = userSettingsService;
            _dataService = dataService;
            _reportDataService = reportDataService;
            _reportHistoryRepository = reportHistoryRepository;
            _conversionRegistry = conversionRegistry;
            _exportService = exportService;
            _importService = importService;
            _dialogService = dialogService;

            _reportTabFactory.OpenReportSettingsRequested += OnOpenReportSettingsRequested;

            NavigateCommand = new DelegateCommand<NavigationItemViewModel>(NavigateCommandExecute);
            NextTabCommand = new DelegateCommand(MoveToNextTabCommandExecute);
            PreviousTabCommand = new DelegateCommand(MoveToPrevTabCommandExecute);
            CloseActiveDocumentCommand = new DelegateCommand(CloseActiveDocumentCommandExecute);
            ToggleSidebarCommand = new DelegateCommand(ToggleSidebarCommandExecute);
            ToggleThemeCommand = new DelegateCommand(ToggleThemeCommandExecute);

            OpenDocuments.CollectionChanged += OnOpenDocumentsCollectionChanged;

            _themeMode = GetStoredThemeMode();
            _isSidebarCollapsed = GetStoredIsSidebarCollapsed();

            FillSidebarItems();
            NavigateTo(NavigationPageType.Dashboard);
        }

        private void FillSidebarItems()
        {
            SidebarMainItems.Add(new NavigationItemViewModel(NavTitleDashboard, IconKeys.Dashboard, NavigationPageType.Dashboard));
            SidebarMainItems.Add(new NavigationItemViewModel(NavTitleHistory, IconKeys.History, NavigationPageType.History));

            SidebarDictionaryItems.Add(new NavigationItemViewModel(NavTitleProducts, IconKeys.Products, NavigationPageType.ProductsDictionary));
            SidebarDictionaryItems.Add(new NavigationItemViewModel(NavTitlePeople, IconKeys.People, NavigationPageType.PeopleDictionary));
            SidebarDictionaryItems.Add(new NavigationItemViewModel(NavTitleServices, IconKeys.Services, NavigationPageType.ServicesDictionary));
            SidebarDictionaryItems.Add(new NavigationItemViewModel(NavTitleMeasurementUnits, IconKeys.MeasurementUnits, NavigationPageType.MeasurementUnitsDictionary));

            SidebarFooterItems.Add(new NavigationItemViewModel(NavTitleSettings, IconKeys.Settings, NavigationPageType.Settings));
        }

        private ThemeMode GetStoredThemeMode()
        {
            var settings = _userSettingsService.GetSettings();

            var themeMode = settings.Theme switch
            {
                UserSettingsDTO.ThemeLight => ThemeMode.Light,
                UserSettingsDTO.ThemeDark => ThemeMode.Dark,
                _ => ThemeMode.Auto
            };

            return themeMode;
        }

        private bool GetStoredIsSidebarCollapsed()
        {
            var settings = _userSettingsService.GetSettings();
            return settings.IsSidebarCollapsed;
        }

        private void SaveSidebarCollapsed()
        {
            var settings = _userSettingsService.GetSettings();
            settings.IsSidebarCollapsed = _isSidebarCollapsed;
            _userSettingsService.SaveSettings(settings);
        }

        private void ApplyTheme(ThemeMode themeMode)
        {
            var application = Application.Current;
            if (application != null)
            {
                application.RequestedThemeVariant = themeMode switch
                {
                    ThemeMode.Light => ThemeVariant.Light,
                    ThemeMode.Dark => ThemeVariant.Dark,
                    _ => ThemeVariant.Default
                };
            }

            var settings = _userSettingsService.GetSettings();
            settings.Theme = themeMode switch
            {
                ThemeMode.Light => UserSettingsDTO.ThemeLight,
                ThemeMode.Dark => UserSettingsDTO.ThemeDark,
                _ => UserSettingsDTO.ThemeAuto
            };
            _userSettingsService.SaveSettings(settings);
        }

        private void ToggleThemeCommandExecute()
        {
            var nextMode = _themeMode switch
            {
                ThemeMode.Auto => ThemeMode.Light,
                ThemeMode.Light => ThemeMode.Dark,
                _ => ThemeMode.Auto
            };

            ThemeMode = nextMode;
        }

        private void NavigateCommandExecute(NavigationItemViewModel item)
        {
            if (item != null)
            {
                NavigateTo(item.PageType);
            }
        }

        private void NavigateTo(NavigationPageType pageType)
        {
            var page = GetOrCreatePage(pageType);
            if (page == null)
            {
                return;
            }

            if (page is HistoryViewModel historyViewModel)
            {
                historyViewModel.Refresh();
            }

            if (page is DashboardViewModel dashboardViewModel)
            {
                dashboardViewModel.Refresh();
            }

            if (page is ISilentRefreshable refreshable && !page.IsDirty)
            {
                refreshable.SilentRefresh();
            }

            SelectedDocument = null;
            ActiveContent = page;
            UpdateNavigationSelection(pageType);
        }

        private void UpdateNavigationSelection(NavigationPageType? pageType)
        {
            UpdateNavigationSelection(SidebarMainItems, pageType);
            UpdateNavigationSelection(SidebarDictionaryItems, pageType);
            UpdateNavigationSelection(SidebarFooterItems, pageType);
        }

        private static void UpdateNavigationSelection(ObservableCollection<NavigationItemViewModel> items, NavigationPageType? pageType)
        {
            foreach (var item in items)
            {
                item.IsSelected = item.PageType == pageType;
            }
        }

        private ITabViewModel? GetOrCreatePage(NavigationPageType pageType)
        {
            if (_pages.TryGetValue(pageType, out var existingPage))
            {
                return existingPage;
            }

            var page = CreatePage(pageType);
            if (page != null)
            {
                page.TabCloseRequested += OnPageCloseRequested;
                _pages[pageType] = page;
            }

            return page;
        }

        private ITabViewModel? CreatePage(NavigationPageType pageType)
        {
            ITabViewModel? page;
            switch (pageType)
            {
                case NavigationPageType.Dashboard:
                    page = CreateDashboardPage();
                    break;
                case NavigationPageType.ReportCatalog:
                    page = CreateReportCatalogPage();
                    break;
                case NavigationPageType.History:
                    page = CreateHistoryPage();
                    break;
                case NavigationPageType.ProductsDictionary:
                    page = new ProductsDictionaryViewModel(_dataService, _exportService, _importService, _dialogService);
                    break;
                case NavigationPageType.PeopleDictionary:
                    page = new PeopleDictionaryViewModel(_dataService, _importService, _dialogService);
                    break;
                case NavigationPageType.ServicesDictionary:
                    page = new ServicesDictionaryViewModel(_reportDataService, _dataService, _dialogService);
                    break;
                case NavigationPageType.MeasurementUnitsDictionary:
                    page = new MeasurementUnitsDictionaryViewModel(_dataService, _dialogService);
                    break;
                case NavigationPageType.ReportsConfiguration:
                case NavigationPageType.CommissionsConfiguration:
                    page = null;
                    break;
                case NavigationPageType.Settings:
                    page = new SettingsHubViewModel(_reportDataService, _exportService, _importService, _dialogService);
                    break;
                default:
                    page = null;
                    break;
            }

            return page;
        }

        private DashboardViewModel CreateDashboardPage()
        {
            var dashboardViewModel = new DashboardViewModel(_reportHistoryRepository, _userSettingsService, _dialogService);
            dashboardViewModel.ReportCreationRequested += OnReportCreationRequested;
            dashboardViewModel.OpenHistoryEntryRequested += OnOpenHistoryEntryRequested;
            dashboardViewModel.NavigateToHistoryRequested += OnDashboardNavigateToHistory;
            dashboardViewModel.NavigateToReportCatalogRequested += OnDashboardNavigateToReportCatalog;
            return dashboardViewModel;
        }

        private ReportCatalogViewModel CreateReportCatalogPage()
        {
            var catalogViewModel = new ReportCatalogViewModel();
            catalogViewModel.ReportCreationRequested += OnReportCreationRequested;
            return catalogViewModel;
        }

        private HistoryViewModel CreateHistoryPage()
        {
            var historyViewModel = new HistoryViewModel(_reportHistoryRepository, _conversionRegistry, _dialogService);
            historyViewModel.OpenHistoryEntryRequested += OnOpenHistoryEntryRequested;
            historyViewModel.CreateFromRequested += OnCreateFromEntryRequested;
            return historyViewModel;
        }

        private void OnReportCreationRequested(object? sender, ReportType reportType)
        {
            var createdTab = _reportTabFactory.Create(reportType);
            if (createdTab != null)
            {
                OpenDocument(createdTab);
                TrackRecentReportType(reportType);
            }
        }

        private void OnDashboardNavigateToHistory(object? sender, EventArgs e)
        {
            NavigateTo(NavigationPageType.History);
        }

        private void OnDashboardNavigateToReportCatalog(object? sender, EventArgs e)
        {
            NavigateTo(NavigationPageType.ReportCatalog);
        }

        public void ExecuteGlobalSearch(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return;
            }

            NavigateTo(NavigationPageType.History);

            if (_pages.TryGetValue(NavigationPageType.History, out var page)
                && page is HistoryViewModel historyViewModel)
            {
                historyViewModel.ClearFiltersCommand.Execute(null);
                historyViewModel.SearchText = query;
            }
        }

        private void TrackRecentReportType(ReportType reportType)
        {
            var settings = _userSettingsService.GetSettings();
            var recent = settings.RecentReportTypes;
            recent.Remove(reportType);
            recent.Insert(0, reportType);
            if (recent.Count > RecentReportTypesLimit)
            {
                recent.RemoveRange(RecentReportTypesLimit, recent.Count - RecentReportTypesLimit);
            }

            _userSettingsService.SaveSettings(settings);
        }

        private void OnOpenHistoryEntryRequested(object? sender, Guid entryId)
        {
            var createdTab = _reportTabFactory.CreateFromHistoryEntry(entryId);
            if (createdTab != null)
            {
                OpenDocument(createdTab);
            }
        }

        private void OnCreateFromEntryRequested(object? sender, CreateFromRequestedEventArgs args)
        {
            var createdTabs = _reportTabFactory.CreateFromConversion(args.EntryId, args.TargetType);
            foreach (var createdTab in createdTabs)
            {
                OpenDocument(createdTab);
            }
        }

        private void OnOpenReportSettingsRequested(object? sender, ReportType reportType)
        {
            NavigateTo(NavigationPageType.Settings);

            if (_pages.TryGetValue(NavigationPageType.Settings, out var page)
                && page is SettingsHubViewModel settingsHub)
            {
                settingsHub.SelectReportType(reportType);
            }
        }

        private void OpenDocument(IReportTabViewModel document)
        {
            document.TabCloseRequested += OnDocumentCloseRequested;
            OpenDocuments.Add(document);
            SelectedDocument = document;
        }

        private void ActivateDocument(IReportTabViewModel document)
        {
            ActiveContent = document;
            UpdateNavigationSelection(null);
        }

        private void OnPageCloseRequested(object? sender, ITabViewModel tabViewModel)
        {
            NavigateTo(NavigationPageType.Dashboard);
        }

        private void OnDocumentCloseRequested(object? sender, ITabViewModel tabViewModel)
        {
            if (tabViewModel is not IReportTabViewModel document)
            {
                return;
            }

            var documentIndex = OpenDocuments.IndexOf(document);
            if (documentIndex < 0)
            {
                return;
            }

            var wasActive = SelectedDocument == document;

            document.TabCloseRequested -= OnDocumentCloseRequested;
            OpenDocuments.Remove(document);

            if (wasActive)
            {
                if (OpenDocuments.Count > 0)
                {
                    var fallbackIndex = documentIndex > 0 ? documentIndex - 1 : 0;
                    SelectedDocument = OpenDocuments[fallbackIndex];
                }
                else
                {
                    NavigateTo(NavigationPageType.Dashboard);
                }
            }
        }

        private void OnOpenDocumentsCollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(HasOpenDocuments));
        }

        private void MoveToNextTabCommandExecute()
        {
            if (OpenDocuments.Count == 0)
            {
                return;
            }

            var currentIndex = GetSelectedDocumentIndex();
            var nextIndex = (currentIndex + 1) % OpenDocuments.Count;
            SelectedDocument = OpenDocuments[nextIndex];
        }

        private void MoveToPrevTabCommandExecute()
        {
            if (OpenDocuments.Count == 0)
            {
                return;
            }

            var currentIndex = GetSelectedDocumentIndex();
            var prevIndex = (currentIndex + OpenDocuments.Count - 1) % OpenDocuments.Count;
            SelectedDocument = OpenDocuments[prevIndex];
        }

        private int GetSelectedDocumentIndex()
        {
            var index = SelectedDocument != null ? OpenDocuments.IndexOf(SelectedDocument) : 0;
            if (index < 0)
            {
                index = 0;
            }

            return index;
        }

        private void CloseActiveDocumentCommandExecute()
        {
            SelectedDocument?.CloseTabCommand.Execute(null);
        }

        private void ToggleSidebarCommandExecute()
        {
            IsSidebarCollapsed = !IsSidebarCollapsed;
            SaveSidebarCollapsed();
        }
    }
}
