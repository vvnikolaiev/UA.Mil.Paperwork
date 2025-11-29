using Microsoft.Extensions.DependencyInjection;
using Mil.Paperwork.Domain;
using Mil.Paperwork.Infrastructure;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.UI.Factories;
using Mil.Paperwork.UI.Managers;
using Mil.Paperwork.UI.Services;
using Mil.Paperwork.UI.ViewModels;
using Mil.Paperwork.UI.ViewModels.Reports;
using Mil.Paperwork.UI.Windows;

namespace Mil.Paperwork.UI.Configuration
{
    public class ServiceConfigurator : IServiceConfigurator
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // Register ViewModels
            services.AddTransient<MainWindowViewModel>();
            services.AddTransient<AssetValuationViewModel>();
            services.AddTransient<AssetDismantlingViewModel>();

            // Register Views
            services.AddTransient<MainWindow>();
            //services.AddTransient<AssetValuationDialogWindow>();
            services.AddTransient<ImportDialogWindow>();

            // Register Factories
            services.AddSingleton<IAssetFactory, AssetFactory>();

            // Register Services
            services.AddSingleton<IDialogService, AvaloniaDialogService>();
            
            InfrastructureServicesRegistrator.Register(services);
            DomainServicesRegistrator.Register(services);

            services.AddSingleton<ReportManager>();
        }
    }
}
