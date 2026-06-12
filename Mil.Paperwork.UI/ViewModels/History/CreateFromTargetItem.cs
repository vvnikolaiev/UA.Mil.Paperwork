using Mil.MVVM.Common;
using Mil.Paperwork.Infrastructure.Enums;
using System;

namespace Mil.Paperwork.UI.ViewModels.History
{
    internal class CreateFromTargetItem
    {
        public Guid EntryId { get; }

        public ReportType TargetType { get; }

        public string Text { get; }

        public IDelegateCommand Command { get; }

        public CreateFromTargetItem(Guid entryId, ReportType targetType, string text, Action<Guid, ReportType> createCallback)
        {
            EntryId = entryId;
            TargetType = targetType;
            Text = text;
            Command = new DelegateCommand(() => createCallback(entryId, targetType));
        }
    }
}
