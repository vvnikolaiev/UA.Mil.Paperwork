using Mil.MVVM.Common;
using Mil.Paperwork.DataAccess.DataModels.History;
using Mil.Paperwork.DataAccess.Enums;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Mil.Paperwork.UI.ViewModels.History
{
    internal class HistoryEntryViewModel : ObservableItem
    {
        private const string NumberSummaryFormat = "№{0} — {1}";
        private const string NumberOnlyFormat = "№{0}";

        public ReportHistoryIndexEntry IndexEntry { get; }

        public Guid Id { get; }

        public ReportType ReportType { get; }

        public string TypeText { get; }

        public DateTime ModifiedAt { get; }

        public HistoryEntryStatus Status { get; }

        public string StatusText { get; }

        public bool IsDraft { get; }

        public bool IsGenerated { get; }

        public string NumberSummaryText { get; }

        public string FileName { get; }

        public string FilePath { get; }

        public bool HasGeneratedFiles { get; }

        public IReadOnlyList<CreateFromTargetItem> CreateTargets { get; }

        public bool HasCreateTargets { get; }

        public HistoryEntryViewModel(ReportHistoryIndexEntry indexEntry, IReadOnlyList<CreateFromTargetItem> createTargets)
        {
            IndexEntry = indexEntry;
            Id = indexEntry.Id;
            ReportType = indexEntry.ReportType;
            TypeText = indexEntry.ReportType.GetDescription();
            ModifiedAt = indexEntry.ModifiedAt;
            Status = indexEntry.Status;
            StatusText = indexEntry.Status.GetDescription();
            IsDraft = indexEntry.Status == HistoryEntryStatus.Draft;
            IsGenerated = indexEntry.Status == HistoryEntryStatus.Generated;
            NumberSummaryText = BuildNumberSummaryText(indexEntry);
            FilePath = indexEntry.GeneratedFiles.FirstOrDefault() ?? string.Empty;
            FileName = Path.GetFileName(FilePath);
            HasGeneratedFiles = indexEntry.GeneratedFiles.Count > 0;
            CreateTargets = createTargets;
            HasCreateTargets = createTargets.Count > 0;
        }

        private static string BuildNumberSummaryText(ReportHistoryIndexEntry indexEntry)
        {
            string result;
            var hasNumber = !string.IsNullOrWhiteSpace(indexEntry.DocumentNumber);
            var hasSummary = !string.IsNullOrWhiteSpace(indexEntry.Summary);

            if (hasNumber && hasSummary)
            {
                result = string.Format(NumberSummaryFormat, indexEntry.DocumentNumber, indexEntry.Summary);
            }
            else if (hasNumber)
            {
                result = string.Format(NumberOnlyFormat, indexEntry.DocumentNumber);
            }
            else
            {
                result = indexEntry.Summary;
            }

            return result;
        }
    }
}
