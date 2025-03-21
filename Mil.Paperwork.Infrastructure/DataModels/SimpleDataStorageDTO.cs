namespace Mil.Paperwork.Infrastructure.DataModels
{
    public class SimpleDataStorageDTO
    {
        public List<ProductDTO> ProductsData { get; set; } = [];
        public List<AssetValuationData> ValuationData { get; set; } = [];
    }
}
