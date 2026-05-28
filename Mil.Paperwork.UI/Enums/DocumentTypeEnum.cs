using System.ComponentModel;

namespace Mil.Paperwork.UI.Enums
{
    internal enum DocumentTypeEnum
    {
        [Description("Списання майна")]
        WriteOff,
        [Description("Залишкова вартість")]
        ResidualValue,
        [Description("Пакет зі списання")]
        WriteOffPackage,
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
        [Description("Акт прийому-передачі ОЗ")]
        HandoverCertificate23,
        //[Description("Акт прийому-передачі запасів")]
        //HandoverCertificate24
    }
}
