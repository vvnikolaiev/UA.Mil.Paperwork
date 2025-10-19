using Mil.Paperwork.Infrastructure.Attributes;

namespace Mil.Paperwork.Infrastructure.DataModels
{
    public class PersonDTO : IPerson
    {
        [ImportColumn("Ім'я", isReqired: true)]
        public string FirstName { get; set; }

        [ImportColumn("Прізвище", isReqired: true)]
        public string LastName { get; set; }

        [ImportColumn("По-батькові")]
        public string Patronymic { get; set; }

        public string FullName => $"{FirstName} {LastName?.ToUpper()}";

        [ImportColumn("Посада", isReqired: false)]
        public string Position { get; set; }

        [ImportColumn("Звання", isReqired: false)]
        public string Rank { get; set; }

        public PersonDTO(string fullName, string position, string rank) : this()
        {
            var name = fullName?.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (name != null && name.Length > 0)
            {
                // Ігор ПЕТРЕНКО
                // ПЕТРЕНКО Ігор Володимирович
                // Петренко
                LastName = name.Length == 1 || name.Length == 3 ? name[0] : name[1];
                FirstName = name.Length == 2 ? name[0] : name.Length == 3 ? name[1] : string.Empty;
                Patronymic = name.Length == 3 ? name[1] : string.Empty;
            }
            else
            {
                FirstName = string.Empty;
                LastName = string.Empty;
                Patronymic = string.Empty;
            }

            Position = position;
            Rank = rank;
        }
        public PersonDTO(string firstName, string lastName, string position, string rank) : this()
        {
            FirstName = firstName;
            LastName = lastName;
            Position = position;
            Rank = rank;
        }

        public PersonDTO()
        {
            FirstName = string.Empty;
            LastName = string.Empty;
            Patronymic = string.Empty;
            Position = string.Empty;
            Rank = string.Empty;
        }
    }
}