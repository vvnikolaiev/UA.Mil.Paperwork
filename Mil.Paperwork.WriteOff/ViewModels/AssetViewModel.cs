using Mil.Paperwork.Domain.DataModels;
using Mil.Paperwork.Infrastructure.DataModels;
using Mil.Paperwork.Infrastructure.MVVM;
using System.Windows.Input;

namespace Mil.Paperwork.WriteOff.ViewModels
{
    public class AssetViewModel : ObservableItem
    {
        private readonly AssetInfo _assetInfo;

        private ProductDTO _selectedProduct;
        private string _name;
        private string _measurementUnit;
        private string _serialNumber;
        private string _nomenclatureCode;
        private int _category;
        private decimal _price;
        private int _count;
        private decimal _wearAndTearCoeff;
        private DateTime _startDate;
        private string _tsRegisterNumber;
        private string _tsDocumentNumber;
        private int _warrantyPeriodYears;

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

        public ICommand ProductSelectedCommand { get; }

        public AssetViewModel(AssetInfo assetInfo)
        {
            _assetInfo = assetInfo;
            UpdateFields();

            ProductSelectedCommand = new DelegateCommand(ProductSelectedExecute);
        }

        public AssetInfo ToAssetInfo()
        {
            _assetInfo.Name = _name;
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
            return _assetInfo;
        }

        private void UpdateFields()
        {
            _name = _assetInfo.Name;
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
