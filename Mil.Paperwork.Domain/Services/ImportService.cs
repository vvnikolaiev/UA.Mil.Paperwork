using Mil.Paperwork.Domain.Helpers;
using Mil.Paperwork.Infrastructure.DataModels;
using Mil.Paperwork.Infrastructure.Helpers;
using Mil.Paperwork.Infrastructure.Services;
using OfficeOpenXml;

namespace Mil.Paperwork.Domain.Services
{
    internal class ImportService : IImportService
    {
        private readonly IFileStorageService _fileStorageService;
        private readonly IReportDataService _reportDataService;

        public ImportService(IFileStorageService fileStorageService, IReportDataService reportDataService)
        {
            _fileStorageService = fileStorageService;
            _reportDataService = reportDataService;
        }

        public bool TryImportSettingsConfigFile(string filePath)
        {
            var config = _fileStorageService.ReadJsonFile<ReportDataConfigDTO>(filePath);
            if (config == null || config.Common == null || config.Invoice == null)
            {
                return false;
            }

            var result = _reportDataService.ImportReportConfig(config);

            return result;
        }

        public List<string> GetExcelTableHeaders(string filePath, int headerRow = 1)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage(filePath);

            var worksheet = package.Workbook.Worksheets[0];
            int lastCol = worksheet.Dimension.End.Column;

            var headers = ExcelDocumentHelper.GetHeaders(headerRow, worksheet);

            return headers;
        }

        public List<Dictionary<string, object>> GetExcelRows(string filePath, int headerRow = 1, int maxRowsCount = 0)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage(filePath);
            var worksheet = package.Workbook.Worksheets[0];

            // Read headers
            var headers = ExcelDocumentHelper.GetHeaders(headerRow, worksheet);

            var firstRow = headerRow > 0 ? headerRow + 1 : 1;
            var rows = ExtractExcelRows(worksheet, firstRow, headers, maxRowsCount);

            return rows;
        }

        private static List<Dictionary<string, object>> ExtractExcelRows(ExcelWorksheet worksheet, int firstRow, List<string> headers, int maxRowsCount)
        {
            var rows = new List<Dictionary<string, object>>();

            var lastCol = worksheet.Dimension.End.Column;
            var lastRow = worksheet.Dimension.End.Row;

            if (maxRowsCount > 0 && lastRow - firstRow > maxRowsCount)
            {
                // take preview data
                lastRow = firstRow + maxRowsCount;
            }

            // Read data rows
            for (int row = firstRow; row < lastRow; row++)
            {
                var rowDict = new Dictionary<string, object>();
                bool isEmpty = true;
                for (int col = 1; col <= lastCol; col++)
                {
                    var value = worksheet.Cells[row, col].Value;
                    rowDict[headers[col - 1]] = value;
                    if (value != null && !string.IsNullOrWhiteSpace(value.ToString()))
                    {
                        isEmpty = false;
                    }
                }
                if (!isEmpty)
                {
                    rows.Add(rowDict);
                }
            }

            return rows;
        }
    }
}
