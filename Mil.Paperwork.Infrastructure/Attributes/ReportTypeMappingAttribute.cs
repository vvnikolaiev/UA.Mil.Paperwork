using Mil.Paperwork.Infrastructure.Enums;

namespace Mil.Paperwork.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ReportTypeMappingAttribute(ReportType reportType) : Attribute
    {
        public ReportType ReportType { get; } = reportType;
    }
}
