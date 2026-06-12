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

        public ReportGenerationResult TryGenerateReport(ITechnicalStateReportData reportData)
        {
            var success = false;
            var outputFiles = new List<string>();

            var assets = reportData.Assets;
            // aggregate assets here if there are duplications in each field but serial number
            var parameters = TechnicalStateReportParameters.FromReportData(reportData);

            foreach (var asset in assets)
            {
                parameters.AssetInfo = asset;

                ITechnicalStateReport report;
                string fileNameFormat;

                report = new TechnicalStateReport(_reportDataService);
                fileNameFormat = TechnicalStateReportHelper.OUTPUT_REPORT_11_NAME_FORMAT;

                success = report.TryCreate(parameters);

                if (success)
                {
                    var fileName = PathsHelper.GetDetailedFileName(asset, fileNameFormat);
                    var savedPath = SaveReport(report, reportData, fileName);
                    outputFiles.Add(savedPath);
                }
            }

            var result = ReportGenerationResult.FromResult(success, outputFiles);
            return result;
        }

        public ReportGenerationResult TryGenerateReport(IInitialTechnicalStateReportData reportData)
        {
            var success = false;
            var outputFiles = new List<string>();

            var assets = reportData.Assets;
            // aggregate assets here if there are duplications in each field but serial number
            foreach (var asset in assets)
            {
                var report = new InitialTechnicalStateReport(_reportDataService);
                success = report.TryCreate(asset, reportData.PersonAccepted, reportData.PersonHanded, reportData.EventType);

                if (success)
                {
                    var fileName = PathsHelper.GetDetailedFileName(asset, TechnicalStateReportHelper.OUTPUT_REPORT_7_NAME_FORMAT);
                    var savedPath = SaveReport(report, reportData, fileName);
                    outputFiles.Add(savedPath);
                }

            }

            var result = ReportGenerationResult.FromResult(success, outputFiles);
            return result;
        }

        private string SaveReport(IReport report, IReportData reportData, string fileName)
        {
            byte[] reportBytes = report.GetReportBytes();

            var destinationPath = reportData.GetDestinationPath();
            var outputPath = Path.Combine(destinationPath, PathsHelper.SanitizeFileName(fileName));

            var savedPath = _fileStorage.SaveFile(outputPath, reportBytes);
            return savedPath;
        }

    }
}
