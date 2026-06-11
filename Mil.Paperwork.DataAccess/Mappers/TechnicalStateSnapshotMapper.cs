using Mil.Paperwork.DataAccess.DataModels.Snapshots;
using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Domain.Enums;

namespace Mil.Paperwork.DataAccess.Mappers
{
    internal static class TechnicalStateSnapshotMapper
    {
        public static TechnicalStateReportSnapshot ToSnapshot(ITechnicalStateReportData data)
        {
            var snapshot = new TechnicalStateReportSnapshot
            {
                EventType = (int)data.EventType,
                Assets = AssetSnapshotMapper.ToSnapshots(data.Assets),
                DocumentDate = data.DocumentDate,
                Reason = data.Reason,
                EventDate = data.EventDate,
                OrdenNumber = data.OrdenNumber,
                OrdenDate = data.OrdenDate,
                GenerateWriteOffActs = data.GenerateWriteOffActs,
                DestinationFolder = data.DestinationFolder
            };

            return snapshot;
        }

        public static TechnicalStateReportData ToReportData(TechnicalStateReportSnapshot snapshot)
        {
            var data = new TechnicalStateReportData
            {
                EventType = (EventType)snapshot.EventType,
                Assets = AssetSnapshotMapper.ToAssetInfos(snapshot.Assets),
                DocumentDate = snapshot.DocumentDate,
                Reason = snapshot.Reason,
                EventDate = snapshot.EventDate,
                OrdenNumber = snapshot.OrdenNumber,
                OrdenDate = snapshot.OrdenDate,
                GenerateWriteOffActs = snapshot.GenerateWriteOffActs,
                DestinationFolder = snapshot.DestinationFolder
            };

            return data;
        }
    }
}
