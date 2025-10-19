using Mil.Paperwork.Domain.DataModels.Assets;
using Mil.Paperwork.Infrastructure.DataModels;

namespace Mil.Paperwork.Domain.DataModels.ReportData
{
    public class InvoceReportData : IInvoceReportData
    {
        public string DocumentNumber { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DueDate { get; set; }
        public string Reason { get; set; }
        public PersonDTO Recipient { get; set; }
        public PersonDTO Transmitter { get; set; }
        public PersonDTO HeadOfService { get; set; }
        public IList<IAssetInfo> Assets { get; set; } = new List<IAssetInfo>();

        public string DestinationFolder { get; set; }

        public string GetDestinationPath()
        {
            return DestinationFolder;
        }
    }
}
