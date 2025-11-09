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

        Dictionary<string, string> GetReportParametersDictionary(ReportType reportType);

        List<ReportParameter> GetReportParameters(ReportType reportType, bool withReload = false);

        CommissionDTO GetCommissionData(CommissionType coimmissionType, bool withReload = false);

        AssetType GetAssetType();

        void SetAssetType(AssetType assetType);

        ICommisionsConfigSection GetCommissionsConfig();

        CommissionDTO GetCommission(ReportType reportType);

        bool ImportReportConfig(ReportDataConfigDTO reportDataConfigDTO);

    }
}
