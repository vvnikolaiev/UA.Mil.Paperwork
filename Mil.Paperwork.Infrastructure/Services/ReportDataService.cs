using Mil.Paperwork.Infrastructure.DataModels;
using Mil.Paperwork.Infrastructure.Enums;

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

        public void SaveReportConfig(IReadOnlyCollection<ReportParameter> parameters, ReportType reportType)
        {
            if (_config == null)
                return;

            switch (reportType)
            {
                case ReportType.QualityStateReport:
                    _config.QualityStateReport = [.. parameters];
                    break;
                case ReportType.TechnicalStateReport:
                    _config.TechnicalStateReport = [.. parameters];
                    break;
                case ReportType.ResidualValueReport:
                    _config.ResidualValueReport = [.. parameters];
                    break;
                case ReportType.AssetValuationReport:
                    _config.AssetValuationReport = [.. parameters];
                    break;
                case ReportType.AssetDismantlingReport:
                    _config.AssetDismantlingReport = [.. parameters];
                    break;
                default:
                    return;
            }

            Save();
        }

        public List<ReportParameter> GetReportParameters(ReportType reportType)
        {
            var config = _config;
            if (config == null)
            {
                return [];
            }

            var result = reportType switch
            {
                ReportType.QualityStateReport => config.QualityStateReport,
                ReportType.TechnicalStateReport => config.TechnicalStateReport,
                ReportType.ResidualValueReport => config.ResidualValueReport,
                ReportType.AssetValuationReport => config.AssetValuationReport,
                ReportType.AssetDismantlingReport => config.AssetDismantlingReport,
                _ => []
            };

            return [.. result];
        }


        public Dictionary<string, string> GetReportParametersDictionary(ReportType reportType)
        {
            var list = GetReportParameters(reportType);
            var result = list.ToDictionary(x => x.Name, y => y.Value);

            return result;
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
                Save();
            }
        }

        private void Save()
        {
            _fileStorageService.WriteJsonToFile(_config, ReportDataConfigFileName);
        }
    }
}
