using Mil.Paperwork.Domain.Services;
using Mil.Paperwork.Infrastructure.DataModels.Configuration;
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
    internal class CommissionsConfigViewModel : ConfigViewModel
    {
        private readonly IReportDataService _reportDataService;
        private CommissionType _selectedCommissionType;
        private ObservableCollection<CommissionerDTO> _currentCommission;
        private string _commissionName;
        private string _commissionDescription;

        public ObservableCollection<CommissionType> CommissionTypes { get; }
        public ObservableCollection<EnumItemDataModel<ExportType>> ExportTypes { get; private set; }

        public CommissionType SelectedCommissionType
        {
            get => _selectedCommissionType;
            set => SetProperty(ref _selectedCommissionType, value);
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

        public ICommand CommissionTypeSelectedCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand SaveLocalCommand { get; }
        public ICommand RefreshCommand { get; }

        public override string Header => "Налаштування комісій";

        protected override string ExportFileTitle => SelectedCommissionType.GetDescription();

        public CommissionsConfigViewModel(
            IReportDataService reportDataService,
            IExportService exportService,
            IImportService importService) : base(exportService, importService)
        {
            _reportDataService = reportDataService;

            CommissionTypes = [.. EnumHelper.GetValues<CommissionType>()];
            SelectedCommissionType = CommissionTypes.FirstOrDefault();

            FillExportTypesCollection();
            UpdateCurrentConfig();

            CommissionTypeSelectedCommand = new DelegateCommand(CommissionTypeSelectedCommandExecute);

            SaveCommand = new DelegateCommand(SaveCommandExecute);
            SaveLocalCommand = new DelegateCommand(SaveLocalCommandExecute);
            RefreshCommand = new DelegateCommand(RefreshCommandExecute);
        }

        public void SelectCommissionType(CommissionType commissionType)
        {
            if (CommissionTypes.Contains(commissionType))
            {
                SelectedCommissionType = commissionType;
                UpdateCurrentConfig();
            }
            else
            {
                MessageBox.Show("Невідомий тип звіту.");
            }
        }

        private void CommissionTypeSelectedCommandExecute()
        {
            UpdateCurrentConfig();
        }

        protected override void UpdateCurrentConfig(bool withReload = false)
        {
            var commissionDTO = _reportDataService.GetCommissionData(SelectedCommissionType, withReload);

            CommissionDescription = commissionDTO.Description;
            CommissionName = commissionDTO.Name;

            CurrentCommission = [.. commissionDTO?.Squad ?? []];
        }

        private void FillExportTypesCollection()
        {
            var types = EnumHelper.GetValuesWithDescriptions<ExportType>().Select(x => new EnumItemDataModel<ExportType>(x.Value, x.Description));
            ExportTypes = [.. types];
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

        private void RefreshCommandExecute()
        {
            var result = MessageBox.Show("Ви впевнені що бажаєте перезавантажити таблицю?", "Підтвердження", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                UpdateCurrentConfig(withReload: true);
            }
        }
    }
}
