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
    public abstract class AssetViewModel : ObservableItem
    {
        private readonly ReportManager _reportManager;
        private readonly IDataService _dataService;
        private readonly INavigationService _navigationService;

        private ProductDTO _selectedProduct;
        private string _name = string.Empty;
        private string _shortName = string.Empty;
        private string _measurementUnit = string.Empty;
        private string _serialNumber = string.Empty;
        private string _nomenclatureCode = string.Empty;
        private int _category = 2;
        private decimal _price = 0;
        private int _count = 1;
        private DateTime _startDate = new DateTime(2023, 01, 01);
        private string _tsRegisterNumber = string.Empty;
        private string _tsDocumentNumber = string.Empty;
        private int _warrantyPeriodYears = 5;
        private AssetValuationViewModel _assetValuation;

        internal abstract IAssetInfo AssetInfo { get; }

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

            ProductSelectedCommand = new DelegateCommand(ProductSelectedExecute);
            OpenAssetValuationPopupCommand = new DelegateCommand(OpenAssetValuationPopup);
        }

        public virtual IAssetInfo ToAssetInfo(DateTime? reportDate = null)
        {
            AssetInfo.Name = _name;
            AssetInfo.ShortName = _shortName;
            AssetInfo.MeasurementUnit = _measurementUnit;
            AssetInfo.SerialNumber = _serialNumber;
            AssetInfo.NomenclatureCode = _nomenclatureCode;
            AssetInfo.InitialCategory = _category;
            AssetInfo.Price = _price;
            AssetInfo.Count = _count;
            AssetInfo.StartDate = _startDate;
            AssetInfo.TSRegisterNumber = _tsRegisterNumber;
            AssetInfo.TSDocumentNumber = _tsDocumentNumber;
            AssetInfo.WarrantyPeriodYears = _warrantyPeriodYears;

            if (reportDate != null)
            {
                AssetInfo.WriteOffDateTime = (DateTime)reportDate;
            }

            if (AssetValuation != null && AssetValuation.IsValid)
            {
                AssetValuation.Price = _price;
                AssetValuation.Name = _name;
                AssetValuation.SerialNumber = _serialNumber;
                AssetInfo.ValuationData = AssetValuation.ToAssetValuationData();
            }


            return AssetInfo;
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
