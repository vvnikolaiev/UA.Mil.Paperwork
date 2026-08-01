using Mil.Paperwork.DataAccess.DataModels.Snapshots;
using Mil.Paperwork.Domain.DataModels;
using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Infrastructure.DataModels;

namespace Mil.Paperwork.DataAccess.Mappers
{
    internal static class EASSnapshotMapper
    {
        public static EASReportSnapshot ToSnapshot(IEASReportData data)
        {
            var snapshot = new EASReportSnapshot
            {
                ReportNum = data.ReportNum,
                ReportDate = data.ReportDate,
                EventDate = data.EventDate,
                EventTime = data.EventTime,
                BattleOrder = data.BattleOrder,
                BattleOrderDate = data.BattleOrderDate,
                SubdivisionName = data.SubdivisionName,
                ReporterRank = data.ReporterRank,
                ReporterName = data.ReporterName,
                WhatHappened = data.WhatHappened,
                OrdenNum = data.OrdenNum,
                OrdenDate = data.OrdenDate,
                Services = ToServiceSnapshots(data.Services),
                Witnesses = ToWitnessSnapshots(data.Witnesses),
                DestinationFolder = data.DestinationFolder
            };

            return snapshot;
        }

        public static EASReportData ToReportData(EASReportSnapshot snapshot)
        {
            var data = new EASReportData
            {
                ReportNum = snapshot.ReportNum,
                ReportDate = snapshot.ReportDate,
                EventDate = snapshot.EventDate,
                EventTime = snapshot.EventTime,
                BattleOrder = snapshot.BattleOrder,
                BattleOrderDate = snapshot.BattleOrderDate,
                SubdivisionName = snapshot.SubdivisionName,
                ReporterRank = snapshot.ReporterRank,
                ReporterName = snapshot.ReporterName,
                WhatHappened = snapshot.WhatHappened,
                OrdenNum = snapshot.OrdenNum,
                OrdenDate = snapshot.OrdenDate,
                Services = ToServices(snapshot.Services),
                Witnesses = ToWitnesses(snapshot.Witnesses),
                DestinationFolder = snapshot.DestinationFolder
            };

            return data;
        }

        private static List<EASServiceSnapshot> ToServiceSnapshots(IList<EASServiceData>? services)
        {
            var result = services?
                .Where(service => service != null)
                .Select(service => new EASServiceSnapshot
                {
                    ServiceName = service.ServiceName,
                    ServiceNameGenitive = service.ServiceNameGenitive,
                    HeadRank = service.HeadRank,
                    HeadName = service.HeadName,
                    HeadPosition = service.HeadPosition,
                    Assets = ToAssetSnapshots(service.Assets)
                })
                .ToList() ?? [];

            return result;
        }

        private static IList<EASServiceData> ToServices(List<EASServiceSnapshot>? snapshots)
        {
            IList<EASServiceData> result = snapshots?
                .Select(snapshot => new EASServiceData
                {
                    ServiceName = snapshot.ServiceName,
                    ServiceNameGenitive = snapshot.ServiceNameGenitive,
                    HeadRank = snapshot.HeadRank,
                    HeadName = snapshot.HeadName,
                    HeadPosition = snapshot.HeadPosition,
                    Assets = ToAssets(snapshot.Assets)
                })
                .ToList() ?? [];

            return result;
        }

        private static List<EASAssetSnapshot> ToAssetSnapshots(IList<EASAssetData>? assets)
        {
            var result = assets?
                .Where(asset => asset != null)
                .Select(asset => new EASAssetSnapshot
                {
                    Name = asset.Name,
                    SerialNumber = asset.SerialNumber,
                    Code = asset.Code,
                    MeasurementUnit = asset.MeasurementUnit,
                    Category = asset.Category,
                    Count = asset.Count,
                    OriginalPrice = asset.OriginalPrice,
                    ResidualPrice = asset.ResidualPrice
                })
                .ToList() ?? [];

            return result;
        }

        private static IList<EASAssetData> ToAssets(List<EASAssetSnapshot>? snapshots)
        {
            IList<EASAssetData> result = snapshots?
                .Select(snapshot => new EASAssetData
                {
                    Name = snapshot.Name,
                    SerialNumber = snapshot.SerialNumber,
                    Code = snapshot.Code,
                    MeasurementUnit = snapshot.MeasurementUnit,
                    Category = snapshot.Category,
                    Count = snapshot.Count,
                    OriginalPrice = snapshot.OriginalPrice,
                    ResidualPrice = snapshot.ResidualPrice
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
