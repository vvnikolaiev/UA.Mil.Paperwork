﻿namespace Mil.Paperwork.Domain.Helpers
{
    public static class ReportHelper
    {
        enum GenderOfNoun
        {
            Masculine,
            Feminine,
            Neuter,
        }

        enum NounCases
        {
            Empty,
            Nominative,
            Genitive,
            Accusative
        }

        class Words
        {
            public string Year { get; set; }
            public string Month { get; set; }
            public string Hour { get; set; }
            public string Names { get; set; }
            public string Hryvna { get; set; }
            public string Kopiyka { get; set; }
        }

        private static readonly Words NominativeWords = new() { Year = "рік", Month = "місяць", Hour = "година", Names = "найменування", Hryvna = "гривня", Kopiyka = "копійка" };
        private static readonly Words GenitiveWords = new() { Year = "років", Month = "місяців", Hour = "годин", Names = "найменувань", Hryvna = "гривень", Kopiyka = "копійок" };
        private static readonly Words AccusativeWords = new() { Year = "роки", Month = "місяці", Hour = "години", Names = "найменування", Hryvna = "гривні", Kopiyka = "копійки" };

        private static readonly Dictionary<NounCases, Words> NounCasesWords = new()
        {
            { NounCases.Empty, new Words() },
            { NounCases.Nominative, NominativeWords },
            { NounCases.Genitive, GenitiveWords },
            { NounCases.Accusative, AccusativeWords },
        };

        private static Dictionary<GenderOfNoun, string[]> Units = new()
        {
            { GenderOfNoun.Neuter, new[] { "нуль", "одне", "два", "три", "чотири", "п'ять", "шість", "сім", "вісім", "дев'ять" }},
            { GenderOfNoun.Masculine, new[] { "нуль", "один", "два", "три", "чотири", "п'ять", "шість", "сім", "вісім", "дев'ять" }},
            { GenderOfNoun.Feminine, new[] { "нуль", "одна", "дві", "три", "чотири", "п'ять", "шість", "сім", "вісім", "дев'ять" }},
        };

        private static readonly string[] Teens = { "десять", "одинадцять", "дванадцять", "тринадцять", "чотирнадцять", "п'ятнадцять", "шістнадцять", "сімнадцять", "вісімнадцять", "дев'ятнадцять" };
        private static readonly string[] Tens = { "", "десять", "двадцять", "тридцять", "сорок", "п'ятдесят", "шістдесят", "сімдесят", "вісімдесят", "дев'яносто" };
        private static readonly string[] Hundreds = { "", "сто", "двісті", "триста", "чотириста", "п'ятсот", "шістсот", "сімсот", "вісімсот", "дев'ятсот" };
        // move to NounCasesWords
        private static readonly string[] Thousands = { "", "тисяча", "тисячі", "тисяч" };
        private static readonly string[] Millions = { "", "мільйон", "мільйони", "мільйонів" };
        private static readonly string[] Billions = { "", "мільярд", "мільярди", "мільярдів" };

        private const string NamesNumberStringFormat = "Всього: {0} ({1}) {2}";

        public const string DATE_FORMAT = "dd.MM.yyyy";

        public static string ConvertCategoryToText(int category)
        {
            var result = category switch
            {
                1 => "I",
                2 => "II",
                3 => "III",
                4 => "IV",
                5 => "V",
                _ => "-"
            };
            return result;
        }

        public static string GetFullAssetName(string assetName, string serialNumber)
        {
            var serialNumberText = string.Empty;

            if (!String.IsNullOrEmpty(serialNumber))
            {
                serialNumberText = String.Format(", серійний номер {0}", serialNumber);
            }

            var result = String.Format("{0}{1}", assetName, serialNumberText);

            return result;
        }

        public static string ConvertNamesNumberToReportString(int number)
        {
            if (number < 0)
                throw new ArgumentOutOfRangeException(nameof(number), "Number must be non-negative.");

            var nounForm = GetNounFormFromNumber(number);
            var numberString = number.ToString();
            var wordNumber = ConvertNumberToWords(number, GenderOfNoun.Neuter);
            var namesText = NounCasesWords[nounForm].Names;
            var result = String.Format(NamesNumberStringFormat, numberString, wordNumber, namesText);

            return result;
        }

        public static string ConvertTotalSumToUkrainianString(decimal totalSum)
        {
            var integerPart = (int)totalSum;
            var fractionalPart = (int)((totalSum - integerPart) * 100);

            var caseHryvnas = GetNounFormFromNumber(integerPart);
            var caseKopiykas = GetNounFormFromNumber(fractionalPart);

            var sumInWords = ConvertNumberToWords(integerPart, GenderOfNoun.Feminine);
            var kopInWords = ConvertNumberToWords(fractionalPart, GenderOfNoun.Feminine);
            var hryvnasText = NounCasesWords[caseHryvnas].Hryvna;
            var kopiykasText = NounCasesWords[caseKopiykas].Kopiyka;

            var result = string.Format("{0}грн. {1}коп. ({2} {3} {4} {5})", integerPart, fractionalPart, sumInWords, hryvnasText, kopInWords, kopiykasText);
            
            return result;
        }

        public static string GetMonthsOperatedText(DateTime startDate, DateTime endDate)
        {
            var monthsOperated = (int)((endDate - startDate).TotalDays / 30);
            var caseMonths = GetNounFormFromNumber(monthsOperated);
            var monthsText = NounCasesWords[caseMonths].Month;

            var result = $"{monthsOperated} {monthsText}";

            return result;
        }

        public static string GetHoursOperatedText(DateTime startDate, DateTime endDate)
        {
            var monthsOperated = (int)((endDate - startDate).TotalDays / 30);
            var hoursOperated = monthsOperated * 24 * 30;
            var caseHours = GetNounFormFromNumber(hoursOperated);
            var hoursText = NounCasesWords[caseHours].Hour;
            var result = $"{hoursOperated} {hoursText}";
            return result;
        }

        public static string GetWarrantyPeriodText(int warrantyPeriodYears)
        {
            var caseHours = GetNounFormFromNumber(warrantyPeriodYears);
            var yearsText = NounCasesWords[caseHours].Year;
            var result = $"{warrantyPeriodYears} {yearsText}";
            return result;
        }

        private static NounCases GetNounFormFromNumber(int number)
        {
            var nounCase = NounCases.Genitive; // most cases (гривень, копійок, найменувань)

            if (number % 10 == 1 && number % 100 != 11)
            {
                nounCase = NounCases.Nominative; // 1 гривня, копійка, найменування
            }
            else if (number % 100 >= 12 && number % 100 <= 14 && number > 20)
            {
                nounCase = NounCases.Genitive; // 3 гривні, копійки, найменування
            }
            else if (number % 10 >= 2 && number % 10 <= 4 && (number < 10 || number > 20))
            {
                nounCase = NounCases.Accusative; // 3 гривні, копійки, найменування
            }

            return nounCase;
        }

        private static string ConvertNumberToWords(int number, GenderOfNoun gender)
        {
            if (number == 0)
                return Units[gender][0];

            var units = Units[gender];

            if (number < 10)
                return units[number];
            else if (number < 20)
                return Teens[number - 10];
            else if (number < 100)
                return Tens[number / 10] + (number % 10 > 0 ? " " + units[number % 10] : "");
            else if (number < 1000)
                return Hundreds[number / 100] + (number % 100 > 0 ? " " + ConvertNumberToWords(number % 100, gender) : "");
            else if (number < 1000000)
                return ConvertLargeNumberToWords(number, 1000, Thousands, GenderOfNoun.Feminine);
            else if (number < 1000000000)
                return ConvertLargeNumberToWords(number, 1000000, Millions, GenderOfNoun.Masculine);
            else
                return ConvertLargeNumberToWords(number, 1000000000, Billions, GenderOfNoun.Masculine);
        }

        // TODO: refactor later?
        private static string ConvertLargeNumberToWords(int number, int divisor, string[] forms, GenderOfNoun gender)
        {
            int quotient = number / divisor;
            int remainder = number % divisor;

            var wordCase = GetNounFormFromNumber(quotient);

            var form = wordCase switch
            {
                NounCases.Nominative => forms[1],
                NounCases.Genitive => forms[3],
                NounCases.Accusative => forms[2],
                _ => forms[0],
            };

            return ConvertNumberToWords(quotient, gender) + " " + form + (remainder > 0 ? " " + ConvertNumberToWords(remainder, gender) : "");
        }
    }
}
