using Mil.Paperwork.DataAccess.DataModels.Snapshots;
using Mil.Paperwork.Domain.DataModels.Parameters;
using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Domain.Enums;

namespace Mil.Paperwork.DataAccess.Mappers
{
    internal static class WriteOffPackageSnapshotMapper
    {
        public static WriteOffPackageReportSnapshot ToSnapshot(WriteOffPackageTabData data)
        {
            var packageData = data.PackageData;

            var snapshot = new WriteOffPackageReportSnapshot
            {
                Assets = AssetSnapshotMapper.ToSnapshots(packageData.Assets),
                DocumentDate = packageData.DocumentDate,
                EventDate = packageData.EventDate,
                OrdenNumber = packageData.OrdenNumber,
                OrdenDate = packageData.OrdenDate,
                BookOfLossesExtract = ToBookExtractSnapshot(packageData.BookOfLossesExtractData),
                ServiceKey = packageData.ServiceKey,
                DestinationFolder = packageData.DestinationFolder,

                Reason = data.Reason,
                EventType = (int)data.EventType,
                GenerateWriteOffPackage = data.GenerateWriteOffPackage,
                GenerateWriteOffActs = data.GenerateWriteOffActs,
                WriteOffRegNumber = data.WriteOffRegNumber,
                WriteOffDocNumber = data.WriteOffDocNumber,
                GenerateQualityStateReportInstead = data.GenerateQualityStateReportInstead,
                QSRRegNumber = data.QSRRegNumber,
                QSRDocNumber = data.QSRDocNumber
            };

            return snapshot;
        }

        public static WriteOffPackageTabData ToReportData(WriteOffPackageReportSnapshot snapshot)
        {
            var packageData = new WriteOffPackageReportData
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

            var data = new WriteOffPackageTabData
            {
                PackageData = packageData,
                Reason = snapshot.Reason,
                EventType = (EventType)snapshot.EventType,
                GenerateWriteOffPackage = snapshot.GenerateWriteOffPackage,
                GenerateWriteOffActs = snapshot.GenerateWriteOffActs,
                WriteOffRegNumber = snapshot.WriteOffRegNumber,
                WriteOffDocNumber = snapshot.WriteOffDocNumber,
                GenerateQualityStateReportInstead = snapshot.GenerateQualityStateReportInstead,
                QSRRegNumber = snapshot.QSRRegNumber,
                QSRDocNumber = snapshot.QSRDocNumber
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
