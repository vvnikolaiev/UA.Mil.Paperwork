using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.UI.ViewModels;
using System;

namespace Mil.Paperwork.UI.Windows;

public partial class ImportDialogWindow : Window
{
    public ImportDialogWindow()
    {
        InitializeComponent();
    }

    private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
    {
        if (e.PropertyType == typeof(DateTime) || e.PropertyType == typeof(DateTime?))
        {
            if (e.Column is DataGridTextColumn textColumn)
            {
                // Set your desired format, e.g. "dd.MM.yyyy"
                (textColumn.Binding as Binding).StringFormat = "dd.MM.yyyy";
            }
        }
    }

    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);
        // ensure theme variant follows application (native) theme
        if (Application.Current?.RequestedThemeVariant != null)
            this.RequestedThemeVariant = Application.Current.RequestedThemeVariant;


        if (DataContext is IWindowViewModel<DialogResult> vm)
        {
            vm.RequestClose += result =>
            {
                // Close with result so ShowDialog returns it
                this.Close(result);
            };
        }
    }

}