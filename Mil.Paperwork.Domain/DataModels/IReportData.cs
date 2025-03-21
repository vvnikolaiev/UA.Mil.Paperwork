namespace Mil.Paperwork.Domain.DataModels
{
    public interface IReportData
    {
        string DestinationFolder { get; }

        string GetDestinationPath();
    }
}
