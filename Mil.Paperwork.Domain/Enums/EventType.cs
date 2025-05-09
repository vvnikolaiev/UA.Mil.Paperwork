using System.ComponentModel;

namespace Mil.Paperwork.Domain.Enums
{
    public enum EventType
    {
        [Description("Втрачено")]
        Lost,
        [Description("Знищено")]
        Destroyed,
    }
}