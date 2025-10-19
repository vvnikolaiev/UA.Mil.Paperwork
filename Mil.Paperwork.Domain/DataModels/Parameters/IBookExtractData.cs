namespace Mil.Paperwork.Domain.DataModels.Parameters
{
    public interface IBookExtractData
    {
        int Year { get; }
        int Number { get; }
        int PageNumber { get; }
        DateTime RecordDate { get; }
    }
}
