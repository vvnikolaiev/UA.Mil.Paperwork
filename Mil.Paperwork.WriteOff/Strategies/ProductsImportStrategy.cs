using Mil.Paperwork.Domain.Services;
using Mil.Paperwork.Infrastructure.DataModels;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.WriteOff.DataModels;
using Mil.Paperwork.WriteOff.Helpers;

namespace Mil.Paperwork.WriteOff.Strategies
{
    internal class ProductsImportStrategy : ImportStrategy<ProductDTO>
    {
        private readonly IDataService _dataService;

        public ProductsImportStrategy(IDataService dataService)
        {
            _dataService = dataService;
        }

        public override ImportDataResult ImportData(List<ImportColumnDefinition> columns, List<Dictionary<string, object>> rows)
        {
            ImportDataResult result;
            try
            {
                var validColumns = FilterValidColumns(columns);
                var dtos = ImportHelper.MapRowsToDtos<ProductDTO>(validColumns, rows);
                _dataService.AlterProductsData(dtos);

                result = new ImportDataResult(true, rows.Count)
                {
                    InvalidRowsCount = rows.Count - dtos.Count
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
