namespace Mil.Paperwork.Infrastructure.DataModels
{
    public interface IPerson
    {
        string FullName { get; set; }
        string Position { get; set; }
        string Rank { get; set; }
    }
}