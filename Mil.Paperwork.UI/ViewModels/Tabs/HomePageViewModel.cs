using Mil.MVVM.Common;
using Mil.Paperwork.Domain.Services;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.DataAccess.Services;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.UI.Enums;
using Mil.Paperwork.UI.Factories;
using Mil.Paperwork.UI.Managers;
using Mil.Paperwork.UI.ViewModels.Dictionaries;
using Mil.Paperwork.UI.ViewModels.Reports;
using System;
using System.Collections.Generic;

namespace Mil.Paperwork.UI.ViewModels.Tabs
{
    internal class HomePageViewModel : ObservableItem, ITabViewModel
    {
        private readonly ReportManager _reportManager;
        private readonly IAssetFactory _assetFactory;
        private readonly IDataService _dataService;
        private readonly IReportDataService _reportDataService;
        private readonly IReportHistoryService _reportHistoryService;
        private readonly IExportService _exportService;
        private readonly IImportService _importService;
        private readonly IDialogService _dialogService;
        private readonly Dictionary<SettingsTabType, ISettingsTabViewModel> _settingTabViewModels;

        public event EventHandler<ITabViewModel> TabAdded;
        public event EventHandler<ITabViewModel> TabSelectionRequested;
        public event EventHandler<ITabViewModel> TabCloseRequested;

        public string Header => "Головна сторінка";

        public bool IsClosed => false;

        public List<ReportItemViewModel> DocumentTypes { get; private set; }

        public IDelegateCommand<ReportType> CreateReportCommand { get; }

        public IDelegateCommand OpenSettingsCommand { get; }
        public IDelegateCommand OpenProductsDictionaryCommand { get; }
        public IDelegateCommand OpenPeopleDictionaryCommand { get; }
        public IDelegateCommand OpenMeasurementUnitsDictionaryCommand { get; }
        public IDelegateCommand OpenCommissionsConfigurationCommand { get; }
        public IDelegateCommand OpenReportConfigurationCommand { get; }
        public IDelegateCommand OpenServicesDictionaryCommand { get; }

        public IDelegateCommand CloseTabCommand => new DelegateCommand(() => { });

        public HomePageViewModel(
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

            _settingTabViewModels = [];
            DocumentTypes = [.. GetAllReportTypes()];

            CreateReportCommand = new DelegateCommand<ReportType>(OpenNewReportTab);
            OpenSettingsCommand = new DelegateCommand(OpenSettingsExecute);
            OpenProductsDictionaryCommand = new DelegateCommand(OpenProductsDictionaryCommandExecute);
            OpenPeopleDictionaryCommand = new DelegateCommand(OpenPeopleDictionaryCommandExecute);
            OpenMeasurementUnitsDictionaryCommand = new DelegateCommand(OpenMeasurementUnitsDictionaryCommandExecute);
            OpenCommissionsConfigurationCommand = new DelegateCommand(OpenCommissionsConfigurationCommandExecute);
            OpenReportConfigurationCommand = new DelegateCommand(OpenReportConfigurationCommandExecute);
            OpenServicesDictionaryCommand = new DelegateCommand(OpenServicesDictionaryCommandExecute);
        }

        private static IList<ReportItemViewModel> GetAllReportTypes()
        {
            var reportTypes = new List<ReportItemViewModel>()
            {
                new(ReportType.WriteOffOrder),
                new(ReportType.ResidualValueReport),
                new(ReportType.WriteOffPackage),
                new(ReportType.AssetValuationReport),
                new(ReportType.AssetDismantlingReport),
                new(ReportType.TechnicalStateReport),
                new(ReportType.CommissioningAct),
                new(ReportType.Invoice),
                new(ReportType.Handover23Act),
            };

            return reportTypes;
        }

        private async void OpenNewReportTab(ReportType documentType)
        {
            IReportTabViewModel? createdTab;
            switch (documentType)
            {
                case ReportType.ResidualValueReport:
                    createdTab = new ResidualValueReportViewModel(_reportManager, _assetFactory, _dataService, _reportDataService, _reportHistoryService, _dialogService);
                    break;
                case ReportType.AssetValuationReport:
                    createdTab = new AssetValuationViewModel(_reportManager, _dataService, _importService, _reportHistoryService, _dialogService);
                    break;
                case ReportType.AssetDismantlingReport:
                    createdTab = new AssetDismantlingViewModel(_reportManager, _dataService, _importService, _reportHistoryService, _dialogService);
                    break;
                case ReportType.TechnicalStateReport:
                    createdTab = new AssetInitialTechnicalStateViewModel(_reportManager, _assetFactory, _dataService, _reportDataService, _reportHistoryService, _dialogService);
                    break;
                case ReportType.WriteOffPackage:
                    createdTab = new AssetTechnicalStateViewModel(_reportManager, _assetFactory, _dataService, _reportDataService, _reportHistoryService, _dialogService);
                    break;
                case ReportType.Invoice:
                    createdTab = new InvoiceReportViewModel(_reportManager, _dataService, _reportHistoryService, _dialogService);
                    break;
                case ReportType.CommissioningAct:
                    createdTab = new CommissioningActReportViewModel(_reportManager, _dataService, _reportDataService, _reportHistoryService, _dialogService);
                    break;
                case ReportType.Handover23Act:
                    createdTab = new Handover23ActViewModel(_reportManager, _dataService, _reportHistoryService, _dialogService);
                    break;
                case ReportType.WriteOffOrder:
                    createdTab = new WriteOffOrderViewModel(_reportManager, _dataService, _reportDataService, _reportHistoryService, _dialogService);
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

        private void OpenCommissionsConfigurationCommandExecute()
        {
            OpenSettingsTab(SettingsTabType.CommissionsConfiguration);
        }

        private void OpenReportConfigurationCommandExecute()
        {
            OpenSettingsTab(SettingsTabType.ReportsConfiguration);
        }

        private void OpenServicesDictionaryCommandExecute()
        {
            OpenSettingsTab(SettingsTabType.ServicesDictionary);
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
                    SettingsTabType.ReportsConfiguration => new ReportConfigViewModel(_reportDataService, _exportService, _importService, _dialogService),
                    SettingsTabType.CommissionsConfiguration => new CommissionsConfigViewModel(_reportDataService, _exportService, _importService, _dialogService),
                    SettingsTabType.ProductDictionary => new ProductsDictionaryViewModel(_dataService, _exportService, _importService, _dialogService),
                    SettingsTabType.PeopleDictionary => new PeopleDictionaryViewModel(_dataService, _importService, _dialogService),
                    SettingsTabType.MeasurementUnitsDictionary => new MeasurementUnitsDictionaryViewModel(_dataService, _dialogService),
                    SettingsTabType.ServicesDictionary => new ServicesDictionaryViewModel(_reportDataService, _dataService, _dialogService),
                    _ => throw new NotImplementedException()
                };

                _settingTabViewModels[settingsTabType] = tabViewModel;
                TabAdded?.Invoke(this, tabViewModel);
            }
        }
    }
}
