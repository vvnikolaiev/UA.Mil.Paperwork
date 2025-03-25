using Mil.Paperwork.Domain.DataModels;
using Mil.Paperwork.Infrastructure.DataModels;

namespace Mil.Paperwork.Domain.Helpers
{
    public static class ReportDataHelper
    {
        public static IList<IAssetValuationData> GetValuationDataCollection(this WriteOffReportData reportData)
        {
            var valuationData = new List<IAssetValuationData>();
            var assetsValuationData = reportData.Assets
                .Where(x => x.ValuationData != null)
                .Select(x => x.ValuationData);

            if (assetsValuationData != null)
            {
                valuationData.AddRange(assetsValuationData);
            }

            if (reportData.Dismantlings != null)
            {
                valuationData.AddRange(reportData.Dismantlings);
            }

            return valuationData;
        }
    }
}
