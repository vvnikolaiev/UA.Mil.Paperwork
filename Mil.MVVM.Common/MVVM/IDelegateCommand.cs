using System.Windows.Input;

namespace Mil.MVVM.Common
{
    public interface IDelegateCommand : ICommand
    {
        void RaiseCanExecuteChanged();
    }

    public interface IDelegateCommand<T> : ICommand<T>
    {
        void RaiseCanExecuteChanged();
    }
}
