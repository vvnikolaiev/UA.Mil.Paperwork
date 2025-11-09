using System.ComponentModel;

namespace Mil.Paperwork.Infrastructure.Enums
{
    public enum CommissionType
    {
        [Description("Комісія з технічного стану")]
        TechnicalStateCommission,
        [Description("Комісія зі списання")]
        WriteOffCommission,
    }
}
