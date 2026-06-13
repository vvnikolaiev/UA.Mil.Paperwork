using Mil.MVVM.Common;
using Mil.Paperwork.UI.Enums;

namespace Mil.Paperwork.UI.ViewModels.Shell
{
    internal class NavigationItemViewModel : ObservableItem
    {
        private bool _isSelected;

        public string Title { get; }

        public string IconKey { get; }

        public NavigationPageType PageType { get; }

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        public NavigationItemViewModel(string title, string iconKey, NavigationPageType pageType)
        {
            Title = title;
            IconKey = iconKey;
            PageType = pageType;
        }
    }
}
