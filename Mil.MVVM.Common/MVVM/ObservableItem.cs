using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Mil.MVVM.Common
{
    public abstract class ObservableItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyValueChanged(object? value, string propertyName)
        {
            OnPropertyChanged(propertyName);
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            NotifyValueChanged(value, propertyName);
            return true;
        }
    }
}
