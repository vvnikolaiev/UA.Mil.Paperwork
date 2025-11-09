using Mil.Paperwork.Infrastructure.DataModels;
using Mil.Paperwork.Infrastructure.Helpers;

namespace Mil.Paperwork.Tests
{
    public class ConfigurationDataFilesTests
    {
        // Helper: walk up from test run directory to repository root (assumes solution file at repo root)
        private static string FindRepoRoot()
        {
            var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
            while (dir != null)
            {
                var sln = Path.Combine(dir.FullName, "Mil.Paperwork.WriteOff.sln");
                if (File.Exists(sln))
                    return dir.FullName;

                dir = dir.Parent;
            }

            throw new InvalidOperationException("Repository root (Mil.Paperwork.WriteOff.sln) not found from current directory.");
        }

        private static string ResolveDataFilePath(string relativePathFromRepoRoot)
        {
            var repoRoot = FindRepoRoot();
            return Path.GetFullPath(Path.Combine(repoRoot, relativePathFromRepoRoot.Replace('/', Path.DirectorySeparatorChar)));
        }

        [Fact]
        public void ReportDataConfig_Json_Should_Deserialize_To_ReportDataConfigDTO()
        {
            var relativePath = Path.Combine("Mil.Paperwork.Infrastructure", "Data", "ReportDataConfig.json");
            var filePath = ResolveDataFilePath(relativePath);

            Assert.True(File.Exists(filePath), $"Expected file at '{filePath}'");

            var json = File.ReadAllText(filePath);
            var config = JsonHelper.ReadJson<ReportDataConfigDTO>(json);

            Assert.NotNull(config);
            Assert.NotNull(config.Common);
            // Basic sanity checks for expected sections
            Assert.NotNull(config.QualityStateReport);
            Assert.NotNull(config.TechnicalStateReport);
            Assert.NotNull(config.ResidualValueReport);
        }

        [Fact]
        public void SimpleDataStorage_Json_Should_Deserialize_To_SimpleDataStorageDTO()
        {
            var relativePath = Path.Combine("Mil.Paperwork.Infrastructure", "Data", "SimpleDataStorage.json");
            var filePath = ResolveDataFilePath(relativePath);

            Assert.True(File.Exists(filePath), $"Expected file at '{filePath}'");

            var json = File.ReadAllText(filePath);
            var storage = JsonHelper.ReadJson<SimpleDataStorageDTO>(json);

            Assert.NotNull(storage);
            Assert.NotNull(storage.ProductsData);
            Assert.NotNull(storage.ValuationData);
            Assert.NotNull(storage.PeopleData);
            Assert.NotNull(storage.MeasurementUnits);

            // Optional: ensure collections are present (could be empty)
            Assert.True(storage.ProductsData.Count >= 0);
            Assert.True(storage.MeasurementUnits.Count >= 0);
        }
    }
}