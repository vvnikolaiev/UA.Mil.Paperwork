using Mil.Paperwork.DataAccess.Conversions;
using Mil.Paperwork.Domain.DataModels;
using Mil.Paperwork.Domain.DataModels.Assets;
using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Infrastructure.DataModels;
using Mil.Paperwork.Infrastructure.DataModels.Configuration;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Services;

namespace Mil.Paperwork.Tests.History
{
    public class EASConversionTests
    {
        private sealed class StubReportDataService : IReportDataService
        {
            private readonly Dictionary<string, MilitaryServiceDTO> _services;
            private readonly string _selectedServiceKey;

            public StubReportDataService(Dictionary<string, MilitaryServiceDTO> services, string selectedServiceKey = "")
            {
                _services = services;
                _selectedServiceKey = selectedServiceKey;
            }

            public Dictionary<string, string> GetReportParametersDictionary(ReportType _) => [];
            public List<ReportParameter> GetReportParameters(ReportType _, bool __ = false) => [];
            public Dictionary<string, string> GetServiceReportParametersDictionary(string? _ = null) => [];
            public List<ReportParameter> GetServiceReportParameters(string? _ = null, bool __ = false) => [];
            public Dictionary<string, MilitaryServiceDTO> GetAllServices(bool _ = false) => _services;
            public string GetSelectedService(bool _ = false) => _selectedServiceKey;
            public CommissionDTO GetCommissionData(CommissionType _, bool __ = false) => new();
            public AssetType GetAssetType() => AssetType.Default;
            public ICommisionsConfigSection GetCommissionsConfig() => null!;
            public CommissionDTO GetCommission(ReportType _) => new();
            public bool ImportReportConfig(ReportDataConfigDTO _) => true;
            public void SaveReportConfigExternally(string _) { }
            public void SaveReportConfig(IReadOnlyCollection<ReportParameter> _, ReportType __) { }
            public void SaveReportConfigTemprorary(IReadOnlyCollection<ReportParameter> _, ReportType __) { }
            public void SaveCommission(CommissionDTO _, CommissionType __) { }
            public void SaveCommissionTemporary(CommissionDTO _, CommissionType __) { }
            public void SaveServiceData(string _, MilitaryServiceDTO __, bool ___ = false) { }
            public void DeleteServiceData(string _) { }
            public void SetDefaultService(string _) { }
        }

        private static MilitaryServiceDTO BuildServiceDto(string nominative, string genitive, string headRank, string headName, string headPosition)
        {
            var dto = new MilitaryServiceDTO();
            dto.ServiceNameFull.Value = nominative;
            dto.ServiceNameGenitive.Value = genitive;
            dto.HeadOfServiceRank.Value = headRank;
            dto.HeadOfServiceName.Value = headName;
            dto.HeadOfServicePosition.Value = headPosition;
            return dto;
        }

        [Fact]
        public void WriteOffOrderToEAS_MapsSimpleFieldsAndLooksUpServiceHeads()
        {
            var services = new Dictionary<string, MilitaryServiceDTO>
            {
                ["svc-1"] = BuildServiceDto("служба зв'язку", "служби зв'язку", "ст. лейтенант", "Олександр Шупер", "Начальник групи зв'язку")
            };
            var conversion = new WriteOffOrderToEASConversion(new StubReportDataService(services));

            var source = new WriteOffOrderReportData
            {
                ReportNum = "42",
                ReportDate = new DateTime(2026, 5, 30),
                EventDate = new DateTime(2026, 5, 28),
                EventTime = "14:35",
                BattleOrder = "7",
                BattleOrderDate = new DateTime(2026, 5, 20),
                SubdivisionName = "1 механізований батальйон",
                ReporterRank = "солдата",
                ReporterName = "Тестового Тест Тестовича",
                WhatHappened = "внаслідок бойового зіткнення",
                Services =
                [
                    new WriteOffServiceData
                    {
                        ServiceName = "служба зв'язку",
                        ServiceNameGenitive = "служби зв'язку",
                        Assets = [new() { Name = "Радіостанція", Count = 2, MeasurementUnit = "шт.", Amount = 3001m }]
                    }
                ]
            };

            var results = conversion.Convert(source);

            var easData = Assert.IsType<EASReportData>(Assert.Single(results));
            Assert.Equal(source.EventDate, easData.EventDate);
            Assert.Equal(source.EventTime, easData.EventTime);
            Assert.Equal(source.BattleOrder, easData.BattleOrder);
            Assert.Equal(source.BattleOrderDate, easData.BattleOrderDate);
            Assert.Equal(source.SubdivisionName, easData.SubdivisionName);
            Assert.Equal(source.ReporterRank, easData.ReporterRank);
            Assert.Equal(source.ReporterName, easData.ReporterName);
            Assert.Equal(source.WhatHappened, easData.WhatHappened);
            Assert.Equal(source.ReportNum, easData.OrdenNum);
            Assert.Equal(source.ReportDate, easData.OrdenDate);
            Assert.Empty(easData.Witnesses);

            var service = Assert.Single(easData.Services);
            Assert.Equal("служба зв'язку", service.ServiceName);
            Assert.Equal("ст. лейтенант", service.HeadRank);
            Assert.Equal("Олександр Шупер", service.HeadName);
            Assert.Equal("Начальник групи зв'язку", service.HeadPosition);

            var asset = Assert.Single(service.Assets);
            Assert.Equal("Радіостанція", asset.Name);
            Assert.Equal(2, asset.Count);
            Assert.Equal(3001m, asset.ResidualPrice);
            Assert.Equal(string.Empty, asset.Code);
            Assert.Equal(0, asset.OriginalPrice);
        }

        [Fact]
        public void WriteOffOrderToEAS_ServiceNotInDictionary_LeavesHeadFieldsEmpty()
        {
            var conversion = new WriteOffOrderToEASConversion(new StubReportDataService([]));

            var source = new WriteOffOrderReportData
            {
                Services = [new WriteOffServiceData { ServiceName = "невідома служба", Assets = [] }]
            };

            var results = conversion.Convert(source);

            var easData = Assert.IsType<EASReportData>(Assert.Single(results));
            var service = Assert.Single(easData.Services);
            Assert.Equal(string.Empty, service.HeadName);
        }

        [Fact]
        public void WriteOffOrderToEAS_WrongSourceType_ReturnsEmpty()
        {
            var conversion = new WriteOffOrderToEASConversion(new StubReportDataService([]));

            var results = conversion.Convert(new ResidualValueReportData());

            Assert.Empty(results);
        }

        private static AssetInfo CreateAsset(string name, string serialNumber)
        {
            return new AssetInfo
            {
                Name = name,
                ShortName = name,
                MeasurementUnit = "шт.",
                SerialNumber = serialNumber,
                NomenclatureCode = "NC-77",
                InitialCategory = 2,
                Price = 1000m,
                Count = 1,
                StartDate = new DateTime(2020, 1, 1)
            };
        }

        [Fact]
        public void WriteOffPackageToEAS_UsesServiceKeyFromPackage_AndMapsRichAssetFields()
        {
            var services = new Dictionary<string, MilitaryServiceDTO>
            {
                ["svc-1"] = BuildServiceDto("речова служба", "речової служби", "сержант", "Тетяна Коробка", "Начальник речової служби")
            };
            var conversion = new WriteOffPackageToEASConversion(new StubReportDataService(services, selectedServiceKey: "other"));

            var source = new WriteOffPackageReportData
            {
                EventDate = new DateTime(2026, 5, 20),
                OrdenNumber = 7,
                OrdenDate = new DateTime(2026, 5, 25),
                ServiceKey = "svc-1",
                Assets = [CreateAsset("Телевізор", "SN-1")]
            };

            var results = conversion.Convert(source);

            var easData = Assert.IsType<EASReportData>(Assert.Single(results));
            Assert.Equal(source.EventDate, easData.EventDate);
            Assert.Equal("7", easData.OrdenNum);
            Assert.Equal(source.OrdenDate, easData.OrdenDate);
            Assert.Empty(easData.Witnesses);

            var service = Assert.Single(easData.Services);
            Assert.Equal("речова служба", service.ServiceName);
            Assert.Equal("сержант", service.HeadRank);
            Assert.Equal("Тетяна Коробка", service.HeadName);

            var asset = Assert.Single(service.Assets);
            Assert.Equal("Телевізор, с/н SN-1", asset.Name);
            Assert.Equal("NC-77", asset.Code);
            Assert.Equal(2, asset.Category);
            Assert.True(asset.OriginalPrice > 0);
            Assert.True(asset.ResidualPrice > 0);
        }

        [Fact]
        public void WriteOffPackageToEAS_MissingServiceKey_FallsBackToSelectedService()
        {
            var services = new Dictionary<string, MilitaryServiceDTO>
            {
                ["default-svc"] = BuildServiceDto("служба зв'язку", "служби зв'язку", "лейтенант", "Петро Грищенко", "Начальник зв'язку")
            };
            var conversion = new WriteOffPackageToEASConversion(new StubReportDataService(services, selectedServiceKey: "default-svc"));

            var source = new WriteOffPackageReportData
            {
                EventDate = new DateTime(2026, 5, 20),
                ServiceKey = string.Empty,
                Assets = [CreateAsset("Антена", "SN-2")]
            };

            var results = conversion.Convert(source);

            var easData = Assert.IsType<EASReportData>(Assert.Single(results));
            var service = Assert.Single(easData.Services);
            Assert.Equal("служба зв'язку", service.ServiceName);
            Assert.Equal("Петро Грищенко", service.HeadName);
        }

        [Fact]
        public void WriteOffPackageToEAS_WrongSourceType_ReturnsEmpty()
        {
            var conversion = new WriteOffPackageToEASConversion(new StubReportDataService([]));

            var results = conversion.Convert(new ResidualValueReportData());

            Assert.Empty(results);
        }
    }
}
