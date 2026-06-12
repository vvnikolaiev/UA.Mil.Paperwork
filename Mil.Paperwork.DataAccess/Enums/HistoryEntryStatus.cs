using System.ComponentModel;

namespace Mil.Paperwork.DataAccess.Enums
{
    public enum HistoryEntryStatus
    {
        [Description("Чернетка")]
        Draft = 1,
        [Description("Згенеровано")]
        Generated = 2
    }
}
