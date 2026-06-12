using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Infrastructure.Enums;

namespace Mil.Paperwork.DataAccess.Conversions
{
    public class InvoiceToCommissioningActConversion : IReportDataConversion
    {
        public ReportType SourceType => ReportType.Invoice;

        public ReportType TargetType => ReportType.CommissioningAct;

        public IReadOnlyList<IReportData> Convert(IReportData source)
        {
            var result = new List<IReportData>();

            if (source is not IInvoceReportData invoiceData)
            {
                return result;
            }

            foreach (var asset in invoiceData.Assets ?? [])
            {
                var actData = new CommissioningActReportData
                {
                    Asset = asset,
                    AssetIds = [new ProductIdentification { SerialNumber = asset.SerialNumber ?? string.Empty }],
                    Count = asset.Count,
                    DocumentNumber = invoiceData.DocumentNumber ?? string.Empty,
                    DocumentDate = invoiceData.DateCreated,
                    PersonHanded = invoiceData.Transmitter,
                    PersonAccepted = invoiceData.Recipient
                };

                result.Add(actData);
            }

            return result;
        }
    }
}
