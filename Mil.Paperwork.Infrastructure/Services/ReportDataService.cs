using Mil.Paperwork.Infrastructure.DataModels;
using Mil.Paperwork.Infrastructure.DataModels.Configuration;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Helpers;
using System.IO;
using System.Text;

namespace Mil.Paperwork.Infrastructure.Services
{
    internal class ReportDataService : IReportDataService
    {
        private readonly IFileStorageService _fileStorageService;
        private readonly ReportDataConfigDTO? _config;
        private ReportDataConfigDTO? _tempConfig;

        public ReportDataService(IFileStorageService fileStorageService)
        {
            _fileStorageService = fileStorageService;

            _config = _fileStorageService.ReadJsonFile<ReportDataConfigDTO>(LocalDataPaths.ReportDataConfig);
            ReloadTempConfig();
        }

        public void SaveReportConfigExternally(string directoryPath)
        {
            var json = JsonHelper.WriteJson(_config);
            var configBytes = Encoding.UTF8.GetBytes(json);
            var path = Path.Combine(directoryPath, Path.GetFileName(LocalDataPaths.ReportDataConfig));
            _fileStorageService.SaveFile(path, configBytes);
        }

        public void SaveReportConfigTemprorary(CommissionDTO commissionDTO, CommissionType commissionType)
        {
            if (_tempConfig != null)
            {
                UpdateCommissionData(commissionDTO, commissionType, _tempConfig);
            }
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

        public void SaveCommission(CommissionDTO configCommission, CommissionType commissionType)
        {
            if (_config == null || _tempConfig == null)
            {
                return;
            }

            SaveReportConfigTemprorary(configCommission, commissionType);
            UpdateCommissionData(configCommission, commissionType, _config);

            Save();
        }

        public void SaveCommissionTemporary(CommissionDTO configCommission, CommissionType commissionType)
        {
            if (_tempConfig != null)
            {
                UpdateCommissionData(configCommission, commissionType, _tempConfig);
            }
        }

        public void SaveServiceData(string key, MilitaryServiceDTO serviceDTO, bool temporary = false)
        {
            if (_tempConfig == null)
            {
                return;
            }

            AlterMilServiceData(key, serviceDTO, _tempConfig);
            
            if (!temporary && _config != null)
            {
                AlterMilServiceData(key, serviceDTO, _config);
                Save();
            }
        }

        public void DeleteServiceData(string key)
        {
            if (_config != null)
            {
                _config.Common.MilServicesData.Remove(key);
                
                if (_config.Common.ServiceKey == key)
                {
                    var newDefault = _config.Common.MilServicesData.FirstOrDefault().Key ?? string.Empty;
                    _config.Common.ServiceKey = newDefault;
                }

                Save();
                ReloadTempConfig();
            }
        }

        public void SetDefaultService(string key)
        {
            if (_config != null)
            {
                if (!_config.Common.MilServicesData.ContainsKey(key))
                {
                    return;
                }

                _config.Common.ServiceKey = key;

                Save();
                ReloadTempConfig();
            }
        }

        public List<ReportParameter> GetReportParameters(ReportType reportType, bool withReload = false)
        {
            var config = GetConfig(withReload);

            if (config == null)
            {
                return [];
            }

            var result = reportType switch
            {
                ReportType.Common => config.Common.MilitaryUnitData,
                ReportType.QualityStateReport => config.QualityStateReport,
                ReportType.TechnicalStateReport => config.TechnicalStateReport,
                ReportType.WriteOffAct => config.TechnicalStateReport,
                ReportType.ResidualValueReport => config.ResidualValueReport,
                ReportType.AssetValuationReport => config.AssetValuationReport,
                ReportType.AssetDismantlingReport => config.AssetDismantlingReport,
                ReportType.CommissioningAct => config.CommissioningAct,
                ReportType.Invoice => config.Invoice,
                ReportType.WriteOffPackage => config.WriteOffPackage,
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

        public string GetSelectedService(bool withReload = false)
        {
            var config = GetConfig(withReload);

            var selectedService = config?.Common.ServiceKey ?? string.Empty;

            return selectedService;
        }

        public Dictionary<string, MilitaryServiceDTO> GetAllServices(bool withReload = false)
        {
            var config = GetConfig(withReload);

            if (config == null)
            {
                return [];
            }
            
            var services = config.Common.MilServicesData.ToDictionary(x => x.Key, y => y.Value);

            return services;
        }


        public Dictionary<string, string> GetServiceReportParametersDictionary(string serviceKey)
        {
            var list = GetServiceReportParameters(serviceKey);
            var result = list.ToDictionary(x => x.Name, y => y.Value);

            return result;
        }

        public List<ReportParameter> GetServiceReportParameters(string serviceKey, bool withReload = false)
        {
            var config = GetConfig(withReload);

            if (config == null)
            {
                return [];
            }

            var sService = serviceKey ?? config.Common.ServiceKey;

            var dataExists = config.Common.MilServicesData.TryGetValue(sService, out var militaryServiceDTO);

            var result = dataExists ? militaryServiceDTO.GetAllParameters() ?? [] : [];

            return [.. result];
        }

        public CommissionDTO GetCommissionData(CommissionType commissionType, bool withReload = false)
        {
            var config = GetConfig(withReload);

            var defaultCommission = new CommissionDTO();
            if (config == null)
            {
                return defaultCommission;
            }

            var result = commissionType switch
            {
                CommissionType.WriteOffCommission => config.Common.CommisionsConfig.WriteOffCommission,
                CommissionType.TechnicalStateCommission => config.Common.CommisionsConfig.TechnicalStateCommission,
                _ => defaultCommission
            };

            return result;
        }

        public AssetType GetAssetType()
        {
            var result = AssetType.Default;

            var defaultServiceKey = _config?.Common.ServiceKey ?? string.Empty;
            var milServiceData = _config?.Common.MilServicesData;

            if (milServiceData != null && milServiceData.TryGetValue(defaultServiceKey, out var militaryServiceDTO))
            {
                var sAssetType = militaryServiceDTO.AssetTypes?.FirstOrDefault() ?? string.Empty;
                if (EnumHelper.TryGetEnumValue(sAssetType, out AssetType assetType))
                {
                    result = assetType;
                }
            }

            return result;
        }
        
        public ICommisionsConfigSection GetCommissionsConfig()
        {
            var config = GetConfig(false);

            var commConfig = config?.Common.CommisionsConfig;

            return commConfig;
        }

        public CommissionDTO GetCommission(ReportType reportType)
        {
            var config = GetConfig(false);

            var dummyCommission = new CommissionDTO();

            if (config?.Common?.CommisionsConfig == null)
            {
                return dummyCommission;
            }

            var commisions = config.Common.CommisionsConfig;

            var result = reportType switch
            {
                ReportType.QualityStateReport => commisions.TechnicalStateCommission,
                ReportType.TechnicalStateReport => commisions.TechnicalStateCommission,
                ReportType.WriteOffAct => commisions.WriteOffCommission,
                ReportType.ResidualValueReport => commisions.WriteOffCommission,
                ReportType.AssetValuationReport => commisions.TechnicalStateCommission,
                ReportType.AssetDismantlingReport => commisions.TechnicalStateCommission,
                ReportType.CommissioningAct => commisions.TechnicalStateCommission,
                ReportType.WriteOffPackage => commisions.WriteOffCommission,
                _ => dummyCommission
            };

            return result;
        }

        public bool ImportReportConfig(ReportDataConfigDTO reportDataConfigDTO)
        {
            var result = true;
            try
            {
                if (reportDataConfigDTO != null)
                {
                    _fileStorageService.WriteJsonToFile(reportDataConfigDTO, LocalDataPaths.ReportDataConfig);
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            catch (Exception ex)
            {
                result = false;
            }

            return result;

        }

        private ReportDataConfigDTO? GetConfig(bool withReload)
        {
            if (withReload)
            {
                ReloadTempConfig();
            }

            var config = _tempConfig;

            return config;
        }

        private void ReloadTempConfig()
        {
            _tempConfig = _fileStorageService.ReadJsonFile<ReportDataConfigDTO>(LocalDataPaths.ReportDataConfig);
        }

        private static void UpdateConfigReportTypeData(IReadOnlyCollection<ReportParameter> parameters, ReportType reportType, ReportDataConfigDTO reportDataConfigDTO)
        {
            if (reportDataConfigDTO == null)
                return;

            switch (reportType)
            {
                case ReportType.Common:
                    reportDataConfigDTO.Common.MilitaryUnitData = [.. parameters];
                    break;
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
                case ReportType.Invoice:
                    reportDataConfigDTO.Invoice = [.. parameters];
                    break;
                case ReportType.WriteOffPackage:
                    reportDataConfigDTO.WriteOffPackage = [.. parameters];
                    break;
                default:
                    return;
            }
        }

        private static void UpdateCommissionData(CommissionDTO commissionDTO, CommissionType commissionType, ReportDataConfigDTO reportDataConfigDTO)
        {
            if (reportDataConfigDTO == null)
                return;

            switch (commissionType)
            {
                case CommissionType.WriteOffCommission:
                    reportDataConfigDTO.Common.CommisionsConfig.WriteOffCommission = commissionDTO;
                    break;
                case CommissionType.TechnicalStateCommission:
                    reportDataConfigDTO.Common.CommisionsConfig.TechnicalStateCommission = commissionDTO;
                    break;
                default: 
                    return;
            }
        }

        private void AlterMilServiceData(string key, MilitaryServiceDTO serviceDTO, ReportDataConfigDTO reportDataConfig)
        {
            if (reportDataConfig != null)
            {
                reportDataConfig.Common.MilServicesData[key] = serviceDTO;
            }
        }

        private void Save()
        {
            _fileStorageService.WriteJsonToFile(_config, LocalDataPaths.ReportDataConfig);
        }
    }
}
