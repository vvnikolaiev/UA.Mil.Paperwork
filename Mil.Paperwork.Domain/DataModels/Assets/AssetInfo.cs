using Mil.Paperwork.Domain.Enums;
using Mil.Paperwork.Infrastructure.DataModels;

namespace Mil.Paperwork.Domain.DataModels.Assets
{

    public abstract class AssetInfo : IAssetInfo
    {
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string MeasurementUnit { get; set; }
        public string SerialNumber { get; set; }
        public string NomenclatureCode { get; set; }
        public int InitialCategory { get; set; } = 2;
        public decimal Price { get; set; }
        public int Count { get; set; } = 1;
        public DateTime StartDate { get; set; } = new DateTime(2023, 01, 01);

        public DateTime? WriteOffDateTime { get; set; }
        public EventType EventType { get; set; }

        public string TSRegisterNumber { get; set; }
        public string TSDocumentNumber { get; set; }

        public int WarrantyPeriodMonths { get; set; } = 1;

        public abstract decimal TotalWearCoefficient { get; }

        // https://zakon.rada.gov.ua/laws/show/759-98-%D0%BF#n330
        public abstract IList<decimal> GetCoefficients();
    }
}