using Mil.Paperwork.Domain.Enums;
using Mil.Paperwork.Infrastructure.DataModels;
using Mil.Paperwork.Infrastructure.Enums;

namespace Mil.Paperwork.Domain.DataModels.Assets
{
    public interface IAssetInfo : IProductData
    {
        string SerialNumber { get; set; }
        int InitialCategory { get; set; }
        int Count { get; set; }

        string TSRegisterNumber { get; set; }
        string TSDocumentNumber { get; set; }

        EventType EventType { get; set; }

        AssetType Service { get; }
    }
}
