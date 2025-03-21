using System.Windows.Input;

namespace Mil.Paperwork.Infrastructure.MVVM
{
    public interface ICommand<T> : ICommand
    {
        void Execute(T parameter);
        bool CanExecute(T parameter);
    }
}
