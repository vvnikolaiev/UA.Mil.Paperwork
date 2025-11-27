using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Dynamic;

namespace Mil.Paperwork.UI.Windows;

public partial class ImportDialogWindow : Window
{
    private ImportViewModel? _viewModel;
    private INotifyCollectionChanged? _currentCollection;
    private INotifyPropertyChanged? _vmNotify;

    public ImportDialogWindow()
    {
        InitializeComponent();

        this.DataContextChanged += ImportDialogWindow_DataContextChanged;
    }

    private void ImportDialogWindow_DataContextChanged(object? sender, EventArgs e)
    {
        if (_vmNotify != null)
        {
            _vmNotify.PropertyChanged -= ViewModel_PropertyChanged;
            _vmNotify = null;
        }

        if (_currentCollection != null)
        {
            _currentCollection.CollectionChanged -= Preview_CollectionChanged;
            _currentCollection = null;
        }

        _viewModel = this.DataContext as ImportViewModel;
        if (_viewModel is not null)
        {
            _vmNotify = _viewModel as INotifyPropertyChanged;
            if (_vmNotify != null)
            {
                _vmNotify.PropertyChanged += ViewModel_PropertyChanged;
            }

            AttachToPreviewCollection(_viewModel.PreviewTable);
            RebuildColumns();
        }
        else
        {
            // clear columns if no view model
            PreviewDataGrid?.Columns.Clear();
        }
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ImportViewModel.PreviewTable))
        {
            if (_currentCollection != null)
            {
                _currentCollection.CollectionChanged -= Preview_CollectionChanged;
                _currentCollection = null;
            }

            AttachToPreviewCollection(_viewModel?.PreviewTable);
            RebuildColumns();
        }
    }

    private void AttachToPreviewCollection(ObservableCollection<ExpandoObject>? collection)
    {
        if (collection is not null)
        {
            _currentCollection = collection;
            if (_currentCollection != null)
            {
                _currentCollection.CollectionChanged += Preview_CollectionChanged;
            }
        }
    }

    private void Preview_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        // rebuild columns when items change (e.g., first row added/removed)
        RebuildColumns();
    }

    private void RebuildColumns()
    {
        if (PreviewDataGrid == null) return;

        PreviewDataGrid.Columns.Clear();

        if (_viewModel?.PreviewTable == null) return;

        var first = _viewModel.PreviewTable.Count > 0 ? _viewModel.PreviewTable[0] : null;
        if (first == null) return;

        if (first is IDictionary<string, object> dict)
        {
            foreach (var kv in dict)
            {
                var key = kv.Key;
                var value = kv.Value;

                // Bind the entire data item and use a converter to extract the value by key.
                var binding = new Binding(".") { Mode = BindingMode.OneWay, Converter = new ExpandoIndexConverter(), ConverterParameter = key };

                var textCol = new DataGridTextColumn
                {
                    Header = key,
                    Binding = binding,
                    IsReadOnly = true
                };

                PreviewDataGrid.Columns.Add(textCol);
            }
        }
    }

    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);
        // ensure theme variant follows application (native) theme
        if (Application.Current?.RequestedThemeVariant != null)
            this.RequestedThemeVariant = Application.Current.RequestedThemeVariant;


        if (DataContext is IWindowViewModel vm)
        {
            vm.RequestClose += () =>
            {
                // Close with result so ShowDialog returns it
                this.Close();
            };
        }
    }

    private class ExpandoIndexConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        {
            if (parameter is not string key)
                return null;

            if (value is IDictionary<string, object> dict)
            {
                if (dict.TryGetValue(key, out var val))
                {
                    if (val == null) return null;

                    // Format DateTime specially
                    if (val is DateTime dt)
                    {
                        return dt.ToString("dd.MM.yyyy");
                    }

                    return val.ToString();
                }
            }

            // If the value is ExpandoObject but not IDictionary (shouldn't happen) try dynamic access
            if (value is DynamicObject dyn && parameter is string k)
            {
                // best-effort: try to get member
                try
                {
                    var dic = value as IDictionary<string, object>;
                    if (dic != null && dic.TryGetValue(k, out var v))
                        return v?.ToString();
                }
                catch { }
            }

            return null;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}