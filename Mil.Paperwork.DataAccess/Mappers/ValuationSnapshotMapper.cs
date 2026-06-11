using Mil.Paperwork.DataAccess.DataModels.Snapshots;
using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Infrastructure.DataModels;

namespace Mil.Paperwork.DataAccess.Mappers
{
    internal static class ValuationSnapshotMapper
    {
        public static ValuationReportSnapshot ToSnapshot(IAssetValuationReportData data)
        {
            var snapshot = new ValuationReportSnapshot
            {
                ValuationData = ToValuationSnapshots(data.ValuationData),
                DestinationFolder = data.DestinationFolder
            };

            return snapshot;
        }

        public static AssetValuationReportData ToReportData(ValuationReportSnapshot snapshot)
        {
            var data = new AssetValuationReportData
            {
                ValuationData = ToValuationData(snapshot.ValuationData),
                DestinationFolder = snapshot.DestinationFolder
            };

            return data;
        }

        public static List<AssetComponentSnapshot> ToComponentSnapshots(IList<AssetComponent>? components)
        {
            var result = components?
                .Where(component => component != null)
                .Select(component => new AssetComponentSnapshot
                {
                    Name = component.Name,
                    Unit = component.Unit,
                    NomenclatureCode = component.NomenclatureCode,
                    Quantity = component.Quantity,
                    Category = component.Category,
                    Price = component.Price,
                    Exclude = component.Exclude
                })
                .ToList() ?? [];

            return result;
        }

        public static IList<AssetComponent> ToComponents(List<AssetComponentSnapshot>? snapshots)
        {
            IList<AssetComponent> result = snapshots?
                .Select(snapshot => new AssetComponent
                {
                    Name = snapshot.Name,
                    Unit = snapshot.Unit,
                    NomenclatureCode = snapshot.NomenclatureCode,
                    Quantity = snapshot.Quantity,
                    Category = snapshot.Category,
                    Price = snapshot.Price,
                    Exclude = snapshot.Exclude
                })
                .ToList() ?? [];

            return result;
        }

        private static List<AssetValuationSnapshot> ToValuationSnapshots(IList<IAssetValuationData?>? valuationData)
        {
            var result = new List<AssetValuationSnapshot>();

            if (valuationData == null)
            {
                return result;
            }

            foreach (var valuation in valuationData)
            {
                if (valuation == null)
                {
                    continue;
                }

                var snapshot = new AssetValuationSnapshot
                {
                    Name = valuation.Name,
                    SerialNumber = valuation.SerialNumber,
                    ShortName = valuation.ShortName,
                    NomenclatureCode = valuation.NomenclatureCode,
                    Price = valuation.Price,
                    Description = valuation.Description,
                    ValuationDate = valuation.ValuationDate,
                    AssetComponents = ToComponentSnapshots(valuation.AssetComponents)
                };

                result.Add(snapshot);
            }

            return result;
        }

        private static IList<IAssetValuationData?> ToValuationData(List<AssetValuationSnapshot>? snapshots)
        {
            IList<IAssetValuationData?> result = snapshots?
                .Select(snapshot => (IAssetValuationData?)new AssetValuationData
                {
                    Name = snapshot.Name,
                    SerialNumber = snapshot.SerialNumber,
                    ShortName = snapshot.ShortName,
                    NomenclatureCode = snapshot.NomenclatureCode,
                    Price = snapshot.Price,
                    Description = snapshot.Description,
                    ValuationDate = snapshot.ValuationDate,
                    AssetComponents = ToComponents(snapshot.AssetComponents)
                })
                .ToList() ?? [];

            return result;
        }
    }
}
