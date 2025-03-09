using Mil.Paperwork.Infrastructure.DataModels;

namespace Mil.Paperwork.Infrastructure.Services
{
    public interface IDataService
    {
        IList<ProductDTO> LoadProductsData();

        void SaveData(IList<ProductDTO> products);
    }
}
