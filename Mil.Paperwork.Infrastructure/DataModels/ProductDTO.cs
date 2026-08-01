using Mil.Paperwork.Infrastructure.Attributes;

namespace Mil.Paperwork.Infrastructure.DataModels
{
    public class ProductDTO : IProductData
    {
        [ImportColumn("Назва", isRequired: true)]
        public string Name { get; set; }
        
        [ImportColumn("Коротка назва", isRequired: false)]
        public string ShortName { get; set; }

        [ImportColumn("Одиниця виміру", isRequired: true)]
        public string MeasurementUnit { get; set; }

        [ImportColumn("Код номенклатури", isRequired: false)]
        public string NomenclatureCode { get; set; }

        [ImportColumn("Ціна", isRequired: true)]
        public decimal Price { get; set; }

        [ImportColumn("Дата вв експл", isRequired: false)]
        public DateTime StartDate { get; set; }

        [ImportColumn("Гарантія (міс.)", isRequired: false)]
        public int WarrantyPeriodMonths { get; set; } = 12;

        [ImportColumn("Рік виробництва", isRequired: false)]
        public int YearManufactured { get; set; }

        [ImportColumn("Ресурс (років)", isRequired: false)]
        public int ResourceYears { get; set; }

        public string AlmostUniqueID => $"{Name}{MeasurementUnit}{Price}{NomenclatureCode}";

        public override string ToString()
        {
            return Name;
        }
    }

    public class ProductComparer : IEqualityComparer<ProductDTO>
    {
        public bool Equals(ProductDTO x, ProductDTO y)
        {
            if (x == null || y == null)
            {
                return false;
            }

            var result = x.Name == y.Name;
            return result;
        }

        public int GetHashCode(ProductDTO obj)
        {
            if (obj == null)
            {
                return 0;
            }

            var result = HashCode.Combine(obj.Name);
            return result;
        }
    }
}
