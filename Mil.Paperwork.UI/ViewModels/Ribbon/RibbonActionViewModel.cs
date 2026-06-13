using System.Windows.Input;

namespace Mil.Paperwork.UI.ViewModels.Ribbon
{
    internal class RibbonActionViewModel
    {
        public string Caption { get; }
        public string IconKey { get; }
        public ICommand Command { get; }
        public bool IsDestructive { get; }

        public RibbonActionViewModel(string caption, string iconKey, ICommand command, bool isDestructive = false)
        {
            Caption = caption;
            IconKey = iconKey;
            Command = command;
            IsDestructive = isDestructive;
        }
    }
}
