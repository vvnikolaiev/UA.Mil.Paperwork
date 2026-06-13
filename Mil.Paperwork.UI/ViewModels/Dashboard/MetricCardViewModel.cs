using Mil.MVVM.Common;

namespace Mil.Paperwork.UI.ViewModels.Dashboard
{
    internal class MetricCardViewModel : ObservableItem
    {
        private int _value;

        public string Label { get; }

        public int Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }

        public MetricCardViewModel(string label, int value)
        {
            Label = label;
            _value = value;
        }
    }
}
