using Mil.Paperwork.UI.Enums;

namespace Mil.Paperwork.UI.ViewModels.Tabs
{
    internal class SettingsSectionItem
    {
        public string Title { get; }
        public SettingsSection Section { get; }

        public SettingsSectionItem(string title, SettingsSection section)
        {
            Title = title;
            Section = section;
        }
    }
}
