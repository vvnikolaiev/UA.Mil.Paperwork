namespace Mil.Paperwork.Domain.Helpers
{
    internal class ResidualValueReportHelper
    {
        public const string REPORT_TEMPLATE_NAME = "ResidualValueReportTemplate.xlsx";
        public const string OUTPUT_REPORT_NAME = "Відомість залишкової вартості.xlsx";

        public const string FIELD_NUMBER_NAMES = "FIELD_NUMBER_NAMES";
        public const string FIELD_SUM = "FIELD_SUM";

        public const string MAPPED_FIELD_DATE = "REPORT_DATE";

        public const string TABLE_COLUMN_SUM_FORMULA = "=SUM(N{0}:N{1})";

        public const int TABLE_COLUMN_INDEX = 1;
        public const int TABLE_COLUMN_NAME = 2;
        public const int TABLE_COLUMN_MEASUREMENT_UNIT = 3;
        public const int TABLE_COLUMN_COUNT = 4;
        public const int TABLE_COLUMN_PRICE = 5;
        public const int TABLE_COLUMN_INDEXATION_COEFF = 6;
        public const int TABLE_COLUMN_CURRENCY_CONVERSION_RATE = 8;
        public const int TABLE_COLUMN_COEFF_E = 9;
        public const int TABLE_COLUMN_COEFF_R = 10;
        public const int TABLE_COLUMN_WEAR_AND_TEAR_COEFF = 11;
        public const int TABLE_COLUMN_COEFF_TS = 12;
        public const int TABLE_COLUMN_VALUATION_REPORT_REFERENCE = 15;
    }
}
