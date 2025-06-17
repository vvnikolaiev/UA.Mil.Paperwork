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

namespace Mil.Paperwork.WriteOff.ViewModels.Reports
{
    internal class AssetInitialTechnicalStateViewModel : BaseReportTabViewModel
    {
        private readonly ReportManager _reportManager;
        private readonly IDataService _dataService;

        private AssetViewModel _assetViewModel;
        private EventType _eventType;
        
        public override string Header => "Тех. стан (№7)";

        public EventType EventType
        {
            get => _eventType;
            set => SetProperty(ref _eventType, value);
        }

        public AssetViewModel Asset
        {
            get => _assetViewModel;
            set => SetProperty(ref _assetViewModel, value);
        }

        public ProductSelectionViewModel ProductSelector { get; }

        public ObservableCollection<EventType> EventTypes { get; private set; }

        public ICommand ProductSelectedCommand { get; }
        public ICommand GenerateReportCommand { get; }
        public ICommand CloseCommand { get; }
        public ICommand OpenConfigurationCommand { get; }

        public AssetInitialTechnicalStateViewModel(ReportManager reportManager, IAssetFactory assetFactory, IDataService dataService)
        {
            _reportManager = reportManager;
            _dataService = dataService;

            _assetViewModel = assetFactory.CreateAssetViewModel();

            ProductSelector = new ProductSelectionViewModel(_dataService);

            FillAssetTypesCollection();

            ProductSelectedCommand = new DelegateCommand(ProductSelectedExecute);
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
                var assets = new List<IAssetInfo>() { _assetViewModel.ToAssetInfo(EventType) };

                GenerateReport(assets, folderName);
                
                var productInfos = assets.Select(DTOConvertionHelper.ConvertToProductDTO).ToList();
                _dataService.AlterProductsData(productInfos);
            }
        }

        protected virtual void GenerateReport(IEnumerable<IAssetInfo> assets, string destinationFolder)
        {
            var reportData = new InitialTechnicalStateReportData
            {
                EventType = _eventType,
                Assets = [.. assets],
                DestinationFolder = destinationFolder
            };

            _reportManager.GenerateInitialTechnicalStateReport(reportData);
        }

        private void FillAssetTypesCollection()
        {
            EventTypes = [.. EnumHelper.GetValues<EventType>()];
        }

        private void ProductSelectedExecute()
        {
            FillProductDetails();
        }

        private void FillProductDetails()
        {
            var product = ProductSelector?.SelectedProduct;
            if (product != null)
            {
                Asset.Name = product.Name;
                Asset.ShortName = product.ShortName;
                Asset.NomenclatureCode = product.NomenclatureCode;
                Asset.Price = product.Price;
                Asset.MeasurementUnit = product.MeasurementUnit;
                Asset.StartDate = product.StartDate;
                Asset.WarrantyPeriodMonths = product.WarrantyPeriodMonths;
            }
            else
            {
                OnPropertyChanged(nameof(Asset.Name));
            }
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
