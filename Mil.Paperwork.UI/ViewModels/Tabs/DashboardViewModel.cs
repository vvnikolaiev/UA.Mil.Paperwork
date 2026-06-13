using Mil.MVVM.Common;
using Mil.Paperwork.DataAccess.Enums;
using Mil.Paperwork.DataAccess.Repositories;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.UI.Helpers;
using Mil.Paperwork.UI.Services;
using Mil.Paperwork.UI.ViewModels.Dashboard;
using Mil.Paperwork.UI.ViewModels.History;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Mil.Paperwork.UI.ViewModels.Tabs
{
    internal class DashboardViewModel : BaseTabViewModel
    {
        private const string TabHeader = "Головна";

        private const string LabelDrafts = "Чернетки в роботі";
        private const string LabelGeneratedThisMonth = "Сформовано цього місяця";
        private const string LabelTotal = "Всього документів";

        private const int MetricIndexDrafts = 0;
        private const int MetricIndexGeneratedThisMonth = 1;
        private const int MetricIndexTotal = 2;

        private const int RecentEntriesLimit = 12;
        private const int MaxReportCardsCount = 4;

        private readonly IReportHistoryRepository _historyRepository;
        private readonly IUserSettingsService _userSettingsService;
        private readonly IDialogService _dialogService;

        public event EventHandler<ReportType> ReportCreationRequested;
        public event EventHandler NavigateToHistoryRequested;
        public event EventHandler NavigateToReportCatalogRequested;
        public event EventHandler<Guid> OpenHistoryEntryRequested;

        public override string Header => TabHeader;

        public ObservableCollection<MetricCardViewModel> Metrics { get; }

        public ObservableCollection<ReportCardViewModel> ReportCards { get; }

        public ObservableCollection<HistoryEntryViewModel> RecentEntries { get; }

        public IDelegateCommand NavigateToHistoryCommand { get; }
        public IDelegateCommand NavigateToReportCatalogCommand { get; }
        public IDelegateCommand<HistoryEntryViewModel> OpenEntryCommand { get; }
        public IDelegateCommand<HistoryEntryViewModel> OpenGeneratedFileCommand { get; }
        public IDelegateCommand<HistoryEntryViewModel> OpenFileFolderCommand { get; }

        public DashboardViewModel(
            IReportHistoryRepository historyRepository,
            IUserSettingsService userSettingsService,
            IDialogService dialogService)
            : base(dialogService)
        {
            _historyRepository = historyRepository;
            _userSettingsService = userSettingsService;
            _dialogService = dialogService;

            Metrics =
            [
                new MetricCardViewModel(LabelDrafts, 0),
                new MetricCardViewModel(LabelGeneratedThisMonth, 0),
                new MetricCardViewModel(LabelTotal, 0)
            ];

            ReportCards = [];
            RecentEntries = [];

            NavigateToHistoryCommand = new DelegateCommand(NavigateToHistoryCommandExecute);
            NavigateToReportCatalogCommand = new DelegateCommand(NavigateToReportCatalogCommandExecute);
            OpenEntryCommand = new DelegateCommand<HistoryEntryViewModel>(OpenEntryCommandExecute);
            OpenGeneratedFileCommand = new DelegateCommand<HistoryEntryViewModel>(OpenGeneratedFileCommandExecute);
            OpenFileFolderCommand = new DelegateCommand<HistoryEntryViewModel>(OpenFileFolderCommandExecute);
        }

        public void Refresh()
        {
            LoadHistoryData();
            RebuildReportCards();
        }

        private void LoadHistoryData()
        {
            var index = _historyRepository.GetIndex();
            var allEntries = index.OrderByDescending(e => e.ModifiedAt).ToList();

            var now = DateTime.Now;
            var draftCount = allEntries.Count(e => e.Status == HistoryEntryStatus.Draft);
            var generatedThisMonthCount = allEntries.Count(e =>
                e.Status == HistoryEntryStatus.Generated &&
                e.ModifiedAt.Year == now.Year &&
                e.ModifiedAt.Month == now.Month);
            var totalCount = allEntries.Count;

            Metrics[MetricIndexDrafts].Value = draftCount;
            Metrics[MetricIndexGeneratedThisMonth].Value = generatedThisMonthCount;
            Metrics[MetricIndexTotal].Value = totalCount;

            RecentEntries.Clear();
            foreach (var entry in allEntries.Take(RecentEntriesLimit))
            {
                RecentEntries.Add(new HistoryEntryViewModel(entry, []));
            }
        }

        private void RebuildReportCards()
        {
            var settings = _userSettingsService.GetSettings();
            var favorites = settings.FavoriteReportTypes;
            var recents = settings.RecentReportTypes;

            var displayedTypes = favorites
                .Concat(recents.Where(t => !favorites.Contains(t)))
                .Take(MaxReportCardsCount)
                .ToList();

            ReportCards.Clear();
            foreach (var reportType in displayedTypes)
            {
                var isFavorite = favorites.Contains(reportType);
                var card = new ReportCardViewModel(reportType, isFavorite, RaiseReportCreationRequested, OnFavoriteChanged);
                ReportCards.Add(card);
            }
        }

        private void RaiseReportCreationRequested(ReportType reportType)
        {
            ReportCreationRequested?.Invoke(this, reportType);
        }

        private void OnFavoriteChanged(ReportType reportType, bool isFavorite)
        {
            var settings = _userSettingsService.GetSettings();
            if (isFavorite)
            {
                if (!settings.FavoriteReportTypes.Contains(reportType))
                {
                    settings.FavoriteReportTypes.Add(reportType);
                }
            }
            else
            {
                settings.FavoriteReportTypes.Remove(reportType);
            }

            _userSettingsService.SaveSettings(settings);
            RebuildReportCards();
        }

        private void NavigateToHistoryCommandExecute()
        {
            NavigateToHistoryRequested?.Invoke(this, EventArgs.Empty);
        }

        private void NavigateToReportCatalogCommandExecute()
        {
            NavigateToReportCatalogRequested?.Invoke(this, EventArgs.Empty);
        }

        private void OpenEntryCommandExecute(HistoryEntryViewModel entry)
        {
            if (entry == null)
            {
                return;
            }

            OpenHistoryEntryRequested?.Invoke(this, entry.Id);
        }

        private async void OpenGeneratedFileCommandExecute(HistoryEntryViewModel entry)
        {
            if (entry == null || !entry.HasGeneratedFiles)
            {
                return;
            }

            await ShellOpenHelper.OpenFilesAsync(entry.IndexEntry.GeneratedFiles, _dialogService);
        }

        private async void OpenFileFolderCommandExecute(HistoryEntryViewModel entry)
        {
            if (entry == null || !entry.HasGeneratedFiles)
            {
                return;
            }

            await ShellOpenHelper.OpenFolderAsync(entry.IndexEntry.GeneratedFiles[0], _dialogService);
        }
    }
}
