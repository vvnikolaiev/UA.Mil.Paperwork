using Mil.Paperwork.Infrastructure.Enums;
using System;

namespace Mil.Paperwork.UI.ViewModels.History
{
    internal class CreateFromRequestedEventArgs : EventArgs
    {
        public Guid EntryId { get; }

        public ReportType TargetType { get; }

        public CreateFromRequestedEventArgs(Guid entryId, ReportType targetType)
        {
            EntryId = entryId;
            TargetType = targetType;
        }
    }
}
