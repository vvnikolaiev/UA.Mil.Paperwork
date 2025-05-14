using Mil.Paperwork.Domain.Enums;

namespace Mil.Paperwork.WriteOff.DataModels
{
    internal class EventTypeDataModel
    {
        public EventType EventType { get; set; }
        public string Title { get; set; }

        public EventTypeDataModel(EventType eventType, string title)
        {
            EventType = eventType;
            Title = title;
        }

        public override string ToString()
        {
            return Title;
        }
    }
}
