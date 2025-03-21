namespace Mil.Paperwork.Domain.Helpers
{
    internal class DismantlingReportHelper
    {
        public const string REPORT_TEMPLATE_NAME = "DismantlingReportTemplate.docx";
        public const string OUTPUT_REPORT_NAME_FORMAT = "Акт якісного стану - РОЗУКОМПЛЕКТАЦІЯ - {0}.docx";

        public const string TABLE_ASSET_CONFIGURATION_NAME = "TABLE_ASSET_CONFIGURATION";

        public const string FIELD_ASSET_NAME = "«ASSET_NAME»";
        public const string FIELD_REGISTRATION_NUMBER = "«REGISTRATION_NUMBER»";
        public const string FIELD_DOCUMENT_NUMBER = "«DOCUMENT_NUMBER»";
        public const string FIELD_DISMANTLING_REASON = "«DISMANTLING_REASON»";
        public const string FIELD_REMAINS_RANGE = "«REMAINS_RANGE»";

        public const int COLUMN_INDEX = 0;
        public const int COLUMN_ASSET_NAME = 1;
        public const int COLUMN_ASSET_NOMENCLATURE_CODE = 2;
        public const int COLUMN_ASSET_MEAS_UNIT = 3;
        public const int COLUMN_ASSET_CATEGORY = 4;
        public const int COLUMN_ASSET_COUNT = 5;
        public const int COLUMN_ASSET_PRICE = 6;
        public const int COLUMN_ASSET_PRICE_TOTAL = 7;
        public const int COLUMN_ASSET_OPERATING_YEARS_NORM = 8;
        public const int COLUMN_ASSET_OPERATING_YEARS = 9;

        public const int COLUMN_COMPONENT_NAME = 10;
        public const int COLUMN_COMPONENT_NOMENCLATURE_CODE = 11;
        public const int COLUMN_COMPONENT_MEAS_UNIT = 12;
        public const int COLUMN_COMPONENT_CATEGORY = 13;
        public const int COLUMN_COMPONENT_COUNT = 14;
        public const int COLUMN_COMPONENT_RESIDUAL_PRICE = 15;
        public const int COLUMN_COMPONENT_PRICE_TOTAL = 16;

        public const int COLUMN_ASSET_FIRST = COLUMN_ASSET_NAME;
        public const int COLUMN_ASSET_LAST = COLUMN_ASSET_OPERATING_YEARS;

        public const int COLUMN_COMPONENT_FIRST = COLUMN_COMPONENT_NAME;
        public const int COLUMN_COMPONENT_LAST = COLUMN_COMPONENT_PRICE_TOTAL;

        public const int TABLE_FONT_SIZE = 10;
        public const string TOTAL_TEXT_FORMAT = "{0} на суму: {1}";
        public const string REMAINS_RANGE_TEXT_FORMAT = "1-{0}";
    }
}
