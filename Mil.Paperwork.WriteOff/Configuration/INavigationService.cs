using Mil.Paperwork.WriteOff.MVVM;
using System.Windows;

namespace Mil.Paperwork.WriteOff.Configuration
{
    public interface INavigationService
    {
        TViewModel GetViewModel<TViewModel>()
            where TViewModel : ObservableItem;

        TViewModel OpenWindow<TWindow, TViewModel>(TViewModel viewModel = null, bool isDialog = true)
            where TViewModel : ObservableItem
            where TWindow : Window;

        void CloseWindow<TViewModel>(TViewModel viewModel) where TViewModel : ObservableItem;

    }
}
