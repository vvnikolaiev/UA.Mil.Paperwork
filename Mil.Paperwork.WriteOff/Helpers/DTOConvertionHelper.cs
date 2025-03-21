﻿using Mil.Paperwork.Domain.DataModels;
using Mil.Paperwork.Infrastructure.DataModels;

namespace Mil.Paperwork.WriteOff.Helpers
{
    internal static class DTOConvertionHelper
    {
        public static ProductDTO ConvertToProductDTO(AssetInfo asset)
        {
            return new ProductDTO
            {
                Name = asset.Name,
                ShortName = asset.ShortName,
                MeasurementUnit = asset.MeasurementUnit,
                NomenclatureCode = asset.NomenclatureCode,
                Price = asset.Price,
                StartDate = asset.StartDate,
                WarrantyPeriodYears = asset.WarrantyPeriodYears
            };
        }
    }
}
