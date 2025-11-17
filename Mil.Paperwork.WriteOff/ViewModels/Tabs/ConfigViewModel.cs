using Mil.Paperwork.Common.Enums;
using Mil.Paperwork.Common.MVVM;
using Mil.Paperwork.Domain.Services;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Services;
using System.Windows.Input;

namespace Mil.Paperwork.WriteOff.ViewModels.Tabs
{
    internal abstract class ConfigViewModel : ObservableItem, ISettingsTabViewModel
    {
        private const string ImportFileFilter = "JSON Files|*.json";
        private const string ImportFileTitle = "Виберіть файл JSON для імпорту";
        private readonly IExportService _exportService;
        private readonly IImportService _importService;
        private readonly IDialogService _dialogService;

        public abstract List<object> ExportData { get; }

        public abstract string Header { get; }
        public bool IsClosed { get; private set; }

        public event EventHandler<ITabViewModel> TabCloseRequested;

        public ICommand<ExportType> ExportDataCommand { get; }
        public ICommand ImportCommand { get; }
        public ICommand CloseCommand { get; }

        public ConfigViewModel(
            IExportService exportService,
            IImportService importService,
            IDialogService dialogService)
        {
            _exportService = exportService;
            _importService = importService;
            _dialogService = dialogService;

            ExportDataCommand = new DelegateCommand<ExportType>(ExportDataCommandExecute);
            ImportCommand = new DelegateCommand(ImportCommandExecute);
            CloseCommand = new DelegateCommand(CloseCommandExecute);
        }

        protected abstract void UpdateCurrentConfig(bool withReload = false);

        private void ExportDataCommandExecute(ExportType exportType)
        {
            if (_dialogService.TryPickFolder(out var folderName))
            {
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

                _dialogService.ShowMessage(message);
            }
        }

        private void ImportCommandExecute()
        {
            var filePath = GetFileToImport();

            if (!string.IsNullOrEmpty(filePath))
            {
                var messageBoxResult = _dialogService.ShowMessage("Ви впевнені що бажаєте імпортувати файл налаштувань?", "Підтвердження", DialogButtons.YesNo);
                if (messageBoxResult == DialogResult.Yes)
                {
                    var result = _importService.TryImportSettingsConfigFile(filePath);

                    var message = result ? "Конфігурація імпортована успішно." : "Помилка прід час імпорту файлу конфігурації";
                    _dialogService.ShowMessage(message);

                    if (result)
                    {
                        UpdateCurrentConfig(true);
                    }
                }
            }
        }

        private string GetFileToImport()
        {
            _dialogService.TryPickFile(out var filePath, ImportFileFilter, ImportFileTitle);
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
