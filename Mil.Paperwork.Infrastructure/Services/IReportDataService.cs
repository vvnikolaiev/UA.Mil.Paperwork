using Mil.Paperwork.Infrastructure.Enums;

namespace Mil.Paperwork.Infrastructure.Services
{
    public interface IReportDataService
    {
        Dictionary<string, string> GetReportConfig(ReportType reportType);

        AssetType GetAssetType();

        void SetAssetType(AssetType assetType);
    }
}
