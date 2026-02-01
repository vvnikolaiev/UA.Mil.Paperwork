using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Mil.Paperwork.Common.MVVM
{
    public abstract class ValidatableObservableItem : ObservableItem, INotifyDataErrorInfo
    {
        private readonly Dictionary<string, List<string>> _errors = [];

        public bool HasErrors => _errors.Any();
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public IEnumerable GetErrors(string? propertyName)
            => propertyName != null && _errors.ContainsKey(propertyName)
                ? _errors[propertyName]
                : Enumerable.Empty<string>();

        protected override void NotifyValueChanged(object? value, string propertyName = null)
        {
            base.NotifyValueChanged(value, propertyName);
            ValidateProperty(value, propertyName);
        }

        protected void ValidateProperty(object? value, string propertyName)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(this) { MemberName = propertyName };

            Validator.TryValidateProperty(value, context, results);

            if (results.Any())
                _errors[propertyName] = results.Select(r => r.ErrorMessage!).ToList();
            else
                _errors.Remove(propertyName);

            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }
    }
}
