using Mil.Paperwork.Infrastructure.Attributes;
using Mil.Paperwork.Infrastructure.Helpers;

namespace Mil.Paperwork.Infrastructure.DataModels
{
    public class PersonDTO : IPerson
    {
        [ImportColumn("Ім'я", isRequired: true)]
        public string FirstName { get; set; }

        [ImportColumn("Прізвище", isRequired: true)]
        public string LastName { get; set; }

        [ImportColumn("По-батькові")]
        public string Patronymic { get; set; }

        public string FullName => $"{FirstName} {LastName?.ToUpper()}";

        [ImportColumn("Посада", isRequired: false)]
        public string Position { get; set; }

        [ImportColumn("Звання", isRequired: false)]
        public string Rank { get; set; }

        public PersonDTO(string fullName, string position, string rank) : this()
        {
            var parsed = PersonNameHelper.ParseFullName(fullName);
            FirstName = parsed.FirstName;
            LastName = parsed.LastName;
            Patronymic = parsed.Patronymic;

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