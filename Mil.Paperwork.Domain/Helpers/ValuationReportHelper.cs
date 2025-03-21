namespace Mil.Paperwork.Domain.Helpers
{
    internal class ValuationReportHelper
    {
        public const string REPORT_TEMPLATE_NAME = "AssetValuationReportTemplate.docx";
        public const string OUTPUT_REPORT_NAME_FORMAT = "Акт оцінки {0}.docx";
        
        public const int TABLE_FONT_SIZE = 11;

        public const string TABLE_ASSET_NAME = "TABLE_ASSET";

        public const string FIELD_ASSET_NAME = "«ASSET_NAME»";
        public const string FIELD_VALUATION_DATE = "«VALUATION_DATE»";
        public const string FIELD_VALUATION_SOURCES = "«VALUATION_SOURCES»";

        public const int COLUMN_INDEX = 0;
        public const int COLUMN_NAME = 1;
        public const int COLUMN_NOMENCLATURE_CODE = 2;
        public const int COLUMN_MEAS_UNIT = 3;
        public const int COLUMN_COUNT = 4;
        public const int COLUMN_PRICE_INITIAL = 5;
        public const int COLUMN_PRICE_TOTAL = 6;

        public const string TOTAL_TEXT_FORMAT = "{0} на суму: {1}";
    }
}
