using Mil.Paperwork.Domain.DataModels;
using Mil.Paperwork.Domain.Helpers;
using Mil.Paperwork.Domain.Reports;
using Mil.Paperwork.Infrastructure.DataModels;
using Mil.Paperwork.Infrastructure.Services;
using System.IO;

namespace Mil.Paperwork.Domain.Services
{
    public class AssetValuationReportService : IReportService<IAssetValuationReportData>
    {
        private readonly IFileStorageService _fileStorage;
        private readonly IReportDataService _reportDataService;

        public AssetValuationReportService(IReportDataService reportDataService, IFileStorageService fileStorage)
        {
            _reportDataService = reportDataService;
            _fileStorage = fileStorage;
        }

        public bool TryGenerateReport(IAssetValuationReportData reportData)
        {
            var result = false;

            var valuationData = reportData.ValuationData;

            foreach (var assetValuationData in valuationData)
            {
                if (assetValuationData == null)
                {
                    continue;
                }

                var report = new AssetValuationReport(_reportDataService);
                result = report.TryCreate(assetValuationData);

                if (result)
                {
                    byte[] reportBytes = report.GetReportBytes();

                    var destinationPath = reportData.GetDestinationPath();
                    // TODO: add short name to the View? Only for the file name
                    var fileName = GetFileName(assetValuationData, ValuationReportHelper.OUTPUT_REPORT_NAME_FORMAT);
                    var outputPath = Path.Combine(destinationPath, PathsHelper.SanitizeFileName(fileName));

                    _fileStorage.SaveFile(outputPath, reportBytes);
                }

            }

            return result;
        }

        private string GetFileName(IAssetValuationData assetData, string fileNameFormat)
        {
            var name = String.IsNullOrEmpty(assetData.ShortName) ? assetData.SerialNumber : $"{assetData.ShortName} {assetData.SerialNumber}";

            var fileName = String.Format(fileNameFormat, name);

            return fileName;
        }
    }
}
