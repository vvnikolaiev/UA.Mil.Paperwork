using Mil.Paperwork.Infrastructure.DataModels;
using Mil.Paperwork.Infrastructure.DataModels.Configuration;
using Mil.Paperwork.Infrastructure.Enums;

namespace Mil.Paperwork.Infrastructure.Services
{
    public interface IReportDataService
    {
        void SaveReportConfigExternally(string directoryPath);
        void SaveReportConfig(IReadOnlyCollection<ReportParameter> parameters, ReportType reportType);
        void SaveReportConfigTemprorary(IReadOnlyCollection<ReportParameter> parameters, ReportType reportType);

        void SaveCommission(CommissionDTO configCommission, CommissionType commissionType);
        void SaveCommissionTemporary(CommissionDTO configCommission, CommissionType commissionType);

        void SaveServiceData(string key, MilitaryServiceDTO serviceDTO, bool temporary = false);
        void DeleteServiceData(string key);
        void SetDefaultService(string key);

        Dictionary<string, string> GetReportParametersDictionary(ReportType reportType);

        List<ReportParameter> GetReportParameters(ReportType reportType, bool withReload = false);

        Dictionary<string, string> GetServiceReportParametersDictionary(string serviceKey = null);

        List<ReportParameter> GetServiceReportParameters(string serviceKey = null, bool withReload = false);

        Dictionary<string, MilitaryServiceDTO> GetAllServices(bool withReload = false);

        string GetSelectedService(bool withReload = false);

        CommissionDTO GetCommissionData(CommissionType coimmissionType, bool withReload = false);

        AssetType GetAssetType();

        ICommisionsConfigSection GetCommissionsConfig();

        CommissionDTO GetCommission(ReportType reportType);

        bool ImportReportConfig(ReportDataConfigDTO reportDataConfigDTO);

    }
}
