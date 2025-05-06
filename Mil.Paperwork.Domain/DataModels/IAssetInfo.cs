using Mil.Paperwork.Infrastructure.DataModels;

namespace Mil.Paperwork.Domain.DataModels
{
    public interface IAssetInfo
    {
        IAssetValuationData? ValuationData { get; set; }

        string Name { get; set; }
        string ShortName { get; set; }
        string MeasurementUnit { get; set; }
        string SerialNumber { get; set; }
        string NomenclatureCode { get; set; }
        // initial category?
        int Category { get; set; }
        // enum state (lost, destroyed, etc.) => II, V, ... category 
        decimal Price { get; set; }
        int Count { get; set; }
        DateTime StartDate { get; set; }

        string TSRegisterNumber { get; set; }
        string TSDocumentNumber { get; set; }

        DateTime WriteOffDateTime { get; set; }

        int WarrantyPeriodYears { get; set; }
        decimal TotalWearCoefficient { get; }

        IList<decimal> GetCoefficients();
    }
}
