using Mil.Paperwork.Infrastructure.DataModels;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Mil.Paperwork.Infrastructure.Services
{
    internal class DataService : IDataService
    {
        private const string ProductsDataFileName = "Data/ProductsData.json";

        private readonly IFileStorageService _fileStorageService;

        private IList<ProductDTO> _existingProducts = new List<ProductDTO>();

        public DataService(IFileStorageService fileStorageService)
        {
            _fileStorageService = fileStorageService;
        }

        public IList<ProductDTO> LoadProductsData()
        {
            try
            {
                _existingProducts = _fileStorageService.ReadJsonFile<List<ProductDTO>>(ProductsDataFileName);
            }
            catch (FileNotFoundException)
            {
                _existingProducts = new List<ProductDTO>();
            }

            return _existingProducts;
        }

        public void SaveData(IList<ProductDTO> newProducts)
        {
            var productsDict = _existingProducts.ToDictionary(p => p.Name, p => p);

            // Add new products if they don't already exist
            foreach (var newProduct in newProducts)
            {
                var productName = newProduct.Name;
                if (String.IsNullOrEmpty(productName))
                {
                    continue;
                }

                if (!productsDict.TryGetValue(productName, out var existingProduct))
                {
                    productsDict.Add(productName, newProduct);
                }
                else
                {
                    existingProduct.StartDate = newProduct.StartDate;
                    existingProduct.Price = newProduct.Price;
                    existingProduct.WarrantyPeriodYears = newProduct.WarrantyPeriodYears;
                    existingProduct.MeasurementUnit = newProduct.MeasurementUnit;
                    existingProduct.NomenclatureCode = newProduct.NomenclatureCode;
                }
            }

            // Sort the list alphabetically by name
            var sortedProducts = _existingProducts.OrderBy(p => p.Name).ToList();

            // Save the updated list
            _fileStorageService.WriteJsonToFile(sortedProducts, ProductsDataFileName);
        }
    }
}
