using Mil.Paperwork.Domain.Services;
using Mil.Paperwork.WriteOff.DataModels;
using System.Data;

namespace Mil.Paperwork.WriteOff.Strategies
{
    internal interface IImportStrategy
    {
        ImportDataResult ImportData(List<ImportColumnDefinition> columns, List<Dictionary<string, object>> rows);
        List<ImportColumnDefinition> GetColumnsToMap();
        DataTable GetDataTable(List<ImportColumnDefinition> columns, List<Dictionary<string, object>> rows);
    }
}
