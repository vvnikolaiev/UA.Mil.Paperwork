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

        public IList<MeasurementUnitDTO> LoadMeasurementUnitsData()
        {
            var data = LoadDataStorageData();

            return data.MeasurementUnits;
        }

        public IList<PersonDTO> LoadPeopleData()
        {
            var data = LoadDataStorageData();

            return data.PeopleData;
        }

        public void SaveProductsData(IList<ProductDTO> products)
        {
            if (products == null)
                return;

            // Sort the list alphabetically by name
            var sortedProducts = products?.OrderBy(p => p.Name).ToList();
            _storageData.ProductsData = sortedProducts ?? [];

            Save();
        }

        public void RemoveProductsData(IList<ProductDTO> productsToRemove)
        {
            if (productsToRemove == null || productsToRemove.Count == 0)
                return;

            var namesToRemove = new HashSet<string>(productsToRemove.Select(p => p.AlmostUniqueID));
            _storageData.ProductsData?.RemoveAll(p => namesToRemove.Contains(p.AlmostUniqueID));
            Save();
        }

        public void AlterProductsData(IList<ProductDTO> newProducts)
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
                    existingProduct.WarrantyPeriodMonths = newProduct.WarrantyPeriodMonths;
                    existingProduct.ResourceYears = newProduct.ResourceYears;
                    existingProduct.MeasurementUnit = newProduct.MeasurementUnit;
                    existingProduct.NomenclatureCode = newProduct.NomenclatureCode;
                }
            }

            SaveProductsData(_storageData.ProductsData);
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


        public void SaveMeasurementUnitsData(IList<MeasurementUnitDTO> units)
        {
            if (units == null)
                return;

            // Sort the list alphabetically by name
            var sortedUnits = units?.OrderBy(p => p.Name).ToList();
            _storageData.MeasurementUnits = sortedUnits ?? [];

            Save();
        }

        public void AlterPeople(IList<PersonDTO> people)
        {
            var peopleDict = _storageData.PeopleData?.ToDictionary(p => p.FullName, p => p) ?? [];

            // Add new products if they don't already exist
            foreach (var newPerson in people)
            {
                var personName = newPerson.FullName;
                if (String.IsNullOrEmpty(personName))
                {
                    continue;
                }

                if (!peopleDict.TryGetValue(personName, out var existingPerson))
                {
                    peopleDict.Add(personName, newPerson);
                    _storageData.PeopleData?.Add(newPerson);
                }
                else
                {
                    existingPerson.Rank = newPerson.Rank;
                    existingPerson.Position = newPerson.Position;
                }
            }

            SavePeopleData(_storageData.PeopleData);
        }

        public void SavePeopleData(IList<PersonDTO> people)
        {
            if (people == null)
                return;

            // Sort the list alphabetically by name
            var sortedPeople = people?.OrderBy(p => p.FullName).ToList();
            _storageData.PeopleData = sortedPeople ?? [];

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
