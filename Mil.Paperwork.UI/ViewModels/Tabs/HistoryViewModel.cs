using Mil.MVVM.Common;
using Mil.Paperwork.DataAccess.Conversions;
using Mil.Paperwork.DataAccess.DataModels.History;
using Mil.Paperwork.DataAccess.Repositories;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Helpers;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.UI.Helpers;
using Mil.Paperwork.UI.ViewModels.History;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Mil.Paperwork.UI.ViewModels.Tabs
{
    internal class HistoryViewModel : BaseTabViewModel
    {
        private const string TabHeader = "Історія";
        private const string AllTypesFilterText = "Всі";
        private const string ConfirmationCaption = "Підтвердження";
        private const string RemoveEntryConfirmation = "Видалити цей запис історії?";

        private const string SortByType = "TypeText";
        private const string SortByDate = "ModifiedAt";
        private const string SortByDocument = "NumberSummaryText";
        private const string SortByFile = "FileName";

        private readonly IReportHistoryRepository _historyRepository;
        private readonly ReportConversionRegistry _conversionRegistry;
        private readonly IDialogService _dialogService;

        private List<HistoryEntryViewModel> _allEntries;
        private HistoryTypeFilterItem? _selectedTypeFilter;
        private HistoryEntryViewModel? _selectedEntry;
        private DateTime? _dateFrom;
        private DateTime? _dateTo;
        private string _searchText = string.Empty;
        private string? _sortMemberPath;
        private ListSortDirection _sortDirection;
        private bool _isEmpty;
        private bool _isFilterEmpty;

        public event EventHandler<Guid> OpenHistoryEntryRequested;
        public event EventHandler<CreateFromRequestedEventArgs> CreateFromRequested;

        public override string Header => TabHeader;

        public bool IsEmpty
        {
            get => _isEmpty;
            private set => SetProperty(ref _isEmpty, value);
        }

        public bool IsFilterEmpty
        {
            get => _isFilterEmpty;
            private set => SetProperty(ref _isFilterEmpty, value);
        }

        public bool HasEntries => Entries.Count > 0;

        public bool IsClosed { get; private set; }

        public ObservableCollection<HistoryEntryViewModel> Entries { get; }

        public ObservableCollection<HistoryTypeFilterItem> TypeFilters { get; }

        public HistoryEntryViewModel? SelectedEntry
        {
            get => _selectedEntry;
            set
            {
                SetProperty(ref _selectedEntry, value);
            }
        }

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
        public IDelegateCommand ClearFiltersCommand { get; }
        public IDelegateCommand<HistoryEntryViewModel> OpenEntryCommand { get; }
        public IDelegateCommand<HistoryEntryViewModel> OpenGeneratedFileCommand { get; }
        public IDelegateCommand<HistoryEntryViewModel> OpenFileFolderCommand { get; }
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
            ClearFiltersCommand = new DelegateCommand(ClearFiltersExecute);
            OpenEntryCommand = new DelegateCommand<HistoryEntryViewModel>(OpenEntryCommandExecute);
            OpenGeneratedFileCommand = new DelegateCommand<HistoryEntryViewModel>(OpenGeneratedFileCommandExecute);
            OpenFileFolderCommand = new DelegateCommand<HistoryEntryViewModel>(OpenFileFolderCommandExecute);
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

        public string? CurrentSortMemberPath => _sortMemberPath;

        public ListSortDirection CurrentSortDirection => _sortDirection;

        public void ApplySortMember(string memberPath, ListSortDirection direction)
        {
            _sortMemberPath = memberPath;
            _sortDirection = direction;
            ApplyFilters();
        }

        public void ClearSort()
        {
            _sortMemberPath = null;
            ApplyFilters();
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

            filtered = ApplySortToFiltered(filtered);

            Entries.Clear();
            foreach (var entry in filtered)
            {
                Entries.Add(entry);
            }

            IsEmpty = _allEntries.Count == 0;
            IsFilterEmpty = _allEntries.Count > 0 && Entries.Count == 0;
            OnPropertyChanged(nameof(HasEntries));
        }

        private IEnumerable<HistoryEntryViewModel> ApplySortToFiltered(IEnumerable<HistoryEntryViewModel> entries)
        {
            if (_sortMemberPath == null)
            {
                var result = entries.OrderByDescending(e => e.ModifiedAt);
                return result;
            }

            if (_sortMemberPath == SortByType)
            {
                var result = _sortDirection == ListSortDirection.Ascending
                    ? entries.OrderBy(e => e.TypeText)
                    : entries.OrderByDescending(e => e.TypeText);
                return result;
            }

            if (_sortMemberPath == SortByDate)
            {
                var result = _sortDirection == ListSortDirection.Ascending
                    ? entries.OrderBy(e => e.ModifiedAt)
                    : entries.OrderByDescending(e => e.ModifiedAt);
                return result;
            }

            if (_sortMemberPath == SortByDocument)
            {
                var result = _sortDirection == ListSortDirection.Ascending
                    ? entries.OrderBy(e => e.NumberSummaryText)
                    : entries.OrderByDescending(e => e.NumberSummaryText);
                return result;
            }

            if (_sortMemberPath == SortByFile)
            {
                var result = _sortDirection == ListSortDirection.Ascending
                    ? entries.OrderBy(e => e.FileName)
                    : entries.OrderByDescending(e => e.FileName);
                return result;
            }

            var defaultResult = entries.OrderByDescending(e => e.ModifiedAt);
            return defaultResult;
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

        private void ClearFiltersExecute()
        {
            _dateFrom = null;
            _dateTo = null;
            _searchText = string.Empty;

            OnPropertyChanged(nameof(DateFrom));
            OnPropertyChanged(nameof(DateTo));
            OnPropertyChanged(nameof(SearchText));

            var allTypesItem = TypeFilters.FirstOrDefault();
            if (_selectedTypeFilter == allTypesItem)
            {
                ApplyFilters();
            }
            else
            {
                SelectedTypeFilter = allTypesItem;
            }
        }

        private async void OpenFileFolderCommandExecute(HistoryEntryViewModel entry)
        {
            if (entry == null || !entry.HasGeneratedFiles)
            {
                return;
            }

            await ShellOpenHelper.OpenFolderAsync(entry.IndexEntry.GeneratedFiles[0], _dialogService);
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
