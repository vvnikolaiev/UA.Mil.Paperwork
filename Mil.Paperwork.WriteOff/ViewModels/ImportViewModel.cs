using Microsoft.Win32;
using Mil.Paperwork.Domain.Services;
using Mil.Paperwork.Infrastructure.MVVM;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.WriteOff.DataModels;
using Mil.Paperwork.WriteOff.Enums;
using Mil.Paperwork.WriteOff.Factories;
using Mil.Paperwork.WriteOff.Helpers;
using Mil.Paperwork.WriteOff.Strategies;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Input;

namespace Mil.Paperwork.WriteOff.ViewModels
{
    internal class ImportViewModel : ObservableItem
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
        private readonly INavigationService _navigationService;

        private IImportStrategy _importStrategy;

        private string _importFilePath;
        private bool _isValid;
        private List<Dictionary<string, object>> _previewRows;
        private DataTable _previewTable;

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

        public bool IsFirstRowContainsHeaders { get; set; } = true;

        public ObservableCollection<ImportColumnDefinition> ColumnsToMap { get; set; }
        public ObservableCollection<string> SourceHeaders { get; set; } = new ObservableCollection<string>();
        public DataTable PreviewTable
        {
            get => _previewTable;
            set => SetProperty(ref _previewTable, value);
        }
        
        public ImportDataResult ImportDataResult { get; set; }

        public ICommand SelectFileCommand { get; }
        public ICommand OKCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand MappingChangedCommand { get; }
        public ICommand HeaderRowChangedCommand { get; }

        public ImportViewModel(IImportService importService, IDataService dataService, INavigationService navigationService)
        {
            _navigationService = navigationService;
            _importService = importService;
            _dataService = dataService;

            ColumnsToMap = [];

            SelectFileCommand = new DelegateCommand(SelectFileCommandExecute);
            OKCommand = new DelegateCommand(OKCommandExecute);
            CancelCommand = new DelegateCommand(CancelCommandExecute);
            MappingChangedCommand = new DelegateCommand(MappingChangedCommandExecute);
            HeaderRowChangedCommand = new DelegateCommand(HeaderRowChangedCommandExecute);
        }

        public void SetImportType(ImportType importType)
        {
            _importStrategy = ImportStrategyFactory.GetStrategy(_dataService, importType);

            var columns = _importStrategy.GetColumnsToMap();

            ColumnsToMap.Clear();
            foreach (var column in columns)
            {
                ColumnsToMap.Add(column);
            }
        }

        private void UpdateSourceHeaders()
        {
            if (string.IsNullOrWhiteSpace(ImportFilePath))
            {
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
            var openFileDialog = new OpenFileDialog
            {
                Filter = ImportFileFilter,
                Title = ImportFileTitle
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var path = openFileDialog.FileName;
                ImportFilePath = path;
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

            if (IsValid)
            {
                // fill the table
                var dataTable = _importStrategy.GetDataTable([.. ColumnsToMap], _previewRows);
                PreviewTable = dataTable;
            }
        }

        private void OKCommandExecute()
        {
            if (!IsValid)
            {
                MessageBox.Show(ImportFileInvalidMappingMessage, ImportFileInvalidMappingTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var result = MessageBox.Show(ImportConfirmationMessage, ImportConfirmationTitle, MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                var data = GetExcelRows();
                var importResult = _importStrategy.ImportData([.. ColumnsToMap], data);
               
                ImportDataResult = importResult;
                
                ImportHelper.ShowImportResultMessage(importResult);
            }

            CloseWindow();
        }
        
        private void CancelCommandExecute()
        {
            var result = MessageBox.Show(ImportConfirmationCancelMessage, ImportConfirmationTitle, MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                CloseWindow();
            }
        }

        private void CloseWindow()
        {
            _navigationService.CloseWindow(this);
        }
    }
}
