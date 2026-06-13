using Mil.MVVM.Common;
using System;

namespace Mil.Paperwork.UI.ViewModels.Tabs
{
    public interface ITabViewModel
    {
        event EventHandler<ITabViewModel> TabCloseRequested;

        string Header { get; }

        bool IsDirty { get; }

        IDelegateCommand CloseTabCommand { get; }
    }
}
