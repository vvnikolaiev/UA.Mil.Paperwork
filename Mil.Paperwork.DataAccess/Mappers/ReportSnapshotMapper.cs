using Mil.Paperwork.DataAccess.DataModels.History;
using Mil.Paperwork.DataAccess.DataModels.Snapshots;
using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Infrastructure.Enums;

namespace Mil.Paperwork.DataAccess.Mappers
{
    public static class ReportSnapshotMapper
    {
        public static ReportSnapshotBase ToSnapshot(ReportType reportType, IReportData reportData)
        {
            ReportSnapshotBase result = reportData switch
            {
                IInvoceReportData invoiceData => InvoiceSnapshotMapper.ToSnapshot(invoiceData),
                IResidualValueReportData residualValueData => ResidualValueSnapshotMapper.ToSnapshot(residualValueData),
                IInitialTechnicalStateReportData initialTechnicalStateData => InitialTechnicalStateSnapshotMapper.ToSnapshot(initialTechnicalStateData),
                ITechnicalStateReportData technicalStateData => TechnicalStateSnapshotMapper.ToSnapshot(technicalStateData),
                IWriteOffPackageReportData writeOffPackageData => WriteOffPackageSnapshotMapper.ToSnapshot(writeOffPackageData),
                ICommissioningActReportData commissioningActData => CommissioningActSnapshotMapper.ToSnapshot(commissioningActData),
                IDismantlingReportData dismantlingData => DismantlingSnapshotMapper.ToSnapshot(dismantlingData),
                IAssetValuationReportData valuationData => ValuationSnapshotMapper.ToSnapshot(valuationData),
                IHandoverReportData handoverData => Handover23SnapshotMapper.ToSnapshot(handoverData),
                IWriteOffOrderReportData writeOffOrderData => WriteOffOrderSnapshotMapper.ToSnapshot(writeOffOrderData),
                IEASReportData easData => EASSnapshotMapper.ToSnapshot(easData),
                _ => throw new NotSupportedException($"Report data of type '{reportData?.GetType().Name}' ({reportType}) is not supported.")
            };

            return result;
        }

        public static IReportData ToReportData(ReportSnapshotBase snapshot)
        {
            IReportData result = snapshot switch
            {
                InvoiceReportSnapshot invoiceSnapshot => InvoiceSnapshotMapper.ToReportData(invoiceSnapshot),
                ResidualValueReportSnapshot residualValueSnapshot => ResidualValueSnapshotMapper.ToReportData(residualValueSnapshot),
                InitialTechnicalStateReportSnapshot initialTechnicalStateSnapshot => InitialTechnicalStateSnapshotMapper.ToReportData(initialTechnicalStateSnapshot),
                TechnicalStateReportSnapshot technicalStateSnapshot => TechnicalStateSnapshotMapper.ToReportData(technicalStateSnapshot),
                WriteOffPackageReportSnapshot writeOffPackageSnapshot => WriteOffPackageSnapshotMapper.ToReportData(writeOffPackageSnapshot),
                CommissioningActReportSnapshot commissioningActSnapshot => CommissioningActSnapshotMapper.ToReportData(commissioningActSnapshot),
                DismantlingReportSnapshot dismantlingSnapshot => DismantlingSnapshotMapper.ToReportData(dismantlingSnapshot),
                ValuationReportSnapshot valuationSnapshot => ValuationSnapshotMapper.ToReportData(valuationSnapshot),
                Handover23ReportSnapshot handoverSnapshot => Handover23SnapshotMapper.ToReportData(handoverSnapshot),
                WriteOffOrderReportSnapshot writeOffOrderSnapshot => WriteOffOrderSnapshotMapper.ToReportData(writeOffOrderSnapshot),
                EASReportSnapshot easSnapshot => EASSnapshotMapper.ToReportData(easSnapshot),
                _ => throw new NotSupportedException($"Snapshot of type '{snapshot?.GetType().Name}' is not supported.")
            };

            return result;
        }

        public static ReportHistoryIndexEntry ToIndexEntry(ReportHistoryEntry entry)
        {
            var indexEntry = new ReportHistoryIndexEntry
            {
                SchemaVersion = entry.SchemaVersion,
                Id = entry.Id,
                ReportType = entry.ReportType,
                Status = entry.Status,
                CreatedAt = entry.CreatedAt,
                ModifiedAt = entry.ModifiedAt,
                DocumentNumber = entry.DocumentNumber,
                Summary = entry.Summary,
                GeneratedFiles = [.. entry.GeneratedFiles]
            };

            FillSearchFields(indexEntry, entry.Snapshot);

            return indexEntry;
        }

        private static void FillSearchFields(ReportHistoryIndexEntry indexEntry, ReportSnapshotBase? snapshot)
        {
            var names = new List<string?>();
            var serialNumbers = new List<string?>();
            var nomenclatureCodes = new List<string?>();

            switch (snapshot)
            {
                case InvoiceReportSnapshot invoiceSnapshot:
                    CollectAssetFields(invoiceSnapshot.Assets, names, serialNumbers, nomenclatureCodes);
                    break;
                case ResidualValueReportSnapshot residualValueSnapshot:
                    CollectAssetFields(residualValueSnapshot.Assets, names, serialNumbers, nomenclatureCodes);
                    break;
                case InitialTechnicalStateReportSnapshot initialTechnicalStateSnapshot:
                    CollectAssetFields(initialTechnicalStateSnapshot.Assets, names, serialNumbers, nomenclatureCodes);
                    break;
                case TechnicalStateReportSnapshot technicalStateSnapshot:
                    CollectAssetFields(technicalStateSnapshot.Assets, names, serialNumbers, nomenclatureCodes);
                    break;
                case WriteOffPackageReportSnapshot writeOffPackageSnapshot:
                    CollectAssetFields(writeOffPackageSnapshot.Assets, names, serialNumbers, nomenclatureCodes);
                    break;
                case Handover23ReportSnapshot handoverSnapshot:
                    CollectAssetFields(handoverSnapshot.Assets, names, serialNumbers, nomenclatureCodes);
                    break;
                case CommissioningActReportSnapshot commissioningActSnapshot:
                    names.Add(commissioningActSnapshot.Asset?.Name);
                    nomenclatureCodes.Add(commissioningActSnapshot.Asset?.NomenclatureCode);
                    serialNumbers.AddRange(commissioningActSnapshot.AssetIds.Select(assetId => assetId.SerialNumber));
                    break;
                case ValuationReportSnapshot valuationSnapshot:
                    names.AddRange(valuationSnapshot.ValuationData.Select(valuation => valuation.Name));
                    serialNumbers.AddRange(valuationSnapshot.ValuationData.Select(valuation => valuation.SerialNumber));
                    nomenclatureCodes.AddRange(valuationSnapshot.ValuationData.Select(valuation => valuation.NomenclatureCode));
                    break;
                case DismantlingReportSnapshot dismantlingSnapshot:
                    names.AddRange(dismantlingSnapshot.Dismantlings.Select(dismantling => dismantling.Name));
                    serialNumbers.AddRange(dismantlingSnapshot.Dismantlings.Select(dismantling => dismantling.SerialNumber));
                    nomenclatureCodes.AddRange(dismantlingSnapshot.Dismantlings.Select(dismantling => dismantling.NomenclatureCode));
                    break;
                case WriteOffOrderReportSnapshot writeOffOrderSnapshot:
                    names.AddRange(writeOffOrderSnapshot.Services.SelectMany(service => service.Assets).Select(asset => asset.Name));
                    break;
                case EASReportSnapshot easSnapshot:
                    names.AddRange(easSnapshot.Services.SelectMany(service => service.Assets).Select(asset => asset.Name));
                    nomenclatureCodes.AddRange(easSnapshot.Services.SelectMany(service => service.Assets).Select(asset => asset.Code));
                    break;
            }

            indexEntry.AssetNames = Normalize(names);
            indexEntry.SerialNumbers = Normalize(serialNumbers);
            indexEntry.NomenclatureCodes = Normalize(nomenclatureCodes);
        }

        private static void CollectAssetFields(
            List<AssetSnapshot> assets,
            List<string?> names,
            List<string?> serialNumbers,
            List<string?> nomenclatureCodes)
        {
            names.AddRange(assets.Select(asset => asset.Name));
            serialNumbers.AddRange(assets.Select(asset => asset.SerialNumber));
            nomenclatureCodes.AddRange(assets.Select(asset => asset.NomenclatureCode));
        }

        private static List<string> Normalize(List<string?> values)
        {
            var result = values
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Select(value => value!)
                .Distinct()
                .ToList();

            return result;
        }
    }
}
