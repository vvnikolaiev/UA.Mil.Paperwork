using Mil.Paperwork.Common.DataModels;
using System.Data;
using System.Dynamic;

namespace Mil.Paperwork.Common.Strategies
{
    public interface IImportStrategy
    {
        ImportDataResult ImportData(List<ImportColumnDefinition> columns, List<Dictionary<string, object>> rows);
        List<ImportColumnDefinition> GetColumnsToMap();
        DataTable GetDataTable(List<ImportColumnDefinition> columns, List<Dictionary<string, object>> rows);
        List<ExpandoObject> GetItemsCollection(List<ImportColumnDefinition> columns, List<Dictionary<string, object>> rows);
    }
}
