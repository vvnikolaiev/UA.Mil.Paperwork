using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Infrastructure.Enums;

namespace Mil.Paperwork.DataAccess.Conversions
{
    public class InvoiceToInitialTechnicalStateConversion : IReportDataConversion
    {
        public ReportType SourceType => ReportType.Invoice;

        public ReportType TargetType => ReportType.TechnicalStateReport;

        public IReadOnlyList<IReportData> Convert(IReportData source)
        {
            var result = new List<IReportData>();

            if (source is not IInvoceReportData invoiceData)
            {
                return result;
            }

            var reportData = new InitialTechnicalStateReportData
            {
                Assets = [.. invoiceData.Assets ?? []],
                PersonAccepted = invoiceData.Recipient,
                PersonHanded = invoiceData.Transmitter
            };

            result.Add(reportData);

            return result;
        }
    }
}
