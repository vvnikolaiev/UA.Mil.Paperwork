namespace Mil.Paperwork.Infrastructure.DataModels
{
    public class MilServiceEntry
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string NominativeName { get; set; } = string.Empty;
        public string GenitiveName { get; set; } = string.Empty;
    }
}
