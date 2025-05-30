using Mil.Paperwork.Infrastructure.MVVM;
using Mil.Paperwork.Domain.Services;
using Mil.Paperwork.WriteOff.Managers;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.WriteOff.Enums;
using Mil.Paperwork.WriteOff.Factories;
using System.Windows.Input;
using Mil.Paperwork.WriteOff.ViewModels.Reports;

namespace Mil.Paperwork.WriteOff.ViewModels
{
    internal class HomePageViewModel : ObservableItem, ITabViewModel
    {
        private readonly ReportManager _reportManager;
        private readonly IAssetFactory _assetFactory;
        private readonly IDataService _dataService;
        private readonly IReportDataService _reportDataService;
        private readonly IExportService _exportService;
        private readonly INavigationService _navigationService;
        private ITabViewModel _settingsViewModel;
        private ITabViewModel _productsDictionaryViewModel;
        private ITabViewModel _reportConfigViewModel;

        public event EventHandler<ITabViewModel> TabAdded;
        public event EventHandler<ITabViewModel> TabSelectionRequested;
        public event EventHandler<ITabViewModel> TabCloseRequested;

        public string Header => "HOME PAGE";

        public bool IsClosed => false;

        public List<ReportItemViewModel> DocumentTypes { get; private set; }

        public ICommand<DocumentTypeEnum> CreateReportCommand { get; }

        public ICommand OpenSettingsCommand { get; }
        public ICommand OpenProductsDictionaryCommand { get; }
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

            DocumentTypes = [.. GetAllReportTypes()];

            CreateReportCommand = new DelegateCommand<DocumentTypeEnum>(AddWriteOffReport);
            OpenSettingsCommand = new DelegateCommand(OpenSettingsExecute);
            OpenProductsDictionaryCommand = new DelegateCommand(OpenProductsDictionaryCommandExecute);
            OpenReportConfigurationCommand = new DelegateCommand(OpenReportConfigurationCommandExecute);
        }

        private IList<ReportItemViewModel> GetAllReportTypes()
        {
            var reportTypes = new List<ReportItemViewModel>()
            {
                new ReportItemViewModel("Списання", DocumentTypeEnum.WriteOff),
                new ReportItemViewModel("Акт тех. стану (№11)", DocumentTypeEnum.TechnicalState11),
                new ReportItemViewModel("Акт тех. стану (№7)", DocumentTypeEnum.TechnicalState7),
                new ReportItemViewModel("Оцінка", DocumentTypeEnum.Valuation),
                new ReportItemViewModel("Розукомплектація", DocumentTypeEnum.Dismantling),
            };

            return reportTypes;
        }

        private void AddWriteOffReport(DocumentTypeEnum documentType)
        {
            ITabViewModel? createdTab;
            switch (documentType)
            {
                case DocumentTypeEnum.WriteOff:
                    createdTab = new WriteOffReportViewModel(_reportManager, _assetFactory, _dataService, _reportDataService, _navigationService);
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
                default:
                    createdTab = null;
                    break;

            }

            if (createdTab != null)
            {
                TabAdded?.Invoke(this, createdTab);
            }
        }

        private void OpenSettingsExecute()
        {
            if (_settingsViewModel?.IsClosed == false)
            {
                TabSelectionRequested?.Invoke(this, _settingsViewModel);
            }
            else
            {
                _settingsViewModel = new SettingsViewModel(_reportDataService);
                TabAdded?.Invoke(this, _settingsViewModel);
            }
        }

        private void OpenProductsDictionaryCommandExecute()
        {
            if (_productsDictionaryViewModel?.IsClosed == false)
            {
                TabSelectionRequested?.Invoke(this, _productsDictionaryViewModel);
            }
            else
            {
                _productsDictionaryViewModel = new ProductsDictionaryViewModel(_dataService, _exportService);
                TabAdded?.Invoke(this, _productsDictionaryViewModel);
            }
        }

        private void OpenReportConfigurationCommandExecute()
        {
            if (_reportConfigViewModel?.IsClosed == false)
            {
                TabSelectionRequested?.Invoke(this, _reportConfigViewModel);
            }
            else
            {
                _reportConfigViewModel = new ReportConfigViewModel(_reportDataService, _exportService);
                TabAdded?.Invoke(this, _reportConfigViewModel);
            }
        }
    }
}
