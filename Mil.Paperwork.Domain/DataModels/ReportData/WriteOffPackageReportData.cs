using Mil.Paperwork.Domain.DataModels.Assets;
using Mil.Paperwork.Domain.DataModels.Parameters;

namespace Mil.Paperwork.Domain.DataModels.ReportData
{
    public class WriteOffPackageReportData : IWriteOffPackageReportData
    {
        public IList<IAssetInfo> Assets { get; set; }

        public DateTime DocumentDate { get; set; }

        public DateTime EventDate { get; set; }

        public int OrdenNumber { get; set; }

        public DateTime OrdenDate { get; set; }

        public IBookExtractData? BookOfLossesExtractData { get; set; }

        public string DestinationFolder { get; set; }

        public string GetDestinationPath()
        {
            return DestinationFolder;
        }
    }
}
