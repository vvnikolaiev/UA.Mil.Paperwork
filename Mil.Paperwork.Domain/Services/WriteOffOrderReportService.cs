using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Domain.Helpers;
using Mil.Paperwork.Domain.Reports;
using Mil.Paperwork.Infrastructure.Services;

namespace Mil.Paperwork.Domain.Services
{
    public class WriteOffOrderReportService : IReportService<IWriteOffOrderReportData>
    {
        private readonly IFileStorageService _fileStorage;
        private readonly IReportDataService _reportDataService;

        public WriteOffOrderReportService(IReportDataService reportDataService, IFileStorageService fileStorage)
        {
            _reportDataService = reportDataService;
            _fileStorage = fileStorage;
        }

        public ReportGenerationResult TryGenerateReport(IWriteOffOrderReportData reportData)
        {
            var report = new WriteOffOrderReport(_reportDataService);
            var outputFiles = new List<string>();

            var created = report.TryCreate(reportData);
            if (created)
            {
                byte[] reportBytes = report.GetReportBytes();
                var outputPath = GetFileName(reportData);
                var savedPath = _fileStorage.SaveFile(outputPath, reportBytes);
                outputFiles.Add(savedPath);
            }

            var result = ReportGenerationResult.FromResult(created, outputFiles);
            return result;
        }

        private static string GetFileName(IWriteOffOrderReportData reportData)
        {
            var destinationPath = reportData.GetDestinationPath();
            var rawFileName = string.Format(WriteOffOrderHelper.OUTPUT_NAME_FORMAT, reportData.ReportNum);
            var fileName = PathsHelper.SanitizeFileName(rawFileName);
            var result = Path.Combine(destinationPath, fileName);
            return result;
        }
    }
}
