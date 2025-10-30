using Mil.Paperwork.Domain.Enums;
using Mil.Paperwork.Infrastructure.DataModels;
using Mil.Paperwork.Infrastructure.Enums;

namespace Mil.Paperwork.Domain.DataModels.Assets
{
    public class AssetInfo : IAssetInfo
    {
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string MeasurementUnit { get; set; }
        public string SerialNumber { get; set; }
        public string NomenclatureCode { get; set; }
        public int InitialCategory { get; set; } = 2;
        public decimal Price { get; set; }
        public int Count { get; set; } = 1;
        public DateTime StartDate { get; set; } = new DateTime(2023, 01, 01);
        public EventType EventType { get; set; }
        public virtual AssetType Service => AssetType.Default;

        public string TSRegisterNumber { get; set; }
        public string TSDocumentNumber { get; set; }

        public int WarrantyPeriodMonths { get; set; } = 1;
        public int YearManufactured { get; set; } = 2023;
        public int ResourceYears { get; set; } = 5;

        public AssetInfo()
        {
        }

        public AssetInfo(IProductData productData)
        {
            Name = productData.Name;
            Price = productData.Price;
            ShortName = productData.ShortName;
            MeasurementUnit = productData.MeasurementUnit;
            WarrantyPeriodMonths = productData.WarrantyPeriodMonths;
            YearManufactured = productData.YearManufactured;
            ResourceYears = productData.ResourceYears;
            StartDate = productData.StartDate;
            NomenclatureCode = productData.NomenclatureCode;
        }
    }
}