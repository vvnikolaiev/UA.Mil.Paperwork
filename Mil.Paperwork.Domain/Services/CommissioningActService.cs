using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Domain.Helpers;
using Mil.Paperwork.Domain.Reports;
using Mil.Paperwork.Infrastructure.Services;
using System.IO;

namespace Mil.Paperwork.Domain.Services
{
    public class CommissioningActService : IReportService<ICommissioningActReportData>
    {
        private readonly IFileStorageService _fileStorage;
        private readonly IReportDataService _reportDataService;

        public CommissioningActService(IReportDataService reportDataService, IFileStorageService fileStorage)
        {
            _reportDataService = reportDataService;
            _fileStorage = fileStorage;
        }

        public bool TryGenerateReport(ICommissioningActReportData reportData)
        {
            var report = new CommissioningActReport(_reportDataService);

            var result = report.TryCreate(reportData);
            if (result)
            {
                byte[] reportBytes = report.GetReportBytes();

                var outputPath = GetFileName(reportData);
                _fileStorage.SaveFile(outputPath, reportBytes);
            }

            return result;
        }

        private string GetFileName(ICommissioningActReportData reportData)
        {
            var destinationPath = reportData.GetDestinationPath();

            var nameParameters = new List<string>();
            if (!string.IsNullOrEmpty(reportData.Asset.ShortName))
            {
                nameParameters.Add(reportData.Asset.ShortName);
            }
            if (!string.IsNullOrEmpty(reportData.DocumentNumber))
            {
                nameParameters.Add(reportData.DocumentNumber);
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