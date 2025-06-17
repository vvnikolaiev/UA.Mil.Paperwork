namespace Mil.Paperwork.Domain.DataModels.ReportData
{
    public interface IReportData
    {
        string DestinationFolder { get; }

        string GetDestinationPath();
    }
}
