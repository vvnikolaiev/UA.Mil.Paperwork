using Mil.Paperwork.Domain.Services;
using Mil.Paperwork.Infrastructure.DataModels;
using Mil.Paperwork.WriteOff.DataModels;
using Mil.Paperwork.WriteOff.Helpers;

namespace Mil.Paperwork.WriteOff.Strategies
{
    internal class ValuationImportStrategy : ImportStrategy<AssetComponent>
    {
        public override ImportDataResult ImportData(List<ImportColumnDefinition> columns, List<Dictionary<string, object>> rows)
        {
            ImportDataResult result;
            try
            {
                var validColumns = FilterValidColumns(columns);
                var dtos = ImportHelper.MapRowsToDtos<AssetComponent>(validColumns, rows);
                
                result = new ImportDataResult(true, rows.Count)
                {
                    InvalidRowsCount = rows.Count - dtos.Count,
                    Rows = [.. dtos]
                };
            }
            catch (Exception ex)
            {
                result = new ImportDataResult(false)
                {
                    ErrorMessage = ex.Message
                };
            }

            return result;
        }
    }
}
