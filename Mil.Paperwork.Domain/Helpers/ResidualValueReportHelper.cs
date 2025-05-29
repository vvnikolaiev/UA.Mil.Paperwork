using Mil.Paperwork.Domain.Resources;
using Mil.Paperwork.Infrastructure.Enums;

namespace Mil.Paperwork.Domain.Helpers
{    
    internal class ResidualValueReportHelper
    {
        public const string REPORT_TEMPLATE_NAME = "ResidualValueReportTemplate.xlsx";
        public const string OUTPUT_REPORT_NAME_FORMAT = "Відомість залишкової вартості {0}.xlsx";

        public const string FIELD_TABLE_HEADER = "FIELD_HEADER_TITLE";
        public const string FIELD_TABLE_COLUMN_NUMBER = "FIELD_TABLE_COLUMN_NUMBER";
        public const string FIELD_NUMBER_NAMES = "FIELD_NUMBER_NAMES";
        public const string FIELD_SUM = "FIELD_SUM";

        public const string RANGE_TO_SHRINK_1_NAME = "FIELDS_TITLE";
        public const string RANGE_TO_SHRINK_2_NAME = "FIELD_RANGE_8";

        public const string RANGE_TO_SHIFT_1_NAME = "COMPLEX_MERGED_RANGE_1";
        public const string RANGE_TO_SHIFT_2_NAME = "COMPLEX_MERGED_RANGE_2";

        public const string REPORT_DATE_PLACEHOLDER = "REPORT_DATE";
        public const string TOTAL_RESIDUAL_SUM_PLACEHOLDER = "TOTAL_RESIDUAL_SUM";

        public const string TABLE_COLUMN_SUM_FORMULA = "=SUM(N{0}:N{1})";

        public const int TABLE_COLUMN_INDEX = 1;
        public const int TABLE_COLUMN_NAME = 2;
        public const int TABLE_COLUMN_MEASUREMENT_UNIT = 3;
        public const int TABLE_COLUMN_COUNT = 4;
        public const int TABLE_COLUMN_PRICE = 5;
        public const int TABLE_COLUMN_DATE_START = 6;
        public const int TABLE_COLUMN_INDEXATION_COEFF = 7;
        public const int TABLE_COLUMN_CURRENCY_CONVERSION_RATE = 9;
        public const int TABLE_FIRST_COEFF_COLUMN = 10;

        public const string RESIDUAL_VALUE_SUM_FORMAT = "# ##0.00_₴";

        public static IList<string> GetCoefficientColumns(AssetType assetType)
        {
            var columns = new List<string>();
            switch (assetType)
            {
                case AssetType.Connectivity:
                    columns.Add(ResidualValueReportStrings.CoeffExploitation);
                    columns.Add(ResidualValueReportStrings.CoeffWork);
                    columns.Add(ResidualValueReportStrings.CoeffZB);
                    columns.Add(ResidualValueReportStrings.CoeffTechState);
                    break;
                case AssetType.Radiochemical:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(assetType), "Unsupported asset type for coefficient columns.");
            }
            return columns;
        }
    }
}
