using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Infrastructure.Enums;

namespace Mil.Paperwork.DataAccess.Conversions
{
    public class InvoiceToHandover23Conversion : IReportDataConversion
    {
        public ReportType SourceType => ReportType.Invoice;

        public ReportType TargetType => ReportType.Handover23Act;

        public IReadOnlyList<IReportData> Convert(IReportData source)
        {
            var result = new List<IReportData>();

            if (source is not IInvoceReportData invoiceData)
            {
                return result;
            }

            var handoverData = new HandoverReportData
            {
                DocumentNumber = invoiceData.DocumentNumber ?? string.Empty,
                DocumentDate = invoiceData.DateCreated,
                ReasonDocumentName = invoiceData.Reason ?? string.Empty,
                ReasonDocumentDate = invoiceData.DateCreated,
                PersonResponsible = invoiceData.Transmitter,
                PersonReceiver = invoiceData.Recipient,
                Assets = [.. invoiceData.Assets ?? []]
            };

            result.Add(handoverData);

            return result;
        }
    }
}
