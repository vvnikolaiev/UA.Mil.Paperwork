using Mil.Paperwork.WriteOff.ViewModels;
using System.Windows.Controls;
using System.Windows.Data;

namespace Mil.Paperwork.WriteOff.Providers
{
    internal interface IDataGridColumnProvider
    {
        /// <summary>
        /// Gets the collection of dynamic columns to be added to the DataGrid.
        /// </summary>
        /// <returns>A collection of DataGridColumn objects.</returns>
        IEnumerable<DataGridColumn> GetColumns();
    }
    internal class RadiochemicalAssetColumnProvider : IDataGridColumnProvider
    {
        public IEnumerable<DataGridColumn> GetColumns()
        {
            var columns = new List<DataGridColumn>()
            {
               new DataGridCheckBoxColumn
                {
                    Header = "Local?",
                    Binding = new Binding(nameof(RadiochemicalAssetInfoViewModel.IsLocal)),
                },
            };
            return columns;
        }
    }
    internal class ConnectivityAssetColumnProvider : IDataGridColumnProvider
    {
        public IEnumerable<DataGridColumn> GetColumns()
        {
            var columns = new List<DataGridColumn>()
            {
               new DataGridTextColumn
                {
                    Header = "WaT Coeff",
                    Binding = new Binding(nameof(ConnectivityAssetInfoViewModel.WearAndTearCoeff)),
                },
            };
            return columns;
        }
    }
}
