namespace Mil.Paperwork.Domain.DataModels.Parameters
{
    public interface IWriteOffPackageParameters
    {
        int OrdenNumber { get; set; }
        DateTime OrdenDate { get; set; }
        decimal TotalWriteOffSum { get; set; }
        IBookExtractData BookOfLossesExtractData { get; set; }
    }
}
