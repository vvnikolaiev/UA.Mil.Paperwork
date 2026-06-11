using Mil.Paperwork.DataAccess.DataModels.Snapshots;
using Mil.Paperwork.Domain.DataModels.ReportData;

namespace Mil.Paperwork.DataAccess.Mappers
{
    internal static class CommissioningActSnapshotMapper
    {
        public static CommissioningActReportSnapshot ToSnapshot(ICommissioningActReportData data)
        {
            var snapshot = new CommissioningActReportSnapshot
            {
                DocumentNumber = data.DocumentNumber,
                DocumentDate = data.DocumentDate,
                Asset = ProductSnapshotMapper.ToSnapshot(data.Asset),
                AssetState = data.AssetState,
                AssetIds = ToAssetIdSnapshots(data.AssetIds),
                CountText = data.CountText,
                Count = data.Count,
                CommissioningLocation = data.CommissioningLocation,
                ShortCharacteristic = data.ShortCharacteristic,
                AssetCompliance = data.AssetCompliance,
                CompletionState = data.CompletionState,
                TestResults = data.TestResults,
                OtherInfo = data.OtherInfo,
                Conclusion = data.Conclusion,
                AttachedDocumentation = data.AttachedDocumentation,
                PersonAccepted = PersonSnapshotMapper.ToSnapshot(data.PersonAccepted),
                PersonHanded = PersonSnapshotMapper.ToSnapshot(data.PersonHanded),
                DestinationFolder = data.DestinationFolder
            };

            return snapshot;
        }

        public static CommissioningActReportData ToReportData(CommissioningActReportSnapshot snapshot)
        {
            var data = new CommissioningActReportData
            {
                DocumentNumber = snapshot.DocumentNumber,
                DocumentDate = snapshot.DocumentDate,
                Asset = ProductSnapshotMapper.ToProduct(snapshot.Asset)!,
                AssetState = snapshot.AssetState,
                AssetIds = ToAssetIds(snapshot.AssetIds),
                CountText = snapshot.CountText,
                Count = snapshot.Count,
                CommissioningLocation = snapshot.CommissioningLocation,
                ShortCharacteristic = snapshot.ShortCharacteristic,
                AssetCompliance = snapshot.AssetCompliance,
                CompletionState = snapshot.CompletionState,
                TestResults = snapshot.TestResults,
                OtherInfo = snapshot.OtherInfo,
                Conclusion = snapshot.Conclusion,
                AttachedDocumentation = snapshot.AttachedDocumentation,
                PersonAccepted = PersonSnapshotMapper.ToPerson(snapshot.PersonAccepted)!,
                PersonHanded = PersonSnapshotMapper.ToPerson(snapshot.PersonHanded)!,
                DestinationFolder = snapshot.DestinationFolder
            };

            return data;
        }

        private static List<ProductIdentificationSnapshot> ToAssetIdSnapshots(IList<IProductIdentification>? assetIds)
        {
            var result = assetIds?
                .Where(assetId => assetId != null)
                .Select(assetId => new ProductIdentificationSnapshot
                {
                    SerialNumber = assetId.SerialNumber,
                    InventoryNumber = assetId.InventoryNumber
                })
                .ToList() ?? [];

            return result;
        }

        private static IList<IProductIdentification> ToAssetIds(List<ProductIdentificationSnapshot>? snapshots)
        {
            IList<IProductIdentification> result = snapshots?
                .Select(snapshot => (IProductIdentification)new ProductIdentification
                {
                    SerialNumber = snapshot.SerialNumber,
                    InventoryNumber = snapshot.InventoryNumber
                })
                .ToList() ?? [];

            return result;
        }
    }
}
