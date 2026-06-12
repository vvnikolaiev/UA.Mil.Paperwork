using Mil.Paperwork.Domain.DataModels.Parameters;
using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Domain.Helpers;
using Mil.Paperwork.Domain.Reports.WriteOff;
using Mil.Paperwork.Infrastructure.Services;
using System.IO;

namespace Mil.Paperwork.Domain.Services
{
    public class WriteOffReportPackageService : IReportService<IWriteOffPackageReportData>
    {
        private readonly IReportDataService _reportDataService;
        private readonly IFileStorageService _fileStorage;

        public WriteOffReportPackageService(IReportDataService reportDataService, IFileStorageService fileStorage)
        {
            _reportDataService = reportDataService;
            _fileStorage = fileStorage;
        }

        public ReportGenerationResult TryGenerateReport(IWriteOffPackageReportData reportData)
        {
            var reports = new List<IWriteOffPackageReport>()
            {
                new WriteOffTitlePage(_reportDataService),
                new WriteOffTableOfContents(_reportDataService),
                new WriteOffConsentSheet(_reportDataService),
                new BookOfLossesExtract(_reportDataService)
            };

            var parameters = WriteOffPackageParameters.FromReportData(reportData);

            var globalSuccess = true;
            var outputFiles = new List<string>();

            foreach (var report in reports)
            {
                var created = report.TryCreate(parameters);

                if (created)
                {
                    var savedPath = SaveReport(report, reportData);
                    outputFiles.Add(savedPath);
                }

                globalSuccess &= created;
            }

            var result = ReportGenerationResult.FromResult(globalSuccess, outputFiles);
            return result;
        }

        private string SaveReport(IWriteOffPackageReport report, IReportData reportData)
        {
            byte[] reportBytes = report.GetReportBytes();

            var destinationPath = reportData.GetDestinationPath();
            var fileName = PathsHelper.SanitizeFileName(report.OutputFileName);
            var outputPath = Path.Combine(destinationPath, fileName);
            var savedPath = _fileStorage.SaveFile(outputPath, reportBytes);
            return savedPath;
        }
    }
}
