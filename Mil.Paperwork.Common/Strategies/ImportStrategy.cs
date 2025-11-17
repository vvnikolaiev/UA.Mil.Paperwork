using Mil.Paperwork.Common.DataModels;
using Mil.Paperwork.Common.Helpers;
using System.Data;

namespace Mil.Paperwork.Common.Strategies
{
    internal abstract class ImportStrategy<T> : IImportStrategy
        where T : new()
    {
        public abstract ImportDataResult ImportData(List<ImportColumnDefinition> columns, List<Dictionary<string, object>> rows);

        public List<ImportColumnDefinition> GetColumnsToMap()
        {
            var type = typeof(T);
            var definitions = ImportHelper.GetColumnsToMap(type);

            return definitions;
        }

        public DataTable GetDataTable(List<ImportColumnDefinition> columns, List<Dictionary<string, object>> rows)
        {
            var validColumns = FilterValidColumns(columns);
            var dtos = ImportHelper.MapRowsToDtos<T>(validColumns, rows);
            var dataTable = ImportHelper.ToDataTable(dtos);
            return dataTable;
        }

        protected List<ImportColumnDefinition> FilterValidColumns(List<ImportColumnDefinition> allColumns)
        {
            var filteredColumns = allColumns.Where(x => !string.IsNullOrEmpty(x.SelectedSourceColumn) && x.ImportColumn).ToList();
            return filteredColumns;
        }
    }
}
