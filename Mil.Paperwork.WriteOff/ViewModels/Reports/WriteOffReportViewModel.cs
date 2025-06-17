using Mil.Paperwork.Infrastructure.MVVM;
using Mil.Paperwork.WriteOff.Managers;
using Mil.Paperwork.WriteOff.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.Domain.Services;
using Mil.Paperwork.WriteOff.Views;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.WriteOff.Factories;
using Mil.Paperwork.Infrastructure.Helpers;
using Mil.Paperwork.Domain.Enums;
using Mil.Paperwork.WriteOff.ViewModels.Tabs;
using Mil.Paperwork.Domain.DataModels.ReportData;

namespace Mil.Paperwork.WriteOff.ViewModels.Reports
{
    internal class WriteOffReportViewModel : BaseReportTabViewModel
    {
        private readonly IAssetFactory _assetFactory;
        private readonly INavigationService _navigationService;
        private readonly WriteOffReportModel _model;

        private int? _eventReportNumber = null;
        private string _registrationNumber = string.Empty;
        private string _documentNumber = string.Empty;
        private DateTime _writeOffDate = DateTime.Now.Date;
        private string _reason = string.Empty;
        private EventType _eventType;
        private string _destinationFolderPath = "C:\\Work\\Temp";
        private AssetViewModel? _selectedAsset;
        private AssetValuationViewModel _selectedValuationItem;
        private ObservableCollection<AssetValuationViewModel> _valuationCollection = [];
        private AssetDismantlingViewModel _selectedDismantlingItem;
        private ObservableCollection<AssetDismantlingViewModel> _dismantleCollection = [];
        private AssetType _selectedAssetType;

        public override string Header => "Списання";

        public ProductSelectionViewModel ProductsSelector { get; }

        public ObservableCollection<AssetViewModel> AssetsCollection { get; set; }

        public AssetViewModel? SelectedAsset
        {
            get => _selectedAsset;
            set => SetProperty(ref _selectedAsset, value);
        }

        public int? EventReportNumber
        {
            get => _eventReportNumber;
            set => SetProperty(ref _eventReportNumber, value);
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

        public EventType EventType
        {
            get => _eventType;
            set => SetProperty(ref _eventType, value);
        }

        public AssetValuationViewModel SelectedValuationItem
        {
            get => _selectedValuationItem;
            set => SetProperty(ref _selectedValuationItem, value);
        }

        public ObservableCollection<AssetValuationViewModel> ValuationCollection
        {
            get => _valuationCollection;
            set => SetProperty(ref _valuationCollection, value);
        }

        public AssetDismantlingViewModel SelectedDismantlingItem
        {
            get => _selectedDismantlingItem;
            set => SetProperty(ref _selectedDismantlingItem, value);
        }

        public ObservableCollection<AssetDismantlingViewModel> DismantleCollection
        {
            get => _dismantleCollection;
            set => SetProperty(ref _dismantleCollection, value);
        }

        public bool IsAnyValuationOrDismantling => ValuationCollection.Count > 0 || DismantleCollection.Count > 0;

        public AssetType SelectedAssetType
        {
            get => _selectedAssetType;
            set => SetProperty(ref _selectedAssetType, value);
        }

        public ObservableCollection<EventType> EventTypes { get; private set; }

        public ICommand<AssetValuationViewModel> OpenValuationItemCommand { get; }
        public ICommand<AssetDismantlingViewModel> OpenDismatlingItemCommand { get; }
        public ICommand GenerateReportCommand { get; }
        public ICommand ClearTableCommand { get; }
        public ICommand AddRowCommand { get; }
        public ICommand RemoveRowCommand { get; }
        public ICommand SelectFolderCommand { get; }
        public ICommand AddValuationCommand { get; }
        public ICommand AddDismantlingCommand { get; }
        public ICommand CloseCommand { get; }

        public WriteOffReportViewModel(
            ReportManager reportManager,
            IAssetFactory assetFactory,
            IDataService dataService,
            IReportDataService reportDataService,
            INavigationService navigationService)
        {
            _assetFactory = assetFactory;
            _navigationService = navigationService;

            _model = new WriteOffReportModel(reportManager, dataService);
            ProductsSelector = new ProductSelectionViewModel(dataService);
            AssetsCollection = new ObservableCollection<AssetViewModel>();

            FillAssetTypesCollection();

            SelectedAssetType = reportDataService.GetAssetType();

            GenerateReportCommand = new DelegateCommand(GenerateReport);
            ClearTableCommand = new DelegateCommand(ClearTable);
            AddRowCommand = new DelegateCommand(AddRow);
            RemoveRowCommand = new DelegateCommand(RemoveRowExecute);
            SelectFolderCommand = new DelegateCommand(SelectFolder);
            AddValuationCommand = new DelegateCommand(AddValuationExecute);
            AddDismantlingCommand = new DelegateCommand(AddDismantlingExecute);
            OpenValuationItemCommand = new DelegateCommand<AssetValuationViewModel>(OpenValuationItemExecute);
            OpenDismatlingItemCommand = new DelegateCommand<AssetDismantlingViewModel>(OpenDismatlingItemExecute);
            CloseCommand = new DelegateCommand(CloseCommandExecute);
        }

        private void GenerateReport()
        {
            var reportData = new WriteOffReportData
            {
                EventReportNumber = EventReportNumber,
                AssetType = SelectedAssetType,
                DestinationFolder = DestinationFolderPath,
                RegistrationNumber = RegistrationNumber,
                DocumentNumber = DocumentNumber,
                Reason = Reason,
                EventType = EventType,
                ReportDate = WriteOffDate,
                Assets = [.. AssetsCollection.Select(x => x.ToAssetInfo(EventType, WriteOffDate))],
                Dismantlings = [.. DismantleCollection.Select(x => x.ToAssetDismantlingData())],
                ValuationData = [.. ValuationCollection.Select(x => x.ToAssetValuationData())]
            };

            _model.GenerateReport(reportData);

            UpdateProductsCollection();
        }

        private void FillAssetTypesCollection()
        {
            EventTypes = [.. EnumHelper.GetValues<EventType>()];
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
            var asset = _assetFactory.CreateAssetViewModel();
            AssetsCollection.Add(asset);

            SelectedAsset = asset;
        }

        private void RemoveRowExecute()
        {
            if (SelectedAsset != null && AssetsCollection != null)
            {
                AssetsCollection.Remove(SelectedAsset);

                SelectedAsset = AssetsCollection.FirstOrDefault();
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

                UpdateProductsCollection();
                OnPropertyChanged(nameof(IsAnyValuationOrDismantling));
            }
        }

        private void AddValuationExecute()
        {
            var viewModel = _navigationService.OpenWindow<AssetValuationDialogWindow, AssetValuationViewModel>();

            if (viewModel.IsValid)
            {
                ValuationCollection.Add(viewModel);

                UpdateProductsCollection();
                OnPropertyChanged(nameof(IsAnyValuationOrDismantling));
            }
        }

        private void OpenValuationItemExecute(AssetValuationViewModel viewModel)
        {
            _navigationService.OpenWindow<AssetValuationDialogWindow, AssetValuationViewModel>(viewModel);
            UpdateProductsCollection();
        }

        private void OpenDismatlingItemExecute(AssetDismantlingViewModel viewModel)
        {
            _navigationService.OpenWindow<AssetValuationDialogWindow, AssetDismantlingViewModel>(viewModel);

            UpdateProductsCollection();
        }

        private void CloseCommandExecute()
        {
            Close();
        }

        private void UpdateProductsCollection()
        {
            var assetsWithProdId = AssetsCollection.Select(x => new { Asset = x, x.SelectedProductId }).ToList();

            ProductsSelector.UpdateProductsCollection(DismantleCollection);

            // restore selected values
            var products = ProductsSelector.Products
                .GroupBy(x => x.AlmostUniqueID)
                .ToDictionary(g => g.Key, g => g.Last());

            foreach (var item in assetsWithProdId)
            {
                // If asset.SelectedProductId is not in the new Products, set it to null or a default value
                var selectedValue = item.SelectedProductId;
                if (selectedValue != null)
                {
                    products.TryGetValue(selectedValue, out var product);

                    item.Asset.SelectedProductId = product?.AlmostUniqueID;
                }
            }
        }
    }
}
