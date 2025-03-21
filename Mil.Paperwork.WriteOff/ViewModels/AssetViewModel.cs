using Mil.Paperwork.Domain.DataModels;
using Mil.Paperwork.Domain.Services;
using Mil.Paperwork.Infrastructure.DataModels;
using Mil.Paperwork.Infrastructure.MVVM;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.WriteOff.Managers;
using Mil.Paperwork.WriteOff.Views;
using System.Windows.Input;

namespace Mil.Paperwork.WriteOff.ViewModels
{
    public class AssetViewModel : ObservableItem
    {
        private readonly ReportManager _reportManager;
        private readonly IDataService _dataService;
        private readonly INavigationService _navigationService;
        private readonly AssetInfo _assetInfo;

        private ProductDTO _selectedProduct;
        private string _name = string.Empty;
        private string _shortName = string.Empty;
        private string _measurementUnit = string.Empty;
        private string _serialNumber = string.Empty;
        private string _nomenclatureCode = string.Empty;
        private int _category = 2;
        private decimal _price = 0;
        private int _count = 1;
        private decimal _wearAndTearCoeff = 0.8m;
        private DateTime _startDate = new DateTime(2023, 01, 01);
        private string _tsRegisterNumber = string.Empty;
        private string _tsDocumentNumber = string.Empty;
        private int _warrantyPeriodYears = 5;
        private AssetValuationViewModel _assetValuation;

        public ProductDTO SelectedProduct
        {
            get => _selectedProduct;
            set => SetProperty(ref _selectedProduct, value);
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string ShortName
        {
            get => _shortName;
            set => SetProperty(ref _shortName, value);
        }

        public string MeasurementUnit
        {
            get => _measurementUnit;
            set => SetProperty(ref _measurementUnit, value);
        }

        public string SerialNumber
        {
            get => _serialNumber;
            set => SetProperty(ref _serialNumber, value);
        }

        public string NomenclatureCode
        {
            get => _nomenclatureCode;
            set => SetProperty(ref _nomenclatureCode, value);
        }

        public int Category
        {
            get => _category;
            set => SetProperty(ref _category, value);
        }

        public decimal Price
        {
            get => _price;
            set => SetProperty(ref _price, value);
        }

        public int Count
        {
            get => _count;
            set => SetProperty(ref _count, value);
        }

        public decimal WearAndTearCoeff
        {
            get => _wearAndTearCoeff;
            set => SetProperty(ref _wearAndTearCoeff, value);
        }

        public DateTime StartDate
        {
            get => _startDate;
            set => SetProperty(ref _startDate, value);
        }

        public string TSRegisterNumber
        {
            get => _tsRegisterNumber;
            set => SetProperty(ref _tsRegisterNumber, value);
        }

        public string TSDocumentNumber
        {
            get => _tsDocumentNumber;
            set => SetProperty(ref _tsDocumentNumber, value);
        }

        public int WarrantyPeriodYears
        {
            get => _warrantyPeriodYears;
            set => SetProperty(ref _warrantyPeriodYears, value);
        }

        public AssetValuationViewModel AssetValuation
        {
            get => _assetValuation;
            set => SetProperty(ref _assetValuation, value);
        }

        public ICommand ProductSelectedCommand { get; }
        public ICommand OpenAssetValuationPopupCommand { get; }


        public AssetViewModel(ReportManager reportManager, IDataService dataService, INavigationService navigationService)
        {
            _reportManager = reportManager;
            _dataService = dataService;
            _navigationService = navigationService;

            _assetInfo = new AssetInfo();

            ProductSelectedCommand = new DelegateCommand(ProductSelectedExecute);
            OpenAssetValuationPopupCommand = new DelegateCommand(OpenAssetValuationPopup);
        }

        public AssetViewModel(AssetInfo assetInfo, ReportManager reportManager, IDataService dataService, INavigationService navigationService)
            : this(reportManager, dataService, navigationService)
        {
            _assetInfo = assetInfo;
            UpdateFields();
        }

        public AssetInfo ToAssetInfo()
        {
            _assetInfo.Name = _name;
            _assetInfo.ShortName = _shortName;
            _assetInfo.MeasurementUnit = _measurementUnit;
            _assetInfo.SerialNumber = _serialNumber;
            _assetInfo.NomenclatureCode = _nomenclatureCode;
            _assetInfo.Category = _category;
            _assetInfo.Price = _price;
            _assetInfo.Count = _count;
            _assetInfo.WearAndTearCoeff = _wearAndTearCoeff;
            _assetInfo.StartDate = _startDate;
            _assetInfo.TSRegisterNumber = _tsRegisterNumber;
            _assetInfo.TSDocumentNumber = _tsDocumentNumber;
            _assetInfo.WarrantyPeriodYears = _warrantyPeriodYears;

            if (AssetValuation != null && AssetValuation.IsValid)
            {
                AssetValuation.TotalPrice = _price;
                AssetValuation.Name = _name;
                AssetValuation.SerialNumber = _serialNumber;
                _assetInfo.ValuationData = AssetValuation.ToAssetValuationData();
            }


            return _assetInfo;
        }

        private void UpdateFields()
        {
            _name = _assetInfo.Name;
            _shortName = _assetInfo.ShortName;
            _measurementUnit = _assetInfo.MeasurementUnit;
            _serialNumber = _assetInfo.SerialNumber;
            _nomenclatureCode = _assetInfo.NomenclatureCode;
            _category = _assetInfo.Category;
            _price = _assetInfo.Price;
            _count = _assetInfo.Count;
            _wearAndTearCoeff = _assetInfo.WearAndTearCoeff;
            _startDate = _assetInfo.StartDate;
            _tsRegisterNumber = _assetInfo.TSRegisterNumber;
            _tsDocumentNumber = _assetInfo.TSDocumentNumber;
            _warrantyPeriodYears = _assetInfo.WarrantyPeriodYears;
        }

        private void ProductSelectedExecute()
        {
            FillProductDetails();
        }

        private void OpenAssetValuationPopup()
        {
            if (AssetValuation == null)
            {
                var asset = this.ToAssetInfo();
                AssetValuation = new AssetValuationViewModel(asset, _reportManager, _dataService, _navigationService);
            }

            _navigationService.OpenWindow<AssetValuationDialogWindow, AssetValuationViewModel>(AssetValuation);
        }

        private void FillProductDetails()
        {
            if (SelectedProduct != null)
            {
                Name = SelectedProduct.Name;
                MeasurementUnit = SelectedProduct.MeasurementUnit;
                NomenclatureCode = SelectedProduct.NomenclatureCode;
                Price = SelectedProduct.Price;
                StartDate = SelectedProduct.StartDate;
                WarrantyPeriodYears = SelectedProduct.WarrantyPeriodYears;
            }
            else
            {
                OnPropertyChanged(nameof(Name));
            }
        }
    }
}
