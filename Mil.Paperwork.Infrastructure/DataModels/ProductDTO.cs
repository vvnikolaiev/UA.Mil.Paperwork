using Mil.Paperwork.Infrastructure.Attributes;

namespace Mil.Paperwork.Infrastructure.DataModels
{
    public class ProductDTO : IProductData
    {
        [ImportColumn("Назва", isReqired: true)]
        public string Name { get; set; }
        
        [ImportColumn("Коротка назва", isReqired: false)]
        public string ShortName { get; set; }

        [ImportColumn("Одиниця виміру", isReqired: true)]
        public string MeasurementUnit { get; set; }

        [ImportColumn("Код номенклатури", isReqired: false)]
        public string NomenclatureCode { get; set; }

        [ImportColumn("Ціна", isReqired: true)]
        public decimal Price { get; set; }

        [ImportColumn("Дата вв експл", isReqired: false)]
        public DateTime StartDate { get; set; }

        [ImportColumn("Гарантія (міс.)", isReqired: false)]
        public int WarrantyPeriodMonths { get; set; } = 12;

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
                return false;

            return x.Name == y.Name;
        }

        public int GetHashCode(ProductDTO obj)
        {
            if (obj == null)
                return 0;

            return HashCode.Combine(obj.Name);
        }
    }
}
