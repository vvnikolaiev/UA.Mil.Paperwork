using Mil.Paperwork.Domain.DataModels;
using Mil.Paperwork.Domain.Helpers;
using Mil.Paperwork.Domain.Reports;
using Mil.Paperwork.Infrastructure.Services;
using System.IO;

namespace Mil.Paperwork.Domain.Services
{
    public class TechnicalStateReportService : IReportService<ITechnicalStateReportData>
    {
        private readonly IReportDataService _reportDataService;
        private readonly IFileStorageService _fileStorage;

        public TechnicalStateReportService(IReportDataService reportDataService, IFileStorageService fileStorage)
        {
            _reportDataService = reportDataService;
            _fileStorage = fileStorage;
        }

        public bool TryGenerateReport(ITechnicalStateReportData reportData)
        {
            var result = false;

            var assets = reportData.Assets;
            foreach (var asset in assets)
            {
                var report = new TechnicalStateReport(_reportDataService);
                result = report.TryCreate(asset, reportData.ReportDate, reportData.Reason);

                if (result)
                {
                    byte[] reportBytes = report.GetReportBytes();

                    var destinationPath = reportData.GetDestinationPath();
                    var name = String.IsNullOrEmpty(asset.SerialNumber) ? asset.TSDocumentNumber : $"{asset.TSDocumentNumber},{asset.SerialNumber}";
                    var fileName = String.Format(TechnicalStateReportHelper.OUTPUT_REPORT_NAME_FORMAT, name);
                    var outputPath = Path.Combine(destinationPath, PathsHelper.SanitizeFileName(fileName));

                    _fileStorage.SaveFile(outputPath, reportBytes);
                }

            }

            return result;
        }
    }
}
