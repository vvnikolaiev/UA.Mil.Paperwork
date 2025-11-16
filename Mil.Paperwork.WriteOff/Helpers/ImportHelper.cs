using Mil.Paperwork.Domain.Services;
using Mil.Paperwork.Infrastructure.Attributes;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.WriteOff.DataModels;
using System.Data;
using System.Reflection;
using System.Windows;

namespace Mil.Paperwork.WriteOff.Helpers
{
    internal static class ImportHelper
    {
        private const string IMPORT_SUCCESSFUL_CAPTION = "Дані імпортовано успішно";
        private const string IMPORT_FAILED_CAPTION = "Помилка імпорту даних";

        private const string IMPORT_SUCCESSFUL_MESSAGE_FORMAT = "Імпортовано {0} рядків.\r\nПропущено {1} рядків через некоректні дані.";
        private const string IMPORT_FAILED_MESSAGE_SIMPLE = "Не вдалося імпортувати дані.";
        private const string IMPORT_FAILED_MESSAGE_FORMAT = "Не вдалося імпортувати дані.\r\nПомилка: {0}";

        public static List<ImportColumnDefinition> GetColumnsToMap(Type type)
        {
            var definitions = type
                .GetProperties()
                .Select(p => p.GetCustomAttribute<ImportColumnAttribute>())
                .Where(attr => attr != null)
                .Select(attr => new ImportColumnDefinition(attr.Title, attr.IsRequired))
                .ToList();

            return definitions;
        }

        public static DataTable ToDataTable<T>(IEnumerable<T> items)
        {
            var dataTable = new DataTable(typeof(T).Name);
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var columns = properties
                .Where(x => x.GetCustomAttribute<ImportColumnAttribute>() != null)
                .Select(prop => new { Prop = prop, Attr = prop.GetCustomAttribute<ImportColumnAttribute>() })
                .ToArray();

            foreach (var col in columns)
            {
                var colTitle = col.Attr.Title ?? col.Prop.Name;
                dataTable.Columns.Add(colTitle, Nullable.GetUnderlyingType(col.Prop.PropertyType) ?? col.Prop.PropertyType);
            }

            foreach (var item in items)
            {
                var values = new object[columns.Length];
                for (int i = 0; i < columns.Length; i++)
                {
                    var prop = columns[i].Prop;
                    values[i] = prop.GetValue(item) ?? DBNull.Value;
                }
                dataTable.Rows.Add(values);
            }

            return dataTable;
        }

        public static List<T> MapRowsToDtos<T>(
            List<ImportColumnDefinition> columnsToMap,
            List<Dictionary<string, object>> excelRows)
            where T : new()
        {
            var result = new List<T>();
            var propsExtended = typeof(T)
                .GetProperties()
                .Select(prop => new { Prop = prop, Attr = prop.GetCustomAttribute<ImportColumnAttribute>() });

            foreach (var row in excelRows)
            {
                var isRowValid = true;

                var dto = new T();
                foreach (var item in propsExtended)
                {
                    var prop = item.Prop;
                    var attr = item.Attr;
                    if (attr == null)
                    {
                        continue;
                    }

                    var column = columnsToMap.FirstOrDefault(c => c.Title == attr.Title);
                    if (column?.SelectedSourceColumn == null) continue;

                    if (row.TryGetValue(column.SelectedSourceColumn, out var value) && value != null)
                    {
                        try
                        {
                            var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                            var converted = Convert.ChangeType(value, targetType);
                            prop.SetValue(dto, converted);
                        }
                        catch
                        {
                            // do not take "broken" lines? or highlight? IValidatable<T> { IsValid; InvalidFields; }
                            isRowValid = false;
                            break;
                        }
                    }
                    else if (attr.IsRequired)
                    {
                        isRowValid = false;
                        break;
                    }
                }

                if (isRowValid)
                {
                    result.Add(dto);
                }
            }

            return result;
        }

        public static string GetImportResultCaption(ImportDataResult importDataResult)
        {
            var caption = importDataResult.IsSuccessful ? IMPORT_SUCCESSFUL_CAPTION : IMPORT_FAILED_CAPTION;
            return caption;
        }

        public static string GetImportResultMessage(ImportDataResult importDataResult)
        {
            string message;
            if (importDataResult.IsSuccessful)
            {
                message = string.Format(IMPORT_SUCCESSFUL_MESSAGE_FORMAT, importDataResult.ImportedRowsCount, importDataResult.InvalidRowsCount);
            }
            else
            {
                message = string.IsNullOrEmpty(importDataResult.ErrorMessage)
                    ? IMPORT_FAILED_MESSAGE_SIMPLE
                    : string.Format(IMPORT_FAILED_MESSAGE_FORMAT, importDataResult.ErrorMessage);
            }

            return message;
        }

        public static DialogIcon GetImportResultIcon(ImportDataResult importDataResult)
        {
            var messageBoxImage = importDataResult.IsSuccessful ? DialogIcon.Information : DialogIcon.Warning;
            return messageBoxImage;
        }
    }
}
