using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Domain.Helpers;
using Mil.Paperwork.Infrastructure.DataModels;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Services;
using Spire.Doc;
using Spire.Doc.Documents;
using System.IO;

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

                var document = new Document();
                document.LoadFromFile(templatePath);

                FillTheFields(reportData, document);

                var tables = document.Sections[0].Tables.Cast<Table>().ToList();

                var table = tables[4]; // ??

                if (table != null)
                {
                    if (reportData.AssetIds == null || reportData.AssetIds.Count == 0)
                    {
                        reportData.AssetIds = [new ProductIdentification()];
                    }

                    var count = reportData.AssetIds.Count == 1 ? reportData.Count : 1;

                    FillTheTable(reportData.Asset, reportData.AssetIds, count, table);
                }

                using var memoryStream = new MemoryStream();
                document.SaveToStream(memoryStream, FileFormat.Docx);
                _reportBytes = memoryStream.ToArray();

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

        private void FillTheFields(ICommissioningActReportData reportData, Document document)
        {
            var reportConfig = _reportDataService.GetReportParametersDictionary(ReportType.CommissioningAct);

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

        private static void FillTheTable(IProductData productData, IList<IProductIdentification> identifiers, int count, Table table)
        {
            var firstRow = table.LastRow;

            int totalCount = 0;
            var countItems = identifiers?.Count ?? 1;

            for (int i = 0; i < countItems; i++)
            {
                TableRow row = table.AddRow();

                var fontSize = CommissioningActHelper.TABLE_FONT_SIZE;

                var identifier = identifiers?.Count >= i ? identifiers[i] : new ProductIdentification();
                var totalSum = productData.Price * count;

                row.Cells[CommissioningActHelper.COLUMN_InventoryNumber].AddText(identifier.InventoryNumber, fontSize);
                row.Cells[CommissioningActHelper.COLUMN_Count].AddNumber(count, fontSize);
                row.Cells[CommissioningActHelper.COLUMN_Price].AddPrice(productData.Price, fontSize);
                row.Cells[CommissioningActHelper.COLUMN_TotalPrice].AddPrice(totalSum, fontSize);
                if (productData.WarrantyPeriodMonths > 0)
                {
                    row.Cells[CommissioningActHelper.COLUMN_WarrantyPeriod].AddNumber(productData.WarrantyPeriodMonths, fontSize);
                }
                if (productData.YearManufactured > 0)
                {
                    row.Cells[CommissioningActHelper.COLUMN_ManufacturedYear].AddNumber(productData.YearManufactured, fontSize);
                }

                row.Cells[CommissioningActHelper.COLUMN_SerialNumber].AddText(identifier.SerialNumber, fontSize);

                totalCount += count;
            }

            table.Rows.Remove(firstRow);

            AddSummaryRow(productData.Price, totalCount, table);
        }

        private static void AddSummaryRow(decimal price, int count, Table table)
        {
            var totalSum = Math.Round(price * count, 2);

            var totalSumText = ReportHelper.ConvertTotalSumToUkrainianString(totalSum);

            // last united string row 
            var textSummaryRow = table.AddRow(false);
            textSummaryRow.Cells[0].AddText(SummaryRowTotalText, CommissioningActHelper.TABLE_FONT_SIZE, HorizontalAlignment.Right);
            textSummaryRow.Cells[CommissioningActHelper.COLUMN_Count].AddNumber(count, CommissioningActHelper.TABLE_FONT_SIZE);
            textSummaryRow.Cells[CommissioningActHelper.COLUMN_Price].AddPrice(totalSum, CommissioningActHelper.TABLE_FONT_SIZE);
            textSummaryRow.Cells[CommissioningActHelper.COLUMN_TotalPrice].AddPrice(totalSum, CommissioningActHelper.TABLE_FONT_SIZE);
        }
    }
}
