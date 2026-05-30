using Mil.Paperwork.Domain.DataModels;

namespace Mil.Paperwork.Domain.Helpers
{
    internal static class WriteOffOrderHelper
    {
        public const string REPORT_TEMPLATE_NAME = "WriteOffOrderTemplate.docx";
        public const string OUTPUT_NAME_FORMAT = "Наказ про списання №{0}.docx";

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

        public static IList<BlockParagraph> BuildServicesBlock(IList<WriteOffServiceData> services)
        {
            var paragraphs = new List<BlockParagraph>();

            for (int i = 0; i < services.Count; i++)
            {
                var service = services[i];

                paragraphs.Add(new BlockParagraph($"\tЗа номенклатурою {service.ServiceName}:", IsBold: false, IndentLevel: 0));

                foreach (var asset in service.Assets)
                {
                    var line = $"-\t{asset.Name} – {asset.Count} {asset.MeasurementUnit}., залишковою вартістю {ReportHelper.GetPriceString(asset.Amount)} грн.;";
                    paragraphs.Add(new BlockParagraph(line, IsBold: false, IndentLevel: 1));
                }

                var subtotal = service.Assets.Sum(a => a.Amount);
                paragraphs.Add(new BlockParagraph(
                    $"\tЗагальна залишкова вартість {service.ServiceNameGenitive} - {ReportHelper.GetPriceString(subtotal)} грн.",
                    IsBold: false,
                    IndentLevel: 0));

                if (i < services.Count - 1)
                    paragraphs.Add(new BlockParagraph(string.Empty, IsBold: false, IndentLevel: 0));
            }

            return paragraphs;
        }

        public static string BuildToHeadsOfServices(IList<WriteOffServiceData> services)
        {
            if (services == null || services.Count == 0)
                return string.Empty;

            var parts = services.Select(s => $"Начальнику {s.ServiceNameGenitive}");
            return string.Join(", ", parts);
        }

        public static IList<BlockParagraph> BuildWitnessesBlock(IList<WriteOffWitnessData> witnesses)
        {
            var paragraphs = new List<BlockParagraph>();
            foreach (var w in witnesses)
            {
                var line = $"-\t{w.Rank} {w.Name}, {w.Position}.";
                paragraphs.Add(new BlockParagraph(line, IsBold: false, IndentLevel: 1));
            }
            return paragraphs;
        }

        public static decimal CalculateTotalSum(IList<WriteOffServiceData> services)
        {
            return Math.Round(services.Sum(s => s.Assets.Sum(a => a.Amount)), 2);
        }
    }
}
