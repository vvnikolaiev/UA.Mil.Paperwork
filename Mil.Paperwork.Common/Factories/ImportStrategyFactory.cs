using Mil.Paperwork.DataAccess.Services;
using Mil.Paperwork.Common.Enums;
using Mil.Paperwork.Common.Strategies;

namespace Mil.Paperwork.Common.Factories
{
    public static class ImportStrategyFactory
    {
        public static IImportStrategy GetStrategy(IDataService dataService, ImportType type)
        {
            IImportStrategy strategy = type switch
            {
                ImportType.Products => new ProductsImportStrategy(dataService),
                ImportType.People => new PeopleImportStrategy(dataService),
                ImportType.Valuation => new ValuationImportStrategy(),
                ImportType.EASAssets => new EASAssetsImportStrategy(),
                _ => throw new NotSupportedException()
            };

            return strategy;
        }
    }
}
