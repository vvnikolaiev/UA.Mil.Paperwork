using Mil.Paperwork.DataAccess.DataModels.Snapshots;
using Mil.Paperwork.Domain.DataModels.ReportData;

namespace Mil.Paperwork.DataAccess.Mappers
{
    internal static class Handover23SnapshotMapper
    {
        public static Handover23ReportSnapshot ToSnapshot(IHandoverReportData data)
        {
            var snapshot = new Handover23ReportSnapshot
            {
                DocumentNumber = data.DocumentNumber,
                DocumentDate = data.DocumentDate,
                DateStart = data.DateStart,
                DateEnd = data.DateEnd,
                Supplier = data.Supplier,
                Receiver = data.Receiver,
                ReasonDocumentName = data.ReasonDocumentName,
                ReasonDocumentNumber = data.ReasonDocumentNumber,
                ReasonDocumentDate = data.ReasonDocumentDate,
                Reason = data.Reason,
                PersonResponsible = PersonSnapshotMapper.ToSnapshot(data.PersonResponsible),
                PersonReceiver = PersonSnapshotMapper.ToSnapshot(data.PersonReceiver),
                Assets = AssetSnapshotMapper.ToSnapshots(data.Assets),
                DestinationFolder = data.DestinationFolder
            };

            return snapshot;
        }

        public static HandoverReportData ToReportData(Handover23ReportSnapshot snapshot)
        {
            var data = new HandoverReportData
            {
                DocumentNumber = snapshot.DocumentNumber,
                DocumentDate = snapshot.DocumentDate,
                DateStart = snapshot.DateStart,
                DateEnd = snapshot.DateEnd,
                Supplier = snapshot.Supplier,
                Receiver = snapshot.Receiver,
                ReasonDocumentName = snapshot.ReasonDocumentName,
                ReasonDocumentNumber = snapshot.ReasonDocumentNumber,
                ReasonDocumentDate = snapshot.ReasonDocumentDate,
                Reason = snapshot.Reason,
                PersonResponsible = PersonSnapshotMapper.ToPerson(snapshot.PersonResponsible)!,
                PersonReceiver = PersonSnapshotMapper.ToPerson(snapshot.PersonReceiver)!,
                Assets = AssetSnapshotMapper.ToAssetInfos(snapshot.Assets),
                DestinationFolder = snapshot.DestinationFolder
            };

            return data;
        }
    }
}
