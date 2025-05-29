using Mil.Paperwork.Infrastructure.DataModels;
using System.IO;

namespace Mil.Paperwork.Infrastructure.Services
{
    internal class DataService : IDataService
    {
        private const string DataStorageFileName = "Data/SimpleDataStorage.json";

        private readonly IFileStorageService _fileStorageService;

        private SimpleDataStorageDTO _storageData;

        public DataService(IFileStorageService fileStorageService)
        {
            _fileStorageService = fileStorageService;
        }

        public IList<ProductDTO> LoadProductsData()
        {
            var data = LoadDataStorageData();

            return data.ProductsData;
        }

        public IList<AssetValuationData> LoadValuationData()
        {
            var data = LoadDataStorageData();

            return data.ValuationData;
        }

        public void RemoveProductsData(IList<ProductDTO> productsToRemove)
        {
            if (productsToRemove == null || productsToRemove.Count == 0)
                return;

            var namesToRemove = new HashSet<string>(productsToRemove.Select(p => p.AlmostUniqueID));
            _storageData.ProductsData?.RemoveAll(p => namesToRemove.Contains(p.AlmostUniqueID));
            Save();
        }

        public void SaveProductsData(IList<ProductDTO> newProducts)
        {
            var productsDict = _storageData.ProductsData?.ToDictionary(p => p.Name, p => p) ?? [];

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
                    _storageData.ProductsData?.Add(newProduct);
                }
                else
                {
                    existingProduct.ShortName = newProduct.ShortName;
                    existingProduct.StartDate = newProduct.StartDate;
                    existingProduct.Price = newProduct.Price;
                    existingProduct.WarrantyPeriodYears = newProduct.WarrantyPeriodYears;
                    existingProduct.MeasurementUnit = newProduct.MeasurementUnit;
                    existingProduct.NomenclatureCode = newProduct.NomenclatureCode;
                }
            }

            // Sort the list alphabetically by name
            var sortedProducts = _storageData.ProductsData?.OrderBy(p => p.Name).ToList();
            _storageData.ProductsData = sortedProducts ?? [];

            Save();
        }

        public void SaveValuationData(IList<IAssetValuationData?> valuationData)
        {
            if (valuationData == null || valuationData.Count == 0)
            {
                return;
            }

            var valuationDataDict = _storageData.ValuationData?.ToDictionary(p => p.Key, p => p) ?? [];

            // Add new products if they don't already exist
            foreach (var assetValuation in valuationData)
            {
                var valuationKey = assetValuation?.Key;
                if (assetValuation == null || String.IsNullOrEmpty(valuationKey))
                {
                    continue;
                }

                if (!valuationDataDict.TryGetValue(valuationKey, out var existingValuation))
                {
                    if (assetValuation is AssetValuationData value)
                    {
                        valuationDataDict.Add(valuationKey, value);
                        _storageData.ValuationData?.Add(value);
                    }
                }
                else
                {
                    existingValuation.ShortName = assetValuation.ShortName;
                    existingValuation.NomenclatureCode = assetValuation.NomenclatureCode;
                    existingValuation.Description = assetValuation.Description;
                    existingValuation.AssetComponents = assetValuation.AssetComponents;
                    existingValuation.Price = assetValuation.Price;
                }
            }

            // Sort the list alphabetically by name
            var sortedValuationData = _storageData.ValuationData?.OrderBy(p => p.Name).ToList();
            _storageData.ValuationData = sortedValuationData ?? [];

            Save();
        }

        private SimpleDataStorageDTO LoadDataStorageData()
        {
            try
            {
                if (_storageData == null)
                {
                    var data = _fileStorageService.ReadJsonFile<SimpleDataStorageDTO>(DataStorageFileName);
                    _storageData = data ?? new SimpleDataStorageDTO();
                }
            }
            catch (FileNotFoundException)
            {
                _storageData = new SimpleDataStorageDTO();
            }

            return _storageData;
        }

        private void Save()
        {
            _fileStorageService.WriteJsonToFile(_storageData, DataStorageFileName);
        }
    }
}
