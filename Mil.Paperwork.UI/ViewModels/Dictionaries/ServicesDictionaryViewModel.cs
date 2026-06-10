using Mil.MVVM.Common;
using Mil.Paperwork.Infrastructure.DataModels;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Helpers;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.UI.ViewModels.Tabs;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Mil.Paperwork.UI.ViewModels.Dictionaries
{
    internal class ServicesDictionaryViewModel : SettingsTabViewModel
    {
        private const string RemoveConfirmation = "Ви впевнені що бажаєте видалити цю службу?";
        private const string SetDefaultConfirmation = "Ви впевнені що бажаєте встановити цю службу за замовчуванням?";
        private const string RefreshConfirmation = "Ви впевнені що бажаєте перезавантажити дані?";
        private const string ConfirmationTitle = "Підтвердження";

        private readonly IReportDataService _reportDataService;
        private readonly IDataService _dataService;
        private readonly IDialogService _dialogService;

        private MilitaryServiceViewModel _selectedService;
        private MilitaryServiceViewModel _defaultServiceSelection;
        private PersonViewModel _selectedHeadPerson;
        private string _defaultServiceKey;
        private bool _suppressHeadPersonSync;

        public ObservableCollection<MilitaryServiceViewModel> Services { get; private set; }
        public ObservableCollection<AssetType> AssetTypes { get; }
        public ObservableCollection<PersonViewModel> People { get; private set; }

        public MilitaryServiceViewModel SelectedService
        {
            get => _selectedService;
            set
            {
                if (_selectedService != null)
                {
                    _selectedService.PropertyChanged -= OnSelectedServicePropertyChanged;
                }

                if (SetProperty(ref _selectedService, value))
                {
                    if (_selectedService != null)
                    {
                        _selectedService.PropertyChanged += OnSelectedServicePropertyChanged;
                    }

                    SaveCommand.RaiseCanExecuteChanged();
                    SaveLocalCommand.RaiseCanExecuteChanged();
                    RemoveItemCommand.RaiseCanExecuteChanged();
                    SetDefaultCommand.RaiseCanExecuteChanged();

                    UpdateSelectedHeadPerson();
                }
            }
        }

        public MilitaryServiceViewModel DefaultServiceSelection
        {
            get => _defaultServiceSelection;
            set
            {
                if (SetProperty(ref _defaultServiceSelection, value))
                {
                    SetDefaultCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public PersonViewModel SelectedHeadPerson
        {
            get => _selectedHeadPerson;
            set
            {
                if (SetProperty(ref _selectedHeadPerson, value)
                    && !_suppressHeadPersonSync
                    && value != null
                    && SelectedService != null)
                {
                    SelectedService.HeadName = value.FullName;
                    SelectedService.HeadRank = value.Rank;
                    SelectedService.HeadPosition = value.Position;
                }
            }
        }

        public override string Header => "Довідник служб";

        public IDelegateCommand AddItemCommand { get; }
        public IDelegateCommand RemoveItemCommand { get; }
        public IDelegateCommand SaveCommand { get; }
        public IDelegateCommand SaveLocalCommand { get; }
        public IDelegateCommand SetDefaultCommand { get; }
        public IDelegateCommand RefreshCommand { get; }

        public ServicesDictionaryViewModel(
            IReportDataService reportDataService,
            IDataService dataService,
            IDialogService dialogService)
            : base(dialogService)
        {
            _reportDataService = reportDataService;
            _dataService = dataService;
            _dialogService = dialogService;

            AssetTypes = [.. EnumHelper.GetValues<AssetType>()];
            People = [];

            AddItemCommand = new DelegateCommand(AddItemCommandExecute);
            RemoveItemCommand = new DelegateCommand(RemoveItemCommandExecute, RemoveItemCanExecute);
            SaveCommand = new DelegateCommand(SaveCommandExecute, SaveCanExecute);
            SaveLocalCommand = new DelegateCommand(SaveLocalCommandExecute, SaveCanExecute);
            SetDefaultCommand = new DelegateCommand(SetDefaultCommandExecute, SetDefaultCanExecute);
            RefreshCommand = new DelegateCommand(RefreshCommandExecute);

            LoadServicesData();
        }

        private void LoadServicesData(bool withReload = false)
        {
            var services = _reportDataService.GetAllServices(withReload);
            _defaultServiceKey = _reportDataService.GetSelectedService(withReload);

            var viewModels = services
                .Select(x =>
                {
                    var vm = new MilitaryServiceViewModel(x.Key, x.Value);
                    vm.SetAsDefault(x.Key == _defaultServiceKey);
                    return vm;
                })
                .OrderByDescending(vm => vm.IsMarkedAsDefault)
                .ToList();

            Services = new ObservableCollection<MilitaryServiceViewModel>(viewModels);
            OnPropertyChanged(nameof(Services));

            ReloadPeople();

            var defaultVm = Services.FirstOrDefault(vm => vm.IsMarkedAsDefault);
            DefaultServiceSelection = defaultVm;
            SelectedService = defaultVm ?? Services.FirstOrDefault();
        }

        private void ReloadPeople()
        {
            var updated = _dataService.LoadPeopleData()
                .Select(p => new PersonViewModel(p))
                .ToList();

            People.Clear();

            foreach (var person in updated)
            {
                People.Add(person);
            }
        }

        private void UpdateSelectedHeadPerson()
        {
            _suppressHeadPersonSync = true;

            if (SelectedService == null)
            {
                SelectedHeadPerson = null;
            }
            else
            {
                SelectedHeadPerson = People.FirstOrDefault(p => p.FullName == SelectedService.HeadName);
            }

            _suppressHeadPersonSync = false;
        }

        private void OnSelectedServicePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MilitaryServiceViewModel.AssetType))
            {
                SaveCommand.RaiseCanExecuteChanged();
                SaveLocalCommand.RaiseCanExecuteChanged();
            }
        }

        private void AddItemCommandExecute()
        {
            var key = System.Guid.NewGuid().ToString();
            var newService = new MilitaryServiceViewModel(key);
            Services.Add(newService);
            SelectedService = newService;
        }

        private async void RemoveItemCommandExecute()
        {
            if (SelectedService == null)
            {
                return;
            }

            var dialogResult = await _dialogService.ShowMessageAsync(RemoveConfirmation, ConfirmationTitle, DialogButtons.YesNo);

            if (dialogResult != DialogResult.Yes)
            {
                return;
            }

            var isDefaultRemoved = false;

            if (SelectedService.IsSaved)
            {
                var key = SelectedService.ServiceKey;
                isDefaultRemoved = key == _defaultServiceKey;
                _reportDataService.DeleteServiceData(key);
            }

            Services.Remove(SelectedService);

            if (isDefaultRemoved)
            {
                LoadServicesData(withReload: true);
            }
            else
            {
                var next = Services.FirstOrDefault(vm => vm.IsMarkedAsDefault) ?? Services.FirstOrDefault();
                SelectedService = next;
            }
        }

        private bool RemoveItemCanExecute()
        {
            var result = SelectedService != null
                && (!SelectedService.IsSaved || Services.Count(x => x.IsSaved) > 1);
            return result;
        }

        private void SaveCommandExecute()
        {
            SaveService(temporary: false);
        }

        private void SaveLocalCommandExecute()
        {
            SaveService(temporary: true);
        }

        private bool SaveCanExecute()
        {
            var result = SelectedService != null && SelectedService.AssetType != AssetType.Default;
            return result;
        }

        private void SaveService(bool temporary)
        {
            if (SelectedService == null)
            {
                return;
            }

            var key = SelectedService.ServiceKey;
            var dto = SelectedService.GetDTO();
            _reportDataService.SaveServiceData(key, dto, temporary);

            if (!temporary)
            {
                SelectedService.SetAsSaved();

                if (!string.IsNullOrEmpty(SelectedService.HeadName))
                {
                    var personDto = new PersonDTO(
                        SelectedService.HeadName,
                        SelectedService.HeadPosition,
                        SelectedService.HeadRank);

                    _dataService.AlterPeople([personDto]);
                    ReloadPeople();
                }
            }
        }

        private async void SetDefaultCommandExecute()
        {
            if (DefaultServiceSelection == null)
            {
                return;
            }

            var dialogResult = await _dialogService.ShowMessageAsync(
                SetDefaultConfirmation, ConfirmationTitle, DialogButtons.YesNo);

            if (dialogResult != DialogResult.Yes)
            {
                return;
            }

            SaveService(temporary: false);

            _reportDataService.SetDefaultService(DefaultServiceSelection.ServiceKey);
            _defaultServiceKey = DefaultServiceSelection.ServiceKey;

            foreach (var vm in Services)
            {
                vm.SetAsDefault(vm.ServiceKey == _defaultServiceKey);
            }

            var selectedKey = SelectedService?.ServiceKey;

            var sorted = Services
                .OrderByDescending(vm => vm.IsMarkedAsDefault)
                .ToList();

            Services.Clear();

            foreach (var vm in sorted)
            {
                Services.Add(vm);
            }

            SelectedService = Services.FirstOrDefault(vm => vm.ServiceKey == selectedKey);
            DefaultServiceSelection = Services.FirstOrDefault(vm => vm.IsMarkedAsDefault);
        }

        private bool SetDefaultCanExecute()
        {
            var result = DefaultServiceSelection != null
                && DefaultServiceSelection.ServiceKey != _defaultServiceKey;
            return result;
        }

        private async void RefreshCommandExecute()
        {
            var dialogResult = await _dialogService.ShowMessageAsync(
                RefreshConfirmation, ConfirmationTitle, DialogButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                LoadServicesData(withReload: true);
            }
        }
    }
}
