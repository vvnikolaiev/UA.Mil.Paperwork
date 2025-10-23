using Mil.Paperwork.Infrastructure.MVVM;
using Mil.Paperwork.WriteOff.Managers;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.WriteOff.Helpers;
using Microsoft.Win32;
using Mil.Paperwork.WriteOff.Factories;
using Mil.Paperwork.Domain.DataModels.Assets;
using Mil.Paperwork.Domain.Enums;
using Mil.Paperwork.Infrastructure.Helpers;
using Mil.Paperwork.WriteOff.ViewModels.Tabs;
using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.WriteOff.ViewModels.Dictionaries;

namespace Mil.Paperwork.WriteOff.ViewModels.Reports
{
    internal class AssetInitialTechnicalStateViewModel : BaseReportTabViewModel
    {
        private readonly ReportManager _reportManager;
        private readonly IDataService _dataService;

        private AssetsTableViewModel _assetsTable;
        private AssetAccetpanceViewModel _assetAcceptance;
        private EventType _eventType;

        public override string Header => "Тех. стан (№7)";

        public EventType EventType
        {
            get => _eventType;
            set => SetProperty(ref _eventType, value);
        }

        public AssetsTableViewModel AssetsTable
        {
            get => _assetsTable;
            set => SetProperty(ref _assetsTable, value);
        }

        public AssetAccetpanceViewModel AssetAcceptance
        {
            get => _assetAcceptance;
            set => SetProperty(ref _assetAcceptance, value);
        }

        public ObservableCollection<EventType> EventTypes { get; private set; }

        public ObservableCollection<MeasurementUnitViewModel> MeasurementUnits { get; }

        public ICommand GenerateReportCommand { get; }
        public ICommand CloseCommand { get; }
        public ICommand OpenConfigurationCommand { get; }

        public AssetInitialTechnicalStateViewModel(ReportManager reportManager, IAssetFactory assetFactory, IDataService dataService)
        {
            _reportManager = reportManager;
            _dataService = dataService;

            AssetsTable = new AssetsTableViewModel(assetFactory, dataService);
            AssetAcceptance = new AssetAccetpanceViewModel(dataService);

            FillAssetTypesCollection();
            MeasurementUnits = [.. _dataService.LoadMeasurementUnitsData().Select(x => new MeasurementUnitViewModel(x))];

            GenerateReportCommand = new DelegateCommand(GenerateReport);
            CloseCommand = new DelegateCommand(CloseCommandExecute);
            OpenConfigurationCommand = new DelegateCommand(OpenConfigurationCommandExecute);
        }

        private void GenerateReport()
        {
            var folderDialog = new OpenFolderDialog();

            if (folderDialog.ShowDialog() == true)
            {
                var folderName = folderDialog.FolderName;
                var assets = AssetsTable.AssetsCollection.Select(x => x.ToAssetInfo(EventType)).ToArray();

                GenerateReport(assets, folderName);

                var productInfos = assets.Select(DTOConvertionHelper.ConvertToProductDTO).ToList();
                _dataService.AlterProductsData(productInfos);
            }
        }

        protected virtual void GenerateReport(IEnumerable<IAssetInfo> assets, string destinationFolder)
        {

            var personAccepted = AssetAcceptance.GetReceiverDTO();
            var personHanded = AssetAcceptance.GetTransmitterDTO();

            var reportData = new InitialTechnicalStateReportData
            {
                EventType = _eventType,
                Assets = [.. assets],
                DestinationFolder = destinationFolder,
                PersonAccepted = personAccepted,
                PersonHanded = personHanded
            };

            _dataService.AlterPeople([personAccepted, personHanded]);

            _reportManager.GenerateInitialTechnicalStateReport(reportData);
        }

        private void FillAssetTypesCollection()
        {
            EventTypes = [.. EnumHelper.GetValues<EventType>()];
        }

        private void CloseCommandExecute()
        {
            Close();
        }

        private void OpenConfigurationCommandExecute()
        {
            OpenSettings(Infrastructure.Enums.ReportType.TechnicalStateReport);
        }
    }
}
