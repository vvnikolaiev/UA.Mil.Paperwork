using Mil.Paperwork.Common.Enums;
using Mil.MVVM.Common;
using Mil.Paperwork.Domain.Services;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.DataAccess.Services;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.UI.ViewModels.Controls;
using Mil.Paperwork.UI.ViewModels.Ribbon;
using Mil.Paperwork.UI.ViewModels.Tabs;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Mil.Paperwork.UI.ViewModels.Dictionaries
{
    internal class PeopleDictionaryViewModel : SettingsTabViewModel, ISilentRefreshable
    {
        private const string GroupRecords = "Записи";
        private const string GroupData = "Дані";
        private const string GroupExchange = "Обмін";

        private const string CaptionAdd = "Додати";
        private const string CaptionRemove = "Видалити";
        private const string CaptionSave = "Зберегти";
        private const string CaptionRefresh = "Оновити";
        private const string CaptionImport = "Імпорт";

        private const string IconKeyAdd = "IconAdd";
        private const string IconKeyRemove = "IconDelete";
        private const string IconKeySave = "IconSaveDraft";
        private const string IconKeyRefresh = "IconRefresh";
        private const string IconKeyImport = "IconImport";

        private readonly IDataService _dataService;
        private readonly IImportService _importService;
        private readonly IDialogService _dialogService;

        private PersonViewModel _selectedPerson;

        public ObservableCollection<PersonViewModel> People { get; }
        public override string Header => "Довідник осіб";

        public PersonViewModel SelectedPerson
        {
            get => _selectedPerson;
            set => SetProperty(ref _selectedPerson, value);
        }

        public IDelegateCommand AddItemCommand { get; }
        public IDelegateCommand RemoveItemCommand { get; }
        public IDelegateCommand SaveCommand { get; }
        public IDelegateCommand RefreshCommand { get; }
        public IDelegateCommand ImportCommand { get; }

        public PeopleDictionaryViewModel(
            IDataService dataService,
            IImportService importService,
            IDialogService dialogService) : base(dialogService)
        {
            _dataService = dataService;
            _importService = importService;
            _dialogService = dialogService;

            People = [.. GetPeopleData()];

            AddItemCommand = new DelegateCommand(AddItemCommandExecute);
            RemoveItemCommand = new DelegateCommand(RemoveItemCommandExecute);
            SaveCommand = new DelegateCommand(SaveCommandExecute);
            RefreshCommand = new DelegateCommand(RefreshCommandExecute);
            ImportCommand = new DelegateCommand(ImportCommandExecute);
        }

        protected override IList<RibbonGroupViewModel> BuildRibbonGroups()
        {
            var groups = new List<RibbonGroupViewModel>
            {
                new RibbonGroupViewModel(GroupRecords, new[]
                {
                    new RibbonActionViewModel(CaptionAdd, IconKeyAdd, AddItemCommand),
                    new RibbonActionViewModel(CaptionRemove, IconKeyRemove, RemoveItemCommand, isDestructive: true)
                }),
                new RibbonGroupViewModel(GroupData, new[]
                {
                    new RibbonActionViewModel(CaptionSave, IconKeySave, SaveCommand),
                    new RibbonActionViewModel(CaptionRefresh, IconKeyRefresh, RefreshCommand)
                }),
                new RibbonGroupViewModel(GroupExchange, new[]
                {
                    new RibbonActionViewModel(CaptionImport, IconKeyImport, ImportCommand)
                })
            };
            return groups;
        }

        public void SilentRefresh()
        {
            RefreshCommandExecute();
        }

        private PersonViewModel[] GetPeopleData()
        {
            var people = _dataService.LoadPeopleData();
            var productViewModels = people.Select(x => new PersonViewModel(x));
            var result = productViewModels.ToArray();
            return result;
        }

        private void AddItemCommandExecute()
        {
            var person = new PersonViewModel();
            People.Add(person);
        }

        private void RemoveItemCommandExecute()
        {
            if (SelectedPerson != null && People.Contains(SelectedPerson))
            {
                People.Remove(SelectedPerson);
            }
        }

        private void SaveCommandExecute()
        {
            var dtos = People.Select(vm => vm.ToDTO()).ToArray();
            _dataService.SavePeopleData(dtos);
        }

        private void RefreshCommandExecute()
        {
            var people = GetPeopleData();
            People.Clear();
            foreach (var person in people)
            {
                People.Add(person);
            }
        }

        private async void ImportCommandExecute()
        {
            var importViewModel = new ImportViewModel(_importService, _dataService, _dialogService);
            importViewModel.SetImportType(ImportType.People);

            await _dialogService.OpenImportWindow(importViewModel);

            if (importViewModel.IsValid)
            {
                RefreshCommandExecute();
            }
        }
    }
}