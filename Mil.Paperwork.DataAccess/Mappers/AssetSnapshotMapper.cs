using Mil.Paperwork.DataAccess.DataModels.Snapshots;
using Mil.Paperwork.DataAccess.Enums;
using Mil.Paperwork.Domain.DataModels.Assets;
using Mil.Paperwork.Domain.Enums;

namespace Mil.Paperwork.DataAccess.Mappers
{
    internal static class AssetSnapshotMapper
    {
        public static List<AssetSnapshot> ToSnapshots(IList<IAssetInfo>? assets)
        {
            var result = assets?.Where(asset => asset != null).Select(ToSnapshot).ToList() ?? [];
            return result;
        }

        public static IList<IAssetInfo> ToAssetInfos(List<AssetSnapshot>? snapshots)
        {
            IList<IAssetInfo> result = snapshots?.Select(ToAssetInfo).ToList<IAssetInfo>() ?? [];
            return result;
        }

        public static AssetSnapshot ToSnapshot(IAssetInfo asset)
        {
            var snapshot = new AssetSnapshot
            {
                Kind = GetKind(asset),
                Name = asset.Name,
                ShortName = asset.ShortName,
                MeasurementUnit = asset.MeasurementUnit,
                SerialNumber = asset.SerialNumber,
                NomenclatureCode = asset.NomenclatureCode,
                InitialCategory = asset.InitialCategory,
                Price = asset.Price,
                Count = asset.Count,
                StartDate = asset.StartDate,
                EventType = (int)asset.EventType,
                TSRegisterNumber = asset.TSRegisterNumber,
                TSDocumentNumber = asset.TSDocumentNumber,
                WarrantyPeriodMonths = asset.WarrantyPeriodMonths,
                YearManufactured = asset.YearManufactured,
                ResourceYears = asset.ResourceYears,
                IsLocal = (asset as RadiochemicalAssetInfo)?.IsLocal,
                WearAndTearCoeff = (asset as ConnectivityAssetInfo)?.WearAndTearCoeff
            };

            return snapshot;
        }

        public static AssetInfo ToAssetInfo(AssetSnapshot snapshot)
        {
            var asset = CreateAssetInfo(snapshot);

            asset.Name = snapshot.Name;
            asset.ShortName = snapshot.ShortName;
            asset.MeasurementUnit = snapshot.MeasurementUnit;
            asset.SerialNumber = snapshot.SerialNumber;
            asset.NomenclatureCode = snapshot.NomenclatureCode;
            asset.InitialCategory = snapshot.InitialCategory;
            asset.Price = snapshot.Price;
            asset.Count = snapshot.Count;
            asset.StartDate = snapshot.StartDate;
            asset.EventType = (EventType)snapshot.EventType;
            asset.TSRegisterNumber = snapshot.TSRegisterNumber;
            asset.TSDocumentNumber = snapshot.TSDocumentNumber;
            asset.WarrantyPeriodMonths = snapshot.WarrantyPeriodMonths;
            asset.YearManufactured = snapshot.YearManufactured;
            asset.ResourceYears = snapshot.ResourceYears;

            return asset;
        }

        private static AssetSnapshotKind GetKind(IAssetInfo asset)
        {
            var kind = asset switch
            {
                RadiochemicalAssetInfo => AssetSnapshotKind.Radiochemical,
                ConnectivityAssetInfo => AssetSnapshotKind.Connectivity,
                _ => AssetSnapshotKind.Default
            };

            return kind;
        }

        private static AssetInfo CreateAssetInfo(AssetSnapshot snapshot)
        {
            AssetInfo asset;
            switch (snapshot.Kind)
            {
                case AssetSnapshotKind.Radiochemical:
                    var radiochemicalAsset = new RadiochemicalAssetInfo();
                    if (snapshot.IsLocal.HasValue)
                    {
                        radiochemicalAsset.IsLocal = snapshot.IsLocal.Value;
                    }

                    asset = radiochemicalAsset;
                    break;
                case AssetSnapshotKind.Connectivity:
                    var connectivityAsset = new ConnectivityAssetInfo();
                    if (snapshot.WearAndTearCoeff.HasValue)
                    {
                        connectivityAsset.WearAndTearCoeff = snapshot.WearAndTearCoeff.Value;
                    }

                    asset = connectivityAsset;
                    break;
                default:
                    asset = new AssetInfo();
                    break;
            }

            return asset;
        }
    }
}
