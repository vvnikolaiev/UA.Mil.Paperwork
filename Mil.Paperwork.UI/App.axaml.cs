using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.Threading;
using System;
using Avalonia.Styling;
using Mil.Paperwork.Infrastructure.DataModels;
using Mil.Paperwork.UI.Configuration;
using Mil.Paperwork.UI.Services;
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
            if (!Design.IsDesignMode)
            {
                // single-instance guard
                _mutex = new Mutex(true, AppName, out var createdNew);
                if (!createdNew)
                {
                    Environment.Exit(0);
                    return;
                }
            }

            SetCurrentCulture();

            // DI
            var serviceCollection = new ServiceCollection();
            // register shared infra & domain services (reuse registrators)
            var serviceConfigurator = new ServiceConfigurator();
            serviceConfigurator.ConfigureServices(serviceCollection);

            var provider = serviceCollection.BuildServiceProvider();

            var userSettingsService = provider.GetRequiredService<IUserSettingsService>();
            var settings = userSettingsService.GetSettings();

            ApplyStoredTheme(settings);

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var mainWindow = new MainWindow();
                var mainWindowViewModel = provider.GetRequiredService<MainWindowViewModel>();
                mainWindow.DataContext = mainWindowViewModel;

                RestoreWindowGeometry(mainWindow, settings);
                mainWindow.Closing += (s, _) => SaveWindowGeometry((Window)s!, userSettingsService);

                desktop.MainWindow = mainWindow;
            }

            base.OnFrameworkInitializationCompleted();
        }

        private static void RestoreWindowGeometry(Window window, UserSettingsDTO settings)
        {
            if (settings.WindowWidth.HasValue && settings.WindowHeight.HasValue)
            {
                window.Width = Math.Max(settings.WindowWidth.Value, window.MinWidth);
                window.Height = Math.Max(settings.WindowHeight.Value, window.MinHeight);
            }

            if (settings.WindowPositionX.HasValue && settings.WindowPositionY.HasValue)
            {
                window.Position = new PixelPoint(settings.WindowPositionX.Value, settings.WindowPositionY.Value);
            }

            if (settings.WindowMaximized)
            {
                window.WindowState = WindowState.Maximized;
            }
        }

        private static void SaveWindowGeometry(Window window, IUserSettingsService userSettingsService)
        {
            var settings = userSettingsService.GetSettings();
            var isMaximized = window.WindowState == WindowState.Maximized;
            settings.WindowMaximized = isMaximized;

            if (!isMaximized)
            {
                settings.WindowPositionX = window.Position.X;
                settings.WindowPositionY = window.Position.Y;
                settings.WindowWidth = window.Width;
                settings.WindowHeight = window.Height;
            }

            userSettingsService.SaveSettings(settings);
        }

        private void ApplyStoredTheme(UserSettingsDTO settings)
        {
            RequestedThemeVariant = settings.Theme switch
            {
                UserSettingsDTO.ThemeLight => ThemeVariant.Light,
                UserSettingsDTO.ThemeDark => ThemeVariant.Dark,
                _ => ThemeVariant.Default
            };
        }

        private void SetCurrentCulture()
        {
            var culture = new CultureInfo("uk-UA");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }
    }
}