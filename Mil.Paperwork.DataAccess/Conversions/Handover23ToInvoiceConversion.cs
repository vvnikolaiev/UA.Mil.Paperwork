using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Infrastructure.Enums;

namespace Mil.Paperwork.DataAccess.Conversions
{
    public class Handover23ToInvoiceConversion : IReportDataConversion
    {
        private const int DefaultValidDays = 10;

        public ReportType SourceType => ReportType.Handover23Act;

        public ReportType TargetType => ReportType.Invoice;

        public IReadOnlyList<IReportData> Convert(IReportData source)
        {
            var result = new List<IReportData>();

            if (source is not IHandoverReportData handoverData)
            {
                return result;
            }

            var invoiceData = new InvoceReportData
            {
                DocumentNumber = handoverData.DocumentNumber ?? string.Empty,
                DateCreated = handoverData.DocumentDate,
                DueDate = handoverData.DocumentDate.AddDays(DefaultValidDays),
                Reason = handoverData.ReasonDocumentName ?? string.Empty,
                Transmitter = handoverData.PersonResponsible,
                Recipient = handoverData.PersonReceiver,
                Assets = [.. handoverData.Assets ?? []]
            };

            result.Add(invoiceData);

            return result;
        }
    }
}
