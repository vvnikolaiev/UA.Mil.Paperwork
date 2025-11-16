using Microsoft.Extensions.DependencyInjection;
using Mil.Paperwork.Domain;
using Mil.Paperwork.Infrastructure;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.WriteOff.Factories;
using Mil.Paperwork.WriteOff.Managers;
using Mil.Paperwork.WriteOff.ViewModels;
using Mil.Paperwork.WriteOff.ViewModels.Reports;
using Mil.Paperwork.WriteOff.Views;

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
            services.AddTransient<CommissioningActReportViewModel>();
            services.AddTransient<InvoiceReportViewModel>();
            services.AddTransient<ImportViewModel>();

            // Register Views
            services.AddTransient<MainWindow>();
            services.AddTransient<AssetValuationDialogWindow>();
            services.AddTransient<WriteOffReportView>();
            services.AddTransient<ImportDialogWindow>();

            // Register Factories
            services.AddSingleton<IAssetFactory, AssetFactory>();

            // Register Services
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IDialogService, WpfDialogService>();
            
            InfrastructureServicesRegistrator.Register(services);
            DomainServicesRegistrator.Register(services);

            services.AddSingleton<ReportManager>();
        }
    }
}
