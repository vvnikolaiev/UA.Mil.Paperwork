﻿namespace Mil.Paperwork.Domain.Helpers
{
    public static class CoefficientsHelper
    {
        // https://zakon.rada.gov.ua/laws/show/759-98-%D0%BF#n330
        // там пиздець. Треба буде щось думати як це можливо автоматизувати.
        // подумати над варіантами формалізації.
        // Можливо надати можливість для самостійного формування цієї частини таблиці.
        // Можливо додати тип майна з заздалегідь підготованими стовпчиками для вводу даних.


        public static decimal GetExploitationCoefficient(DateTime startDate, DateTime endDate)
        {
            var exploitationDays = (int)(endDate - startDate).TotalDays;
            int exploitationYears = exploitationDays / 365;

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

        private static readonly Dictionary<int, Dictionary<int, decimal>> IndexationTable = new()
        {
            { 1997, new() { { 2025, 5.768m }, { 2024, 5.654m }, { 2023, 5.654m }, { 2022, 4.849m }, { 2021, 4.849m }, { 2020, 4.849m }, { 2019, 4.849m }, { 2018, 4.849m }, { 2017, 4.676m }, { 2016, 4.567m }, { 2015, 3.426m }, { 2014, 2.982m } } },
            { 1998, new() { { 2025, 3.438m }, { 2024, 3.371m }, { 2023, 3.371m }, { 2022, 2.891m }, { 2021, 2.891m }, { 2020, 2.891m }, { 2019, 2.891m }, { 2018, 2.891m }, { 2017, 2.788m }, { 2016, 2.723m }, { 2015, 2.042m }, { 2014, 1.778m } } },
            { 1999, new() { { 2025, 3.435m }, { 2024, 3.368m }, { 2023, 3.368m }, { 2022, 2.888m }, { 2021, 2.888m }, { 2020, 2.888m }, { 2019, 2.888m }, { 2018, 2.888m }, { 2017, 2.785m }, { 2016, 2.720m }, { 2015, 2.040m }, { 2014, 1.776m } } },
            { 2000, new() { { 2025, 3.123m }, { 2024, 3.062m }, { 2023, 3.062m }, { 2022, 2.626m }, { 2021, 2.626m }, { 2020, 2.626m }, { 2019, 2.626m }, { 2018, 2.626m }, { 2017, 2.532m }, { 2016, 2.473m }, { 2015, 1.855m }, { 2014, 1.614m } } },
            { 2001, new() { { 2025, 2.860m }, { 2024, 2.804m }, { 2023, 2.804m }, { 2022, 2.404m }, { 2021, 2.404m }, { 2020, 2.404m }, { 2019, 2.404m }, { 2018, 2.404m }, { 2017, 2.319m }, { 2016, 2.264m }, { 2015, 1.699m }, { 2014, 1.478m } } },
            { 2002, new() { { 2025, 2.470m }, { 2024, 2.421m }, { 2023, 2.421m }, { 2022, 2.076m }, { 2021, 2.076m }, { 2020, 2.076m }, { 2019, 2.076m }, { 2018, 2.076m }, { 2017, 2.002m }, { 2016, 1.955m }, { 2015, 1.467m }, { 2014, 1.277m } } },
            { 2003, new() { { 2025, 2.470m }, { 2024, 2.421m }, { 2023, 2.421m }, { 2022, 2.076m }, { 2021, 2.076m }, { 2020, 2.076m }, { 2019, 2.076m }, { 2018, 2.076m }, { 2017, 2.002m }, { 2016, 1.955m }, { 2015, 1.467m }, { 2014, 1.277m } } },
            { 2004, new() { { 2025, 2.470m }, { 2024, 2.421m }, { 2023, 2.421m }, { 2022, 2.076m }, { 2021, 2.076m }, { 2020, 2.076m }, { 2019, 2.076m }, { 2018, 2.076m }, { 2017, 2.002m }, { 2016, 1.955m }, { 2015, 1.467m }, { 2014, 1.277m } } },
            { 2005, new() { { 2025, 2.414m }, { 2024, 2.367m }, { 2023, 2.367m }, { 2022, 2.030m }, { 2021, 2.030m }, { 2020, 2.030m }, { 2019, 2.030m }, { 2018, 2.030m }, { 2017, 1.957m }, { 2016, 1.911m }, { 2015, 1.434m }, { 2014, 1.248m } } },
            { 2006, new() { { 2025, 2.407m }, { 2024, 2.360m }, { 2023, 2.360m }, { 2022, 2.024m }, { 2021, 2.024m }, { 2020, 2.024m }, { 2019, 2.024m }, { 2018, 2.024m }, { 2017, 1.951m }, { 2016, 1.906m }, { 2015, 1.430m }, { 2014, 1.244m } } },
            { 2007, new() { { 2025, 2.369m }, { 2024, 2.322m }, { 2023, 2.322m }, { 2022, 1.992m }, { 2021, 1.992m }, { 2020, 1.992m }, { 2019, 1.992m }, { 2018, 1.992m }, { 2017, 1.921m }, { 2016, 1.876m }, { 2015, 1.407m }, { 2014, 1.225m } } },
            { 2008, new() { { 2025, 2.222m }, { 2024, 2.179m }, { 2023, 2.179m }, { 2022, 1.868m }, { 2021, 1.868m }, { 2020, 1.868m }, { 2019, 1.868m }, { 2018, 1.868m }, { 2017, 1.802m }, { 2016, 1.760m }, { 2015, 1.320m }, { 2014, 1.149m } } },
            { 2009, new() { { 2025, 1.979m }, { 2024, 1.940m }, { 2023, 1.940m }, { 2022, 1.664m }, { 2021, 1.664m }, { 2020, 1.664m }, { 2019, 1.664m }, { 2018, 1.664m }, { 2017, 1.604m }, { 2016, 1.567m }, { 2015, 1.175m }, { 2014, 1.023m } } },
            { 2010, new() { { 2025, 1.934m }, { 2024, 1.896m }, { 2023, 1.896m }, { 2022, 1.626m }, { 2021, 1.626m }, { 2020, 1.626m }, { 2019, 1.626m }, { 2018, 1.626m }, { 2017, 1.568m }, { 2016, 1.532m }, { 2015, 1.149m }, { 2014, 1.000m } } },
            { 2011, new() { { 2025, 1.934m }, { 2024, 1.896m }, { 2023, 1.896m }, { 2022, 1.626m }, { 2021, 1.626m }, { 2020, 1.626m }, { 2019, 1.626m }, { 2018, 1.626m }, { 2017, 1.568m }, { 2016, 1.532m }, { 2015, 1.149m }, { 2014, 1.000m } } },
            { 2012, new() { { 2025, 1.934m }, { 2024, 1.896m }, { 2023, 1.896m }, { 2022, 1.626m }, { 2021, 1.626m }, { 2020, 1.626m }, { 2019, 1.626m }, { 2018, 1.626m }, { 2017, 1.568m }, { 2016, 1.532m }, { 2015, 1.149m }, { 2014, 1.000m } } },
            { 2013, new() { { 2025, 1.934m }, { 2024, 1.896m }, { 2023, 1.896m }, { 2022, 1.626m }, { 2021, 1.626m }, { 2020, 1.626m }, { 2019, 1.626m }, { 2018, 1.626m }, { 2017, 1.568m }, { 2016, 1.532m }, { 2015, 1.149m }, { 2014, 1.000m } } },
            { 2014, new() { { 2025, 1.934m }, { 2024, 1.896m }, { 2023, 1.896m }, { 2022, 1.626m }, { 2021, 1.626m }, { 2020, 1.626m }, { 2019, 1.626m }, { 2018, 1.626m }, { 2017, 1.568m }, { 2016, 1.532m }, { 2015, 1.149m }, { 2014, 1.000m } } },
            { 2015, new() { { 2025, 1.683m }, { 2024, 1.650m }, { 2023, 1.650m }, { 2022, 1.415m }, { 2021, 1.415m }, { 2020, 1.415m }, { 2019, 1.415m }, { 2018, 1.415m }, { 2017, 1.365m }, { 2016, 1.333m }, { 2015, 1.000m }, { 2014, 1.000m } } },
            { 2016, new() { { 2025, 1.263m }, { 2024, 1.238m }, { 2023, 1.238m }, { 2022, 1.062m }, { 2021, 1.062m }, { 2020, 1.062m }, { 2019, 1.062m }, { 2018, 1.062m }, { 2017, 1.024m }, { 2016, 1.000m }, { 2015, 1.000m }, { 2014, 1.000m } } },
            { 2017, new() { { 2025, 1.233m }, { 2024, 1.209m }, { 2023, 1.209m }, { 2022, 1.037m }, { 2021, 1.037m }, { 2020, 1.037m }, { 2019, 1.037m }, { 2018, 1.037m }, { 2017, 1.000m }, { 2016, 1.000m }, { 2015, 1.000m }, { 2014, 1.000m } } },
            { 2018, new() { { 2025, 1.189m }, { 2024, 1.166m }, { 2023, 1.166m }, { 2022, 1.000m }, { 2021, 1.000m }, { 2020, 1.000m }, { 2019, 1.000m }, { 2018, 1.000m }, { 2017, 1.000m }, { 2016, 1.000m }, { 2015, 1.000m }, { 2014, 1.000m } } },
            { 2019, new() { { 2025, 1.189m }, { 2024, 1.166m }, { 2023, 1.166m }, { 2022, 1.000m }, { 2021, 1.000m }, { 2020, 1.000m }, { 2019, 1.000m }, { 2018, 1.000m }, { 2017, 1.000m }, { 2016, 1.000m }, { 2015, 1.000m }, { 2014, 1.000m } } },
            { 2020, new() { { 2025, 1.189m }, { 2024, 1.166m }, { 2023, 1.166m }, { 2022, 1.000m }, { 2021, 1.000m }, { 2020, 1.000m }, { 2019, 1.000m }, { 2018, 1.000m }, { 2017, 1.000m }, { 2016, 1.000m }, { 2015, 1.000m }, { 2014, 1.000m } } },
            { 2021, new() { { 2025, 1.189m }, { 2024, 1.166m }, { 2023, 1.166m }, { 2022, 1.000m }, { 2021, 1.000m }, { 2020, 1.000m }, { 2019, 1.000m }, { 2018, 1.000m }, { 2017, 1.000m }, { 2016, 1.000m }, { 2015, 1.000m }, { 2014, 1.000m } } },
            { 2022, new() { { 2025, 1.189m }, { 2024, 1.166m }, { 2023, 1.166m }, { 2022, 1.000m }, { 2021, 1.000m }, { 2020, 1.000m }, { 2019, 1.000m }, { 2018, 1.000m }, { 2017, 1.000m }, { 2016, 1.000m }, { 2015, 1.000m }, { 2014, 1.000m } } },
            { 2023, new() { { 2025, 1.020m }, { 2024, 1.000m }, { 2023, 1.000m }, { 2022, 1.000m }, { 2021, 1.000m }, { 2020, 1.000m }, { 2019, 1.000m }, { 2018, 1.000m }, { 2017, 1.000m }, { 2016, 1.000m }, { 2015, 1.000m }, { 2014, 1.000m } } },
            { 2024, new() { { 2025, 1.020m }, { 2024, 1.000m }, { 2023, 1.000m }, { 2022, 1.000m }, { 2021, 1.000m }, { 2020, 1.000m }, { 2019, 1.000m }, { 2018, 1.000m }, { 2017, 1.000m }, { 2016, 1.000m }, { 2015, 1.000m }, { 2014, 1.000m } } },
            { 2025, new() { { 2025, 1.000m }, { 2024, 1.000m }, { 2023, 1.000m }, { 2022, 1.000m }, { 2021, 1.000m }, { 2020, 1.000m }, { 2019, 1.000m }, { 2018, 1.000m }, { 2017, 1.000m }, { 2016, 1.000m }, { 2015, 1.000m }, { 2014, 1.000m } } }
        };

        public static decimal GetIndexationCoefficient(DateTime startDate, DateTime endDate)
        {
            const int MinYear = 1997;
            var startYear = startDate.Year;
            var endYear = endDate.Year;

            var result = 1.0m;

            if (startYear < MinYear)
            {
                startYear = MinYear;
            }

            if (IndexationTable.ContainsKey(startYear) && IndexationTable[startYear].ContainsKey(endYear))
            {
                result = IndexationTable[startYear][endYear];
            }

            return result;
        }
    }
}
