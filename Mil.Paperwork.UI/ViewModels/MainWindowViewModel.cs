using Mil.MVVM.Common;
using Mil.Paperwork.Domain.Services;
using Mil.Paperwork.DataAccess.Services;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.UI.Factories;
using Mil.Paperwork.UI.Managers;
using Mil.Paperwork.UI.ViewModels.Tabs;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Mil.Paperwork.UI.ViewModels
{
    public partial class MainWindowViewModel : ObservableItem
    {
        private readonly ReportManager _reportManager;
        private readonly IAssetFactory _assetFactory;
        private readonly IDataService _dataService;
        private readonly IReportDataService _reportDataService;
        private readonly IReportHistoryService _reportHistoryService;
        private readonly IExportService _exportService;
        private readonly IImportService _importService;
        private readonly IDialogService _dialogService;
        private ITabViewModel? _selectedTab;
        private int _selectedTabIndex;

        public ITabViewModel? SelectedTab
        {
            get => _selectedTab;
            set => SetProperty(ref _selectedTab, value);
        }

        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set => SetProperty(ref _selectedTabIndex, value);
        }

        public ObservableCollection<ITabViewModel> Tabs { get; set; } = [];

        public ICommand NextTabCommand { get; }
        public ICommand PreviousTabCommand { get; }


        public MainWindowViewModel(
            ReportManager reportManager,
            IAssetFactory assetFactory,
            IDataService dataService,
            IReportDataService reportDataService,
            IReportHistoryService reportHistoryService,
            IExportService exportService,
            IImportService importService,
            IDialogService dialogService)
        {
            _reportManager = reportManager;
            _assetFactory = assetFactory;
            _dataService = dataService;
            _reportDataService = reportDataService;
            _reportHistoryService = reportHistoryService;
            _exportService = exportService;
            _importService = importService;
            _dialogService = dialogService;

            NextTabCommand = new DelegateCommand(MoveToNextTabCommandExecute);
            PreviousTabCommand = new DelegateCommand(MoveToPrevTabCommandExecute);

            AddHomeTab();
        }

        private void MoveToNextTabCommandExecute()
        {
            SelectedTabIndex = (SelectedTabIndex + 1) % Tabs.Count;
        }

        private void MoveToPrevTabCommandExecute()
        {
            SelectedTabIndex = (SelectedTabIndex + Tabs.Count - 1) % Tabs.Count;
        }

        private void AddHomeTab()
        {
            var homePageVM = new HomePageViewModel(_reportManager, _assetFactory, _dataService, _reportDataService, _reportHistoryService, _exportService, _importService, _dialogService);
            homePageVM.TabAdded += OnNewTabAdded;
            homePageVM.TabSelectionRequested += OnTabSelectRequested;

            AddNeTab(homePageVM);
        }

        private void OnNewTabAdded(object? sender, ITabViewModel tabViewModel)
        {
            AddNeTab(tabViewModel);
        }

        private void OnTabSelectRequested(object? sender, ITabViewModel tabViewModel)
        {
            if (tabViewModel != null)
            {
                SelectedTab = tabViewModel;
            }
        }

        private void OnTabCloseRequested(object? sender, ITabViewModel tabViewModel)
        {
            CloseTab(tabViewModel);
        }

        private void AddNeTab(ITabViewModel tabViewModel)
        {
            if (tabViewModel != null)
            {
                Tabs.Add(tabViewModel);
                SelectedTab = tabViewModel;
                tabViewModel.TabCloseRequested += OnTabCloseRequested;
            }
        }

        private void CloseTab(ITabViewModel tabViewModel)
        {
            if (tabViewModel != null && Tabs != null)
            {
                Tabs.Remove(tabViewModel);
                tabViewModel.TabCloseRequested -= OnTabCloseRequested;

                if (SelectedTab == tabViewModel)
                {
                    SelectedTab = Tabs.FirstOrDefault();
                }

                // TODO: dispose?
                //tabViewModel.Dispose();
            }
        }
    }
}
