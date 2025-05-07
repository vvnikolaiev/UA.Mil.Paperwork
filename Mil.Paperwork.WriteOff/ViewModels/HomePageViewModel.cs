using Mil.Paperwork.Infrastructure.MVVM;
using Mil.Paperwork.Domain.Services;
using Mil.Paperwork.WriteOff.Managers;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.WriteOff.Enums;
using Mil.Paperwork.WriteOff.Factories;
using System.Windows.Input;

namespace Mil.Paperwork.WriteOff.ViewModels
{
    internal class HomePageViewModel : ObservableItem, ITabViewModel
    {
        private readonly ReportManager _reportManager;
        private readonly IAssetFactory _assetFactory;
        private readonly IDataService _dataService;
        private readonly IReportDataService _reportDataService;
        private readonly INavigationService _navigationService;
        private SettingsViewModel _settingsViewModel;

        public event EventHandler<ITabViewModel> TabAdded;
        public event EventHandler<ITabViewModel> TabSelectionRequested;
        public event EventHandler<ITabViewModel> TabCloseRequested;

        public string Header => "HOME PAGE";

        public bool IsClosed => false;

        public List<ReportItemViewModel> DocumentTypes { get; private set; }

        public ICommand<DocumentTypeEnum> CreateReportCommand { get; }

        public ICommand OpenSettingsCommand { get; }

        public HomePageViewModel(
            ReportManager reportManager,
            IAssetFactory assetFactory,
            IDataService dataService,
            IReportDataService reportDataService,
            INavigationService navigationService)
        {
            _reportManager = reportManager;
            _assetFactory = assetFactory;
            _dataService = dataService;
            _reportDataService = reportDataService;
            _navigationService = navigationService;

            DocumentTypes = [.. GetAllReportTypes()];

            CreateReportCommand = new DelegateCommand<DocumentTypeEnum>(AddWriteOffReport);
            OpenSettingsCommand = new DelegateCommand(OpenSettingsExecute);
        }

        private IList<ReportItemViewModel> GetAllReportTypes()
        {
            var reportTypes = new List<ReportItemViewModel>()
            {
                new ReportItemViewModel("Списання", DocumentTypeEnum.WriteOff),
                new ReportItemViewModel("Акт тех. стану", DocumentTypeEnum.TechnicalState11),
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
                case DocumentTypeEnum.TechnicalState11:
                    createdTab = new AssetTechnicalStateViewModel(_reportManager, _assetFactory, _dataService, _navigationService);
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
    }
}
