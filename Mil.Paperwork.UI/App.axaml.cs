using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.Linq;
using System.Threading;
using System;
using Mil.Paperwork.UI.Configuration;
using Mil.Paperwork.UI.ViewModels;
using Mil.Paperwork.UI.Windows;

namespace Mil.Paperwork.UI
{
    public partial class App : Application
    {
        const string AppName = "Mil.Paperwork.WriteOff";
        private static Mutex? _mutex;

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);

#if DEBUG
            this.AttachDeveloperTools();
#endif
        }

        public override void OnFrameworkInitializationCompleted()
        {
            // single-instance guard
            _mutex = new Mutex(true, AppName, out var createdNew);
            if (!createdNew)
            {
                // another instance exists -> exit
                Environment.Exit(0);
                return;
            }

            SetCurrentCulture();

            // DI
            var serviceCollection = new ServiceCollection();
            // register shared infra & domain services (reuse registrators)
            var serviceConfigurator = new ServiceConfigurator();
            serviceConfigurator.ConfigureServices(serviceCollection);

            var provider = serviceCollection.BuildServiceProvider();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                DisableAvaloniaDataAnnotationValidation();

                var mainWindow = new MainWindow();
                var mainWindowViewModel = provider.GetRequiredService<MainWindowViewModel>();
                mainWindow.DataContext = mainWindowViewModel;

                desktop.MainWindow = mainWindow;
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void DisableAvaloniaDataAnnotationValidation()
        {
            var dataValidationPluginsToRemove =
                BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

            foreach (var plugin in dataValidationPluginsToRemove)
                BindingPlugins.DataValidators.Remove(plugin);
        }

        private void SetCurrentCulture()
        {
            var culture = new CultureInfo("uk-UA");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }
    }
}