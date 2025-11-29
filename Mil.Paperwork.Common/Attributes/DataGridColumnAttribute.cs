using Mil.Paperwork.Common.Enums;

namespace Mil.Paperwork.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DataGridColumnAttribute : Attribute
    {
        public string Header { get; }
        public ColumnType Type { get; }
        public bool IsReadOnly { get; }

        public DataGridColumnAttribute(string header, ColumnType type = ColumnType.Text, bool isReadOnly = false)
        {
            Header = header;
            IsReadOnly = isReadOnly;
            Type = type;
        }
    }
}
