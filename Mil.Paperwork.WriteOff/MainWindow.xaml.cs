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
}