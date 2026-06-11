using Mil.Paperwork.DataAccess.DataModels.Snapshots;
using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Infrastructure.DataModels;

namespace Mil.Paperwork.DataAccess.Mappers
{
    internal static class DismantlingSnapshotMapper
    {
        public static DismantlingReportSnapshot ToSnapshot(IDismantlingReportData data)
        {
            var snapshot = new DismantlingReportSnapshot
            {
                Dismantlings = ToDismantlingSnapshots(data.Dismantlings),
                DestinationFolder = data.DestinationFolder
            };

            return snapshot;
        }

        public static DismantlingReportData ToReportData(DismantlingReportSnapshot snapshot)
        {
            var data = new DismantlingReportData
            {
                Dismantlings = ToDismantlings(snapshot.Dismantlings),
                DestinationFolder = snapshot.DestinationFolder
            };

            return data;
        }

        private static List<AssetDismantlingSnapshot> ToDismantlingSnapshots(IList<AssetDismantlingData>? dismantlings)
        {
            var result = dismantlings?
                .Where(dismantling => dismantling != null)
                .Select(dismantling => new AssetDismantlingSnapshot
                {
                    Name = dismantling.Name,
                    SerialNumber = dismantling.SerialNumber,
                    ShortName = dismantling.ShortName,
                    NomenclatureCode = dismantling.NomenclatureCode,
                    Price = dismantling.Price,
                    Description = dismantling.Description,
                    ValuationDate = dismantling.ValuationDate,
                    AssetComponents = ValuationSnapshotMapper.ToComponentSnapshots(dismantling.AssetComponents),
                    RegistrationNumber = dismantling.RegistrationNumber,
                    DocumentNumber = dismantling.DocumentNumber,
                    Reason = dismantling.Reason,
                    Category = dismantling.Category,
                    MeasurementUnit = dismantling.MeasurementUnit
                })
                .ToList() ?? [];

            return result;
        }

        private static IList<AssetDismantlingData> ToDismantlings(List<AssetDismantlingSnapshot>? snapshots)
        {
            IList<AssetDismantlingData> result = snapshots?
                .Select(snapshot => new AssetDismantlingData
                {
                    Name = snapshot.Name,
                    SerialNumber = snapshot.SerialNumber,
                    ShortName = snapshot.ShortName,
                    NomenclatureCode = snapshot.NomenclatureCode,
                    Price = snapshot.Price,
                    Description = snapshot.Description,
                    ValuationDate = snapshot.ValuationDate,
                    AssetComponents = ValuationSnapshotMapper.ToComponents(snapshot.AssetComponents),
                    RegistrationNumber = snapshot.RegistrationNumber,
                    DocumentNumber = snapshot.DocumentNumber,
                    Reason = snapshot.Reason,
                    Category = snapshot.Category,
                    MeasurementUnit = snapshot.MeasurementUnit
                })
                .ToList() ?? [];

            return result;
        }
    }
}
