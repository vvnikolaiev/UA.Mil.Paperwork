using Mil.MVVM.Common;
using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Domain.Enums;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Helpers;
using Mil.Paperwork.DataAccess.Services;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.UI.Factories;
using Mil.Paperwork.UI.Helpers;
using Mil.Paperwork.UI.Managers;
using Mil.Paperwork.UI.ViewModels.Ribbon;
using Mil.Paperwork.UI.ViewModels.Tabs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Mil.Paperwork.UI.ViewModels.Reports
{
    internal class ResidualValueReportViewModel : BaseReportTabViewModel, IReportDataLoadable<IResidualValueReportData>
    {
        private readonly IDataService _dataService;
        private readonly ReportManager _reportManager;
        private readonly IDialogService _dialogService;
        private readonly IReportDataService _reportDataService;
        private int? _eventReportNumber = null;
        private DateTimeOffset _writeOffDate = DateTimeOffset.Now.Date;
        private EventType _eventType;
        private string _destinationFolderPath = "C:\\Work\\Temp";
        private AssetsTableViewModel _assetsTable;
        private AssetType _selectedAssetType;

        private const string HeaderText = "Залишкова вартість";

        public override string Header => EventReportNumber != null ? $"{HeaderText} ({EventReportNumber})" : HeaderText;

        public ObservableCollection<MetalCostViewModel> MetalCostCollection { get; set; }

        public AssetsTableViewModel AssetsTable
        {
            get => _assetsTable;
            set => SetProperty(ref _assetsTable, value);
        }

        public int? EventReportNumber
        {
            get => _eventReportNumber;
            set
            {
                SetProperty(ref _eventReportNumber, value);
                OnPropertyChanged(nameof(Header));
            }
        }

        public DateTimeOffset WriteOffDate
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

        public IDelegateCommand GenerateReportCommand { get; }
        public IDelegateCommand SelectFolderCommand { get; }

        protected override ReportType HistoryReportType => ReportType.ResidualValueReport;

        protected override string AutomationIdPrefix => "ResidualValue";

        public ResidualValueReportViewModel(
            ReportManager reportManager,
            IAssetFactory assetFactory,
            IDataService dataService,
            IReportDataService reportDataService,
            IReportHistoryService reportHistoryService,
            IDialogService dialogService) : base(reportHistoryService, dialogService)
        {
            _dataService = dataService;
            _reportManager = reportManager;
            _dialogService = dialogService;
            _reportDataService = reportDataService;

            AssetsTable = new AssetsTableViewModel(assetFactory, dataService, dialogService);

            MetalCostCollection = [.. EnumHelper.GetDescriptionDictionary<MetalType>().Select(x => new MetalCostViewModel(x.Key))];

            SelectedAssetType = reportDataService.GetAssetType();

            GenerateReportCommand = new DelegateCommand(GenerateReport);
            SelectFolderCommand = new DelegateCommand(SelectFolder);

            ResumeDirtyTracking();
        }

        protected override IList<RibbonGroupViewModel> BuildRibbonGroups()
        {
            var documentGroup = CreateDocumentGroup(GenerateReportCommand);
            var tableGroup = CreateTableGroup(AssetsTable.AddRowCommand, AssetsTable.RemoveRowCommand, null, AssetsTable.ClearTableCommand);
            var reportGroup = CreateReportGroup(null, SelectFolderCommand);
            var groups = new List<RibbonGroupViewModel> { documentGroup, tableGroup, reportGroup };
            return groups;
        }

        protected override IReportData BuildReportData()
        {
            var assets = AssetsTable.AssetsCollection.Select(x => x.ToAssetInfo(EventType));
            var reportData = new ResidualValueReportData
            {
                EventReportNumber = EventReportNumber,
                AssetType = SelectedAssetType,
                DestinationFolder = DestinationFolderPath,
                EventDate = WriteOffDate.Date,
                Assets = [.. assets],
                MetalCosts = MetalCostCollection.ToDictionary(x => x.Metal, x => x.Cost),
                ServiceKey = _reportDataService.GetSelectedService()
            };

            return reportData;
        }

        private void GenerateReport()
        {
            var reportData = (ResidualValueReportData)BuildReportData();

            reportData.DestinationFolder = Path.Combine(reportData.DestinationFolder, $"{reportData.EventDate:yyyyMMdd} {reportData.EventReportNumber}");

            GenerateReport(reportData);

            ResetDirtyState();
        }

        private void GenerateReport(IResidualValueReportData reportData)
        {
            if (reportData == null || reportData.Assets == null)
            {
                return;
            }

            var productInfos = reportData.Assets.Select(DTOConvertionHelper.ConvertToProductDTO).ToList();
            _dataService.AlterProductsData(productInfos);
            _reportManager.GenerateResidualValueReport(reportData, EnsureHistoryEntryId());
        }

        public void LoadReportData(IResidualValueReportData data)
        {
            WithDirtyTrackingSuspended(() =>
            {
                EventReportNumber = data.EventReportNumber;
                WriteOffDate = new DateTimeOffset(data.EventDate);
                SelectedAssetType = data.AssetType;
                AssetsTable.LoadAssets(data.Assets ?? []);

                foreach (var metalCostVm in MetalCostCollection)
                {
                    if (data.MetalCosts.TryGetValue(metalCostVm.Metal, out var cost))
                    {
                        metalCostVm.Cost = cost;
                    }
                }
            });
        }

        private void SelectFolder()
        {
            if (_dialogService.TryPickFolder(out var folderName))
            {
                DestinationFolderPath = folderName;
            }
        }
    }

    public class MetalCostViewModel : ObservableItem
    {
        public string Title { get; }
        public MetalType Metal { get; }
        private decimal _cost = 0;
        public decimal Cost
        {
            get => _cost;
            set => SetProperty(ref _cost, value);
        }

        public MetalCostViewModel(MetalType metal, decimal cost = 0)
        {
            Metal = metal;
            Title = metal.GetDescription();
            Cost = cost;
        }
    }
}
