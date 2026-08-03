using Mil.Paperwork.Domain.DataModels.Assets;
using Mil.Paperwork.Domain.DataModels.Parameters;
using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Domain.Services;
using Mil.Paperwork.DataAccess.Services;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.UI.Factories;
using Mil.Paperwork.UI.Helpers;
using Mil.Paperwork.UI.Managers;
using Mil.Paperwork.UI.ViewModels.Tabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mil.Paperwork.UI.ViewModels.Reports
{
    internal class AssetTechnicalStateViewModel : AssetInitialTechnicalStateViewModel, IReportDataLoadable<IWriteOffPackageReportData>
    {
        private readonly ReportManager _reportManager;

        private DateTime _documentDate = DateTime.Now.Date;
        private DateTime _eventDate = DateTime.Now.Date;
        private int _ordenNumber = 0;
        private DateTime _ordenDate = DateTime.Now.Date;
        private string _reason = string.Empty;
        private bool _generateWriteOffActs = true;
        private bool _generateWriteOffPackage = true;
        private bool _generateQualityStateReportInstead = false;
        private string _qsrRegNumber = string.Empty;
        private string _qsrDocNumber = string.Empty;
        private string _writeOffRegNumber = string.Empty;
        private string _writeOffDocNumber = string.Empty;

        private int _bookOfLossesYear = DateTimeOffset.Now.Year;
        private int _bookOfLossesNumber;
        private int _bookOfLossesPage;
        private DateTimeOffset _bookOfLossesExtractDate = DateTimeOffset.Now.Date;

        private const string HeaderText = "Пакет списання";

        public override string Header => OrdenNumber > 0 ? $"{HeaderText} (н. {OrdenNumber})" : HeaderText;

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
            set
            {
                if (SetProperty(ref _ordenNumber, value))
                {
                    OnPropertyChanged(nameof(Header));
                }
            }
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

        public DateTimeOffset BookOfLossesExtractDate
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

        public bool GenerateQualityStateReportInstead
        {
            get => _generateQualityStateReportInstead;
            set => SetProperty(ref _generateQualityStateReportInstead, value);
        }

        public string QSRDocNumber
        {
            get => _qsrDocNumber;
            set => SetProperty(ref _qsrDocNumber, value);
        }

        public string QSRRegNumber
        {
            get => _qsrRegNumber;
            set => SetProperty(ref _qsrRegNumber, value);
        }

        public string WriteOffDocNumber
        {
            get => _writeOffDocNumber;
            set => SetProperty(ref _writeOffDocNumber, value);
        }

        public string WriteOffRegNumber
        {
            get => _writeOffRegNumber;
            set => SetProperty(ref _writeOffRegNumber, value);
        }

        protected override ReportType HistoryReportType => ReportType.WriteOffPackage;

        public AssetTechnicalStateViewModel(
            ReportManager reportManager,
            IAssetFactory assetFactory,
            IDataService dataService,
            IReportDataService reportDataService,
            IReportHistoryService reportHistoryService,
            IDialogService dialogService)
            : base(reportManager, assetFactory, dataService, reportDataService, reportHistoryService, dialogService)
        {
            _reportManager = reportManager;
            ResumeDirtyTracking();
        }

        protected override IReportData BuildReportData()
        {
            var assets = AssetsTable.AssetsCollection.Select(x => x.ToAssetInfo(EventType)).ToArray();
            var reportData = BuildWriteOffPackageData(assets, string.Empty);

            return reportData;
        }

        protected override async Task GenerateReport(IEnumerable<IAssetInfo> assets, string destinationFolder)
        {
            var results = new List<ReportGenerationResult>();

            if (GenerateWriteOffPackage)
            {
                var packageResult = await GenerateWriteOffReports(assets, destinationFolder);
                results.Add(packageResult);
            }

            // inroduce a new parameter to IAssetInfo to mark assets for write-off, and use it here instead of checking SerialNumber
            var valuableAssets = _generateWriteOffActs ? [.. assets.Where(x => !string.IsNullOrEmpty(x.SerialNumber))] : assets;
            var writeOffAssets = _generateWriteOffActs ? [.. assets.Where(x => string.IsNullOrEmpty(x.SerialNumber))] : Array.Empty<IAssetInfo>();

            var mainResult = GenerateQualityStateReportInstead
                ? await GenerateQualityStateReport(valuableAssets, destinationFolder)
                : await GenerateTechnicalStateReport(valuableAssets, destinationFolder);
            results.Add(mainResult);

            if (_generateWriteOffActs && writeOffAssets.Any())
            {
                var writeOffActResult = await GenerateWriteOffActReport(writeOffAssets, destinationFolder);
                results.Add(writeOffActResult);
            }

            var combinedSuccess = results.All(x => x.Success);
            var combinedFiles = results.SelectMany(x => x.OutputFiles).ToList();
            var combinedResult = ReportGenerationResult.FromResult(combinedSuccess, combinedFiles);
            await RecordGeneratedAsync(combinedResult);

            ResetDirtyState();
        }

        private async Task<ReportGenerationResult> GenerateQualityStateReport(IEnumerable<IAssetInfo> assets, string destinationFolder)
        {
            var reportData = new CommonWriteOffReportData
            {
                DocumentNumber = QSRDocNumber,
                RegistrationNumber = QSRRegNumber,
                DocumentDate = _documentDate.Date,
                Reason = _reason,
                EventDate = _eventDate.Date,
                EventType = EventType,
                Assets = [.. assets],
                DestinationFolder = destinationFolder,
                OrdenNumber = _ordenNumber,
                OrdenDate = _ordenDate.Date,
            };

            var result = await RunReportAsync(TextFormatHelper.QualityStateReportName, "Помилка генерації звіту",
                () => _reportManager.GenerateQualityStateReport(reportData));
            return result;
        }

        private async Task<ReportGenerationResult> GenerateWriteOffActReport(IEnumerable<IAssetInfo> assets, string destinationFolder)
        {
            var writeOffReportData = new CommonWriteOffReportData
            {
                DocumentNumber = WriteOffDocNumber,
                RegistrationNumber = WriteOffRegNumber,
                DocumentDate = _documentDate.Date,
                Reason = _reason,
                EventDate = _eventDate.Date,
                EventType = EventType,
                Assets = [.. assets],
                DestinationFolder = destinationFolder,
                OrdenNumber = _ordenNumber,
                OrdenDate = _ordenDate.Date,
            };

            var result = await RunReportAsync(TextFormatHelper.WriteOffActReportName, "Помилка генерації звіту",
                () => _reportManager.GenerateWriteOffActReport(writeOffReportData));
            return result;
        }

        private async Task<ReportGenerationResult> GenerateTechnicalStateReport(IEnumerable<IAssetInfo> assets, string destinationFolder)
        {
            var reportData = new TechnicalStateReportData
            {
                DocumentDate = _documentDate.Date,
                Reason = _reason,
                EventDate = _eventDate.Date,
                EventType = EventType,
                Assets = [.. assets],
                DestinationFolder = destinationFolder,
                OrdenNumber = _ordenNumber,
                OrdenDate = _ordenDate.Date,
                GenerateWriteOffActs = _generateWriteOffActs
            };

            var result = await RunReportAsync(TextFormatHelper.TechnicalStateReportName, "Помилка генерації звіту",
                () => _reportManager.GenerateTechnicalStateReport(reportData));
            return result;
        }

        private WriteOffPackageReportData BuildWriteOffPackageData(IEnumerable<IAssetInfo> assets, string destinationFolder)
        {
            var extract = new BookExtractData
            {
                Year = BookOfLossesYear,
                Number = BookOfLossesNumber,
                PageNumber = BookOfLossesPage,
                RecordDate = BookOfLossesExtractDate.Date
            };

            var writeOffPackageData = new WriteOffPackageReportData
            {
                DocumentDate = _documentDate.Date,
                EventDate = _eventDate.Date,
                Assets = [.. assets],
                DestinationFolder = destinationFolder,
                OrdenNumber = _ordenNumber,
                OrdenDate = _ordenDate.Date,
                BookOfLossesExtractData = extract,
                ServiceKey = _reportDataService.GetSelectedService()
            };

            return writeOffPackageData;
        }

        public void LoadReportData(IWriteOffPackageReportData data)
        {
            WithDirtyTrackingSuspended(() =>
            {
                DocumentDate = data.DocumentDate;
                EventDate = data.EventDate;
                OrdenNumber = data.OrdenNumber;
                OrdenDate = data.OrdenDate;

                var extract = data.BookOfLossesExtractData;
                if (extract != null)
                {
                    BookOfLossesYear = extract.Year;
                    BookOfLossesNumber = extract.Number;
                    BookOfLossesPage = extract.PageNumber;
                    BookOfLossesExtractDate = new DateTimeOffset(extract.RecordDate);
                }

                AssetsTable.LoadAssets(data.Assets ?? []);
            });
        }

        private async Task<ReportGenerationResult> GenerateWriteOffReports(IEnumerable<IAssetInfo> assets, string destinationFolder)
        {
            var writeOffPackageData = BuildWriteOffPackageData(assets, destinationFolder);

            var result = await RunReportAsync(TextFormatHelper.WriteOffPackageName, "Помилка генерації пакету",
                () => _reportManager.GenerateWriteOffPackage(writeOffPackageData));
            return result;
        }
    }
}
