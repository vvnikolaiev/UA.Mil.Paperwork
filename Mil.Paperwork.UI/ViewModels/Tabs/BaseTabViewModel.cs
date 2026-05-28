using Mil.Paperwork.Infrastructure.Enums;
using Mil.MVVM.Common;
using Mil.Paperwork.Infrastructure.Services;
using System;

namespace Mil.Paperwork.UI.ViewModels.Tabs
{
    internal abstract class BaseTabViewModel : ValidatableObservableItem, ITabViewModel
    {
        private readonly IDialogService _dialogService;

        protected virtual string TabCloseConfirmation => "Ви впевнені, що хочете закрити цю вкладку?";

        public event EventHandler<ITabViewModel> TabCloseRequested;

        public abstract string Header { get; }

        public IDelegateCommand CloseTabCommand { get; }

        public BaseTabViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
            CloseTabCommand = new DelegateCommand(CloseTabCommandExecute);
        }

        protected async void Close()
        {
            var dlgResult = await _dialogService.ShowMessageAsync(TabCloseConfirmation, "Підтвердження", DialogButtons.YesNo);
            if (dlgResult == DialogResult.Yes)
            {
                TabCloseRequested.Invoke(this, this);
            }
        }

        private async void CloseTabCommandExecute()
        {
            Close();
        }
    }
}