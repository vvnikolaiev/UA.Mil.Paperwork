using Mil.Paperwork.Domain.DataModels.Assets;
using Mil.Paperwork.Domain.Enums;

namespace Mil.Paperwork.Domain.DataModels.Parameters
{
    public interface ITechnicalStateReportParameters
    {
        IAssetInfo AssetInfo { get; set; }
        EventType EventType { get; set; }
        DateTime DocumentDate { get; set; }
        DateTime EventDate { get; set; }
        int OrdenNumber { get; set; }
        DateTime OrdenDate { get; set; }
        string Reason { get; set; }
    }
}
