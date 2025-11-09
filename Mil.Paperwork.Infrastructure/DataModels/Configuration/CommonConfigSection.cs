namespace Mil.Paperwork.Infrastructure.DataModels.Configuration
{
    public class CommonConfigSection
    {
        public string AssetType { get; set; }

        public List<ReportParameter> MilitaryUnitData { get; set; }

        public CommisionsConfigSection CommisionsConfig { get; set; }
    }
}
