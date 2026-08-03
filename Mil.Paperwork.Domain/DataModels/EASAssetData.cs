using Mil.Paperwork.Infrastructure.Attributes;

namespace Mil.Paperwork.Domain.DataModels
{
    public class EASAssetData
    {
        [ImportColumn("Найменування", isRequired: true)]
        public string Name { get; set; } = string.Empty;

        [ImportColumn("Код номенклатури")]
        public string Code { get; set; } = string.Empty;

        [ImportColumn("Од. вим.")]
        public string MeasurementUnit { get; set; } = string.Empty;

        [ImportColumn("Категорія")]
        public int Category { get; set; } = 2;

        [ImportColumn("К-сть")]
        public int Count { get; set; } = 1;

        [ImportColumn("Первісна вартість")]
        public decimal OriginalPrice { get; set; }

        [ImportColumn("Залишкова вартість")]
        public decimal ResidualPrice { get; set; }
    }
}
