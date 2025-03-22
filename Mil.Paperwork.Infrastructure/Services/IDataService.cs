using Mil.Paperwork.Infrastructure.DataModels;

namespace Mil.Paperwork.Infrastructure.Services
{
    public interface IDataService
    {
        IList<ProductDTO> LoadProductsData();
        IList<AssetValuationData> LoadValuationData();

        void SaveProductsData(IList<ProductDTO> products);
        void SaveValuationData(IList<IAssetValuationData?> valuationData);
    }
}
