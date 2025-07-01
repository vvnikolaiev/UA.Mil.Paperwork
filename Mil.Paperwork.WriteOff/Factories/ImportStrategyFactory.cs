using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.WriteOff.Enums;
using Mil.Paperwork.WriteOff.Strategies;

namespace Mil.Paperwork.WriteOff.Factories
{
    internal static class ImportStrategyFactory
    {
        public static IImportStrategy GetStrategy(IDataService dataService, ImportType type)
        {
            IImportStrategy strategy = type switch
            {
                ImportType.Products => new ProductsImportStrategy(dataService),
                ImportType.People => new PeopleImportStrategy(dataService),
                ImportType.Valuation => new ValuationImportStrategy(),
                _ => throw new NotSupportedException()
            };

            return strategy;
        }
    }
}
