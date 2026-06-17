using System.Windows.Input;

namespace Mil.Paperwork.UI.ViewModels.Ribbon
{
    internal class RibbonActionViewModel
    {
        public string Caption { get; }
        public string IconKey { get; }
        public ICommand Command { get; }
        public bool IsDestructive { get; }
        public string AutomationId { get; }

        public RibbonActionViewModel(string caption, string iconKey, ICommand command, string automationId, bool isDestructive = false)
        {
            Caption = caption;
            IconKey = iconKey;
            Command = command;
            AutomationId = automationId;
            IsDestructive = isDestructive;
        }
    }
}
