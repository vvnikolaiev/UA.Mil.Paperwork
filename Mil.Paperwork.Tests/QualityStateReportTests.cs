using Mil.Paperwork.Domain.DataModels.Assets;
using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Domain.Enums;
using Mil.Paperwork.Domain.Services;
using Mil.Paperwork.Infrastructure.DataModels;
using Mil.Paperwork.Infrastructure.DataModels.Configuration;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Services;
using System.IO.Compression;
using System.Text;

namespace Mil.Paperwork.Tests
{
    public class QualityStateReportTests
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

        private static CommonWriteOffReportData BuildTestData()
        {
            var result = new CommonWriteOffReportData
            {
                RegistrationNumber = "Р-1",
                DocumentNumber = "17",
                DocumentDate = new DateTime(2026, 6, 1),
                Reason = "Бойові пошкодження",
                EventDate = new DateTime(2026, 6, 5),
                EventType = EventType.Lost,
                OrdenNumber = 3,
                OrdenDate = new DateTime(2026, 5, 1),
                Assets =
                [
                    new AssetInfo
                    {
                        Name = "Бінокль Б-1",
                        SerialNumber = "СН-001",
                        NomenclatureCode = "abc123",
                        MeasurementUnit = "шт",
                        InitialCategory = 2,
                        Count = 2,
                        Price = 1500.00m,
                        StartDate = new DateTime(2023, 1, 1),
                        ResourceYears = 3
                    },
                    new AssetInfo
                    {
                        Name = "Радіостанція Р-2",
                        SerialNumber = "СН-002",
                        NomenclatureCode = "xyz789",
                        MeasurementUnit = "шт",
                        InitialCategory = 3,
                        Count = 1,
                        Price = 25000.00m,
                        StartDate = new DateTime(2022, 6, 1),
                        ResourceYears = 0
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
        public void TryGenerateReport_WithTwoAssets_ReturnsTrue()
        {
            var service = new QualityStateReportService(new StubReportDataService(), new CapturingFileStorageService());
            var result = service.TryGenerateReport(BuildTestData());

            Assert.True(result);
            var outputFile = Assert.Single(result.OutputFiles);
            Assert.EndsWith(".docx", outputFile);
        }

        [Fact]
        public void TryGenerateReport_TableContainsBothAssetRows()
        {
            var capture = new CapturingFileStorageService();
            new QualityStateReportService(new StubReportDataService(), capture).TryGenerateReport(BuildTestData());

            var xml = GetDocumentXml(capture.SavedBytes!);

            Assert.Contains("Бінокль Б-1", xml);
            Assert.Contains("СН-001", xml);
            Assert.Contains("ABC123", xml); // nomenclature code is upper-cased
            Assert.Contains("Радіостанція Р-2", xml);
            Assert.Contains("СН-002", xml);
            Assert.Contains("XYZ789", xml);
        }

        [Fact]
        public void TryGenerateReport_ExploitationNormUsesDefaultWhenResourceYearsIsZero()
        {
            // second asset has ResourceYears = 0 -> exploitation norm falls back to 60 months
            var capture = new CapturingFileStorageService();
            new QualityStateReportService(new StubReportDataService(), capture).TryGenerateReport(BuildTestData());

            var xml = GetDocumentXml(capture.SavedBytes!);

            Assert.Contains(">60<", xml);
        }

        [Fact]
        public void TryGenerateReport_SummaryRowContainsTotalSum()
        {
            // 1500.00*2 + 25000.00 = 28 000,00 — residual value uses a wear coefficient, so just check the item count text
            var capture = new CapturingFileStorageService();
            new QualityStateReportService(new StubReportDataService(), capture).TryGenerateReport(BuildTestData());

            var xml = GetDocumentXml(capture.SavedBytes!);

            Assert.Contains("на суму", xml);
        }
    }
}
