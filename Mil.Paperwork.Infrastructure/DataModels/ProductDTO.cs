namespace Mil.Paperwork.Infrastructure.DataModels
{
    public class ProductDTO : IProductData
    {
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string MeasurementUnit { get; set; }
        public string NomenclatureCode { get; set; }
        public decimal Price { get; set; }
        public DateTime StartDate { get; set; }
        public int WarrantyPeriodMonths { get; set; }

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
