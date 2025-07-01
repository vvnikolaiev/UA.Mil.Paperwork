using Mil.Paperwork.Domain.Helpers;
using Mil.Paperwork.Infrastructure.Attributes;
using Mil.Paperwork.Infrastructure.Helpers;
using Mil.Paperwork.Infrastructure.Services;
using OfficeOpenXml;
using System.IO;
using System.Reflection;
using System.Text;

namespace Mil.Paperwork.Domain.Services
{
    public class ImportColumnDefinition
    {
        public string Title { get; }
        public bool IsRequired { get; }
        public string? SelectedSourceColumn { get; set; }

        public bool ImportColumn { get; set; } = true;

        public ImportColumnDefinition(string title, bool isRequired)
        {
            Title = title;
            IsRequired = isRequired;
        }
    }

    internal class ExportService : IExportService
    {
        private readonly IFileStorageService _fileStorage;

        public ExportService(IFileStorageService fileStorage)
        {
            _fileStorage = fileStorage;
        }

        public bool TryExportToJson<T>(IEnumerable<T> data, string folderName, string fileNameFormat)
        {
            try
            {
                var fileName = string.Format(fileNameFormat, DateTime.Now.ToString(ExportHelper.DATE_FORMAT));
                var filePath = Path.Combine(folderName, fileName);

                var json = JsonHelper.WriteJson(data);
                var reportBytes = Encoding.UTF8.GetBytes(json);

                _fileStorage.SaveFile(filePath, reportBytes);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool TryExportToExcel<T>(IEnumerable<T> data, string folderName, string fileNameFormat)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            try
            {
                using var package = new ExcelPackage();
                var worksheet = package.Workbook.Worksheets.Add("Export");

                // Write headers
                var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                for (int i = 0; i < properties.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = properties[i].Name;
                }

                // Write data
                int row = 2;
                foreach (var item in data)
                {
                    for (int col = 0; col < properties.Length; col++)
                    {
                        var value = properties[col].GetValue(item);
                        if (value is DateTime dt)
                        {
                            value = dt.ToString(ReportHelper.DATE_FORMAT);
                        }

                        worksheet.Cells[row, col + 1].Value = value;
                    }
                    row++;
                }

                var fileName = string.Format(fileNameFormat, DateTime.Now.ToString(ExportHelper.DATE_FORMAT));
                fileName = PathsHelper.SanitizeFileName(fileName);
                var filePath = Path.Combine(folderName, fileName);

                using var reportStream = new MemoryStream();
                package.SaveAs(reportStream);
                var reportBytes = reportStream.ToArray();

                _fileStorage.SaveFile(filePath, reportBytes);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
