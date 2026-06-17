using Avalonia.Threading;
using Mil.MVVM.Common;
using Mil.Paperwork.Infrastructure.DataModels;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Helpers;
using Mil.Paperwork.DataAccess.Services;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.UI.Helpers;
using Mil.Paperwork.UI.ViewModels.Ribbon;
using Mil.Paperwork.UI.ViewModels.Tabs;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Mil.Paperwork.UI.ViewModels.Dictionaries
{
    internal class ServicesDictionaryViewModel : SettingsTabViewModel, ISilentRefreshable
    {
        private const string RemoveConfirmation = "Ви впевнені що бажаєте видалити цю службу?";
        private const string SetDefaultConfirmation = "Ви впевнені що бажаєте встановити цю службу за замовчуванням?";
        private const string RefreshConfirmation = "Ви впевнені що бажаєте перезавантажити дані?";
        private const string ConfirmationTitle = "Підтвердження";

        private const string GroupRecords = "Записи";
        private const string GroupData = "Дані";
        private const string GroupService = "Служба";

        private const string CaptionAdd = "Додати";
        private const string CaptionRemove = "Видалити";
        private const string CaptionSave = "Зберегти";
        private const string CaptionSaveLocal = "Зберегти тимч.";
        private const string CaptionRefresh = "Оновити";
        private const string CaptionSetDefault = "За замовч.";

        private const string AutomationIdAdd = "ServicesDictionary_AddAction";
        private const string AutomationIdRemove = "ServicesDictionary_RemoveAction";
        private const string AutomationIdSave = "ServicesDictionary_SaveAction";
        private const string AutomationIdSaveLocal = "ServicesDictionary_SaveLocalAction";
        private const string AutomationIdRefresh = "ServicesDictionary_RefreshAction";
        private const string AutomationIdSetDefault = "ServicesDictionary_SetDefaultAction";

        private readonly IReportDataService _reportDataService;
        private readonly IDataService _dataService;
        private readonly IDialogService _dialogService;

        private MilitaryServiceViewModel _selectedService;
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

        protected override IList<RibbonGroupViewModel> BuildRibbonGroups()
        {
            var groups = new List<RibbonGroupViewModel>
            {
                new RibbonGroupViewModel(GroupRecords, new[]
                {
                    new RibbonActionViewModel(CaptionAdd, IconKeys.Add, AddItemCommand, AutomationIdAdd),
                    new RibbonActionViewModel(CaptionRemove, IconKeys.Remove, RemoveItemCommand, AutomationIdRemove, isDestructive: true)
                }),
                new RibbonGroupViewModel(GroupData, new[]
                {
                    new RibbonActionViewModel(CaptionSave, IconKeys.Save, SaveCommand, AutomationIdSave),
                    new RibbonActionViewModel(CaptionSaveLocal, IconKeys.SaveLocal, SaveLocalCommand, AutomationIdSaveLocal),
                    new RibbonActionViewModel(CaptionRefresh, IconKeys.Refresh, RefreshCommand, AutomationIdRefresh)
                }),
                new RibbonGroupViewModel(GroupService, new[]
                {
                    new RibbonActionViewModel(CaptionSetDefault, IconKeys.SetDefault, SetDefaultCommand, AutomationIdSetDefault)
                })
            };
            return groups;
        }

        public void SilentRefresh()
        {
            LoadServicesData(withReload: true);
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
            var toSelect = defaultVm ?? Services.FirstOrDefault();
            Dispatcher.UIThread.Post(() => SelectedService = toSelect);
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
            if (SelectedService == null)
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

            _reportDataService.SetDefaultService(SelectedService.ServiceKey);
            _defaultServiceKey = SelectedService.ServiceKey;

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
        }

        private bool SetDefaultCanExecute()
        {
            var result = SelectedService != null
                && SelectedService.ServiceKey != _defaultServiceKey;
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
