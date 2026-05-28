using System.Windows.Input;

namespace Mil.MVVM.Common
{
    public interface ICommand<T> : ICommand
    {
        void Execute(T parameter);
        bool CanExecute(T parameter);
    }
}
