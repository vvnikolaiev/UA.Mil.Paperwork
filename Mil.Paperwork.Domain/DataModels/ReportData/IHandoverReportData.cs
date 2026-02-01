using Mil.Paperwork.Domain.DataModels.Assets;
using Mil.Paperwork.Infrastructure.DataModels;

namespace Mil.Paperwork.Domain.DataModels.ReportData
{
    public interface IHandoverReportData : IReportData
    {
        string DocumentNumber { get; set; }
        DateTime DocumentDate { get; set; }
        DateTime? DateStart { get; set; }
        DateTime? DateEnd { get; set; }

        string Supplier { get; set; }
        string Receiver { get; set;}

        string ReasonDocumentName { get; set; }
        string ReasonDocumentNumber { get; set; }
        DateTime ReasonDocumentDate { get; set; }

        string Reason { get; set; }
        PersonDTO PersonResponsible { get; set; }
        PersonDTO PersonReceiver { get; set; }
        IList<IAssetInfo> Assets { get; set; }
    }
}
