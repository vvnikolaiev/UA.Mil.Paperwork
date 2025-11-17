using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Helpers;
using Mil.Paperwork.Common.MVVM;
using Mil.Paperwork.Infrastructure.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Mil.Paperwork.WriteOff.ViewModels.Tabs
{
    internal class ServicesConfigViewModel : ObservableItem, ISettingsTabViewModel
    {
        private readonly IReportDataService _reportDataService;
        private readonly IDialogService _dialogService;
        private ObservableCollection<MilitaryServiceViewModel> _services;
        private MilitaryServiceViewModel _currentService;
        private string _defaultServiceKey;

        public ObservableCollection<MilitaryServiceViewModel> Services
        {
            get => _services;
            set => SetProperty(ref _services, value);
        }

        public ObservableCollection<AssetType> AssetTypes { get; }

        public MilitaryServiceViewModel CurrentService
        {
            get => _currentService;
            set => SetProperty(ref _currentService, value);
        }

        public string Header => "Налаштування служб";

        public bool IsClosed { get; private set; }

        public event EventHandler<ITabViewModel> TabCloseRequested;

        public ICommand ServiceSelectedCommand { get; }
        public ICommand AddNewServiceCommand { get; }
        public ICommand MarkAsDefaultCommand { get; }
        public ICommand DeleteServiceCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand SaveLocalCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand CloseCommand { get; }


        public ServicesConfigViewModel(
            IReportDataService reportDataService,
            IDialogService dialogService)
        {
            _reportDataService = reportDataService;
            _dialogService = dialogService;

            AssetTypes = [.. EnumHelper.GetValues<AssetType>()];

            LoadServicesData();

            ServiceSelectedCommand = new DelegateCommand(ServiceSelectedCommandExecute);

            AddNewServiceCommand = new DelegateCommand(AddNewServiceCommandExecute);
            MarkAsDefaultCommand = new DelegateCommand(MarkAsDefaultCommandExecute, MarkAsDefaultCanExecute);
            DeleteServiceCommand = new DelegateCommand(DeleteServiceCommandExecute, DeleteServiceCanExecute);
            SaveCommand = new DelegateCommand(SaveCommandExecute);
            SaveLocalCommand = new DelegateCommand(SaveLocalCommandExecute);
            RefreshCommand = new DelegateCommand(RefreshCommandExecute);
            CloseCommand = new DelegateCommand(CloseCommandExecute);
        }

        public void SelectService(MilitaryServiceViewModel serviceViewModel)
        {
            if (serviceViewModel != null)
            {
                CurrentService = serviceViewModel;
            }
        }

        private void LoadServicesData(bool withReload = false)
        {
            var services = _reportDataService.GetAllServices(withReload);
            var viewModels = services.Select(x => new MilitaryServiceViewModel(x.Key, x.Value));

            Services = [.. viewModels];

            SelectDefaultService();
        }

        private void SelectDefaultService(bool withReload = true)
        {
            if (withReload)
            {
                UpdateDefaultServiceKey();
            }

            var selectedService = Services.FirstOrDefault(x => x.IsMarkedAsDefault);
            SelectService(selectedService);
        }

        private void UpdateDefaultServiceKey()
        {
            _defaultServiceKey = _reportDataService.GetSelectedService();
            UpdateServicesDefaultFlags();
        }

        private void UpdateServicesDefaultFlags()
        {
            if (Services == null) return;
            foreach (var s in Services)
            {
                s.SetAsDefault(s.ServiceKey == _defaultServiceKey);
            }
        }

        private void ServiceSelectedCommandExecute()
        {
            SelectService(_currentService);
        }

        private void AddNewServiceCommandExecute()
        {
            var key = Guid.NewGuid().ToString();
            var newService = new MilitaryServiceViewModel(key);

            Services.Add(newService);

            SelectService(newService);
        }

        private void MarkAsDefaultCommandExecute()
        {
            var result = _dialogService.ShowMessage("Ви впевнені що бажаєте встановити цю службу за замовчуванням?", "Підтвердження", DialogButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                SaveService(CurrentService, false);

                _reportDataService.SetDefaultService(CurrentService.ServiceKey);

                UpdateDefaultServiceKey();
            }
        }

        private bool MarkAsDefaultCanExecute()
        {
            return !CurrentService?.IsMarkedAsDefault ?? false;
        }

        private void DeleteServiceCommandExecute()
        {
            var result = _dialogService.ShowMessage("Ви впевнені що бажаєте видалити цю службу?", "Підтвердження", DialogButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                var isDefaultRemoved = false;
                if (CurrentService.IsSaved)
                {
                    var key = CurrentService.ServiceKey;
                    isDefaultRemoved = key == _defaultServiceKey;

                    _reportDataService.DeleteServiceData(key);
                }

                Services.Remove(CurrentService);

                SelectDefaultService(withReload: isDefaultRemoved);
            }
        }

        private bool DeleteServiceCanExecute()
        {
            var result = !CurrentService.IsSaved || Services.Count(x => x.IsSaved) > 1;
            return result;
        }

        private void SaveCommandExecute()
        {
            SaveService(CurrentService, false);
        }

        private void SaveLocalCommandExecute()
        {
            SaveService(CurrentService, true);
        }

        private void SaveService(MilitaryServiceViewModel serviceViewModel, bool isTemporary)
        {
            var key = serviceViewModel.ServiceKey;
            var dto = serviceViewModel.GetDTO();

            _reportDataService.SaveServiceData(key, dto, isTemporary);

            if (!isTemporary)
            {
                CurrentService.SetAsSaved();
            }
        }

        private void RefreshCommandExecute()
        {
            var result = _dialogService.ShowMessage("Ви впевнені що бажаєте перезавантажити дані?", "Підтвердження", DialogButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                LoadServicesData(withReload: true);
            }
        }

        private void CloseCommandExecute()
        {
            IsClosed = true;
            TabCloseRequested?.Invoke(this, this);
        }
    }
}
