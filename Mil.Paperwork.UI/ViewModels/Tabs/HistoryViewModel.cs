using Mil.MVVM.Common;
using Mil.Paperwork.DataAccess.Conversions;
using Mil.Paperwork.DataAccess.DataModels.History;
using Mil.Paperwork.DataAccess.Repositories;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Helpers;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.UI.ViewModels.History;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Mil.Paperwork.UI.ViewModels.Tabs
{
    internal class HistoryViewModel : BaseTabViewModel
    {
        private const string TabHeader = "Історія";
        private const string AllTypesFilterText = "Всі";
        private const string ConfirmationCaption = "Підтвердження";
        private const string ErrorCaption = "Помилка";
        private const string RemoveEntryConfirmation = "Видалити цей запис історії?";
        private const string OpenEntryNotSupportedMessage = "Відкриття запису з історії ще не підтримується.";
        private const string PathNotFoundMessageFormat = "Файл або теку не знайдено:\n{0}";
        private const string OpenPathErrorMessageFormat = "Не вдалося відкрити:\n{0}";

        private readonly IReportHistoryRepository _historyRepository;
        private readonly ReportConversionRegistry _conversionRegistry;
        private readonly IDialogService _dialogService;

        private List<HistoryEntryViewModel> _allEntries;
        private HistoryTypeFilterItem? _selectedTypeFilter;
        private DateTime? _dateFrom;
        private DateTime? _dateTo;
        private string _searchText = string.Empty;

        public event EventHandler<Guid> OpenHistoryEntryRequested;
        public event EventHandler<CreateFromRequestedEventArgs> CreateFromRequested;

        public override string Header => TabHeader;

        public bool IsClosed { get; private set; }

        public ObservableCollection<HistoryEntryViewModel> Entries { get; }

        public ObservableCollection<HistoryTypeFilterItem> TypeFilters { get; }

        public HistoryTypeFilterItem? SelectedTypeFilter
        {
            get => _selectedTypeFilter;
            set
            {
                if (SetProperty(ref _selectedTypeFilter, value))
                {
                    ApplyFilters();
                }
            }
        }

        public DateTime? DateFrom
        {
            get => _dateFrom;
            set
            {
                if (SetProperty(ref _dateFrom, value))
                {
                    ApplyFilters();
                }
            }
        }

        public DateTime? DateTo
        {
            get => _dateTo;
            set
            {
                if (SetProperty(ref _dateTo, value))
                {
                    ApplyFilters();
                }
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    ApplyFilters();
                }
            }
        }

        public IDelegateCommand RefreshCommand { get; }
        public IDelegateCommand<HistoryEntryViewModel> OpenEntryCommand { get; }
        public IDelegateCommand<HistoryEntryViewModel> OpenGeneratedFileCommand { get; }
        public IDelegateCommand<HistoryEntryViewModel> RemoveEntryCommand { get; }

        public HistoryViewModel(
            IReportHistoryRepository historyRepository,
            ReportConversionRegistry conversionRegistry,
            IDialogService dialogService)
            : base(dialogService)
        {
            _historyRepository = historyRepository;
            _conversionRegistry = conversionRegistry;
            _dialogService = dialogService;

            _allEntries = [];
            Entries = [];
            TypeFilters = [];

            RefreshCommand = new DelegateCommand(Refresh);
            OpenEntryCommand = new DelegateCommand<HistoryEntryViewModel>(OpenEntryCommandExecute);
            OpenGeneratedFileCommand = new DelegateCommand<HistoryEntryViewModel>(OpenGeneratedFileCommandExecute);
            RemoveEntryCommand = new DelegateCommand<HistoryEntryViewModel>(RemoveEntryCommandExecute);

            TabCloseRequested += OnTabCloseRequested;

            Refresh();
        }

        public void Refresh()
        {
            var index = _historyRepository.GetIndex();
            var entries = index
                .Select(entry => new HistoryEntryViewModel(entry, BuildCreateTargets(entry)))
                .OrderByDescending(entry => entry.ModifiedAt)
                .ToList();

            _allEntries = entries;

            RebuildTypeFilters();
            ApplyFilters();
        }

        protected override void Close()
        {
            RaiseTabCloseRequested();
        }

        private IReadOnlyList<CreateFromTargetItem> BuildCreateTargets(ReportHistoryIndexEntry indexEntry)
        {
            var targetTypes = _conversionRegistry.GetTargets(indexEntry.ReportType);
            var targets = targetTypes
                .Select(targetType => new CreateFromTargetItem(indexEntry.Id, targetType, targetType.GetDescription(), RaiseCreateFromRequested))
                .ToList();

            return targets;
        }

        private void RaiseCreateFromRequested(Guid entryId, ReportType targetType)
        {
            CreateFromRequested?.Invoke(this, new CreateFromRequestedEventArgs(entryId, targetType));
        }

        private void OnTabCloseRequested(object? sender, ITabViewModel tabViewModel)
        {
            IsClosed = true;
        }

        private void RebuildTypeFilters()
        {
            var selectedValue = _selectedTypeFilter?.Value;

            TypeFilters.Clear();

            var allTypesItem = new HistoryTypeFilterItem(null, AllTypesFilterText);
            TypeFilters.Add(allTypesItem);

            var presentTypes = _allEntries
                .Select(entry => entry.ReportType)
                .Distinct()
                .OrderBy(type => type.GetDescription())
                .ToList();

            foreach (var reportType in presentTypes)
            {
                TypeFilters.Add(new HistoryTypeFilterItem(reportType, reportType.GetDescription()));
            }

            var newSelection = TypeFilters.FirstOrDefault(item => item.Value == selectedValue) ?? allTypesItem;
            SelectedTypeFilter = newSelection;
        }

        private void ApplyFilters()
        {
            var filtered = _allEntries.AsEnumerable();

            var typeFilter = _selectedTypeFilter?.Value;
            if (typeFilter != null)
            {
                filtered = filtered.Where(entry => entry.ReportType == typeFilter.Value);
            }

            if (_dateFrom != null)
            {
                filtered = filtered.Where(entry => entry.ModifiedAt.Date >= _dateFrom.Value.Date);
            }

            if (_dateTo != null)
            {
                filtered = filtered.Where(entry => entry.ModifiedAt.Date <= _dateTo.Value.Date);
            }

            if (!string.IsNullOrWhiteSpace(_searchText))
            {
                filtered = filtered.Where(MatchesSearchText);
            }

            Entries.Clear();
            foreach (var entry in filtered)
            {
                Entries.Add(entry);
            }
        }

        private bool MatchesSearchText(HistoryEntryViewModel entry)
        {
            var indexEntry = entry.IndexEntry;
            var result = ContainsIgnoreCase(indexEntry.DocumentNumber, _searchText)
                || indexEntry.AssetNames.Any(name => ContainsIgnoreCase(name, _searchText))
                || indexEntry.SerialNumbers.Any(serialNumber => ContainsIgnoreCase(serialNumber, _searchText))
                || indexEntry.NomenclatureCodes.Any(code => ContainsIgnoreCase(code, _searchText));

            return result;
        }

        private static bool ContainsIgnoreCase(string? source, string value)
        {
            var result = source?.Contains(value, StringComparison.OrdinalIgnoreCase) == true;
            return result;
        }

        private async void OpenEntryCommandExecute(HistoryEntryViewModel entry)
        {
            if (entry == null)
            {
                return;
            }

            var handler = OpenHistoryEntryRequested;
            if (handler != null)
            {
                handler.Invoke(this, entry.Id);
            }
            else
            {
                await _dialogService.ShowMessageAsync(OpenEntryNotSupportedMessage, ConfirmationCaption);
            }
        }

        private async void OpenGeneratedFileCommandExecute(HistoryEntryViewModel entry)
        {
            if (entry == null || !entry.HasGeneratedFiles)
            {
                return;
            }

            var generatedFiles = entry.IndexEntry.GeneratedFiles;

            string pathToOpen;
            bool pathExists;
            if (generatedFiles.Count == 1)
            {
                pathToOpen = generatedFiles[0];
                pathExists = File.Exists(pathToOpen);
            }
            else
            {
                pathToOpen = Path.GetDirectoryName(generatedFiles[0]) ?? generatedFiles[0];
                pathExists = Directory.Exists(pathToOpen);
            }

            if (!pathExists)
            {
                var notFoundMessage = string.Format(PathNotFoundMessageFormat, pathToOpen);
                await _dialogService.ShowMessageAsync(notFoundMessage, ErrorCaption, DialogButtons.OK, DialogIcon.Error);
                return;
            }

            try
            {
                Process.Start(new ProcessStartInfo(pathToOpen) { UseShellExecute = true });
            }
            catch (Exception)
            {
                var errorMessage = string.Format(OpenPathErrorMessageFormat, pathToOpen);
                await _dialogService.ShowMessageAsync(errorMessage, ErrorCaption, DialogButtons.OK, DialogIcon.Error);
            }
        }

        private async void RemoveEntryCommandExecute(HistoryEntryViewModel entry)
        {
            if (entry == null)
            {
                return;
            }

            var dlgResult = await _dialogService.ShowMessageAsync(RemoveEntryConfirmation, ConfirmationCaption, DialogButtons.YesNo);
            if (dlgResult == DialogResult.Yes)
            {
                _historyRepository.Delete(entry.Id);
                _allEntries.Remove(entry);
                Entries.Remove(entry);
            }
        }
    }
}
