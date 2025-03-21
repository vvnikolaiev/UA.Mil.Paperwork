using Mil.Paperwork.Domain.Helpers;
using System.Globalization;

namespace Mil.Paperwork.Tests.Helpers
{
    public class ReportHelperTests
    {
        public ReportHelperTests()
        {
            var culture = new CultureInfo("uk-UA");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }

        [Theory]
        [InlineData(1, "Всього: 1 (одне) найменування")]
        [InlineData(2, "Всього: 2 (два) найменування")]
        [InlineData(3, "Всього: 3 (три) найменування")]
        [InlineData(4, "Всього: 4 (чотири) найменування")]
        [InlineData(5, "Всього: 5 (п'ять) найменувань")]
        [InlineData(9, "Всього: 9 (дев'ять) найменувань")]
        [InlineData(10, "Всього: 10 (десять) найменувань")]
        [InlineData(11, "Всього: 11 (одинадцять) найменувань")]
        [InlineData(21, "Всього: 21 (двадцять одне) найменування")]
        [InlineData(100, "Всього: 100 (сто) найменувань")]
        [InlineData(101, "Всього: 101 (сто одне) найменування")]
        [InlineData(1000, "Всього: 1000 (одна тисяча) найменувань")]
        public void ConvertNamesNumberToReportString_ShouldReturnCorrectString(int number, string expected)
        {
            var result = ReportHelper.ConvertNamesNumberToReportString(number);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(1.25, "1грн. 25коп. (одна гривня двадцять п'ять копійок)")]
        [InlineData(2.50, "2грн. 50коп. (дві гривні п'ятдесят копійок)")]
        [InlineData(12.43, "12грн. 43коп. (дванадцять гривень сорок три копійки)")]
        [InlineData(5.75, "5грн. 75коп. (п'ять гривень сімдесят п'ять копійок)")]
        [InlineData(11.99, "11грн. 99коп. (одинадцять гривень дев'яносто дев'ять копійок)")]
        [InlineData(21.01, "21грн. 1коп. (двадцять одна гривня одна копійка)")]
        [InlineData(100.10, "100грн. 10коп. (сто гривень десять копійок)")]
        [InlineData(113.79, "113грн. 79коп. (сто тринадцять гривень сімдесят дев'ять копійок)")]
        [InlineData(111.20, "111грн. 20коп. (сто одинадцять гривень двадцять копійок)")]
        [InlineData(4913.83, "4913грн. 83коп. (чотири тисячі дев'ятсот тринадцять гривень вісімдесят три копійки)")]
        [InlineData(5712, "5712грн. 0коп. (п'ять тисяч сімсот дванадцять гривень нуль копійок)")]
        [InlineData(6001, "6001грн. 0коп. (шість тисяч одна гривня нуль копійок)")]
        [InlineData(6723.31, "6723грн. 31коп. (шість тисяч сімсот двадцять три гривні тридцять одна копійка)")]
        [InlineData(79533.9, "79533грн. 90коп. (сімдесят дев'ять тисяч п'ятсот тридцять три гривні дев'яносто копійок)")]
        [InlineData(89125.64, "89125грн. 64коп. (вісімдесят дев'ять тисяч сто двадцять п'ять гривень шістдесят чотири копійки)")]
        [InlineData(653412.05, "653412грн. 5коп. (шістсот п'ятдесят три тисячі чотириста дванадцять гривень п'ять копійок)")]
        [InlineData(123456.78, "123456грн. 78коп. (сто двадцять три тисячі чотириста п'ятдесят шість гривень сімдесят вісім копійок)")]
        [InlineData(987654.32, "987654грн. 32коп. (дев'ятсот вісімдесят сім тисяч шістсот п'ятдесят чотири гривні тридцять дві копійки)")]
        [InlineData(123456789.01, "123456789грн. 1коп. (сто двадцять три мільйони чотириста п'ятдесят шість тисяч сімсот вісімдесят дев'ять гривень одна копійка)")]
        [InlineData(987654321.99, "987654321грн. 99коп. (дев'ятсот вісімдесят сім мільйонів шістсот п'ятдесят чотири тисячі триста двадцять одна гривня дев'яносто дев'ять копійок)")]
        public void ConvertTotalSumToUkrainianString_ShouldReturnCorrectString(decimal totalSum, string expected)
        {
            var result = ReportHelper.ConvertTotalSumToUkrainianString(totalSum);
            Assert.Equal(expected, result);
        }


        [Theory]
        [InlineData("01.01.2023", "01.02.2023", "1 місяць")]
        [InlineData("01.01.2023", "05.03.2023", "2 місяці")]
        [InlineData("01.01.2023", "01.04.2023", "3 місяці")]
        [InlineData("01.01.2023", "01.05.2023", "4 місяці")]
        [InlineData("01.01.2023", "01.06.2023", "5 місяців")]
        [InlineData("11.01.2023", "18.12.2023", "11 місяців")]
        [InlineData("11.01.2023", "8.11.2023", "10 місяців")]
        public void GetMonthsOperatedText_ShouldReturnCorrectString(string startDateText, string endDateText, string expected)
        {
            var startDate = DateTime.Parse(startDateText);
            var endDate = DateTime.Parse(endDateText);

            var result = ReportHelper.GetMonthsOperatedText(startDate, endDate);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("01.01.2023", "01.02.2023", "720 годин")]
        [InlineData("01.01.2023", "05.03.2023", "1440 годин")]
        [InlineData("01.01.2023", "01.04.2023", "2160 годин")]
        [InlineData("01.01.2023", "01.05.2023", "2880 годин")]
        [InlineData("01.01.2023", "01.06.2023", "3600 годин")]
        [InlineData("11.01.2023", "18.12.2023", "7920 годин")]
        [InlineData("11.01.2023", "8.11.2023", "7200 годин")]
        public void GetHoursOperatedText_ShouldReturnCorrectString(string startDateText, string endDateText, string expected)
        {
            var startDate = DateTime.Parse(startDateText);
            var endDate = DateTime.Parse(endDateText);
            var result = ReportHelper.GetHoursOperatedText(startDate, endDate);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(1, "1 рік")]
        [InlineData(2, "2 роки")]
        [InlineData(3, "3 роки")]
        [InlineData(4, "4 роки")]
        [InlineData(5, "5 років")]
        [InlineData(10, "10 років")]
        [InlineData(11, "11 років")]
        public void GetWarrantyPeriodText_ShouldReturnCorrectString(int warrantyPeriodYears, string expected)
        {
            var result = ReportHelper.GetWarrantyPeriodText(warrantyPeriodYears);
            Assert.Equal(expected, result);
        }
    }
}
