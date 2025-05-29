using Mil.Paperwork.Domain.Helpers;

namespace Mil.Paperwork.Tests.Helpers
{
    public class ResidualPriceHelperTests
    {
        [Theory]
        [InlineData(1000, 1.02, 0.99, 5, 5049)]
        [InlineData(999, 1.111, 0.8, 7, 6215.37)]
        [InlineData(111, 1.331, 0.677, 9, 900.18)]
        [InlineData(99512, 2.367, 0.111, 99, 2588402.52)]
        [InlineData(65341, 3.123, 0.77, 19, 2985396.85)]
        public void CalculateResidualPrice_ShouldReturnCorrectResidualPrice(
            decimal price,
            decimal indexationCoeff,
            decimal skzCoeff,
            int count,
            decimal expectedResidualPrice)
        {
            // Act
            var result = ResidualPriceHelper.CalculateResidualPrice(price, indexationCoeff, skzCoeff, count);
            // Assert
            Assert.Equal(expectedResidualPrice, result);
        }


        [Theory]
        [InlineData(1900, 2025, 5.768)]
        [InlineData(1997, 2025, 5.768)]
        [InlineData(1998, 2025, 3.438)]
        [InlineData(2015, 2015, 1.0)]
        [InlineData(2015, 2024, 1.65)]
        [InlineData(2023, 2025, 1.02)]
        [InlineData(2025, 2025, 1.0)]
        public void GetIndexationCoefficient_ReturnsExpectedValue(int startYear, int endYear, decimal expected)
        {
            // Arrange
            var startDate = new DateTime(startYear, 1, 1);
            var endDate = new DateTime(endYear, 1, 1);

            // Act
            var result = CoefficientsHelper.GetIndexationCoefficient(startDate, endDate);

            // Assert
            Assert.Equal(expected, result, 3);
        }

        [Fact]
        public void GetIndexationCoefficient_EndDateIsNull_Returns1()
        {
            // Arrange
            var startDate = new DateTime(2010, 1, 1);

            // Act
            var result = CoefficientsHelper.GetIndexationCoefficient(startDate, null);

            // Assert
            Assert.Equal(1.0m, result);
        }

        [Fact]
        public void GetIndexationCoefficient_StartYearBelowMin_UsesMinYear()
        {
            // Arrange
            var startDate = new DateTime(1980, 1, 1); // < 1997
            var endDate = new DateTime(2025, 1, 1);

            // Act
            var result = CoefficientsHelper.GetIndexationCoefficient(startDate, endDate);

            // Assert
            Assert.Equal(5.768m, result); // 1997-2025
        }

        [Fact]
        public void GetIndexationCoefficient_YearNotInTable_Returns1()
        {
            // Arrange
            var startDate = new DateTime(2050, 1, 1); // not in table
            var endDate = new DateTime(2051, 1, 1);   // not in table

            // Act
            var result = CoefficientsHelper.GetIndexationCoefficient(startDate, endDate);

            // Assert
            Assert.Equal(1.0m, result);
        }
    }
}
