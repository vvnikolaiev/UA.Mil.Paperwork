using Microsoft.Win32;
using Mil.Paperwork.Domain.Services;
using Mil.Paperwork.Infrastructure.MVVM;
using Mil.Paperwork.WriteOff.Enums;
using System.Windows;
using System.Windows.Input;

namespace Mil.Paperwork.WriteOff.ViewModels.Tabs
{
    internal abstract class ConfigViewModel : ObservableItem, ISettingsTabViewModel
    {
        private const string ImportFileFilter = "JSON Files|*.json";
        private const string ImportFileTitle = "Виберіть файл JSON для імпорту";
        private readonly IExportService _exportService;
        private readonly IImportService _importService;

        public abstract List<object> ExportData { get; }

        public abstract string Header { get; }
        public bool IsClosed { get; private set; }

        public event EventHandler<ITabViewModel> TabCloseRequested;

        public ICommand<ExportType> ExportDataCommand { get; }
        public ICommand ImportCommand { get; }
        public ICommand CloseCommand { get; }

        public ConfigViewModel(
            IExportService exportService,
            IImportService importService)
        {
            _exportService = exportService;
            _importService = importService;

            ExportDataCommand = new DelegateCommand<ExportType>(ExportDataCommandExecute);
            ImportCommand = new DelegateCommand(ImportCommandExecute);
            CloseCommand = new DelegateCommand(CloseCommandExecute);
        }

        protected abstract void UpdateCurrentConfig(bool withReload = false);

        private void ExportDataCommandExecute(ExportType exportType)
        {
            var folderDialog = new OpenFolderDialog();

            if (folderDialog.ShowDialog() == true)
            {
                var folderName = folderDialog.FolderName;
                var reportParameters = ExportData;

                var fileNameFormat = GetFileNameFormat(exportType);

                var result = exportType switch
                {
                    ExportType.Json => _exportService.TryExportSettingsConfigFile(folderName),
                    //ExportType.Json => _exportService.TryExportToJson(reportParameters, folderName, fileNameFormat),
                    ExportType.Excel => _exportService.TryExportToExcel(reportParameters, folderName, fileNameFormat),
                    _ => throw new ArgumentOutOfRangeException(nameof(exportType), exportType, null)
                };

                string message;
                if (result)
                {
                    message = $"Дані експортовано успішно. Каталог:\r\n{folderName}";
                }
                else
                {
                    message = $"Помилка експорту данних.";
                }

                MessageBox.Show(message);
            }
        }

        private void ImportCommandExecute()
        {
            var filePath = GetFileToImport();

            if (!string.IsNullOrEmpty(filePath))
            {
                var messageBoxResult = MessageBox.Show("Ви впевнені що бажаєте імпортувати файл налаштувань?", "Підтвердження", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    var result = _importService.TryImportSettingsConfigFile(filePath);

                    var message = result ? "Конфігурація імпортована успішно." : "Помилка прід час імпорту файлу конфігурації";
                    MessageBox.Show(message);

                    if (result)
                    {
                        UpdateCurrentConfig(true);
                    }
                }
            }
        }

        private string GetFileToImport()
        {
            string filePath = string.Empty;
            var openFileDialog = new OpenFileDialog
            {
                Filter = ImportFileFilter,
                Title = ImportFileTitle
            };

            if (openFileDialog.ShowDialog() == true)
            {
                filePath = openFileDialog.FileName;
            }

            return filePath;
        }

        private void CloseCommandExecute()
        {
            IsClosed = true;
            TabCloseRequested?.Invoke(this, this);
        }

        protected abstract string ExportFileTitle { get; }

        private string GetFileNameFormat(ExportType exportType)
        {
            var reportTypeTitle = ExportFileTitle;

            var extension = exportType switch
            {
                ExportType.Json => "json",
                ExportType.Excel => "xlsx",
                _ => throw new ArgumentOutOfRangeException(nameof(exportType), exportType, null)
            };

            return $"Конфігурація {reportTypeTitle} {{0}}.{extension}";
        }
    }
}
