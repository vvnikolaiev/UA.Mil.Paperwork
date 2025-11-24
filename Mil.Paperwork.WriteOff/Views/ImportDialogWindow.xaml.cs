using Mil.Paperwork.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Mil.Paperwork.WriteOff.Views
{
    /// <summary>
    /// Interaction logic for ImportDialogWindow.xaml
    /// </summary>
    public partial class ImportDialogWindow : Window
    {
        // Event used by window to close with a result
        public event Action? RequestClose;

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
    }
}
