namespace Mil.Paperwork.Domain.Helpers
{
    internal class TechnicalStateReportHelper
    {
        public const string WRITEOFF_ACT_TEMPLATE_NAME = "WriteOffActTemplate.docx";
        public const string REPORT7_TEMPLATE_NAME = "TechnicalState7ReportTemplate.docx";
        public const string REPORT11_TEMPLATE_NAME = "TechnicalState11ReportTemplate.docx";
        public const string OUTPUT_REPORT_7_NAME_FORMAT = "Акт технічного стану (№7) {0}.docx";
        public const string OUTPUT_REPORT_11_NAME_FORMAT = "Акт технічного стану (№11) {0}.docx";
        public const string OUTPUT_WRITE_OFF_ACT_NAME_FORMAT = "Акт списання {0}.docx";

        public const string TABLE_ASSET_NAME = "TABLE_ASSET";
        public const string TABLE_OPERATIONAL_INDICATORS_NAME = "TABLE_OPERATIONAL_INDICATORS";

        public const int TABLE_FONT_SIZE = 10;

        public const string FIELD_ASSET_NAME = "«ASSET_NAME»";
        public const string FIELD_REGISTRATION_NUMBER = "«REG_NUM»";
        public const string FIELD_DOCUMENT_NUMBER = "«DOC_NUM»";
        public const string FIELD_DOCUMENT_DATE = "«DOC_DATE»";
        public const string FIELD_ASSET_INITIAL_CATEGORY = "«ASSET_INITIAL_CATEGORY»";
        public const string FIELD_ASSET_RESIDUAL_CATEGORY = "«ASSET_RESIDUAL_CATEGORY»";
        public const string FIELD_ASSET_INITIAL_CATEGORY_TEXT = "«ASSET_INITIAL_CATEGORY_TEXT»";
        public const string FIELD_ASSET_RESIDUAL_CATEGORY_TEXT = "«ASSET_RESIDUAL_CATEGORY_TEXT»";
        public const string FIELD_REASON = "«REASON»";
        public const string FIELD_EVENT_DATE = "«EVENT_DATE»";
        public const string FIELD_ORDEN_NUMBER = "«ORDEN_NUM»";
        public const string FIELD_ORDEN_DATE = "«ORDEN_DATE»";
        //TODO: new fields IV - Technical State
        public const string FIELD_CATEGORY = "«CATEGORY»";
        public const string FIELD_STORAGE_CONDTIONS = "«STORAGE_CONDTIONS»";

        public const int ROW_COMMISIONING_YEAR = 0;
        public const int ROW_MONTHS_OPERATED = 1;
        public const int ROW_HOURS_OPERATED = 2;
        public const int ROW_TECHNICAL_RESOURCE = 3;
        public const int ROW_TECHNICAL_OPERATIONAL_TERM = 4;
        public const int ROW_WARRANTY_RESOURCE = 5;
        public const int ROW_WARRANTY_PERIOD_YEARS = 6;
        public const int ROW_REPAIR_DESCRIPTION_AND_DATE = 7;
        public const int ROW_IN_OPERATING_SINCE_REPAIR_MONTHS = 8;
        public const int ROW_OPERATING_RESOURCE_SINCE_REPAIR = 9;
        public const int ROW_INCOMPLETENESS_RESOURCE = 10;
        public const int ROW_INCOMPLETENESS_OPERATIONAL_TERM = 11;
        public const int ROW_INCOMPLETENESS_WARRANTY_RESOURCE = 12;
        public const int ROW_INCOMPLETENESS_WARRANTY_TERM = 13;

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

        public const int WOA_COLUMN_INDEX = 0;
        public const int WOA_COLUMN_NAME = 1;
        public const int WOA_COLUMN_NOMENCLATURE_CODE = 2;
        public const int WOA_COLUMN_MEAS_UNIT = 3;
        public const int WOA_COLUMN_CATEGORY = 4;
        public const int WOA_COLUMN_PRICE_INITIAL = 5;
        public const int WOA_COLUMN_COUNT = 6;
        public const int WOA_COLUMN_PRICE_RESIDUAL = 7;
    }
}
