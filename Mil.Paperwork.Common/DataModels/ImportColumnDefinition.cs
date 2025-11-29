using Mil.Paperwork.Common.MVVM;

namespace Mil.Paperwork.Common.DataModels
{
    public class ImportColumnDefinition : ObservableItem
    {
        private string? _selectedSourceColumn;
        private bool _importColumn = true;

        public string Title { get; }
        public bool IsRequired { get; }
        public string? SelectedSourceColumn
        {
            get => _selectedSourceColumn; 
            set
            {
                if (SetProperty(ref _selectedSourceColumn, value))
                {
                    RaiseMappingChanged();
                }
            }
        }

        public bool ImportColumn
        {
            get => _importColumn;
            set
            {
                if (SetProperty(ref _importColumn, value))
                {
                    RaiseMappingChanged();
                }
            }
        }

        public EventHandler? MappingChanged;

        public ImportColumnDefinition(string title, bool isRequired)
        {
            Title = title;
            IsRequired = isRequired;
        }

        private void RaiseMappingChanged()
        {
            MappingChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
