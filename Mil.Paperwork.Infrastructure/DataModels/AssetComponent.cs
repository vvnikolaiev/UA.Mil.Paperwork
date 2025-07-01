using Mil.Paperwork.Infrastructure.Attributes;

namespace Mil.Paperwork.Infrastructure.DataModels
{
    public class AssetComponent
    {
        [ImportColumn("Назва", isReqired: true)]
        public string Name { get; set; }

        [ImportColumn("Од вимір")]
        public string Unit { get; set; }

        [ImportColumn("Код номенкл")]
        public string NomenclatureCode { get; set; }

        [ImportColumn("Кількість")]
        public int Quantity { get; set; } = 1;

        [ImportColumn("Категорія")]
        public int Category { get; set; } = 2;

        [ImportColumn("Ціна", isReqired: true)]
        public decimal Price { get; set; }

        public bool Exclude { get; set; }
    }
}
