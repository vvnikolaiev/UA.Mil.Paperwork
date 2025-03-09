using Mil.Paperwork.Domain.DataModels;
using Mil.Paperwork.Domain.Services;
using Mil.Paperwork.Infrastructure.Services;
using System.DirectoryServices;
using System.Windows;

namespace Mil.Paperwork.WriteOff.Managers
{
    public class ReportManager
    {
        private readonly IReportService _qualityStateReport;
        private readonly IReportService _technicalStateReport;
        private readonly IReportService _residualValue;

        private readonly IFileStorageService _fileStorageService;
        private readonly IDataService _dataService;

        public ReportManager(
            QualityStateReportService qualityStateReport,
            TechnicalStateReportService technicalStateReport,
            ResidualValueReportService residualValue,
            IFileStorageService fileStorageService,
            IDataService dataService)
        {
            _qualityStateReport = qualityStateReport;
            _technicalStateReport = technicalStateReport;
            _residualValue = residualValue;
            _fileStorageService = fileStorageService;
            _dataService = dataService;
        }

        public void GenerateWriteOffReport(WriteOffReportData reportData)
        {
            var qualityStateReportResult = _qualityStateReport.TryGenerateReport(reportData);
            var technicalStateReportResult = _technicalStateReport.TryGenerateReport(reportData);
            var residualValueReportResult = _residualValue.TryGenerateReport(reportData);

            string qualityStateReportResultStatus, technicalStateReportResultStatus, residualValueReportResultStatus;

            qualityStateReportResultStatus = qualityStateReportResult ? "Акт зміни якісного стану сформовано успішно." : "Не вдалося сформувати Акт зміни якісного стану.";
            technicalStateReportResultStatus = technicalStateReportResult ? "Акт технічного стану сформовано успішно." : "Не вдалося сформувати Акт технічного стану.";
            residualValueReportResultStatus = residualValueReportResult ? "Відомість залишкової вартості сформована успішно." : "Не вдалося сформувати Відомість залишкової вартості.";
            
            var message = $"{residualValueReportResultStatus}\n{technicalStateReportResultStatus}\n{qualityStateReportResultStatus}";

            MessageBox.Show(message);
        }
    }
}
