using Mil.Paperwork.DataAccess.DataModels.Snapshots;
using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Domain.Enums;

namespace Mil.Paperwork.DataAccess.Mappers
{
    internal static class InitialTechnicalStateSnapshotMapper
    {
        public static InitialTechnicalStateReportSnapshot ToSnapshot(IInitialTechnicalStateReportData data)
        {
            var snapshot = new InitialTechnicalStateReportSnapshot
            {
                EventType = (int)data.EventType,
                Assets = AssetSnapshotMapper.ToSnapshots(data.Assets),
                PersonAccepted = PersonSnapshotMapper.ToSnapshot(data.PersonAccepted),
                PersonHanded = PersonSnapshotMapper.ToSnapshot(data.PersonHanded),
                DestinationFolder = data.DestinationFolder
            };

            return snapshot;
        }

        public static InitialTechnicalStateReportData ToReportData(InitialTechnicalStateReportSnapshot snapshot)
        {
            var data = new InitialTechnicalStateReportData
            {
                EventType = (EventType)snapshot.EventType,
                Assets = AssetSnapshotMapper.ToAssetInfos(snapshot.Assets),
                PersonAccepted = PersonSnapshotMapper.ToPerson(snapshot.PersonAccepted)!,
                PersonHanded = PersonSnapshotMapper.ToPerson(snapshot.PersonHanded)!,
                DestinationFolder = snapshot.DestinationFolder
            };

            return data;
        }
    }
}
