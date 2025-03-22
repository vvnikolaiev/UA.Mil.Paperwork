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

        public Dictionary<string, string> GetReportConfig(ReportType reportType)
        {
            var emptyDict = new Dictionary<string, string>();
            if (_config == null)
            {
                return emptyDict;
            }

            var fieldsMap = reportType switch
            {
                ReportType.QualityStateReport => _config.QualityStateReport,
                ReportType.TechnicalStateReport => _config.TechnicalStateReport,
                ReportType.ResidualValueReport => _config.ResidualValueReport,
                ReportType.AssetValuationReport => _config.AssetValuationReport,
                ReportType.AssetDismantlingReport => _config.AssetDismantlingReport,
                _ => emptyDict
            };

            return fieldsMap;
        }
    }
}
