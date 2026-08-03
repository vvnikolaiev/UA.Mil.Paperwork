using Mil.Paperwork.Domain.DataModels;
using Mil.Paperwork.Domain.DataModels.Parameters;
using Mil.Paperwork.Infrastructure.DataModels;

namespace Mil.Paperwork.Domain.Helpers
{
    public static class EASHelper
    {
        public const string REPORT_TEMPLATE_NAME = "EASTemplate.docx";
        public const string OUTPUT_NAME_FORMAT = "Єдиний акт списання №{0}.docx";

        public const string FIELD_REPORT_NUM = "REPORT_NUM";
        public const string FIELD_REPORT_DATE = "REPORT_DATE";
        public const string FIELD_EVENT_DATE = "EVENT_DATE";
        public const string FIELD_EVENT_TIME = "EVENT_TIME";
        public const string FIELD_BATTLE_ORDER = "BATTLE_ORDER";
        public const string FIELD_BATTLE_ORDER_DATE = "BATTLE_ORDER_DATE";
        public const string FIELD_SUBDIVISION_NAME = "SUBDIVISION_NAME";
        public const string FIELD_REPORTER_RANK = "REPORTER_RANK";
        public const string FIELD_REPORTER_NAME = "REPORTER_NAME";
        public const string FIELD_WHAT_HAPPENED = "WHAT_HAPPENED";
        public const string FIELD_ORDEN_NUM = "ORDEN_NUM";
        public const string FIELD_ORDEN_DATE = "ORDEN_DATE";
        public const string FIELD_EVENT_WITNESSES_BLOCK = "EVENT_WITNESSES_BLOCK";
        public const string FIELD_EVENT_WITNESSES_TEXT = "EVENT_WITNESSES_TEXT";
        public const string FIELD_HEADS_OF_SERVICES_BLOCK = "HEADS_OF_SERVICES_BLOCK";

        public const string ASSETS_TABLE_NAME = "ASSETS_TABLE";

        private const int AssetsTableHeaderRowCount = 4;
        private const int AssetsTableColumnNumber = 0;
        private const int AssetsTableColumnName = 1;
        private const int AssetsTableColumnCode = 3;
        private const int AssetsTableColumnUnit = 4;
        private const int AssetsTableColumnCategory = 5;
        private const int AssetsTableColumnCount = 6;
        private const int AssetsTableColumnOriginalPrice = 7;
        private const int AssetsTableColumnResidualPrice = 8;
        private const int AssetsTableColumnSum = 9;
        private const int AssetsTableGroupTitleSpan = 10;
        private const int AssetsTableGroupFooterSpan = 9;
        private const int AssetsTableGrandTotalSpan = 3;

        private const int WitnessesBlockFontSize = 11;
        private const int HeadsOfServicesBlockFontSize = 12;

        private const string GroupTitleFormat = "номенклатура {0}";
        private const string GroupFooterLabel = "Усього за зазначеною номенклатурою";
        private const string GrandTotalLabel = "Усього";

        private const string PersonPositionLineFormat = "{0} військової частини {1},";
        private const string WitnessNameLineFormat = "{0}\t{1} {2}";
        private const string HeadNameLineFormat = "{0}\t{1}";
        private const string WitnessTextItemFormat = "{0} {1} {2} {3}, {4} військової частини {5}";
        private const string WitnessTextSeparator = "; ";

        internal static IList<BlockParagraph> BuildWitnessesBlock(IList<PersonDTO> witnesses, string milUnit)
        {
            var paragraphs = new List<BlockParagraph>();

            foreach (var witness in witnesses)
            {
                paragraphs.Add(new BlockParagraph(
                    string.Format(PersonPositionLineFormat, witness.Position, milUnit),
                    IndentLevel: 0, FontSize: WitnessesBlockFontSize));

                paragraphs.Add(new BlockParagraph(
                    string.Format(WitnessNameLineFormat, witness.Rank, witness.FirstName, witness.LastName),
                    IndentLevel: 0, FontSize: WitnessesBlockFontSize));
            }

            return paragraphs;
        }

        internal static string BuildWitnessesText(IList<PersonDTO> witnesses, string milUnit)
        {
            var items = witnesses.Select(w => string.Format(
                WitnessTextItemFormat, w.Rank, w.LastName.ToUpper(), w.FirstName, w.Patronymic, w.Position, milUnit));

            var result = string.Join(WitnessTextSeparator, items);
            return result;
        }

        internal static IList<BlockParagraph> BuildHeadsOfServicesBlock(IList<EASServiceData> services, string milUnit)
        {
            var paragraphs = new List<BlockParagraph>();

            foreach (var service in services)
            {
                paragraphs.Add(new BlockParagraph(
                    string.Format(PersonPositionLineFormat, service.HeadPosition, milUnit),
                    IndentLevel: 0, FontSize: HeadsOfServicesBlockFontSize, IsBold: true));

                paragraphs.Add(new BlockParagraph(
                    string.Format(HeadNameLineFormat, service.HeadRank, service.HeadName),
                    IndentLevel: 0, FontSize: HeadsOfServicesBlockFontSize, IsBold: true));
            }

            return paragraphs;
        }

        internal static decimal CalculateAssetSum(EASAssetData asset)
        {
            var result = Math.Round(asset.Count * asset.ResidualPrice, 2);
            return result;
        }

        internal static decimal CalculateGroupSubtotal(EASServiceData service)
        {
            var result = Math.Round(service.Assets.Sum(CalculateAssetSum), 2);
            return result;
        }

        internal static decimal CalculateGrandTotal(IList<EASServiceData> services)
        {
            var result = Math.Round(services.Sum(CalculateGroupSubtotal), 2);
            return result;
        }

        internal static int CalculateGrandTotalCount(IList<EASServiceData> services)
        {
            var result = services.Sum(s => s.Assets.Sum(a => a.Count));
            return result;
        }

        internal static void FillAssetsTable(WordTable table, IList<EASServiceData> services)
        {
            // Drop the example groups that ship with the template, keeping only the 4 static header rows.
            while (table.LastRow.GetRowIndex() >= AssetsTableHeaderRowCount)
            {
                table.RemoveRow(table.LastRow);
            }

            var titleParams = new WordCellParameters(11, WordHorizontalAlignment.Center, isBold: true);
            var nameParams = new WordCellParameters(11, WordHorizontalAlignment.Left);
            var cellParams = new WordCellParameters(11, WordHorizontalAlignment.Center);
            var labelParams = new WordCellParameters(11, WordHorizontalAlignment.Left, isBold: true);
            var totalParams = new WordCellParameters(11, WordHorizontalAlignment.Center, isBold: true);

            var titleRowIndices = new List<int>();
            var footerRowIndices = new List<int>();
            var assetNumber = 0;

            // Pass 1 — append every row (title/asset/footer/grand-total) in the unmerged 9-cell
            // shape inherited from the header's index row, writing text into the columns that
            // matter for each row's role and leaving the rest blank. Merging happens afterwards
            // (pass 2) because AddRow() always clones the immediately preceding row's cell shape,
            // and CreateMergedCell cannot be reversed — merging early would corrupt the shape that
            // subsequent AddRow() calls rely on.
            foreach (var service in services)
            {
                var titleRow = table.AddRow();
                titleRowIndices.Add(titleRow.GetRowIndex());
                titleRow.GetCell(AssetsTableColumnNumber).AddText(
                    string.Format(GroupTitleFormat, service.ServiceNameGenitive), titleParams);

                foreach (var asset in service.Assets)
                {
                    assetNumber++;
                    var row = table.AddRow();
                    var sum = CalculateAssetSum(asset);

                    row.GetCell(AssetsTableColumnNumber).AddNumber(assetNumber, cellParams);
                    row.GetCell(AssetsTableColumnName).AddText(asset.Name, nameParams);
                    row.GetCell(AssetsTableColumnCode).AddText(asset.Code, cellParams);
                    row.GetCell(AssetsTableColumnUnit).AddText(asset.MeasurementUnit, cellParams);
                    row.GetCell(AssetsTableColumnCategory).AddText(ReportHelper.ConvertCategoryToText(asset.Category), cellParams);
                    row.GetCell(AssetsTableColumnCount).AddNumber(asset.Count, cellParams);
                    row.GetCell(AssetsTableColumnOriginalPrice).AddPrice(asset.OriginalPrice, cellParams);
                    row.GetCell(AssetsTableColumnResidualPrice).AddPrice(asset.ResidualPrice, cellParams);
                    row.GetCell(AssetsTableColumnSum).AddPrice(sum, cellParams);
                }

                var footerRow = table.AddRow();
                footerRowIndices.Add(footerRow.GetRowIndex());
                footerRow.GetCell(AssetsTableColumnNumber).AddText(GroupFooterLabel, labelParams);
                footerRow.GetCell(AssetsTableColumnSum).AddPrice(CalculateGroupSubtotal(service), totalParams);
            }

            var totalRow = table.AddRow();
            var totalRowIndex = totalRow.GetRowIndex();
            totalRow.GetCell(AssetsTableColumnNumber).AddText(GrandTotalLabel, labelParams);
            totalRow.GetCell(AssetsTableColumnCount).AddNumber(CalculateGrandTotalCount(services), totalParams);
            totalRow.GetCell(AssetsTableColumnSum).AddPrice(CalculateGrandTotal(services), totalParams);

            // Pass 2 — merge title/footer/grand-total rows into their final shape now that no
            // further AddRow() calls will clone them.
            foreach (var index in titleRowIndices)
            {
                table.GetRow(index).CreateMergedCell(AssetsTableColumnNumber, AssetsTableGroupTitleSpan);
            }

            foreach (var index in footerRowIndices)
            {
                table.GetRow(index).CreateMergedCell(AssetsTableColumnNumber, AssetsTableGroupFooterSpan);
            }

            table.GetRow(totalRowIndex).CreateMergedCell(AssetsTableColumnNumber, AssetsTableGrandTotalSpan);
        }
    }
}
