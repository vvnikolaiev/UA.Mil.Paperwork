using Mil.Paperwork.Domain.DataModels.Assets;
using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Infrastructure.DataModels;
using Mil.Paperwork.Infrastructure.Enums;

namespace Mil.Paperwork.DataAccess.Conversions
{
    public class CommissioningActToInvoiceConversion : IReportDataConversion
    {
        private const int DefaultValidDays = 10;

        public ReportType SourceType => ReportType.CommissioningAct;

        public ReportType TargetType => ReportType.Invoice;

        public IReadOnlyList<IReportData> Convert(IReportData source)
        {
            var result = new List<IReportData>();

            if (source is not ICommissioningActReportData actData || actData.Asset == null)
            {
                return result;
            }

            var invoiceData = new InvoceReportData
            {
                DocumentNumber = actData.DocumentNumber ?? string.Empty,
                DateCreated = actData.DocumentDate,
                DueDate = actData.DocumentDate.AddDays(DefaultValidDays),
                Transmitter = ConversionHelper.ToPersonDTO(actData.PersonHanded),
                Recipient = ConversionHelper.ToPersonDTO(actData.PersonAccepted),
                Assets = [ConversionHelper.ToAssetInfo(actData)]
            };

            result.Add(invoiceData);

            return result;
        }
    }
}
