using Mil.Paperwork.Domain.DataModels.Assets;
using Mil.Paperwork.Infrastructure.DataModels;

namespace Mil.Paperwork.Domain.DataModels.ReportData
{
    public interface IInvoceReportData : IReportData
    {
        string DocumentNumber { get; set; }
        DateTime DateCreated { get; set; }
        DateTime DueDate { get; set; }
        string Reason { get; set; }
        PersonDTO Recipient { get; set; }
        PersonDTO Transmitter { get; set; }
        IList<IAssetInfo> Assets { get; set; }
    }
}
