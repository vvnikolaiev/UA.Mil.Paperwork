using Mil.Paperwork.Domain.DataModels.Parameters;
using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Domain.Helpers;
using Mil.Paperwork.Infrastructure.DataModels;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Services;

namespace Mil.Paperwork.Domain.Reports
{
    internal class CommissioningActReport : IReport
    {
        private const string SummaryRowTotalText = "Всього:";

        private readonly IReportDataService _reportDataService;

        private byte[] _reportBytes;

        public CommissioningActReport(IReportDataService reportDataService)
        {
            _reportDataService = reportDataService;
        }

        public bool TryCreate(ICommissioningActReportData reportData)
        {
            try
            {
                var templatePath = PathsHelper.GetTemplatePath(CommissioningActHelper.REPORT_TEMPLATE_NAME);

                using var document = WordDocument.LoadFromFile(templatePath);

                FillCommission(document);
                FillTheFields(reportData, document);

                var table = document.GetTable(CommissioningActHelper.TABLE_ASSETS_NAME);

                if (table != null)
                {
                    if (reportData.AssetIds == null || reportData.AssetIds.Count == 0)
                        reportData.AssetIds = [new ProductIdentification()];

                    var count = reportData.AssetIds.Count == 1 ? reportData.Count : 1;
                    FillTheTable(reportData.Asset, reportData.AssetIds, count, table);
                }

                _reportBytes = document.GetBytes();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return false;
            }
        }

        public byte[] GetReportBytes()
        {
            return _reportBytes;
        }

        private void FillCommission(WordDocument document)
        {
            var dictCommissionFields = ReportParametersHelper.GetCommission(ReportType.CommissioningAct, _reportDataService);
            document.ReplaceFields(dictCommissionFields);
        }

        private void FillTheFields(ICommissioningActReportData reportData, WordDocument document)
        {
            var reportConfig = ReportParametersHelper.GetFullParametersDictionary(ReportType.CommissioningAct, _reportDataService);

            document.ReplaceField(CommissioningActHelper.FIELD_DOC_NUMBER, reportData.DocumentNumber);
            document.ReplaceField(CommissioningActHelper.FIELD_DOC_DATE, reportData.DocumentDate.ToString(ReportHelper.DATE_FORMAT));
            document.ReplaceField(CommissioningActHelper.FIELD_ASSET_NAME, reportData.Asset.Name);
            document.ReplaceField(CommissioningActHelper.FIELD_ASSET_STATE, reportData.AssetState);
            document.ReplaceField(CommissioningActHelper.FIELD_COUNT_TEXT, reportData.CountText);
            document.ReplaceField(CommissioningActHelper.FIELD_COMMISSIONED_LOCATIONN, reportData.CommissioningLocation ?? string.Empty);
            document.ReplaceField(CommissioningActHelper.FIELD_SHORT_CHARACTERISTIC, reportData.ShortCharacteristic);
            document.ReplaceField(CommissioningActHelper.FIELD_COMPLETION_STATE, reportData.CompletionState);
            document.ReplaceField(CommissioningActHelper.FIELD_ASSET_COMPLIANCE, reportData.AssetCompliance);
            document.ReplaceField(CommissioningActHelper.FIELD_TEST_RESULTS, reportData.TestResults);
            document.ReplaceField(CommissioningActHelper.FIELD_OTHER_INFO, reportData.OtherInfo);
            document.ReplaceField(CommissioningActHelper.FIELD_COMISSION_CONCLUSION, reportData.Conclusion);
            document.ReplaceField(CommissioningActHelper.FIELD_ATTACHED_DOCUMENTATION, reportData.AttachedDocumentation);
            document.ReplaceField(CommissioningActHelper.FIELD_ACCEPTED_PERSON_POSITION, reportData.PersonAccepted?.Position ?? string.Empty);
            document.ReplaceField(CommissioningActHelper.FIELD_ACCEPTED_PERSON_RANK, reportData.PersonAccepted?.Rank ?? string.Empty);
            document.ReplaceField(CommissioningActHelper.FIELD_ACCEPTED_PERSON_NAME, reportData.PersonAccepted?.FullName ?? string.Empty);
            document.ReplaceField(CommissioningActHelper.FIELD_HANDED_PERSON_POSITION, reportData.PersonHanded?.Position ?? string.Empty);
            document.ReplaceField(CommissioningActHelper.FIELD_HANDED_PERSON_RANK, reportData.PersonHanded?.Rank ?? string.Empty);
            document.ReplaceField(CommissioningActHelper.FIELD_HANDED_PERSON_NAME, reportData.PersonHanded?.FullName ?? string.Empty);

            document.ReplaceFields(reportConfig);
        }

        private static void FillTheTable(IProductData productData, IList<IProductIdentification> identifiers, int count, WordTable table)
        {
            var firstRow = table.LastRow;
            int totalCount = 0;
            var countItems = identifiers?.Count ?? 1;
            var cellParameters = new WordCellParameters(CommissioningActHelper.TABLE_FONT_SIZE, WordHorizontalAlignment.Center);

            for (int i = 0; i < countItems; i++)
            {
                var row = table.AddRow();
                var identifier = identifiers?.Count >= i ? identifiers[i] : new ProductIdentification();
                var totalSum = productData.Price * count;

                row.GetCell(CommissioningActHelper.COLUMN_InventoryNumber).AddText(identifier.InventoryNumber, cellParameters);
                row.GetCell(CommissioningActHelper.COLUMN_Count).AddNumber(count, cellParameters);
                row.GetCell(CommissioningActHelper.COLUMN_Price).AddPrice(productData.Price, cellParameters);
                row.GetCell(CommissioningActHelper.COLUMN_TotalPrice).AddPrice(totalSum, cellParameters);
                if (productData.ResourceYears > 0)
                    row.GetCell(CommissioningActHelper.COLUMN_WarrantyPeriod).AddNumber(productData.ResourceYears * 12, cellParameters);
                if (productData.YearManufactured > 0)
                    row.GetCell(CommissioningActHelper.COLUMN_ManufacturedYear).AddNumber(productData.YearManufactured, cellParameters);
                row.GetCell(CommissioningActHelper.COLUMN_SerialNumber).AddText(identifier.SerialNumber, cellParameters);

                totalCount += count;
            }

            table.RemoveRow(firstRow);

            AddSummaryRow(productData.Price, totalCount, table);
        }

        private static void AddSummaryRow(decimal price, int count, WordTable table)
        {
            var nameCellParameters = new WordCellParameters(CommissioningActHelper.TABLE_FONT_SIZE, WordHorizontalAlignment.Right);
            var cellParameters = new WordCellParameters(CommissioningActHelper.TABLE_FONT_SIZE, WordHorizontalAlignment.Center);

            var totalSum = Math.Round(price * count, 2);

            var textSummaryRow = table.AddRow();
            textSummaryRow.GetCell(0).AddText(SummaryRowTotalText, nameCellParameters);
            textSummaryRow.GetCell(CommissioningActHelper.COLUMN_Count).AddNumber(count, cellParameters);
            textSummaryRow.GetCell(CommissioningActHelper.COLUMN_Price).AddPrice(totalSum, cellParameters);
            textSummaryRow.GetCell(CommissioningActHelper.COLUMN_TotalPrice).AddPrice(totalSum, cellParameters);
        }
    }
}
