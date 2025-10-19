using Mil.Paperwork.Infrastructure.Attributes;
using Mil.Paperwork.Infrastructure.Enums;

namespace Mil.Paperwork.Infrastructure.DataModels
{

    public class ReportDataConfigDTO
    {
        public CommonConfigSection Common { get; set; }
        
        [ReportTypeMapping(ReportType.QualityStateReport)]
        public List<ReportParameter> QualityStateReport { get; set; }

        [ReportTypeMapping(ReportType.TechnicalStateReport)]
        public List<ReportParameter> TechnicalStateReport { get; set; }

        [ReportTypeMapping(ReportType.ResidualValueReport)]
        public List<ReportParameter> ResidualValueReport { get; set; }
        
        [ReportTypeMapping(ReportType.AssetValuationReport)]
        public List<ReportParameter> AssetValuationReport { get; set; }
        
        [ReportTypeMapping(ReportType.AssetDismantlingReport)]
        public List<ReportParameter> AssetDismantlingReport { get; set; }
        
        [ReportTypeMapping(ReportType.CommissioningAct)]
        public List<ReportParameter> CommissioningAct { get; set; }

        [ReportTypeMapping(ReportType.Invoice)]
        public List<ReportParameter> Invoice { get; set; }

        [ReportTypeMapping(ReportType.WriteOffPackage)]
        public List<ReportParameter> WriteOffPackage { get; set; }
    }
}
