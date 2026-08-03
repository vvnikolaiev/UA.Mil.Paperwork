using Mil.Paperwork.Domain.Enums;

namespace Mil.Paperwork.Domain.DataModels.ReportData
{
    public class WriteOffPackageTabData : IReportData
    {
        public WriteOffPackageReportData PackageData { get; set; } = new();

        public string Reason { get; set; } = string.Empty;

        public EventType EventType { get; set; }

        public bool GenerateWriteOffPackage { get; set; } = true;

        public bool GenerateWriteOffActs { get; set; } = true;

        public string WriteOffRegNumber { get; set; } = string.Empty;

        public string WriteOffDocNumber { get; set; } = string.Empty;

        public bool GenerateQualityStateReportInstead { get; set; }

        public string QSRRegNumber { get; set; } = string.Empty;

        public string QSRDocNumber { get; set; } = string.Empty;

        public string DestinationFolder => PackageData.DestinationFolder;

        public string GetDestinationPath()
        {
            var destinationPath = PackageData.GetDestinationPath();
            return destinationPath;
        }
    }
}
