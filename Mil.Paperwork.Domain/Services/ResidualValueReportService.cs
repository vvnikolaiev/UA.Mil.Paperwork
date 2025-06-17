using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Domain.Helpers;
using Mil.Paperwork.Domain.Reports;
using Mil.Paperwork.Infrastructure.Services;
using System.IO;

namespace Mil.Paperwork.Domain.Services
{
    public class ResidualValueReportService : IReportService<WriteOffReportData>
    {
        private readonly IFileStorageService _fileStorage;
        private readonly IReportDataService _reportDataService;

        public ResidualValueReportService(IReportDataService reportDataService, IFileStorageService fileStorage)
        {
            _reportDataService = reportDataService;
            _fileStorage = fileStorage;
        }

        public bool TryGenerateReport(WriteOffReportData reportData)
        {
            var result = false;

            try
            {
                var report = new ResidualValueReport(_reportDataService);
                if (report.TryCreate(reportData))
                {
                    byte[] reportBytes = report.GetReportBytes();

                    var outputPath = GetOutputReportFilePath(reportData);
                    _fileStorage.SaveFile(outputPath, reportBytes);

                    result = true;
                }

                Console.WriteLine("Дані додано успішно!");
            }
            catch
            {
                result = false;
            }

            return result;
        }

        private string GetOutputReportFilePath(WriteOffReportData reportData)
        {
            var destinationPath = reportData.GetDestinationPath();
            var addParam = (reportData.EventReportNumber ?? 0) > 0
                ? reportData.EventReportNumber.ToString()
                : reportData.ReportDate.ToString("dd-MM-yyyy");
            var fileName = string.Format(ResidualValueReportHelper.OUTPUT_REPORT_NAME_FORMAT, addParam);
            var outputPath = Path.Combine(destinationPath, fileName);

            return outputPath;
        }
    }
}
