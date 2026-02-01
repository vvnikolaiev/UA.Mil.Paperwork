using Mil.Paperwork.Domain.DataModels.Assets;
using Mil.Paperwork.Infrastructure.DataModels;

namespace Mil.Paperwork.Domain.DataModels.ReportData
{
    public class HandoverReportData : IHandoverReportData
    {
        public string DocumentNumber { get; set; }
        public DateTime DocumentDate { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public string Supplier { get; set; }
        public string Receiver { get; set;}
        public string ReasonDocumentName { get; set; }
        public string ReasonDocumentNumber { get; set; }
        public DateTime ReasonDocumentDate { get; set; }
        public string Reason { get; set; }
        public PersonDTO PersonResponsible { get; set; }
        public PersonDTO PersonReceiver { get; set; }
        public IList<IAssetInfo> Assets { get; set; } = [];

        public string DestinationFolder { get; set; }

        public string GetDestinationPath()
        {
            return DestinationFolder;
        }
    }
}
