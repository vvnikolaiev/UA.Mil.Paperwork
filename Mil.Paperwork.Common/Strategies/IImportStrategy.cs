using Mil.Paperwork.Common.DataModels;
using System.Data;

namespace Mil.Paperwork.Common.Strategies
{
    public interface IImportStrategy
    {
        ImportDataResult ImportData(List<ImportColumnDefinition> columns, List<Dictionary<string, object>> rows);
        List<ImportColumnDefinition> GetColumnsToMap();
        DataTable GetDataTable(List<ImportColumnDefinition> columns, List<Dictionary<string, object>> rows);
    }
}
