using Mil.Paperwork.Common.MVVM;
using System;

namespace Mil.Paperwork.UI.ViewModels.Tabs
{
    public interface ITabViewModel
    {
        event EventHandler<ITabViewModel> TabCloseRequested;

        string Header { get; }
        
        IDelegateCommand CloseTabCommand { get; }
    }
}
