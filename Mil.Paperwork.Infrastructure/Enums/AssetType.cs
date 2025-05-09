using System.ComponentModel;

namespace Mil.Paperwork.Infrastructure.Enums
{
    public enum AssetType
    {
        [Description("Зв'язок")]
        Connectivity,
        [Description("РХБЗ")]
        Radiochemical,
        [Description("???")]
        Default
    }
}
