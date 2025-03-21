using Mil.Paperwork.Domain.DataModels;
using Mil.Paperwork.Domain.Helpers;
using Mil.Paperwork.Domain.Reports;
using Mil.Paperwork.Infrastructure.Services;
using System.IO;

namespace Mil.Paperwork.Domain.Services
{
    public class AssetDismantlingReportService : IReportService<IDismantlingReportData>
    {
        private readonly IFileStorageService _fileStorage;
        private readonly IReportDataService _reportDataService;

        public AssetDismantlingReportService(IReportDataService reportDataService, IFileStorageService fileStorage)
        {
            _reportDataService = reportDataService;
            _fileStorage = fileStorage;
        }

        public bool TryGenerateReport(IDismantlingReportData reportData)
        {
            var result = false;

            foreach (var assetDismantlingData in reportData.Dismantlings)
            {
                if (assetDismantlingData == null)
                {
                    continue;
                }

                var report = new AssetDismantlingReport(_reportDataService);
                result = report.TryCreate(assetDismantlingData);

                if (result)
                {
                    byte[] reportBytes = report.GetReportBytes();

                    var destinationPath = reportData.GetDestinationPath();
                    // TODO: add short name to the View? Only for the file name
                    var name = assetDismantlingData.SerialNumber;
                    var fileName = String.Format(DismantlingReportHelper.OUTPUT_REPORT_NAME_FORMAT, name);
                    var outputPath = Path.Combine(destinationPath, PathsHelper.SanitizeFileName(fileName));

                    _fileStorage.SaveFile(outputPath, reportBytes);
                }

            }

            return result;
        }
    }
}
