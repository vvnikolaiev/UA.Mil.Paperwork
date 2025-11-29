using Avalonia;
using Avalonia.Controls;

namespace Mil.Paperwork.UI.Views.Controls;

public partial class AssetAcceptanceView : UserControl
{
    public static readonly StyledProperty<string> AcceptedLabelProperty = 
        AvaloniaProperty.Register<AssetAcceptanceView, string>(nameof(AcceptedLabel), "Прийняв*:");

    public string AcceptedLabel
    {
        get => GetValue(AcceptedLabelProperty);
        set => SetValue(AcceptedLabelProperty, value);
    }

    public string HandedLabel
    {
        get => (string)GetValue(HandedLabelProperty);
        set => SetValue(HandedLabelProperty, value);
    }

    public static readonly StyledProperty<string> HandedLabelProperty =
        AvaloniaProperty.Register<AssetAcceptanceView, string>(nameof(HandedLabel), "Здав*:");


    public AssetAcceptanceView()
    {
        InitializeComponent();
    }
}