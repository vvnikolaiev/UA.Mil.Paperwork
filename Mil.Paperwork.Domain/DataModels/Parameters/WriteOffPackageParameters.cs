using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Domain.Helpers;

namespace Mil.Paperwork.Domain.DataModels.Parameters
{
    internal struct WriteOffPackageParameters : IWriteOffPackageParameters
    {
        public int OrdenNumber { get; set; }
        public DateTime OrdenDate { get; set; }
        public IBookExtractData BookOfLossesExtractData { get; set; }
        public decimal TotalWriteOffSum { get; set; }

        public static WriteOffPackageParameters FromReportData(ITechnicalStateReportData reportData)
        {
            return new WriteOffPackageParameters
            {
                OrdenNumber = reportData.OrdenNumber,
                OrdenDate = reportData.OrdenDate,
                BookOfLossesExtractData = reportData.BookOfLossesExtractData,
                TotalWriteOffSum = ResidualPriceHelper.CalculateTotalReportSum(reportData.Assets, reportData.EventDate, true)
            };
        }
    }
}
