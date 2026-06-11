using Mil.Paperwork.DataAccess.DataModels.Snapshots;
using Mil.Paperwork.Domain.DataModels.ReportData;

namespace Mil.Paperwork.DataAccess.Mappers
{
    internal static class InvoiceSnapshotMapper
    {
        public static InvoiceReportSnapshot ToSnapshot(IInvoceReportData data)
        {
            var snapshot = new InvoiceReportSnapshot
            {
                DocumentNumber = data.DocumentNumber,
                DateCreated = data.DateCreated,
                DueDate = data.DueDate,
                Reason = data.Reason,
                Recipient = PersonSnapshotMapper.ToSnapshot(data.Recipient),
                Transmitter = PersonSnapshotMapper.ToSnapshot(data.Transmitter),
                HeadOfService = PersonSnapshotMapper.ToSnapshot((data as InvoceReportData)?.HeadOfService),
                Assets = AssetSnapshotMapper.ToSnapshots(data.Assets),
                DestinationFolder = data.DestinationFolder
            };

            return snapshot;
        }

        public static InvoceReportData ToReportData(InvoiceReportSnapshot snapshot)
        {
            var data = new InvoceReportData
            {
                DocumentNumber = snapshot.DocumentNumber,
                DateCreated = snapshot.DateCreated,
                DueDate = snapshot.DueDate,
                Reason = snapshot.Reason,
                Recipient = PersonSnapshotMapper.ToPerson(snapshot.Recipient)!,
                Transmitter = PersonSnapshotMapper.ToPerson(snapshot.Transmitter)!,
                HeadOfService = PersonSnapshotMapper.ToPerson(snapshot.HeadOfService)!,
                Assets = AssetSnapshotMapper.ToAssetInfos(snapshot.Assets),
                DestinationFolder = snapshot.DestinationFolder
            };

            return data;
        }
    }
}
