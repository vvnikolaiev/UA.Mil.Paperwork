using Mil.Paperwork.DataAccess.DataModels.Snapshots;
using Mil.Paperwork.Infrastructure.DataModels;

namespace Mil.Paperwork.DataAccess.Mappers
{
    internal static class PersonSnapshotMapper
    {
        public static PersonSnapshot? ToSnapshot(IPerson? person)
        {
            if (person == null)
            {
                return null;
            }

            var snapshot = new PersonSnapshot
            {
                FirstName = person.FirstName,
                LastName = person.LastName,
                Patronymic = person.Patronymic,
                Position = person.Position,
                Rank = person.Rank
            };

            return snapshot;
        }

        public static PersonDTO? ToPerson(PersonSnapshot? snapshot)
        {
            if (snapshot == null)
            {
                return null;
            }

            var person = new PersonDTO
            {
                FirstName = snapshot.FirstName,
                LastName = snapshot.LastName,
                Patronymic = snapshot.Patronymic,
                Position = snapshot.Position,
                Rank = snapshot.Rank
            };

            return person;
        }
    }
}
