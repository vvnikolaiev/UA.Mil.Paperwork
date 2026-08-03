using Mil.Paperwork.Common.DataModels;
using Mil.Paperwork.Common.Enums;
using Mil.Paperwork.Common.Factories;
using Mil.Paperwork.Common.Strategies;
using Mil.Paperwork.Domain.DataModels;

namespace Mil.Paperwork.Tests
{
    public class EASAssetsImportStrategyTests
    {
        private const string ColumnName = "Найменування";
        private const string ColumnSerialNumber = "Серійний номер";
        private const string ColumnCode = "Код номенклатури";
        private const string ColumnMeasurementUnit = "Од. вим.";
        private const string ColumnCategory = "Категорія";
        private const string ColumnCount = "К-сть";
        private const string ColumnOriginalPrice = "Первісна вартість";
        private const string ColumnResidualPrice = "Залишкова вартість";

        private static IImportStrategy CreateStrategy()
        {
            var strategy = ImportStrategyFactory.GetStrategy(null!, ImportType.EASAssets);
            return strategy;
        }

        // Maps every EAS column onto an identically named source column.
        private static List<ImportColumnDefinition> MapAllColumns(IImportStrategy strategy)
        {
            var columns = strategy.GetColumnsToMap();
            foreach (var column in columns)
            {
                column.SelectedSourceColumn = column.Title;
            }

            return columns;
        }

        [Fact]
        public void GetColumnsToMap_ReturnsAllEightColumns_WithOnlyNameRequired()
        {
            var strategy = CreateStrategy();

            var columns = strategy.GetColumnsToMap();

            var expectedTitles = new[]
            {
                ColumnName, ColumnCode, ColumnMeasurementUnit,
                ColumnCategory, ColumnCount, ColumnOriginalPrice, ColumnResidualPrice
            };
            Assert.Equal(expectedTitles, columns.Select(c => c.Title));
            Assert.Equal([ColumnName], columns.Where(c => c.IsRequired).Select(c => c.Title));
        }

        [Fact]
        public void ImportData_MapsEveryFieldOntoTheAssetData()
        {
            var strategy = CreateStrategy();
            var columns = MapAllColumns(strategy);
            var rows = new List<Dictionary<string, object>>
            {
                new()
                {
                    [ColumnName] = "Радіостанція Motorola",
                    [ColumnCode] = "5820-01-123",
                    [ColumnMeasurementUnit] = "шт",
                    [ColumnCategory] = 4,
                    [ColumnCount] = 3,
                    [ColumnOriginalPrice] = 12500.50m,
                    [ColumnResidualPrice] = 3001.25m
                }
            };

            var result = strategy.ImportData(columns, rows);

            Assert.True(result.IsSuccessful);
            Assert.Equal(1, result.ImportedRowsCount);
            Assert.Equal(0, result.InvalidRowsCount);

            var asset = Assert.IsType<EASAssetData>(Assert.Single(result.Rows));
            Assert.Equal("Радіостанція Motorola", asset.Name);
            Assert.Equal("5820-01-123", asset.Code);
            Assert.Equal("шт", asset.MeasurementUnit);
            Assert.Equal(4, asset.Category);
            Assert.Equal(3, asset.Count);
            Assert.Equal(12500.50m, asset.OriginalPrice);
            Assert.Equal(3001.25m, asset.ResidualPrice);
        }

        [Fact]
        public void ImportData_DropsRowsMissingTheRequiredNameColumn()
        {
            var strategy = CreateStrategy();
            var columns = MapAllColumns(strategy);
            var rows = new List<Dictionary<string, object>>
            {
                new() { [ColumnName] = "Акумулятор", [ColumnResidualPrice] = 500m },
                new() { [ColumnResidualPrice] = 700m }
            };

            var result = strategy.ImportData(columns, rows);

            Assert.True(result.IsSuccessful);
            Assert.Equal(2, result.ImportedRowsCount);
            Assert.Equal(1, result.InvalidRowsCount);

            var asset = Assert.IsType<EASAssetData>(Assert.Single(result.Rows));
            Assert.Equal("Акумулятор", asset.Name);
        }

        [Fact]
        public void ImportData_DropsRowsWithANonNumericPrice()
        {
            var strategy = CreateStrategy();
            var columns = MapAllColumns(strategy);
            var rows = new List<Dictionary<string, object>>
            {
                new() { [ColumnName] = "Кабель", [ColumnResidualPrice] = 120m },
                new() { [ColumnName] = "Антена", [ColumnResidualPrice] = "не вказано" }
            };

            var result = strategy.ImportData(columns, rows);

            Assert.Equal(1, result.InvalidRowsCount);

            var asset = Assert.IsType<EASAssetData>(Assert.Single(result.Rows));
            Assert.Equal("Кабель", asset.Name);
        }

        [Fact]
        public void ImportData_UsesDefaults_ForColumnsTheUserDidNotMap()
        {
            var strategy = CreateStrategy();
            var columns = strategy.GetColumnsToMap();
            var nameColumn = columns.Single(c => c.Title == ColumnName);
            nameColumn.SelectedSourceColumn = ColumnName;
            var rows = new List<Dictionary<string, object>>
            {
                new() { [ColumnName] = "Ноутбук" }
            };

            var result = strategy.ImportData(columns, rows);

            var asset = Assert.IsType<EASAssetData>(Assert.Single(result.Rows));
            Assert.Equal("Ноутбук", asset.Name);
            Assert.Equal(1, asset.Count);
            Assert.Equal(2, asset.Category);
            Assert.Equal(string.Empty, asset.MeasurementUnit);
            Assert.Equal(0m, asset.ResidualPrice);
        }
    }
}
