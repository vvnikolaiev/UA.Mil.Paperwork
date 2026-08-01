using Mil.Paperwork.Domain.DataModels;
using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Infrastructure.DataModels.Configuration;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Services;

namespace Mil.Paperwork.DataAccess.Conversions
{
    public class WriteOffOrderToEASConversion : IReportDataConversion
    {
        private readonly IReportDataService _reportDataService;

        public WriteOffOrderToEASConversion(IReportDataService reportDataService)
        {
            _reportDataService = reportDataService;
        }

        public ReportType SourceType => ReportType.WriteOffOrder;

        public ReportType TargetType => ReportType.EAS;

        public IReadOnlyList<IReportData> Convert(IReportData source)
        {
            var result = new List<IReportData>();

            if (source is not IWriteOffOrderReportData writeOffOrderData)
            {
                return result;
            }

            var allServices = _reportDataService.GetAllServices();

            var reportData = new EASReportData
            {
                EventDate = writeOffOrderData.EventDate,
                EventTime = writeOffOrderData.EventTime,
                BattleOrder = writeOffOrderData.BattleOrder,
                BattleOrderDate = writeOffOrderData.BattleOrderDate,
                SubdivisionName = writeOffOrderData.SubdivisionName,
                ReporterRank = writeOffOrderData.ReporterRank,
                ReporterName = writeOffOrderData.ReporterName,
                WhatHappened = writeOffOrderData.WhatHappened,
                OrdenNum = writeOffOrderData.ReportNum,
                OrdenDate = writeOffOrderData.ReportDate,
                Services = [.. (writeOffOrderData.Services ?? []).Select(service => ToEASServiceData(service, allServices))],
                Witnesses = []
            };

            result.Add(reportData);

            return result;
        }

        private static EASServiceData ToEASServiceData(WriteOffServiceData service, Dictionary<string, MilitaryServiceDTO> allServices)
        {
            MilitaryServiceDTO? match = allServices.Values
                .Where(dto => dto.ServiceNameFull?.Value == service.ServiceName)
                .Select(dto => (MilitaryServiceDTO?)dto)
                .FirstOrDefault();

            var result = new EASServiceData
            {
                ServiceName = service.ServiceName,
                ServiceNameGenitive = service.ServiceNameGenitive,
                HeadRank = match?.HeadOfServiceRank?.Value ?? string.Empty,
                HeadName = match?.HeadOfServiceName?.Value ?? string.Empty,
                HeadPosition = match?.HeadOfServicePosition?.Value ?? string.Empty,
                Assets = [.. (service.Assets ?? []).Select(ToEASAssetData)]
            };

            return result;
        }

        private static EASAssetData ToEASAssetData(WriteOffServiceAssetData asset)
        {
            var result = new EASAssetData
            {
                Name = asset.Name,
                MeasurementUnit = asset.MeasurementUnit,
                Count = asset.Count,
                ResidualPrice = asset.Amount
            };

            return result;
        }
    }
}
