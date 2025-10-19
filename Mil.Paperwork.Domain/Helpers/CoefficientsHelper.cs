namespace Mil.Paperwork.Domain.Helpers
{
    public static class CoefficientsHelper
    {
        // https://zakon.rada.gov.ua/laws/show/759-98-%D0%BF#n11
        // там пиздець. Треба буде щось думати як це можливо автоматизувати.
        // подумати над варіантами формалізації.
        // Можливо надати можливість для самостійного формування цієї частини таблиці.
        // Можливо додати тип майна з заздалегідь підготованими стовпчиками для вводу даних.

        public static decimal GetExploitationCoefficient(DateTime startDate, DateTime? endDate)
        {
            if (endDate == null)
            {
                return 1.0m;
            }

            int exploitationYears = GetExploitationYears(startDate, endDate);

            var result = exploitationYears switch
            {
                > 20 => 0.3m,
                > 15 => 0.6m,
                > 10 => 0.8m,
                > 5 => 0.0m,
                _ => 1
            };

            return result;
        }

        public static decimal GetWorkCoefficient(DateTime startDate, DateTime? endDate, int resourceYears)
        {
            var result = 1m;
            if (resourceYears > 0)
            {
                int exploitationYears = GetExploitationYears(startDate, endDate);
                var resouceLeftPercentage = Convert.ToInt32(((decimal)(resourceYears - exploitationYears) / resourceYears) * 100m);

                result = GetWorkCoefficient(resouceLeftPercentage);
            }

            return result;
        }

        public static decimal GetWorkCoefficient(int resouceLeftPercentage)
        {
            var result = resouceLeftPercentage switch
            {
                >= 75 => 1,
                >= 50 => 0.9m,
                >= 25 => 0.8m,
                _ => 0.7m
            };

            return result;
        }

        public static decimal GetTechnicalStateCoefficient(int category)
        {
            var result = category switch
            {
                1 or 2 => 1,
                3 or 4 => 0.8m,
                _ => 0.7m
            };

            return result;
        }

        private static readonly Dictionary<int, decimal> IndexationByYearTable = new()
        {
            { 1997, 1.001m },
            { 1998, 1.1m },
            { 1999, 1.092m },
            { 2000, 1.158m },
            { 2001, 1m },
            { 2002, 1m },
            { 2003, 1m },
            { 2004, 1.023m },
            { 2005, 1.003m },
            { 2006, 1.016m },
            { 2007, 1.066m },
            { 2008, 1.123m },
            { 2009, 1.023m },
            { 2010, 1m },
            { 2011, 1m },
            { 2012, 1m },
            { 2013, 1m },
            { 2014, 1.149m },
            { 2015, 1.333m },
            { 2016, 1.024m },
            { 2017, 1.037m },
            { 2018, 1m },
            { 2019, 1m },
            { 2020, 1m },
            { 2021, 1m },
            { 2022, 1.166m },
            { 2023, 1m },
            { 2024, 1.02m },
            { 2025, 1m }
        };

        public static decimal GetIndexationCoefficient(int startYear, int? endYear)
        {
            if (endYear == null || endYear < startYear)
            {
                return 1.0m;
            }

            const int MinYear = 1997;

            if (endYear < startYear)
                return 1m;

            decimal coefficient = 1m;
            for (int year = startYear; year < endYear; year++)
            {
                if (year < MinYear)
                {
                    continue;
                }
                else if (IndexationByYearTable.TryGetValue(year, out var value))
                {
                    coefficient *= value;
                }
                else
                {
                    break;
                }
            }
            return Math.Round(coefficient, 3);
        }

        private static int GetExploitationYears(DateTime startDate, DateTime? endDate)
        {
            if (endDate == null)
            {
                return 0;
            }

            var exploitationDays = (int)(endDate.Value - startDate).TotalDays;
            int exploitationYears = exploitationDays / 365;

            return exploitationYears;
        }
    }
}
