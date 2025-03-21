using System.Xml.Serialization;

namespace Mil.Paperwork.Infrastructure.DataModels
{
    public interface IAssetValuationData
    {
        string Name { get; }
        string SerialNumber { get; }
        string ShortName { get; set; }
        string NomenclatureCode { get; set; }
        decimal TotalPrice { get; set; }
        string Description { get; set; }
        DateTime ValuationDate { get; }
        IList<AssetComponent> AssetComponents { get; set; }

        [XmlIgnore]
        int AssetComponentsCount { get; }
        
        [XmlIgnore]
        string Key { get; }
    }
}
