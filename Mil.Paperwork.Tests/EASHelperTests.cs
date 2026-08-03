using Mil.Paperwork.Domain.DataModels;
using Mil.Paperwork.Domain.Helpers;
using Mil.Paperwork.Infrastructure.DataModels;

namespace Mil.Paperwork.Tests
{
    public class EASHelperTests
    {
        private static PersonDTO[] TestWitnesses =>
        [
            new() { Rank = "старший солдат", LastName = "Ремха", FirstName = "Тарас", Patronymic = "Юрійович", Position = "снайпер" },
            new() { Rank = "рядовий", LastName = "Фатенко", FirstName = "Володимир", Patronymic = "Миколайович", Position = "навідник-оператор" }
        ];

        [Fact]
        public void BuildWitnessesText_JoinsWitnessesWithSemicolonAndUppercasesSurname()
        {
            var result = EASHelper.BuildWitnessesText([.. TestWitnesses], "А4682");

            Assert.Equal(
                "старший солдат РЕМХА Тарас Юрійович, снайпер військової частини А4682; " +
                "рядовий ФАТЕНКО Володимир Миколайович, навідник-оператор військової частини А4682",
                result);
        }

        [Fact]
        public void BuildWitnessesText_EmptyList_ReturnsEmptyString()
        {
            var result = EASHelper.BuildWitnessesText([], "А4682");

            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void BuildWitnessesBlock_ProducesTwoParagraphsPerWitness()
        {
            var result = EASHelper.BuildWitnessesBlock([.. TestWitnesses], "А4682");

            Assert.Equal(4, result.Count);
            Assert.Equal("снайпер військової частини А4682,", result[0].Text);
            Assert.Equal("старший солдат\tТарас Ремха", result[1].Text);
        }

        [Fact]
        public void BuildHeadsOfServicesBlock_ProducesTwoParagraphsPerService()
        {
            var services = new List<EASServiceData>
            {
                new() { HeadPosition = "Начальник служби", HeadRank = "сержант", HeadName = "Тетяна Коробка" }
            };

            var result = EASHelper.BuildHeadsOfServicesBlock(services, "А4682");

            Assert.Equal(2, result.Count);
            Assert.Equal("Начальник служби військової частини А4682,", result[0].Text);
            Assert.Equal("сержант\tТетяна Коробка", result[1].Text);
            Assert.True(result[0].IsBold);
            Assert.True(result[1].IsBold);
        }

        private static EASServiceData BuildTwoAssetService()
        {
            return new EASServiceData
            {
                Assets =
                [
                    new() { Count = 2, ResidualPrice = 100.555m },
                    new() { Count = 1, ResidualPrice = 50.001m }
                ]
            };
        }

        [Fact]
        public void CalculateAssetSum_RoundsToTwoDecimals()
        {
            var asset = new EASAssetData { Count = 3, ResidualPrice = 10.005m };

            var result = EASHelper.CalculateAssetSum(asset);

            Assert.Equal(30.02m, result);
        }

        [Fact]
        public void CalculateGroupSubtotal_SumsAllAssetSums()
        {
            var service = BuildTwoAssetService();

            // asset1: 2 * 100.555 = 201.11 ; asset2: 1 * 50.001 = 50.00 ; total = 251.11
            var result = EASHelper.CalculateGroupSubtotal(service);

            Assert.Equal(251.11m, result);
        }

        [Fact]
        public void CalculateGrandTotal_SumsAllGroupSubtotals()
        {
            var services = new List<EASServiceData> { BuildTwoAssetService(), BuildTwoAssetService() };

            var result = EASHelper.CalculateGrandTotal(services);

            Assert.Equal(502.22m, result);
        }

        [Fact]
        public void CalculateGrandTotalCount_SumsAllAssetCounts()
        {
            var services = new List<EASServiceData> { BuildTwoAssetService(), BuildTwoAssetService() };

            var result = EASHelper.CalculateGrandTotalCount(services);

            Assert.Equal(6, result);
        }
    }
}
