using Mil.Paperwork.Domain.DataModels.Assets;
using Mil.Paperwork.Domain.Enums;
using Mil.Paperwork.Infrastructure.DataModels;
using Mil.Paperwork.Infrastructure.MVVM;

namespace Mil.Paperwork.WriteOff.ViewModels
{
    public class InvoiceAssetViewModel : AssetViewModel
    {
        private IAssetInfo _assetInfo;

        internal override IAssetInfo AssetInfo => _assetInfo;

        public InvoiceAssetViewModel() : base()
        {
            _assetInfo = new AssetInfo();
        }
    }

    public abstract class WriteOffAssetViewModel : AssetViewModel
    {
        protected EventType eventType = EventType.Lost;
        public EventType EventType
        {
            get => eventType;
            set => SetProperty(ref eventType, value);
        }

        public virtual IAssetInfo ToAssetInfo(EventType eventType = EventType.None)
        {
            var assetInfo = base.ToAssetInfo();
            assetInfo.EventType = eventType;

            return assetInfo;
        }
    }

    public abstract class AssetViewModel : ObservableItem
    {
        private string _selectedProductId = string.Empty;
        private string _name = string.Empty;
        private string _shortName = string.Empty;
        private string _measurementUnit = string.Empty;
        private string _serialNumber = string.Empty;
        private string _nomenclatureCode = string.Empty;
        private int _category = 2;
        private decimal _price = 0;
        private int _count = 1;
        private int _yearManufacured = 1;
        private DateTime _startDate = new DateTime(2023, 01, 01);
        private string _tsRegisterNumber = string.Empty;
        private string _tsDocumentNumber = string.Empty;
        private int _warrantyPeriodMonths = 12;
        private int _resourceYears = 5;

        internal abstract IAssetInfo AssetInfo { get; }

        public string SelectedProductId
        {
            get => _selectedProductId;
            set => SetProperty(ref _selectedProductId, value);
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

        public int WarrantyPeriodMonths
        {
            get => _warrantyPeriodMonths;
            set => SetProperty(ref _warrantyPeriodMonths, value);
        }


        public int YearManufacured
        {
            get => _yearManufacured;
            set => SetProperty(ref _yearManufacured, value);
        }


        public int ResourceYears
        {
            get => _resourceYears;
            set => SetProperty(ref _resourceYears, value);
        }

        public ICommand<ProductDTO> ProductSelectedCommand { get; }

        public AssetViewModel()
        {
            ProductSelectedCommand = new DelegateCommand<ProductDTO>(ProductSelectedExecute);
        }

        public virtual IAssetInfo ToAssetInfo()
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
            AssetInfo.WarrantyPeriodMonths = _warrantyPeriodMonths;
            AssetInfo.YearManufactured = _yearManufacured;
            AssetInfo.ResourceYears = _resourceYears;

            return AssetInfo;
        }

        private void ProductSelectedExecute(ProductDTO product)
        {
            FillProductDetails(product);
        }

        private void FillProductDetails(ProductDTO product)
        {
            if (product != null)
            {
                Name = product.Name;
                MeasurementUnit = product.MeasurementUnit;
                NomenclatureCode = product.NomenclatureCode;
                Price = product.Price;
                StartDate = product.StartDate;
                WarrantyPeriodMonths = product.WarrantyPeriodMonths;
                YearManufacured = product.YearManufactured;
                ResourceYears = product.ResourceYears;
            }
            else
            {
                OnPropertyChanged(nameof(Name));
            }
        }
    }
}
