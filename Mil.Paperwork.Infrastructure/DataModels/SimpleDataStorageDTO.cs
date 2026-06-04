namespace Mil.Paperwork.Infrastructure.DataModels
{
    public class SimpleDataStorageDTO
    {
        public List<ProductDTO> ProductsData { get; set; } = [];
        public List<AssetValuationData> ValuationData { get; set; } = [];
        public List<PersonDTO> PeopleData { get; set; } = [];
        public List<MeasurementUnitDTO> MeasurementUnits { get; set; } = [];
    }
}
