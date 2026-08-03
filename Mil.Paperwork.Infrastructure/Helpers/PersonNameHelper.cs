namespace Mil.Paperwork.Infrastructure.Helpers
{
    public static class PersonNameHelper
    {
        public static (string FirstName, string LastName, string Patronymic) ParseFullName(string? fullName)
        {
            var name = fullName?.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (name == null || name.Length == 0)
            {
                return (string.Empty, string.Empty, string.Empty);
            }

            // Ігор ПЕТРЕНКО
            // ПЕТРЕНКО Ігор Володимирович
            // Петренко
            var lastName = name.Length == 1 || name.Length == 3 ? name[0] : name[1];
            var firstName = name.Length == 2 ? name[0] : name.Length == 3 ? name[1] : string.Empty;
            var patronymic = name.Length == 3 ? name[2] : string.Empty;

            return (firstName, lastName, patronymic);
        }

        public static string FormatFullName(string? lastName, string? firstName, string? patronymic)
        {
            var parts = new[] { lastName, firstName, patronymic }.Where(p => !string.IsNullOrWhiteSpace(p));
            var result = string.Join(" ", parts);
            return result;
        }
    }
}
