using Mil.Paperwork.Infrastructure.MVVM;
using Mil.Paperwork.Domain.Services;
using Mil.Paperwork.WriteOff.Managers;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.WriteOff.Enums;
using Mil.Paperwork.WriteOff.Factories;
using System.Windows.Input;
using Mil.Paperwork.WriteOff.ViewModels.Reports;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.WriteOff.ViewModels.Dictionaries;

namespace Mil.Paperwork.WriteOff.ViewModels.Tabs
{
    internal class HomePageViewModel : ObservableItem, ITabViewModel
    {
        private readonly ReportManager _reportManager;
        private readonly IAssetFactory _assetFactory;
        private readonly IDataService _dataService;
        private readonly IReportDataService _reportDataService;
        private readonly IExportService _exportService;
        private readonly INavigationService _navigationService;

        private readonly Dictionary<SettingsTabType, ISettingsTabViewModel> _settingTabViewModels;

        public event EventHandler<ITabViewModel> TabAdded;
        public event EventHandler<ITabViewModel> TabSelectionRequested;
        public event EventHandler<ITabViewModel> TabCloseRequested;

        public string Header => "HOME PAGE";

        public bool IsClosed => false;

        public List<ReportItemViewModel> DocumentTypes { get; private set; }

        public ICommand<DocumentTypeEnum> CreateReportCommand { get; }

        public ICommand OpenSettingsCommand { get; }
        public ICommand CheckSumCommand { get; }
        public ICommand OpenProductsDictionaryCommand { get; }
        public ICommand OpenPeopleDictionaryCommand { get; }
        public ICommand OpenMeasurementUnitsDictionaryCommand { get; }
        public ICommand OpenReportConfigurationCommand { get; }

        public HomePageViewModel(
            ReportManager reportManager,
            IAssetFactory assetFactory,
            IDataService dataService,
            IReportDataService reportDataService,
            IExportService exportService,
            INavigationService navigationService)
        {
            _reportManager = reportManager;
            _assetFactory = assetFactory;
            _dataService = dataService;
            _reportDataService = reportDataService;
            _exportService = exportService;
            _navigationService = navigationService;

            _settingTabViewModels = [];
            DocumentTypes = [.. GetAllReportTypes()];

            CreateReportCommand = new DelegateCommand<DocumentTypeEnum>(OpenNewReportTab);
            OpenSettingsCommand = new DelegateCommand(OpenSettingsExecute);
            CheckSumCommand = new DelegateCommand(CheckSumCommandExecute);
            OpenProductsDictionaryCommand = new DelegateCommand(OpenProductsDictionaryCommandExecute);
            OpenPeopleDictionaryCommand = new DelegateCommand(OpenPeopleDictionaryCommandExecute);
            OpenMeasurementUnitsDictionaryCommand = new DelegateCommand(OpenMeasurementUnitsDictionaryCommandExecute);
            OpenReportConfigurationCommand = new DelegateCommand(OpenReportConfigurationCommandExecute);
        }

        private static IList<ReportItemViewModel> GetAllReportTypes()
        {
            var reportTypes = new List<ReportItemViewModel>()
            {
                //new(DocumentTypeEnum.WriteOff),
                new(DocumentTypeEnum.ResidualValue),
                new(DocumentTypeEnum.TechnicalState11),
                new(DocumentTypeEnum.TechnicalState7),
                new(DocumentTypeEnum.Valuation),
                new(DocumentTypeEnum.Dismantling),
                new(DocumentTypeEnum.Invoice),
                new(DocumentTypeEnum.CommisioningAct),
            };

            return reportTypes;
        }

        private void OpenNewReportTab(DocumentTypeEnum documentType)
        {
            IReportTabViewModel? createdTab;
            switch (documentType)
            {
                case DocumentTypeEnum.WriteOff:
                    createdTab = new WriteOffReportViewModel(_reportManager, _assetFactory, _dataService, _reportDataService, _navigationService);
                    break;
                case DocumentTypeEnum.ResidualValue:
                    createdTab = new ResidualValueReportViewModel(_reportManager, _assetFactory, _dataService, _reportDataService);
                    break;
                case DocumentTypeEnum.Valuation:
                    createdTab = new AssetValuationViewModel(_reportManager, _dataService, _navigationService);
                    break;
                case DocumentTypeEnum.Dismantling:
                    createdTab = new AssetDismantlingViewModel(_reportManager, _dataService, _navigationService);
                    break;
                case DocumentTypeEnum.TechnicalState7:
                    createdTab = new AssetInitialTechnicalStateViewModel(_reportManager, _assetFactory, _dataService);
                    break;
                case DocumentTypeEnum.TechnicalState11:
                    createdTab = new AssetTechnicalStateViewModel(_reportManager, _assetFactory, _dataService);
                    break;
                case DocumentTypeEnum.Invoice:
                    createdTab = new InvoiceReportViewModel(_reportManager, _dataService);
                    break;
                case DocumentTypeEnum.CommisioningAct:
                    createdTab = new CommissioningActReportViewModel(_reportManager, _dataService, _reportDataService);
                    break;
                default:
                    createdTab = null;
                    break;

            }

            if (createdTab != null)
            {
                createdTab.OpenReportSettingsRequested += OnOpenReportSettingsRequested;

                TabAdded?.Invoke(this, createdTab);
            }
        }

        private void OnOpenReportSettingsRequested(object? sender, ReportType reportType)
        {
            OpenSettingsTab(SettingsTabType.ReportsConfiguration);
            if (_settingTabViewModels.TryGetValue(SettingsTabType.ReportsConfiguration, out var tabViewModel))
            {
                if (tabViewModel is ReportConfigViewModel reportConfigViewModel)
                {
                    reportConfigViewModel.SelectReportType(reportType);
                }
            }
        }

        private void OpenSettingsExecute()
        {
            OpenSettingsTab(SettingsTabType.Settings);
        }

        private void CheckSumCommandExecute()
        {
            //OpenSettingsTab(SettingsTabType.CheckSum);
        }

        private void OpenProductsDictionaryCommandExecute()
        {
            OpenSettingsTab(SettingsTabType.ProductDictionary);
        }

        private void OpenPeopleDictionaryCommandExecute()
        {
            OpenSettingsTab(SettingsTabType.PeopleDictionary);
        }

        private void OpenMeasurementUnitsDictionaryCommandExecute()
        {
            OpenSettingsTab(SettingsTabType.MeasurementUnitsDictionary);
        }

        private void OpenReportConfigurationCommandExecute()
        {
            OpenSettingsTab(SettingsTabType.ReportsConfiguration);
        }

        private void OpenSettingsTab(SettingsTabType settingsTabType)
        {
            _settingTabViewModels.TryGetValue(settingsTabType, out var tabViewModel);

            if (tabViewModel?.IsClosed == false)
            {
                TabSelectionRequested?.Invoke(this, tabViewModel);
            }
            else
            {
                tabViewModel = settingsTabType switch
                {
                    SettingsTabType.Settings => new SettingsViewModel(_reportDataService),
                    SettingsTabType.ReportsConfiguration => new ReportConfigViewModel(_reportDataService, _exportService),
                    SettingsTabType.ProductDictionary => new ProductsDictionaryViewModel(_dataService, _exportService, _navigationService),
                    SettingsTabType.PeopleDictionary => new PeopleDictionaryViewModel(_dataService, _navigationService),
                    SettingsTabType.MeasurementUnitsDictionary => new MeasurementUnitsDictionaryViewModel(_dataService),
                    _ => throw new NotImplementedException()
                };

                _settingTabViewModels[settingsTabType] = tabViewModel;
                TabAdded?.Invoke(this, tabViewModel);
            }
        }
    }
}
