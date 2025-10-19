using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Domain.Helpers;
using Mil.Paperwork.Domain.Reports;
using Mil.Paperwork.Infrastructure.Services;
using System.IO;

namespace Mil.Paperwork.Domain.Services
{
    public class InvoiceReportService : IReportService<IInvoceReportData>
    {
        private readonly IFileStorageService _fileStorage;
        private readonly IReportDataService _reportDataService;

        public InvoiceReportService(IReportDataService reportDataService, IFileStorageService fileStorage)
        {
            _reportDataService = reportDataService;
            _fileStorage = fileStorage;
        }

        public bool TryGenerateReport(IInvoceReportData reportData)
        {
            var report = new InvoiceReport(_reportDataService);

            var result = report.TryCreate(reportData);
            if (result)
            {
                byte[] reportBytes = report.GetReportBytes();

                var outputPath = GetFileName(reportData);
                _fileStorage.SaveFile(outputPath, reportBytes);
            }

            return result;
        }

        private string GetFileName(IInvoceReportData reportData)
        {
            var destinationPath = reportData.GetDestinationPath();

            var nameParameters = new List<string>();

            if (!string.IsNullOrEmpty(reportData.DocumentNumber))
            {
                nameParameters.Add(reportData.DocumentNumber);
            }

            nameParameters.Add(reportData.DateCreated.ToString(ReportHelper.DATE_FORMAT));

            var name = nameParameters.Any() ? string.Join(",", nameParameters) : string.Empty;

            var rawFileName = String.Format(InvoiceReportHelper.OUTPUT_REPORT_NAME_TEMPLATE, name);
            var fileName = PathsHelper.SanitizeFileName(rawFileName);
            var outputPath = Path.Combine(destinationPath, fileName);

            return outputPath;
        }
    }
}

