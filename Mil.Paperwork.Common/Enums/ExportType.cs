using System.ComponentModel;

namespace Mil.Paperwork.Common.Enums
{
    public enum ExportType
    {
        [Description("Повну конфігурацію (json)")]
        Json,
        [Description("Таблицю (.xlsx)")]
        Excel
    }
}
