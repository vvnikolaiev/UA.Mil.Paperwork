using System.ComponentModel;

namespace Mil.Paperwork.Infrastructure.Enums
{
    public enum ReportType
    {
        [Description("Загальна інформація")]
        Common,
        [Description("Акт якісного стану")]
        QualityStateReport,
        [Description("Акт технічного стану")]
        TechnicalStateReport,
        [Description("Акт списання")]
        WriteOffAct,
        [Description("Відомість залишкової вартості")]
        ResidualValueReport,
        [Description("Акт оцінки")]
        AssetValuationReport, 
        [Description("Акт зміни якісного (технічного) стану (розкомплектація)")]
        AssetDismantlingReport,
        [Description("Накладна (вимога)")]
        Invoice,
        [Description("Акт введення в експлуатацію")]
        CommissioningAct,
        [Description("Пакет документів для списання")]
        WriteOffPackage
    }
}
