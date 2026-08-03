namespace Mil.Paperwork.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ImportColumnAttribute(string title, bool isRequired = false) : Attribute
    {
        public string Title { get; } = title;
        public bool IsRequired { get; } = isRequired;
    }
}
