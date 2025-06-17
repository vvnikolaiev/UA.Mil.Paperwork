using Mil.Paperwork.Infrastructure.DataModels;
using Mil.Paperwork.Infrastructure.Enums;

namespace Mil.Paperwork.Infrastructure.Services
{
    internal class ReportDataService : IReportDataService
    {
        private const string ReportDataConfigFileName = "Data/ReportDataConfig.json";

        private readonly IFileStorageService _fileStorageService;
        private readonly ReportDataConfigDTO? _config;
        private ReportDataConfigDTO? _tempConfig;

        public ReportDataService(IFileStorageService fileStorageService)
        {
            _fileStorageService = fileStorageService;

            _config = _fileStorageService.ReadJsonFile<ReportDataConfigDTO>(ReportDataConfigFileName);
            ReloadTempConfig();
        }

        public void SaveReportConfigTemprorary(IReadOnlyCollection<ReportParameter> parameters, ReportType reportType)
        {
            if (_tempConfig != null)
            {
                UpdateConfigReportTypeData(parameters, reportType, _tempConfig);
            }
        }

        public void SaveReportConfig(IReadOnlyCollection<ReportParameter> parameters, ReportType reportType)
        {
            if (_config == null || _tempConfig == null)
            {
                return;
            }

            SaveReportConfigTemprorary(parameters, reportType);
            UpdateConfigReportTypeData(parameters, reportType, _config);

            Save();
        }

        public List<ReportParameter> GetReportParameters(ReportType reportType, bool withReload = false)
        {
            if (withReload)
            {
                ReloadTempConfig();
            }

            var config = _tempConfig;
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
                ReportType.CommissioningAct => config.CommissioningAct,
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
                _tempConfig.Common.AssetType = _config.Common.AssetType;
                Save();
            }
        }

        private void ReloadTempConfig()
        {
            _tempConfig = _fileStorageService.ReadJsonFile<ReportDataConfigDTO>(ReportDataConfigFileName);
        }

        private static void UpdateConfigReportTypeData(IReadOnlyCollection<ReportParameter> parameters, ReportType reportType, ReportDataConfigDTO reportDataConfigDTO)
        {
            if (reportDataConfigDTO == null)
                return;

            switch (reportType)
            {
                case ReportType.QualityStateReport:
                    reportDataConfigDTO.QualityStateReport = [.. parameters];
                    break;
                case ReportType.TechnicalStateReport:
                    reportDataConfigDTO.TechnicalStateReport = [.. parameters];
                    break;
                case ReportType.ResidualValueReport:
                    reportDataConfigDTO.ResidualValueReport = [.. parameters];
                    break;
                case ReportType.AssetValuationReport:
                    reportDataConfigDTO.AssetValuationReport = [.. parameters];
                    break;
                case ReportType.AssetDismantlingReport:
                    reportDataConfigDTO.AssetDismantlingReport = [.. parameters];
                    break;
                case ReportType.CommissioningAct:
                    reportDataConfigDTO.CommissioningAct = [.. parameters];
                    break;
                default:
                    return;
            }
        }

        private void Save()
        {
            _fileStorageService.WriteJsonToFile(_config, ReportDataConfigFileName);
        }
    }
}
