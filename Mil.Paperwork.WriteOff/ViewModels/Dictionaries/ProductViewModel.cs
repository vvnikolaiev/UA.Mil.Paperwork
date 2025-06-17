using Mil.Paperwork.Infrastructure.DataModels;
using Mil.Paperwork.Infrastructure.MVVM;

namespace Mil.Paperwork.WriteOff.ViewModels.Dictionaries
{
    public class ProductViewModel : ObservableItem
    {
        private string _name = string.Empty;
        private string _shortName = string.Empty;
        private string _measurementUnit = string.Empty;
        private string _nomenclatureCode = string.Empty;
        private decimal _price = 0;
        private DateTime _startDate = DateTime.Today;
        private int _warrantyPeriodMonths = 12;

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

        public string NomenclatureCode
        {
            get => _nomenclatureCode;
            set => SetProperty(ref _nomenclatureCode, value);
        }

        public decimal Price
        {
            get => _price;
            set => SetProperty(ref _price, value);
        }

        public DateTime StartDate
        {
            get => _startDate;
            set => SetProperty(ref _startDate, value);
        }

        public int WarrantyPeriodMonths
        {
            get => _warrantyPeriodMonths;
            set => SetProperty(ref _warrantyPeriodMonths, value);
        }

        public ProductViewModel() { }

        public ProductViewModel(ProductDTO dto)
        {
            Name = dto.Name;
            ShortName = dto.ShortName;
            MeasurementUnit = dto.MeasurementUnit;
            NomenclatureCode = dto.NomenclatureCode;
            Price = dto.Price;
            StartDate = dto.StartDate;
            WarrantyPeriodMonths = dto.WarrantyPeriodMonths;
        }

        public ProductDTO ToProductDTO()
        {
            return new ProductDTO
            {
                Name = Name,
                ShortName = ShortName,
                MeasurementUnit = MeasurementUnit,
                NomenclatureCode = NomenclatureCode,
                Price = Price,
                StartDate = StartDate,
                WarrantyPeriodMonths = WarrantyPeriodMonths
            };
        }
    }
}
