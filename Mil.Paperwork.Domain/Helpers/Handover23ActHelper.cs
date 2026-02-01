namespace Mil.Paperwork.Domain.Helpers
{
    internal class Handover23ActHelper
    {
        public const string SummaryRowTotalText = "Всього:";
        public const string WithoutNumberText = "б/н";
        public const string DocumentPropsFormat = "№{0} від {1}";

        public const string REPORT_TEMPLATE_NAME = "HandoverCertificate23Template.docx";

        public const string OUTPUT_REPORT_NAME_TEMPLATE = "Акт п-п основних засобів {0}.docx";

        public const string FIELD_DOC_DATE = "DOC_DATE"; // 
        public const string FIELD_DOC_NUMBER = "DOC_NUMBER"; // 
        public const string FIELD_DATE_START = "DATE_START"; // 
        public const string FIELD_DATE_END = "DATE_END"; // 15.10.2025
        public const string FIELD_SUPPLIER = "SUPPLIER"; // ТОВАРИСТВО З ОБМЕЖЕНОЮ ВІДПОВІДАЛЬНІСТЮ "БЛА БЛА"
        public const string FIELD_RECEIVER = "RECEIVER"; // військова частина A9999
        public const string FIELD_REASON_DOCUMENT_NAME = "REASON_DOCUMENT_NAME"; // акт приймання-передачі гуманітарної (благодійної) допомоги 
        public const string FIELD_REASON_DOCUMENT_PROPS = "REASON_DOCUMENT_PROPS"; // №б/н від 15.10.2025
        public const string FIELD_MATERIALLY_RESPONSIBLE = "MATERIALLY_RESPONSIBLE"; // Олександр МОРОЗ
        public const string FIELD_MATERIALLY_RESPONSIBLE_POSITION = "MATERIALLY_RESPONSIBLE_POSITION"; // Начальник складу майна зв’язку в/ч А9999
        public const string FIELD_PERSON_RECIPIENT_NAME = "PERSON_RECIPIENT_NAME"; // Олександр МОРОЗ
        public const string FIELD_PERSON_RECIPIENT_POSITION = "PERSON_RECIPIENT_POSITION"; // Начальник складу майна зв’язку в/ч А9999

        public const string TABLE_ASSETS_NAME = "TABLE_ASSETS";
        public const int TABLE_FONT_SIZE = 9;

        public const int COLUMN_INDEX = 0;
        public const int COLUMN_ASSET_NAME = 1;
        public const int COLUMN_NOMENCLATURE_CODE = 2;
        public const int COLUMN_BATCH_NUMBER = 3;
        public const int COLUMN_PRICE = 4;
        public const int COLUMN_COUNT_OUT = 5;
        public const int COLUMN_CATEGORY_OUT = 6;
        public const int COLUMN_COUNT_IN = 7;
        public const int COLUMN_CATEGORY_IN = 8;
        public const int COLUMN_TOTAL_PRICE = 9;
        public const int COLUMN_WEAR_N_TEAR = 10;
        public const int COLUMN_WEAR_N_TEAR_TOTAL = 11;
        public const int COLUMN_YEAR_MANUFACTURED = 12;
        public const int COLUMN_SERIAL_NUMBER = 13;
        public const int COLUMN_PASSPORT_NUMBER = 14;


        public static string GetReasonDocProps(string docNumber, DateTime docDate)
        {
            var numberPart = string.IsNullOrEmpty(docNumber) ? WithoutNumberText : docNumber;
            return string.Format(DocumentPropsFormat, numberPart, docDate.ToString(ReportHelper.DATE_FORMAT));
        }

    }
}
