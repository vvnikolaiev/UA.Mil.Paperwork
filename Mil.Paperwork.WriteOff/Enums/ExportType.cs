using System.ComponentModel;

namespace Mil.Paperwork.WriteOff.Enums
{
    internal enum ExportType
    {
        [Description("RAW data (json)")]
        Json,
        [Description("Table (.xlsx)")]
        Excel
    }
}
