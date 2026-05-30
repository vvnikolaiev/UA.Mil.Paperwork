using Mil.Paperwork.Domain.DataModels;
using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Domain.Services;
using Mil.Paperwork.Infrastructure.DataModels;
using Mil.Paperwork.Infrastructure.DataModels.Configuration;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Services;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;

namespace Mil.Paperwork.Tests
{
    public class WriteOffOrderReportTests
    {
        private sealed class StubReportDataService : IReportDataService
        {
            public Dictionary<string, string> GetReportParametersDictionary(ReportType _) => [];
            public List<ReportParameter> GetReportParameters(ReportType _, bool __ = false) => [];
            public Dictionary<string, string> GetServiceReportParametersDictionary(string? _ = null) => [];
            public List<ReportParameter> GetServiceReportParameters(string? _ = null, bool __ = false) => [];
            public Dictionary<string, MilitaryServiceDTO> GetAllServices(bool _ = false) => [];
            public string GetSelectedService(bool _ = false) => string.Empty;
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

        private sealed class CapturingFileStorageService : IFileStorageService
        {
            public byte[]? SavedBytes { get; private set; }
            public void SaveFile(string _, byte[] bytes) => SavedBytes = bytes;
            public T? ReadJsonFile<T>(string _, string? __ = null) => default;
            public void WriteJsonToFile<T>(T _, string __, string? ___ = null) { }
            public void WriteJsonToFile<T>(T _, string __) { }
        }

        private static WriteOffWitnessData[] TestWitnesses =>
        [
            new() { Rank = "молодший сержант", Name = "УДАЛОВ Володимир Віталійович", Position = "командир міномета мінометного взводу" },
            new() { Rank = "старший солдат",   Name = "КОВАЛЬОВ Дмитро Миколайович",  Position = "майстер – номер обслуги мінометного взводу" }
        ];

        private static WriteOffOrderReportData BuildTestData() => new()
        {
            ReportNum = "42",
            ReportDate = new DateTime(2026, 5, 30),
            EventDate = new DateTime(2026, 5, 28),
            EventTime = "14:35",
            BattleOrder = "7",
            BattleOrderDate = new DateTime(2026, 5, 20),
            BattleOrderLocation = "н.п. Тест",
            SubdivisionName = "1 механізований батальйон",
            ReporterRank = "солдата",
            ReporterName = "Тестового Тест Тестовича",
            CreatorPosition = "Начальник служби",
            CreatorRank = "майор",
            CreatorName = "Іванов І.І.",
            MilUnitApproval = "в/ч А1234",
            WhatHappened = "Внаслідок бойового зіткнення майно прийшло до непридатності.",
            Services =
            [
                new WriteOffServiceData
                {
                    ServiceName = "Інженерна служба",
                    ServiceNameGenitive = "інженерної служби",
                    Assets =
                    [
                        new() { Name = "Лопата саперна", Count = 3, MeasurementUnit = "шт", Amount = 450.00m },
                        new() { Name = "Мотузка страховна", Count = 1, MeasurementUnit = "шт", Amount = 1200.50m }
                    ]
                },
                new WriteOffServiceData
                {
                    ServiceName = "Служба зв'язку",
                    ServiceNameGenitive = "служби зв'язку",
                    Assets =
                    [
                        new() { Name = "Радіостанція Р-187П1", Count = 1, MeasurementUnit = "шт", Amount = 35000.00m }
                    ]
                }
            ],
            DestinationFolder = Path.GetTempPath()
        };

        private static string GetDocumentXml(byte[] docxBytes)
        {
            using var zip = new ZipArchive(new MemoryStream(docxBytes), ZipArchiveMode.Read);
            var entry = zip.GetEntry("word/document.xml")!;
            using var reader = new StreamReader(entry.Open(), Encoding.UTF8);
            return reader.ReadToEnd();
        }

        [Fact]
        public void TryGenerateReport_WithTwoServices_ReturnsTrue()
        {
            var service = new WriteOffOrderReportService(new StubReportDataService(), new CapturingFileStorageService());
            Assert.True(service.TryGenerateReport(BuildTestData()));
        }

        [Fact]
        public void TryGenerateReport_OutputIsValidDocx()
        {
            var capture = new CapturingFileStorageService();
            new WriteOffOrderReportService(new StubReportDataService(), capture).TryGenerateReport(BuildTestData());

            Assert.NotNull(capture.SavedBytes);
            Assert.True(capture.SavedBytes!.Length > 1000);

            using var zip = new ZipArchive(new MemoryStream(capture.SavedBytes), ZipArchiveMode.Read);
            Assert.NotNull(zip.GetEntry("word/document.xml"));
        }

        [Fact]
        public void TryGenerateReport_FormFieldsAreFilledInDocument()
        {
            var capture = new CapturingFileStorageService();
            new WriteOffOrderReportService(new StubReportDataService(), capture).TryGenerateReport(BuildTestData());

            var xml = GetDocumentXml(capture.SavedBytes!);

            // Form fields that are explicitly replaced before ReplaceFields(config)
            Assert.Contains("42",                                   xml); // REPORT_NUM
            Assert.Contains("28.05.2026",                          xml); // EVENT_DATE
            Assert.Contains("14:35",                                xml); // EVENT_TIME
            Assert.Contains("н.п. Тест",                           xml); // BATTLE_ORDER_LOCATION
            Assert.Contains("1 механізований батальйон",           xml); // SUBDIVISION_NAME
            Assert.Contains("солдата",                             xml); // REPORTER_RANK
            Assert.Contains("Тестового Тест Тестовича",            xml); // REPORTER_NAME
            Assert.Contains("Начальник служби",                    xml); // CREATOR_POSITION
            Assert.Contains("майор",                               xml); // CREATOR_RANK
            Assert.Contains("Іванов І.І.",                         xml); // CREATOR_NAME
            Assert.Contains("в/ч А1234",                           xml); // MIL_UNIT_APPROVAL
            Assert.Contains("бойового зіткнення",                  xml); // WHAT_HAPPENED
        }

        [Fact]
        public void TryGenerateReport_ServicesBlockContainsAllItems()
        {
            var capture = new CapturingFileStorageService();
            new WriteOffOrderReportService(new StubReportDataService(), capture).TryGenerateReport(BuildTestData());

            var xml = GetDocumentXml(capture.SavedBytes!);

            Assert.Contains("Інженерна служба",    xml);
            Assert.Contains("інженерної служби",   xml);
            Assert.Contains("Лопата саперна",      xml);
            Assert.Contains("Мотузка страховна",   xml);
            Assert.Contains("Служба зв",           xml); // зв'язку (apostrophe may be escaped in XML)
            Assert.Contains("Радіостанція",        xml);
        }

        [Fact]
        public void TryGenerateReport_TotalSumIsCorrect()
        {
            // 450 + 1200.50 + 35000 = 36 650,50
            var capture = new CapturingFileStorageService();
            new WriteOffOrderReportService(new StubReportDataService(), capture).TryGenerateReport(BuildTestData());

            var xml = GetDocumentXml(capture.SavedBytes!);

            // TOTAL_SUM field — formatted via ReportHelper.GetPriceString which uses Ukrainian culture
            Assert.Contains("36650",  xml);
        }

        [Fact]
        public void TryGenerateReport_WitnessesBlockContainsAllWitnesses()
        {
            var data = BuildTestData();
            data.Witnesses = [.. TestWitnesses];

            var capture = new CapturingFileStorageService();
            new WriteOffOrderReportService(new StubReportDataService(), capture).TryGenerateReport(data);

            var xml = GetDocumentXml(capture.SavedBytes!);

            Assert.Contains("УДАЛОВ",                                  xml);
            Assert.Contains("молодший сержант",                        xml);
            Assert.Contains("командир міномета",                       xml);
            Assert.Contains("КОВАЛЬОВ",                                xml);
            Assert.Contains("старший солдат",                          xml);
            Assert.Contains("майстер",                                 xml);
        }

        [Fact]
        public void TryGenerateReport_WitnessesBlockMergeFieldIsGone()
        {
            var data = BuildTestData();
            data.Witnesses = [.. TestWitnesses];

            var capture = new CapturingFileStorageService();
            new WriteOffOrderReportService(new StubReportDataService(), capture).TryGenerateReport(data);

            var xml = GetDocumentXml(capture.SavedBytes!);

            var remaining = Regex.Matches(xml, @"MERGEFIELD\s+&quot;?EVENT_WITNESSES&quot;?")
                                 .Concat(Regex.Matches(xml, @"MERGEFIELD\s+EVENT_WITNESSES"));
            Assert.Empty(remaining);
        }

        [Fact]
        public void TryGenerateReport_ServicesBlockMergeFieldIsGone()
        {
            var capture = new CapturingFileStorageService();
            new WriteOffOrderReportService(new StubReportDataService(), capture).TryGenerateReport(BuildTestData());

            var xml = GetDocumentXml(capture.SavedBytes!);

            // SERVICES_BLOCK merge field must be replaced wholesale — no instrText for it should remain
            var remaining = Regex.Matches(xml, @"MERGEFIELD\s+&quot;?SERVICES_BLOCK&quot;?")
                                 .Concat(Regex.Matches(xml, @"MERGEFIELD\s+SERVICES_BLOCK"));
            Assert.Empty(remaining);
        }
    }
}
