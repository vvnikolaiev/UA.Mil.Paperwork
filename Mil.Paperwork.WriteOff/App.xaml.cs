using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using Mil.Paperwork.WriteOff.Configuration;
using System.Globalization;

namespace Mil.Paperwork.WriteOff
{
    public partial class App : Application
    {
        const string appName = "Mil.Paperwork.WriteOff";
        private static Mutex _mutex;

        protected override void OnStartup(StartupEventArgs e)
        {
            _mutex = new Mutex(true, appName, out var createdNew);

            if (!createdNew)
            {
                MessageBox.Show("Another instance of the application is already running.", "Instance Already Running", MessageBoxButton.OK, MessageBoxImage.Information);
                Current.Shutdown();
                return;
            }

            base.OnStartup(e);

            SetCurrentCulture();
            
            var serviceCollection = new ServiceCollection();
            var serviceConfigurator = new ServiceConfigurator();
            serviceConfigurator.ConfigureServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var mainWindow = serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void SetCurrentCulture()
        {
            var culture = new CultureInfo("uk-UA");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _mutex?.ReleaseMutex();
            base.OnExit(e);
        }
    }
}
