using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.WriteOff.Providers;
using Mil.Paperwork.WriteOff.ViewModels;
using Spire.Xls;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Mil.Paperwork.WriteOff.Views
{
    /// <summary>
    /// Interaction logic for WriteOffReportView.xaml
    /// </summary>
    public partial class WriteOffReportView : UserControl
    {
        private readonly Style _textBlockStyle;

        public WriteOffReportView()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;

            _textBlockStyle = new Style(typeof(TextBlock));
            _textBlockStyle.Setters.Add(new Setter(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center));
            _textBlockStyle.Setters.Add(new Setter(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Center));
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext is WriteOffReportViewModel viewModel)
            {
                ReplaceColumns(viewModel);

                viewModel.PropertyChanged += ViewModelPropertyChanged;
            }
        }
        private IDataGridColumnProvider GetColumnProvider(AssetType assetType)
        {
            return assetType switch
            {
                AssetType.Connectivity => new ConnectivityAssetColumnProvider(),
                AssetType.Radiochemical => new RadiochemicalAssetColumnProvider(),
                _ => throw new ArgumentOutOfRangeException(nameof(assetType), "Unsupported report type")
            };
        }

        private void ViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(WriteOffReportViewModel.SelectedAssetType))
            {
                if (DataContext is WriteOffReportViewModel viewModel)
                {
                    ReplaceColumns(viewModel);
                }
            }
        }

        private void ReplaceColumns(WriteOffReportViewModel viewModel)
        {
            var columnProvider = GetColumnProvider(viewModel.SelectedAssetType);

            // Clear existing dynamic columns
            var dynamicColumns = AssetsDataGrid.Columns
                .Where(c => c.Header?.ToString()?.StartsWith("Dynamic") == true)
                .ToList();

            foreach (var column in dynamicColumns)
            {
                AssetsDataGrid.Columns.Remove(column);
            }

            // Add new dynamic columns from the provider
            foreach (var column in columnProvider.GetColumns())
            {
                if (column is DataGridTextColumn dataGridTextColumn)
                {
                    dataGridTextColumn.ElementStyle = _textBlockStyle;
                }

                AssetsDataGrid.Columns.Add(column);
            }
        }
    }
}
