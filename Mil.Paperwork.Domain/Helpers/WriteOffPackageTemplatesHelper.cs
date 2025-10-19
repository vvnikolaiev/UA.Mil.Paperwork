namespace Mil.Paperwork.Domain.Helpers
{
    internal static class WriteOffPackageTemplatesHelper
    {
        public const string OUTPUT_TITLE_PAGE_NAME = "Титульний лист матеріалів.docx";
        public const string OUTPUT_CONSENT_SHEET_NAME = "Аркуш погодження вч.docx";
        public const string OUTPUT_TABLE_OF_CONTENTS_NAME = "Опис документів.docx";
        public const string OUTPUT_BOOK_OF_LOSSES_NAME = "Витяг з книги обліку втрат.docx";

        public const string EXTRACT_FROM_BOOK_OF_LOSSES_TEMPLATE_NAME = "ExtractFromBookOfLossesTemplate.docx";
        public const string CONSENT_SHEET_TEMPLATE_NAME = "MilUnitConsentFormTemplate.docx";
        public const string WRITE_OFF_TITLE_PAGE_TEMPLATE_NAME = "WriteOffTitlePageTemplate.docx";
        public const string WRITE_OFF_TABLE_OF_CONTENTS_TEMPLATE_NAME = "WriteOffTableOfContentsTemplate.docx";

        public const string TABLE_OF_CONTENTS_NAME = "TABLE_OF_CONTENTS";
        public const int TABLE_OF_CONTENTS_FONT_SIZE = 12;
        public const string TABLE_BOOK_RECORD_NAME = "TABLE_BOOK_RECORD";
        public const int TABLE_BOOK_RECORD_FONT_SIZE = 8;

        // simple plan: create a new row for each asset in the book
        // and Merge the first two cells with the common data: Date and Orden.
        // Calculate the total sum of war losses.
        public const int TABLE_BOOK_COLUMN_DATE = 0;
        public const int TABLE_BOOK_COLUMN_ORDEN = 1;
        public const int TABLE_BOOK_COLUMN_ASSET_NAME = 2;
        public const int TABLE_BOOK_COLUMN_ASSET_CODE = 3;
        public const int TABLE_BOOK_COLUMN_ASSET_UNIT = 4;
        public const int TABLE_BOOK_COLUMN_ASSET_COUNT = 5;
        public const int TABLE_BOOK_COLUMN_ASSET_PRICE = 6;
        public const int TABLE_BOOK_COLUMN_ASSET_SUM = 7;
        public const int TABLE_BOOK_COLUMN_ASSET_WAR_LOSSES = 17;

        public const string FIELD_TOTAL_WRITE_OFF_SUM = "«TOTAL_WRITE_OFF_SUM»";

        public const string FIELD_BOOK_YEAR = "«BOOK_YEAR»";
        public const string FIELD_BOOK_NUM = "«BOOK_NUM»";
        public const string FIELD_PAGE_NUM = "«PAGE_NUM»";
        public const string FIELD_RECORD_DATE = "«RECORD_DATE»";
        public const string FIELD_ORDEN_DATE = "«ORDEN_DATE»";
        public const string FIELD_ORDEN_NUM = "«ORDEN_NUM»";
    }
}
