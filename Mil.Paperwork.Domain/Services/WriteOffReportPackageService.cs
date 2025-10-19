using Mil.Paperwork.Domain.DataModels.Parameters;
using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Domain.Helpers;
using Mil.Paperwork.Domain.Reports.WriteOff;
using Mil.Paperwork.Infrastructure.Services;
using System.IO;

namespace Mil.Paperwork.Domain.Services
{
    public class WriteOffReportPackageService : IReportService<ITechnicalStateReportData>
    {
        private readonly IReportDataService _reportDataService;
        private readonly IFileStorageService _fileStorage;

        public WriteOffReportPackageService(IReportDataService reportDataService, IFileStorageService fileStorage)
        {
            _reportDataService = reportDataService;
            _fileStorage = fileStorage;
        }

        public bool TryGenerateReport(ITechnicalStateReportData reportData)
        {
            var reports = new List<IWriteOffPackageReport>()
            {
                new WriteOffTitlePage(_reportDataService),
                new WriteOffTableOfContents(_reportDataService),
                new WriteOffConsentSheet(_reportDataService),
                new BookOfLossesExtract(_reportDataService)
            };

            var parameters = WriteOffPackageParameters.FromReportData(reportData);

            var globalResult = true;

            foreach (var report in reports)
            {
                var result = report.TryCreate(parameters);

                if (result)
                {
                    SaveReport(report, reportData);
                }

                globalResult &= result;
            }

            return globalResult;
        }

        private void SaveReport(IWriteOffPackageReport report, IReportData reportData)
        {
            byte[] reportBytes = report.GetReportBytes();

            var destinationPath = reportData.GetDestinationPath();
            var fileName = PathsHelper.SanitizeFileName(report.OutputFileName);
            var outputPath = Path.Combine(destinationPath, fileName);
            _fileStorage.SaveFile(outputPath, reportBytes);
        }
    }
}
