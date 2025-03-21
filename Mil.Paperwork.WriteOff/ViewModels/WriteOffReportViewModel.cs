using Mil.Paperwork.Infrastructure.MVVM;
using Mil.Paperwork.Domain.DataModels;
using Mil.Paperwork.WriteOff.Managers;
using Mil.Paperwork.WriteOff.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using Mil.Paperwork.Infrastructure.DataModels;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.Domain.Services;
using Mil.Paperwork.WriteOff.Views;

namespace Mil.Paperwork.WriteOff.ViewModels
{
    public class WriteOffReportViewModel : ObservableItem, ITabViewModel
    {
        private readonly ReportManager _reportManager;
        private readonly IDataService _dataService;
        private readonly INavigationService _navigationService;
        private WriteOffReportModel _model;
        private IList<ProductDTO> _loadedProducts;
        private ObservableCollection<ProductDTO> _products;
        private string _registrationNumber = string.Empty;
        private string _documentNumber = string.Empty;
        private DateTime _writeOffDate = DateTime.Now.Date;
        private string _reason = string.Empty;
        private string _destinationFolderPath = "C:\\Work\\Temp";
        private AssetViewModel _selectedAsset;
        private AssetValuationViewModel _selectedDismantlingItem;
        private ObservableCollection<AssetDismantlingViewModel> _dismantleCollection = [];

        public event EventHandler<ITabViewModel> TabCloseRequested;

        public string Header => "Списання";

        public ObservableCollection<AssetViewModel> AssetsCollection { get; set; }

        public ObservableCollection<ProductDTO> Products
        {
            get => _products;
            set => SetProperty(ref _products, value);
        }

        public AssetViewModel SelectedAsset
        {
            get => _selectedAsset;
            set => SetProperty(ref _selectedAsset, value);
        }

        public string RegistrationNumber
        {
            get => _registrationNumber;
            set => SetProperty(ref _registrationNumber, value);
        }
        public string DocumentNumber
        {
            get => _documentNumber;
            set => SetProperty(ref _documentNumber, value);
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

        public string Reason
        {
            get => _reason;
            set => SetProperty(ref _reason, value);
        }

        public AssetValuationViewModel SelectedDismantlingItem
        {
            get => _selectedDismantlingItem;
            set => SetProperty(ref _selectedDismantlingItem, value);
        }

        public ObservableCollection<AssetDismantlingViewModel> DismantleCollection
        {
            get => _dismantleCollection;
            set => SetProperty(ref _dismantleCollection, value);
        }

        public ICommand<AssetValuationViewModel> OpenDismatlingItemCommand { get; }
        public ICommand CopySelectedAssetCommand { get; }
        public ICommand GenerateReportCommand { get; }
        public ICommand ClearTableCommand { get; }
        public ICommand AddRowCommand { get; }
        public ICommand RemoveRowCommand { get; }
        public ICommand SelectFolderCommand { get; }
        public ICommand AddDismantlingCommand { get; }
        public ICommand CloseCommand { get; }

        public WriteOffReportViewModel(ReportManager reportManager, IDataService dataService, INavigationService navigationService)
        {
            _reportManager = reportManager;
            _dataService = dataService;
            _navigationService = navigationService;

            _model = new WriteOffReportModel(reportManager, dataService);
            AssetsCollection = new ObservableCollection<AssetViewModel>();
            UpdateProductsCollection();

            GenerateReportCommand = new DelegateCommand(GenerateReport);
            ClearTableCommand = new DelegateCommand(ClearTable);
            AddRowCommand = new DelegateCommand(AddRow);
            RemoveRowCommand = new DelegateCommand(RemoveRowExecute);
            SelectFolderCommand = new DelegateCommand(SelectFolder);
            CopySelectedAssetCommand = new DelegateCommand(CopySelectedAssetExecute);
            AddDismantlingCommand = new DelegateCommand(AddDismantlingExecute);
            OpenDismatlingItemCommand = new DelegateCommand<AssetValuationViewModel>(OpenDismatlingItemExecute);
            CloseCommand = new DelegateCommand(CloseCommandExecute);
        }

        private void GenerateReport()
        {
            var reportData = new WriteOffReportData
            {
                DestinationFolder = DestinationFolderPath,
                RegistrationNumber = RegistrationNumber,
                DocumentNumber = DocumentNumber,
                Reason = Reason,
                ReportDate = WriteOffDate,
                Assets = [.. AssetsCollection.Select(x => x.ToAssetInfo())],
                Dismantlings = [.. DismantleCollection.Select(x => x.ToAssetDismantlingData())]
            };

            _model.GenerateReport(reportData);

            UpdateProductsCollection();
        }

        private void UpdateProductsCollection()
        {
            _loadedProducts = _model.LoadProductData();

            UpdateMergedProductsCollection();
        }

        private void UpdateMergedProductsCollection()
        {
            var products = new List<ProductDTO>();

            var excludedItems = DismantleCollection
                .Where(x => x != null && x.IsValid)
                .SelectMany(x => x.Components
                            .Where(c => c != null && x.IsValid && c.Exclude)
                            .Select(c => c.ToProductDTO()));

            if (excludedItems != null)
            {
                products.AddRange(excludedItems);
            }

            products.AddRange(_loadedProducts);

            Products = [.. products];
        }

        private void ClearTable()
        {
            if (MessageBox.Show("Are you sure you want to clear the table?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                AssetsCollection.Clear();
            }
        }

        private void AddRow()
        {
            var asset = CreateDefaultAsset();
            AssetsCollection.Add(asset);

            SelectedAsset = asset;
        }

        private void RemoveRowExecute()
        {
            if (SelectedAsset != null)
            {
                AssetsCollection.Remove(SelectedAsset);

                SelectedAsset = AssetsCollection.FirstOrDefault();
            }
        }

        private void CopySelectedAssetExecute()
        {
            if (SelectedAsset != null)
            {
                var copiedAsset = new AssetInfo
                {
                    Name = _selectedAsset.Name,
                    ShortName = _selectedAsset.ShortName,
                    MeasurementUnit = _selectedAsset.MeasurementUnit,
                    SerialNumber = _selectedAsset.SerialNumber,
                    NomenclatureCode = _selectedAsset.NomenclatureCode,
                    Category = _selectedAsset.Category,
                    Price = _selectedAsset.Price,
                    Count = _selectedAsset.Count,
                    WearAndTearCoeff = _selectedAsset.WearAndTearCoeff,
                    StartDate = _selectedAsset.StartDate,
                    TSRegisterNumber = _selectedAsset.TSRegisterNumber,
                    TSDocumentNumber = _selectedAsset.TSDocumentNumber,
                    WarrantyPeriodYears = _selectedAsset.WarrantyPeriodYears
                };

                var copiedItem = new AssetViewModel(copiedAsset, _reportManager, _dataService, _navigationService);

                AssetsCollection.Add(copiedItem);

                SelectedAsset = copiedItem;
            }
        }

        private void SelectFolder()
        {
            var a = new OpenFolderDialog();
            if (a.ShowDialog() == true)
            {
                DestinationFolderPath = a.FolderName;
            }
        }

        private void AddDismantlingExecute()
        {
            var viewModel = _navigationService.OpenWindow<AssetValuationDialogWindow, AssetDismantlingViewModel>();
            
            if (viewModel.IsValid)
            {
                DismantleCollection.Add(viewModel);

                UpdateMergedProductsCollection();
            }
        }

        private void OpenDismatlingItemExecute(AssetValuationViewModel viewModel)
        {
            _navigationService.OpenWindow<AssetValuationDialogWindow, AssetValuationViewModel>(viewModel);

            UpdateMergedProductsCollection();
        }

        private void CloseCommandExecute()
        {
            if (MessageBox.Show("Are you sure you want to close this tab?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                TabCloseRequested.Invoke(this, this);
            }
        }

        private AssetViewModel CreateDefaultAsset()
        {
            var assetVM = new AssetViewModel(_reportManager, _dataService, _navigationService);
            return assetVM;
        }
    }
}
