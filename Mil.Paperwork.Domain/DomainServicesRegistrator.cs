using Microsoft.Extensions.DependencyInjection;
using Mil.Paperwork.Domain.Factories;
using Mil.Paperwork.Domain.Services;
using Mil.Paperwork.Infrastructure.Services;

namespace Mil.Paperwork.Domain
{
    public static class DomainServicesRegistrator
    {
        public static void Register(IServiceCollection services)
        {
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IAssetFactory, AssetFactory>();

            services.AddSingleton<QualityStateReportService>();
            services.AddSingleton<TechnicalStateReportService>();
            services.AddSingleton<ResidualValueReportService>();
            services.AddSingleton<AssetDismantlingReportService>();
            services.AddSingleton<AssetValuationReportService>();
        }
    }
}
