using Mil.Paperwork.Infrastructure.MVVM;
using Mil.Paperwork.Domain.DataModels;
using Mil.Paperwork.WriteOff.Managers;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Mil.Paperwork.Infrastructure.DataModels;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.WriteOff.Helpers;
using Microsoft.Win32;
using Mil.Paperwork.WriteOff.Factories;
using Mil.Paperwork.Domain.DataModels.Assets;
using Mil.Paperwork.Domain.Enums;
using Mil.Paperwork.Infrastructure.Helpers;
using Mil.Paperwork.WriteOff.DataModels;

namespace Mil.Paperwork.WriteOff.ViewModels
{
    internal class AssetInitialTechnicalStateViewModel : ObservableItem, ITabViewModel
    {
        private readonly ReportManager _reportManager;
        private readonly IDataService _dataService;

        private AssetViewModel _assetViewModel;
        private EventType _eventType;
        
        public event EventHandler<ITabViewModel> TabCloseRequested;

        public virtual string Header => "Тех. стан (№7)";

        public bool IsClosed { get; private set; }

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

        public ObservableCollection<EnumItemDataModel<EventType>> EventTypes { get; private set; }

        public ICommand ProductSelectedCommand { get; }
        public ICommand GenerateReportCommand { get; }
        public ICommand CloseCommand { get; }

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
                _dataService.SaveProductsData(productInfos);
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
            var eventTypes = EnumHelper.GetValuesWithDescriptions<EventType>().Select(x => new EnumItemDataModel<EventType>(x.Value, x.Description));
            EventTypes = [.. eventTypes];
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
                Asset.WarrantyPeriodYears = product.WarrantyPeriodYears;
            }
            else
            {
                OnPropertyChanged(nameof(Asset.Name));
            }
        }

        private void CloseCommandExecute()
        {
            if (MessageBox.Show("Are you sure you want to close this tab?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                TabCloseRequested.Invoke(this, this);
                IsClosed = true;
            }
        }

    }
}
