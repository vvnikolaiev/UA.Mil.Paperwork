using Mil.Paperwork.Domain.DataModels.Assets;
using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Domain.Helpers;
using Mil.Paperwork.Domain.Reports;
using Mil.Paperwork.Infrastructure.Services;
using System.IO;

namespace Mil.Paperwork.Domain.Services
{
    public class ResidualValueReportService : IReportService<IResidualValueReportData>
    {
        private readonly IFileStorageService _fileStorage;
        private readonly IReportDataService _reportDataService;

        public ResidualValueReportService(IReportDataService reportDataService, IFileStorageService fileStorage)
        {
            _reportDataService = reportDataService;
            _fileStorage = fileStorage;
        }

        public bool TryGenerateReport(IResidualValueReportData reportData)
        {
            var result = true;

            try
            {
                var report = new ResidualValueReport(_reportDataService);

                foreach (var asset in reportData.Assets)
                {
                    var residualReportData = new ResidualValueReportData();

                    result &= report.TryCreate(asset, reportData.MetalCosts, reportData.AssetType, reportData.EventDate);

                    if (result)
                    {
                        byte[] reportBytes = report.GetReportBytes();

                        var outputPath = GetOutputReportFilePath(asset, reportData);
                        _fileStorage.SaveFile(outputPath, reportBytes);
                    }
                }

                Console.WriteLine("Дані додано успішно!");
            }
            catch
            {
                result = false;
            }

            return result;
        }

        private string GetOutputReportFilePath(IAssetInfo asset, IResidualValueReportData reportData)
        {
            const int MAX_NAME_LENGTH = 30;
            var destinationPath = reportData.GetDestinationPath();
            var addParam = (reportData.EventReportNumber ?? 0) > 0
                ? $"Рапорт №{reportData.EventReportNumber}"
                : reportData.EventDate.ToString("dd-MM-yyyy");


            var serialNumber = string.IsNullOrEmpty(asset.SerialNumber) ? string.Empty: $"{asset.SerialNumber}";
            var shortName = string.IsNullOrEmpty(asset.ShortName) ? serialNumber : $"{asset.ShortName} {serialNumber}";

            var name = string.IsNullOrEmpty(shortName) ? asset.Name : shortName;

            name = name.Length > MAX_NAME_LENGTH ? name.Substring(0, MAX_NAME_LENGTH) : name;

            var rawFileName = string.Format(ResidualValueReportHelper.OUTPUT_REPORT_NAME_FORMAT, $"{addParam}, {name}");

            var fileName = PathsHelper.SanitizeFileName(rawFileName);
            var outputPath = Path.Combine(destinationPath, fileName);

            return outputPath;
        }
    }
}
