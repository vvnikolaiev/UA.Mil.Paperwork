using System.ComponentModel;

namespace Mil.Paperwork.Infrastructure.Enums
{
    public enum AssetType
    {
        [Description("Невизначений")]
        Default = 0,
        [Description("Зв'язок")]
        Connectivity,
        [Description("РХБЗ")]
        Radiochemical,
    }
}
