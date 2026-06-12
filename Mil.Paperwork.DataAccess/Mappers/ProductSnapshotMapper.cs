using Mil.Paperwork.DataAccess.DataModels.Snapshots;
using Mil.Paperwork.Infrastructure.DataModels;

namespace Mil.Paperwork.DataAccess.Mappers
{
    internal static class ProductSnapshotMapper
    {
        public static ProductSnapshot? ToSnapshot(IProductData? product)
        {
            if (product == null)
            {
                return null;
            }

            var snapshot = new ProductSnapshot
            {
                Name = product.Name,
                ShortName = product.ShortName,
                MeasurementUnit = product.MeasurementUnit,
                NomenclatureCode = product.NomenclatureCode,
                Price = product.Price,
                StartDate = product.StartDate,
                WarrantyPeriodMonths = product.WarrantyPeriodMonths,
                YearManufactured = product.YearManufactured,
                ResourceYears = product.ResourceYears
            };

            return snapshot;
        }

        public static ProductDTO? ToProduct(ProductSnapshot? snapshot)
        {
            if (snapshot == null)
            {
                return null;
            }

            var product = new ProductDTO
            {
                Name = snapshot.Name,
                ShortName = snapshot.ShortName,
                MeasurementUnit = snapshot.MeasurementUnit,
                NomenclatureCode = snapshot.NomenclatureCode,
                Price = snapshot.Price,
                StartDate = snapshot.StartDate,
                WarrantyPeriodMonths = snapshot.WarrantyPeriodMonths,
                YearManufactured = snapshot.YearManufactured,
                ResourceYears = snapshot.ResourceYears
            };

            return product;
        }
    }
}
