using Mil.Paperwork.Infrastructure.Enums;

namespace Mil.Paperwork.UI.ViewModels.History
{
    internal class HistoryTypeFilterItem
    {
        public ReportType? Value { get; }

        public string Text { get; }

        public HistoryTypeFilterItem(ReportType? value, string text)
        {
            Value = value;
            Text = text;
        }
    }
}
