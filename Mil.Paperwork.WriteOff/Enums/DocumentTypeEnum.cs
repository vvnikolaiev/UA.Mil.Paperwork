using System.ComponentModel;

namespace Mil.Paperwork.WriteOff.Enums
{
    internal enum DocumentTypeEnum
    {
        [Description("Списання майна")]
        WriteOff,
        [Description("Залишкова вартість")]
        ResidualValue,
        [Description("Акт тех. стану (№11)")]
        TechnicalState11,
        [Description("Акт тех. стану (№7)")]
        TechnicalState7,
        [Description("Акт оцінки")]
        Valuation,
        [Description("Розукомплектування")]
        Dismantling,
        [Description("Накладна (вимога)")]
        Invoice,
        [Description("Акт введення в експлуатацію")]
        CommisioningAct,
    }
}
