using Mil.Paperwork.Infrastructure.MVVM;
using Mil.Paperwork.Domain.Services;
using Mil.Paperwork.WriteOff.Managers;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.WriteOff.Enums;

namespace Mil.Paperwork.WriteOff.ViewModels
{
    internal class HomePageViewModel : ObservableItem, ITabViewModel
    {
        private readonly ReportManager _reportManager;
        private readonly IDataService _dataService;
        private readonly INavigationService _navigationService;

        public event EventHandler<ITabViewModel> TabAdded;
        public event EventHandler<ITabViewModel> TabCloseRequested;

        public string Header => "HOME PAGE";

        public List<ReportViewModelItem> DocumentTypes { get; private set; }

        public ICommand<DocumentTypeEnum> CreateReportCommand { get; }

        public HomePageViewModel(ReportManager reportManager, IDataService dataService, INavigationService navigationService)
        {
            _reportManager = reportManager;
            _dataService = dataService;
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
                    createdTab = new WriteOffReportViewModel(_reportManager, _dataService, _navigationService);
                    break;
                case DocumentTypeEnum.Valuation:
                    createdTab = new AssetValuationViewModel(_reportManager, _dataService, _navigationService);
                    break;
                case DocumentTypeEnum.Dismantling:
                    createdTab = new AssetDismantlingViewModel(_reportManager, _dataService, _navigationService);
                    break;
                case DocumentTypeEnum.TechnicalState11:
                    createdTab = new AssetTechnicalStateViewModel(_reportManager, _dataService, _navigationService);
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
