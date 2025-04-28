namespace Mil.Paperwork.Domain.DataModels
{
    public interface ITechnicalStateReportData : IReportData
    {
        string Reason { get; }

        DateTime ReportDate { get; }

        IList<IAssetInfo> Assets { get; set; }
    }
}
