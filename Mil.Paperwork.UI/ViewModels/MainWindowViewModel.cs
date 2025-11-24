using Mil.Paperwork.Common.MVVM;
using Mil.Paperwork.Domain.Services;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.UI.Factories;
using Mil.Paperwork.UI.Managers;
using Mil.Paperwork.UI.ViewModels.Tabs;
using System.Collections.ObjectModel;
using System.Linq;

namespace Mil.Paperwork.UI.ViewModels
{
    public partial class MainWindowViewModel : ObservableItem
    {
        private readonly ReportManager _reportManager;
        private readonly IAssetFactory _assetFactory;
        private readonly IDataService _dataService;
        private readonly IReportDataService _reportDataService;
        private readonly IExportService _exportService;
        private readonly IImportService _importService;
        private readonly IDialogService _dialogService;
        private ITabViewModel? _selectedTab;

        public ITabViewModel? SelectedTab
        {
            get => _selectedTab;
            set => SetProperty(ref _selectedTab, value);
        }
        public ObservableCollection<ITabViewModel> Tabs { get; set; } = [];

        public MainWindowViewModel(
            ReportManager reportManager,
            IAssetFactory assetFactory,
            IDataService dataService,
            IReportDataService reportDataService,
            IExportService exportService,
            IImportService importService,
            IDialogService dialogService)
        {
            _reportManager = reportManager;
            _assetFactory = assetFactory;
            _dataService = dataService;
            _reportDataService = reportDataService;
            _exportService = exportService;
            _importService = importService;
            _dialogService = dialogService;

            AddHomeTab();
        }

        private void AddHomeTab()
        {
            var homePageVM = new HomePageViewModel(_reportManager, _assetFactory, _dataService, _reportDataService, _exportService, _importService, _dialogService);
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
