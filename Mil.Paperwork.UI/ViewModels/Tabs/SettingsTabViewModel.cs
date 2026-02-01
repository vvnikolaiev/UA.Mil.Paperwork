using Mil.Paperwork.Infrastructure.Services;

namespace Mil.Paperwork.UI.ViewModels.Tabs
{
    internal abstract class SettingsTabViewModel : BaseTabViewModel, ISettingsTabViewModel
    {
        public bool IsClosed { get; protected set; }

        protected SettingsTabViewModel(IDialogService dialogService) : base(dialogService)
        {
            TabCloseRequested += OnTabCloseRequested;
        }

        private void OnTabCloseRequested(object sender, ITabViewModel e)
        {
            IsClosed = true;
        }
    }
}