using Mil.Paperwork.Domain.DataModels.Assets;

namespace Mil.Paperwork.Domain.DataModels.Parameters
{
    public interface IWriteOffPackageParameters
    {
        int OrdenNumber { get; set; }
        DateTime OrdenDate { get; set; }
        DateTime EventDate { get; set; }
        decimal TotalWriteOffSum { get; set; }
        IBookExtractData BookOfLossesExtractData { get; set; }

        IList<IAssetInfo> Assets { get; set; }
    }
}
