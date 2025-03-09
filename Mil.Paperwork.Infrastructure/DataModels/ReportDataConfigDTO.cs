namespace Mil.Paperwork.Infrastructure.DataModels
{
    public class ReportDataConfigDTO
    {
        public Dictionary<string, string> QualityStateReport { get; set; }
        public Dictionary<string, string> TechnicalStateReport { get; set; }
        public Dictionary<string, string> ResidualValueReport { get; set; }
    }
}
