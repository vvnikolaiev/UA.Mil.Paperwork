using Mil.Paperwork.Infrastructure.Attributes;

namespace Mil.Paperwork.Infrastructure.DataModels
{
    public class PersonDTO : IPerson
    {
        [ImportColumn("Ім'я", isReqired: true)]
        public string FullName { get; set; }

        [ImportColumn("Посада", isReqired: false)]
        public string Position { get; set; }

        [ImportColumn("Звання", isReqired: false)]
        public string Rank { get; set; }

        public PersonDTO(string fullName, string position, string rank) : this()
        {
            FullName = fullName;
            Position = position;
            Rank = rank;
        }

        public PersonDTO()
        {
            FullName = string.Empty;
            Position = string.Empty;
            Rank = string.Empty;
        }
    }
}