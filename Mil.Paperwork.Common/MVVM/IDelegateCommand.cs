using System.Windows.Input;

namespace Mil.Paperwork.Common.MVVM
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
