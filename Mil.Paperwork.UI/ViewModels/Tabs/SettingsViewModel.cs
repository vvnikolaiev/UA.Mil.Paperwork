using Mil.MVVM.Common;
using Mil.Paperwork.Infrastructure.Services;
using System;

namespace Mil.Paperwork.UI.ViewModels.Tabs
{
    internal class SettingsViewModel : ObservableItem, ISettingsTabViewModel
    {
        public string Header => "Settings";

        public bool IsDirty => false;

        public bool IsClosed { get; private set; }

        public event EventHandler<ITabViewModel> TabCloseRequested;

        public IDelegateCommand CloseTabCommand { get; }

        public SettingsViewModel()
        {
            CloseTabCommand = new DelegateCommand(CloseTabCommandExecute);
        }

        private void CloseTabCommandExecute()
        {
            TabCloseRequested.Invoke(this, this);
            IsClosed = true;
        }
    }
}
