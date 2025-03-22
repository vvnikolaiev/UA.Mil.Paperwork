namespace Mil.Paperwork.WriteOff.Helpers
{
    internal static class TextFormatHelper
    {
        public const string ReportCreatedSuccessfullyFormat = "{0} сформовано успішно.";
        public const string FailedToCreateReportFormat = "Не вдалося сформувати {0}.";

        public const string ResidualValueReportName = "Акт залишкової вартості";
        public const string QualityStateReportName = "Акт зміни якісного стану";
        public const string TechnicalStateReportName = "Акт технічного стану";
        public const string ValuationReportName = "Акт оцінки майна";
        public const string DismantlingReportName = "Акт зміни якісного стану - РОЗУКОМПЛЕКТАЦІЇ - майна";

        private const string DismantlingReasonP1TextFormat = "У зв’язку із втратою {0} для {1}, {2},";
        private const string DismantlingReasonP2TextFormat = "необхідно вилучити {0} {1} для {2} подальшого списання";

        public static string GetReportStatusMessage(string reportName, bool isSuccessful)
        {
            var textFormat = isSuccessful ? ReportCreatedSuccessfullyFormat : FailedToCreateReportFormat;
            var result = string.Format(textFormat, reportName);
            return result;
        }

        public static string GetDismantlingDescriptionText(string[] excludedComponentNames, string assetFullName, string reason)
        {
            if (excludedComponentNames == null || excludedComponentNames.Length == 0)
            {
                return string.Empty;
            }

            var itemsCount = excludedComponentNames.Length;

            // TODO: REFACTOR constants
            var componentsText = itemsCount == 1 ? excludedComponentNames.FirstOrDefault() : "компонентів";
            var lostText = itemsCount == 1 ? "втрачену" : "втрачені";
            var itText = itemsCount == 1 ? "його" : "їхнього";

            var part1 = string.Format(DismantlingReasonP1TextFormat, componentsText, assetFullName, reason);

            var ultimateItemIndex = itemsCount - 1;
            var penultimateItemIndex = itemsCount - 2;

            string componentNamesText = string.Empty;
            for (int i = 0; i < itemsCount; i++)
            {
                componentNamesText += excludedComponentNames[i];

                if (i == ultimateItemIndex)
                {
                    continue;
                }
                else if (i == penultimateItemIndex)
                {
                    componentNamesText += " та ";
                }
                else
                {
                    componentNamesText += ", ";
                }
            }

            //необхідно вилучити втрачену акумуляторну батарею ємністю 3100 мАг для її подальшого списання,
            var part2 = string.Format(DismantlingReasonP2TextFormat, lostText, componentNamesText, itText);

            return $"{part1} {part2}";
        }
    }
}
