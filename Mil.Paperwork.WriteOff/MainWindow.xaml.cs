using Mil.Paperwork.WriteOff.ViewModels;
using System.Windows;
using System.Windows.Controls;
namespace Mil.Paperwork.WriteOff;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow(MainViewModel mainViewModel)
    {
        InitializeComponent();
        DataContext = mainViewModel;
    }

    private void ExportButton_Click(object sender, RoutedEventArgs e)
    {
        var btn = sender as Button;
        if (btn != null)
        {
            btn.ContextMenu.PlacementTarget = btn;
            btn.ContextMenu.IsOpen = true;
        }
    }
}