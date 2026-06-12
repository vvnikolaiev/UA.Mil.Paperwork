using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Domain.Helpers;
using Mil.Paperwork.Domain.Reports;
using Mil.Paperwork.Infrastructure.Services;

namespace Mil.Paperwork.Domain.Services
{
    public class QualityStateReportService : IReportService<ICommonWriteOffReportData>
    {
        private readonly IFileStorageService _fileStorage;
        private readonly IReportDataService _reportDataService;

        public QualityStateReportService(IReportDataService reportDataService, IFileStorageService fileStorage)
        {
            _reportDataService = reportDataService;
            _fileStorage = fileStorage;
        }

        public ReportGenerationResult TryGenerateReport(ICommonWriteOffReportData reportData)
        {
            var report = new QualityStateReport(_reportDataService);

            var created = report.TryCreate(reportData);
            var rawFileName = String.Format(QualityStateReportHelper.OUTPUT_REPORT_NAME_TEMPLATE, reportData.DocumentNumber);
            var savedPath = SaveReport(report, reportData, rawFileName);

            var result = ReportGenerationResult.FromResult(created, [savedPath]);
            return result;
        }

        private string SaveReport(IReport report, IReportData reportData, string fileName)
        {
            byte[] reportBytes = report.GetReportBytes();

            var destinationPath = reportData.GetDestinationPath();
            var outputPath = Path.Combine(destinationPath, PathsHelper.SanitizeFileName(fileName));

            var savedPath = _fileStorage.SaveFile(outputPath, reportBytes);
            return savedPath;
        }
    }

    public class WriteOffActReportService : IReportService<ICommonWriteOffReportData>
    {
        private readonly IFileStorageService _fileStorage;
        private readonly IReportDataService _reportDataService;

        public WriteOffActReportService(IReportDataService reportDataService, IFileStorageService fileStorage)
        {
            _reportDataService = reportDataService;
            _fileStorage = fileStorage;
        }

        public ReportGenerationResult TryGenerateReport(ICommonWriteOffReportData reportData)
        {
            var writeOffReport = new WriteOffActReport(_reportDataService);
            var created = writeOffReport.TryCreate(reportData);

            var fileName = String.Format(TechnicalStateReportHelper.OUTPUT_WRITE_OFF_ACT_NAME_FORMAT, reportData.DocumentNumber);
            var savedPath = SaveReport(writeOffReport, reportData, fileName);

            var result = ReportGenerationResult.FromResult(created, [savedPath]);
            return result;
        }

        private string SaveReport(IReport report, IReportData reportData, string fileName)
        {
            byte[] reportBytes = report.GetReportBytes();

            var destinationPath = reportData.GetDestinationPath();
            var outputPath = Path.Combine(destinationPath, PathsHelper.SanitizeFileName(fileName));

            var savedPath = _fileStorage.SaveFile(outputPath, reportBytes);
            return savedPath;
        }
    }
}

