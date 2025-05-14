using System.ComponentModel;

namespace Mil.Paperwork.Domain.Enums
{
    public enum EventType
    {
        [Description("Взято на облік")]
        Accounted,
        [Description("Втрачено")]
        Lost,
        [Description("Знищено")]
        Destroyed,
    }
}