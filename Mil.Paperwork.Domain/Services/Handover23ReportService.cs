using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Domain.Helpers;
using Mil.Paperwork.Domain.Reports;
using Mil.Paperwork.Infrastructure.Services;

namespace Mil.Paperwork.Domain.Services
{
    public class Handover23ReportService : IReportService<IHandoverReportData>
    {
        private readonly IFileStorageService _fileStorage;
        private readonly IReportDataService _reportDataService;

        public Handover23ReportService(IReportDataService reportDataService, IFileStorageService fileStorage)
        {
            _reportDataService = reportDataService;
            _fileStorage = fileStorage;
        }

        public ReportGenerationResult TryGenerateReport(IHandoverReportData reportData)
        {
            var report = new Handover23ActReport(_reportDataService);
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

        private string GetFileName(IHandoverReportData reportData)
        {
            var destinationPath = reportData.GetDestinationPath();

            var nameParameters = new List<string>();

            if (!string.IsNullOrEmpty(reportData.DocumentNumber))
            {
                nameParameters.Add($"№{reportData.DocumentNumber}");
            }

            if (!string.IsNullOrEmpty(reportData.Supplier))
            {
                nameParameters.Add($"({reportData.Supplier})");
            }

            var name = nameParameters.Any() ? string.Join(" ", nameParameters) : string.Empty;

            var rawFileName = String.Format(Handover23ActHelper.OUTPUT_REPORT_NAME_TEMPLATE, name);
            var fileName = PathsHelper.SanitizeFileName(rawFileName);
            var outputPath = Path.Combine(destinationPath, fileName);

            return outputPath;
        }
    }
}

