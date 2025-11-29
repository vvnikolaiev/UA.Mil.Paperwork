using Mil.Paperwork.Common.DataModels;
using Mil.Paperwork.Common.Enums;
using Mil.Paperwork.Common.Factories;
using Mil.Paperwork.Common.Helpers;
using Mil.Paperwork.Common.MVVM;
using Mil.Paperwork.Common.Strategies;
using Mil.Paperwork.Domain.Services;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Windows.Input;

namespace Mil.Paperwork.UI.ViewModels
{
    internal class ImportViewModel : ObservableItem, IWindowViewModel
    {
        private const string ImportFileFilter = "Excel Files|*.xlsx;*.xls";
        private const string ImportFileTitle = "Виберіть файл Excel для імпорту";
        private const string ImportFileInvalidMappingMessage = "Будь ласка, заповніть всі обов'язкові поля для імпорту.";
        private const string ImportConfirmationMessage = "Ви впевнені що бажаєте імпортувати ці дані?\r\nРядки з відсутніми або некоректно введеними даними будуть пропущені.";
        private const string ImportConfirmationCancelMessage = "Ви впевнені що бажаєте скасувати імпорт даних?";
        private const string ImportFileInvalidMappingTitle = "Помилка мапінгу";
        private const string ImportConfirmationTitle = "Підтвердження";

        private readonly IImportService _importService;
        private readonly IDataService _dataService;
        private readonly IDialogService _dialogService;

        private IImportStrategy _importStrategy;

        private string _importFilePath;
        private bool _isValid;
        private bool _isFirstRowContainsHeaders = true;
        private List<Dictionary<string, object>> _previewRows = [];
        private ObservableCollection<ExpandoObject> _previewTable;

        public string Title => "Імпорт даних";

        public string ImportFilePath
        {
            get => _importFilePath;
            set => SetProperty(ref _importFilePath, value);
        }

        public bool IsValid
        {
            get => _isValid;
            set => SetProperty(ref _isValid, value);
        }

        public bool IsFirstRowContainsHeaders
        {
            get => _isFirstRowContainsHeaders;
            set
            {
                if (SetProperty(ref _isFirstRowContainsHeaders, value))
                {
                    HeaderRowChangedCommandExecute();
                }
            }
        }

        public ObservableCollection<ImportColumnDefinition> ColumnsToMap { get; set; }
        public ObservableCollection<string> SourceHeaders { get; set; } = [];
        public ObservableCollection<ExpandoObject> PreviewTable
        {
            get => _previewTable;
            set => SetProperty(ref _previewTable, value);
        }
        
        public ImportDataResult ImportDataResult { get; set; }

        public event Action RequestClose;

        public ICommand SelectFileCommand { get; }
        public ICommand OKCommand { get; }
        public ICommand CancelCommand { get; }

        public ImportViewModel(
            IImportService importService, 
            IDataService dataService, 
            IDialogService dialogService)
        {
            _importService = importService;
            _dataService = dataService;
            _dialogService = dialogService;

            // initialize collections
            ColumnsToMap = [];
            PreviewTable = [];

            SelectFileCommand = new DelegateCommand(SelectFileCommandExecute);
            OKCommand = new DelegateCommand(OKCommandExecute);
            CancelCommand = new DelegateCommand(CancelCommandExecute);
        }

        public void SetImportType(ImportType importType)
        {
            _importStrategy = ImportStrategyFactory.GetStrategy(_dataService, importType);

            var columns = _importStrategy.GetColumnsToMap();

            ClearColumns();
            foreach (var column in columns)
            {
                ColumnsToMap.Add(column);
                column.MappingChanged += OnColumnMappingChanged;
            }

            // ensure mapping validity and preview are updated for new columns
            UpdateSourceHeaders();
            UpdateMappingPreview();
        }

        private void OnColumnMappingChanged(object? sender, EventArgs e)
        {
            MappingChangedCommandExecute();
        }

        private void ClearColumns()
        {
            foreach (var column in ColumnsToMap)
            {
                column.MappingChanged -= OnColumnMappingChanged;
            }
            ColumnsToMap.Clear();
        }

        private void UpdateSourceHeaders()
        {
            if (string.IsNullOrWhiteSpace(ImportFilePath))
            {
                SourceHeaders.Clear();
                return;
            }

            var headerRowIndex = GetHeaderRowIndex();
            var headers = _importService.GetExcelTableHeaders(ImportFilePath, headerRowIndex);

            SourceHeaders.Clear();
            foreach (var header in headers)
            {
                SourceHeaders.Add(header);
            }

            LoadPreviewData();
            UpdateMappingPreview();
        }

        private void LoadPreviewData()
        {
            _previewRows = GetExcelRows(10);
        }

        private List<Dictionary<string, object>> GetExcelRows(int maxRowsCount = 0)
        {
            var headerRowIndex = GetHeaderRowIndex();
            var rows = _importService.GetExcelRows(ImportFilePath, headerRowIndex, maxRowsCount);
            return rows;
        }

        private int GetHeaderRowIndex()
        {
            var headerRowIndex = IsFirstRowContainsHeaders ? 1 : -1;
            return headerRowIndex;
        }

        private void MappingChangedCommandExecute()
        {
            UpdateMappingPreview();
        }

        private void HeaderRowChangedCommandExecute()
        {
            UpdateSourceHeaders();
            UpdateMappingPreview();
        }

        private void SelectFileCommandExecute()
        {
            if (_dialogService.TryPickFile(out var filePath, ImportFileFilter, ImportFileTitle))
            {
                ImportFilePath = filePath;
                UpdateSourceHeaders();
            }
        }

        private void UpdateIsMappingValid()
        {
            var isAllRequiredFieldsMapped = ColumnsToMap.Any(x => x.IsRequired && string.IsNullOrEmpty(x.SelectedSourceColumn)) == false;
            IsValid = isAllRequiredFieldsMapped;
        }

        private void UpdateMappingPreview()
        {
            UpdateIsMappingValid();

            // if mapping not valid or there is no strategy or no preview rows - clear preview
            if (!IsValid || _importStrategy == null || _previewRows == null || !_previewRows.Any())
            {
                PreviewTable = [];
                return;
            }

            // Build preview items using strategy
            try
            {
                var items = _importStrategy.GetItemsCollection(ColumnsToMap.ToList(), _previewRows);
                if (items == null)
                {
                    PreviewTable = [];
                    return;
                }

                PreviewTable = new ObservableCollection<ExpandoObject>(items);
            }
            catch
            {
                // on any error - clear preview to avoid binding exceptions in UI
                PreviewTable = [];
            }
        }

        private async void OKCommandExecute()
        {
            if (!IsValid)
            {
                await _dialogService.ShowMessageAsync(ImportFileInvalidMappingMessage, ImportFileInvalidMappingTitle, DialogButtons.OK, DialogIcon.Error);
                return;
            }

            var result = await _dialogService.ShowMessageAsync(ImportConfirmationMessage, ImportConfirmationTitle, DialogButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                var data = GetExcelRows();
                var importResult = _importStrategy.ImportData([.. ColumnsToMap], data);
               
                ImportDataResult = importResult;
                
                var message = ImportHelper.GetImportResultMessage(importResult);
                var caption = ImportHelper.GetImportResultCaption(importResult);
                var icon = ImportHelper.GetImportResultIcon(importResult);

                await _dialogService.ShowMessageAsync(message, caption, icon: icon);
            }

            CloseWindow();
        }
        
        private async void CancelCommandExecute()
        {
            var result = await _dialogService.ShowMessageAsync(ImportConfirmationCancelMessage, ImportConfirmationTitle, DialogButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                CloseWindow();
            }
        }

        private void CloseWindow()
        {
            RequestClose?.Invoke();
        }
    }
}
