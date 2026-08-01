using Mil.Paperwork.Domain.DataModels;
using Mil.Paperwork.Domain.DataModels.Assets;
using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Domain.Helpers;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Services;

namespace Mil.Paperwork.DataAccess.Conversions
{
    public class WriteOffPackageToEASConversion : IReportDataConversion
    {
        private readonly IReportDataService _reportDataService;

        public WriteOffPackageToEASConversion(IReportDataService reportDataService)
        {
            _reportDataService = reportDataService;
        }

        public ReportType SourceType => ReportType.WriteOffPackage;

        public ReportType TargetType => ReportType.EAS;

        public IReadOnlyList<IReportData> Convert(IReportData source)
        {
            var result = new List<IReportData>();

            if (source is not IWriteOffPackageReportData writeOffPackageData)
            {
                return result;
            }

            var allServices = _reportDataService.GetAllServices();
            var serviceKey = !string.IsNullOrEmpty(writeOffPackageData.ServiceKey) && allServices.ContainsKey(writeOffPackageData.ServiceKey)
                ? writeOffPackageData.ServiceKey
                : _reportDataService.GetSelectedService();

            allServices.TryGetValue(serviceKey, out var serviceDto);

            var easService = new EASServiceData
            {
                ServiceName = serviceDto.ServiceNameFull?.Value ?? string.Empty,
                ServiceNameGenitive = serviceDto.ServiceNameGenitive?.Value ?? string.Empty,
                HeadRank = serviceDto.HeadOfServiceRank?.Value ?? string.Empty,
                HeadName = serviceDto.HeadOfServiceName?.Value ?? string.Empty,
                HeadPosition = serviceDto.HeadOfServicePosition?.Value ?? string.Empty,
                Assets = [.. (writeOffPackageData.Assets ?? []).Select(asset => ToEASAssetData(asset, writeOffPackageData.EventDate))]
            };

            var reportData = new EASReportData
            {
                EventDate = writeOffPackageData.EventDate,
                OrdenNum = writeOffPackageData.OrdenNumber.ToString(),
                OrdenDate = writeOffPackageData.OrdenDate,
                Services = [easService],
                Witnesses = []
            };

            result.Add(reportData);

            return result;
        }

        private static EASAssetData ToEASAssetData(IAssetInfo asset, DateTime reportDate)
        {
            var result = new EASAssetData
            {
                Name = asset.Name,
                SerialNumber = asset.SerialNumber,
                Code = asset.NomenclatureCode,
                MeasurementUnit = asset.MeasurementUnit,
                Category = asset.InitialCategory,
                Count = asset.Count,
                OriginalPrice = ResidualPriceHelper.CalculateIndexedPrice(asset, reportDate),
                ResidualPrice = ResidualPriceHelper.CalculateResidualPriceForItem(asset, reportDate)
            };

            return result;
        }
    }
}
