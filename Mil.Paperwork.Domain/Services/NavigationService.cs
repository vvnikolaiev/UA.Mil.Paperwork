using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace Mil.Paperwork.Domain.Services
{
    public class NavigationService : INavigationService
    {
        private readonly IServiceProvider _serviceProvider;

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void OpenWindow<TViewModel>() where TViewModel : class
        {
            var window = _serviceProvider.GetRequiredService<Window>();
            var viewModel = _serviceProvider.GetRequiredService<TViewModel>();
            window.DataContext = viewModel;
            window.Show();
        }
    }
}
