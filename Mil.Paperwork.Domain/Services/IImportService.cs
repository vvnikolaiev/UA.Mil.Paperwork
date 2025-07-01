﻿namespace Mil.Paperwork.Domain.Services
{
    public interface IImportService
    {
        List<string> GetExcelTableHeaders(string filePath, int headerRow = 1);
        List<Dictionary<string, object>> GetExcelRows(string filePath, int headerRow = 1, int maxRowsCount = 0);
    }
}
