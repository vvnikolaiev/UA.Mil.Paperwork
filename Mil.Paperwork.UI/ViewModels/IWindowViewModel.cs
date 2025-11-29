using System;

namespace Mil.Paperwork.UI.ViewModels
{
    public interface IWindowViewModel
    {
        string Title { get; }

        event Action RequestClose;
    }

    public interface IWindowViewModel<T>
    {
        string Title { get; }

        event Action<T> RequestClose;
    }
}