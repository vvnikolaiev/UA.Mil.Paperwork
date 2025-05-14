using Mil.Paperwork.Domain.Enums;

namespace Mil.Paperwork.Domain.DataModels.Assets
{
    public interface IAssetInfo
    {
        string Name { get; set; }
        string ShortName { get; set; }
        string MeasurementUnit { get; set; }
        string SerialNumber { get; set; }
        string NomenclatureCode { get; set; }
        // initial category?
        int InitialCategory { get; set; }
        // enum state (lost, destroyed, etc.) => II, V, ... category 
        decimal Price { get; set; }
        int Count { get; set; }
        DateTime StartDate { get; set; }

        string TSRegisterNumber { get; set; }
        string TSDocumentNumber { get; set; }

        DateTime WriteOffDateTime { get; set; }

        EventType EventType { get; set; }

        int WarrantyPeriodYears { get; set; }
        decimal TotalWearCoefficient { get; }

        IList<decimal> GetCoefficients();
    }
}
