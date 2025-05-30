namespace Mil.Paperwork.Domain.Services
{
    public interface IExportService
    {
        bool TryExportToJson<T>(IEnumerable<T> data, string filePath, string fileNameFormat);
        bool TryExportToExcel<T>(IEnumerable<T> data, string filePath, string fileNameFormat);
    }
}
