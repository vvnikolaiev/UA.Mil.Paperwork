using Microsoft.Extensions.DependencyInjection;
using Mil.Paperwork.WriteOff.Managers;
using Mil.Paperwork.WriteOff.ViewModels;
using Mil.Paperwork.WriteOff.Views;
using Mil.Paperwork.Infrastructure;
using Mil.Paperwork.Domain;

namespace Mil.Paperwork.WriteOff.Configuration
{
    public class ServiceConfigurator : IServiceConfigurator
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // Register ViewModels
            services.AddTransient<MainViewModel>();
            services.AddTransient<WriteOffReportViewModel>();
            services.AddTransient<AssetValuationViewModel>();
            services.AddTransient<AssetDismantlingViewModel>();

            // Register Views
            services.AddTransient<MainWindow>();
            services.AddTransient<AssetValuationDialogWindow>();
            services.AddTransient<WriteOffReportView>();

            // Register Services
            InfrastructureServicesRegistrator.Register(services);
            DomainServicesRegistrator.Register(services);

            services.AddSingleton<ReportManager>();
        }
    }
}
