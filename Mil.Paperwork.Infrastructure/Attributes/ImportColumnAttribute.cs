namespace Mil.Paperwork.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ImportColumnAttribute(string title, bool isReqired = false) : Attribute
    {
        public string Title { get; } = title;
        public bool IsRequired { get; } = isReqired;
    }
}
