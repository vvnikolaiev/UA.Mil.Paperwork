using Microsoft.Extensions.DependencyInjection;
using Mil.Paperwork.Infrastructure.MVVM;
using System.Windows;

namespace Mil.Paperwork.Domain.Services
{
    public class NavigationService : INavigationService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<ObservableItem, Window> _viewModelWindowMap = [];

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public TViewModel OpenWindow<TWindow, TViewModel>(TViewModel viewModel = null, bool isDialog = true) 
            where TViewModel : ObservableItem
            where TWindow : Window
        {
            var window = _serviceProvider.GetRequiredService<TWindow>();
            if (viewModel == null)
            {
                viewModel = _serviceProvider.GetRequiredService<TViewModel>();
            }

            window.DataContext = viewModel;
            _viewModelWindowMap[viewModel] = window;

            if (isDialog)
            {
                window.ShowDialog();
            }
            else
            {
                window.Show();
            }

            return viewModel;
        }

        public void CloseWindow<TViewModel>(TViewModel viewModel) where TViewModel : ObservableItem
        {
            if (_viewModelWindowMap.TryGetValue(viewModel, out var window))
            {
                window.Close();
                _viewModelWindowMap.Remove(viewModel);
            }
        }
    }
}
