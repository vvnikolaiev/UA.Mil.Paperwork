using Mil.Paperwork.Domain.DataModels;
using Mil.Paperwork.WriteOff.Managers;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.WriteOff.Factories;
using Mil.Paperwork.Domain.DataModels.Assets;

namespace Mil.Paperwork.WriteOff.ViewModels
{
    internal class AssetTechnicalStateViewModel : AssetInitialTechnicalStateViewModel
    {
        private readonly ReportManager _reportManager;

        private DateTime _reportDate = DateTime.Now.Date;
        private string _reason = string.Empty;

        public override string Header => "Тех. стан (№11)";

        public string Reason
        {
            get => _reason;
            set => SetProperty(ref _reason, value);
        }

        public DateTime ReportDate
        {
            get => _reportDate;
            set => SetProperty(ref _reportDate, value);
        }

        public AssetTechnicalStateViewModel(ReportManager reportManager, IAssetFactory assetFactory, IDataService dataService) 
            : base(reportManager, assetFactory, dataService)
        {
            _reportManager = reportManager;
        }

        protected override void GenerateReport(IEnumerable<IAssetInfo> assets, string destinationFolder)
        {
            var reportData = new TechnicalStateReportData
            {
                Reason = _reason,
                ReportDate = _reportDate,
                EventType = EventType,
                Assets = [.. assets],
                DestinationFolder = destinationFolder
            };

            _reportManager.GenerateTechnicalStateReport(reportData);
        }
    }
}
