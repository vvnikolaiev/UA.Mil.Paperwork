using Mil.Paperwork.Domain.DataModels;
using Mil.Paperwork.Domain.DataModels.Assets;
using Mil.Paperwork.Domain.Helpers;
using Mil.Paperwork.Domain.Reports;
using Mil.Paperwork.Infrastructure.Services;
using System.IO;

namespace Mil.Paperwork.Domain.Services
{
    public class TechnicalStateReportService : IReportService<ITechnicalStateReportData>, IReportService<IInitialTechnicalStateReportData>
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
            // aggregate assets here if there are duplications in each field but serial number
            foreach (var asset in assets)
            {
                var report = new TechnicalStateReport(_reportDataService);
                result = report.TryCreate(asset, reportData.EventType, reportData.ReportDate, reportData.Reason);

                if (result)
                {
                    var fileName = GetFileName(asset, TechnicalStateReportHelper.OUTPUT_REPORT_11_NAME_FORMAT);
                    SaveReport(report, reportData, fileName);
                }

            }

            return result;
        }

        public bool TryGenerateReport(IInitialTechnicalStateReportData reportData)
        {
            var result = false;

            var assets = reportData.Assets;
            // aggregate assets here if there are duplications in each field but serial number
            foreach (var asset in assets)
            {
                var report = new InitialTechnicalStateReport(_reportDataService);
                result = report.TryCreate(asset, reportData.EventType);

                if (result)
                {
                    var fileName = GetFileName(asset, TechnicalStateReportHelper.OUTPUT_REPORT_7_NAME_FORMAT);
                    SaveReport(report, reportData, fileName);
                }

            }

            return result;
        }

        private string GetFileName(IAssetInfo asset, string fileNameFormat)
        {
            var name = String.IsNullOrEmpty(asset.SerialNumber) ? asset.TSDocumentNumber : $"{asset.TSDocumentNumber},{asset.SerialNumber}";
            var fileName = String.Format(fileNameFormat, name);

            return fileName;
        }

        private void SaveReport(IReport report, IReportData reportData, string fileName)
        {
            byte[] reportBytes = report.GetReportBytes();

            var destinationPath = reportData.GetDestinationPath();
            var outputPath = Path.Combine(destinationPath, PathsHelper.SanitizeFileName(fileName));

            _fileStorage.SaveFile(outputPath, reportBytes);
        }

    }
}
