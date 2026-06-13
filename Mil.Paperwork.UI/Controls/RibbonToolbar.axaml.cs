using Avalonia;
using Avalonia.Controls;
using Mil.Paperwork.UI.ViewModels.Ribbon;
using System.Collections.Generic;

namespace Mil.Paperwork.UI.Controls
{
    internal partial class RibbonToolbar : UserControl
    {
        public static readonly StyledProperty<IList<RibbonGroupViewModel>?> GroupsProperty =
            AvaloniaProperty.Register<RibbonToolbar, IList<RibbonGroupViewModel>?>(nameof(Groups));

        public static readonly StyledProperty<string?> StatusContentProperty =
            AvaloniaProperty.Register<RibbonToolbar, string?>(nameof(StatusContent));

        public IList<RibbonGroupViewModel>? Groups
        {
            get { return GetValue(GroupsProperty); }
            set { SetValue(GroupsProperty, value); }
        }

        public string? StatusContent
        {
            get { return GetValue(StatusContentProperty); }
            set { SetValue(StatusContentProperty, value); }
        }

        public RibbonToolbar()
        {
            InitializeComponent();
        }
    }
}
