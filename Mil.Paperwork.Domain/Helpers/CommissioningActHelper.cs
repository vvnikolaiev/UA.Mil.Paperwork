using OfficeOpenXml;

namespace Mil.Paperwork.Domain.Helpers
{
    public class CommissioningActHelper
    {
        public const string FIELD_DOC_NUMBER = "DOC_NUMBER";
        public const string FIELD_ASSET_NAME = "ASSET_NAME";
        public const string FIELD_ASSET_STATE = "ASSET_STATE";
        public const string FIELD_COUNT_TEXT = "COUNT_TEXT";
        public const string FIELD_SHORT_CHARACTERISTIC = "SHORT_CHARACTERISTIC";
        public const string FIELD_COMPLETION_STATE = "COMPLETION_STATE";
        public const string FIELD_ASSET_COMPLIANCE = "ASSET_COMPLIANCE";
        public const string FIELD_TEST_RESULTS = "TEST_RESULTS";
        public const string FIELD_OTHER_INFO = "OTHER_INFO";
        public const string FIELD_COMISSION_CONCLUSION = "COMISSION_CONCLUSION";
        public const string FIELD_ATTACHED_DOCUMENTATION = "ATTACHED_DOCUMENTATION";
        public const string FIELD_ACCEPTED_PERSON_POSITION = "ACCEPTED_PERSON_POSITION";
        public const string FIELD_ACCEPTED_PERSON_RANK = "ACCEPTED_PERSON_RANK";
        public const string FIELD_ACCEPTED_PERSON_NAME = "ACCEPTED_PERSON_NAME";
        public const string FIELD_HANDED_PERSON_POSITION = "HANDED_PERSON_POSITION";
        public const string FIELD_HANDED_PERSON_RANK = "HANDED_PERSON_RANK";
        public const string FIELD_HANDED_PERSON_NAME = "HANDED_PERSON_NAME";
        public const string FIELD_COMMISSIONED_LOCATIONN = "COMMISSIONED_LOCATION";

        public const string REPORT_TEMPLATE_NAME = "CommissioningActTemplate.docx";
        public const string OUTPUT_REPORT_NAME_TEMPLATE = "Акт введення в експлуатацію {0}.docx";
        public const string TABLE_ASSETS_NAME = "TABLE_ASSETS";

        public const int TABLE_FONT_SIZE = 9;

        public const int COLUMN_InventoryNumber = 0;
        public const int COLUMN_Count = 1;
        public const int COLUMN_Price = 2;
        public const int COLUMN_TotalPrice = 3;
        public const int COLUMN_WarrantyPeriod = 7;
        public const int COLUMN_SerialNumber = 9;
    }
}
