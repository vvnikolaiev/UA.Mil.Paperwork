using Mil.Paperwork.Infrastructure.DataModels;
using Mil.Paperwork.Infrastructure.Enums;
using System.Security.Permissions;

namespace Mil.Paperwork.Infrastructure.Services
{
    internal class ReportDataService : IReportDataService
    {
        private const string ReportDataConfigFileName = "Data/ReportDataConfig.json";
        
        private readonly IFileStorageService _fileStorageService;
        private readonly ReportDataConfigDTO? _config;

        public ReportDataService(IFileStorageService fileStorageService)
        {
            _fileStorageService = fileStorageService;
            _config = _fileStorageService.ReadJsonFile<ReportDataConfigDTO>(ReportDataConfigFileName);
        }

        public Dictionary<string, string> GetReportConfig(ReportType reportType)
        {
            var config = _config;
            var emptyDict = new Dictionary<string, string>();
            if (config == null)
            {
                return emptyDict;
            } 

            var fieldsMap = reportType switch
            {
                ReportType.QualityStateReport => config.QualityStateReport.ToDictionary(),
                ReportType.TechnicalStateReport => config.TechnicalStateReport.ToDictionary(),
                ReportType.ResidualValueReport => config.ResidualValueReport.ToDictionary(),
                ReportType.AssetValuationReport => config.AssetValuationReport.ToDictionary(),
                ReportType.AssetDismantlingReport => config.AssetDismantlingReport.ToDictionary(),
                _ => new Dictionary<string, string>()
            };

            return fieldsMap;
        }

        public AssetType GetAssetType()
        {
            if (Enum.TryParse<AssetType>(_config?.Common.AssetType, true, out var assetType))
            {
                return assetType;
            }
            else
            {
                return AssetType.Default;
            }
        }

        public void SetAssetType(AssetType assetType)
        {
            if (_config != null)
            {
                _config.Common.AssetType = assetType.ToString();
                _fileStorageService.WriteJsonToFile(_config, ReportDataConfigFileName);
            }
        }
    }
}
