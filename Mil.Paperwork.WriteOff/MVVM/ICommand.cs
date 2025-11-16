using System.Windows.Input;

namespace Mil.Paperwork.WriteOff.MVVM
{
    public interface ICommand<T> : ICommand
    {
        void Execute(T parameter);
        bool CanExecute(T parameter);
    }
}
