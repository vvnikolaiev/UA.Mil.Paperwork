namespace Mil.Paperwork.Common.DataModels
{
    public class ImportColumnDefinition
    {
        public string Title { get; }
        public bool IsRequired { get; }
        public string? SelectedSourceColumn { get; set; }

        public bool ImportColumn { get; set; } = true;

        public ImportColumnDefinition(string title, bool isRequired)
        {
            Title = title;
            IsRequired = isRequired;
        }
    }
}
