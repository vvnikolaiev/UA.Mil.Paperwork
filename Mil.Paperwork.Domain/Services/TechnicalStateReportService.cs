using Mil.Paperwork.Domain.DataModels.Parameters;
using Mil.Paperwork.Domain.DataModels.ReportData;
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
            var parameters = TechnicalStateReportParameters.FromReportData(reportData);

            foreach (var asset in assets)
            {
                parameters.AssetInfo = asset;

                ITechnicalStateReport report;
                string fileNameFormat;

                // TODO: figure out a better way to choose report type
                if (reportData.GenerateWriteOffActs && string.IsNullOrEmpty(asset.SerialNumber))
                {
                    report = new WriteOffActReport(_reportDataService);
                    fileNameFormat = TechnicalStateReportHelper.OUTPUT_WRITE_OFF_ACT_NAME_FORMAT;
                }
                else
                {
                    report = new TechnicalStateReport(_reportDataService);
                    fileNameFormat = TechnicalStateReportHelper.OUTPUT_REPORT_11_NAME_FORMAT;
                }

                result = report.TryCreate(parameters);

                if (result)
                {
                    var fileName = PathsHelper.GetDetailedFileName(asset, fileNameFormat);
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
                result = report.TryCreate(asset, reportData.PersonAccepted, reportData.PersonHanded, reportData.EventType);

                if (result)
                {
                    var fileName = PathsHelper.GetDetailedFileName(asset, TechnicalStateReportHelper.OUTPUT_REPORT_7_NAME_FORMAT);
                    SaveReport(report, reportData, fileName);
                }

            }

            return result;
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
