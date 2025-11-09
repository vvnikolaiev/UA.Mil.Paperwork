namespace Mil.Paperwork.Infrastructure.DataModels.Configuration
{
    public class CommisionsConfigSection : ICommisionsConfigSection
    {
        public string HeadOfCommissionPattern { get; set; }
        public string CommissionPersonPattern { get; set; }

        public CommissionDTO TechnicalStateCommission { get; set; }
        public CommissionDTO WriteOffCommission { get; set; }
    }
}
