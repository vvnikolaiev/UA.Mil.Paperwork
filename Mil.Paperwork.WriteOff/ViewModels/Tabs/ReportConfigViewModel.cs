using Microsoft.Win32;
using Mil.Paperwork.Domain.Services;
using Mil.Paperwork.Infrastructure.DataModels;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Helpers;
using Mil.Paperwork.Infrastructure.MVVM;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.WriteOff.DataModels;
using Mil.Paperwork.WriteOff.Enums;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Mil.Paperwork.WriteOff.ViewModels.Tabs
{
    internal class ReportConfigViewModel : ObservableItem, ISettingsTabViewModel
    {
        private ReportType _selectedReportType;
        private ObservableCollection<ReportParameter> _currentConfig;
        private readonly IReportDataService _reportDataService;
        private readonly IExportService _exportService;

        public ObservableCollection<ReportType> ReportTypes { get; }
        public ObservableCollection<EnumItemDataModel<ExportType>> ExportTypes { get; private set; }

        public ReportType SelectedReportType
        {
            get => _selectedReportType;
            set => SetProperty(ref _selectedReportType, value);
        }

        public ObservableCollection<ReportParameter> CurrentConfig
        {
            get => _currentConfig;
            set => SetProperty(ref _currentConfig, value);
        }

        public ICommand ReportTypeSelectedCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand<ExportType> ExportDataCommand { get; }
        public ICommand ImportCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand CloseCommand { get; }

        public string Header => "Налаштування звітів";

        public bool IsClosed { get; private set; }

        public event EventHandler<ITabViewModel> TabCloseRequested;

        public ReportConfigViewModel(IReportDataService reportDataService, IExportService exportService)
        {
            _reportDataService = reportDataService;
            _exportService = exportService;

            ReportTypes = [.. EnumHelper.GetValues<ReportType>()];
            SelectedReportType = ReportTypes.FirstOrDefault();

            FillExportTypesCollection();
            UpdateCurrentConfig(SelectedReportType);

            ReportTypeSelectedCommand = new DelegateCommand(ReportTypeSelectedCommandExecute);

            SaveCommand = new DelegateCommand(SaveCommandExecute);
            ExportDataCommand = new DelegateCommand<ExportType>(ExportDataCommandExecute);
            ImportCommand = new DelegateCommand(ImportCommandExecute, () => false);
            RefreshCommand = new DelegateCommand(RefreshCommandExecute);
            CloseCommand = new DelegateCommand(CloseCommandExecute);
        }

        private void ReportTypeSelectedCommandExecute()
        {
            UpdateCurrentConfig(SelectedReportType);
        }

        private void UpdateCurrentConfig(ReportType? reportType)
        {
            if (reportType != null)
            {
                var reportConfig = _reportDataService.GetReportParameters(reportType.Value);

                CurrentConfig = [.. reportConfig ?? []];
            }
            else
            {
                CurrentConfig = [];
            }
        }

        private void FillExportTypesCollection()
        {
            var types = EnumHelper.GetValuesWithDescriptions<ExportType>().Select(x => new EnumItemDataModel<ExportType>(x.Value, x.Description));
            ExportTypes = [.. types];
        }

        private void SaveCommandExecute()
        {
            _reportDataService.SaveReportConfig(CurrentConfig.ToList(), SelectedReportType);
        }

        private void ExportDataCommandExecute(ExportType exportType)
        {
            var folderDialog = new OpenFolderDialog();

            if (folderDialog.ShowDialog() == true)
            {
                var folderName = folderDialog.FolderName;
                var reportParameters = CurrentConfig.ToList();

                var fileNameFormat = GetFileNameFormat(SelectedReportType, exportType);

                var result = exportType switch
                {
                    ExportType.Json => _exportService.TryExportToJson(reportParameters, folderName, fileNameFormat),
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
            /* Import logic */
        }

        private void RefreshCommandExecute()
        {
            var result = MessageBox.Show("Ви впевнені що бажаєте перезавантажити таблицю?", "Підтвердження", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                UpdateCurrentConfig(SelectedReportType);
            }
        }

        private void CloseCommandExecute()
        {
            IsClosed = true;
            TabCloseRequested?.Invoke(this, this);
        }

        private static string GetFileNameFormat(ReportType reportType, ExportType exportType)
        {
            var reportTypeTitle = reportType.GetDescription();

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
