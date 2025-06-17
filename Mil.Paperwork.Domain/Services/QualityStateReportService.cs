using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Domain.Helpers;
using Mil.Paperwork.Domain.Reports;
using Mil.Paperwork.Infrastructure.Services;
using System.IO;

namespace Mil.Paperwork.Domain.Services
{
    public class QualityStateReportService : IReportService<WriteOffReportData>
    {
        private readonly IFileStorageService _fileStorage;
        private readonly IReportDataService _reportDataService;

        public QualityStateReportService(IReportDataService reportDataService, IFileStorageService fileStorage)
        {
            _reportDataService = reportDataService;
            _fileStorage = fileStorage;
        }

        public bool TryGenerateReport(WriteOffReportData reportData)
        {
            var report = new QualityStateReport(_reportDataService);

            var result = report.TryCreate(reportData);
            if (result)
            {
                byte[] reportBytes = report.GetReportBytes();

                var outputPath = GetFileName(reportData);
                _fileStorage.SaveFile(outputPath, reportBytes);
            }

            return result;
        }

        private string GetFileName(WriteOffReportData reportData)
        {
            var destinationPath = reportData.GetDestinationPath();
            var rawFileName = String.Format(QualityStateReportHelper.OUTPUT_REPORT_NAME_TEMPLATE, reportData.DocumentNumber);
            var fileName = PathsHelper.SanitizeFileName(rawFileName);
            var outputPath = Path.Combine(destinationPath, fileName);

            return outputPath;
        }
    }
}

