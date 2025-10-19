namespace Mil.Paperwork.Infrastructure.DataModels
{
    public interface IPerson
    {
        string FirstName { get; set; }
        string LastName { get; set; }
        string Patronymic { get; set; }
        string FullName { get; }
        string Position { get; set; }
        string Rank { get; set; }
    }
}