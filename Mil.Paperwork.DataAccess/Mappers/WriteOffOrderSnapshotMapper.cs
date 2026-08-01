using Mil.Paperwork.DataAccess.DataModels.Snapshots;
using Mil.Paperwork.Domain.DataModels;
using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Infrastructure.DataModels;

namespace Mil.Paperwork.DataAccess.Mappers
{
    internal static class WriteOffOrderSnapshotMapper
    {
        public static WriteOffOrderReportSnapshot ToSnapshot(IWriteOffOrderReportData data)
        {
            var snapshot = new WriteOffOrderReportSnapshot
            {
                ReportNum = data.ReportNum,
                ReportDate = data.ReportDate,
                EventDate = data.EventDate,
                EventTime = data.EventTime,
                BattleOrder = data.BattleOrder,
                BattleOrderDate = data.BattleOrderDate,
                BattleOrderLocation = data.BattleOrderLocation,
                SubdivisionName = data.SubdivisionName,
                ReporterRank = data.ReporterRank,
                ReporterName = data.ReporterName,
                CreatorPosition = data.CreatorPosition,
                CreatorRank = data.CreatorRank,
                CreatorName = data.CreatorName,
                MilUnitApproval = data.MilUnitApproval,
                WhatHappened = data.WhatHappened,
                Services = ToServiceSnapshots(data.Services),
                Witnesses = ToWitnessSnapshots(data.Witnesses),
                DestinationFolder = data.DestinationFolder
            };

            return snapshot;
        }

        public static WriteOffOrderReportData ToReportData(WriteOffOrderReportSnapshot snapshot)
        {
            var data = new WriteOffOrderReportData
            {
                ReportNum = snapshot.ReportNum,
                ReportDate = snapshot.ReportDate,
                EventDate = snapshot.EventDate,
                EventTime = snapshot.EventTime,
                BattleOrder = snapshot.BattleOrder,
                BattleOrderDate = snapshot.BattleOrderDate,
                BattleOrderLocation = snapshot.BattleOrderLocation,
                SubdivisionName = snapshot.SubdivisionName,
                ReporterRank = snapshot.ReporterRank,
                ReporterName = snapshot.ReporterName,
                CreatorPosition = snapshot.CreatorPosition,
                CreatorRank = snapshot.CreatorRank,
                CreatorName = snapshot.CreatorName,
                MilUnitApproval = snapshot.MilUnitApproval,
                WhatHappened = snapshot.WhatHappened,
                Services = ToServices(snapshot.Services),
                Witnesses = ToWitnesses(snapshot.Witnesses),
                DestinationFolder = snapshot.DestinationFolder
            };

            return data;
        }

        private static List<WriteOffServiceSnapshot> ToServiceSnapshots(IList<WriteOffServiceData>? services)
        {
            var result = services?
                .Where(service => service != null)
                .Select(service => new WriteOffServiceSnapshot
                {
                    ServiceName = service.ServiceName,
                    ServiceNameGenitive = service.ServiceNameGenitive,
                    Assets = ToServiceAssetSnapshots(service.Assets)
                })
                .ToList() ?? [];

            return result;
        }

        private static IList<WriteOffServiceData> ToServices(List<WriteOffServiceSnapshot>? snapshots)
        {
            IList<WriteOffServiceData> result = snapshots?
                .Select(snapshot => new WriteOffServiceData
                {
                    ServiceName = snapshot.ServiceName,
                    ServiceNameGenitive = snapshot.ServiceNameGenitive,
                    Assets = ToServiceAssets(snapshot.Assets)
                })
                .ToList() ?? [];

            return result;
        }

        private static List<WriteOffServiceAssetSnapshot> ToServiceAssetSnapshots(IList<WriteOffServiceAssetData>? assets)
        {
            var result = assets?
                .Where(asset => asset != null)
                .Select(asset => new WriteOffServiceAssetSnapshot
                {
                    Name = asset.Name,
                    Count = asset.Count,
                    MeasurementUnit = asset.MeasurementUnit,
                    Amount = asset.Amount
                })
                .ToList() ?? [];

            return result;
        }

        private static IList<WriteOffServiceAssetData> ToServiceAssets(List<WriteOffServiceAssetSnapshot>? snapshots)
        {
            IList<WriteOffServiceAssetData> result = snapshots?
                .Select(snapshot => new WriteOffServiceAssetData
                {
                    Name = snapshot.Name,
                    Count = snapshot.Count,
                    MeasurementUnit = snapshot.MeasurementUnit,
                    Amount = snapshot.Amount
                })
                .ToList() ?? [];

            return result;
        }

        private static List<PersonSnapshot> ToWitnessSnapshots(IList<PersonDTO>? witnesses)
        {
            var result = witnesses?
                .Where(witness => witness != null)
                .Select(witness => PersonSnapshotMapper.ToSnapshot(witness))
                .Where(snapshot => snapshot != null)
                .Select(snapshot => snapshot!)
                .ToList() ?? [];

            return result;
        }

        private static IList<PersonDTO> ToWitnesses(List<PersonSnapshot>? snapshots)
        {
            IList<PersonDTO> result = snapshots?
                .Select(snapshot => PersonSnapshotMapper.ToPerson(snapshot))
                .Where(person => person != null)
                .Select(person => person!)
                .ToList() ?? [];

            return result;
        }
    }
}
