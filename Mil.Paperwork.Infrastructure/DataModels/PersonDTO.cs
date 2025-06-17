namespace Mil.Paperwork.Infrastructure.DataModels
{
    public class PersonDTO : IPerson
    {
        public string FullName { get; set; }
        public string Position { get; set; }
        public string Rank { get; set; }
        public PersonDTO(string fullName, string position, string rank) : this()
        {
            FullName = fullName;
            Position = position;
            Rank = rank;
        }
        public PersonDTO() { }
    }
}