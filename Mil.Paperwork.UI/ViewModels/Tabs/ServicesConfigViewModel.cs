using Mil.Paperwork.Common.MVVM;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Helpers;
using Mil.Paperwork.Infrastructure.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Mil.Paperwork.UI.ViewModels.Tabs
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
            set
            {
                if (SetProperty(ref _currentService, value))
                {
                    SelectService(_currentService);
                }
            }
        }

        public string Header => "Налаштування служб";

        public bool IsClosed { get; private set; }

        public event EventHandler<ITabViewModel> TabCloseRequested;

        public IDelegateCommand AddNewServiceCommand { get; }
        public IDelegateCommand MarkAsDefaultCommand { get; }
        public IDelegateCommand DeleteServiceCommand { get; }
        public IDelegateCommand SaveCommand { get; }
        public IDelegateCommand SaveLocalCommand { get; }
        public IDelegateCommand RefreshCommand { get; }
        public IDelegateCommand CloseCommand { get; }


        public ServicesConfigViewModel(
            IReportDataService reportDataService,
            IDialogService dialogService)
        {
            _reportDataService = reportDataService;
            _dialogService = dialogService;

            AddNewServiceCommand = new DelegateCommand(AddNewServiceCommandExecute);
            MarkAsDefaultCommand = new DelegateCommand(MarkAsDefaultCommandExecute, MarkAsDefaultCanExecute);
            DeleteServiceCommand = new DelegateCommand(DeleteServiceCommandExecute, DeleteServiceCanExecute);
            SaveCommand = new DelegateCommand(SaveCommandExecute);
            SaveLocalCommand = new DelegateCommand(SaveLocalCommandExecute);
            RefreshCommand = new DelegateCommand(RefreshCommandExecute);
            CloseCommand = new DelegateCommand(CloseCommandExecute);

            AssetTypes = [.. EnumHelper.GetValues<AssetType>()];

            LoadServicesData();
        }

        public void SelectService(MilitaryServiceViewModel serviceViewModel)
        {
            if (serviceViewModel != null)
            {
                CurrentService = serviceViewModel;
            }

            DeleteServiceCommand?.RaiseCanExecuteChanged();
            MarkAsDefaultCommand?.RaiseCanExecuteChanged();
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

        private void AddNewServiceCommandExecute()
        {
            var key = Guid.NewGuid().ToString();
            var newService = new MilitaryServiceViewModel(key);

            Services.Add(newService);

            SelectService(newService);
        }

        private async void MarkAsDefaultCommandExecute()
        {
            var result = await _dialogService.ShowMessageAsync("Ви впевнені що бажаєте встановити цю службу за замовчуванням?", "Підтвердження", DialogButtons.YesNo);
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

        private async void DeleteServiceCommandExecute()
        {
            var result = await _dialogService.ShowMessageAsync("Ви впевнені що бажаєте видалити цю службу?", "Підтвердження", DialogButtons.YesNo);
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
            var result = CurrentService != null && !CurrentService.IsSaved || Services.Count(x => x.IsSaved) > 1;
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

        private async void RefreshCommandExecute()
        {
            var result = await _dialogService.ShowMessageAsync("Ви впевнені що бажаєте перезавантажити дані?", "Підтвердження", DialogButtons.YesNo);
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
