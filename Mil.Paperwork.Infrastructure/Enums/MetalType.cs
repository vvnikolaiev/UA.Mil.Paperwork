using System.ComponentModel;

namespace Mil.Paperwork.Infrastructure.Enums
{
    public enum MetalType
    {
        [Description("Золото")]
        XAU, // Gold
        [Description("Срібло")]
        XAG, // Silver
        [Description("Платина")]
        XPT, // Platinum
        [Description("Паладій")]
        XPD,  // Palladium
        [Description("Алюміній")]
        ALU, // Aluminum
        [Description("Мідь")]
        CU, // Copper
        [Description("Сталь")]
        BLACK, // Black metal
    }
}
