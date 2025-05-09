using Mil.Paperwork.Domain.DataModels.Assets;
using Mil.Paperwork.Domain.Enums;

namespace Mil.Paperwork.Domain.DataModels
{
    public class TechnicalStateReportData : ITechnicalStateReportData
    {
        public string Reason { get; set; }

        public DateTime ReportDate { get; set; }


        public EventType EventType { get; set; }

        public IList<IAssetInfo> Assets { get; set; }

        public string DestinationFolder { get; set; }

        public string GetDestinationPath()
        {
            return DestinationFolder;
        }
    }
}
