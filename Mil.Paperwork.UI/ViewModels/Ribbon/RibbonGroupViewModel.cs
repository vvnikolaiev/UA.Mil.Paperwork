using System.Collections.Generic;

namespace Mil.Paperwork.UI.ViewModels.Ribbon
{
    internal class RibbonGroupViewModel
    {
        public string GroupTitle { get; }
        public IReadOnlyList<RibbonActionViewModel> Actions { get; }

        public RibbonGroupViewModel(string groupTitle, IReadOnlyList<RibbonActionViewModel> actions)
        {
            GroupTitle = groupTitle;
            Actions = actions;
        }
    }
}
