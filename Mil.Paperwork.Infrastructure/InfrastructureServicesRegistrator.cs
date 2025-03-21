using Microsoft.Extensions.DependencyInjection;
using Mil.Paperwork.Infrastructure.Services;

namespace Mil.Paperwork.Infrastructure
{
    public static class InfrastructureServicesRegistrator
    {
        public static void Register(IServiceCollection services)
        {
            services.AddSingleton<IFileStorageService, FileStorageService>();
            services.AddSingleton<IDataService, DataService>();
            services.AddSingleton<IReportDataService, ReportDataService>();
        }
    }
}
