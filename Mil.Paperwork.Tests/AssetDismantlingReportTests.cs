using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Domain.Services;
using Mil.Paperwork.Infrastructure.DataModels;
using Mil.Paperwork.Infrastructure.DataModels.Configuration;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Services;
using System.IO.Compression;
using System.Text;

namespace Mil.Paperwork.Tests
{
    public class AssetDismantlingReportTests
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
            public List<byte[]> SavedBytes { get; } = [];

            public string SaveFile(string path, byte[] bytes)
            {
                SavedBytes.Add(bytes);
                return path;
            }

            public T? ReadJsonFile<T>(string _, string? __ = null)
            {
                return default;
            }

            public void WriteJsonToFile<T>(T _, string __, string? ___ = null) { }
            public void WriteJsonToFile<T>(T _, string __) { }
        }

        private static DismantlingReportData BuildTestData()
        {
            var assetDismantlingData = new AssetDismantlingData
            {
                Name = "Радіостанція Р-187П1",
                SerialNumber = "СН-100",
                NomenclatureCode = "rs187",
                ShortName = "Р-187",
                RegistrationNumber = "Р-9",
                DocumentNumber = "21",
                Reason = "Втрата комплектуючих",
                Category = 2,
                Price = 35000.00m,
                AssetComponents =
                [
                    new AssetComponent { Name = "Акумулятор", Unit = "шт", NomenclatureCode = "bat1", Quantity = 1, Category = 2, Price = 1200.00m, Exclude = true },
                    new AssetComponent { Name = "Антена", Unit = "шт", NomenclatureCode = "ant1", Quantity = 2, Category = 2, Price = 300.00m, Exclude = false },
                    new AssetComponent { Name = "Гарнітура", Unit = "шт", NomenclatureCode = "hs1", Quantity = 1, Category = 2, Price = 450.00m, Exclude = false }
                ]
            };

            var result = new DismantlingReportData
            {
                Dismantlings = [assetDismantlingData],
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
        public void TryGenerateReport_WithOneDismantling_ReturnsTrue()
        {
            var service = new AssetDismantlingReportService(new StubReportDataService(), new CapturingFileStorageService());
            var result = service.TryGenerateReport(BuildTestData());

            Assert.True(result);
            var outputFile = Assert.Single(result.OutputFiles);
            Assert.EndsWith(".docx", outputFile);
        }

        [Fact]
        public void TryGenerateReport_TableContainsBothComponentRows()
        {
            var capture = new CapturingFileStorageService();
            new AssetDismantlingReportService(new StubReportDataService(), capture).TryGenerateReport(BuildTestData());

            var xml = GetDocumentXml(capture.SavedBytes.Single());

            // excluded component is sorted last, so "Антена" (not excluded) gets row "1.1", "Акумулятор" gets "1.2"
            Assert.Contains("Антена", xml);
            Assert.Contains("ANT1", xml);
            Assert.Contains("Акумулятор", xml);
            Assert.Contains("BAT1", xml);
            Assert.Contains(">1.1<", xml);
            Assert.Contains(">1.2<", xml);
        }

        [Fact]
        public void TryGenerateReport_AssetColumnsAreFilledOnce()
        {
            // asset-level columns are vertically merged across all 3 component rows — the asset name is written
            // once into the merged cell (plus once via the ASSET_NAME field), never once per component row
            var capture = new CapturingFileStorageService();
            new AssetDismantlingReportService(new StubReportDataService(), capture).TryGenerateReport(BuildTestData());

            var xml = GetDocumentXml(capture.SavedBytes.Single());

            var componentCount = BuildTestData().Dismantlings.Single().AssetComponentsCount;
            var occurrences = xml.Split("Радіостанція Р-187П1").Length - 1;
            Assert.True(occurrences < componentCount, $"expected fewer than {componentCount} occurrences (merged cell + field), found {occurrences}");
        }

        [Fact]
        public void TryGenerateReport_SummaryRowContainsTotalText()
        {
            var capture = new CapturingFileStorageService();
            new AssetDismantlingReportService(new StubReportDataService(), capture).TryGenerateReport(BuildTestData());

            var xml = GetDocumentXml(capture.SavedBytes.Single());

            Assert.Contains("на суму", xml);
        }
    }
}
