using Mil.Paperwork.Domain.Services;
using Mil.Paperwork.Infrastructure.DataModels;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Helpers;
using Mil.Paperwork.WriteOff.MVVM;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.WriteOff.DataModels;
using Mil.Paperwork.WriteOff.Enums;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Mil.Paperwork.WriteOff.ViewModels.Tabs
{
    internal class ReportConfigViewModel : ConfigViewModel
    {
        private readonly IReportDataService _reportDataService;
        private readonly IDialogService _dialogService;

        private ReportType _selectedReportType;
        private ObservableCollection<ReportParameter> _currentConfig;

        protected override string ExportFileTitle => SelectedReportType.GetDescription();

        public ObservableCollection<ReportType> ReportTypes { get; }
        public ObservableCollection<EnumItemDataModel<ExportType>> ExportTypes { get; private set; }

        public ReportType SelectedReportType
        {
            get => _selectedReportType;
            set => SetProperty(ref _selectedReportType, value);
        }

        public override List<object> ExportData => [.. CurrentConfig];

        public ObservableCollection<ReportParameter> CurrentConfig
        {
            get => _currentConfig;
            set => SetProperty(ref _currentConfig, value);
        }

        public ICommand ReportTypeSelectedCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand SaveLocalCommand { get; }
        public ICommand RefreshCommand { get; }

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
            SelectedReportType = ReportTypes.FirstOrDefault();

            FillExportTypesCollection();
            UpdateCurrentConfig();

            ReportTypeSelectedCommand = new DelegateCommand(ReportTypeSelectedCommandExecute);

            SaveCommand = new DelegateCommand(SaveCommandExecute);
            SaveLocalCommand = new DelegateCommand(SaveLocalCommandExecute);
            RefreshCommand = new DelegateCommand(RefreshCommandExecute);
        }

        public void SelectReportType(ReportType reportType)
        {
            if (ReportTypes.Contains(reportType))
            {
                SelectedReportType = reportType;
                UpdateCurrentConfig();
            }
            else
            {
                _dialogService.ShowMessage("Невідомий тип звіту.");
            }
        }

        private void ReportTypeSelectedCommandExecute()
        {
            UpdateCurrentConfig();
        }

        protected override void UpdateCurrentConfig(bool withReload = false)
        {
            var reportConfig = _reportDataService.GetReportParameters(SelectedReportType, withReload);

            CurrentConfig = [.. reportConfig ?? []];
        }

        private void FillExportTypesCollection()
        {
            var types = EnumHelper.GetValuesWithDescriptions<ExportType>().Select(x => new EnumItemDataModel<ExportType>(x.Value, x.Description));
            ExportTypes = [.. types];
        }

        private void SaveCommandExecute()
        {
            _reportDataService.SaveReportConfig([.. CurrentConfig], SelectedReportType);
        }

        private void SaveLocalCommandExecute()
        {
            _reportDataService.SaveReportConfigTemprorary([.. CurrentConfig], SelectedReportType);
        }

        private void RefreshCommandExecute()
        {
            var result = _dialogService.ShowMessage("Ви впевнені що бажаєте перезавантажити таблицю?", "Підтвердження", DialogButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                UpdateCurrentConfig(withReload: true);
            }
        }
    }
}
