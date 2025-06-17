namespace Mil.Paperwork.Domain.DataModels
{
    public class Person : IPerson
    {
        public string FullName { get; set; }
        public string Position { get; set; }
        public string Rank { get; set; }
        public Person(string fullName, string position, string rank) : this()
        {
            FullName = fullName;
            Position = position;
            Rank = rank;
        }
        public Person() { }
    }
}