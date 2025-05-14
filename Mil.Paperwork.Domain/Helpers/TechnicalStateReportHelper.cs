namespace Mil.Paperwork.Domain.Helpers
{
    internal class TechnicalStateReportHelper
    {
        public const string REPORT7_TEMPLATE_NAME = "TechnicalState7ReportTemplate.docx";
        public const string REPORT11_TEMPLATE_NAME = "TechnicalState11ReportTemplate.docx";
        public const string OUTPUT_REPORT_7_NAME_FORMAT = "Акт технічного стану (№7) {0}.docx";
        public const string OUTPUT_REPORT_11_NAME_FORMAT = "Акт технічного стану (№11) {0}.docx";

        public const string TABLE_ASSET_NAME = "TABLE_ASSET";
        public const string TABLE_OPERATIONAL_INDICATORS_NAME = "TABLE_OPERATIONAL_INDICATORS";

        public const string FIELD_ASSET_NAME = "«ASSET_NAME»";
        public const string FIELD_REGISTRATION_NUMBER = "«REGISTRATION_NUMBER»";
        public const string FIELD_DOCUMENT_NUMBER = "«DOCUMENT_NUMBER»";
        public const string FIELD_ASSET_INITIAL_CATEGORY = "«ASSET_INITIAL_CATEGORY»";
        public const string FIELD_ASSET_RESIDUAL_CATEGORY = "«ASSET_RESIDUAL_CATEGORY»";
        public const string FIELD_ASSET_INITIAL_CATEGORY_TEXT = "«ASSET_INITIAL_CATEGORY_TEXT»";
        public const string FIELD_ASSET_RESIDUAL_CATEGORY_TEXT = "«ASSET_RESIDUAL_CATEGORY_TEXT»";
        public const string FIELD_REASON = "«REASON»";

        public const int ROW_COMMISIONING_YEAR = 1;
        public const int ROW_MONTHS_OPERATED = 2;
        public const int ROW_HOURS_OPERATED = 3;
        public const int ROW_TECHNICAL_RESOURCE = 4;
        public const int ROW_TECHNICAL_OPERATIONAL_TERM = 5;
        public const int ROW_WARRANTY_RESOURCE = 6;
        public const int ROW_WARRANTY_PERIOD_YEARS = 7;
        public const int ROW_REPAIR_DESCRIPTION_AND_DATE = 8;
        public const int ROW_IN_OPERATING_SINCE_REPAIR_MONTHS = 9;
        public const int ROW_OPERATING_RESOURCE_SINCE_REPAIR = 10;
        public const int ROW_INCOMPLETENESS_RESOURCE = 11;
        public const int ROW_INCOMPLETENESS_OPERATIONAL_TERM = 12;
        public const int ROW_INCOMPLETENESS_WARRANTY_RESOURCE = 13;
        public const int ROW_INCOMPLETENESS_WARRANTY_TERM = 14;

        public const int COLUMN_INDEX = 0;
        public const int COLUMN_NAME = 1;
        public const int COLUMN_NOMENCLATURE_CODE = 2;
        public const int COLUMN_MEAS_UNIT = 3;
        public const int COLUMN_COUNT = 4;
        public const int COLUMN_CATEGORY_INITIAL = 5;
        public const int COLUMN_CATEGORY_RESIDUAL = 6;
        public const int COLUMN_PRICE_INITIAL = 7;
        public const int COLUMN_PRICE_RESIDUAL = 8;
        public const int COLUMN_FACTORY_NUMBER = 9;
        public const int COLUMN_MANUFACTURER = 10;
        public const int COLUMN_PASSPORT_NUMBER = 11;
    }
}
