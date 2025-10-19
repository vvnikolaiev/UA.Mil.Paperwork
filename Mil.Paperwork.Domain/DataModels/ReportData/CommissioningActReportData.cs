using Mil.Paperwork.Infrastructure.DataModels;

namespace Mil.Paperwork.Domain.DataModels.ReportData
{
    public class CommissioningActReportData : ICommissioningActReportData
    {
        public string DestinationFolder { get; set; }

        public string DocumentNumber { get; set; } = string.Empty;
        public DateTime DocumentDate { get; set; } = DateTime.Now.Date;
        
        public IProductData Asset { get; set; }
        public string AssetState { get; set; } = "прид.";
        public IList<IProductIdentification> AssetIds { get; set; }

        //public string SerialNumber { get; set; } = string.Empty;
        public string CountText { get; set; } = string.Empty;
        public int Count { get; set; } = 1;
        public string CommissioningLocation { get; set; } = string.Empty;
        public string ShortCharacteristic { get; set; } = string.Empty;
        public string AssetCompliance { get; set; } = "відповідає";
        public string CompletionState { get; set; } = "не потрібна";
        public string TestResults { get; set; } = string.Empty;
        public string OtherInfo { get; set; } = string.Empty;
        public string Conclusion { get; set; } = "ввести в експлуатацію";
        public string AttachedDocumentation { get; set; } = string.Empty;
        public IPerson PersonAccepted { get; set; }
        public IPerson PersonHanded { get; set; }

        public string GetDestinationPath()
        {
            return DestinationFolder;
        }
    }
}
