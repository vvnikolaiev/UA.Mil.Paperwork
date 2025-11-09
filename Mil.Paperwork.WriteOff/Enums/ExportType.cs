using System.ComponentModel;

namespace Mil.Paperwork.WriteOff.Enums
{
    internal enum ExportType
    {
        [Description("Повну конфігурацію (json)")]
        Json,
        [Description("Таблицю (.xlsx)")]
        Excel
    }
}
