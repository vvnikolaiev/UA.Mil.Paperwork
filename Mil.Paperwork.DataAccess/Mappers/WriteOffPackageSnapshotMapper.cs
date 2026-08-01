using Mil.Paperwork.DataAccess.DataModels.Snapshots;
using Mil.Paperwork.Domain.DataModels.Parameters;
using Mil.Paperwork.Domain.DataModels.ReportData;

namespace Mil.Paperwork.DataAccess.Mappers
{
    internal static class WriteOffPackageSnapshotMapper
    {
        public static WriteOffPackageReportSnapshot ToSnapshot(IWriteOffPackageReportData data)
        {
            var snapshot = new WriteOffPackageReportSnapshot
            {
                Assets = AssetSnapshotMapper.ToSnapshots(data.Assets),
                DocumentDate = data.DocumentDate,
                EventDate = data.EventDate,
                OrdenNumber = data.OrdenNumber,
                OrdenDate = data.OrdenDate,
                BookOfLossesExtract = ToBookExtractSnapshot(data.BookOfLossesExtractData),
                ServiceKey = data.ServiceKey,
                DestinationFolder = data.DestinationFolder
            };

            return snapshot;
        }

        public static WriteOffPackageReportData ToReportData(WriteOffPackageReportSnapshot snapshot)
        {
            var data = new WriteOffPackageReportData
            {
                Assets = AssetSnapshotMapper.ToAssetInfos(snapshot.Assets),
                DocumentDate = snapshot.DocumentDate,
                EventDate = snapshot.EventDate,
                OrdenNumber = snapshot.OrdenNumber,
                OrdenDate = snapshot.OrdenDate,
                BookOfLossesExtractData = ToBookExtractData(snapshot.BookOfLossesExtract),
                ServiceKey = snapshot.ServiceKey,
                DestinationFolder = snapshot.DestinationFolder
            };

            return data;
        }

        private static BookExtractSnapshot? ToBookExtractSnapshot(IBookExtractData? bookExtractData)
        {
            if (bookExtractData == null)
            {
                return null;
            }

            var snapshot = new BookExtractSnapshot
            {
                Year = bookExtractData.Year,
                Number = bookExtractData.Number,
                PageNumber = bookExtractData.PageNumber,
                RecordDate = bookExtractData.RecordDate
            };

            return snapshot;
        }

        private static IBookExtractData? ToBookExtractData(BookExtractSnapshot? snapshot)
        {
            if (snapshot == null)
            {
                return null;
            }

            var bookExtractData = new BookExtractData
            {
                Year = snapshot.Year,
                Number = snapshot.Number,
                PageNumber = snapshot.PageNumber,
                RecordDate = snapshot.RecordDate
            };

            return bookExtractData;
        }
    }
}
