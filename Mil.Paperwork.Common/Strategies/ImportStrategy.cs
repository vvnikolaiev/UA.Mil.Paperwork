using Mil.Paperwork.Common.DataModels;
using Mil.Paperwork.Common.Helpers;
using System.Data;
using System.Dynamic;
using System.Reflection;

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

        public List<ExpandoObject> GetItemsCollection(List<ImportColumnDefinition> columns, List<Dictionary<string, object>> rows)
        {
            var validColumns = FilterValidColumns(columns);
            var dtos = ImportHelper.MapRowsToDtos<T>(validColumns, rows);
            var result = ToExpandoList(dtos);
            return result;
        }

        protected List<ImportColumnDefinition> FilterValidColumns(List<ImportColumnDefinition> allColumns)
        {
            var filteredColumns = allColumns.Where(x => !string.IsNullOrEmpty(x.SelectedSourceColumn) && x.ImportColumn).ToList();
            return filteredColumns;
        }

        public static List<ExpandoObject> ToExpandoList<T>(List<T> items)
        {
            var result = new List<ExpandoObject>();

            foreach (var item in items)
            {
                IDictionary<string, object> expando = new ExpandoObject();

                // reflect over all public instance properties
                foreach (var prop in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    var value = prop.GetValue(item);
                    expando[prop.Name] = value;
                }

                result.Add((ExpandoObject)expando);
            }

            return result;
        }

    }
}
