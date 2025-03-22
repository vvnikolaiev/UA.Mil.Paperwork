using Mil.Paperwork.Infrastructure.MVVM;
using Mil.Paperwork.Domain.Services;
using Mil.Paperwork.WriteOff.Managers;
using Mil.Paperwork.Infrastructure.Services;
using System.Collections.ObjectModel;

namespace Mil.Paperwork.WriteOff.ViewModels
{
    public class MainViewModel : ObservableItem
    {
        private readonly ReportManager _reportManager;
        private readonly IDataService _dataService;
        private readonly INavigationService _navigationService;

        private ITabViewModel? _selectedTab;

        public ITabViewModel? SelectedTab
        {
            get => _selectedTab; 
            set => SetProperty(ref _selectedTab, value);
        }
        public ObservableCollection<ITabViewModel> Tabs { get; set; } = [];

        public MainViewModel(ReportManager reportManager, IDataService dataService, INavigationService navigationService)
        {
            _reportManager = reportManager;
            _dataService = dataService;
            _navigationService = navigationService;

            AddHomeTab();
        }

        private void AddHomeTab()
        {
            var homePageVM = new HomePageViewModel(_reportManager, _dataService, _navigationService);
            homePageVM.TabAdded += OnNewTabAdded;

            AddNeTab(homePageVM);
        }

        private void OnNewTabAdded(object? sender, ITabViewModel tabViewModel)
        {
            AddNeTab(tabViewModel);
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
