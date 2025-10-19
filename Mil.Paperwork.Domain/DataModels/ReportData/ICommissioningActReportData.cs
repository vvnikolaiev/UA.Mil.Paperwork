using Mil.Paperwork.Infrastructure.DataModels;

namespace Mil.Paperwork.Domain.DataModels.ReportData
{
    public interface IProductIdentification
    {
        string SerialNumber { get; set; }
        string InventoryNumber { get; set; }
    }

    public class ProductIdentification : IProductIdentification
    {
        public string SerialNumber { get; set; } = string.Empty;
        public string InventoryNumber { get; set; } = string.Empty;
    }

    public interface ICommissioningActReportData : IReportData
    {
        IProductData Asset { get; set; }

        string AssetState { get; set; }

        IList<IProductIdentification> AssetIds { get; set; }
        
        string CountText { get; set; }
        int Count { get; set; }
        string DocumentNumber { get; set; }
        DateTime DocumentDate { get; set; }
        
        string CommissioningLocation { get; set; }
        string ShortCharacteristic { get; set; }
        string AssetCompliance { get; set; }
        string CompletionState { get; set; }
        string TestResults { get; set; }
        string OtherInfo { get; set; }
        string Conclusion { get; set; }
        string AttachedDocumentation { get; set; }

        IPerson PersonAccepted { get; set; }
        IPerson PersonHanded { get; set; }
    }
}
