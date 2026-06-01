using Mil.MVVM.Common;

namespace Mil.Paperwork.UI.ViewModels.Reports
{
    internal class WriteOffServiceAssetViewModel : ObservableItem
    {
        private string _name = string.Empty;
        private int _count = 1;
        private string _measurementUnit = string.Empty;
        private decimal _amount;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public int Count
        {
            get => _count;
            set => SetProperty(ref _count, value);
        }

        public string MeasurementUnit
        {
            get => _measurementUnit;
            set => SetProperty(ref _measurementUnit, value);
        }

        public decimal Amount
        {
            get => _amount;
            set => SetProperty(ref _amount, value);
        }
    }
}
