namespace Mil.Paperwork.WriteOff.DataModels
{
    internal class EnumItemDataModel<T> where T : Enum
    {
        public T Value { get; set; }
        public string Title { get; set; }

        public EnumItemDataModel(T value, string title)
        {
            Value = value;
            Title = title;
        }

        public override string ToString()
        {
            return Title;
        }
    }
}
