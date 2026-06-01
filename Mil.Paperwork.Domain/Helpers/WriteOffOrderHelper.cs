using Mil.Paperwork.Domain.DataModels;

namespace Mil.Paperwork.Domain.Helpers
{
    public static class WriteOffOrderHelper
    {
        public const string REPORT_TEMPLATE_NAME = "WriteOffOrderTemplate.docx";
        public const string OUTPUT_NAME_FORMAT = "Наказ про списання №{0}.docx";

        public const int DocumentFontSize = 14;

        public const string MilUnitConfigKey = "MIL_UNIT";

        public const string FIELD_SERVICES_BLOCK = "SERVICES_BLOCK";
        public const string FIELD_EVENT_WITNESSES = "EVENT_WITNESSES";
        public const string FIELD_REPORT_NUM = "REPORT_NUM";
        public const string FIELD_REPORT_DATE = "REPORT_DATE";
        public const string FIELD_EVENT_DATE = "EVENT_DATE";
        public const string FIELD_EVENT_TIME = "EVENT_TIME";
        public const string FIELD_BATTLE_ORDER = "BATTLE_ORDER";
        public const string FIELD_BATTLE_ORDER_DATE = "BATTLE_ORDER_DATE";
        public const string FIELD_BATTLE_ORDER_LOCATION = "BATTLE_ORDER_LOCATION";
        public const string FIELD_SUBDIVISION_NAME = "SUBDIVISION_NAME";
        public const string FIELD_REPORTER_RANK = "REPORTER_RANK";
        public const string FIELD_REPORTER_NAME = "REPORTER_NAME";
        public const string FIELD_CREATOR_POSITION = "REPORT_CREATOR_POSITION";
        public const string FIELD_CREATOR_RANK = "REPORT_CREATOR_RANK";
        public const string FIELD_CREATOR_NAME = "REPORT_CREATOR_NAME";
        public const string FIELD_MIL_UNIT_APPROVAL = "MIL_UNIT_APPROVAL";
        public const string FIELD_WHAT_HAPPENED = "WHAT_HAPPENED";
        public const string FIELD_TOTAL_SUM = "TOTAL_SUM";
        public const string FIELD_TOTAL_SUM_TEXT = "TOTAL_SUM_TEXT";
        public const string FIELD_TO_HEADS_OF_SERVICES = "TO_HEADS_OF_SERVICES";

        private const int SumRoundingPrecision = 2;
        private const string ServicesSeparator = ", ";
        private const string ServiceHeaderFormat = "\tЗа номенклатурою {0}:";
        private const string AssetLineFormat = "-\t{0} – {1} {2}, залишковою вартістю {3} грн.;";
        private const string ServiceSubtotalFormat = "\tЗагальна залишкова вартість майна {0} – {1} грн.";
        private const string HeadOfServiceFormat = "начальнику {0}";
        private const string WitnessLineFormat = "-\t{0} {1}, {2} військової частини {3}.";

        internal static IList<BlockParagraph> BuildServicesBlock(IList<WriteOffServiceData> services)
        {
            var paragraphs = new List<BlockParagraph>();

            for (int i = 0; i < services.Count; i++)
            {
                var service = services[i];
                var serviceNameGen = service.ServiceNameGenitive?.ToLower();

                paragraphs.Add(new BlockParagraph(
                    service.ServiceName ?? string.Empty, IsBold: true, IndentLevel: 0, FontSize: DocumentFontSize));

                paragraphs.Add(new BlockParagraph(
                    string.Format(ServiceHeaderFormat, serviceNameGen), IsBold: false, IndentLevel: 0, FontSize: DocumentFontSize));

                foreach (var asset in service.Assets)
                {
                    var line = string.Format(AssetLineFormat,
                        asset.Name, asset.Count, asset.MeasurementUnit,
                        ReportHelper.GetPriceString(asset.Amount));
                    paragraphs.Add(new BlockParagraph(line, IndentLevel: 1, FontSize: DocumentFontSize));
                }

                var subtotal = service.Assets.Sum(a => a.Amount);
                paragraphs.Add(new BlockParagraph(
                    string.Format(ServiceSubtotalFormat, serviceNameGen, ReportHelper.GetPriceString(subtotal)), IndentLevel: 0, FontSize: DocumentFontSize));

                if (i < services.Count - 1)
                    paragraphs.Add(new BlockParagraph(string.Empty, IndentLevel: 0, FontSize: DocumentFontSize));
            }

            return paragraphs;
        }

        internal static string BuildToHeadsOfServices(IList<WriteOffServiceData> services)
        {
            if (services == null || services.Count == 0)
                return string.Empty;

            var parts = services.Select(s => string.Format(HeadOfServiceFormat, s.ServiceNameGenitive?.ToLower()));
            var sServices = string.Join(ServicesSeparator, parts);
            
            var result = string.IsNullOrEmpty(sServices) ? sServices : char.ToUpper(sServices[0]) + sServices[1..];
            return result;
        }

        internal static IList<BlockParagraph> BuildWitnessesBlock(IList<WriteOffWitnessData> witnesses, string milUnit)
        {
            var paragraphs = new List<BlockParagraph>();
            foreach (var w in witnesses)
            {
                var line = string.Format(WitnessLineFormat, w.Rank, w.Name, w.Position, milUnit);
                paragraphs.Add(new BlockParagraph(line, IndentLevel: 1, FontSize: DocumentFontSize));
            }
            return paragraphs;
        }

        internal static decimal CalculateTotalSum(IList<WriteOffServiceData> services)
        {
            return Math.Round(services.Sum(s => s.Assets.Sum(a => a.Amount)), SumRoundingPrecision);
        }
    }
}
