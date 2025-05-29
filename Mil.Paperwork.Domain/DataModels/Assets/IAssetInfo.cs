using Mil.Paperwork.Domain.Enums;
using Mil.Paperwork.Infrastructure.DataModels;

namespace Mil.Paperwork.Domain.DataModels.Assets
{
    public interface IAssetInfo : IProductData
    {
        string SerialNumber { get; set; }
        // initial category?
        int InitialCategory { get; set; }
        // enum state (lost, destroyed, etc.) => II, V, ... category 
        int Count { get; set; }

        string TSRegisterNumber { get; set; }
        string TSDocumentNumber { get; set; }

        DateTime? WriteOffDateTime { get; set; }

        EventType EventType { get; set; }

        decimal TotalWearCoefficient { get; }

        IList<decimal> GetCoefficients();
    }
}
