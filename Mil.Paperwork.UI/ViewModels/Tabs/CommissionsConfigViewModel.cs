using Mil.Paperwork.Common.Enums;
using Mil.Paperwork.Common.MVVM;
using Mil.Paperwork.Domain.Services;
using Mil.Paperwork.Infrastructure.DataModels.Configuration;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Helpers;
using Mil.Paperwork.Infrastructure.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Mil.Paperwork.UI.ViewModels.Tabs
{
    internal class CommissionsConfigViewModel : ConfigViewModel
    {
        private readonly IReportDataService _reportDataService;
        private readonly IDialogService _dialogService;
        private CommissionType _selectedCommissionType;
        private ObservableCollection<CommissionerDTO> _currentCommission;
        private string _commissionName;
        private string _commissionDescription;

        public ObservableCollection<CommissionType> CommissionTypes { get; }
        public ObservableCollection<ExportType> ExportTypes { get; private set; }

        public CommissionType SelectedCommissionType
        {
            get => _selectedCommissionType;
            set
            {
                if (SetProperty(ref _selectedCommissionType, value))
                {
                    UpdateCurrentConfig();
                }
            }
        }

        public override List<object> ExportData => [.. CurrentCommission];

        public ObservableCollection<CommissionerDTO> CurrentCommission
        {
            get => _currentCommission;
            set => SetProperty(ref _currentCommission, value);
        }

        public string CommissionName
        {
            get => _commissionName;
            set => SetProperty(ref _commissionName, value);
        }

        public string CommissionDescription
        {
            get => _commissionDescription;
            set => SetProperty(ref _commissionDescription, value);
        }

        public IDelegateCommand SaveCommand { get; }
        public IDelegateCommand SaveLocalCommand { get; }
        public IDelegateCommand RefreshCommand { get; }

        public override string Header => "Налаштування комісій";

        protected override string ExportFileTitle => SelectedCommissionType.GetDescription();

        public CommissionsConfigViewModel(
            IReportDataService reportDataService,
            IExportService exportService,
            IImportService importService,
            IDialogService dialogService) : base(exportService, importService, dialogService)
        {
            _reportDataService = reportDataService;
            _dialogService = dialogService;

            CommissionTypes = [.. EnumHelper.GetValues<CommissionType>()];
            ExportTypes = [.. EnumHelper.GetValues<ExportType>()];
            SelectedCommissionType = CommissionTypes.FirstOrDefault();

            UpdateCurrentConfig();

            SaveCommand = new DelegateCommand(SaveCommandExecute);
            SaveLocalCommand = new DelegateCommand(SaveLocalCommandExecute);
            RefreshCommand = new DelegateCommand(RefreshCommandExecute);
        }

        public async void SelectCommissionType(CommissionType commissionType)
        {
            if (CommissionTypes.Contains(commissionType))
            {
                SelectedCommissionType = commissionType;
                UpdateCurrentConfig();
            }
            else
            {
                await _dialogService.ShowMessageAsync("Невідомий тип звіту.");
            }
        }

        protected override void UpdateCurrentConfig(bool withReload = false)
        {
            var commissionDTO = _reportDataService.GetCommissionData(SelectedCommissionType, withReload);

            CommissionDescription = commissionDTO.Description;
            CommissionName = commissionDTO.Name;

            CurrentCommission = [.. commissionDTO?.Squad ?? []];
        }

        private void SaveCommandExecute()
        {
            var commission = GetCurrentCommissionDTO();
            _reportDataService.SaveCommission(commission, SelectedCommissionType);
        }

        private void SaveLocalCommandExecute()
        {
            var commission = GetCurrentCommissionDTO();
            _reportDataService.SaveCommissionTemporary(commission, SelectedCommissionType);
        }

        private CommissionDTO GetCurrentCommissionDTO()
        {
            return new CommissionDTO()
            {
                Name = CommissionName,
                Description = CommissionDescription,
                Squad = [.. CurrentCommission]
            };
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
