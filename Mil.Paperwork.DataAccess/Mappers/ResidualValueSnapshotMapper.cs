using Mil.Paperwork.DataAccess.DataModels.Snapshots;
using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Infrastructure.Enums;

namespace Mil.Paperwork.DataAccess.Mappers
{
    internal static class ResidualValueSnapshotMapper
    {
        public static ResidualValueReportSnapshot ToSnapshot(IResidualValueReportData data)
        {
            var snapshot = new ResidualValueReportSnapshot
            {
                AssetType = data.AssetType,
                Assets = AssetSnapshotMapper.ToSnapshots(data.Assets),
                MetalCosts = ToMetalCostsSnapshot(data.MetalCosts),
                EventDate = data.EventDate,
                EventReportNumber = data.EventReportNumber,
                DestinationFolder = data.DestinationFolder
            };

            return snapshot;
        }

        public static ResidualValueReportData ToReportData(ResidualValueReportSnapshot snapshot)
        {
            var data = new ResidualValueReportData
            {
                AssetType = snapshot.AssetType,
                Assets = AssetSnapshotMapper.ToAssetInfos(snapshot.Assets),
                MetalCosts = ToMetalCosts(snapshot.MetalCosts),
                EventDate = snapshot.EventDate,
                EventReportNumber = snapshot.EventReportNumber,
                DestinationFolder = snapshot.DestinationFolder
            };

            return data;
        }

        private static Dictionary<string, decimal> ToMetalCostsSnapshot(IDictionary<MetalType, decimal>? metalCosts)
        {
            var result = metalCosts?.ToDictionary(pair => pair.Key.ToString(), pair => pair.Value) ?? [];
            return result;
        }

        private static IDictionary<MetalType, decimal> ToMetalCosts(Dictionary<string, decimal>? metalCostsSnapshot)
        {
            var result = new Dictionary<MetalType, decimal>();

            if (metalCostsSnapshot != null)
            {
                foreach (var pair in metalCostsSnapshot)
                {
                    if (Enum.TryParse<MetalType>(pair.Key, out var metalType))
                    {
                        result[metalType] = pair.Value;
                    }
                }
            }

            return result;
        }
    }
}
