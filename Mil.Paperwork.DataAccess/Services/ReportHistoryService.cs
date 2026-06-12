using Mil.Paperwork.DataAccess.DataModels.History;
using Mil.Paperwork.DataAccess.DataModels.Snapshots;
using Mil.Paperwork.DataAccess.Enums;
using Mil.Paperwork.DataAccess.Mappers;
using Mil.Paperwork.DataAccess.Repositories;
using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Infrastructure.Enums;

namespace Mil.Paperwork.DataAccess.Services
{
    internal class ReportHistoryService : IReportHistoryService
    {
        private const int SummaryAssetNamesCount = 3;
        private const string SummaryAssetNamesSeparator = ", ";
        private const string SummaryMoreItemsFormat = " (+{0})";

        private readonly IReportHistoryRepository _repository;

        public ReportHistoryService(IReportHistoryRepository repository)
        {
            _repository = repository;
        }

        public Guid SaveDraft(ReportType reportType, IReportData reportData, Guid? entryId)
        {
            var result = Save(reportType, reportData, HistoryEntryStatus.Draft, null, entryId);
            return result;
        }

        public Guid SaveGenerated(ReportType reportType, IReportData reportData, IReadOnlyList<string> generatedFiles, Guid? entryId)
        {
            var result = Save(reportType, reportData, HistoryEntryStatus.Generated, generatedFiles, entryId);
            return result;
        }

        private Guid Save(
            ReportType reportType,
            IReportData reportData,
            HistoryEntryStatus status,
            IReadOnlyList<string>? generatedFiles,
            Guid? entryId)
        {
            var snapshot = ReportSnapshotMapper.ToSnapshot(reportType, reportData);
            var existingEntry = entryId.HasValue ? _repository.GetEntry(entryId.Value) : null;

            var entry = new ReportHistoryEntry
            {
                Id = entryId ?? Guid.NewGuid(),
                ReportType = reportType,
                Status = status,
                CreatedAt = existingEntry?.CreatedAt ?? default,
                DocumentNumber = GetDocumentNumber(snapshot),
                GeneratedFiles = generatedFiles?.ToList() ?? existingEntry?.GeneratedFiles ?? [],
                Snapshot = snapshot
            };

            entry.Summary = GetSummary(entry);

            _repository.Save(entry);

            return entry.Id;
        }

        private static string GetDocumentNumber(ReportSnapshotBase snapshot)
        {
            var result = snapshot switch
            {
                InvoiceReportSnapshot invoiceSnapshot => invoiceSnapshot.DocumentNumber,
                CommissioningActReportSnapshot commissioningActSnapshot => commissioningActSnapshot.DocumentNumber,
                Handover23ReportSnapshot handoverSnapshot => handoverSnapshot.DocumentNumber,
                WriteOffOrderReportSnapshot writeOffOrderSnapshot => writeOffOrderSnapshot.ReportNum,
                ResidualValueReportSnapshot residualValueSnapshot => residualValueSnapshot.EventReportNumber?.ToString() ?? string.Empty,
                TechnicalStateReportSnapshot technicalStateSnapshot => GetOrdenBasedNumber(technicalStateSnapshot.OrdenNumber),
                WriteOffPackageReportSnapshot writeOffPackageSnapshot => GetOrdenBasedNumber(writeOffPackageSnapshot.OrdenNumber),
                DismantlingReportSnapshot dismantlingSnapshot => dismantlingSnapshot.Dismantlings.FirstOrDefault()?.DocumentNumber ?? string.Empty,
                _ => string.Empty
            };

            return result ?? string.Empty;
        }

        private static string GetOrdenBasedNumber(int ordenNumber)
        {
            var result = ordenNumber > 0 ? ordenNumber.ToString() : string.Empty;
            return result;
        }

        private static string GetSummary(ReportHistoryEntry entry)
        {
            var indexEntry = ReportSnapshotMapper.ToIndexEntry(entry);
            var assetNames = indexEntry.AssetNames;

            var summary = string.Join(SummaryAssetNamesSeparator, assetNames.Take(SummaryAssetNamesCount));
            if (assetNames.Count > SummaryAssetNamesCount)
            {
                summary += string.Format(SummaryMoreItemsFormat, assetNames.Count - SummaryAssetNamesCount);
            }

            return summary;
        }
    }
}
