using Mil.Paperwork.Infrastructure.MVVM;
using Mil.Paperwork.Domain.DataModels;
using Mil.Paperwork.WriteOff.Managers;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Mil.Paperwork.Infrastructure.DataModels;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.Domain.Services;
using Mil.Paperwork.WriteOff.Helpers;
using Microsoft.Win32;
using Mil.Paperwork.WriteOff.Factories;

namespace Mil.Paperwork.WriteOff.ViewModels
{
    internal class AssetTechnicalStateViewModel : ObservableItem, ITabViewModel
    {
        private readonly ReportManager _reportManager;
        private readonly IDataService _dataService;

        private AssetViewModel _assetViewModel;
        private DateTime _reportDate = DateTime.Now.Date;
        private string _reason = string.Empty;

        private ProductDTO _selectedProduct;
        private ObservableCollection<ProductDTO> _products;

        public event EventHandler<ITabViewModel> TabCloseRequested;

        public string Header => "Тех. стан";

        public bool IsClosed { get; private set; }

        public string Reason
        {
            get => _reason;
            set => SetProperty(ref _reason, value);
        }

        public DateTime ReportDate
        {
            get => _reportDate;
            set => SetProperty(ref _reportDate, value);
        }

        public AssetViewModel Asset
        {
            get => _assetViewModel;
            set => SetProperty(ref _assetViewModel, value);
        }

        public ProductDTO SelectedProduct
        {
            get => _selectedProduct;
            set => SetProperty(ref _selectedProduct, value);
        }

        public ObservableCollection<ProductDTO> Products
        {
            get => _products;
            set => SetProperty(ref _products, value);
        }

        public ICommand ProductSelectedCommand { get; }
        public ICommand GenerateReportCommand { get; }
        public ICommand CloseCommand { get; }


        public AssetTechnicalStateViewModel(ReportManager reportManager, IAssetFactory assetFactory, IDataService dataService, INavigationService navigationService)
        {
            _reportManager = reportManager;
            _dataService = dataService;

            _assetViewModel = assetFactory.CreateAssetViewModel();
            
            UpdateProductsCollection();

            ProductSelectedCommand = new DelegateCommand(ProductSelectedExecute);
            GenerateReportCommand = new DelegateCommand(GenerateReport);
            CloseCommand = new DelegateCommand(CloseCommandExecute);
        }

        private void GenerateReport()
        {
            var folderDialog = new OpenFolderDialog();
            // TODO: folderDialog.InitialDirectory = ;
            if (folderDialog.ShowDialog() == true)
            {
                var folderName = folderDialog.FolderName;
                var assets = new List<IAssetInfo>() { _assetViewModel.ToAssetInfo(ReportDate) };
                var reportData = new TechnicalStateReportData
                {
                    Reason = _reason,
                    ReportDate = _reportDate,
                    Assets = assets,
                    DestinationFolder = folderName
                };

                _reportManager.GenerateTechnicalStateReport(reportData);
                var productInfos = reportData.Assets.Select(DTOConvertionHelper.ConvertToProductDTO).ToList();
                _dataService.SaveProductsData(productInfos);
            }
        }

        private void UpdateProductsCollection()
        {
            Products = [.. _dataService.LoadProductsData()];
        }

        private void ProductSelectedExecute()
        {
            FillProductDetails();
        }

        private void FillProductDetails()
        {
            if (SelectedProduct != null)
            {
                Asset.Name = SelectedProduct.Name;
                Asset.NomenclatureCode = SelectedProduct.NomenclatureCode;
                Asset.Price = SelectedProduct.Price;
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
