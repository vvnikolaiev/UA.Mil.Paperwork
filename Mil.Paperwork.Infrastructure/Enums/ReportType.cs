using System.ComponentModel;

namespace Mil.Paperwork.Infrastructure.Enums
{
    public enum ReportType
    {
        [Description("Загальна інформація")]
        Common = 0,
        [Description("Акт якісного стану")]
        QualityStateReport = 1,
        [Description("Акт тех. стану (№7)")]
        TechnicalStateReport = 2,
        [Description("Акт списання")]
        WriteOffAct = 3,
        [Description("Відомість залишкової вартості")]
        ResidualValueReport = 4,
        [Description("Акт оцінки")]
        AssetValuationReport = 5,
        [Description("Розукомплектування")]
        AssetDismantlingReport = 6,
        [Description("Накладна (вимога)")]
        Invoice = 7,
        [Description("Акт введення в експлуатацію")]
        CommissioningAct = 8,
        [Description("Акт прийому-передачі ОЗ")]
        Handover23Act = 9,
        [Description("Пакет зі списання")]
        WriteOffPackage = 10,
        [Description("Наказ про списання")]
        WriteOffOrder = 11
    }
}
