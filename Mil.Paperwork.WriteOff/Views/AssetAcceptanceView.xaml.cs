using System.Windows;
using System.Windows.Controls;

namespace Mil.Paperwork.WriteOff.Views
{
    /// <summary>
    /// Interaction logic for AssetAcceptanceView.xaml
    /// </summary>
    public partial class AssetAcceptanceView : UserControl
    {
        public string AcceptedLabel
        {
            get => (string)GetValue(AcceptedLabelProperty);
            set => SetValue(AcceptedLabelProperty, value);
        }

        public static readonly DependencyProperty AcceptedLabelProperty =
            DependencyProperty.Register(nameof(AcceptedLabel), typeof(string), typeof(AssetAcceptanceView), new PropertyMetadata("Здав*:"));

        public string RecievedLabel
        {
            get => (string)GetValue(RecievedLabelProperty);
            set => SetValue(RecievedLabelProperty, value);
        }

        public static readonly DependencyProperty RecievedLabelProperty =
            DependencyProperty.Register(nameof(RecievedLabel), typeof(string), typeof(AssetAcceptanceView), new PropertyMetadata("Прийняв*:"));

        public AssetAcceptanceView()
        {
            InitializeComponent();
        }
    }
}
