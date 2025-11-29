using Mil.Paperwork.Common.Enums;
using Mil.Paperwork.Common.MVVM;
using Mil.Paperwork.Domain.Services;
using Mil.Paperwork.Infrastructure.DataModels;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Helpers;
using Mil.Paperwork.Infrastructure.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Mil.Paperwork.UI.ViewModels.Tabs
{
    internal class ReportConfigViewModel : ConfigViewModel
    {
        private readonly IReportDataService _reportDataService;
        private readonly IDialogService _dialogService;

        private ReportType _selectedReportType;
        private ObservableCollection<ReportParameter> _currentConfig;

        protected override string ExportFileTitle => SelectedReportType.GetDescription();

        public ObservableCollection<ReportType> ReportTypes { get; }
        public ObservableCollection<ExportType> ExportTypes { get; private set; }

        public ReportType SelectedReportType
        {
            get => _selectedReportType;
            set
            {
                if (SetProperty(ref _selectedReportType, value))
                {
                    UpdateCurrentConfig();
                }
            }
        }

        public override List<object> ExportData => [.. CurrentConfig];

        public ObservableCollection<ReportParameter> CurrentConfig
        {
            get => _currentConfig;
            set => SetProperty(ref _currentConfig, value);
        }

        public IDelegateCommand SaveCommand { get; }
        public IDelegateCommand SaveLocalCommand { get; }
        public IDelegateCommand RefreshCommand { get; }

        public override string Header => "Налаштування звітів";

        public ReportConfigViewModel(
            IReportDataService reportDataService,
            IExportService exportService,
            IImportService importService,
            IDialogService dialogService) : base(exportService, importService, dialogService)
        {
            _reportDataService = reportDataService;
            _dialogService = dialogService;

            ReportTypes = [.. EnumHelper.GetValues<ReportType>()];
            ExportTypes = [.. EnumHelper.GetValues<ExportType>()];
            SelectedReportType = ReportTypes.FirstOrDefault();

            UpdateCurrentConfig();

            SaveCommand = new DelegateCommand(SaveCommandExecute);
            SaveLocalCommand = new DelegateCommand(SaveLocalCommandExecute);
            RefreshCommand = new DelegateCommand(RefreshCommandExecute);
        }

        public async void SelectReportType(ReportType reportType)
        {
            if (ReportTypes.Contains(reportType))
            {
                SelectedReportType = reportType;
                UpdateCurrentConfig();
            }
            else
            {
                await _dialogService.ShowMessageAsync("Невідомий тип звіту.");
            }
        }

        protected override void UpdateCurrentConfig(bool withReload = false)
        {
            var reportConfig = _reportDataService.GetReportParameters(SelectedReportType, withReload);

            CurrentConfig = [.. reportConfig ?? []];
        }

        private void SaveCommandExecute()
        {
            _reportDataService.SaveReportConfig([.. CurrentConfig], SelectedReportType);
        }

        private void SaveLocalCommandExecute()
        {
            _reportDataService.SaveReportConfigTemprorary([.. CurrentConfig], SelectedReportType);
        }

        private async void RefreshCommandExecute()
        {
            var result = await _dialogService.ShowMessageAsync("Ви впевнені що бажаєте перезавантажити таблицю?", "Підтвердження", DialogButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                UpdateCurrentConfig(withReload: true);
            }
        }
    }
}
