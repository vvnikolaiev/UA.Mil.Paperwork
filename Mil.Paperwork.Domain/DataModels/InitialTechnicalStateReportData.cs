using Mil.Paperwork.Domain.DataModels.Assets;
using Mil.Paperwork.Domain.Enums;

namespace Mil.Paperwork.Domain.DataModels
{
    public class InitialTechnicalStateReportData : IInitialTechnicalStateReportData
    {
        public EventType EventType { get; set; }

        public IList<IAssetInfo> Assets { get; set; }

        public string DestinationFolder { get; set; }

        public string GetDestinationPath()
        {
            return DestinationFolder;
        }
    }
}
