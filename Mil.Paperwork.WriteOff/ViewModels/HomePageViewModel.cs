using Mil.Paperwork.Infrastructure.MVVM;
using Mil.Paperwork.Domain.Services;
using Mil.Paperwork.WriteOff.Managers;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.WriteOff.Enums;
using Mil.Paperwork.WriteOff.Factories;

namespace Mil.Paperwork.WriteOff.ViewModels
{
    internal class HomePageViewModel : ObservableItem, ITabViewModel
    {
        private readonly ReportManager _reportManager;
        private readonly IAssetFactory _assetFactory;
        private readonly IDataService _dataService;
        private readonly IReportDataService _reportDataService;
        private readonly INavigationService _navigationService;

        public event EventHandler<ITabViewModel> TabAdded;
        public event EventHandler<ITabViewModel> TabCloseRequested;

        public string Header => "HOME PAGE";

        public List<ReportViewModelItem> DocumentTypes { get; private set; }

        public ICommand<DocumentTypeEnum> CreateReportCommand { get; }

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
        }

        private IList<ReportViewModelItem> GetAllReportTypes()
        {
            var reportTypes = new List<ReportViewModelItem>()
            {
                new ReportViewModelItem("Списання", DocumentTypeEnum.WriteOff),
                new ReportViewModelItem("Акт тех. стану", DocumentTypeEnum.TechnicalState11),
                new ReportViewModelItem("Оцінка", DocumentTypeEnum.Valuation),
                new ReportViewModelItem("Розукомплектація", DocumentTypeEnum.Dismantling),
            };

            return reportTypes;
        }

        private void AddWriteOffReport(DocumentTypeEnum documentType)
        {
            ITabViewModel createdTab;
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
    }
}
