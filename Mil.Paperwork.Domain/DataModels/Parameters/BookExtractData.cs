namespace Mil.Paperwork.Domain.DataModels.Parameters
{
    public struct BookExtractData : IBookExtractData
    {
        public int Year { get; set; }

        public int Number { get; set; }

        public int PageNumber { get; set; }

        public DateTime RecordDate { get; set; }
    }
}
