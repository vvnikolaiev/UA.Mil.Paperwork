using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Domain.Helpers;
using Mil.Paperwork.Domain.Reports;
using Mil.Paperwork.Infrastructure.Services;
using System.IO;

namespace Mil.Paperwork.Domain.Services
{
    public class CommissioningActService : IReportService<ICommissioningActReportData>, IReportService<IList<ICommissioningActReportData>>
    {
        private readonly IFileStorageService _fileStorage;
        private readonly IReportDataService _reportDataService;

        public CommissioningActService(IReportDataService reportDataService, IFileStorageService fileStorage)
        {
            _reportDataService = reportDataService;
            _fileStorage = fileStorage;
        }

        public ReportGenerationResult TryGenerateReport(IList<ICommissioningActReportData> reportData)
        {
            var success = true;
            var outputFiles = new List<string>();

            foreach (var data in reportData)
            {
                var actResult = TryGenerateReport(data);
                success &= actResult.Success;
                outputFiles.AddRange(actResult.OutputFiles);
            }

            var result = ReportGenerationResult.FromResult(success, outputFiles);
            return result;
        }

        public ReportGenerationResult TryGenerateReport(ICommissioningActReportData reportData)
        {
            var report = new CommissioningActReport(_reportDataService);
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

        private string GetFileName(ICommissioningActReportData reportData)
        {
            var destinationPath = reportData.GetDestinationPath();

            var nameParameters = new List<string>();
            if (!string.IsNullOrEmpty(reportData.DocumentNumber))
            {
                nameParameters.Add($"№{reportData.DocumentNumber}");
            }
            if (!string.IsNullOrEmpty(reportData.Asset.ShortName))
            {
                nameParameters.Add(reportData.Asset.ShortName);
            }
            var numbers = string.Join(", ", reportData?.AssetIds?.Select(x => x.SerialNumber) ?? []);
            if (!string.IsNullOrEmpty(numbers))
            {
                nameParameters.Add(numbers);
            }

            var name = nameParameters.Any() ? string.Join(",", nameParameters) : string.Empty;

            var rawFileName = String.Format(CommissioningActHelper.OUTPUT_REPORT_NAME_TEMPLATE, name);
            var fileName = PathsHelper.SanitizeFileName(rawFileName);
            var outputPath = Path.Combine(destinationPath, fileName);

            return outputPath;
        }
    }
}