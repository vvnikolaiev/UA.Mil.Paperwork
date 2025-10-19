using Mil.Paperwork.Domain.DataModels.Assets;
using Mil.Paperwork.Infrastructure.DataModels;

namespace Mil.Paperwork.WriteOff.Helpers
{
    internal static class DTOConvertionHelper
    {
        public static ProductDTO ConvertToProductDTO(IAssetInfo asset)
        {
            return new ProductDTO
            {
                Name = asset.Name,
                ShortName = asset.ShortName,
                MeasurementUnit = asset.MeasurementUnit,
                NomenclatureCode = asset.NomenclatureCode,
                Price = asset.Price,
                StartDate = asset.StartDate,
                WarrantyPeriodMonths = asset.WarrantyPeriodMonths,
                ResourceYears = asset.ResourceYears
            };
        }
    }
}
