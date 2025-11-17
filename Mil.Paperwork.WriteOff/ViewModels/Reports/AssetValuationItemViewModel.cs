using Mil.Paperwork.Infrastructure.DataModels;
using Mil.Paperwork.Common.MVVM;

namespace Mil.Paperwork.WriteOff.ViewModels.Reports
{
    public class AssetValuationItemViewModel : ObservableItem
    {
        private string _name;
        private string _nomenclatureCode;
        private string _unit;
        private int _quantity = 1;
        private decimal _price;

        private bool _exclude;

        public bool Exclude
        {
            get => _exclude;
            set
            {
                if (SetProperty(ref _exclude, value))
                {
                    AssetComponentExcludedChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string NomenclatureCode
        {
            get => _nomenclatureCode;
            set => SetProperty(ref _nomenclatureCode, value);
        }

        public string MeasurementUnit
        {
            get => _unit;
            set => SetProperty(ref _unit, value);
        }

        public int Quantity
        {
            get => _quantity;
            set => SetProperty(ref _quantity, value);
        }

        public decimal Price
        {
            get => _price;
            set => SetProperty(ref _price, value);
        }

        public virtual bool IsValid => Quantity > 0 && !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(MeasurementUnit);

        public event EventHandler AssetComponentExcludedChanged;

        public AssetValuationItemViewModel() { }

        public AssetValuationItemViewModel(AssetComponent assetComponent) : base()
        {
            Name = assetComponent.Name;
            MeasurementUnit = assetComponent.Unit;
            Quantity = assetComponent.Quantity;
            Price = assetComponent.Price;
            Exclude = assetComponent.Exclude;
            NomenclatureCode = assetComponent.NomenclatureCode;
        }

        public virtual AssetComponent ToAssetComponent()
        {
            var assetComponent = new AssetComponent()
            {
                Name = _name,
                Unit = _unit,
                Quantity = _quantity,
                Price = _price,
                Exclude = _exclude
            };
            return assetComponent;
        }

        internal ProductDTO ToProductDTO()
        {
            var product = new ProductDTO()
            {
                Name = _name,
                MeasurementUnit = _unit,
                NomenclatureCode = _nomenclatureCode,
                Price = _price,
                StartDate = new DateTime(2023, 01, 01)
            };
            return product;
        }

    }
}