using Mil.Paperwork.Domain.DataModels;
using Mil.Paperwork.Infrastructure.DataModels;

namespace Mil.Paperwork.Domain.Helpers
{
    public static class ReportDataHelper
    {
        public static IList<IAssetValuationData?> GetValuationDataCollection(this WriteOffReportData reportData)
        {

            var valuationData = new List<IAssetValuationData?>();

            if (reportData.ValuationData != null)
            {
                valuationData.AddRange(reportData.ValuationData);
            }

            if (reportData.Dismantlings != null)
            {
                valuationData.AddRange(reportData.Dismantlings);
            }

            return valuationData;
        }
    }
}
