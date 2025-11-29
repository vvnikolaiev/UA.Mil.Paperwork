using Mil.Paperwork.Common.Enums;
using Mil.Paperwork.Common.MVVM;
using Mil.Paperwork.Domain.Services;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.UI.ViewModels.Tabs;
using Mil.Paperwork.UI.Windows;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Mil.Paperwork.UI.ViewModels.Dictionaries
{
    internal class PeopleDictionaryViewModel : ISettingsTabViewModel
    {
        private readonly IDataService _dataService;
        private readonly IImportService _importService;
        private readonly IDialogService _dialogService;

        public ObservableCollection<PersonViewModel> People { get; }
        public string Header => "Довідник осіб";
        public bool IsClosed { get; private set; }

        public event EventHandler<ITabViewModel> TabCloseRequested;

        public IDelegateCommand AddItemCommand { get; }
        public IDelegateCommand<PersonViewModel> RemoveItemCommand { get; }
        public IDelegateCommand SaveCommand { get; }
        public IDelegateCommand RefreshCommand { get; }
        public IDelegateCommand ImportCommand { get; }
        public IDelegateCommand CloseCommand { get; }

        public PeopleDictionaryViewModel(
            IDataService dataService, 
            IImportService importService,
            IDialogService dialogService)
        {
            _dataService = dataService;
            _importService = importService;
            _dialogService = dialogService;

            People = [.. GetPeopleData()];

            AddItemCommand = new DelegateCommand(AddItemCommandExecute);
            RemoveItemCommand = new DelegateCommand<PersonViewModel>(RemoveItemCommandExecute);
            SaveCommand = new DelegateCommand(SaveCommandExecute);
            RefreshCommand = new DelegateCommand(RefreshCommandExecute);
            ImportCommand = new DelegateCommand(ImportCommandExecute);
            CloseCommand = new DelegateCommand(CloseCommandExecute);
        }


        private PersonViewModel[] GetPeopleData()
        {
            var people = _dataService.LoadPeopleData();
            var productViewModels = people.Select(x => new PersonViewModel(x));
            return productViewModels.ToArray();
        }

        private void AddItemCommandExecute()
        {
            var person = new PersonViewModel();
            People.Add(person);
        }

        private void RemoveItemCommandExecute(PersonViewModel person)
        {
            if (person != null && People.Contains(person))
            {
                People.Remove(person);
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

        private async void CloseCommandExecute()
        {
            if (await _dialogService.ShowMessageAsync("Close tab?", "Confirmation", DialogButtons.YesNo) == DialogResult.Yes)
            {
                TabCloseRequested?.Invoke(this, this);
                IsClosed = true;
            }
        }
    }
}