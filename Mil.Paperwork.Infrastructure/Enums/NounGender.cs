using System.ComponentModel;

namespace Mil.Paperwork.Infrastructure.Enums
{
    public enum NounGender
    {
        [Description("Чоловічий")]
        Masculine,
        [Description("Жіночий")]
        Feminine,
        [Description("Середній")]
        Neuter,
    }
}
