using Mil.Paperwork.Common.DataModels;
using Mil.Paperwork.Common.Helpers;
using Mil.Paperwork.Domain.DataModels;

namespace Mil.Paperwork.Common.Strategies
{
    internal class EASAssetsImportStrategy : ImportStrategy<EASAssetData>
    {
        public override ImportDataResult ImportData(List<ImportColumnDefinition> columns, List<Dictionary<string, object>> rows)
        {
            ImportDataResult result;
            try
            {
                var validColumns = FilterValidColumns(columns);
                var dtos = ImportHelper.MapRowsToDtos<EASAssetData>(validColumns, rows);

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
