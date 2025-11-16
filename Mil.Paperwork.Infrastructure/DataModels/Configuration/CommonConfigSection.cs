namespace Mil.Paperwork.Infrastructure.DataModels.Configuration
{
    public class CommonConfigSection
    {
        public string ServiceKey { get; set; }
        
        public Dictionary<string, MilitaryServiceDTO> MilServicesData { get; set; }

        public List<ReportParameter> MilitaryUnitData { get; set; }

        public CommisionsConfigSection CommisionsConfig { get; set; }
    }
}
