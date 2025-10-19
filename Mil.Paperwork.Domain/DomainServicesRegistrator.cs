using Microsoft.Extensions.DependencyInjection;
using Mil.Paperwork.Domain.Services;

namespace Mil.Paperwork.Domain
{
    public static class DomainServicesRegistrator
    {
        public static void Register(IServiceCollection services)
        {
            services.AddSingleton<INavigationService, NavigationService>();

            services.AddSingleton<QualityStateReportService>();
            services.AddSingleton<TechnicalStateReportService>();
            services.AddSingleton<WriteOffReportPackageService>();
            services.AddSingleton<ResidualValueReportService>();
            services.AddSingleton<AssetDismantlingReportService>();
            services.AddSingleton<AssetValuationReportService>();
            services.AddSingleton<CommissioningActService>();
            services.AddSingleton<InvoiceReportService>();

            services.AddSingleton<IExportService, ExportService>();
            services.AddSingleton<IImportService, ImportService>();
        }
    }
}
