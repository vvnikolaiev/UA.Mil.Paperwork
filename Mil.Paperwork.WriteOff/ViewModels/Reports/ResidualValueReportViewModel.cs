using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Domain.Enums;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Helpers;
using Mil.Paperwork.WriteOff.MVVM;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.WriteOff.Factories;
using Mil.Paperwork.WriteOff.Helpers;
using Mil.Paperwork.WriteOff.Managers;
using Mil.Paperwork.WriteOff.ViewModels.Tabs;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;

namespace Mil.Paperwork.WriteOff.ViewModels.Reports
{
    internal class ResidualValueReportViewModel : BaseReportTabViewModel
    {
        private readonly IDataService _dataService;
        private readonly ReportManager _reportManager;
        private readonly IDialogService _dialogService;
        private int? _eventReportNumber = null;
        private DateTime _writeOffDate = DateTime.Now.Date;
        private EventType _eventType;
        private string _destinationFolderPath = "C:\\Work\\Temp";
        private AssetsTableViewModel _assetsTable;
        private AssetType _selectedAssetType;

        public override string Header => "Залишкова вартість";

        public ObservableCollection<MetalCostViewModel> MetalCostCollection { get; set; }

        public AssetsTableViewModel AssetsTable
        {
            get => _assetsTable;
            set => SetProperty(ref _assetsTable, value);
        }

        public int? EventReportNumber
        {
            get => _eventReportNumber;
            set => SetProperty(ref _eventReportNumber, value);
        }

        public DateTime WriteOffDate
        {
            get => _writeOffDate;
            set => SetProperty(ref _writeOffDate, value);
        }

        public string DestinationFolderPath
        {
            get => _destinationFolderPath;
            set => SetProperty(ref _destinationFolderPath, value);
        }

        public EventType EventType
        {
            get => _eventType;
            set => SetProperty(ref _eventType, value);
        }

        public AssetType SelectedAssetType
        {
            get => _selectedAssetType;
            set => SetProperty(ref _selectedAssetType, value);
        }

        public ICommand GenerateReportCommand { get; }
        public ICommand SelectFolderCommand { get; }
        public ICommand CloseCommand { get; }

        public ResidualValueReportViewModel(
            ReportManager reportManager,
            IAssetFactory assetFactory,
            IDataService dataService,
            IReportDataService reportDataService,
            IDialogService dialogService) : base(dialogService)
        {
            _dataService = dataService;
            _reportManager = reportManager;
            _dialogService = dialogService;

            AssetsTable = new AssetsTableViewModel(assetFactory, dataService, dialogService);

            MetalCostCollection = [.. EnumHelper.GetDescriptionDictionary<MetalType>().Select(x => new MetalCostViewModel(x.Key))];

            SelectedAssetType = reportDataService.GetAssetType();

            GenerateReportCommand = new DelegateCommand(GenerateReport);
            SelectFolderCommand = new DelegateCommand(SelectFolder);
            CloseCommand = new DelegateCommand(CloseCommandExecute);
        }

        private void GenerateReport()
        {
            var assets = AssetsTable.AssetsCollection.Select(x => x.ToAssetInfo(EventType));
            var reportData = new ResidualValueReportData
            {
                EventReportNumber = EventReportNumber,
                AssetType = SelectedAssetType,
                DestinationFolder = DestinationFolderPath,
                EventDate = WriteOffDate,
                Assets = [.. assets],
                MetalCosts = MetalCostCollection.ToDictionary(x => x.Metal, x => x.Cost)
            };

            reportData.DestinationFolder = Path.Combine(reportData.DestinationFolder, $"{reportData.EventDate:yyyyMMdd} {reportData.EventReportNumber}");

            GenerateReport(reportData);

            AssetsTable.Refresh();
        }

        private void GenerateReport(IResidualValueReportData reportData)
        {
            if (reportData == null || reportData.Assets == null)
            {
                return;
            }

            var productInfos = reportData.Assets.Select(DTOConvertionHelper.ConvertToProductDTO).ToList();
            _dataService.AlterProductsData(productInfos);
            _reportManager.GenerateResidualValueReport(reportData);
        }

        private void SelectFolder()
        {
            if (_dialogService.TryPickFolder(out var folderName))
            {
                DestinationFolderPath = folderName;
            }
        }

        private void CloseCommandExecute()
        {
            Close();
        }
    }

    public class MetalCostViewModel : ObservableItem
    {
        public string Title { get; }
        public MetalType Metal { get; }
        private decimal _cost;
        public decimal Cost
        {
            get => _cost;
            set { _cost = value; OnPropertyChanged(); }
        }

        public MetalCostViewModel(MetalType metal, decimal cost = 0)
        {
            Metal = metal;
            Title = metal.GetDescription();
            Cost = cost;
        }
    }
}
