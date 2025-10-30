using Mil.Paperwork.Domain.DataModels.Assets;
using Mil.Paperwork.Domain.DataModels.Parameters;
using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.WriteOff.Factories;
using Mil.Paperwork.WriteOff.Managers;

namespace Mil.Paperwork.WriteOff.ViewModels.Reports
{
    internal class AssetTechnicalStateViewModel : AssetInitialTechnicalStateViewModel
    {
        private readonly ReportManager _reportManager;

        private DateTime _documentDate = DateTime.Now.Date;
        private DateTime _eventDate = DateTime.Now.Date;
        private int _ordenNumber = 0;
        private DateTime _ordenDate = DateTime.Now.Date;
        private string _reason = string.Empty;
        private bool _generateWriteOffActs = true;
        private bool _generateWriteOffPackage = true;

        private int _bookOfLossesYear = DateTime.Now.Year;
        private int _bookOfLossesNumber;
        private int _bookOfLossesPage;
        private DateTime _bookOfLossesExtractDate = DateTime.Now.Date;

        public override string Header => "Тех. стан (№11)";

        public string Reason
        {
            get => _reason;
            set => SetProperty(ref _reason, value);
        }

        public DateTime DocumentDate
        {
            get => _documentDate;
            set => SetProperty(ref _documentDate, value);
        }

        public DateTime EventDate
        {
            get => _eventDate;
            set => SetProperty(ref _eventDate, value);
        }

        public int OrdenNumber
        {
            get => _ordenNumber;
            set => SetProperty(ref _ordenNumber, value);
        }

        public DateTime OrdenDate
        {
            get => _ordenDate;
            set => SetProperty(ref _ordenDate, value);
        }

        public int BookOfLossesYear
        {
            get => _bookOfLossesYear;
            set => SetProperty(ref _bookOfLossesYear, value);
        }

        public int BookOfLossesNumber
        {
            get => _bookOfLossesNumber;
            set => SetProperty(ref _bookOfLossesNumber, value);
        }

        public int BookOfLossesPage
        {
            get => _bookOfLossesPage;
            set => SetProperty(ref _bookOfLossesPage, value);
        }

        public DateTime BookOfLossesExtractDate
        {
            get => _bookOfLossesExtractDate;
            set => SetProperty(ref _bookOfLossesExtractDate, value);
        }

        public bool GenerateWriteOffActs
        {
            get => _generateWriteOffActs;
            set => SetProperty(ref _generateWriteOffActs, value);
        }

        public bool GenerateWriteOffPackage
        {
            get => _generateWriteOffPackage;
            set => SetProperty(ref _generateWriteOffPackage, value);
        }

        public AssetTechnicalStateViewModel(ReportManager reportManager, IAssetFactory assetFactory, IDataService dataService, IReportDataService reportDataService) 
            : base(reportManager, assetFactory, dataService, reportDataService)
        {
            _reportManager = reportManager;
        }

        protected override void GenerateReport(IEnumerable<IAssetInfo> assets, string destinationFolder)
        {
            BookExtractData? extract = null;

            if (GenerateWriteOffPackage)
            {
                extract = new BookExtractData
                {
                    Year = BookOfLossesYear,
                    Number = BookOfLossesNumber,
                    PageNumber = BookOfLossesPage,
                    RecordDate = BookOfLossesExtractDate
                };
            }

            var reportData = new TechnicalStateReportData
            {
                DocumentDate = _documentDate,
                Reason = _reason,
                EventDate = _eventDate,
                EventType = EventType,
                Assets = [.. assets],
                DestinationFolder = destinationFolder,
                OrdenNumber = _ordenNumber,
                OrdenDate = _ordenDate,
                GenerateWriteOffActs = _generateWriteOffActs,
                BookOfLossesExtractData = extract
            };


            _reportManager.GenerateTechnicalStateReport(reportData);
        }
    }
}
