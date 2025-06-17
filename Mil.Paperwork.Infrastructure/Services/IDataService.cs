using Mil.Paperwork.Infrastructure.DataModels;

namespace Mil.Paperwork.Infrastructure.Services
{
    public interface IDataService
    {
        IList<ProductDTO> LoadProductsData();
        IList<AssetValuationData> LoadValuationData();
        IList<MeasurementUnitDTO> LoadMeasurementUnitsData();
        IList<PersonDTO> LoadPeopleData();

        void SaveProductsData(IList<ProductDTO> products);
        void AlterProductsData(IList<ProductDTO> products);
        void RemoveProductsData(IList<ProductDTO> products);
        void SaveValuationData(IList<IAssetValuationData?> valuationData); 
        
        void SaveMeasurementUnitsData(IList<MeasurementUnitDTO> units);
        void SavePeopleData(IList<PersonDTO> people);
        void AlterPeople(IList<PersonDTO> people);
    }
}
