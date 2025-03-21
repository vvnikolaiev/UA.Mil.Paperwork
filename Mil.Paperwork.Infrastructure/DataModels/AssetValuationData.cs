using System.Xml.Serialization;

namespace Mil.Paperwork.Infrastructure.DataModels
{
    public class AssetValuationData : IAssetValuationData
    {
        //"VALUATION_SOURCES": "з урахуванням закупочних цін офіційного сайту ТОВ ДОЛЯ І КО. ЛТД (сайт: https://dolya.ua/ua/), та наявною в мережі \"Інтернет\" інформацією про вартість і опис аналогічних об'єктів",

        public string Name { get; set; }

        public string SerialNumber { get; set; }

        public string ShortName { get; set; }
        public string NomenclatureCode { get; set; }

        public decimal TotalPrice { get; set; }
        public string Description { get; set; }

        public  DateTime ValuationDate { get; set; } = new DateTime(2023, 01, 01);

        public IList<AssetComponent> AssetComponents { get; set; }

        [XmlIgnore]
        public int AssetComponentsCount => AssetComponents?.Count ?? 0;

        [XmlIgnore]
        public string Key => $"{Name}{ValuationDate:ddMMyyyy}";
    }
}
