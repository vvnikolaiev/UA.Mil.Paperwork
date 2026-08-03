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
    public class EASReportTests
    {
        private sealed class StubReportDataService : IReportDataService
        {
            public Dictionary<string, string> GetReportParametersDictionary(ReportType _)
            {
                Dictionary<string, string> result = [];
                return result;
            }

            public List<ReportParameter> GetReportParameters(ReportType _, bool __ = false)
            {
                List<ReportParameter> result = [];
                return result;
            }

            public Dictionary<string, string> GetServiceReportParametersDictionary(string? _ = null)
            {
                Dictionary<string, string> result = [];
                return result;
            }

            public List<ReportParameter> GetServiceReportParameters(string? _ = null, bool __ = false)
            {
                List<ReportParameter> result = [];
                return result;
            }

            public Dictionary<string, MilitaryServiceDTO> GetAllServices(bool _ = false)
            {
                Dictionary<string, MilitaryServiceDTO> result = [];
                return result;
            }

            public string GetSelectedService(bool _ = false)
            {
                return string.Empty;
            }

            public CommissionDTO GetCommissionData(CommissionType _, bool __ = false)
            {
                var result = new CommissionDTO();
                return result;
            }

            public AssetType GetAssetType()
            {
                return AssetType.Default;
            }

            public ICommisionsConfigSection GetCommissionsConfig()
            {
                return null!;
            }

            public CommissionDTO GetCommission(ReportType _)
            {
                var result = new CommissionDTO();
                return result;
            }

            public bool ImportReportConfig(ReportDataConfigDTO _)
            {
                return true;
            }

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

            public string SaveFile(string path, byte[] bytes)
            {
                SavedBytes = bytes;
                return path;
            }

            public T? ReadJsonFile<T>(string _, string? __ = null)
            {
                return default;
            }

            public void WriteJsonToFile<T>(T _, string __, string? ___ = null) { }
            public void WriteJsonToFile<T>(T _, string __) { }
        }

        private static PersonDTO[] TestWitnesses =>
        [
            new() { Rank = "старший солдат", LastName = "Ремха", FirstName = "Тарас", Patronymic = "Юрійович", Position = "снайпер розвідувального відділення" },
            new() { Rank = "рядовий", LastName = "Фатенко", FirstName = "Володимир", Patronymic = "Миколайович", Position = "навідник-оператор розвідувального відділення" }
        ];

        private static EASReportData BuildTestData()
        {
            var result = new EASReportData
            {
                ReportNum = "17",
                ReportDate = new DateTime(2026, 6, 1),
                EventDate = new DateTime(2026, 5, 28),
                EventTime = "14:35",
                BattleOrder = "7",
                BattleOrderDate = new DateTime(2026, 5, 20),
                SubdivisionName = "1 механізований батальйон",
                ReporterRank = "солдата",
                ReporterName = "Тестового Тест Тестовича",
                WhatHappened = "внаслідок бойового зіткнення",
                OrdenNum = "5",
                OrdenDate = new DateTime(2026, 5, 30),
                Services =
                [
                    new EASServiceData
                    {
                        ServiceName = "Служба зв'язку",
                        ServiceNameGenitive = "служби зв'язку та кібербезпеки",
                        HeadRank = "старший лейтенант",
                        HeadName = "Олександр Шупер",
                        HeadPosition = "Начальник групи зв'язку та кібербезпеки",
                        Assets =
                        [
                            new() { Name = "Планшет Samsung Galaxy Tab Active 3", Code = "-", MeasurementUnit = "шт.", Category = 2, Count = 1, OriginalPrice = 15519.99m, ResidualPrice = 10131.45m },
                            new() { Name = "Карта пам'яті 128 GB", Code = "-", MeasurementUnit = "шт.", Category = 2, Count = 1, OriginalPrice = 303.42m, ResidualPrice = 173.31m }
                        ]
                    },
                    new EASServiceData
                    {
                        ServiceName = "Речова служба",
                        ServiceNameGenitive = "речової служби",
                        HeadRank = "сержант",
                        HeadName = "Тетяна Коробка",
                        HeadPosition = "Начальник речової служби",
                        Assets =
                        [
                            new() { Name = "Телевізор LG 32LQ630B6LA", Code = "Д3000000Y", MeasurementUnit = "шт.", Category = 2, Count = 1, OriginalPrice = 9500.00m, ResidualPrice = 8527.20m }
                        ]
                    }
                ],
                DestinationFolder = Path.GetTempPath()
            };
            return result;
        }

        private static string GetDocumentXml(byte[] docxBytes)
        {
            using var zip = new ZipArchive(new MemoryStream(docxBytes), ZipArchiveMode.Read);
            var entry = zip.GetEntry("word/document.xml")!;
            using var reader = new StreamReader(entry.Open(), Encoding.UTF8);
            var result = reader.ReadToEnd();
            return result;
        }

        [Fact]
        public void TryGenerateReport_WithTwoServices_ReturnsTrue()
        {
            var service = new EASReportService(new StubReportDataService(), new CapturingFileStorageService());
            var result = service.TryGenerateReport(BuildTestData());

            Assert.True(result);
            var outputFile = Assert.Single(result.OutputFiles);
            Assert.EndsWith(".docx", outputFile);
        }

        [Fact]
        public void TryGenerateReport_OutputIsValidDocx()
        {
            var capture = new CapturingFileStorageService();
            new EASReportService(new StubReportDataService(), capture).TryGenerateReport(BuildTestData());

            Assert.NotNull(capture.SavedBytes);
            Assert.True(capture.SavedBytes!.Length > 1000);

            using var zip = new ZipArchive(new MemoryStream(capture.SavedBytes), ZipArchiveMode.Read);
            Assert.NotNull(zip.GetEntry("word/document.xml"));
        }

        [Fact]
        public void TryGenerateReport_FormFieldsAreFilledInDocument()
        {
            var capture = new CapturingFileStorageService();
            new EASReportService(new StubReportDataService(), capture).TryGenerateReport(BuildTestData());

            var xml = GetDocumentXml(capture.SavedBytes!);

            Assert.Contains("17", xml); // REPORT_NUM
            Assert.Contains("28.05.2026", xml); // EVENT_DATE
            Assert.Contains("14:35", xml); // EVENT_TIME
            Assert.Contains("1 механізований батальйон", xml); // SUBDIVISION_NAME
            Assert.Contains("солдата", xml); // REPORTER_RANK
            Assert.Contains("Тестового Тест Тестовича", xml); // REPORTER_NAME
            Assert.Contains("бойового зіткнення", xml); // WHAT_HAPPENED
            Assert.Contains("30.05.2026", xml); // ORDEN_DATE
        }

        [Fact]
        public void TryGenerateReport_AssetsTableContainsAllGroupsAndNoLeftoverExampleData()
        {
            var capture = new CapturingFileStorageService();
            new EASReportService(new StubReportDataService(), capture).TryGenerateReport(BuildTestData());

            var xml = GetDocumentXml(capture.SavedBytes!);

            Assert.Contains("номенклатура служби зв", xml);
            Assert.Contains("номенклатура речової служби", xml);
            Assert.Contains("Планшет Samsung Galaxy Tab Active 3", xml);
            Assert.Contains("Телевізор LG 32LQ630B6LA", xml);
            Assert.Contains("Усього за зазначеною номенклатурою", xml);
            Assert.Contains("Усього", xml);

            // Leftover example data shipped in the template must be fully replaced.
            Assert.DoesNotContain("Киркомотига", xml);
            Assert.DoesNotContain("EnerSol", xml);
            Assert.DoesNotContain("номенклатура служби забезпечення засобами зв", xml);
        }

        [Fact]
        public void TryGenerateReport_AssetsTableTotalsAreCorrect()
        {
            // group1 = 10131.45 + 173.31 = 10304.76 ; group2 = 8527.20 ; grand total = 18831.96
            var capture = new CapturingFileStorageService();
            new EASReportService(new StubReportDataService(), capture).TryGenerateReport(BuildTestData());

            var xml = GetDocumentXml(capture.SavedBytes!);

            Assert.Contains("10 304,76", xml);
            Assert.Contains("8 527,20", xml);
            Assert.Contains("18 831,96", xml);
        }

        [Fact]
        public void TryGenerateReport_WitnessesBlockAndTextContainAllWitnesses()
        {
            var data = BuildTestData();
            data.Witnesses = [.. TestWitnesses];

            var capture = new CapturingFileStorageService();
            new EASReportService(new StubReportDataService(), capture).TryGenerateReport(data);

            var xml = GetDocumentXml(capture.SavedBytes!);

            Assert.Contains("Ремха", xml);
            Assert.Contains("Фатенко", xml);
            Assert.Contains("снайпер розвідувального відділення", xml);
        }

        [Fact]
        public void TryGenerateReport_HeadsOfServicesBlockContainsAllHeads()
        {
            var capture = new CapturingFileStorageService();
            new EASReportService(new StubReportDataService(), capture).TryGenerateReport(BuildTestData());

            var xml = GetDocumentXml(capture.SavedBytes!);

            Assert.Contains("Олександр Шупер", xml);
            Assert.Contains("Тетяна Коробка", xml);
            Assert.Contains("Начальник групи зв", xml);
            Assert.Contains("Начальник речової служби", xml);
        }

        [Fact]
        public void TryGenerateReport_BlockMergeFieldsAreGone()
        {
            var data = BuildTestData();
            data.Witnesses = [.. TestWitnesses];

            var capture = new CapturingFileStorageService();
            new EASReportService(new StubReportDataService(), capture).TryGenerateReport(data);

            var xml = GetDocumentXml(capture.SavedBytes!);

            // Only fields filled via ReplaceFieldWithBlock have their MERGEFIELD instruction fully
            // removed (the whole placeholder paragraph is replaced). EVENT_WITNESSES_TEXT uses plain
            // ReplaceField, which keeps the field code and only swaps the displayed result text.
            foreach (var field in new[] { "EVENT_WITNESSES_BLOCK", "HEADS_OF_SERVICES_BLOCK" })
            {
                var remaining = Regex.Matches(xml, $@"MERGEFIELD\s+&quot;?{field}&quot;?")
                                     .Concat(Regex.Matches(xml, $@"MERGEFIELD\s+{field}\b"));
                Assert.Empty(remaining);
            }
        }
    }
}
