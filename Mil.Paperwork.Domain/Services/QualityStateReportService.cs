using Mil.Paperwork.Domain.DataModels;
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

                var destinationPath = reportData.GetDestinationPath();
                var fileName = PathsHelper.SanitizeFileName(String.Format(QualityStateReportHelper.OUTPUT_REPORT_NAME_TEMPLATE, reportData.DocumentNumber));
                var outputPath = Path.Combine(destinationPath, fileName);

                _fileStorage.SaveFile(outputPath, reportBytes);
            }

            return result;
        }
    }
}

