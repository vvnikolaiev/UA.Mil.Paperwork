using Mil.Paperwork.Domain.DataModels;
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
                    var destinationPath = reportData.GetDestinationPath();
                    var outputPath = Path.Combine(destinationPath, ResidualValueReportHelper.OUTPUT_REPORT_NAME);
                    byte[] reportBytes = report.GetReportBytes();

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
    }
}
