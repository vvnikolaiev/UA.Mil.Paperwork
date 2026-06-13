using Mil.Paperwork.Infrastructure.Enums;
using Mil.MVVM.Common;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.UI.ViewModels.Ribbon;
using System;
using System.Collections.Generic;

namespace Mil.Paperwork.UI.ViewModels.Tabs
{
    internal abstract class BaseTabViewModel : ValidatableObservableItem, ITabViewModel
    {
        private readonly IDialogService _dialogService;
        private IList<RibbonGroupViewModel>? _ribbonGroups;

        protected virtual string TabCloseConfirmation => "Ви впевнені, що хочете закрити цю вкладку?";

        public event EventHandler<ITabViewModel> TabCloseRequested;

        public abstract string Header { get; }

        public virtual bool IsDirty => false;

        public virtual IList<RibbonGroupViewModel> RibbonGroups
        {
            get
            {
                if (_ribbonGroups == null)
                {
                    _ribbonGroups = BuildRibbonGroups();
                }
                return _ribbonGroups;
            }
        }

        public IDelegateCommand CloseTabCommand { get; }

        public BaseTabViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
            CloseTabCommand = new DelegateCommand(CloseTabCommandExecute);
        }

        protected virtual IList<RibbonGroupViewModel> BuildRibbonGroups()
        {
            var groups = new List<RibbonGroupViewModel>();
            return groups;
        }

        protected virtual async void Close()
        {
            var dlgResult = await _dialogService.ShowMessageAsync(TabCloseConfirmation, "Підтвердження", DialogButtons.YesNo);
            if (dlgResult == DialogResult.Yes)
            {
                RaiseTabCloseRequested();
            }
        }

        protected void RaiseTabCloseRequested()
        {
            TabCloseRequested?.Invoke(this, this);
        }

        private async void CloseTabCommandExecute()
        {
            Close();
        }
    }
}