namespace Mil.Paperwork.Domain.Helpers
{
    public class InvoiceReportHelper
    {
        public const string REPORT_TEMPLATE_NAME = "InvoiceTemplate.docx";
        public const string OUTPUT_REPORT_NAME_TEMPLATE = "Накладна {0}.docx";

        public const string FIELD_DUE_DAY = "DUE_DAY";
        public const string FIELD_DUE_MONTH = "DUE_MONTH";
        public const string FIELD_DUE_YEAR = "DUE_YEAR";
        public const string FIELD_DATE_DAY = "DAY";
        public const string FIELD_DATE_MONTH = "MONTH";
        public const string FIELD_DATE_YEAR = "YEAR";

        public const string FIELD_DATE = "DATE";

        public const string FIELD_DOCUMENT_NUMBER = "DOCUMENT_NUMBER";
        public const string FIELD_REASON = "REASON";
        public const string FIELD_RECIPIENT_SURNAME = "RECIPIENT_SURNAME";
        public const string FIELD_PERSON_TRANSMITTER = "TRNSMTR";
        public const string FIELD_PERSON_TRANSMITTER_RANK = "TRNSMTR_RANK";
        public const string FIELD_PERSON_TRANSMITTER_POSITION = "TRNSMTR_POSITION";
        public const string FIELD_PERSON_RECIPIENT = "RECIPIENT";
        public const string FIELD_PERSON_RECIPIENT_RANK = "RECIPIENT_RANK";
        public const string FIELD_PERSON_RECIPIENT_POSITION = "RECIPIENT_POSITION";

        public const string FIELD_ASSETS_COUNT_TEXT = "ASSETS_COUNT_TEXT";
        public const string FIELD_UNITS_TEXT = "UNITS_TEXT";
        public const string FIELD_TOTAL_SUM_TEXT = "TOTAL_SUM_TEXT";
        public const string FIELD_CENTS = "CENTS";

        public const string TABLE_ASSETS_NAME = "TABLE_ASSETS";
        public const int TABLE_FONT_SIZE = 10;

        public const int COLUMN_INDEX = 0;
        public const int COLUMN_ASSET_NAME = 1;
        public const int COLUMN_NOMENCLATURE_CODE = 2;
        public const int COLUMN_MEASUREMENT_UNIT = 3;
        public const int COLUMN_CATEGORY = 4;
        public const int COLUMN_PRICE = 5;
        public const int COLUMN_COUNT_OUT = 6;
        public const int COLUMN_COUNT_IN = 7;
        public const int COLUMN_TOTAL_PRICE = 8;
        public const int COLUMN_NOTE = 9;
    }
}
